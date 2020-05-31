using VirusSimulatorAvalonia.Models.defs;
using VirusSimulatorAvalonia.Models.lib.things;
using VirusSimulatorAvalonia.Models.things.inanimates.paths.street;

namespace VirusSimulatorAvalonia.Models.things.inanimates.buildings {

  public abstract class Building : Inanimate {

    public uint floorsNum;
    public uint peopleCapacity;

    public uint currentPeopleInside = 0;
    public float effectiveArea;
    public Street streetAddress;

    public Node streetEntryPoint;
    public Node sidewalkEntryPoint;
    public Coordinates buildingDoorCoordinates;
    
    protected Building( float xCoordinate, float yCoordinate,
      float halfWidth, float halfHeight, ushort floorsNum) :
      base( xCoordinate, yCoordinate, halfWidth, halfHeight) {
      this.floorsNum = floorsNum;
      this.effectiveArea = 4.0f * halfHeight * halfHeight * floorsNum;
    }

    public void setDoorCoordinates( float xCoordinate, float yCoordinate) {
      this.buildingDoorCoordinates = new Coordinates( xCoordinate, 
        yCoordinate, 0);    
    }
    
    public void makeEntryPointsOn( Street street, ushort streetSide) {
      this.sidewalkEntryPoint = street.makePedestrianEntryPointOnSideFor( 
        streetSide, this.buildingDoorCoordinates);
      this.streetEntryPoint = street.makeVehicleEntryPointOnSideFor( 
        streetSide, this.buildingDoorCoordinates);
    }

  }
}