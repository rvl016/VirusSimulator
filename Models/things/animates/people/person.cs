using System;
using System.Linq;
using System.Collections.Generic;
using VirusSimulatorAvalonia.Models.defs;
using VirusSimulatorAvalonia.Models.lib.things;
using VirusSimulatorAvalonia.Models.lib.events;
using VirusSimulatorAvalonia.Models.hidden.god.world;
using VirusSimulatorAvalonia.Models.things.inanimates.buildings.residence;
using VirusSimulatorAvalonia.Models.things.animates.vehicles;
using VirusSimulatorAvalonia.Models.things.virus;

namespace VirusSimulatorAvalonia.Models.things.animates.people {
  public sealed class Person : Animate<Person> {

    public bool isUsingMask;
    public bool isImmune; 
    public ushort age;
    public float healthIndex;
    public float walkSpeed;
    public Virus virus;
    public Routine routine;
    public Route currentRoute = null;
    public Node currentTarget = null;
    public Person interactingWith;
    public List<Person> friends;
    public Vehicle ownVehicle;
    public Residence home;
    
    public Person( Residence house, ushort houseFloor) : 
      base( house.coordinates.x, house.coordinates.y, houseFloor) {
      this.home = house;
      this.routine = new Routine( house);
      ThingsPackage.add( this);
    } 

    protected override void iterateThroughPath() {
      if (this.statusIncludes( Defs.dead) || this.interactingWith != null || 
        this.willInteractWithPeople())
        return;
      if (this.currentTarget == null) {
        terminateTrip();
        return;
      }
      if (this.coordinates.isOn( this.currentTarget.coordinates)) {
        goToNextNode();
        return;
      }
      this.coordinates.moveTowardsWith( this.currentTarget.coordinates, 
        this.walkSpeed);
      callSchedulerFor( this.iterateThroughPath);
    }

    private void terminateTrip() {
      changeAccommodationTo( (Accommodable) this.currentRoute.destination);
      this.changeStatus( Defs.moving, false);
      enterBuilding();
    }

    private void goToNextNode() {
      this.currentTarget = this.currentRoute.getNextNodeOnRoute();
      iterateThroughPath();
    }

    private void enterBuilding() {
      this.coordinates.setCoordinatesTo( this.accommodation.coordinates);
      this.coordinates.z = this.routine.destinationFloor;
      defineNextRoute();
      // TODO: move around building
    }

    // Only vehicles and people should do this
    protected override void defineNextRoute() {
      ulong compromiseTime;
      Route route;
      do 
        (compromiseTime, route) = this.routine.getNextCompromise();
      while (route != null && this.accommodation == route.destination);
      if (route == null || ! route.destination.isOpen) 
        callSchedulerForLater( defineNextRoute, Consts.retryInterval);
      else {
        this.currentRoute = route;
        callSchedulerForAt( tryToFollowRoute, compromiseTime);
      }
    }
      
    private void tryToFollowRoute() {
      Accommodable street = this.accommodation.endPoints.First();
      if (! street.canAccommodate( this))
        callSchedulerForLater( tryToFollowRoute, Consts.retryInterval);
      else { 
        getInAccommodation( street);
        followRouteFromStreet();
      }
    }

    private void followRouteFromStreet() {
      this.changeStatus( Defs.moving, true);
      this.currentTarget = defineFirstStreetNode();
      this.coordinates.setCoordinatesTo( this.currentTarget.coordinates);
      iterateThroughPath();
    }

    private Node defineFirstStreetNode() {
      this.currentRoute = this.currentRoute.callRoute();
      return this.currentRoute.startRoute();
    }

    private bool willInteractWithPeople() {
      foreach (Person person in getAnimatesOnSight()) {
        bool areFriends = this.friends.Exists( friend => friend == person);
        if (RandomEvents.bothWillInteractTogether( areFriends) &&
          person.interactingWith == null) {
          this.interactWith( person, areFriends);
          return true;
        }
      }
      return false;
    }

    private void interactWith( Person person, bool areFriends) {
      uint interactionTime = RandomEvents.getPeopleMutualInteractionTime(
        areFriends);
      person.interactWithFor( this, interactionTime);
      this.interactWithFor( person, interactionTime);
    }
      
    private void interactWithFor( Person person, uint interactionTime) {
      this.changeStatus( Defs.interacting, true);
      this.interactingWith = person;
      this.callSchedulerForLater( this.endInteraction, interactionTime);
    }

    private void endInteraction() {
      this.interactingWith = null;
      this.changeStatus( Defs.interacting, false);
      // TODO: Will it make a friend?
      this.iterateThroughPath();
    }
    
    protected override List<Person> getAnimatesOnSight() {
      return this.getAccessiblePeople().FindAll( 
        person => this.coordinates.getDistance( person.coordinates) 
        < Consts.personRadius);
    }

    public List<Person> getAccessiblePeople() {
      return this.accommodation.getPeopleNextTo( this);
    }

    private void changeAccommodationTo( Accommodable accommodable) {
      getOutCurrentAccommodation();
      getInAccommodation( accommodable);
    } 

    private void getOutCurrentAccommodation() {
      this.accommodation.eject( this);
      this.accommodation = null;
    }

    private void getInAccommodation( Accommodable accommodable) {
      accommodable.host( this);
      this.accommodation = accommodable;
    }

    public void becomeImmune() {
      this.changeStatus( Defs.immune, true);
    }

    public void die() {
      // May cause process crash
      this.coordinates = null;
      this.changeStatus( Defs.dead, true);
    }
  }
}