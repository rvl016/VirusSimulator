using System;
using System.Collections.Generic;
using VirusSimulatorAvalonia.Models.defs;
using VirusSimulatorAvalonia.Models.lib.common;

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

    // Only on same x,y plane
    public float getDistance( Coordinates that) {
      if (this.z != that.z)
        return float.MaxValue;
      return (float) Math.Sqrt( (float) Math.Pow( this.x - that.x, 2) + 
        (float) Math.Pow( this.y - that.y, 2));
    }

    public Coordinates getRelativeCoordinates( float deltaX, float deltaY) {
      return new Coordinates( this.x + deltaX, this.y + deltaY, this.z);
    }

    public List<ushort> getCloserDirectionsTo( Coordinates that) {
      int dx = (int) (that.x - this.x);
      int dy = (int) (that.y - this.y);
      return Common.getCloserDirectionsToDelta( dx, dy);
    }

    public bool isOn( Coordinates that) {
      return this.getDistance( that) < Consts.floatingPointMargin;
    }

    public ushort getRelativeSideOnAxisTo( ushort axis, Coordinates that) {
      if (axis == Defs.horizontal ? this.y > that.y : this.x > that.x)
        return Defs.left;
      return Defs.right;
    }

    public Coordinates makeConvexCombination( Coordinates that, 
      float fraction) {
      float convexX = this.x * fraction + that.x * (1.0f - fraction);
      float convexY = this.y * fraction + that.y * (1.0f - fraction);
      return new Coordinates( convexX, convexY, 0);
    }

    public static int findConsecutivePointsForCovexCobinationOf( 
      List<Coordinates> list, Coordinates ofPoint, ushort direction) {
      for (int i = 0; i < list.Count - 1; i++) 
        if (list[i].validateConvexFractionFromOn( list[i + 1], ofPoint, 
          direction))
          return i + 1;
      throw new Exception( 
        "New point is not a convex combination of any from the list!");
    }

    public bool validateConvexFractionFromOn( Coordinates that, 
      Coordinates betweenPoint, ushort direction) {
      float convexFraction;
      if (direction == Defs.horizontal)
        convexFraction = this.getHorizontalConvexFraction( betweenPoint, that);
      else
        convexFraction = this.getVerticalConvexFraction( betweenPoint, that);
      return convexFraction >= .0f && convexFraction <= 1.0f;
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
