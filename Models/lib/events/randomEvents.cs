namespace VirusSimulatorAvalonia.Models.lib.events {
  public static class RandomEvents {

    public static uint getPeopleMutualInteractionTime( bool areFriends) {
      //TODO: Make this random
      if (areFriends)
        return 10;
      return 2;
    }

    public static bool bothWillInteractTogether( bool areFriends) {
      //TODO: Make this random
      if (areFriends)
        return true;
      return false;
    }

    // Virus Related
    public static bool virusWillKillHost( ushort hostAge, float hostHealthIndex) {
      //TODO: Make this random
      return true;
    }

    public static uint getVirusIncubationTimeByAgeAndHealthIdx( 
      ushort hostAge, float hostHealthIndex) {
      //TODO: Make this random
      return 10;
    }

    public static uint howManySecsSpreadingWillLastByAgeAndHealthIdx(
      ushort hostAge, float hostHealthIndex) {
      //TODO: Make this random
      return 10;
    }
    
    public static bool virusWillKillHostByAgeAndHealthIdx(
      ushort hostAge, float hostHealthIndex) {
      //TODO: Make this random
      return true;
    }

    public static bool virusWillInfectPerson() {
      //TODO: Make this random
      return true;
    }
  }
}