using VirusSimulatorAvalonia.Models.things.inanimates.buildings;

public sealed class Commercial : Building {

  private uint openTime;
  private uint closeTime;

  public Commercial( float xCoordinate, float yCoordinate, 
    float halfWidth, float halfHeight, ushort floorsNum) :
    base( xCoordinate, yCoordinate, halfWidth, halfHeight, floorsNum) {
    this.setOpen( false);
  }

  public void setWorkingHours( uint openTime, uint closeTime) {
    this.openTime = openTime;
    this.closeTime = closeTime;
    if (God.)
  }
}