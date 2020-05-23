using Troschuetz.Random;
using VirusSimulatorAvalonia.Models.defs;

namespace VirusSimulatorAvalonia.Models.lib.map.algorithms {

  public class RecursiveDivision : MapAlgorithm {
    
    static TRandom random = new TRandom();
    private short[,] map;
    private uint minSpaceBetweenStreets; 

    private bool isDivisionPointValidBetween( int number, uint init, uint end) {
      return number - this.minSpaceBetweenStreets <= init ||
          number + this.minSpaceBetweenStreets >= end ? false : true;
    }
    private uint getDivisionPoint( uint init, uint end) {
      float middle = (float) (init + end) / 2.0f;
      float lengthBetween = (float) end - init + 1.0f;
      int divisionPoint;
      do {
        // Why 6.0f ? Because we want most streets next to middle point
        divisionPoint = (int) (random.Normal( middle, lengthBetween / 6.0f) +
         .5d);
      } while( ! isDivisionPointValidBetween( divisionPoint, init, end));
      return (uint) divisionPoint;
    }


    private void drawStreetBetweenAt( bool orientation, uint init, uint end, 
      uint divisionPoint) {
      for (uint i = init; i <= end; i++) {
        if (orientation == Consts.horizontal)
          this.map[divisionPoint,i]++; 
        else 
          this.map[i,divisionPoint]++;
      }
    }

    private void makeSubMapQuadrants( uint xInit, uint yInit, uint xEnd, 
      uint yEnd) {
      uint xDivisionPoint = getDivisionPoint( xInit, xEnd);
      uint yDivisionPoint = getDivisionPoint( yInit, yEnd);
      drawStreetBetweenAt( Consts.horizontal, xInit, xEnd, yDivisionPoint);
      drawStreetBetweenAt( Consts.vertical, yInit, yEnd, xDivisionPoint);

      makeSubMapQuadrants( xInit, yInit, xDivisionPoint, yDivisionPoint);
      makeSubMapQuadrants( xInit, yDivisionPoint, xDivisionPoint, yEnd);
      makeSubMapQuadrants( xDivisionPoint, yInit, xEnd, yDivisionPoint);
      makeSubMapQuadrants( xDivisionPoint, yDivisionPoint, xEnd, yEnd);
    }

    public short[,] run( short[,] map, uint minSpaceBetweenStreets) {
      this.map = map;
      this.minSpaceBetweenStreets = minSpaceBetweenStreets;
      makeSubMapQuadrants( 0, 0, (uint) map.GetUpperBound( 1), 
        (uint) map.GetUpperBound( 0));
      highlightBounds();     
      return this.map;
    }
  }
}