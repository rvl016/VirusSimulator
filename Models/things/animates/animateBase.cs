using System;
using System.Collections.Generic;
using VirusSimulatorAvalonia.Models.things;
using VirusSimulatorAvalonia.Models.things.inanimates;

namespace VirusSimulatorAvalonia.Models.things.animates {
  public abstract class Animate<T> : Thing {


    protected Animate( float xCoordinate, float yCoordinate, ushort zCoordinate) : 
      base( xCoordinate, yCoordinate, zCoordinate) {
    }

    protected abstract void iterateThroughPath();

    protected abstract void defineNextRoute();
      
    protected abstract List<T> getAnimatesOnSight();

    // getSight
    // It can move
    // It can stay still
    // It can interact with inanimates
    // It can interact with animates
        
  }
}