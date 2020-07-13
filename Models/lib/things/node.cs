using System;
using System.Linq;
using System.Collections.Generic;
using VirusSimulatorAvalonia.Models.defs;
using VirusSimulatorAvalonia.Models.lib.common;
using VirusSimulatorAvalonia.Models.things.inanimates.buildings;

namespace VirusSimulatorAvalonia.Models.lib.things {

  public sealed class Node {

    private sealed class TrackerAnnotations {
      public bool visited = false;
      public Node next = null;
      public Node previous = null;
    }

    public static Node pedestrianMasterNode = new Node();
    public static Node vehicleMasterNode = new Node();
    public Coordinates coordinates;
    public List<Node> neighbors = new List<Node>();
    public Navegable nodeOwner;
    public Building entryPoint = null;
    public List<Building> isPathToBuilding = new List<Building>();
    private Dictionary<Building,Node> destination2Neighbor = 
      new Dictionary<Building,Node>();
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

    public bool isVisited( uint routeId) {
      if (! this.trackersAnnotations.ContainsKey( routeId))
        this.trackersAnnotations.Add( routeId, new TrackerAnnotations());
      return this.trackersAnnotations.
        GetValueOrDefault( routeId).visited;
    }

    public void setVisited( uint routeId, bool isVisited = true) {
      this.trackersAnnotations.GetValueOrDefault( routeId).visited = isVisited;
    }

    public void setNextNeighbor( uint routeId, Node neighbor) {
      this.trackersAnnotations.GetValueOrDefault( routeId).next = neighbor;
      neighbor.trackersAnnotations.GetValueOrDefault( routeId).previous = this;
    }

    public Node getNextNeighbor( uint routeId) {
      return this.trackersAnnotations.GetValueOrDefault( routeId).next;
    }

    public Node getPreviousNeighbor( uint routeId) {
      return this.trackersAnnotations.GetValueOrDefault( routeId).previous;
    }

    public void dupAnnotationFromTo( uint fromRoute, uint toRoute) {
      this.trackersAnnotations.Add( toRoute, 
        this.trackersAnnotations.GetValueOrDefault( fromRoute));
    }

    public Node getNeighbourOnDirection( ushort direction) {
      int dx = Common.getDxOfDirection( direction);
      int dy = Common.getDyOfDirection( direction);
      return this.neighbors.FirstOrDefault( node => 
        (node.coordinates.x - this.coordinates.x) * dx > Consts.
        floatingPointMargin || 
        (node.coordinates.y - this.coordinates.y) * dy > Consts.
        floatingPointMargin);
    }

    public void addDestination( Building destination, Node neighbor) {
      if (this.destination2Neighbor.GetValueOrDefault( destination) == null) 
        this.destination2Neighbor.Add( destination, neighbor);
    }

    public Node getNextNodeToDestination( Building destination) {
      return this.destination2Neighbor.GetValueOrDefault( destination);
    }

    public Node getNextNodeOnRoute( uint routeId) {
      return this.trackersAnnotations.GetValueOrDefault( routeId).next;
    }

    public static void makeNewRouteAnnotation( uint routeId, Node masterNode) {
      masterNode.neighbors.ForEach( node => 
        node.trackersAnnotations.Add( routeId, new TrackerAnnotations()));
    }

    public static void destroyRouteAnnotations( uint routeId, Node masterNode) {
      masterNode.neighbors.ForEach( node =>
        node.trackersAnnotations.Remove( routeId));
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

    public void setEntryPoint( Building building) {
      this.entryPoint = building;
    }
  }
}