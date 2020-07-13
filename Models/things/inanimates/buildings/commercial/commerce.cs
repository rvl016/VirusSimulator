using VirusSimulatorAvalonia.Models.defs;
using VirusSimulatorAvalonia.Models.hidden.god;
using VirusSimulatorAvalonia.Models.hidden.god.world;
using VirusSimulatorAvalonia.Models.things.inanimates.buildings;


namespace VirusSimulatorAvalonia.Models.things.inanimates.buildings.commerce {
  public sealed class Commerce : Building {

    private uint openTime;
    private uint closeTime;
    private uint employeesNumber = 0;

    public Commerce( float xCoordinate, float yCoordinate, 
      float halfWidth, float halfHeight, ushort floorsNum) :
      base( xCoordinate, yCoordinate, halfWidth, halfHeight, floorsNum) {
      this.setOpenStatus( false);
      ThingsPackage.add( this);
    }

    public void setWorkingHours( uint openTime, uint closeTime) {
      this.openTime = openTime;
      this.closeTime = closeTime;
      uint currentTime = God.getCurrentTime();
      if (currentTime >= openTime && currentTime < closeTime)
        this.setOpenStatus( true);
    }

    public override void setOpenStatus( bool open) {
      if (open && statusIncludes( Defs.mandatoryClose))
        return;
      this.changeStatus( Defs.open, open);  
    }

    public override void definePeopleCapacity() {
      this.peopleCapacity = (uint) (this.effectiveArea * Consts.
        commercePopulationFactor);
    }

    protected override void iterateLifeCycle() {
      
    }

    public override void dumpProperties() {
      
    }

  }

}