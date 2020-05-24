using System;
using VirusSimulatorAvalonia.Models.defs;
using VirusSimulatorAvalonia.Models.things.inanimates.paths;
using VirusSimulatorAvalonia.Models.things.inanimates.paths.corner;
using VirusSimulatorAvalonia.Models.things.inanimates.paths.street;

namespace VirusSimulatorAvalonia.Models.lib.map {
  sealed class Mapper {
    private short[,] map;
    private MapAlgorithm mapAlgorithm;
    public void sketchEmptyMap( int width, int height) {
      this.map = new short[height, width];
    }

    public void makeMapBluePrint<MapAlgorithmImplementation>( 
      uint minSpaceBetweenStreets) where MapAlgorithmImplementation : 
      MapAlgorithm, new() {
      this.mapAlgorithm = new MapAlgorithmImplementation();
      this.map = this.mapAlgorithm.run( this.map, minSpaceBetweenStreets);
      this.makeCrossesOnBounds();
    } 

    public uint getFirstStreetOnUpperXAxis() {
      for (uint x = 0; x <= this.map.GetUpperBound( 1); x++)
        if (this.map[0,x] > 0)
          return x;
      throw new Exception( 
        "Map blueprint doesn't have street marker on first line.");
    }

    private void goThroughDirectionsFromCorner( uint x, uint y, Corner corner) {
      if (this.map[y,x + 1] > 0)
        makePaths( x + 1, y, Defs.right, corner);
      if (this.map[y,x + 1] > 0)
        makePaths( x, y + 1, Defs.lower, corner);
      if (this.map[y,x - 1] > 0)
        makePaths( x - 1, y, Defs.left, corner);
      if (this.map[y - 1,x] > 0)
        makePaths( x, y - 1, Defs.upper, corner);
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
      uint length = dx == 1 ? x - startX : dx == -1 ? startX - x : dy == 1 ? 
        y - startY : startY - y;
      makeStreet()
      Street newStreet = new Street( startX * Consts.roadWidth + 
        Consts.roadHalfWidth, startY * Consts.roadWidth + Consts.roadHalfWidth, 
        Consts.roadHalfWidth, length * Consts.roadWidth, direction == Defs.upper || direction == Defs.lower ? Defs.vertical : Defs.horizontal, corner);
    }

    private void makePathAtWithDirectionAndFromCorner( uint x, uint y, 
      uint direction, Corner corner = null) {
      Path path;
      if (this.map[y,x] < 0) return;
      if (this.map[y,x] > 1) {
        this.map[x,y] = -1;
        this.goThroughDirectionFromCorner( x, y, new Corner( x * 
          Consts.roadWidth + Consts.roadHalfWidth, y * Consts.roadHalfWidth + 
          Consts.roadHalfWidth, Consts.roadHalfWidth, corner));
        return;
      }
      this.defineStreetFromWithDirectionAndFromCorner( x, y, direction, corner);
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

    public void buildStreetsAndCrosses( float streetWidth) {
      uint xinit = getFirstStreetOnUpperXAxis();
      makePaths( xinit, 0); 
    }

    public void resetMap() {
      Array.Clear( this.map, 0, this.map.Length);
    }
  }

  interface MapAlgorithm {
    short[,] run( short[,] map, uint minSpaceBetweenStreets);
  }
}