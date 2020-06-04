using System;
using VirusSimulatorAvalonia.Models.defs;
using VirusSimulatorAvalonia.Models.hidden.god;
using VirusSimulatorAvalonia.Models.things.inanimates.buildings;

namespace VirusSimulatorAvalonia.Models.lib.things {

  public sealed class Routine {

    private sealed class Target {
      public Building origin = null;
      public Building destination = null;
      public Route route = null;
      public ushort importance = 0;
      public uint startsAt = 0;
      public uint endsAt = Consts.aDayInSeconds - 1;
      public Target next;
    }

    Building home;
    Target targetHead;
    Target targetPtr = null;
    Target currentTarget = null;

    public Routine( Building home) {
      this.home = home;
      this.targetHead = new Target();
      this.targetHead.origin = home;
      this.targetHead.destination = home;
    }

    public void makeTargetToBetween( Building destination, 
      uint startsAt, uint endsAt) {
      Target target = new Target();
      target.destination = destination;
      target.startsAt = startsAt;
      target.endsAt = endsAt;
      insertTargetInRoutine( target);
    }

    private void insertTargetInRoutine( Target target) {
      if (this.targetPtr == null)
        throw new Exception( 
          "Inserting target without checking if it fits in schedule.");
      this.targetPtr.endsAt = target.startsAt;
      Target newHomeTarget = getNewHomeTargetBetween( target.endsAt, 
        this.targetPtr.endsAt);
      newHomeTarget.next = this.targetPtr.next;
      target.next = newHomeTarget;
      this.targetPtr.next = target;
      this.targetPtr = null;
    }

    public bool routineFitsInSchedule( uint startsAt, uint endsAt) {
      this.targetPtr = this.targetHead;
      while (this.targetPtr != null && this.targetPtr.endsAt < endsAt 
        && this.targetPtr.startsAt > startsAt)
        this.targetPtr = this.targetPtr.next;
      return this.targetPtr == null ? false : this.targetPtr.importance == 0;
    }

    private Target getNewHomeTargetBetween( uint startsAt, uint endsAt) {
      Target newTarget = new Target();
      newTarget.origin = this.home;
      newTarget.destination = this.home;
      newTarget.startsAt = startsAt;
      newTarget.endsAt = endsAt;
      return newTarget;
    }

  }
}