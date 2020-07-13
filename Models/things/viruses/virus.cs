using System;
using System.Collections.Generic;

using VirusSimulatorAvalonia.Models.defs;
using VirusSimulatorAvalonia.Models.lib.events;
using VirusSimulatorAvalonia.Models.hidden.god;
using VirusSimulatorAvalonia.Models.hidden.god.world;
using VirusSimulatorAvalonia.Models.things;
using VirusSimulatorAvalonia.Models.things.animates;
using VirusSimulatorAvalonia.Models.things.animates.people;

namespace VirusSimulatorAvalonia.Models.things.virus {
  public sealed class Virus : Thing {

    private float currentInfectionRadius;
    private ulong whenImmunityWillSettleInSecs;
    public Person host;
    Virus( Person host) : 
      base( host.coordinates) {
      ThingsPackage.add( this);
      host.virus = this;
      this.host = host;
      this.setIncubation();
    }
    public override void dumpProperties() {

    }

    private void setIncubation() {
      this.changeStatus( Defs.incubating, true);
      uint incubationTime = RandomEvents.getVirusIncubationTimeByAgeAndHealthIdx(
        this.host.age, this.host.healthIndex);
      callSchedulerForLater( this.terminateIncubation, incubationTime);
    }

    private void terminateIncubation() {
      this.changeStatus( Defs.incubating, false);
      this.setCurrentInfectionRadius();
      this.defineWhenImmunityWillSettle();
      this.iterateLifeCycle();
    }

    private void setCurrentInfectionRadius() {
      this.currentInfectionRadius = 
        Consts.virusBaseRadius / this.host.healthIndex;
    } 

    private void defineWhenImmunityWillSettle() {
      this.whenImmunityWillSettleInSecs = God.secondsSinceEpoch + 
        RandomEvents.howManySecsSpreadingWillLastByAgeAndHealthIdx(
        this.host.age, this.host.healthIndex);
    }

    private void iterateLifeCycle() {
      if (God.secondsSinceEpoch >= this.whenImmunityWillSettleInSecs) {
        this.host.becomeImmune();
        this.die();
        return;
      }
      this.tryToInfectPeople();
      if (this.willKillHost()) {
        this.killHost();
        return;
      }
      this.callSchedulerFor( this.iterateLifeCycle);
    }

    private void tryToInfectPeople() {
      List<Person> potentialHosts = this.getPeopleOnSight();
      potentialHosts.ForEach( person => {
        bool willInfectPerson = RandomEvents.virusWillInfectPerson();
        if (willInfectPerson && ! person.statusIncludes( Defs.immune) 
          && person.virus == null)
          new Virus( person);
      });
    }

    private bool willKillHost() {
      return RandomEvents.virusWillKillHostByAgeAndHealthIdx(
        this.host.age, this.host.healthIndex);
    }

    private void killHost() {
      this.host.die();
      this.die();
    }

    private void die() {
      ThingsPackage.remove( this);
      this.host.virus = null;
      this.host = null;
    }

    private List<Person> getPeopleOnSight() {
      List<Person> people = this.host.getAccessiblePeople();
      return people.FindAll( person => person.coordinates.getDistance( 
        this.coordinates) < this.currentInfectionRadius);
    }
  }
}