using System;
using VirusSimulatorAvalonia.Models.things.inanimates.paths;

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

    private void makePaths( uint x, uint y) {
      if (this.map[x,y] < 0) return;
      if (this.map[x,y] > 1) {
        new Corner()
      }
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