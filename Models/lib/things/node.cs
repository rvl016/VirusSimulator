using System.Linq;
using System.Collections.Generic;
using VirusSimulatorAvalonia.Models.defs;
using VirusSimulatorAvalonia.Models.lib.common;
using VirusSimulatorAvalonia.Models.things.inanimates.buildings;

namespace VirusSimulatorAvalonia.Models.lib.things {

  public sealed class Node {

    private sealed class TrackerAnnotations {
      public bool visited = false;
      public bool hasbeenBlocked = false;
    }

    public static Node pedestrianMasterNode = new Node();
    public static Node vehicleMasterNode = new Node();
    public Coordinates coordinates;
    public List<Node> neighbors;
    public Navegable nodeOwner;
    public Building entryPoint = null;
    private Dictionary<uint,TrackerAnnotations> trackersAnnotations = 
      new Dictionary<uint,TrackerAnnotations>();

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

    public void switchMutualNeighborsFromTo( Node fromNode, Node toNode) {
      Node.removeDoubleLinkBetween( this, fromNode);
      Node.makeDoubleLinkBetween( this, toNode);
    }

    public void switchNeighborFromTo( Node fromNode, Node toNode) {
      Node.removeLinkFromTo( this, fromNode);
      Node.makeLinkFromTo( this, toNode);
    }

    public bool isVisited( uint trackerId) {
      if (! this.trackersAnnotations.ContainsKey( trackerId))
        this.trackersAnnotations.Add( trackerId, new TrackerAnnotations());
      return this.trackersAnnotations.
        GetValueOrDefault( trackerId).visited;
    }

    public Node getNeighbourOnDirection( ushort direction) {
      Node node = null;
      int dx = Common.getDxOfDirection( direction);
      int dy = Common.getDyOfDirection( direction);
      node = this.neighbors.FirstOrDefault( node => 
        (node.coordinates.x - this.coordinates.x) * dx > Consts.
        floatingPointMargin || 
        (node.coordinates.y - this.coordinates.y) * dy > Consts.
        floatingPointMargin);
      return node;
    }

    public void setEntryPoint( Building building) {
      this.entryPoint = building;
    }
  }
}