namespace VirusSimulatorAvalonia.Models.lib.things {

  public interface Navegable {
    Node currentMasterNode { 
      get; 
    }

    bool isOpen {
      get;
    }
  }
}