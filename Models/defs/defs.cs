namespace VirusSimulatorAvalonia.Models.defs {
    public static class Defs {
        static readonly ushort moving = 0b1;
        static readonly ushort attached = 0b10;
        static readonly ushort incubating = 0b100;
        static readonly ushort open = 0b1000;
        static readonly ushort interacting = 0b10000;
        static readonly ushort onFoot = 0b100000;
        static readonly ushort byCar = 0b1000000;
        static readonly ushort byBus = 0b10000000;
    }
}