using System.Linq;
using System.Collections.Generic;
using VirusSimulatorAvalonia.Models.defs;
using VirusSimulatorAvalonia.Models.lib.common;
using VirusSimulatorAvalonia.Models.lib.things;
using VirusSimulatorAvalonia.Models.things.inanimates;
using VirusSimulatorAvalonia.Models.things.animates.people;
using VirusSimulatorAvalonia.Models.things.animates.vehicles;

namespace VirusSimulatorAvalonia.Models.things.inanimates.paths {
  public abstract class Path : Inanimate, Navegable, Accommodable {
    
    public Node currentMasterNode {
      get;
      set;
    }
    public Dictionary<uint,Vehicle> vehicles;
    public Dictionary<uint,Person> people;
    public override List<Accommodable> endPoints {
      get;
      set;
    }
    public abstract bool isMountable { get; }
    
    protected Path( float xCoordinate, float yCoordinate, float halfWidth, 
      float halfHeight) :
      base( xCoordinate, yCoordinate, halfWidth, halfHeight) {
      this.people = new Dictionary<uint,Person>();
      this.vehicles = new Dictionary<uint,Vehicle>();
    }

    protected abstract void makePathNodes();

    public abstract List<Node> getPedestrianPathNodesOnSide( ushort direction);

    public abstract List<Node> getVehiclePathNodesOnSide( ushort direction);
    
    protected abstract void connectToVehiclePathOnDirection( Path that, 
      ushort direction);

    public override void dumpProperties() {

    }

    protected override void iterateLifeCycle() {

    }

    public bool canAccommodate( Vehicle vehicle) {
      //TODO: Maybe here is the opportunity to verify vehicle's licence plate.
      return this.isOpen;
    }

    public bool canAccommodate( Person person) {
      return this.isOpen;
    }

    public void connectToPathOnDirection( Path that, ushort direction) {
      this.connectToPedestrianPathOnDirection( that, direction);
      this.connectToVehiclePathOnDirection( that, direction);
    }

    protected void connectToPedestrianPathOnDirection( Path that, 
      ushort direction) {
      List<Node> thisNodes = this.getPedestrianPathNodesOnSide( direction);
      List<Node> thatNodes = that.getPedestrianPathNodesOnSide( 
        Common.getOppositeDirectionOf( direction));
      thatNodes = thatNodes.OrderBy( node => thisNodes.First().coordinates.
        getDistance( node.coordinates)).ToList();
      Node.makeDoubleLinkBetween( thisNodes.First(), thatNodes.First()); 
      Node.makeDoubleLinkBetween( thisNodes.Last(), thatNodes.Last()); 
    }


    public void makeEndPointOn( Path endpoint) {
      this.endPoints.Add( endpoint);
    }

    protected float getRoadMiddleRelativeWidth() {
      return this.halfWidth * Consts.road2sidewalkRatio / 
        (Consts.road2sidewalkRatio + 1.0f) / 2.0f;     
    }

    protected float getSidewalkMiddleRelativeWidth() {
      return this.halfWidth * (Consts.road2sidewalkRatio + .5f) /
        (Consts.road2sidewalkRatio + 1.0f);
    }

    // Accommodable implementation
    public bool canEnter( Person person) {
      return true;
    }

    public bool canEnter( Vehicle vehicle) {
      return true;
    }

    public void host( Person person, ushort floor) {
      this.people.Add( person.id, person);
    }

    public void host( Vehicle vehicle) {
      this.vehicles.Add( vehicle.id, vehicle);
    }

    public void eject( Person person) {
      this.people.Remove( person.id);
    }

    public void eject( Vehicle vehicle) {
      this.vehicles.Remove( vehicle.id);
    }

    public List<Person> getPeopleNextTo( Person person) {
      return this.people.Select( personEntry => personEntry.Value).ToList();
    }

    public List<Vehicle> getVehiclesNextTo( Vehicle vehicle) {
      return this.vehicles.Select( vehicleEntry => vehicleEntry.Value).ToList();
    }

    ///////////////////////////////////////////////////////////////////////////
  }
}