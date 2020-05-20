using System;

namespace VirusSimulatorAvalonia.Models.lib.schedule {
  public static class Schedule {
    
    public static void enqueueUrgentTask( Action action) {
      UrgentQueue.enqueue( action);
    }

    public static void scheduleTask( Action action, long whenInSeconds) {
      ActionHeap.insert( action, whenInSeconds);
    }
  }
}