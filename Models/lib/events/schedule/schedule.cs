using System;

namespace VirusSimulatorAvalonia.Models.lib.schedule {
  public static class Schedule {
    
    public static void enqueueUrgentTask( Action action) {
      UrgentQueue.enqueue( action);
    }

    public static void scheduleTask( Action action, ulong whenInSeconds) {
      ScheduleHeap.insert( action, whenInSeconds);
    }
  }
}