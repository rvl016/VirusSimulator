using VirusSimulatorAvalonia.Models.defs;
using VirusSimulatorAvalonia.Models.hidden.god.world;
using VirusSimulatorAvalonia.Models.things.animates.people;
using VirusSimulatorAvalonia.Models.things.animates.vehicles;

namespace VirusSimulatorAvalonia.Models.things.inanimates.buildings.residence {

  public sealed class Residence : Building {

    private uint residentsNumber = 0;

    public Residence( float xCoordinate, float yCoordinate, 
      float halfWidth, float halfHeight, ushort floorsNum) :
      base( xCoordinate, yCoordinate, halfWidth, halfHeight, floorsNum) {
      this.setOpenStatus( true);
      ThingsPackage.add( this);
    }
    
    public override void definePeopleCapacity() {
      this.peopleCapacity = (uint) (this.effectiveArea * Consts.
        residencePopulationFactor);
    }


    protected override void iterateLifeCycle() {

    }

    public override void dumpProperties() {
      
    }
  }
}