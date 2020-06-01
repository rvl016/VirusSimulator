using VirusSimulatorAvalonia.Models.things.inanimates.buildings;

namespace VirusSimulator.Models.lib.things {

  public interface Buildable<BuildingType>
    where BuildingType : Building {
    void create( uint[] startCoordinates, uint[] endCoordinates, 
      uint[] doorCoordinates) {

    } 
  }
}