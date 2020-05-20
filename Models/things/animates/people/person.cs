using System;
using System.Collections.Generic;
using VirusSimulatorAvalonia.Models.lib.things;
using VirusSimulatorAvalonia.Models.lib.events;
using VirusSimulatorAvalonia.Models.things.animates;

namespace VirusSimulatorAvalonia.Models.things.animates.people {
  public sealed class Person : Animate<Person> {

    public bool isUsingMask;
    public bool isImmune; 
    public ushort age;
    public float healthIndex;
    public Virus virus;
    public Target routineTarget;
    public Route pathRoutes;
    public Person interactingWith;
    public List<Person> friends;
    public Vehicle ownVehicle;
    public Accommodable accommodation; 
    
    // how to make getIn and getOutgeneric to fit people, vehicles and paths?
    private bool TryToGetIn( Accommodable accommodable) {
      if (! accommodable.canAccommodate( this))
        return false;
      accommodable.host( this);
      this.accommodation = accommodable;
      return true;
    } 

    // only people and vehicles getOut just reproduce
    private void getOutAcommodation() {
      this.accommodation.eject( this);
      this.accommodation = null;
    }

    private override void iterateThroughPath() {

    }

    // Only vehicles and people should do this
    protected override void defineNextTarget() {
      this.target = this.routineTarget.getNextTarget();
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
      this.callSchedulerFor( this.endInteraction, interactionTime);
    }
      
    public void interactWithFor( Person person, uint interactionTime) {
      this.interactingWith = person;
      this.callSchedulerFor( this.endInteraction, interactionTime);
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