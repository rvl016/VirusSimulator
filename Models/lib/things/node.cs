using System.Collections.Generic;
using VirusSimulatorAvalonia.Models.defs;
using VirusSimulatorAvalonia.Models.things.inanimates.buildings;

namespace VirusSimulatorAvalonia.Models.lib.things {

  public sealed class Node {
    public static Node pedestrianMasterNode = new Node();
    public static Node vehicleMasterNode = new Node();
    public Navegable nodeOwner;
    public List<Node> neighbors;
    public Building entryPoint = null;
    public Coordinates coordinates;

    public static Node makeNodeInPathWithCoordinates( Navegable path, 
      Coordinates coordinates) {
      Node newNode = new Node();
      Node masterNode = path.currentMasterNode;
      masterNode.neighbors.Add( newNode);
      newNode.nodeOwner = path;
      newNode.coordinates = coordinates;
      return newNode;
    } 

    public void switchMutualNeighborsFromTo( Node fromNode, Node toNode) {
      Node.removeDoubleLinkBetween( this, fromNode);
      Node.makeDoubleLinkBetween( this, toNode);
    }

    public void switchNeighborFromTo( Node fromNode, Node toNode) {
      Node.removeLinkFromTo( this, fromNode);
      Node.makeLinkFromTo( this, toNode);
    }

    public static void makeLinkFromTo( Node fromNode, Node toNode) {
      fromNode.neighbors.Add( toNode);
    }

    public static void removeLinkFromTo( Node fromNode, Node toNode) {
      fromNode.neighbors.Remove( toNode);
    }

    public static void makeDoubleLinkBetween( Node nodeOne, Node nodeTwo) {
      makeLinkFromTo( nodeOne, nodeTwo);
      makeLinkFromTo( nodeTwo, nodeOne);
    }

    public static void removeDoubleLinkBetween( Node nodeOne, Node nodeTwo) {
      removeLinkFromTo( nodeOne, nodeTwo);
      removeLinkFromTo( nodeTwo, nodeOne);
    }

    public Node getNeighbourWithDirection( ushort direction) {
      Node node = null;
      if (direction == Defs.right)
        node = this.neighbors.Find( node => (node.coordinates.x - 
          this.coordinates.x) > Consts.floatingPointMargin);
      if (direction == Defs.lower)
        node = this.neighbors.Find( node => (node.coordinates.y - 
          this.coordinates.y) > Consts.floatingPointMargin);
      if (direction == Defs.left)
        node = this.neighbors.Find( node => (node.coordinates.x - 
          this.coordinates.x) < -Consts.floatingPointMargin);
      if (direction == Defs.upper)
        node = this.neighbors.Find( node => (node.coordinates.y - 
          this.coordinates.y) < -Consts.floatingPointMargin);
      return node;
    }

    public static int findIndexOfBreakingPointFromBy( List<Node> fromList, 
      Node byNode) {
      if ()
    }
  }
}