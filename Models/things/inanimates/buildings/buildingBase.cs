using System;
using System.Linq;
using System.Collections.Generic;
using VirusSimulatorAvalonia.Models.defs;
using VirusSimulatorAvalonia.Models.lib.things;
using VirusSimulatorAvalonia.Models.things.animates.people;
using VirusSimulatorAvalonia.Models.things.animates.vehicles;
using VirusSimulatorAvalonia.Models.things.inanimates.paths.street;

namespace VirusSimulatorAvalonia.Models.things.inanimates.buildings {

  public abstract class Building : Inanimate, Accommodable {

    public uint floorsNum;
    public uint peopleCapacity;
    public uint currentPeopleInside = 0;
    public float effectiveArea;
    public Street streetAddress;
    public override List<Accommodable> endPoints {
      get { return new List<Accommodable> { (Accommodable) streetAddress }; }
    }
    public Node streetEntryPoint;
    public Node sidewalkEntryPoint;
    public Coordinates buildingDoorCoordinates;
    public Dictionary<ushort,Dictionary<uint,Person>> floor2id2person;
    public Dictionary<uint,ushort> id2floor;
    
    protected Building( float xCoordinate, float yCoordinate,
      float halfWidth, float halfHeight, ushort floorsNum) :
      base( xCoordinate, yCoordinate, halfWidth, halfHeight) {
      this.floorsNum = floorsNum;
      this.effectiveArea = halfHeight * halfHeight * floorsNum;
      this.id2floor = new Dictionary<uint, ushort>();
      this.floor2id2person = 
        new Dictionary<ushort, Dictionary<uint, Person>>();
      Enumerable.Range( 0, floorsNum).ToList().ForEach( floor => {
        this.floor2id2person[(ushort) floor] = new Dictionary<uint, Person>();
      });
    }

    public abstract void definePeopleCapacity();

    public bool canAccommodate( Vehicle vehicle) {
      return true;
    }

    public bool canAccommodate( Person person) {
      return this.currentPeopleInside < this.peopleCapacity; 
    }

    public List<Vehicle> getVehiclesNextTo( Vehicle vehicle) {
      return new List<Vehicle>();
    }

    public List<Person> getPeopleNextTo( Person person) {
      ushort personsFloor = this.id2floor[person.id];
      return this.floor2id2person[personsFloor].Values.ToList();
    }

    public void host( Vehicle vehicle) {
      vehicle.setAttached( true);
    }

    public void host( Person person, ushort floor) {
      this.currentPeopleInside++;
      this.id2floor.Add( person.id, floor);
      this.floor2id2person[floor].Add( person.id, person);
    }

    public void eject( Vehicle vehicle) {
      vehicle.setAttached( false);
    }

    public void eject( Person person) {
      ushort personsFloor = this.id2floor[person.id];
      this.floor2id2person[personsFloor].Remove( person.id);
      this.id2floor.Remove( person.id);
      this.currentPeopleInside--;
    }

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