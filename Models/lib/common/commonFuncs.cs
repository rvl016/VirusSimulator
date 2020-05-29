using System.Collections.Generic;
using VirusSimulatorAvalonia.Models.defs;

namespace VirusSimulatorAvalonia.Models.lib.common {
  public static class Common {

    public static Dictionary<ushort,Dictionary<char,int>> directionIterable = 
      new Dictionary<ushort, Dictionary<char, int>> {
        { Defs.upper, new Dictionary<char, int> { { 'x', 0 }, { 'y', -1 } } },
        { Defs.lower, new Dictionary<char, int> { { 'x', 0 }, { 'y', 1 } } },
        { Defs.left, new Dictionary<char, int> { { 'x', -1 }, { 'y', 0 } } },
        { Defs.right, new Dictionary<char, int> { { 'x', 1 }, { 'y', 0 } } }
      };
  }

}