using System;	
using System.Collections.Generic;
using VirusSimulatorAvalonia.Models.defs;
using VirusSimulatorAvalonia.Models.hidden.god;
using VirusSimulatorAvalonia.Models.lib.things;

namespace VirusSimulatorAvalonia.Models.things {

  public abstract class Thing {
    
    protected static uint maxId = 0;
    public uint id;
    public ushort status;

    public Coordinates coordinates;

    protected Thing( float xCoordinate, float yCoordinate, ushort zCoordinate = 0) {
      this.id = maxId++;
      this.coordinates = new Coordinates( xCoordinate, yCoordinate, zCoordinate);
    }

    protected Thing( Coordinates coordinates) {
      this.id = maxId++;
      this.coordinates = coordinates;
    }
    
    public abstract Dictionary<string,string> dumpProperties(); 
    protected abstract void iterateLifeCycle();
    
    protected void changeStatus( ushort changedStateParam, bool isTrue) {
      short paramIsTrue = (short) (isTrue ? 1 : 0);
      this.status = (ushort) ((this.status & ~ changedStateParam) &
       (changedStateParam * paramIsTrue));
    }

    protected void toggleStatus( ushort changedStateParam) {
      this.changeStatus( changedStateParam, 
        (this.status & changedStateParam) == 0);
    }

    protected bool statusIncludes( short state) {
      return (this.status & state) != 0;
    }

    protected void callSchedulerForAt( Action laterAction, 
      ulong time) {
      Schedule.scheduleTask( laterAction, time);
    }

    protected void callSchedulerForLater( Action laterAction, 
      uint secondsFromNow) {
      ulong whenActionRunsInSeconds = God.secondsSinceEpoch + secondsFromNow;
      Schedule.scheduleTask( laterAction, whenActionRunsInSeconds);
    }

    protected void callSchedulerFor( Action action) {
      Schedule.enqueueAction( action);
    }

  }
}