using System;

namespace VirusSimulatorAvalonia.Models.lib.schedule {
  public static class Schedule {
    
    public static void enqueueUrgentTask( Action action) {
      UrgentQueue.enqueue( action);
    }

    public static void scheduleTask( long whenInSeconds, Action action) {
      ActionHeap.insert( whenInSeconds, action);
    }
  }
}