using System;
using System.Collections.Generic;

using VirusSimulatorAvalonia.Models.defs;
using VirusSimulatorAvalonia.Models.lib.events;
using VirusSimulatorAvalonia.Models.hidden.god.world;
using VirusSimulatorAvalonia.Models.things;
using VirusSimulatorAvalonia.Models.things.animates;
using VirusSimulatorAvalonia.Models.things.animates.people;

namespace VirusSimulatorAvalonia.Models.things.virus {
  public sealed class Virus : Thing {

    float currentInfectionRadius;
    ulong whenImmunityWillSettleInSecs;
    Person host;
    Virus( Person host) : 
      base( host.coordinates) {
      ThingsPackage.add( this);
      host.virus = this;
      this.host = host;
      this.setIncubation();
    }
    public override Dictionary<string,string> dumpProperties() {
      return new Dictionary<string, string>();
    }

    private void setIncubation() {
      this.changeStatus( Consts.incubating, true);
      uint incubationTime = RandomEvents.getVirusIncubationTimeByAgeAndHealthIdx(
        this.host.age, this.host.healthIndex);
      callSchedulerForLater( this.terminateIncubation, incubationTime);
    }

    private void terminateIncubation() {
      this.changeStatus( Consts.incubating, false);
      this.defineWhenImmunityWillSettle();
      this.iterateLifeCycle();
    }

    private void defineWhenImmunityWillSettle() {
      this.whenImmunityWillSettleInSecs = God.secondsSinceEpoch + 
        RandomEvents.howManySecsSpreadingWillLastByAgeAndHealthIdx(
        this.host.age, this.host.healthIndex);
    }

    protected override void iterateLifeCycle() {
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

    private bool willKillHost() {
      return RandomEvents.virusWillKillHostByAgeAndHealthIdx(
        this.host.age, this.host.healthIndex);
    }

    private void killHost() {
      this.host.die();
      this.die();
    }

    private void die() {
      this.host.virus = null;
      this.host = null;
      ThingsPackage.delVirus( this);
    }
  }
}