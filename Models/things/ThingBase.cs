using System;	
using System.Collections.Generic;
using VirusSimulatorAvalonia.Models.lib.things;

namespace VirusSimulatorAvalonia.Models.things {
  public abstract class Thing {
    
    protected static int maxId = 0;
    public int id;
    public short status;

    public Action nextAction;
    public Action scheduledAction;
    public Coordinates coordinates;
    protected Thing( float xCoordinate, float yCoordinate, ushort zCoordinate = 0) {
      this.id = maxId++;
      this.coordinates = new Coordinates( xCoordinate, yCoordinate, zCoordinate);
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

    protected void callSchedulerFor( Action laterAction, uint dayTimeInSeconds) {
      ulong whenActionRunsInSeconds = God.secondsSinceEpoch - 
        God.secondsSinceEpoch % Consts.aDayInSeconds + dayTimeInSeconds;
      Schedule.scheduleTask( laterAction, whenActionRunsInSeconds);
    }

    protected void callSchedulerFor( Action animation) {
      Schedule.enqueueAnimation( animation);
    }

  }
}