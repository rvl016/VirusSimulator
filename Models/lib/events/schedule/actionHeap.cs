using System;
using System.Collections.Generic;

namespace VirusSimulatorAvalonia.Models.lib.schedule {
  public class ActionHeap {
    private long timestamp;
    private Action actionPtr;
    private List<ActionHeap> children;
    private ActionHeap parent;
    private ActionHeap sibling;

    private static List<ActionHeap> heapTrees;

    ActionHeap( Action actionPtr, long timestamp) {
      this.timestamp = timestamp;
      this.actionPtr = actionPtr;
    } 

    public static void insert( Action actionPtr, long timeStamp) {

    }

    public static long getMin() {
      long minTimestamp;
      return minTimestamp;
    }

    private static void mergeTrees() {

    }

    public static Action extractMin() {

    }
  }
}