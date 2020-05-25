using System;
using System.Collections.Generic;
using VirusSimulatorAvalonia.Models.things;
using VirusSimulatorAvalonia.Models.things.inanimates;

namespace VirusSimulatorAvalonia.Models.things.animates {
  public abstract class Animate<T> : Thing {

    protected Inanimate target;

    protected Animate( float xCoordinate, float yCoordinate, ushort zCoordinate) : 
      base( xCoordinate, yCoordinate, zCoordinate) {
      this.target = null;
      this.nextAction = null;
    }

    protected abstract void iterateThroughPath();

    // Only vehicles and people should do this
    protected abstract void defineNextTarget();
      
    protected abstract List<T> getSight();

    // getSight
    // It can move
    // It can stay still
    // It can interact with inanimates
    // It can interact with animates
        
  }
}