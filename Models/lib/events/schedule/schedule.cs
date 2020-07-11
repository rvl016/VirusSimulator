using System;

using VirusSimulatorAvalonia.Models.hidden.god;

namespace VirusSimulatorAvalonia.Models.lib.schedule {
  public static class Scheduler {
    
    public static void enqueueUrgentTask( Action action) {
      UrgentQueue.enqueue( action);
    }

    public static void scheduleTask( Action action, ulong whenInSeconds) {
      ScheduledHeap.insert( action, whenInSeconds);
    }

    public static void generateNextFrame() {
      UrgentQueue.runCurrentActions();
      runOverdueSchedule();
    }

    private static void runOverdueSchedule() {
      while (! ScheduledHeap.isEmpty() 
        && ScheduledHeap.getMinTimestamp() >= God.getCurrentTime()) {
        Action nextAction = ScheduledHeap.extractNextAction();
        nextAction();
      }
    }
  }
}