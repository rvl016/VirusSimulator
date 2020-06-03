using System.Collections.Generic;
using VirusSimulatorAvalonia.Models.things.inanimates.buildings;

namespace VirusSimulatorAvalonia.Models.lib.things {

  public sealed class Route {

    private List<Node> path;
    private List<Node> backupPath;
    private Building origin;
    private Building destination;

    public static newRoute( Building origin, Building destination)
    public Route( Building origin, Building destination) {
      this.path = new List<Node>();
      this.origin = origin;
      this.destination = destination;
    }

    public void makePathFrom( Node origin, Node destination) {

    }
  }
}