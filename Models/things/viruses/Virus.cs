using System;
using VirusSimulatorAvalonia.Models.things;
using VirusSimulatorAvalonia.Models.things.animates;
using VirusSimulatorAvalonia.Models.things.animates.people;

namespace VirusSimulatorAvalonia.Models.viruses {
  public sealed class Virus : Thing {

    float currentInfectionRadius;
    Person host;
    Virus( Person host) : 
      base( 0.0f, 0.0f, 0) {
      this.host = host;
      this.setIncubation();
      this.changeStatus( Consts.incubating, true);
    }

    ~Virus() {
      // take virus out of things package
    }

    public override Dictionary<string,string> dumpProperties() {

    }

    private override void iterateLifeCycle() {

    }

    private void tryToKillHost() {

    }


  }
}