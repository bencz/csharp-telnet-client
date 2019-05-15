using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AutoCoder.Windows.Lines;
using AutoCoder.Core.Enums;

namespace AutoCoder.Windows.Triangles
{
  /// <summary>
  /// the 3 TriangleLine lines which make up a triangle.
  /// </summary>
  public class TriangleLines : List<TriangleLine>
  {
    public TriangleLines( 
      LineCoordinates Side1, LineCoordinates Side2, LineCoordinates Side3 )
    {
      {
        var line = new TriangleLine() { SideName = "Side1", LineCoor = Side1 };
        this.Add(line);
      }

      {
        var line = new TriangleLine() { SideName = "Side2", LineCoor = Side2 };
        this.Add(line);
      }

      {
        var line = new TriangleLine() { SideName = "Side3", LineCoor = Side3 };
        this.Add(line);
      }
    }

    public TriangleVertex OppositeVertex(TriangleLine TriLine)
    {
      var startCoor = TriLine.StartLine.LineCoor;
      var endCoor = TriLine.EndLine.LineCoor;
      var loc = LineCoordinates.CommonEndPoint(startCoor, endCoor) ;
      var angle = LineCoordinates.AngleBetween(startCoor, endCoor);
      
      var oppVertex = new TriangleVertex()
      {
        Angle = angle,
        Line = TriLine.LineCoor,
        Location = loc
      };

      return oppVertex;
    }

    public bool TryMatch()
    {
      bool gotMatch = true;
      foreach (var triLine in this)
      {
        gotMatch = TryMatch(triLine, WhichEndPoint.Start);
        if (gotMatch == true)
        {
          gotMatch = TryMatch(triLine, WhichEndPoint.End);
        }
        if (gotMatch == false)
          break;
      }
      return gotMatch;
    }

    public bool TryMatch( TriangleLine TriLine, WhichEndPoint WhichEnd)
    {
      bool gotMatch = false ;
      foreach( var otherLine in this)
      {
        if ( gotMatch == true )
          break; 
        if ( TriLine == otherLine )
          continue ;
        else if ( WhichEnd == WhichEndPoint.Start )
        {
          if (TriLine.StartLine != null)
          {
            gotMatch = true;
            break;
          }

          if ((otherLine.StartLine == null)
            && (TriLine.LineCoor.Start.Equals(otherLine.LineCoor.Start)))
          {
            otherLine.StartLine = TriLine;
            TriLine.StartLine = otherLine;
            gotMatch = true;
          }

          else if ((otherLine.EndLine == null)
            && (TriLine.LineCoor.Start.Equals(otherLine.LineCoor.End)))
          {
            TriLine.StartLine = otherLine;
            otherLine.EndLine = TriLine;
            gotMatch = true;
          }
        }
        else if ( WhichEnd == WhichEndPoint.End)
        {
          if (TriLine.EndLine != null)
          {
            gotMatch = true;
            break;
          }

          if ((otherLine.StartLine == null)
            && (TriLine.LineCoor.End.Equals(otherLine.LineCoor.Start)))
          {
            otherLine.StartLine = TriLine;
            TriLine.EndLine = otherLine;
            gotMatch = true;
          }

          else if ((otherLine.EndLine == null)
            && (TriLine.LineCoor.End.Equals(otherLine.LineCoor.End)))
          {
            TriLine.EndLine = otherLine;
            otherLine.EndLine = TriLine;
            gotMatch = true;
          }

        }
      }
      return gotMatch ;
    }

  }
}
