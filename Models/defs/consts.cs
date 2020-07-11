
namespace VirusSimulatorAvalonia.Models.defs {
  public static class Consts {
    public static readonly float personSpeed = 0.0f;
    public static readonly float carBaseSpeed = 0.0f;
    public static readonly float virusBaseRadius = 0.0f;
    public static readonly float personRadius = 0.0f;
    public static readonly float vehicleRadius = 0.0f;
    public static readonly float virusInfectionProbability = 0.005f;
    public static readonly float floatingPointMargin = 1.0e-6f;  
    public static readonly float roadHalfWidth = 20.0f;
    public static readonly float roadWidth = roadHalfWidth * 2.0f;
    public static readonly float road2sidewalkRatio = 4.0f;

    // Time Related
    public static readonly uint retryInterval = 5;
    public static readonly uint aDayInSeconds = 86400;
    
    // Random Generation Related
    public static readonly double randomBuildingFloorsScaleFactor = .25d;
    public static readonly float residenceProportion = .6f;
    public static readonly float commerceProportion = .38f;
    public static readonly float quarentineProportion = .02f;

    // Population Related

    public static readonly float commercePopulationFactor = 4.0f;

    public static readonly float residencePopulationFactor = 1.5f;

    public static readonly float quarentinePopulationFactor = 9.0f;
    
  }
}