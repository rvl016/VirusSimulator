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

    public Coordinates makeConvexCombination( Coordinates that, 
      float fraction) {
      float convexX = this.x * fraction + that.x * (1.0f - fraction);
      float convexY = this.y * fraction + that.y * (1.0f - fraction);
      return new Coordinates( convexX, convexY, 0);
    }

    public float getVerticalConvexFraction( 
      Coordinates betweenPoint, Coordinates that) {
      return (betweenPoint.y - that.y) / (this.y - that.y);
    }

    public float getHorizontalConvexFraction( 
      Coordinates betweenPoint, Coordinates that) {
      return (betweenPoint.x - that.x) / (this.x - that.x);
    }

  }
}
