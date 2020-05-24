using System;
using System.Linq;
using System.Collections.Generic;
using VirusSimulatorAvalonia.Models.defs;
using VirusSimulatorAvalonia.Models.lib.things;

using VirusSimulatorAvalonia.Models.things.inanimates.paths;

namespace VirusSimulatorAvalonia.Models.things.inanimates.paths.street {
  sealed class Street : Path {

    public ushort orientation;
    public List<Node> rightSideVehiclePathNodes;
    public List<Node> rightSidePedestrianPathNodes;
    public List<Node> leftSidePedestrianPathNodes;
    public List<Node> leftSideVehiclePathNodes;

    public Street( float xCoordinate, float yCoordinate, float halfWidth, 
      float halfHeight, ushort orientation, Path path = null) : 
      base( xCoordinate, yCoordinate, halfWidth, halfHeight) {
      if (path != null)
        this.endPoints = new List<Path> { path };
      this.orientation = orientation;
      if (this.orientation == Defs.horizontal)
        this.makeNodeQuadrature = this.makeHorizontalNodeQuadrature;
      else
        this.makeNodeQuadrature = this.makeVerticalNodeQuadrature;
      this.makePathNodes();
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
      linkFunction( northeastNode, northwestNode);
      linkFunction( southwestNode, southeastNode);
      return new [] { new List<Node> { southeastNode, southwestNode }, 
        new List<Node> { northwestNode, northeastNode } };
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

    public abstract Dictionary<string,string> dumpProperties(); 

    protected abstract void iterateLifeCycle();
  }  
}