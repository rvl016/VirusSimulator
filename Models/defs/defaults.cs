
namespace VirusSimulatorAvalonia.Models.defs {
  
  public static class Defaults {

    // Between 0 and 1.
    public static readonly float minUnemploymentRate = .2f;
    
    // Between 0 and 1.
    public static readonly float worldDensity = .9f;

    // Should be between 0 and 1 for a more realistic age distribution.
    // Watch out for negative values.
    public static readonly float populationAgeFactor = .75f;

    public static readonly ulong timeDelta = 1;
  }
}