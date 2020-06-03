using System;
using System.Linq;
using System.Collections.Generic;
using Troschuetz.Random;
using VirusSimulatorAvalonia.Models.defs;
using VirusSimulatorAvalonia.Models.hidden.god;
using VirusSimulatorAvalonia.Models.hidden.god.world;
using VirusSimulatorAvalonia.Models.things.inanimates.buildings;

namespace VirusSimulatorAvalonia.Models.lib.population.bloom {

  public static class Bloomer {

    private static TRandom random = new TRandom();

    public static void generateWorldAnimates() {
      
    }

    private static void defineBuildingsCapacity() {
      List<Building> buildings = ThingsPackage.getBuildings(); 
      buildings.ForEach( building => building.definePeopleCapacity());
    } 

    private static void unknowmName() {
      uint peopleNum = getPeoplePopulation();
      uint employeesNumber = getEmployeesNumber( peopleNum);
      List<ushort> peopleAges = getPeopleAges( peopleNum);
    }

    private static uint getEmployeesNumber( uint peopleNum) {
      uint maxJobs = Math.Min( getCommercesTotalCapacity(), peopleNum);
      uint maxJobsAllowed = (uint) ((1.0f - God.minUnemploymentRate) * 
        peopleNum);
      return Math.Min( maxJobs, maxJobsAllowed);
    }

    private static List<ushort> getPeopleAges( uint peopleNum) {
      return random.RayleighSamples( 
        10.0d + God.populationAgeFactor * 20.0d).Select( 
        age => (ushort) age).Take( (int) peopleNum).ToList();
    }

    private static uint getPeoplePopulation() {
      uint residencesTotalCapacity = (uint) ThingsPackage.residences.Aggregate(
        0, (acc, residence) => acc + (int) residence.peopleCapacity);
      return (uint) (residencesTotalCapacity * God.worldDensity);
    }

    private static uint getCommercesTotalCapacity() {
      return (uint) ThingsPackage.commerces.Aggregate( 0, 
        (acc, commerce) => acc + (int) commerce.peopleCapacity);
    }
  } 
}