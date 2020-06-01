using VirusSimulatorAvalonia.Models.defs;

namespace VirusSimulatorAvalonia.Models.hidden.god {

  public static class God {

    public static ulong secondsSinceEpoch = 0;
    public static uint worldPopulation = 0;
    public static uint numberOfInfected = 0;
    public static uint numberOfImmune = 0;
    public static uint deaths = 0;

    public static void generateWorld() {

    }
     
    public static void generateThings() {

    }

    public static void destroyWorld() {

    }

    public static uint getCurrentTime() {
      return (uint) (secondsSinceEpoch / Consts.aDayInSeconds);
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