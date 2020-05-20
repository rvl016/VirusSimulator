using System.Collections.Generic;
using VirusSimulatorAvalonia.Models.lib.things;
using VirusSimulatorAvalonia.Models.things.inanimates;

namespace VirusSimulatorAvalonia.Models.things.inanimates.paths {
  public class Path : Inanimate, Navegable {
    
    Node currentMasterNode;
    
    protected Path( float xCoordinate, float yCoordinate, float halfWidth, 
      float halfHeight) :
      base( xCoordinate, yCoordinate, halfWidth, halfHeight) {
    }

    public override void makeEndPointOn( Path endpoint) {
      this.endPoints.Add( endpoint);
    }
    public abstract Dictionary<string,string> dumpProperties(); 

    protected abstract void iterateLifeCycle();
      
  }
}