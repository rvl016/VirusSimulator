using System;
using System.Linq;
using System.Collections.Generic;
using Troschuetz.Random;
using VirusSimulatorAvalonia.Models.defs;
using VirusSimulatorAvalonia.Models.lib.common;
using VirusSimulatorAvalonia.Models.lib.map.streetAlgorithms;
using VirusSimulatorAvalonia.Models.lib.map.buildingAlgorithms;
using VirusSimulatorAvalonia.Models.things.inanimates.buildings;
using VirusSimulatorAvalonia.Models.things.inanimates.buildings.residence;
using VirusSimulatorAvalonia.Models.things.inanimates.buildings.commerce;
using VirusSimulatorAvalonia.Models.things.inanimates.buildings.quarentine;
using VirusSimulatorAvalonia.Models.things.inanimates.paths;
using VirusSimulatorAvalonia.Models.things.inanimates.paths.corner;
using VirusSimulatorAvalonia.Models.things.inanimates.paths.street;

namespace VirusSimulatorAvalonia.Models.lib.map {
  sealed class Mapper {

    private static readonly uint minSpaceBetweenStreets = 3;
    private static TRandom random = new TRandom();
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

    // I don't know what to do to improve these if/else... =(
    public void buildBuildings() {
      List<Tuple<Street,uint[][]>> buildingOrders = 
        getRandomizedBuildingsOrders();
      Building building;
      for (int i = 0; i < buildingOrders.Count; i++) {
        if (i / buildingOrders.Count < Consts.residenceProportion)
          building = makeResidenceWithAt( buildingOrders[i].Item2);
        else if (i / buildingOrders.Count < Consts.residenceProportion + 
          Consts.commerceProportion)
          building = makeCommerceWithAt( buildingOrders[i].Item2);
        else 
          building = makeQuarentineWithAt( buildingOrders[i].Item2);
        placeBuildingOnStreet( building, buildingOrders[i].Item1, 
          buildingOrders[i].Item2[2]);
      }
    }

    private List<Tuple<Street,uint[][]>> getRandomizedBuildingsOrders() {
      List<Tuple<Street,uint[][]>> buildingList = this.street2Buildings.
        ToList().SelectMany( pair => {
          Street street = pair.Key;
          return pair.Value.Select( buildingCoordinates => 
            new Tuple<Street,uint[][]> ( street, buildingCoordinates )
        );
      }).ToList();
      return buildingList.Select( pair => new { value = pair, 
        rand = random.ContinuousUniform( 0, 1) }).OrderBy( pair => pair.rand).
        Select( pair => pair.value).ToList();
    }

    private Residence makeResidenceWithAt( uint[][] coordinates) {
      return makeBuildingWithAt<Residence>( coordinates, 
        (xCoordinate, yCoordinate, halfWidth, halfHeight, floorsNum) => 
        new Residence( xCoordinate, yCoordinate, halfWidth, 
        halfHeight, floorsNum));
    }
    
    private Commerce makeCommerceWithAt( uint[][] coordinates) {
      return makeBuildingWithAt<Commerce>( coordinates, 
        (xCoordinate, yCoordinate, halfWidth, halfHeight, floorsNum) => 
        new Commerce( xCoordinate, yCoordinate, halfWidth, 
        halfHeight, floorsNum));
    }
    
    private Quarentine makeQuarentineWithAt( uint[][] coordinates) {
      return makeBuildingWithAt<Quarentine>( coordinates, 
        (xCoordinate, yCoordinate, halfWidth, halfHeight, floorsNum) => 
        new Quarentine( xCoordinate, yCoordinate, halfWidth, 
        halfHeight, floorsNum));
    }

    private BuildingType makeBuildingWithAt<BuildingType>( uint[][] coordinates, 
      Func<float,float,float,float,ushort,BuildingType> newBuilding) 
      where BuildingType : Building {
      uint[] startCoordinates = coordinates[0], endCoordinates = coordinates[1];
      uint[] doorCoordinates = coordinates[2];
      ushort floorsNumber = getRandomBuildingFloorNumber( startCoordinates,
        endCoordinates);
      var (xCoordinate, yCoordinate) = Building.getCoordinatesFromBounds(
        startCoordinates, endCoordinates);
      var (halfWidth, halfHeight) = Building.getDimesionsFromBounds(
        startCoordinates, endCoordinates);
      return newBuilding( xCoordinate, yCoordinate, halfWidth, 
        halfHeight, floorsNumber);
    }

    private void placeBuildingOnStreet( Building building, 
      Street street, uint[] doorCoordinates) {
      var (doorXcoord, doorYcoord) = Building.getCoordinatesFromDiscrete(
        doorCoordinates);
      building.setDoorCoordinates( doorXcoord, doorYcoord);
      building.streetAddress = street;
      ushort streetSide = street.getThingRelativeSide( building);
      building.makeEntryPointsOn( streetSide);
    }

    private void goThroughDirectionsFromCorner( uint x, uint y, 
      Corner corner) {
      if (x < this.map.GetUpperBound( 1) && this.map[y,x + 1] > 0)
        this.goThroughStreet( x + 1, y, Defs.right, corner);
      if (y < this.map.GetUpperBound( 0) && this.map[y + 1,x] > 0)
        this.goThroughStreet( x, y + 1, Defs.down, corner);
      if (x > 0 && this.map[y,x - 1] > 0)
        this.goThroughStreet( x - 1, y, Defs.left, corner);
      if (y > 0 && this.map[y - 1,x] > 0)
        this.goThroughStreet( x, y - 1, Defs.up, corner);
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
        Consts.roadHalfWidth, halfHeight, direction == Defs.up || 
        direction == Defs.down ? Defs.vertical : Defs.horizontal, corner);
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
      int dy = direction == Defs.down ? 1 : (direction == Defs.up ? -1 : 0);
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

    private ushort getRandomBuildingFloorNumber( uint[] startCoordinates,
      uint[] endCoordinates) {
      uint buildingArea = (endCoordinates[0] - startCoordinates[0]) * 
        (endCoordinates[1] - endCoordinates[1]);
      double gammaParameter = (double) buildingArea * Consts.
        randomBuildingFloorsScaleFactor;
      return (ushort) (1 + random.Gamma( gammaParameter, gammaParameter));
    }

  }

// Any implementation must NOT generate directly adjacent street corners! 
// Any implementation must generate corners on dead ends!
// Any implementation must return a matrix with 1s for street and 
// any number greater than 1 for corners! (else, 0)
  interface StreetMapAlgorithm {
    short[,] run( short[,] map, uint minSpaceBetweenStreets);
  }

// The List on Dictionary is an array with 3 pairs: southwest coordinates,
//  northeast coordinates and door coordinates, respectively.
  interface BuildingMapAlgorithm {
    Dictionary<Street,List<uint[][]>> run( Path[,] pathMap);
  }
}