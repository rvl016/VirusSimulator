using System;
using System.Linq;
using System.Collections.Generic;

namespace VirusSimulatorAvalonia.Models.lib.schedule {
  public static class AnimateQueue {

    static List<Action> currentAnimations = new List<Action>();
    static List<Action> futureAnimations = new List<Action>();
    
    public static void runCurrentAnimations() {
      currentAnimations = futureAnimations;
      futureAnimations.Clear();
      currentAnimations.ForEach( animation => animation());
      currentAnimations.Clear();
    }

    public static void enqueue( Action animation) {
      futureAnimations.Append( animation);
    }
  }
}