using System;
using System.Linq;
using System.Collections.Generic;

namespace VirusSimulatorAvalonia.Models.lib.schedule {
  public class ScheduledHeap {
    private ulong timestamp;
    private Action actionPtr;
    private List<ScheduledHeap> children;
    private ScheduledHeap sibling;
    private static ScheduledHeap head = null;

    public ScheduledHeap( Action actionPtr, ulong timestamp) {
      this.timestamp = timestamp;
      this.actionPtr = actionPtr;
      this.children = new List<ScheduledHeap>();
    } 

    public static void insert( Action actionPtr, ulong timestamp) {
      ScheduledHeap newHeap = new ScheduledHeap( actionPtr, timestamp);
      makeUnionWith( newHeap);
    }

    public static ulong getMinTimestamp() {
      ScheduledHeap minHeap = getHeapWithMinHeadTimestamp();
      if (minHeap == null)
        return ulong.MaxValue;
      return minHeap.timestamp;
    }

    public static Action extractNextAction() {
      ScheduledHeap minHeap = getHeapWithMinHeadTimestamp();
      ScheduledHeap decomposedHead = decomposeHeap( minHeap);
      makeUnionWith( decomposedHead);
      return minHeap.actionPtr;
    }

    public static bool isEmpty() {
      return head == null;
    }

    private static ScheduledHeap decomposeHeap( ScheduledHeap heap) {
      for (int i = 1; i < heap.children.Count(); i++)
        heap.children[i].sibling = heap.children[i - 1];
      heap.children.First().sibling = null;
      return heap.children.Last();
    }

    private static void makeUnionWith( ScheduledHeap that) {
      if (head == null) {
        head = that;
        return;
      }
      mergeHeapHeadsWith( that);
      deepMergeHeaps();
    }

    private static void mergeHeapHeadsWith( ScheduledHeap that) {
      ScheduledHeap ptr = head;
      while (that != null) {
        ptr = goThroughFromWhileThatHasLargerDegree( ptr, that);
        ScheduledHeap oldThat = that;
        that = that.sibling;
        insertThatAfterThis( oldThat, ptr);
      }
    }

    private static void deepMergeHeaps() {
      ScheduledHeap prevPtr = null, ptr = head, nextPtr = head.sibling;
      while (nextPtr != null) {
        ScheduledHeap nextNextPtr = nextPtr.sibling;
        if (FirstSiblingsFollowsRuleForMerging( ptr, nextPtr, nextNextPtr)) {
          ptr = ptr.mergeWith( nextPtr);
          concatenateHeapsHeads( prevPtr, ptr, nextNextPtr);
        }
        else {
          prevPtr = ptr;
          ptr = nextPtr;
        }
        nextPtr = nextNextPtr;
      }
    }

    private static bool FirstSiblingsFollowsRuleForMerging( 
      ScheduledHeap ptr, ScheduledHeap nextPtr, ScheduledHeap nextNextPtr) {
      return ptr.getDegree() == nextPtr.getDegree() 
        && (nextNextPtr == null 
        || nextPtr.getDegree() != nextNextPtr.getDegree());
    }

    private static void concatenateHeapsHeads( ScheduledHeap prevPtr, 
      ScheduledHeap ptr, ScheduledHeap nextPtr) {
      if (prevPtr != null) 
        prevPtr.sibling = ptr;
      else 
        head = ptr;
      ptr.sibling = nextPtr;
    }

    private static ScheduledHeap goThroughFromWhileThatHasLargerDegree( 
      ScheduledHeap ptr, ScheduledHeap that) {
      while (that.getDegree() > ptr.getDegree() && ptr.sibling != null) 
        ptr = ptr.sibling;
      return ptr;
    }

    private static void insertThatAfterThis( ScheduledHeap that, 
      ScheduledHeap ptr) {
      ScheduledHeap nextPtr = ptr.sibling;
      ptr.sibling = that;
      that.sibling = nextPtr; 
    }

    private ScheduledHeap mergeWith( ScheduledHeap that) {
      if (this.timestamp < that.timestamp)
        return this.foldWith( that);
      return that.foldWith( this);
    }

    private ScheduledHeap foldWith( ScheduledHeap that) {
      ScheduledHeap pivot = this.children.First();
      this.children.Insert( 0, that);
      that.sibling = pivot;
      return this;
    }

    private static ScheduledHeap getHeapWithMinHeadTimestamp() {
      ScheduledHeap minHeap = head, ptr = head;
      while (ptr != null) {
        if (ptr.timestamp < minHeap.timestamp) 
          minHeap = ptr;
        ptr = ptr.sibling;
      }
      return minHeap;
    }

    private int getDegree() {
      return this.children.Count();
    }
  }
}