using System;
using System.Collections.Generic;
using VirusSimulatorAvalonia.Models.defs;
using VirusSimulatorAvalonia.Models.lib.things;

namespace VirusSimulatorAvalonia.Models.things.animates {
  public abstract class Animate<AnimateType> : Thing {

    public Accommodable accommodation; 

    protected Animate( float xCoordinate, float yCoordinate, ushort zCoordinate) : 
      base( xCoordinate, yCoordinate, zCoordinate) {
    }

    protected abstract void iterateThroughPath();

    protected abstract void defineNextRoute();
      
    protected abstract List<AnimateType> getAnimatesOnSight();

    public void setAttached( bool isAttached) {
      this.changeStatus( Defs.attached, isAttached);
    }

    // getSight
    // It can move
    // It can stay still
    // It can interact with inanimates
    // It can interact with animates
        
  }
}