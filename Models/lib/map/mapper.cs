using System;
using System.Linq;
using System.Collections.Generic;
using Troschuetz.Random;
using VirusSimulatorAvalonia.Models.defs;
using VirusSimulatorAvalonia.Models.lib.common;
using VirusSimulatorAvalonia.Models.lib.map.streetAlgorithms;
using VirusSimulatorAvalonia.Models.lib.map.buildingAlgorithms;
using VirusSimulatorAvalonia.Models.things.inanimates.paths;
using VirusSimulatorAvalonia.Models.things.inanimates.paths.corner;
using VirusSimulatorAvalonia.Models.things.inanimates.paths.street;

namespace VirusSimulatorAvalonia.Models.lib.map {
  sealed class Mapper {

    static readonly uint minSpaceBetweenStreets = 3;
    static TRandom random = new TRandom();
    private short[,] map;
    private Path[,] pathMap;
    private StreetMapAlgorithm mapAlgorithm;
    private BuildingMapAlgorithm buildingsAlgorithm;
    private Dictionary<Street,List<uint[][]>> street2Buildings;

    public void sketchEmptyMap( int width, int height) {
      this.map = new short[height, width];
      this.pathMap = new Path[height,width];
    }

    public void resetMap() {
      Array.Clear( this.map, 0, this.map.Length);
      Array.Clear( this.pathMap, 0, this.pathMap.Length);
    }
    
    public void makeInanimateWorld( int width, int height) {
      sketchEmptyMap( width, height);
      makeMapBluePrint<RecursiveDivision>( minSpaceBetweenStreets, 
        width, height);
      buildStreetsAndCrosses();
      makeBuildingsBluePrint<BuildingAlgorithm>();
      buildBuildings();
    }

    public void makeMapBluePrint<StreetMapAlgorithmImplementation>( 
      uint minSpaceBetweenStreets, int width, int height) 
      where StreetMapAlgorithmImplementation : StreetMapAlgorithm, new() {
      this.mapAlgorithm = new StreetMapAlgorithmImplementation();
      this.sketchEmptyMap( width, height);
      this.map = this.mapAlgorithm.run( this.map, minSpaceBetweenStreets);
    } 

    public void makeBuildingsBluePrint<BuildingMapAlgorithmImplementation>()
      where BuildingMapAlgorithmImplementation : BuildingMapAlgorithm, 
      new() {
      this.buildingsAlgorithm = new BuildingMapAlgorithmImplementation();
      this.street2Buildings = this.buildingsAlgorithm.run( this.pathMap);
    }

    public void buildStreetsAndCrosses() {
      uint xInit = getFirstCrossOnUpperXAxis();
      Corner newCorner = new Corner( Consts.roadWidth * xInit + 
        Consts.roadHalfWidth, Consts.roadHalfWidth, Consts.roadHalfWidth);
      this.pathMap[0,xInit] = newCorner;
      this.map[0,xInit] = -1;
      goThroughDirectionsFromCorner( xInit, 0, newCorner);
    }

    public void buildBuildings() {
      this.street2Buildings.ToList().ForEach( pair => {
        Street street = pair.Key;
        pair.Value.ForEach( buildingCoordinates => 
          makeBuildingWithAt( buildingCoordinates, street)
        );
      });
    }

    private void makeBuildingWithAt( uint[][] coordinates, Street street) {
      uint[] startCoordinates = coordinates[0];
      uint[] endCoordinates = coordinates[1];
      uint[] doorCoordinates = coordinates[2];
      uint area = (endCoordinates[0] - startCoordinates[0]) * 
        (endCoordinates[1] - endCoordinates[1]);
      ushort floorsNumber = getRandomBuildingFloorNumber( area);
      
    }

    private void goThroughDirectionsFromCorner( uint x, uint y, 
      Corner corner) {
      if (x < this.map.GetUpperBound( 1) && this.map[y,x + 1] > 0)
        this.goThroughStreet( x + 1, y, Defs.right, corner);
      if (y < this.map.GetUpperBound( 0) && this.map[y + 1,x] > 0)
        this.goThroughStreet( x, y + 1, Defs.lower, corner);
      if (x > 0 && this.map[y,x - 1] > 0)
        this.goThroughStreet( x - 1, y, Defs.left, corner);
      if (y > 0 && this.map[y - 1,x] > 0)
        this.goThroughStreet( x, y - 1, Defs.upper, corner);
    }
    
    private void goThroughStreet( uint startX, uint startY, ushort direction, 
      Corner corner) {
      int dx = Common.getDxOfDirection( direction);
      int dy = Common.getDyOfDirection( direction);
      uint streetLength = 0, x = startX, y = startY;
      while (this.map[y,x] == 1) {
        streetLength++;
        this.map[y,x] = -1;
        x = (uint) (x + dx);
        y = (uint) (y + dy);
      }
      Street newStreet = makeStreetFromCorner( Consts.roadWidth * (float) 
        (startX + x) / 2.0f, Consts.roadWidth * (float) (startY + y) / 2.0f, 
        (float) streetLength * Consts.roadHalfWidth, direction, corner);
      this.setStreetOnPathMap( startX, startY, direction, streetLength,
        newStreet);
      this.makeCornerFromStreet( x, y, direction, newStreet);
    }

    private Street makeStreetFromCorner( float xCoordinate, float yCoordinate, 
      float halfHeight, ushort direction, Corner corner) {
      Street newStreet = new Street( xCoordinate, yCoordinate, 
        Consts.roadHalfWidth, halfHeight, direction == Defs.upper || 
        direction == Defs.lower ? Defs.vertical : Defs.horizontal, corner);
      corner.connectToPathOnDirection( newStreet, direction);
      return newStreet;
    }

    private void makeCornerFromStreet( uint x, uint y, ushort direction, 
      Street street) {
      if (this.map[y,x] > 1) {
        this.map[y,x] = -1;
        Corner newCorner = new Corner( Consts.roadWidth * x + 
          Consts.roadHalfWidth, Consts.roadWidth * y + Consts.roadHalfWidth, 
          Consts.roadHalfWidth, street);
        street.connectToPathOnDirection( newCorner, direction);
        this.pathMap[y,x] = newCorner;
        goThroughDirectionsFromCorner( x, y, newCorner);
      } 
      else {
        Path corner = this.pathMap[y,x];
        street.connectToPathOnDirection( corner, direction);
      }
    }

    private void setStreetOnPathMap( uint startX, uint startY, 
      ushort direction, uint length, Street street) {
      uint x = startX, y = startY;
      int dx = direction == Defs.right ? 1 : (direction == Defs.left ? -1 : 0);
      int dy = direction == Defs.lower ? 1 : (direction == Defs.upper ? -1 : 0);
      while (length > 0) {
        this.pathMap[y,x] = street;
        x = (uint) (x + dx);
        y = (uint) (y + dy);
        length--;
      }
    }

    private uint getFirstCrossOnUpperXAxis() {
      for (uint x = 0; x <= this.map.GetUpperBound( 1); x++)
        if (this.map[0,x] > 1)
          return x;
      throw new Exception( 
        "Map blueprint doesn't have street marker on first line.");
    }

    private ushort getRandomBuildingFloorNumber( uint buildingArea) {
      return (ushort) (1 + random.Gamma( (double) buildingArea / 4.0d, 
        4.0d / (double) buildingArea));
    }

  }

// Any implementation must NOT generate directly adjacent street corners! 
// Any implementation must generate corners on dead ends!
// Any implementation must return a matrix with 1s for street and 
// any number greater than 1 for corners!
  interface StreetMapAlgorithm {
    short[,] run( short[,] map, uint minSpaceBetweenStreets);
  }

  interface BuildingMapAlgorithm {
    Dictionary<Street,List<uint[][]>> run( Path[,] pathMap);
  }
}