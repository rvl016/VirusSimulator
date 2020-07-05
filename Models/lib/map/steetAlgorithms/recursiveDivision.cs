using Troschuetz.Random;
using VirusSimulatorAvalonia.Models.defs;

namespace VirusSimulatorAvalonia.Models.lib.map.streetAlgorithms {

  public sealed class RecursiveDivision : StreetMapAlgorithm {
    
    static TRandom random = new TRandom();
    private short[,] map;
    private ushort currentOrientation;
    private uint currentLowerLimit;
    private uint minSpaceBetweenStreets; 

    public short[,] run( short[,] map, uint minSpaceBetweenStreets) {
      this.map = map;
      this.minSpaceBetweenStreets = minSpaceBetweenStreets;
      if (this.minSpaceBetweenStreets == 0)
        throw new System.Exception( 
          "Recursive division: Minimum space between streets is 1!");
      this.makeSubMapQuadrants( 0, 0, (uint) map.GetUpperBound( 1), 
        (uint) map.GetUpperBound( 0));
      this.makeCrossesOnBounds();
      return this.map;
    }

    private void makeCrossesOnBounds() {
      for (uint i = 0; i <= this.map.GetUpperBound( 1); i++) {
        if (this.map[0,i] == 1) 
          this.map[0,i]++;
        if (this.map[this.map.GetUpperBound( 0),i] > 0) 
          this.map[this.map.GetUpperBound( 0),i]++;
      }
      for (uint i = 0; i <= this.map.GetUpperBound( 0); i++) {
        if (this.map[i,0] == 1) 
          this.map[i,0]++;
        if (this.map[i,this.map.GetUpperBound( 1)] > 0) 
          this.map[i,this.map.GetUpperBound( 1)]++;
      }
    }

    private void makeSubMapQuadrants( uint xInit, uint yInit, uint xEnd, 
      uint yEnd) {
      this.setStreetParameters( Defs.vertical, yInit);
      uint xDivisionPoint = getDivisionPointBetween( xInit, xEnd);
      this.setStreetParameters( Defs.horizontal, xInit);
      uint yDivisionPoint = getDivisionPointBetween( yInit, yEnd);
      drawStreetBetweenAt( Defs.horizontal, xInit, xEnd, yDivisionPoint);
      drawStreetBetweenAt( Defs.vertical, yInit, yEnd, xDivisionPoint);

      // Order matters here! Otherwise checking adjacent street corners may fail.
      makeSubMapQuadrants( xInit, yInit, xDivisionPoint, yDivisionPoint);
      makeSubMapQuadrants( xInit, yDivisionPoint, xDivisionPoint, yEnd);
      makeSubMapQuadrants( xDivisionPoint, yInit, xEnd, yDivisionPoint);
      makeSubMapQuadrants( xDivisionPoint, yDivisionPoint, xEnd, yEnd);
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

    private uint getDivisionPointBetween( uint init, uint end) {
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

    private bool isDivisionPointImplyingAdjacentCorners( int number) {
      if (this.currentOrientation == Defs.horizontal) {
        if (this.map[number + 1,this.currentLowerLimit] > 1 || 
          this.map[number - 1,this.currentLowerLimit] > 1)
          return true;
      }
      else if (this.map[this.currentLowerLimit, number + 1] > 1 || 
        this.map[this.currentLowerLimit, number - 1] > 1)
        return true;
      return false;
    }

    private bool isDivisionPointValidBetween( int number, uint init, uint end) {
      if (number - this.minSpaceBetweenStreets <= init ||
        number + this.minSpaceBetweenStreets >= end)
        return false;
      return ! isDivisionPointImplyingAdjacentCorners( number);
    }

    private void setStreetParameters( ushort orientation, uint lowerLimit) {
      this.currentOrientation = orientation;
      this.currentLowerLimit = lowerLimit;
    }
  }
}