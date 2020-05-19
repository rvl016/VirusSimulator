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

    public static bool virusWillKillHost( uint hostAge, uint hostHealthIndex) {
      return true;
    }
  }
}