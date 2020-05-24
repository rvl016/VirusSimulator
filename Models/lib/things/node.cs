using System.Collections.Generic;
using VirusSimulatorAvalonia.Models.defs;

namespace VirusSimulatorAvalonia.Models.lib.things {

  public sealed class Node {
    public static Node pedestrianMasterNode = new Node();
    public static Node vehicleMasterNode = new Node();
    public Navegable nodeOwner;
    public List<Node> neighbors;
    public Coordinates coordinates;

    private Node() {

    }

    public static Node makeNodeInPathWithCoordinates( Navegable path, 
      Coordinates coordinates) {
      Node newNode = new Node();
      Node masterNode = path.currentMasterNode;
      masterNode.neighbors.Add( newNode);
      newNode.nodeOwner = path;
      newNode.coordinates = coordinates;
      return newNode;
    } 

    public static void makeLinkFromTo( Node fromNode, Node toNode) {
      fromNode.neighbors.Add( toNode);
    }

    public static void makeDoubleLinkBetween( Node nodeOne, Node nodeTwo) {
      makeLinkFromTo( nodeOne, nodeTwo);
      makeLinkFromTo( nodeTwo, nodeOne);
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
  }
}