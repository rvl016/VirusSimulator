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

    ActionHeap( long timestamp, Action actionPtr) {
      this.timestamp = timestamp;
      this.actionPtr = actionPtr;
    } 

    public static void insert( long timeStamp, Action actionPtr) {

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