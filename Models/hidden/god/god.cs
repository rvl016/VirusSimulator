using VirusSimulatorAvalonia.Models.defs;

using VirusSimulatorAvalonia.Models.lib.schedule;

using VirusSimulatorAvalonia.Models.things.animates.people;

namespace VirusSimulatorAvalonia.Models.hidden.god {

  public static class God {

    public static ulong secondsSinceEpoch = 0;
    public static uint worldPopulation = 0;
    public static uint numberOfInfected = 0;
    public static uint numberOfImmune = 0;
    public static uint deaths = 0;
    public static float minUnemploymentRate = Defaults.minUnemploymentRate;
    public static float worldDensity = Defaults.worldDensity;
    public static float populationAgeFactor = Defaults.populationAgeFactor;
    public static ulong timeDelta = Defaults.timeDelta;

    public static void generateWorld() {

    }
     
    public static void generateThings() {

    }

    public static void destroyEveryLife() {

    }

    public static void destroyWorld() {

    }
    
    public static void announceDeath( Person person) {
      // TODO: Echo death as event
      worldPopulation--;
    }

    public static void announceNewImmune( Person person) {
      numberOfImmune++;
      numberOfInfected--;
      // TODO: Echo new immune as event
    }

    public static void iterateWorldThroughTimeDelta() {
      ulong timeDeltaLeft = timeDelta;
      for (; timeDeltaLeft > 0; timeDeltaLeft--) 
        Scheduler.generateNextFrame();
    }

    public static uint getCurrentTime() {
      return (uint) (secondsSinceEpoch / Consts.aDayInSeconds);
    }

    public static ulong getTodayMidNight() {
      ulong thisDay = secondsSinceEpoch / Consts.aDayInSeconds; 
      return thisDay * Consts.aDayInSeconds;
    }

    public static ulong getTomorrowMidNight() {
      return getTodayMidNight() + Consts.aDayInSeconds;
    }

    
    // It must hold all parameters of the universe and all universe 
    //   rely on then, with except of the dictator, that is above 
    //   god's concern. (He actually may define god's universal parameters)
    // it should be directly interfaced with ViewModel for output

    // Parameters related to inanimates
      // Streets
        // conditional forbidden
          // rule that vehicle should follow 
      // Buildings 
        // comercial / industrial
          // allowed business time
    // Parameters related to animates
      // Virus
        // virulency
        // incubation window
        // death factor
      // People
        // in strict quarantine
        // mandatory mask usage
  }
}