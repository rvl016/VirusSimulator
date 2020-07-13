using System;
using System.Runtime.CompilerServices;
using Troschuetz.Random;
using VirusSimulatorAvalonia.Models.defs;

[assembly: InternalsVisibleTo( "VirusSimulatorTest")]
namespace VirusSimulatorAvalonia.Models.lib.map.streetAlgorithms {

  public sealed class RecursiveDivision : StreetMapAlgorithm {
    
    private short[,] map;
    private uint minSpaceBetweenStreets;

    public short[,] run( short[,] map, uint minSpaceBetweenStreets) {
      this.setUpData( map, minSpaceBetweenStreets);
      this.makeSubMapQuadrantsOn( new Rect( 0, 0, 
        (uint) map.GetUpperBound( 1), (uint) map.GetUpperBound( 0)));
      this.makeCrossesOnBounds();
      return this.map;
    }

    private void makeSubMapQuadrantsOn( Rect rect) {
      if (! rect.canBeDividedWithAtLeast( this.minSpaceBetweenStreets) ||
        ! DivisionPoint.hasDivisionWithNoAdjacentStreets( rect))
        return;
      SimpleCoordinates divisionCoordinates = getDivisionCoordinates( rect);
      drawStreetsAroundIn( divisionCoordinates, rect);
      (Rect firstQuadrant, Rect secondQuadrant, Rect thirdQuadrant,
        Rect fourthQuadrant) = rect.divideAt( divisionCoordinates);
      // Order matters here! Otherwise checking adjacent street corners may fail.
      makeSubMapQuadrantsOn( secondQuadrant);
      makeSubMapQuadrantsOn( thirdQuadrant);
      makeSubMapQuadrantsOn( firstQuadrant);
      makeSubMapQuadrantsOn( fourthQuadrant);
    }

    private void makeCrossesOnBounds() {
      for (uint i = 0; i <= this.map.GetUpperBound( 1); i++) {
        if (this.map[0,i] == 1) 
          this.map[0,i]++;
        if (this.map[this.map.GetUpperBound( 0),i] == 1) 
          this.map[this.map.GetUpperBound( 0),i]++;
      }
      for (uint i = 0; i <= this.map.GetUpperBound( 0); i++) {
        if (this.map[i,0] == 1) 
          this.map[i,0]++;
        if (this.map[i,this.map.GetUpperBound( 1)] == 1) 
          this.map[i,this.map.GetUpperBound( 1)]++;
      }
    }

    private SimpleCoordinates getDivisionCoordinates( Rect rect) {
      DivisionPoint.setStreetParameters( Defs.vertical, rect.lowerBound.y);
      uint xDivisionPoint = DivisionPoint.getDivisionPointBetween( 
        rect.lowerBound.x, rect.upperBound.x);
      DivisionPoint.setStreetParameters( Defs.horizontal, 
        rect.lowerBound.x);
      uint yDivisionPoint = DivisionPoint.getDivisionPointBetween( 
        rect.lowerBound.y, rect.upperBound.y);
      return new SimpleCoordinates( xDivisionPoint, yDivisionPoint);
    }

    private void drawStreetsAroundIn( SimpleCoordinates divisionCoordinates,
      Rect rect) {
      drawStreetBetweenAt( Defs.horizontal, rect.lowerBound.x, 
        rect.upperBound.x, divisionCoordinates.y);
      drawStreetBetweenAt( Defs.vertical, rect.lowerBound.y, 
        rect.upperBound.y, divisionCoordinates.x);
      this.map[divisionCoordinates.y,divisionCoordinates.x] += 2;
    }

    private void drawStreetBetweenAt( ushort orientation, uint init, uint end, 
      uint divisionPoint) {
      for (uint i = init; i <= end; i++) {
        if (orientation == Defs.horizontal)
          this.map[divisionPoint,i]++; 
        else 
          this.map[i,divisionPoint]++;
      }
    }

    private void setUpData( short[,] map, uint minSpaceBetweenStreets) {
      if (minSpaceBetweenStreets == 0)
        throw new Exception( 
          "Recursive division: Minimum space between streets is 1!");
      this.map = map;
      DivisionPoint.map = map;
      this.minSpaceBetweenStreets = minSpaceBetweenStreets;
      DivisionPoint.minSpaceBetweenStreets = minSpaceBetweenStreets;
    }
  }

  internal static class DivisionPoint {

    private static TRandom random = new TRandom();
    public static uint minSpaceBetweenStreets;
    public static ushort currentOrientation;
    public static uint currentLowerLimit;
    public static short[,] map;

    public static uint getDivisionPointBetween( uint init, uint end) {
      float middle = (float) (init + end) / 2.0f;
      float lengthBetween = (float) end - init + 1.0f;
      int divisionPoint;
      do {
        // Why 6.0f ? Because we want most streets next to middle point
        divisionPoint = (int) (random.Normal( middle, lengthBetween / 6.0f) +
         .5d);
      } while (! isDivisionPointValidBetween( divisionPoint, init, end));
      return (uint) divisionPoint;
    }

    public static void setStreetParameters( 
      ushort orientation, uint lowerLimit) {
      currentOrientation = orientation;
      currentLowerLimit = lowerLimit;
    }

    public static bool hasDivisionWithNoAdjacentStreets( Rect rect) {
      return hasHorizDivisionWithNoAdjacentStreets( rect)
        && hasVertDivisionWithNoAdjacentStreets( rect);
    }

    private static bool isDivisionPointValidBetween( 
      int number, uint init, uint end) {
      if (number - minSpaceBetweenStreets <= init ||
        number + minSpaceBetweenStreets >= end)
        return false;
      return ! isDivisionPointImplyingAdjacentCorners( number);
    }

    // Only checks at lower bounds (min x/y)
    private static bool isDivisionPointImplyingAdjacentCorners( int number) {
      if (currentOrientation == Defs.horizontal) {
        if (map[number + 1,currentLowerLimit] > 1 || 
          map[number - 1,currentLowerLimit] > 1)
          return true;
      }
      else if (map[currentLowerLimit,number + 1] > 1 || 
        map[currentLowerLimit,number - 1] > 1)
        return true;
      return false;
    }

    private static bool hasVertDivisionWithNoAdjacentStreets( Rect rect) {
      uint xMin = rect.lowerBound.x + minSpaceBetweenStreets + 1;
      uint xMax = rect.upperBound.x - minSpaceBetweenStreets - 1;
      for (uint x = xMin; x <= xMax; x++) 
        if (map[rect.lowerBound.y,x - 1] < 2 
          && map[rect.lowerBound.y,x + 1] < 2 
          && map[rect.upperBound.y,x - 1] < 2
          && map[rect.upperBound.y,x + 1] < 2)
          return true;
      return false;
    }

    private static bool hasHorizDivisionWithNoAdjacentStreets( Rect rect) {
      uint yMin = rect.lowerBound.y + minSpaceBetweenStreets + 1;
      uint yMax = rect.upperBound.y - minSpaceBetweenStreets - 1;
      for (uint y = yMin; y <= yMax; y++) 
        if (map[y - 1,rect.lowerBound.x] < 2 
          && map[y + 1,rect.lowerBound.x] < 2 
          && map[y - 1,rect.upperBound.x] < 2 
          && map[y + 1,rect.upperBound.x] < 2)
          return true;
      return false;
    }
  }

  internal class Rect {

    public SimpleCoordinates lowerBound;
    public SimpleCoordinates upperBound;

    public Rect( 
      SimpleCoordinates lowerBound, SimpleCoordinates upperBound) {
      this.lowerBound = lowerBound;
      this.upperBound = upperBound;
    }

    public Rect( uint xInit, uint yInit, uint xEnd, uint yEnd) {
      this.lowerBound = new SimpleCoordinates( xInit, yInit);
      this.upperBound = new SimpleCoordinates( xEnd, yEnd);
    }

    public bool canBeDividedWithAtLeast( uint length) {
      return this.upperBound.x - this.lowerBound.x > 2 * length + 1 
        && this.upperBound.y - this.lowerBound.y > 2 * length + 1;
    }

    public (Rect,Rect,Rect,Rect) divideAt( 
      SimpleCoordinates divisionCoordinates) {
      (SimpleCoordinates,SimpleCoordinates) lowerMerge = 
        this.lowerBound.mergeWith( divisionCoordinates);
      (SimpleCoordinates,SimpleCoordinates) upperMerge = 
        divisionCoordinates.mergeWith( this.upperBound);
      Rect secondQuadrant = new Rect( lowerBound, divisionCoordinates);
      Rect thirdQuadrant = new Rect( lowerMerge.Item1, upperMerge.Item1);
      Rect firstQuadrant = new Rect( lowerMerge.Item2, upperMerge.Item2);
      Rect forthQuadrant = new Rect( divisionCoordinates, upperBound);
      return (firstQuadrant, secondQuadrant, thirdQuadrant, forthQuadrant);
    }
  }

  internal class SimpleCoordinates {

    public uint x;
    public uint y;

    public SimpleCoordinates( uint x, uint y) {
      this.x = x;
      this.y = y;
    }

    public (SimpleCoordinates,SimpleCoordinates) mergeWith( 
      SimpleCoordinates that) {
      SimpleCoordinates withThisX = new SimpleCoordinates( this.x, that.y);
      SimpleCoordinates withThisY = new SimpleCoordinates( that.x, this.y);
      return (withThisX, withThisY);
    }
  }

}