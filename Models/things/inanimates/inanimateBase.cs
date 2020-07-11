using System.Collections.Generic;
using VirusSimulatorAvalonia.Models.defs;
using VirusSimulatorAvalonia.Models.lib.things;
using VirusSimulatorAvalonia.Models.things.inanimates.paths;


namespace VirusSimulatorAvalonia.Models.things.inanimates {

  public abstract class Inanimate : Thing {

    public bool isOpen {
      get { return this.getOpenStatus(); }
      set { this.setOpenStatus( value); }
    }
    public abstract List<Accommodable> endPoints {
      get;
      set;
    }
    public float halfWidth;
    public float halfHeight;
      // It can hold animates
      // It can be open or close
    protected Inanimate( float xCoordinate, float yCoordinate, float halfWidth, 
      float halfHeight) :
      base( xCoordinate, yCoordinate, 0) {
      this.halfHeight = halfHeight;
      this.halfWidth = halfWidth;
      this.endPoints = new List<Accommodable>();
    }

    protected abstract void iterateLifeCycle();

    public abstract void makeEndPointOn( Path endpoint);

    protected void setOpenStatus( bool open) {
      this.changeStatus( Defs.open, open);
    } 

    protected bool getOpenStatus() {
      return this.statusIncludes( Defs.open);
    }
  }
}