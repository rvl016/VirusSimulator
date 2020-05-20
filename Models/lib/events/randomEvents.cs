namespace VirusSimulatorAvalonia.Models.lib.events {
  public static class RandomEvents {

    public static uint getPeopleMutualInteractionTime( bool areFriends) {
      if (areFriends)
        return 10;
      return 2;
    }

    public static bool bothWillInteractTogether( bool areFriends) {
      if (areFriends)
        return true;
      return false;
    }

    // Virus Related
    public static bool virusWillKillHost( ushort hostAge, float hostHealthIndex) {
      return true;
    }

    public static uint getVirusIncubationTimeByAgeAndHealthIdx( 
      ushort hostAge, float hostHealthIndex) {
      return 10;
    }

    public static uint howManySecsSpreadingWillLastByAgeAndHealthIdx(
      ushort hostAge, float hostHealthIndex) {
      return 10;
    }
    
    public static bool virusWillKillHostByAgeAndHealthIdx(
      ushort hostAge, float hostHealthIndex) {
      return true;
    }
  }
}