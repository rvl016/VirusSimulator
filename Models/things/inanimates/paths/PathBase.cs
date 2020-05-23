using System.Collections.Generic;
using VirusSimulatorAvalonia.Models.lib.things;
using VirusSimulatorAvalonia.Models.things.inanimates;

namespace VirusSimulatorAvalonia.Models.things.inanimates.paths {
  public abstract class Path : Inanimate, Navegable {
    
    public Node currentMasterNode {
      get;
      set;
    }
    protected Path( float xCoordinate, float yCoordinate, float halfWidth, 
      float halfHeight) :
      base( xCoordinate, yCoordinate, halfWidth, halfHeight) {
    }

    public override void makeEndPointOn( Path endpoint) {
      this.endPoints.Add( endpoint);
    }

    public abstract void makePathNodes();
  }
}