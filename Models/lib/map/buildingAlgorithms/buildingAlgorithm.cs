using System;
using System.Collections.Generic;
using Troschuetz.Random;
using VirusSimulatorAvalonia.Models.defs;
using VirusSimulatorAvalonia.Models.lib.common;
using VirusSimulatorAvalonia.Models.things.inanimates.paths;
using VirusSimulatorAvalonia.Models.things.inanimates.paths.street;
using VirusSimulatorAvalonia.Models.lib.map;

namespace VirusSimulatorAvalonia.Models.lib.map.buildingAlgorithms {
  
  public sealed class BuildingAlgorithm : BuildingMapAlgorithm {
    
    static TRandom random = new TRandom();
    static readonly uint minBuildingLength = 2;
    private Path[,] pathMap;
    private Dictionary<Street,List<uint[][]>> map;
    private bool[,] occupiedTerrain;
    private uint[] buildingNorthwestCoords = new uint[2];
    private uint[] buildingSoutheastCoords = new uint[2];
    private uint[] buildingDoorCoords = new uint[2];
    private uint buildingHeight;
    private uint buildingWidth;

    public Dictionary<Street,List<uint[][]>> run( Path[,] pathMap) {
      this.map = new Dictionary<Street,List<uint[][]>>();
      this.occupiedTerrain = new bool[pathMap.GetLength( 0),
        pathMap.GetLength( 1)];
      this.pathMap = pathMap;
      findTerrainsFrom( 0, 0);
      return this.map;
    }

    private void findTerrainsFrom( uint startX, uint startY) {
      if (startY == this.pathMap.GetLength( 0)) return;
      if (startX == this.pathMap.GetLength( 1)) findTerrainsFrom( 0, 
        startY + 1);
      while (this.pathMap[startY,startX] != null && this.
        occupiedTerrain[startY,startX]) 
        startX++;
      uint endX = getXupperBoundFromAt( startX, startY);
      uint endY = getYupperBoundFromWith( startX, startY, endX);
      buildOnTerrain( startX, startY, endX, endY);
      findTerrainsFrom( endX + 1, startY);
    }

    private uint getYupperBoundFromWith( uint startX, uint startY, 
      uint maxX) {
      uint endY = startY, x;
      do {
        endY++;
        x = startX;
        while (x < this.pathMap.GetUpperBound( 1) && 
          this.pathMap[endY,x + 1] == null) 
          x++;
      } while (x >= maxX);
      return endY;
    }

    private uint getXupperBoundFromAt( uint startX, uint y) {
      uint endX = startX;
      while (endX < this.pathMap.GetUpperBound( 1) && 
        this.pathMap[y,endX + 1] == null) 
        endX++;
      return endX;
    }
    
    private void buildOnTerrain( uint startX, uint startY, uint endX, 
      uint endY) {
      if (this.pathMap[startY,Math.Min( endX + 1, 
        pathMap.GetUpperBound( 1))] != null)
        buildFacadeAtWith( endX, startY, Defs.right);
      if (this.pathMap[Math.Min( endY + 1, this.pathMap.GetUpperBound( 0)), 
        endX] != null)
        buildFacadeAtWith( endX, endY, Defs.down);
      if (this.pathMap[endY,Math.Max( 0, startX - 1)] != null)
        buildFacadeAtWith( endX, endY, Defs.left);
      if (this.pathMap[Math.Max( 0, startY - 1),startX] != null)
        buildFacadeAtWith( startX, startY, Defs.up);
    }

    private void buildFacadeAtWith( uint x, uint y, ushort direction) {
      int dx = getNextClockwiseDxOf( direction);
      int dy = getNextClockwiseDyOf( direction);
      uint[] maxDimensions;
      do {
        maxDimensions = getMaxDimensionsFrom( x, y, direction);
        if (maxDimensions[0] >= minBuildingLength && 
          maxDimensions[1] >= minBuildingLength) {
          setBuildingIn( x, y, maxDimensions[0], maxDimensions[1], direction);
          x = x + (uint) (maxDimensions[0]); 
          y = y + (uint) (maxDimensions[0]); 
        }
        else {
          x = (uint) (x + dx);
          y = (uint) (y + dy);
        }
      } while (maxDimensions[0] >= minBuildingLength);
    }

    private void setBuildingIn( uint x, uint y, uint parallelLen, 
      uint perpendicularLen, ushort direction) {
      generateBuildingWith( x, y, parallelLen, perpendicularLen, direction);
      Street street = getStreetForBuildingOrDieFrom( x, y, direction);
      addBuildingToListAt( street);
      fillOccupiedTerrain();
    }

    private void generateBuildingWith( uint x, uint y, 
      uint parallelLength, uint perpendicularLength, ushort direction) {
      if (direction == Defs.left || direction == Defs.right)
        generateRandomBuildingDimensions( perpendicularLength, parallelLength);
      else
        generateRandomBuildingDimensions( parallelLength, perpendicularLength);
      int dx = direction == Defs.right || direction == Defs.down ? -1 : 1;
      int dy = direction == Defs.left || direction == Defs.down ? -1 : 1;
      generateBuildingCoordinatesFromWith( x, y, dx, dy);
    }

    private Street getStreetForBuildingOrDieFrom( uint x, uint y, 
      ushort direction) {
      uint length;
      ushort streetDirection;
      if (direction == Defs.down || direction == Defs.up) {
        length = this.buildingWidth;
        streetDirection = direction == Defs.down ? Defs.left : Defs.right;
      }
      else {
        length = this.buildingHeight;
        streetDirection = direction == Defs.left ? Defs.up : Defs.down;
      }
      Street street = findStreetForBuilding( x, y, length, streetDirection);
      if (street == null)
        throw new Exception( "There is no street for the building!");
      return street;
    }

    // this is will return the lengths with respect of street orientation!
    private uint[] getMaxDimensionsFrom( uint x, uint y, ushort direction) {
      int perpendicularDx = getOppositeDxOf( direction);
      int perpendicularDy = getOppositeDyOf( direction);
      uint perpendicularLength = getLengthFrom( x, y, perpendicularDx, 
        perpendicularDy);
      x = (uint) (x + (perpendicularLength - 1) * perpendicularDx);
      y = (uint) (y + (perpendicularLength - 1) * perpendicularDx);
      int parallelDx = getNextClockwiseDxOf( direction);
      int parallelDy = getNextClockwiseDyOf( direction);
      uint parallelLength = 0, totalParallelLength = 0;
      while (totalParallelLength < minBuildingLength) {
        parallelLength = getLengthFrom( x, y, parallelDx, parallelDy);
        x = (uint) (x + parallelLength * parallelDx);
        y = (uint) (y + parallelLength * parallelDx);
        totalParallelLength += parallelLength;
        if (totalParallelLength >= minBuildingLength) break;
        x = (uint) (x - perpendicularDx);
        y = (uint) (y - perpendicularDx);
        perpendicularLength--;
      }
      return new uint[2] { totalParallelLength, perpendicularLength };
    }

    uint getLengthFrom( uint x, uint y, int dx, int dy) {
      uint length = 0;
      while (isMapCoordinatesFree( x, y)) {
        x = (uint) (x + dx);
        y = (uint) (y + dy);
        length++;
      }
      return length;
    }

    Street findStreetForBuilding( uint startX, uint startY, uint length,
      ushort streetDirection) {
      Dictionary<char,int> streetCoords = getStreetCoordsForBuilding( startX,
        startY, length, streetDirection);
      uint x = (uint) streetCoords.GetValueOrDefault( 'x');
      uint y = (uint) streetCoords.GetValueOrDefault( 'y');
      this.buildingDoorCoords[0] = x;
      this.buildingDoorCoords[1] = y;
      return (Street) this.pathMap[y,x];
    }

    void addBuildingToListAt( Street street) {
      uint[][] coordinates = new uint[][] { this.buildingNorthwestCoords,
        this.buildingSoutheastCoords, this.buildingDoorCoords }; 
      if (this.map.GetValueOrDefault( street) == null) 
        this.map.Add( street, new List<uint[][]>() { coordinates });
      else {
        List<uint[][]> coordinatesList = this.map.GetValueOrDefault( street);
        coordinatesList.Add( coordinates);
      }
    }

    private Dictionary<char,int> getStreetCoordsForBuilding( uint startX,
      uint startY, uint length, ushort streetDirection) {
      int middleX = (int) startX + (int) length / 2 * Common.
        getDxOfDirection( streetDirection);
      int middleY = (int) startY + (int) length / 2 * Common.
        getDyOfDirection( streetDirection);
      int plusSideX = middleX, plusSideY = middleY;
      int minusSideX = middleX, minusSideY = middleY;
      if (length % 2 == 0) {
        minusSideX -= Common.getDxOfDirection( streetDirection);
        minusSideY -= Common.getDyOfDirection( streetDirection);
      }
      return bidirectionalStreetSearch( plusSideX, plusSideY, middleX,
        middleY, length / 2, streetDirection);
    }

    // This is a way I find to get buildings entry points next to 
    //   building center
    private Dictionary<char,int> bidirectionalStreetSearch( int plusSideX,
      int plusSideY, int minusSideX, int minusSideY, uint length, 
      ushort direction) {
      for (uint i = length; i > 0; i--) {
        if (this.pathMap[plusSideY,plusSideX].isMountable) 
          return new Dictionary<char,int> { { 'x', plusSideX }, 
            { 'y', plusSideY } };
        if (this.pathMap[minusSideY,minusSideX].isMountable) 
          return new Dictionary<char,int> { { 'x', minusSideX }, 
            { 'y', minusSideY } };
        plusSideX += Common.getDxOfDirection( direction);
        plusSideY += Common.getDyOfDirection( direction);
        minusSideX -= Common.getDxOfDirection( direction);
        minusSideY -= Common.getDyOfDirection( direction);
      }
      return null;
    }

    private void fillOccupiedTerrain() {
      uint minX = this.buildingNorthwestCoords[0];
      uint minY = this.buildingNorthwestCoords[1];
      uint maxX = this.buildingSoutheastCoords[0];
      uint maxY = this.buildingSoutheastCoords[1];
      for (uint x = minX; x < maxY; x++) 
        for (uint y = minY; y < maxY; y++) 
          this.occupiedTerrain[y,x] = true;
    }

    private void generateBuildingCoordinatesFromWith( uint x, uint y, 
      int dx, int dy) {
      this.buildingNorthwestCoords[0] = dx < 0 ? (uint) (x + 
        dx * this.buildingWidth) : x;
      this.buildingNorthwestCoords[1] = dy < 0 ? (uint) (y + 
        dy) * this.buildingHeight : y;
      this.buildingSoutheastCoords[0] = dx < 0 ? x : (uint) (x +
        dx * this.buildingWidth);
      this.buildingSoutheastCoords[1] = dy < 0 ? y : (uint) (y +
        dy * this.buildingHeight);
    }

    private void generateRandomBuildingDimensions( 
      uint maxWidth, uint maxHeight) {
      uint height = 0, width = 0;
      height = Math.Max( minBuildingLength, Math.Min( (uint) Math.Ceiling( 
        random.Beta( 2.5d, 4.0d) * maxHeight), maxHeight - minBuildingLength));
      float scaling = 1.0f / (1.0f - (float) (height / maxWidth));
      width = Math.Max( minBuildingLength, Math.Min( (uint) Math.Ceiling( 
        random.Beta( 6.0d, 6.0d * scaling) * maxWidth), maxWidth - 
        minBuildingLength));
      this.buildingWidth = width;
      this.buildingHeight = height;
    }

    private bool isMapCoordinatesFree( uint x, uint y) {
      return isInBounds( x, y) && ! this.occupiedTerrain[y,x] && 
        this.pathMap[y,x] == null;
    }

    private bool isInBounds( uint x, uint y) {
      return x >= 0 && y >= 0 && y < this.pathMap.GetLength( 0) && 
        x < this.pathMap.GetLength( 1);
    }

    int getNextClockwiseDxOf( ushort direction) {
      return Common.getDxOfDirection( Common.getNextClockwiseDirection( 
        direction));
    }

    int getNextClockwiseDyOf( ushort direction) {
      return Common.getDyOfDirection( Common.getNextClockwiseDirection( 
        direction));
    }

    int getOppositeDxOf( ushort direction) {
      return Common.getDxOfDirection( Common.getOppositeDirection( direction));
    }

    int getOppositeDyOf( ushort direction) {
      return Common.getDyOfDirection( Common.getOppositeDirection( direction));
    }
  } 
}