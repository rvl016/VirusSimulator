// Unused...
namespace VirusSimulatorAvalonia.Models.lib.map.protoStructs {
  public class Node {
    public uint xCoordinate;
    public uint yCoordinate;

  }

  public class Edge {
    public uint xCoordinate;
    public uint yCoordinate;
    public ushort orientation;
    public Node upperNode;
  }
}