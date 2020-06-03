using VirusSimulatorAvalonia.Models.defs;
using VirusSimulatorAvalonia.Models.hidden.god.world;
using VirusSimulatorAvalonia.Models.things.inanimates.buildings;

namespace VirusSimulatorAvalonia.Models.things.inanimates.buildings.quarentine {

  public sealed class Quarentine : Building {

    private uint confinedNumber = 0;
    
    public Quarentine( float xCoordinate, float yCoordinate, 
      float halfWidth, float halfHeight, ushort floorsNum) :
      base( xCoordinate, yCoordinate, halfWidth, halfHeight, floorsNum) {
      this.setOpenStatus( true);
      ThingsPackage.add( this);
    }
    
    public override void definePeopleCapacity() {
      this.peopleCapacity = (uint) (this.effectiveArea * Consts.
        quarentinePopulationFactor);
    }
  }
}