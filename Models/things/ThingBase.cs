using System;	
using System.Collections.Generic;
using VirusSimulatorAvalonia.Models.lib.things;

namespace VirusSimulatorAvalonia.Models.things {
  public abstract class Thing {
    
    protected static uint maxId = 0;
    public uint id;
    public short status;

    public Action nextAction;
    public Action scheduledAction;
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
    protected void changeStatus( short changedStateParam, bool isTrue) {
      short paramIsTrue = (short) (isTrue ? 1 : 0);
      this.status = (short) ((this.status & ~ changedStateParam) & (changedStateParam * 
        paramIsTrue));
    }

    protected void toggleStatus( short changedStateParam) {
      this.changeStatus( changedStateParam, (this.status & changedStateParam) == 0);
    }

    protected bool statusIncludes( short state) {
      return (this.status & state) != 0;
    }

    protected void callSchedulerForAt( Action laterAction, 
      uint dayTimeInSeconds) {
      ulong whenActionRunsInSeconds = God.secondsSinceEpoch - 
        God.secondsSinceEpoch % Consts.aDayInSeconds + dayTimeInSeconds;
      Schedule.scheduleTask( laterAction, whenActionRunsInSeconds);
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