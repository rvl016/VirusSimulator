using Troschuetz.Random;
using VirusSimulatorAvalonia.Models.things.inanimates.paths;
using VirusSimulatorAvalonia.Models.lib.map;

namespace VirusSimulatorAvalonia.Models.lib.map.buildingAlgorithms {
  
  public sealed class BuildingAlgorithm : BuildingMapAlgorithm {
    
    static TRandom random = new TRandom();
    private short[,] map;

    public short[,] run( Path[,] pathMap) {
      this.map = new short[pathMap.GetLength( 0), pathMap.GetLength( 1)];
    }
  } 
}