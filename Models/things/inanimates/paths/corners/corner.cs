using System.Collections.Generic;
using VirusSimulatorAvalonia.Models.lib.things;
using VirusSimulatorAvalonia.Models.defs;
using VirusSimulatorAvalonia.Models.things.inanimates.paths;

namespace VirusSimulatorAvalonia.Models.things.inanimates.paths.corner {

  sealed class Corner : Path {


    Corner( float xCoordinate, float yCoordinate, float halfWidth,
      List<Path> streets) : base( xCoordinate, yCoordinate, halfWidth, 
      halfWidth) {
      streets.ForEach( street => this.makeEndPointOn( street));

      this.currentMasterNode = null;
    }
    public abstract Dictionary<string,string> dumpProperties(); 

    protected abstract void iterateLifeCycle();
  }
}