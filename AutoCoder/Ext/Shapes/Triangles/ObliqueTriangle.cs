using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using AutoCoder.Ext.Shapes.LineClasses;
using AutoCoder.Windows.Lines;
using AutoCoder.Windows.Triangles;
using AutoCoder.Windows.Primitives;

namespace AutoCoder.Ext.Shapes.Triangles
{
  public class ObliqueTriangle
  {

    private LineCoordinates _Side3;
    public LineCoordinates Side3
    {
      get
      {
        if (_Side3 == null)
        {
          if ((_Vertex1 == null) || (_Vertex2 == null))
            throw new ApplicationException("need Vertex1 and Vertex2 to calc Side3");
          _Side3 = new LineCoordinates(this.Vertex1.Location, this.Vertex2.Location);
        }
        return _Side3;
      }
    }

    TriangleVertex _Vertex1 ;
    public TriangleVertex Vertex1
    {
      get 
      { 
        return _Vertex1; 
      }
      set
      {
        _Vertex1 = value;
      }
    }

    TriangleVertex _Vertex2;
    public TriangleVertex Vertex2
    {
      get
      {
        return _Vertex2;
      }
      set
      {
        _Vertex2 = value;
      }
    }

    TriangleVertex _Vertex3;
    public TriangleVertex Vertex3
    {
      get
      {
        // calc the 3rd vertex from the other two.
        if ((_Vertex3 == null) && ( _Vertex2 != null ) && (_Vertex1 != null ))
        {

          // the angle of this 3rd vertex
          double angle3 = 180 - Vertex1.Angle - Vertex2.Angle;

          // the location of this vertex is the common end point of the lines opposite
          // the two other vertices.
          var loc = LineCoordinates.CommonEndPoint(Vertex1.Line, Vertex2.Line);

          // the LineCoor of the opposite line.
          var ep1 = Vertex1.Line.OtherPoint(loc);
          var ep2 = Vertex2.Line.OtherPoint(loc);
          var oppLine = new LineCoordinates(ep1, ep2);

          _Vertex3 = new TriangleVertex()
          {
            Angle = angle3,
            Location = loc,
            Line = oppLine 
          };
        }
        return _Vertex3;
      }
      set
      {
        _Vertex3 = value;
      }
    }

    /// <summary>
    /// compute the length of side 3 from the angle of side 3 and the length of side 1
    /// and length of side 2.
    /// </summary>
    /// <param name="Side1Lgth"></param>
    /// <param name="Side2Lgth"></param>
    /// <param name="Vertex3Angle"></param>
    /// <returns></returns>
    public static double CalcSideFromSideSideAngle(
      double Side1, double Side2, Angle360 Vertex3)
    {
      // formula:
      // side3 * side3 = (side1 * side1) + (side2 * side2) -
      //                 (( 2 * side1 * side2 ) * cosine angle3)

      var cos3 = Vertex3.Radians.Cos.Value;

      var side3 = Math.Sqrt((Side1 * Side1) + (Side2 * Side2) -
        ((2 * Side1 * Side2) * cos3));

      return side3;
    }

    /// <summary>
    /// compute the length of side 2 from length of side 1, angle of side 1 and angle of
    /// side 2.
    /// </summary>
    /// <param name="Side1"></param>
    /// <param name="Vertex1"></param>
    /// <param name="Vertex2"></param>
    /// <returns></returns>
    public static double CalcSideFromAngleAngleSide(
      double Side1, Angle360 Vertex1, Angle360 Vertex2)
    {
      // formula: (sin vertex1)/side1 = (sin vertex2)/side2
      // side2 * ((sin vertex1)/side1) = (sin vertex2)
      // side2 = (sin vertex2) / ((sin vertex1)/side1)

      var side2 = Vertex2.Radians.Sine / (Vertex1.Radians.Sine / Side1);
      return side2;
    }

    /// <summary>
    /// Construct an ObliqueTriangle from three lines. If the end points of all the 
    /// lines do not meet to form a triangle, return null.
    /// </summary>
    /// <param name="Line1"></param>
    /// <param name="Line2"></param>
    /// <param name="Line3"></param>
    /// <returns></returns>
    public static ObliqueTriangle TryConstructFromLines(
      LineCoordinates Line1, LineCoordinates Line2, LineCoordinates Line3)
    {
      ObliqueTriangle ot = null;

      // line up the points of the lines
      var linePoints = new Point[] { Line1.Start, Line1.End, Line2.Start, Line2.End,
        Line3.Start, Line3.End };

      // start of line1 matches the end points of the other 2 lines.

      return ot;
    }

    public static ObliqueTriangle ConstructFromVertexPositions(
      Point Pos1, Point Pos2, Point Pos3)
    {
      // setup the left most vertex, then the upper vertex and lower vertex.
      var rv = ObliqueTriangle.SplitLeftMostPoint(new Point[] { Pos1, Pos2, Pos3 });
      Point pt1 = rv.Item1; // pt1 is left most point

      var rv2 = ObliqueTriangle.SplitTopMostPoint(rv.Item2);
      Point pt2 = rv2.Item1; // pt2 is top most point of pt2 and pt3.
      Point pt3 = rv2.Item2[0];

      // Create lines between the points. Line numbers match the number of the opposite
      // point.
      LineCoordinates line1 = new LineCoordinates(pt2, pt3);
      LineCoordinates line2 = new LineCoordinates(pt1, pt3);
      LineCoordinates line3 = new LineCoordinates(pt1, pt2);

      // the angle of each vertex.
      var ang1 = LineCoordinates.AngleBetween(line2, line3);
      var ang2 = LineCoordinates.AngleBetween(line1, line3);
      var ang3 = LineCoordinates.AngleBetween(line2, line1);

      var vertex1 = new TriangleVertex()
      {
        Angle = ang1,
        Location = pt1,
        Line = line1
      };

      var vertex2 = new TriangleVertex()
      {
        Angle = ang2,
        Location = pt2,
        Line = line2
      };

      var vertex3 = new TriangleVertex()
      {
        Angle = ang3,
        Location = pt3,
        Line = line3
      };

      var ot = new ObliqueTriangle()
      {
        Vertex1 = vertex1,
        Vertex2 = vertex2,
        Vertex3 = vertex3
      };

      return ot;
    }

    /// <summary>
    /// split the array of input points into one point that is the left most point
    /// and then an array of the remaining points.
    /// </summary>
    /// <param name="Points"></param>
    /// <returns></returns>
    private static Tuple<Point, Point[]> SplitLeftMostPoint(Point[] Points)
    {
      int leftMostIx = -1;
      
      // find index of the left most point in the array.
      for (int ix = 0; ix < Points.Length; ++ix)
      {
        if (leftMostIx == -1)
          leftMostIx = ix;
        else if (Points[ix].X < Points[leftMostIx].X)
        {
          leftMostIx = ix;
        }
      }

      Point[] otherPoints = new Point[Points.Length - 1] ;
      int tx = 0 ;
      for( int ix = 0 ; ix < Points.Length ; ++ix)
      {
        if ( ix != leftMostIx )
        {
          otherPoints[tx] = Points[ix] ;
          tx += 1 ;
        }
      }

      return new Tuple<Point,Point[]>(Points[leftMostIx], otherPoints) ;
    }


    /// <summary>
    /// split the array of input points into one point that is the upper most point
    /// and then an array of the remaining points.
    /// </summary>
    /// <param name="Points"></param>
    /// <returns></returns>
    private static Tuple<Point, Point[]> SplitTopMostPoint(Point[] Points)
    {
      int topMostIx = -1;

      // find index of the top most point in the array.
      for (int ix = 0; ix < Points.Length; ++ix)
      {
        if (topMostIx == -1)
          topMostIx = ix;
        else if (Points[ix].Y < Points[topMostIx].Y)
        {
          topMostIx = ix;
        }
      }

      Point[] otherPoints = new Point[Points.Length - 1];
      int tx = 0;
      for (int ix = 0; ix < Points.Length; ++ix)
      {
        if (ix != topMostIx)
        {
          otherPoints[tx] = Points[ix];
          tx += 1;
        }
      }

      return new Tuple<Point, Point[]>(Points[topMostIx], otherPoints);
    }


  }
}
