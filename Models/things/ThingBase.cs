using System;	
using System.Collections.Generic;
using VirusSimulatorAvalonia.Models.lib.things;

namespace VirusSimulatorAvalonia.Models.things {
  public abstract class ThingBase {
    
    protected static int maxId = 0;
    public Coordinates coordinates;
    public int id;
    public short status;
    protected ThingBase( float xCoordinate, float yCoordinate, ushort zCoordinate = 0) {
      this.id = maxId++;
      this.coordinates = new Coordinates( xCoordinate, yCoordinate, zCoordinate);
    }

    public abstract Dictionary<string,string> dumpProperties(); 
    protected void changeStatus( short changedStateParam, bool isTrue) {
      short paramIsTrue = (short) (isTrue ? 1 : 0);
      this.status = (short) ((this.status & ~ changedStateParam) & (changedStateParam * 
        paramIsTrue));
    }

    protected void callSchedulerFor( Action laterAction, int dayTimeInSeconds) {
      long whenActionRunsInSeconds = God.secondsSinceEpoch - 
        God.secondsSinceEpoch % Consts.aDayInSeconds + dayTimeInSeconds;
      Schedule.scheduleTask( laterAction, whenActionRunsInSeconds);
    }
  }
}