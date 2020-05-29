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
    static uint minBuildingLength = 2;
    private Path[,] pathMap;
    private Dictionary<Street,List<uint[]>> map;
    private bool[,] occupiedTerrain;

    public Dictionary<Street,List<uint[]>> run( Path[,] pathMap) {
      this.map = new Dictionary<Street,List<uint[]>>();
      this.occupiedTerrain = new bool[pathMap.GetLength( 0),
        pathMap.GetLength( 1)];
      this.pathMap = pathMap;
      findTerrainsFrom( 0, 0);
      return this.map;
    }

    void findTerrainsFrom( uint startX, uint startY) {
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

    uint getYupperBoundFromWith( uint startX, uint startY, uint maxX) {
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

    uint getXupperBoundFromAt( uint startX, uint y) {
      uint endX = startX;
      while (endX < this.pathMap.GetUpperBound( 1) && 
        this.pathMap[y,endX + 1] == null) 
        endX++;
      return endX;
    }
    void buildOnTerrain( uint startX, uint startY, uint endX, uint endY) {
      if (this.pathMap[startY,Math.Min( endX + 1, 
        pathMap.GetLength( 1))] != null)
        buildFromEastStreet( startY, endY, startX, endX - startX + 1);
      if (this.pathMap[Math.Min( endY + 1, this.pathMap.GetLength( 0)), 
        endX] != null)
        buildFromSouthStreet( startY, endY, endX, endY - startY + 1);
    }

    void buildFromSouthStreet( uint startX, uint endX, uint y, uint yLength) {
      while (endX - startX > minBuildingLength) {
        uint[] buildingDimensions = randomBuildingDimensions( yLength, 
          endY - startY + 1);
        uint[] buildingCoordinates = getBuildingCoordinates( x, startY, 
          (int) -buildingDimensions[0], (int) buildingDimensions[1]);
        Street street = findStreetForBuilding( buildingCoordinates, x + 1, 
          startY, buildingDimensions[1], Defs.lower);
        addBuildingToList( street, buildingCoordinates);
        fillOccupiedTerrain( buildingCoordinates);
        endX -= buildingDimensions[1];
      }
    }

    void buildFromEastStreet( uint startY, uint endY, uint x, uint xLength) {
      while (endY - startY > minBuildingLength) {
        uint[] buildingDimensions = randomBuildingDimensions( xLength, 
          endY - startY + 1);
        uint[] buildingCoordinates = getBuildingCoordinates( x, startY, 
          (int) -buildingDimensions[0], (int) buildingDimensions[1]);
        Street street = findStreetForBuilding( buildingCoordinates, x + 1, 
          startY, buildingDimensions[1], Defs.lower);
        addBuildingToList( street, buildingCoordinates);
        fillOccupiedTerrain( buildingCoordinates);
        startY += buildingDimensions[1];
      }
    }


    Street findStreetForBuilding( uint[] buildingCoordinates, uint startX, 
      uint startY, uint length, ushort direction) {
      Dictionary<char,int> streetCoords = getStreetCoordsForBuilding( startX,
        startY, length, direction);
      uint x = (uint) streetCoords.GetValueOrDefault( 'x');
      uint y = (uint) streetCoords.GetValueOrDefault( 'y');
      buildingCoordinates[4] = x;
      buildingCoordinates[5] = y;
      return (Street) this.pathMap[y,x];
    }

    void addBuildingToList( Street street, uint[] coordinates) {
      if (this.map.GetValueOrDefault( street) == null) 
        this.map.Add( street, new List<uint[]>() { coordinates });
      else {
        List<uint[]> coordinatesList = this.map.GetValueOrDefault( street);
        coordinatesList.Add( coordinates);
      }
    }

    Dictionary<char,int> getStreetCoordsForBuilding( uint startX, uint startY,
      uint length, ushort direction) {
      Dictionary<char,int> delta = Common.directionIterable.
        GetValueOrDefault( direction);
      int middleX = (int) startX + (int) length / 2 * delta.
        GetValueOrDefault( 'x');
      int middleY = (int) startY + (int) length / 2 * delta.
        GetValueOrDefault( 'y');
      int plusSideX = middleX, plusSideY = middleY;
      int minusSideX = middleX, minusSideY = middleY;
      if (length % 2 == 0) {
        minusSideX -= delta.GetValueOrDefault( 'x');
        minusSideY -= delta.GetValueOrDefault( 'y');
      }
      return bidirectionalStreetSearch( plusSideX, plusSideY, middleX,
        middleY, delta, length / 2);
    }

    // This is a way I find to get buildings entry points next to 
    //   building center
    Dictionary<char,int> bidirectionalStreetSearch( int plusSideX, int plusSideY, int minusSideX, int minusSideY, Dictionary<char,int> delta, uint length) {
      for (uint i = length; i > 0; i--) {
        if (this.pathMap[plusSideX,plusSideY].isMountable) 
          return new Dictionary<char,int> { { 'x', plusSideX }, 
            { 'y', plusSideY } };
        if (this.pathMap[minusSideX,minusSideY].isMountable) 
          return new Dictionary<char,int> { { 'x', minusSideX }, 
            { 'y', minusSideY } };
        plusSideX += delta.GetValueOrDefault( 'x');
        plusSideY += delta.GetValueOrDefault( 'y');
        minusSideX -= delta.GetValueOrDefault( 'x');
        minusSideY -= delta.GetValueOrDefault( 'y');
      }
      return null;
    }

    void fillOccupiedTerrain( uint[] buildingCoordinates) {
      for (uint x = buildingCoordinates[0]; x < buildingCoordinates[2]; x++) 
        for (uint y = buildingCoordinates[1]; y < buildingCoordinates[3]; y++) 
          this.occupiedTerrain[y,x] = true;
    }

    uint[] getBuildingCoordinates( uint startX, uint startY, int deltaX, 
      int deltaY) {
      uint[] coordinates = new uint[6];
      coordinates[0] = deltaX < 0 ? (uint) (startX + deltaX) : startX;
      coordinates[1] = deltaY < 0 ? (uint) (startY + deltaY) : startY;
      coordinates[2] = deltaX < 0 ? startX : (uint) (startX + deltaX);
      coordinates[3] = deltaY < 0 ? startY : (uint) (startY + deltaY);
      return coordinates;
    }


    private List<Street> getStreetsAround( uint startX, uint startY, uint endX, 
      uint endY) {
      List<Street> streetsAround = new List<Street>;
      if (startY != 0 && pathMap[startY - 1,(startX + endX / 2)] != null)
        streetsAround.Add( (Street) pathMap[startY - 1,(startX + endX / 2)]);
      if (startY != 0 && pathMap[startY - 1,(startX + endX / 2)] != null)
        streetsAround.Add( (Street) pathMap[startY - 1,(startX + endX / 2)]);
      if (startY != 0 && pathMap[startY - 1,(startX + endX / 2)] != null)
        streetsAround.Add( (Street) pathMap[startY - 1,(startX + endX / 2)]);
      if (startY != 0 && pathMap[startY - 1,(startX + endX / 2)] != null)
        streetsAround.Add( (Street) pathMap[startY - 1,(startX + endX / 2)]);
      return streetsAround;
    }

    static private uint[] randomBuildingDimensions( uint maxWidth, 
      uint maxHeight) {
      uint height = 0, width = 0;
      height = Math.Max( minBuildingLength, Math.Min( (uint) Math.Ceiling( 
        random.Beta( 2.5d, 4.0d) * maxHeight), maxHeight - minBuildingLength));
      float scaling = 1.0f / (1.0f - (float) (height / maxWidth));
      width = Math.Max( minBuildingLength, Math.Min( (uint) Math.Ceiling( 
        random.Beta( 6.0d, 6.0d * scaling) * maxWidth), maxWidth - 
        minBuildingLength));
      return new uint[2] { width, height };
    }

    private uint clipToLimits( uint value, uint start, uint end) {
      return Math.Max( Math.Min( value, end), start);
    }
  } 
}