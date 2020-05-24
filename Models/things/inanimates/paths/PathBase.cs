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

    protected abstract void makePathNodes();

    public abstract List<Node> getPedestrianPathNodes( ushort direction);

    public abstract List<Node> getVehiclePathNodes( ushort direction);
    
    public override void makeEndPointOn( Path endpoint) {
      this.endPoints.Add( endpoint);
    }

    protected float getRoadMiddleRelativeWidth() {
      return this.halfWidth * Consts.road2sidewalkRatio / 
        (Consts.road2sidewalkRatio + 1.0f) / 2.0f;     
    }

    protected float getSidewalkMiddleRelativeWidth() {
      return this.halfWidth * (Consts.road2sidewalkRatio + .5f) /
        (Consts.road2sidewalkRatio + 1.0f);
    }
  }
}