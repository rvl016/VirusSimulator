using System;
using VirusSimulatorAvalonia.Models.defs;
using VirusSimulatorAvalonia.Models.hidden.god;
using VirusSimulatorAvalonia.Models.things.inanimates.buildings;

namespace VirusSimulatorAvalonia.Models.lib.things {

  public sealed class Routine {

    private sealed class Target {
      public Building origin = null;
      public Building destination = null;
      public ushort destinationFloor;
      public Route route = null;
      public ushort importance = 0;
      public uint startsAt = 0;
      public uint endsAt = Consts.aDayInSeconds - 1;
      public Target next = null;
    }

    private Building home;
    private Target targetHead;
    private Target targetPtr = null;
    private Target currentTarget = null;
    private ushort numberOfDestinations = 1;
    public ushort destinationFloor;

    public Routine( Building home) {
      this.home = home;
      this.targetHead = new Target();
      this.targetHead.origin = home;
      this.targetHead.destination = home;
    }

    public void makeTargetToBetweenBy( Building destination, ushort floor, 
      uint startsAt, uint endsAt, ushort transport) {
      Target target = new Target();
      target.destination = destination;
      target.destinationFloor = floor;
      target.startsAt = startsAt;
      target.endsAt = endsAt;
      target.importance = 1;
      insertTargetInRoutine( target);
      makeRouteForBy( target, transport);
    }

    public bool routineFitsInSchedule( uint startsAt, uint endsAt) {
      this.targetPtr = this.targetHead;
      while (this.targetPtr != null && this.targetPtr.endsAt <= endsAt
        && this.targetPtr.startsAt >= startsAt)
        this.targetPtr = this.targetPtr.next;
      return this.targetPtr == null ? false : this.targetPtr.importance == 0;
    }

    public (ulong,Route) getNextCompromise() {
      if (this.numberOfDestinations == 1)
        return (0, null);
      if (this.currentTarget == null)
        this.currentTarget = this.targetHead;
      this.destinationFloor = this.currentTarget.destinationFloor;
      return (getNextCompromiseTime(), getNextCompromisseRoute());
    }

    private ulong getNextCompromiseTime() {
      ulong time = this.currentTarget.startsAt;
      if (this.currentTarget.startsAt < God.getCurrentTime()) 
        time += God.getTomorrowMidNight();
      else 
        time += God.getTodayMidNight();
      return time;
    }

    private Route getNextCompromisseRoute() {
      Route route = this.currentTarget.route;
      this.currentTarget = this.currentTarget.next;
      return route;
    }

    private void insertTargetInRoutine( Target target) {
      if (this.targetPtr == null)
        throw new Exception( 
          "Inserting target without checking if it fits in schedule.");
      this.targetPtr.endsAt = target.startsAt;
      target.origin = this.targetPtr.destination;
      Target laterTarget = insertHomeTargetAfterNewTarget( target);
      this.targetPtr.next = target;
      this.numberOfDestinations += 2;
      this.targetPtr = null;
    }

    private void makeRouteForBy( Target target, ushort transport) {
      target.route = Route.newRouteBetweenBy( target.origin, 
        target.destination, transport);
      target.next.route = Route.newRouteBetweenBy( target.destination, 
        target.next.destination, transport);
    }

    private Target insertHomeTargetAfterNewTarget( Target target) {
      Target homeTarget = getNewHomeTargetBetween( target.endsAt, 
        this.targetPtr.endsAt);
      homeTarget.origin = target.destination;
      homeTarget.next = this.targetPtr.next;
      target.next = homeTarget;
      return homeTarget;
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