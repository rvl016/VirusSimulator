using System;
using System.Linq;
using System.Collections.Generic;
using VirusSimulatorAvalonia.Models.defs;
using VirusSimulatorAvalonia.Models.hidden.god.world;
using VirusSimulatorAvalonia.Models.lib.things;

using VirusSimulatorAvalonia.Models.things.inanimates.paths;

namespace VirusSimulatorAvalonia.Models.things.inanimates.paths.street {
  public sealed class Street : Path {

    public ushort orientation;
    public List<Node> rightSideVehiclePathNodes;
    public List<Node> rightSidePedestrianPathNodes;
    public List<Node> leftSidePedestrianPathNodes;
    public List<Node> leftSideVehiclePathNodes;
    public List<Building> rightSideBuildings;
    public List<Building> leftSideBuildings;

    public Street( float xCoordinate, float yCoordinate, float halfWidth, 
      float halfHeight, ushort orientation, Path path = null) : 
      base( xCoordinate, yCoordinate, halfWidth, halfHeight) {
      ThingsPackage.add( this);
      if (path != null)
        this.endPoints = new List<Path> { path };
      this.orientation = orientation;
      if (this.orientation == Defs.horizontal)
        this.makeNodeQuadrature = this.makeHorizontalNodeQuadrature;
      else
        this.makeNodeQuadrature = this.makeVerticalNodeQuadrature;
      this.makePathNodes();
    }

    public override void connectToVehiclePathOnDirection( Path that, 
      ushort direction) {
      List<Node> thisNodes = this.getVehiclePathNodes( direction);
      List<Node> thatNodes = that.getVehiclePathNodes( 
        Defs.oppositeDirectionOf( direction));
      thatNodes = thatNodes.OrderBy( node => thisNodes.First().coordinates.
        getDistance( node.coordinates)).ToList();
      // 0 neighbor implies that this street node is right handed
      if (thisNodes.First().neighbors.Count() == 0) {
        Node.makeLinkFromTo( thisNodes.First(), thatNodes.First()); 
        Node.makeLinkFromTo( thatNodes.Last(), thisNodes.Last());
      }
      else {
        Node.makeLinkFromTo( thatNodes.First(), thisNodes.First()); 
        Node.makeLinkFromTo( thisNodes.Last(), thatNodes.Last());
      }
    } 

    // Returns array of (list of nodes): first elements are right nodes.
    private Func<Action<Node,Node>,float,List<Node>[]> makeNodeQuadrature;
    private List<Node>[] makeHorizontalNodeQuadrature( 
      Action<Node,Node> linkFunction, float relativeWidth) {
      Node northeastNode = Node.makeNodeInPathWithCoordinates( this, 
        this.coordinates.getRelativeCoordinates( this.halfHeight,
        -relativeWidth));
      Node southeastNode = Node.makeNodeInPathWithCoordinates( this, 
        this.coordinates.getRelativeCoordinates( this.halfHeight,
        relativeWidth));
      Node southwestNode = Node.makeNodeInPathWithCoordinates( this, 
        this.coordinates.getRelativeCoordinates( -this.halfHeight, 
        relativeWidth));
      Node northwestNode = Node.makeNodeInPathWithCoordinates( this, 
        this.coordinates.getRelativeCoordinates( -this.halfHeight, 
        -relativeWidth));
      linkFunction( southwestNode, southeastNode);
      linkFunction( northeastNode, northwestNode);
      return new [] { new List<Node> { southwestNode, southeastNode }, 
        new List<Node> { northeastNode, northwestNode } };
    }

    private List<Node>[] makeVerticalNodeQuadrature( Action<Node,Node> linkFunction, 
      float relativeWidth) {
      Node northeastNode = Node.makeNodeInPathWithCoordinates( this, 
        this.coordinates.getRelativeCoordinates( relativeWidth, 
        -this.halfHeight));
      Node southeastNode = Node.makeNodeInPathWithCoordinates( this, 
        this.coordinates.getRelativeCoordinates( relativeWidth, 
        this.halfHeight));
      Node southwestNode = Node.makeNodeInPathWithCoordinates( this, 
        this.coordinates.getRelativeCoordinates( -relativeWidth, 
        this.halfHeight));
      Node northwestNode = Node.makeNodeInPathWithCoordinates( this, 
        this.coordinates.getRelativeCoordinates( -relativeWidth, 
        -this.halfHeight));
      linkFunction( southeastNode, northeastNode);
      linkFunction( northwestNode, southwestNode);
      return new [] { new List<Node> { northeastNode, southeastNode }, 
        new List<Node> { northwestNode, southeastNode } };
    }

    protected override void makePathNodes() {
      List<Node>[] nodesBySide;
      this.currentMasterNode = Node.pedestrianMasterNode;
      nodesBySide = this.makeNodeQuadrature( 
        Node.makeDoubleLinkBetween, this.getSidewalkMiddleRelativeWidth());
      this.rightSidePedestrianPathNodes = nodesBySide[0];
      this.leftSidePedestrianPathNodes = nodesBySide[1];

      this.currentMasterNode = Node.vehicleMasterNode;
      nodesBySide = this.makeNodeQuadrature( Node.makeLinkFromTo, 
        this.getRoadMiddleRelativeWidth());
      this.rightSideVehiclePathNodes = nodesBySide[0];
      this.leftSideVehiclePathNodes = nodesBySide[1];
    }

    public override List<Node> getPedestrianPathNodes( ushort direction) {
      List<Node> allPedestrianNodes = this.rightSidePedestrianPathNodes.Concat( 
        this.leftSidePedestrianPathNodes).ToList();
      return this.filterNodesForDirection( allPedestrianNodes, direction);
    }

    public override List<Node> getVehiclePathNodes( ushort direction) {
      List<Node> allVehicleNodes = this.rightSideVehiclePathNodes.Concat( 
        this.leftSideVehiclePathNodes).ToList();
      return this.filterNodesForDirection( allVehicleNodes, direction);
    }

    private List<Node> filterNodesForDirection( List<Node> nodes, 
      ushort direction) {
      if (this.orientation == Defs.horizontal)
        return nodes.FindAll( node => direction == Defs.upper ? 
          node.coordinates.x > this.coordinates.x : node.coordinates.x < 
          this.coordinates.x);
      return nodes.FindAll( node => direction == Defs.upper ? 
        node.coordinates.y < this.coordinates.y : node.coordinates.y > 
        this.coordinates.y);
    }

    private Node makeEntryPointNodeFromBy( List<Node> nodes, 
      Coordinates buildingCoordinates) {
      float convexFraction;
      if (this.orientation == Defs.horizontal)
        convexFraction = nodes.First().coordinates.getHorizontalConvexFraction(
          buildingCoordinates, nodes.Last().coordinates); 
      else    
        convexFraction = nodes.First().coordinates.getVerticalConvexFraction(
          buildingCoordinates, nodes.Last().coordinates);
      Coordinates nodeCoordinates = nodes.First().coordinates.
        makeConvexCombination( nodes.Last().coordinates, convexFraction);
      return Node.makeNodeInPathWithCoordinates( this, nodeCoordinates);
    }

    public Node makePedestrianRightEntryPointOnSide( ushort streetSide, 
      Coordinates buildingCoordinates) {
        this.currentMasterNode = Node.pedestrianMasterNode;
        Node entryPoint = this.makeEntryPointNodeFromBy( this.
          rightSidePedestrianPathNodes, buildingCoordinates);
        int afterNodeIdx = this.rightSidePedestrianPathNodes.FindIndex( 
          node => node.coordinates.x > entryPoint.coordinates.x);
        this.rightSidePedestrianPathNodes[afterNodeIdx - 1].
          switchMutualNeighborsFromTo( this.rightSidePedestrianPathNodes[
          afterNodeIdx], entryPoint);
        Node.makeDoubleLinkBetween( entryPoint, this.
          rightSidePedestrianPathNodes[afterNodeIdx]);
        this.rightSidePedestrianPathNodes.Insert( afterNodeIdx, entryPoint);
    }
    public Node makeHorizontalEntryPointOnSide( ushort streetSide, 
      float entryHeight) {
      if (streetSide == Defs.right) {
        this.currentMasterNode = Node.pedestrianMasterNode;
        Node entryPoint = Node.makeNodeInPathWithCoordinates( this, 
          new Coordinates( entryHeight, this.rightSidePedestrianPathNodes.
          First().coordinates.y, 0));
        int afterNodeIdx = this.rightSidePedestrianPathNodes.FindIndex( 
          node => node.coordinates.x > entryHeight);
        this.rightSidePedestrianPathNodes[afterNodeIdx - 1].
          switchMutualNeighborsFromTo( this.rightSidePedestrianPathNodes[
          afterNodeIdx], entryPoint);
        Node.makeDoubleLinkBetween( entryPoint, this.
          rightSidePedestrianPathNodes[afterNodeIdx]);
        this.rightSidePedestrianPathNodes.Insert( afterNodeIdx, entryPoint);

        this.currentMasterNode = Node.vehicleMasterNode;
        Node entryPoint = Node.makeNodeInPathWithCoordinates( this, 
          new Coordinates( entryHeight, this.rightSideVehiclePathNodes.
          First().coordinates.y, 0));
        int afterNodeIdx = this.rightSideVehiclePathNodes.FindIndex( 
          node => node.coordinates.x > entryHeight);
        this.rightSideVehiclePathNodes[afterNodeIdx - 1].
          switchNeighborFromTo( this.rightSideVehiclePathNodes[
          afterNodeIdx], entryPoint);
        Node.makeLinkFromTo( entryPoint, this.
          rightSideVehiclePathNodes[afterNodeIdx]);
        this.rightSideVehiclePathNodes.Insert( afterNodeIdx, entryPoint);

      }
      else {
        this.currentMasterNode = Node.pedestrianMasterNode;
        Node entryPoint = Node.makeNodeInPathWithCoordinates( this, 
          new Coordinates( entryHeight, this.leftSidePedestrianPathNodes.
          First().coordinates.y, 0));
        int afterNodeIdx = this.leftSidePedestrianPathNodes.FindIndex( 
          node => node.coordinates.x < entryHeight);
        this.leftSidePedestrianPathNodes[afterNodeIdx - 1].
          switchMutualNeighborsFromTo( this.leftSidePedestrianPathNodes[
          afterNodeIdx], entryPoint);
        Node.makeDoubleLinkBetween( entryPoint, this.
          leftSidePedestrianPathNodes[afterNodeIdx]);
        this.leftSidePedestrianPathNodes.Insert( afterNodeIdx, entryPoint);

        this.currentMasterNode = Node.vehicleMasterNode;
        Node entryPoint = Node.makeNodeInPathWithCoordinates( this, 
          new Coordinates( entryHeight, this.leftSideVehiclePathNodes.
          First().coordinates.y, 0));
        int afterNodeIdx = this.leftSideVehiclePathNodes.FindIndex( 
          node => node.coordinates.x < entryHeight);
        this.leftSideVehiclePathNodes[afterNodeIdx - 1].
          switchNeighborFromTo( this.leftSideVehiclePathNodes[
          afterNodeIdx], entryPoint);
        Node.makeLinkFromTo( entryPoint, this.
          leftSideVehiclePathNodes[afterNodeIdx]);
        this.leftSideVehiclePathNodes.Insert( afterNodeIdx, entryPoint);
      }
    }
    
    public Node makeVerticalEntryPointOnSide( ushort streetSide, float entryHeight) {
      if (streetSide == Defs.right) {
        this.currentMasterNode = Node.pedestrianMasterNode;
        Node entryPoint = Node.makeNodeInPathWithCoordinates( this, 
          new Coordinates( this.rightSidePedestrianPathNodes.
          First().coordinates.x, entryHeight, 0));
        int afterNodeIdx = this.rightSidePedestrianPathNodes.FindIndex( 
          node => node.coordinates.y < entryHeight);
        this.rightSidePedestrianPathNodes[afterNodeIdx - 1].
          switchMutualNeighborsFromTo( this.rightSidePedestrianPathNodes[
          afterNodeIdx], entryPoint);
        Node.makeDoubleLinkBetween( entryPoint, this.
          rightSidePedestrianPathNodes[afterNodeIdx]);
        this.rightSidePedestrianPathNodes.Insert( afterNodeIdx, entryPoint);

        this.currentMasterNode = Node.vehicleMasterNode;
        Node entryPoint = Node.makeNodeInPathWithCoordinates( this, 
          new Coordinates( this.rightSideVehiclePathNodes.
          First().coordinates.x, entryHeight, 0));
        int afterNodeIdx = this.rightSideVehiclePathNodes.FindIndex( 
          node => node.coordinates.y < entryHeight);
        this.rightSideVehiclePathNodes[afterNodeIdx - 1].
          switchNeighborFromTo( this.rightSideVehiclePathNodes[
          afterNodeIdx], entryPoint);
        Node.makeLinkFromTo( entryPoint, this.
          rightSideVehiclePathNodes[afterNodeIdx]);
        this.rightSideVehiclePathNodes.Insert( afterNodeIdx, entryPoint);

      }
      else {
        
        this.currentMasterNode = Node.pedestrianMasterNode;
        Node entryPoint = Node.makeNodeInPathWithCoordinates( this, 
          new Coordinates( this.leftSidePedestrianPathNodes.
          First().coordinates.x, entryHeight, 0));
        int afterNodeIdx = this.leftSidePedestrianPathNodes.FindIndex( 
          node => node.coordinates.y > entryHeight);
        this.leftSidePedestrianPathNodes[afterNodeIdx - 1].
          switchMutualNeighborsFromTo( this.leftSidePedestrianPathNodes[
          afterNodeIdx], entryPoint);
        Node.makeDoubleLinkBetween( entryPoint, this.
          leftSidePedestrianPathNodes[afterNodeIdx]);
        this.leftSidePedestrianPathNodes.Insert( afterNodeIdx, entryPoint);

        this.currentMasterNode = Node.vehicleMasterNode;
        Node entryPoint = Node.makeNodeInPathWithCoordinates( this, 
          new Coordinates( this.leftSideVehiclePathNodes.
          First().coordinates.x, entryHeight, 0));
        int afterNodeIdx = this.leftSideVehiclePathNodes.FindIndex( 
          node => node.coordinates.y > entryHeight);
        this.leftSideVehiclePathNodes[afterNodeIdx - 1].
          switchNeighborFromTo( this.leftSideVehiclePathNodes[
          afterNodeIdx], entryPoint);
        Node.makeLinkFromTo( entryPoint, this.
          leftSideVehiclePathNodes[afterNodeIdx]);
        this.leftSideVehiclePathNodes.Insert( afterNodeIdx, entryPoint);
      }
    }
    public Node makeEntryPointOnSide( ushort streetSide, float entryHeight) {
      if (this.orientation == Defs.horizontal) {
      }
      else {
      }
    } 

    public abstract Dictionary<string,string> dumpProperties(); 

    protected abstract void iterateLifeCycle();
  }  
}