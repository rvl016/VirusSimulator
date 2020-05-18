using System;

namespace VirusSimulatorAvalonia.Models.lib.schedule {
  public static class Schedule {
    
    public static void enqueueAnimation( Action animation) {
      AnimateQueue.enqueue( animation);
    }

    public static void scheduleTask( long whenInSeconds, Action action) {
      ActionHeap.insert( whenInSeconds, action);
    }
  }
}