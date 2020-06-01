namespace VirusSimulatorAvalonia.Models.defs {
  public static class Defs {
    // Things derivative classes states
    public static readonly ushort moving = 0b1;
    public static readonly ushort attached = 0b10;
    public static readonly ushort incubating = 0b100;
    public static readonly ushort open = 0b1000;
    public static readonly ushort mandatoryClose = 0b10000;
    public static readonly ushort interacting = 0b100000;

    // People means of transport
    public static readonly ushort onFoot = 0b1;
    public static readonly ushort byCar = 0b10;
    public static readonly ushort byBus = 0b100;

    // Directions and orientations (modify and you may break things!)
    public static readonly ushort horizontal = 0b0;
    public static readonly ushort vertical = 0b1;
    public static readonly ushort right = 0b0;
    public static readonly ushort left = 0b1;
    public static readonly ushort up = 0b10;
    public static readonly ushort down = 0b11;
  }
}