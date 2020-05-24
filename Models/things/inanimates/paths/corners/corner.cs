using System;
using System.Collections.Generic;
using VirusSimulatorAvalonia.Models.lib.things;
using VirusSimulatorAvalonia.Models.defs;
using VirusSimulatorAvalonia.Models.things.inanimates.paths;

namespace VirusSimulatorAvalonia.Models.things.inanimates.paths.corner {

  sealed class Corner : Path {

    public List<Node> pedestrianNodes;
    public List<Node> vehicleNodes;

    public Corner( float xCoordinate, float yCoordinate, float halfWidth, Path path = null) : 
      base( xCoordinate, yCoordinate, halfWidth, 
      halfWidth) {
      if (path != null)
        this.endPoints = new List<Path> { path };
      this.makePathNodes();
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
  
    protected override void makePathNodes() {
      this.currentMasterNode = Node.pedestrianMasterNode;
      this.pedestrianNodes = this.makeNodeQuadrature( 
        Node.makeDoubleLinkBetween, this.getSidewalkMiddleRelativeWidth());

      this.currentMasterNode = Node.vehicleMasterNode;
      this.vehicleNodes = this.makeNodeQuadrature( Node.makeLinkFromTo, 
        this.getRoadMiddleRelativeWidth());
    }

    public abstract Dictionary<string,string> dumpProperties(); 
    protected abstract void iterateLifeCycle();
  }
}