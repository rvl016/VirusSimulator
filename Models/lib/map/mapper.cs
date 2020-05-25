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
    }

    public void makeMapBluePrint<StreetMapAlgorithmImplementation>( 
      uint minSpaceBetweenStreets, int height, int width) 
      where StreetMapAlgorithmImplementation : StreetMapAlgorithm, new() {
      this.mapAlgorithm = new StreetMapAlgorithmImplementation();
      this.sketchEmptyMap( height, width);
      this.map = this.mapAlgorithm.run( this.map, minSpaceBetweenStreets);
      this.makeCrossesOnBounds();
    } 

    public uint getFirstCrossOnUpperXAxis() {
      for (uint x = 0; x <= this.map.GetUpperBound( 1); x++)
        if (this.map[0,x] > 1)
          return x;
      throw new Exception( 
        "Map blueprint doesn't have street marker on first line.");
    }

    private void makeCrossesOnBounds() {
      for (uint i = 0; i <= this.map.GetUpperBound( 1); i++) {
        if (this.map[0,i] > 0) 
          this.map[0,i]++;
        if (this.map[this.map.GetUpperBound( 0),i] > 0) 
          this.map[this.map.GetUpperBound( 0),i]++;
      }
      for (uint i = 0; i <= this.map.GetUpperBound( 0); i++) {
        if (this.map[i,0] > 0) 
          this.map[i,0]++;
        if (this.map[i,this.map.GetUpperBound( 1)] > 0) 
          this.map[i,this.map.GetUpperBound( 1)]++;
      }
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

    private Street makeStreetFromCorner( float xCoordinate, float yCoordinate, 
      float halfHeight, ushort direction, Corner corner) {
      Street newStreet = new Street( xCoordinate, yCoordinate, 
        Consts.roadHalfWidth, halfHeight, direction == Defs.upper || 
        direction == Defs.lower ? Defs.vertical : Defs.horizontal, corner);
      corner.connectToPathOnDirection( newStreet, direction);
      return newStreet;
    }

    private void goThroughStreet( uint startX, uint startY, ushort direction, 
      Corner corner) {
      uint x = startX;
      uint y = startY;
      int dx = direction == Defs.right ? 1 : (direction == Defs.left ? -1 : 0);
      int dy = direction == Defs.lower ? 1 : (direction == Defs.upper ? -1 : 0);
      uint streetLength = 0;
      do {
        streetLength++;
        this.map[y,x] = -1;
        x = (uint) (x + dx);
        y = (uint) (y + dy);
      } while (this.map[y,x] == 1);
      uint length = (uint) (Math.Abs( dx) == 1 ? Math.Abs( x - startX) : 
        Math.Abs( y - startY));
      Street newStreet = makeStreetFromCorner( Consts.roadWidth * (float) 
        (startX + x) / 2.0f, Consts.roadWidth * (float) (startY + y) / 2.0f, 
        (float) length * Consts.roadHalfWidth, direction, corner);
      this.makeCornerFromStreet( x, y, direction, newStreet);
    }

    public build
    public void buildStreetsAndCrosses() {
      uint xInit = getFirstCrossOnUpperXAxis();
      Corner newCorner = new Corner( Consts.roadWidth * xInit + 
        Consts.roadHalfWidth, Consts.roadHalfWidth, Consts.roadHalfWidth);
      this.pathMap[0,xInit] = newCorner;
      this.map[0,xInit] = -1;
      goThroughDirectionsFromCorner( xInit, 0, newCorner);
    }

    public void resetMap() {
      Array.Clear( this.map, 0, this.map.Length);
    }
  }

// Any implementation must NOT generate directly adjacent street corners! 
  interface StreetMapAlgorithm {
    short[,] run( short[,] map, uint minSpaceBetweenStreets);
  }
}