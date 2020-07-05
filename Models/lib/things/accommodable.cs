using System.Collections.Generic;
using VirusSimulatorAvalonia.Models.things.animates.people;
using VirusSimulatorAvalonia.Models.things.animates.vehicles;

namespace VirusSimulatorAvalonia.Models.lib.things {
  public interface Accommodable {

    List<Accommodable> endPoints { get; }
    Coordinates coordinates { get; }
    bool canAccommodate( Person person);
    bool canAccommodate( Vehicle vehicle);
    void host( Person person);
    void host( Vehicle vehicle);
    void eject( Person person);
    void eject( Vehicle vehicle);
    List<Person> getPeopleNextTo( Person person);
    List<Vehicle> getVehiclesNextTo( Vehicle vehicle);
    
  }
}