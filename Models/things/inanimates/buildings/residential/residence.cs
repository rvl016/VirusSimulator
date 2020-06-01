using VirusSimulatorAvalonia.Models.hidden.god.world;
using VirusSimulatorAvalonia.Models.things.inanimates.buildings;

namespace VirusSimulatorAvalonia.Models.things.inanimates.buildings.residence {

  public sealed class Residence : Building {

    private uint residentsNumber = 0;

    public Residence( float xCoordinate, float yCoordinate, 
      float halfWidth, float halfHeight, ushort floorsNum) :
      base( xCoordinate, yCoordinate, halfWidth, halfHeight, floorsNum) {
      this.setOpenStatus( true);
      ThingsPackage.add( this);
    }
    
  }
}