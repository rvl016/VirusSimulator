using System.Linq;
using System.Collections.Generic;

namespace VirusSimulatorAvalonia.Models.lib.things {

  public sealed class Node {
    public static Node pedestrianMasterNode = new Node();
    public static Node vehicleMasterNode = new Node();
    public Navegable nodeOwner;
    public List<Node> neighbors;
    public Coordinates coordinates;

    private Node() {

    }

    public Node makeNodeInPathOnCoordinates( Navegable path, float xCoordinate, 
      float yCoordinate) {
      Node newNode = new Node();
      Node masterNode = path.currentMasterNode;
      masterNode.neighbors.Append( newNode);
      this.coordinates = new Coordinates( xCoordinate, yCoordinate, 0);
      return newNode;
    } 

    public void makeLinkFromTo( Node fromNode, Node toNode) {
      fromNode.neighbors.Append( toNode);
    }

    public void makeDoubleLink( Node nodeOne, Node nodeTwo) {
      makeLinkFromTo( nodeOne, nodeTwo);
      makeLinkFromTo( nodeTwo, nodeOne);
    }
  }
}