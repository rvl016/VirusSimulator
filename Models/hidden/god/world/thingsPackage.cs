using System.Linq;
using System.Collections.Generic;
using VirusSimulatorAvalonia.Models.things.inanimates.paths.corner;
using VirusSimulatorAvalonia.Models.things.inanimates.paths.street;
using VirusSimulatorAvalonia.Models.things.inanimates.buildings;
using VirusSimulatorAvalonia.Models.things.inanimates.buildings.quarentine;
using VirusSimulatorAvalonia.Models.things.inanimates.buildings.commerce;
using VirusSimulatorAvalonia.Models.things.inanimates.buildings.residence;
using VirusSimulatorAvalonia.Models.things.animates.vehicle;
using VirusSimulatorAvalonia.Models.things.virus;
using VirusSimulatorAvalonia.Models.things.animates.people;

namespace VirusSimulatorAvalonia.Models.hidden.god.world {

  public static class ThingsPackage {

    public static List<Person> people = new List<Person>();
    public static List<Virus> viruses = new List<Virus>();
    public static List<Vehicle> vehicles = new List<Vehicle>();
    public static List<Residence> residences = new List<Residence>();
    public static List<Commerce> commerces = new List<Commerce>();
    public static List<Quarentine> quarentines = new List<Quarentine>();
    public static List<Street> streets = new List<Street>();
    public static List<Corner> corners = new List<Corner>();

    public static void add( Person person) {
      people.Add( person);
      God.worldPopulation++;
    }

    public static void add( Virus virus) {
      viruses.Add( virus);
      God.numberOfInfected++;
    }

    public static void add( Vehicle vehicle) {
      vehicles.Add( vehicle);
    }

    public static void add( Residence residence) {
      residences.Add( residence);
    }

    public static void add( Commerce commerce) {
      commerces.Add( commerce);
    }

    public static void add( Quarentine quarentine) {
      quarentines.Add( quarentine);
    }

    public static void add( Street street) {
      streets.Add( street);
    }

    public static void add( Corner corner) {
      corners.Add( corner);
    }

    public static void remove( Person person) {
      people.Remove( person);
      God.deaths++;
      God.worldPopulation--;    
    }

    public static void remove( Virus virus) {
      viruses.Remove( virus);
      God.numberOfInfected--;    
    }

    public static void removeAll() {
      people.Clear();
      viruses.Clear();
      vehicles.Clear();
      residences.Clear();
      commerces.Clear();
      quarentines.Clear();
      streets.Clear();
      corners.Clear();   
    }

    public static List<Building> getBuildings() {
      return residences.Cast<Building>().Concat( commerces).ToList().
        Concat( quarentines).ToList();
    }

  }
} 