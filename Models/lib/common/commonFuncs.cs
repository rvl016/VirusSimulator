using System.Collections.Generic;
using VirusSimulatorAvalonia.Models.defs;

namespace VirusSimulatorAvalonia.Models.lib.common {
  public static class Common {

    public static Dictionary<ushort,Dictionary<char,int>> directionIterable = 
      new Dictionary<ushort, Dictionary<char, int>> {
        { Defs.up, new Dictionary<char, int> { { 'x', 0 }, { 'y', -1 } } },
        { Defs.down, new Dictionary<char, int> { { 'x', 0 }, { 'y', 1 } } },
        { Defs.left, new Dictionary<char, int> { { 'x', -1 }, { 'y', 0 } } },
        { Defs.right, new Dictionary<char, int> { { 'x', 1 }, { 'y', 0 } } }
      };

    public static List<ushort> clockwiseDirections = new List<ushort> {
      Defs.right, Defs.down, Defs.left, Defs.up
    };

    public static int getDxOfDirection( ushort direction) {
      return directionIterable.GetValueOrDefault( direction).
        GetValueOrDefault( 'x');
    }

    public static int getDyOfDirection( ushort direction) {
      return directionIterable.GetValueOrDefault( direction).
        GetValueOrDefault( 'y');
    }

    public static ushort getOppositeDirection( ushort direction) {
      return getNextClockwiseDirection( direction, 2);
    }

    public static ushort getNextClockwiseDirection( ushort direction, 
      uint di = 1) {
      int idx = clockwiseDirections.FindIndex( dir => dir == direction);
      return clockwiseDirections[(idx + (int) di) % 4];
    }

  }

}