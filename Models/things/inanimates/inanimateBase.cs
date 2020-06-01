using System.Collections.Generic;
using VirusSimulatorAvalonia.Models.defs;
using VirusSimulatorAvalonia.Models.lib.things;
using VirusSimulatorAvalonia.Models.things.inanimates.paths;


namespace VirusSimulatorAvalonia.Models.things.inanimates {

  public abstract class Inanimate : Thing {

    public List<Path> endPoints;
    public float halfWidth;
    public float halfHeight;
      // It can hold animates
      // It can be open or close
    protected Inanimate( float xCoordinate, float yCoordinate, float halfWidth, 
      float halfHeight) :
      base( xCoordinate, yCoordinate, 0) {
      this.halfHeight = halfHeight;
      this.halfWidth = halfWidth;
      this.endPoints = new List<Path>();
    }

    public abstract Dictionary<string,string> dumpProperties(); 

    protected abstract void iterateLifeCycle();

    public abstract void makeEndPointOn( Path endpoint);

    public abstract void setOpenStatus( bool open); 

    public bool isOpen() {
      return this.statusIncludes( (short) Defs.open);
    }
  }
}