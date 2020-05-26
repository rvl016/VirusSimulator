using System;
using VirusSimulatorAvalonia.Models.defs;
using VirusSimulatorAvalonia.Models.things.inanimates.paths;
using VirusSimulatorAvalonia.Models.things.inanimates.paths.corner;
using VirusSimulatorAvalonia.Models.things.inanimates.paths.street;

namespace VirusSimulatorAvalonia.Models.lib.map {
  sealed class Mapper {
    private short[,] map;
    private Path[,] pathMap;
    private StreetMapAlgorithm mapAlgorithm;

    public void sketchEmptyMap( int width, int height) {
      this.map = new short[height, width];
      this.pathMap = new Path[height,width];
    }

    public void resetMap() {
      Array.Clear( this.map, 0, this.map.Length);
      Array.Clear( this.pathMap, 0, this.pathMap.Length);
    }
    
    public void makeMapBluePrint<StreetMapAlgorithmImplementation>( 
      uint minSpaceBetweenStreets, int height, int width) 
      where StreetMapAlgorithmImplementation : StreetMapAlgorithm, new() {
      this.mapAlgorithm = new StreetMapAlgorithmImplementation();
      this.sketchEmptyMap( height, width);
      this.map = this.mapAlgorithm.run( this.map, minSpaceBetweenStreets);
    } 


    public void buildStreetsAndCrosses() {
      uint xInit = getFirstCrossOnUpperXAxis();
      Corner newCorner = new Corner( Consts.roadWidth * xInit + 
        Consts.roadHalfWidth, Consts.roadHalfWidth, Consts.roadHalfWidth);
      this.pathMap[0,xInit] = newCorner;
      this.map[0,xInit] = -1;
      goThroughDirectionsFromCorner( xInit, 0, newCorner);
    }

    private void goThroughDirectionsFromCorner( uint x, uint y, Corner corner) {
      if (this.map[y,x + 1] > 0)
        this.goThroughStreet( x + 1, y, Defs.right, corner);
      if (this.map[y,x + 1] > 0)
        this.goThroughStreet( x, y + 1, Defs.lower, corner);
      if (this.map[y,x - 1] > 0)
        this.goThroughStreet( x - 1, y, Defs.left, corner);
      if (this.map[y - 1,x] > 0)
        this.goThroughStreet( x, y - 1, Defs.upper, corner);
    }
    
    private void goThroughStreet( uint startX, uint startY, ushort direction, 
      Corner corner) {
      uint x = startX;
      uint y = startY;
      int dx = direction == Defs.right ? 1 : (direction == Defs.left ? -1 : 0);
      int dy = direction == Defs.lower ? 1 : (direction == Defs.upper ? -1 : 0);
      uint streetLength = 0;
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
      uint x = startX;
      uint y = startY;
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

  }

// Any implementation must NOT generate directly adjacent street corners! 
// Any implementation must generate corners on dead ends!
// Any implementation must return a matrix with 1s for street and 
// any number greater than 1 for corners!
  interface StreetMapAlgorithm {
    short[,] run( short[,] map, uint minSpaceBetweenStreets);
  }

  interface BuildingMapAlgorithm {
    short[,] run( Path[,] pathMap);
  }
}