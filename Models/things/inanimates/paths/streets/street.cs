using System.Collections.Generic;
using VirusSimulatorAvalonia.Models.things.inanimates.paths;
using VirusSimulatorAvalonia.Models.lib.things;

namespace VirusSimulatorAvalonia.Models.things.inanimates.paths.corner {
  sealed class Street : Path {
    public Node currentMasterNode {
      get;
    }
    Street( float xCoordinate, float yCoordinate, float halfWidth, 
      float halfHeight, List<Path> streets) : 
      base( xCoordinate, yCoordinate, halfWidth, halfHeight) {
      for (Street street in streets) {
        
      }
      this.currentMasterNode = null;
    }
  }  
}