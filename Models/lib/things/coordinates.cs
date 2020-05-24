using System;

namespace VirusSimulatorAvalonia.Models.lib.things {
  public sealed class Coordinates {
    public float x;
    public float y;
    public ushort z;

    public Coordinates( float xCoordinate, float yCoordinate, ushort zCoordinate) {
      this.x = xCoordinate;
      this.y = yCoordinate;
      this.z = zCoordinate;
    }

    public float getDistance( Coordinates that) {
      if (this.z != that.z)
        return float.MaxValue;
      return (float) Math.Sqrt( (float) Math.Pow( this.x - that.x, 2) + 
        (float) Math.Pow( this.y - that.y, 2));
    }

    public Coordinates getRelativeCoordinates( float deltaX, float deltaY) {
      return new Coordinates( this.x + deltaX, this.y + deltaY, this.z);
    }
  }
}
