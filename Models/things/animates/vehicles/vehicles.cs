using System.Linq;
using System.Collections.Generic;
using VirusSimulatorAvalonia.Models.defs;
using VirusSimulatorAvalonia.Models.lib.things;
using VirusSimulatorAvalonia.Models.lib.events;
using VirusSimulatorAvalonia.Models.hidden.god.world;
using VirusSimulatorAvalonia.Models.things.inanimates.buildings.residence;
using VirusSimulatorAvalonia.Models.things.animates;
using VirusSimulatorAvalonia.Models.things.virus;

namespace VirusSimulatorAvalonia.Models.things.animates.vehicles {
  public sealed class Vehicle : Animate<Vehicle> {


    Vehicle( float xCoordinate, float yCoordinate) : 
      base( xCoordinate, yCoordinate, 0) {
    }

  }
}