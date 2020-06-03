using System;
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

    public abstract void definePeopleCapacity();

    public static (float, float) getCoordinatesFromBounds( 
      uint[] startCoordinates, uint[] endCoordinates) {
      float xCoordinate = (startCoordinates[0] + endCoordinates[0]) * 
        Consts.roadHalfWidth;
      float yCoordinate = (startCoordinates[1] + endCoordinates[1]) * 
        Consts.roadHalfWidth;
      return (xCoordinate, yCoordinate);
    }

    public static (float, float) getDimesionsFromBounds(
      uint[] startCoordinates, uint[] endCoordinates) {
      float halfWidth = (endCoordinates[0] - startCoordinates[0]) * 
        Consts.roadHalfWidth;
      float halfHeight = (endCoordinates[1] - startCoordinates[1]) * 
        Consts.roadHalfWidth;
      return (halfWidth, halfHeight);
    }

    public static (float, float) getCoordinatesFromDiscrete(
      uint[] coordinates) {
      float xCoordinate = coordinates[0] * Consts.roadWidth + 
        Consts.roadHalfWidth;
      float yCoordinate = coordinates[1] * Consts.roadWidth + 
        Consts.roadHalfWidth;
      return (xCoordinate, yCoordinate);
    }

    public void makeEntryPointsOn( ushort streetSide) {
      Street street = this.streetAddress;
      if (street == null)
        throw new Exception( "Building has no street yet!");
      makeSidewalkEntryPointOn( streetSide);
      makeStreetEntryPointOn( streetSide);
    }

    public void setDoorCoordinates( float xCoordinate, float yCoordinate) {
      this.buildingDoorCoordinates = new Coordinates( xCoordinate, 
        yCoordinate, 0);    
    }
    
    public void makeSidewalkEntryPointOn( ushort streetSide) {
      Street street = this.streetAddress;
      this.sidewalkEntryPoint = street.makePedestrianEntryPointOnSideFor( 
        streetSide, this.buildingDoorCoordinates);
      this.sidewalkEntryPoint.setEntryPoint( this);
    }

    public void makeStreetEntryPointOn( ushort streetSide) {
      Street street = this.streetAddress;
      this.streetEntryPoint = street.makeVehicleEntryPointOnSideFor( 
        streetSide, this.buildingDoorCoordinates);
      this.streetEntryPoint.setEntryPoint( this); 
    }  
  }
}