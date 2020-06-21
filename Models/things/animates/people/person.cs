using System;
using System.Linq;
using System.Collections.Generic;
using VirusSimulatorAvalonia.Models.defs;
using VirusSimulatorAvalonia.Models.lib.things;
using VirusSimulatorAvalonia.Models.lib.events;
using VirusSimulatorAvalonia.Models.hidden.god.world;
using VirusSimulatorAvalonia.Models.things.inanimates.buildings.residence;
using VirusSimulatorAvalonia.Models.things.animates;
using VirusSimulatorAvalonia.Models.things.virus;

namespace VirusSimulatorAvalonia.Models.things.animates.people {
  public sealed class Person : Animate<Person> {

    public bool isUsingMask;
    public bool isImmune; 
    public ushort age;
    public float healthIndex;
    public Virus virus;
    public Routine routine;
    public Route currentRoute = null;
    public Person interactingWith;
    public List<Person> friends;
    public Vehicle ownVehicle;
    public Residence home;
    public Accommodable accommodation; 
    private bool canAnimate = true;
    
    public Person( Residence house, ushort houseFloor) : 
      base( house.coordinates.x, house.coordinates.y, houseFloor) {
      this.home = house;
      this.routine = new Routine( house);
      ThingsPackage.add( this);
    } 

    private bool TryToGetIn( Accommodable accommodable) {
      if (! accommodable.canAccommodate( this))
        return false;
      accommodable.host( this);
      this.accommodation = accommodable;
      return true;
    } 

    private void getOutAcommodation() {
      this.accommodation.eject( this);
      this.accommodation = null;
    }

    private override void iterateThroughPath() {

    }

    // Only vehicles and people should do this
    protected override void defineNextTarget() {
      ulong compromiseTime;
      Route route;
      do 
        (compromiseTime, route) = this.routine.getNextCompromise();
      while (route != null && this.accommodation == route.destination);
      if (route == null) 
        callSchedulerForLater( defineNextTarget, Consts.retryInterval);
      else
        callSchedulerForAt( getOutAndFollowRoute, compromiseTime);
    }
      
    private void getOutAndFollowRoute() {
      Accommodable street = this.accommodation.endPoints.First();
      if (! street.canAccommodate( this))
        callSchedulerForLater( getOutAndFollowRoute, Consts.retryInterval);
      this.accommodation.eject( this);
      this.
    }

    private void endInteraction() {
      this.interactingWith = null;
      this.iterateThroughPath();
    }

    private void interactWith( Person person, bool areFriends) {
      uint interactionTime = RandomEvents.getPeopleMutualInteractionTime(
        areFriends);
      this.interactingWith = person;
      person.interactWithFor( this, interactionTime);
      this.callSchedulerForLater( this.endInteraction, interactionTime);
    }
      
    public void interactWithFor( Person person, uint interactionTime) {
      this.interactingWith = person;
      this.callSchedulerForLater( this.endInteraction, interactionTime);
    }

    protected override List<Person> getSight() {
      return this.accommodation.getPeopleNextTo( this).FindAll( 
        person => this.coordinates.getDistance( person.coordinates) < 
        Consts.personRadius);
    }
    
    private void tryToInteractWithPeople() {
      List<Person> peopleOnThisSight = getSight();
      peopleOnThisSight.ForEach( person => {
        bool areFriends = this.friends.Exists( friend => friend == person);
        if (RandomEvents.bothWillInteractTogether( areFriends) &&
          person.interactingWith == null) {
          this.interactWith( person, areFriends);
          return;
        }
      });
    }
  }
}