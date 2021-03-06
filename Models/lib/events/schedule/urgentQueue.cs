using System;
using System.Linq;
using System.Collections.Generic;

namespace VirusSimulatorAvalonia.Models.lib.schedule {
  public static class UrgentQueue {

    static List<Action> currentActions = new List<Action>();
    static List<Action> nextActions = new List<Action>();
    
    public static void runCurrentActions() {
      currentActions = nextActions;
      nextActions.Clear();
      currentActions.ForEach( doAction => {
        try {
          doAction();
        } 
        catch {
          Console.Out.WriteLine( "UrgentQueue: can't do an action.");
        }
      });
      currentActions.Clear();
    }

    public static void enqueue( Action action) {
      nextActions.Append( action);
    }
  }
}