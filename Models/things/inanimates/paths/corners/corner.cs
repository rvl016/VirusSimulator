using System;
using System.Linq;
using System.Collections.Generic;
using VirusSimulatorAvalonia.Models.lib.common;
using VirusSimulatorAvalonia.Models.lib.things;
using VirusSimulatorAvalonia.Models.hidden.god.world;
using VirusSimulatorAvalonia.Models.defs;
using VirusSimulatorAvalonia.Models.things.inanimates.paths;

namespace VirusSimulatorAvalonia.Models.things.inanimates.paths.corner {

  public sealed class Corner : Path {

    public List<Node> pedestrianNodes;
    public List<Node> vehicleNodes;
    public override bool isMountable { get { return false; }}

    public Corner( float xCoordinate, float yCoordinate, float halfWidth,
      Path path = null) : 
      base( xCoordinate, yCoordinate, halfWidth, 
      halfWidth) {
      ThingsPackage.add( this);
      if (path != null)
        this.endPoints = new List<Accommodable> { path };
      this.makePathNodes();
    }

    public override List<Node> getPedestrianPathNodesOnSide( 
      ushort direction) {
      return this.pedestrianNodes;
    }

    public override List<Node> getVehiclePathNodesOnSide( ushort direction) {
      return this.vehicleNodes;
    }

    protected override void makePathNodes() {
      this.currentMasterNode = Node.pedestrianMasterNode;
      this.pedestrianNodes = this.makeNodeQuadrature( 
        Node.makeDoubleLinkBetween, this.getSidewalkMiddleRelativeWidth());

      this.currentMasterNode = Node.vehicleMasterNode;
      this.vehicleNodes = this.makeNodeQuadrature( Node.makeLinkFromTo, 
        this.getRoadMiddleRelativeWidth());
    }

    private List<Node> makeNodeQuadrature( 
      Action<Node,Node> linkFunction, float relativeWidth) {
      Node northeastNode = Node.makeNodeInPathWithCoordinates( this, 
        this.coordinates.getRelativeCoordinates( relativeWidth,
        -relativeWidth));
      Node southeastNode = Node.makeNodeInPathWithCoordinates( this, 
        this.coordinates.getRelativeCoordinates( relativeWidth,
        relativeWidth));
      Node southwestNode = Node.makeNodeInPathWithCoordinates( this, 
        this.coordinates.getRelativeCoordinates( -relativeWidth, 
        relativeWidth));
      Node northwestNode = Node.makeNodeInPathWithCoordinates( this, 
        this.coordinates.getRelativeCoordinates( -relativeWidth, 
        -relativeWidth));
      linkFunction( northeastNode, northwestNode);
      linkFunction( northwestNode, southwestNode);
      linkFunction( southwestNode, southeastNode);
      linkFunction( southeastNode, northeastNode);
      return new List<Node> { southeastNode, southwestNode, northwestNode, 
        northeastNode };
    }

    protected override void connectToVehiclePathOnDirection( Path that, 
      ushort direction) {
      List<Node> thisNodes = this.getVehiclePathNodesOnSide( direction);
      List<Node> thatNodes = that.getVehiclePathNodesOnSide( 
        Common.getOppositeDirectionOf( direction));
      thatNodes = thatNodes.OrderBy( node => thisNodes.First().coordinates.
        getDistance( node.coordinates)).ToList();
      // 0 neighbor implies that that street node is right handed
      if (thatNodes.First().neighbors.Count() == 0) {
        Node.makeLinkFromTo( thatNodes.First(), thisNodes.First()); 
        Node.makeLinkFromTo( thisNodes.Last(), thatNodes.Last());
      }
      else {
        Node.makeLinkFromTo( thisNodes.First(), thatNodes.First()); 
        Node.makeLinkFromTo( thatNodes.Last(), thisNodes.Last());
      }
    } 
  }
}