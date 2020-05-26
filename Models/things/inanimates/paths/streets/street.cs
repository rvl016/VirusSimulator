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
    public List<Node> currentPathNodeList;
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

    private Node makeEntryPointNodeFromBy( Coordinates buildingCoordinates) {
      float convexFraction;
      if (this.orientation == Defs.horizontal)
        convexFraction = this.currentPathNodeList.First().coordinates.
          getHorizontalConvexFraction( buildingCoordinates, this.
          currentPathNodeList.Last().coordinates); 
      else    
        convexFraction = this.currentPathNodeList.First().coordinates.
          getVerticalConvexFraction( buildingCoordinates, this.
          currentPathNodeList.Last().coordinates);
      Coordinates nodeCoordinates = this.currentPathNodeList.First().
        coordinates.makeConvexCombination( this.currentPathNodeList.Last().
        coordinates, convexFraction);
      return Node.makeNodeInPathWithCoordinates( this, nodeCoordinates);
    }

    private void insertEntryPointOnCurrentPedestrianNodeList( Node entryPoint, 
      int insertIndex) {
      this.currentPathNodeList[insertIndex - 1].switchMutualNeighborsFromTo( 
        this.currentPathNodeList[insertIndex], entryPoint);
      Node.makeDoubleLinkBetween( entryPoint, 
        this.currentPathNodeList[insertIndex]);
      this.currentPathNodeList.Insert( insertIndex, entryPoint);
    }

    private void insertEntryPointOnCurrentVehicleNodeList( Node entryPoint, 
      int insertIndex) {
      this.currentPathNodeList[insertIndex - 1].switchNeighborFromTo( this.
        currentPathNodeList[insertIndex], entryPoint);
      Node.makeLinkFromTo( entryPoint, this.currentPathNodeList[insertIndex]);
      this.currentPathNodeList.Insert( insertIndex, entryPoint);
    }

    private int getEntryPointInsertionIndex( Coordinates buildingCoordinates) {
        List<Coordinates> pathNodesCoordinates = this.
          currentPathNodeList.Select( node => node.coordinates).ToList();
        int insertIndex = Coordinates.findConsecutivePointsForCovexCobinationOf(
          pathNodesCoordinates, buildingCoordinates, this.orientation);
      return insertIndex;
    }

    public Node makePedestrianEntryPointOnSideFor( ushort streetSide, 
      Coordinates buildingCoordinates) {
      this.currentMasterNode = Node.pedestrianMasterNode;
      if (streetSide == Defs.right)
        this.currentPathNodeList = this.rightSidePedestrianPathNodes; 
      else 
        this.currentPathNodeList = this.leftSidePedestrianPathNodes;
      Node entryPoint = this.makeEntryPointNodeFromBy( buildingCoordinates);
      int insertIndex = this.getEntryPointInsertionIndex( buildingCoordinates);
      this.insertEntryPointOnCurrentPedestrianNodeList( entryPoint, 
        insertIndex);
      return entryPoint;
    }

    public Node makeVehicleEntryPointOnSideFor( ushort streetSide, 
      Coordinates buildingCoordinates) {
      this.currentMasterNode = Node.vehicleMasterNode;
      if (streetSide == Defs.right)
        this.currentPathNodeList = this.rightSideVehiclePathNodes; 
      else 
        this.currentPathNodeList = this.leftSideVehiclePathNodes;
      Node entryPoint = this.makeEntryPointNodeFromBy( buildingCoordinates);
      int insertIndex = this.getEntryPointInsertionIndex( buildingCoordinates);
      this.insertEntryPointOnCurrentVehicleNodeList( entryPoint, 
        insertIndex);
      return entryPoint;
    }

    public abstract Dictionary<string,string> dumpProperties(); 

    protected abstract void iterateLifeCycle();
  }  
}