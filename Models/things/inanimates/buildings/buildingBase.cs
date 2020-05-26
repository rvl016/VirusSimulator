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
    
    protected Building( float xCoordinate, float yCoordinate,
      float halfWidth, float halfHeight, ushort floorsNum) :
      base( xCoordinate, yCoordinate, halfWidth, halfHeight) {
      this.floorsNum = floorsNum;
      this.effectiveArea = 4.0f * halfHeight * halfHeight * floorsNum;
    }

    public void makeEntryPointsOn( Street street, ushort streetSide) {
      this.sidewalkEntryPoint = street.
        makePedestrianEntryPointOnSideFor( streetSide, this.coordinates);
      this.streetEntryPoint = street.
        makeVehicleEntryPointOnSideFor( streetSide, this.coordinates);
    }
  }
}