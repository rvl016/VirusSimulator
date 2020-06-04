using System.Linq;
using System.Collections.Generic;
using VirusSimulatorAvalonia.Models.defs;
using VirusSimulatorAvalonia.Models.things.inanimates.buildings;

namespace VirusSimulatorAvalonia.Models.lib.things {

  public sealed class Route {

    private static 
      Dictionary<(Building,Building,ushort),List<Route>> allRoutes;
    private readonly Node masterNode;
    private ushort transport;
    private Node currentNode = null;
    private Building origin;
    private Node originNode;
    private Building destination;
    private static uint maxId = 0;
    private uint id; 
    private uint users = 0;
    private bool isDead = false;
    private bool isReady = false;

    public static Route newRouteBetweenBy( Building origin, 
      Building destination, ushort transport) {
      if (getRouteBetweenBy( origin, destination, transport) == null) 
        return new Route( origin, destination, transport);
      return getRouteBetweenBy( origin, destination, transport);
    } 

    private Route( Building origin, Building destination, ushort transport) {
      this.origin = origin;
      this.destination = destination;
      this.id = maxId++;
      this.transport = transport;
      this.masterNode = transport == Defs.onFoot ? Node.pedestrianMasterNode :
        Node.vehicleMasterNode;
      allRoutes.Add( (origin, destination, transport), new List<Route>() { 
        this });
      this.originNode = transport == Defs.onFoot ? 
        this.origin.sidewalkEntryPoint : this.origin.streetEntryPoint;
    }

    public bool tryToMakePathFrom( Node origin) {
      return findPathFrom( origin);
    }

    public Route requestAlternativeRoute() {
      if (getAllAlternativeRoutes().Last() != this)
        return getAllAlternativeRoutes().Last();
      return redrawRouteFromCurrentNode();
    }

    public Route redrawRouteFromCurrentNode() {
      Route newRoute = makeRouteWithSameParameters();
      dupRouteUntilCurrentNodeOn( newRoute);
      newRoute.findPathFrom( this.currentNode.getPreviousNeighbor( this.id));
      return newRoute;
    }

    private Route makeRouteWithSameParameters() {
      Route newRoute = new Route( this.origin, this.destination, 
        this.transport);
      getAllAlternativeRoutes().Append( newRoute);
      return newRoute;
    }

    private void dupRouteUntilCurrentNodeOn( Route newRoute) {
      Node node = this.originNode;
      for (; node != this.currentNode; node = node.getNextNeighbor( this.id))
        node.dupAnnotationFromTo( this.id, newRoute.id);
    }

    private List<Route> getAllAlternativeRoutes() {
      return allRoutes.GetValueOrDefault( (this.origin, this.destination, 
        this.transport));
    }

    public Node startRoute() {
      if (! this.isReady) {
        findPathFrom( this.originNode);
        this.isReady = true;
      }
      this.currentNode = this.originNode;
      this.users++;
      return iterateThroughRoute(); 
    }

    public Node getNextNodeOnRoute() {
      if (this.currentNode == null)
        return null;
      return iterateThroughRoute();
    }

    private Node iterateThroughRoute() {
      this.currentNode = this.currentNode.getNextNeighbor( this.id);
      if (this.currentNode == null)
        this.users--;
      return this.currentNode;
    }

    // Whenever you will start using a route, you must call it before anything.
    public Route callRoute() {
      if (this.isDead)
        return getRouteBetweenBy( this.origin, this.destination, 
          this.transport);
      return this;
    }
    // Because of the following:
    public static void destroyOldRoutes() {
      foreach (List<Route> routes in allRoutes.Values) 
        while (routes.Count() > 1 && routes.First().users == 0) 
          destroyFirstRouteFrom( routes);
    }

    public static Route getRouteBetweenBy( Building origin, 
      Building destination, ushort transport) {
      return allRoutes.GetValueOrDefault( (origin, destination, transport)).
        LastOrDefault();
    }

    private static void destroyFirstRouteFrom( List<Route> routes) {
      Route toBeDeleted = routes.First();
      Node.destroyRouteAnnotations( toBeDeleted.id, toBeDeleted.masterNode);
      toBeDeleted.isDead = true;
      routes.RemoveAt( 0);
    }

    private bool findPathFrom( Node origin) {
      origin.setVisited( this.id);
      if (origin.nodeOwner == this.destination) 
        return true;
      List<Node> validNeighbors = getValidNeighborsNodesFrom( origin);
      foreach (Node neighbor in validNeighbors)
        if (findPathFrom( neighbor)) {
          origin.setNextNeighbor( this.id, neighbor);
          return true;
        }
      return false;
    }

    private List<Node> getValidNeighborsNodesFrom( Node origin) {
      List<ushort> directions = getCloserDirectionsToDestinationFrom( origin);
      List<Node> neighbors = new List<Node>();
      foreach (ushort direction in directions) {
        Node neighbor = origin.getNeighbourOnDirection( direction);
        if (neighbor != null && ! neighbor.isVisited( this.id) && 
          neighbor.nodeOwner.isOpen)
          neighbors.Add( neighbor);
      }
      return neighbors;
    }

    private List<ushort> getCloserDirectionsToDestinationFrom( Node origin) {
      return origin.coordinates.getCloserDirectionsTo( 
        this.destination.coordinates);     
    }
  }
}