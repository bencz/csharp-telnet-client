using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Shapes;
using System.Windows.Media;
using AutoCoder.Ext.Shapes.Triangles;
using System.Xml.Linq;
using AutoCoder.Ext.Shapes.LineClasses;
using AutoCoder.Ext.Shapes;
using AutoCoder.Windows.Triangles;
using AutoCoder.Ext;
using AutoCoder.Windows.Primitives;
using AutoCoder.Ext.Windows;

namespace AutoCoder.Windows.Lines
{
  /// <summary>
  /// the start and end position of a line.
  /// </summary>
  public class LineCoordinates
  {
    public LineCoordinates(Point Start, Point End)
    {
      // store the start and end pos so that the line runs from left to right.
      // if the line is vertical, store the top of the line as the start.
      if (Start.Y > End.Y)
      {
        if (Start.X < End.X)
        {
          this.Start = Start;
          this.End = End;
        }
        else
        {
          this.Start = End;
          this.End = Start;
        }
      }
      else
      {
        if (Start.X <= End.X)
        {
          this.Start = Start;
          this.End = End;
        }
        else
        {
          this.Start = End;
          this.End = Start;
        }
      }
    }

    public LineCoordinates(Point Start, LineVector Vector)
      : this(Start, Vector.EndPoint(Start))
    {
    }

    LineCoordinates _AdjacentSideLine;

    /// <summary>
    /// the line that runs along the x-axis from the start of this line to the X position
    /// of the end of this line.
    /// This line, the adjacent line and the opposite side line are the three lines
    /// which form a right triangle. With this line being the hypotenuse of that triangle
    /// </summary>
    public LineCoordinates AdjacentSideLine
    {
      get
      {
        if (_AdjacentSideLine == null)
        {
          Point origin = new Point(this.Start.X, this.Start.Y);
          Point adjacentSide = new Point(this.End.X, this.Start.Y);
          _AdjacentSideLine =
            new LineCoordinates(origin, adjacentSide);
        }
        return _AdjacentSideLine;
      }
    }

    private double? _Angle;

    /// <summary>
    /// The angle of the line relative to a horizontal line that intersects the Start
    /// position of the line. A positive angle means the line slopes down. A negative 
    /// angle means it slopes up. A vertical line has an angle of -90. A horizontal
    /// line has an angle of zero.
    /// </summary>
    public double Angle
    {
      get
      {
        // angle of the line relative to a horizontal line that interesect the start pos
        // of the line.
        if (_Angle == null)
        {
          Point origin = new Point(this.Start.X, this.Start.Y);
          var end2 = new Point(this.Start.X + this.Width, this.Start.Y);
          var end3 = new Point(end2.X, end2.Y + this.Height);
          var v2 = end2 - origin;
          var v3 = end3 - origin;
          _Angle = Vector.AngleBetween(v2, v3);





          Point adjacentSide = new Point(this.End.X, this.Start.Y);
          Point lineEndPos = new Point(this.End.X, this.End.Y);

          Vector originToAdjacentSide = adjacentSide - origin;
          Vector originToLineEnd = lineEndPos - origin;

          var x_Angle = Vector.AngleBetween(originToAdjacentSide, originToLineEnd);
        }
        return _Angle.Value;
      }
    }

    /// <summary>
    /// Return the other end point of the line. The point that is not the input
    /// start/end point.
    /// </summary>
    /// <param name="StartEndPoint"></param>
    /// <returns></returns>
    public Point OtherPoint(Point StartEndPoint)
    {
      if (this.Start.Equals(StartEndPoint))
        return this.End;
      else if (this.End.Equals(StartEndPoint))
        return this.Start;
      else
        throw new ApplicationException("not an end point on the line");
    }

    /// <summary>
    /// compute the angle between two lines.
    /// </summary>
    /// <param name="Line1"></param>
    /// <param name="Line2"></param>
    /// <returns></returns>
    public static double AngleBetween(LineCoordinates Line1, LineCoordinates Line2)
    {
      // the start point of each line is the common point between the two lines.
      var commonPoint = LineCoordinates.CommonEndPoint(Line1, Line2);

      // the end points of each line.
      var end1 = Line1.OtherPoint(commonPoint);
      var end2 = Line2.OtherPoint(commonPoint);

      // the 360 degree angles of each line.
      var angle1 = LineExt.GetAngle(commonPoint, end1);
      var angle2 = LineExt.GetAngle(commonPoint, end2);

      // the angle between is the diff between the 360 degree angles of the 2 lines.
      var angleBet = Angle360.Subtract(angle1, angle2);
      if (angleBet.Value > 180)
        angleBet = Angle360.Subtract(angle2, angle1);

      return angleBet.Value ;
    }

    /// <summary>
    /// the bottom most position of the line.
    /// </summary>
    public double BottomMost
    {
      get
      {
        if (Start.Y > End.Y)
          return Start.Y;
        else
          return End.Y;
      }
    }

    Rect? _BoundedRect;

    /// <summary>
    /// a rectangle that is parallel to the x-axis that is drawn on the coordinates of
    /// this line.
    /// </summary>
    public Rect BoundedRect
    {
      get
      {
        if (_BoundedRect == null)
        {

          // note: regarding whether StrokeThickness should be factored into the calc of
          //       the bounding rect of the line, consider that depending on the angle of
          //       the line the thickness expands the rect in the x or y direction.
          double halfThickness = 0.00;
          if (this.StrokeThickness > 1.00)
            halfThickness = this.StrokeThickness / 2;


          if (this.SlopesUp == true)
          {
            _BoundedRect = new Rect(this.Start.X, this.End.Y,
              this.End.X - this.Start.X + 1.00,
              this.Start.Y - this.End.Y + 1.00);
          }
          else
          {
            _BoundedRect = new Rect(this.Start.X, this.Start.Y,
              this.End.X - this.Start.X + 1.00,
              this.End.Y - this.Start.Y + 1.00);
          }
        }
        return _BoundedRect.Value;
      }
    }

#if skip
    public static double CalcTangent(double AdjLgth, double OppLgth)
    {
      var tan = OppLgth / AdjLgth;
      return tan;
    }
#endif

    public static double CalcAngleFromTangent(double Tangent)
    {
      var radians = Math.Atan(Tangent);
      double angle = radians / (Math.PI / 180);
      return angle;
    }

    public LineCoordinates CalcAdjacentSideLine(double OppLgth)
    {
      // calc the adj side line lgth from the angle of this line and the given
      // opposite side lgth.
      var angle = Math.Abs(this.Angle);
      var radians = angle * (Math.PI / 180);
      var tan = Math.Tan(radians);
      var adjLgth = OppLgth / tan;

      var adjLine = new HorizontalLineCoordinates(this.Start, adjLgth);

      return adjLine;
    }

    /// <summary>
    /// Calc the length of the adjacent side from the length of the opposite side and
    /// the tangent of the adjacent side angle.
    /// </summary>
    /// <param name="Tan"></param>
    /// <param name="OppLgth"></param>
    /// <returns></returns>
    public static double CalcAdjacentLgth(double Tan, LineHeight OppLgth)
    {
      var adjLgth = Math.Abs(OppLgth.Value) / Tan;
      return adjLgth;
    }

    public static double CalcLineHeight(double Start, double End)
    {
      double ht = End - Start;
      if (ht < 0)
        ht += -1;
      else
        ht += 1;
      return ht;
    }

    public static double CalcLineLength(Point Start, Point End)
    {
      double ht = Math.Abs(CalcLineHeight(Start.Y, End.Y));
      double wd = Math.Abs(CalcLineWidth(Start.X, End.X));
      double lx = Math.Sqrt((ht * ht) + (wd * wd));
      return lx;
    }

    public static double CalcLineWidth(double Start, double End)
    {
      double wd = End - Start;
      if (wd < 0)
        wd += -1;
      else
        wd += 1;
      return wd;
    }

    public static double CalcTangent(double Angle)
    {
      var angle = Math.Abs(Angle);
      var radians = angle * (Math.PI / 180);
      var tan = Math.Tan(radians);
      return tan;
    }

    /// <summary>
    /// return the point on the line that lines up with the specified position
    /// on the x-axis base line.
    /// </summary>
    /// <param name="AdjSideLgth"></param>
    /// <returns></returns>
    public Point CalcHorizontalPointOnLine(double AdjSideLgth)
    {
      if (this.IsVertical == true)
      {
        throw new ApplicationException("line is vertical");
      }
      else if (this.IsHorizontal == true)
      {
        return new Point(this.Start.X + AdjSideLgth - 1.00, this.Start.Y);
      }
      else
      {
        var oppLine = this.CalcOppositeSideLine(AdjSideLgth);
        if (this.SlopesUp == true)
          return oppLine.Start;
        else
          return oppLine.End;
      }
    }

    /// <summary>
    /// The opposite side line is the line opposite the angle between this line and the
    /// adjacent line. This line, the adjacent line and the opposite line form a right
    /// triangle. Where the right angle of the right triangle is at the intersection of
    /// the adjacent line and the opposite side line.
    /// 
    /// Use this method to get the position on the line of the opposite side line as
    /// the length of the adjacent line is varied.
    /// </summary>
    /// <param name="AdjacentSideLength"></param>
    /// <returns></returns>
    public LineCoordinates CalcOppositeSideLine(double AdjacentSideLength)
    {
      var adjLine = new HorizontalLineCoordinates(this.Start, AdjacentSideLength);

      var radians = this.Angle * (Math.PI / 180);
      var tan = Math.Tan(radians);

      // length of the opposite side line 
      var oppLgth = AdjacentSideLength * tan;

      // the end point of the opposite side line. Where the end point is either
      // above the adjacent line or below it. Depending on if the line ( the 
      // hypotenuse ) goes up or down.
      if (this.SlopesUp == true)
      {
        var oppTop = new Point(adjLine.End.X, adjLine.End.Y + oppLgth + 1.00);
        return new LineCoordinates(oppTop, adjLine.End);
      }
      else
      {
        var oppBot = new Point(adjLine.End.X, adjLine.End.Y + oppLgth - 1.00);
        return new LineCoordinates(adjLine.End, oppBot);
      }
    }

    public LineCoordinates xCalcOppositeSideLine(double AdjacentSideLength)
    {
      var adjSide = new HorizontalLineCoordinates(this.Start, AdjacentSideLength);

      var angle = Math.Abs(this.Angle);

      var radians = angle * (Math.PI / 180);
      var tan = Math.Tan(radians);

      // length of the opposite side line.
      var oppLgth = AdjacentSideLength * tan;

      // the end point of the opposite side line. Where the end point is either
      // above the adjacent line or below it. Depending on if the line ( the 
      // hypotenuse ) goes up or down.
      if (this.SlopesUp == true)
      {
        var oppTop = new Point(adjSide.End.X, adjSide.End.Y - oppLgth + 1.00);
        return new VerticalLineCoordinates(oppTop, oppLgth);
      }
      else
      {
        return new VerticalLineCoordinates(adjSide.End, oppLgth);
      }
    }

    public Point CalcVerticalPointOnLine(double OppSideLgth)
    {
      if (this.IsVertical == true)
      {
        return new Point(this.Start.X, this.Start.Y + OppSideLgth - 1.00);
      }
      else if (this.IsHorizontal == true)
      {
        throw new ApplicationException("line is horizontal");
      }
      else
      {
        var adjLine = this.CalcAdjacentSideLine(OppSideLgth);
        return adjLine.End;
      }
    }

    /// <summary>
    /// return the start/end points the two lines have in common. The two lines form a 
    /// vertex. The common end point is the point location of that vertex.
    /// </summary>
    /// <param name="Line1"></param>
    /// <param name="Line2"></param>
    /// <returns></returns>
    public static Point CommonEndPoint(LineCoordinates Line1, LineCoordinates Line2)
    {
      if (Line1.Start.Equals(Line2.Start))
        return Line1.Start;
      else if (Line1.End.Equals(Line2.Start))
        return Line1.End;
      else if (Line1.Start.Equals(Line2.End))
        return Line1.Start;
      else if (Line1.End.Equals(Line2.End))
        return Line1.End;

      else
        throw new ApplicationException("lines do not have end points in common");
    }

    /// <summary>
    /// Determine if this line intercepts any point of the shape.
    /// </summary>
    /// <param name="Shape"></param>
    /// <returns></returns>
    public bool DoesIntersect(Shape Shape)
    {
      Rect rect = Shape.GetBoundedRect();

      // line starts on the x-axis after the right most point of the shape.
      if (Start.X > rect.Right)
        return false;

        // line ends on the x-axis before the left most point of the shape.
      else if (End.X < rect.Left)
        return false;

        // line starts anywhere within the rect of the shape.
      else if (rect.Contains(this.Start))
        return true;

        // line end point is within the rect of the shape.
      else if (rect.Contains(this.End))
        return true;

      else if (this.IsHorizontal)
      {
        // the y-axis position of the line is either above the top of the rect or below
        // the bottom of the rect.
        if ((Start.Y < rect.Top) || (Start.Y > rect.Bottom))
          return false;

        else
          return true;
      }

      else if (this.IsVertical)
      {
        // the x-axis position of the line is either before the left side of the rect or
        // after the right side of the rect.
        if ((Start.X < rect.Left) || (Start.X > rect.Right))
          return false;

        else
          return true;
      }

      else
      {
        // line starts before the rect. calc where the line is on the y-axis as it 
        // crosses the x-axis of the left side of the shape.
        double adjLgth = rect.Left - this.Start.X + 1.00;
        var oppLine = this.CalcOppositeSideLine(adjLgth);

        if (this.SlopesDown)
        {
          if (oppLine.End.Y > rect.Bottom)
            return false;
          else
            return true;
        }

        else
        {
          if (oppLine.Start.Y < rect.Top)
            return false;
          else
            return true;
        }
      }
    }

    /// <summary>
    /// the rate of change in the y-axis for each unit of the x-axis.
    /// The value is the tangent of the angle of the line. Where the line runs from left
    /// to right, and the angle is the angle between the line and the x-axis where it
    /// intersects the starting point of the line.
    /// </summary>
    public double VerticalToHorizontalRateOfChange
    {
      get
      {
        var radians = this.Angle * (Math.PI / 180);
        var tan = Math.Tan(radians);
        return tan;

#if skip
        double horizLgth = 20;

        var coor = this.CalcOppositeSideLine(horizLgth);
        double vertLgth = coor.Height;
        if (this.SlopesUp)
          vertLgth = vertLgth * -1.00;
        double rateChange = vertLgth / horizLgth;

        {
          var xx = rateChange * horizLgth;

          double horizLgth2 = 50;
          var coor2 = this.CalcOppositeSideLine(horizLgth2);
          double vertLgth2 = coor2.Height;
          if (this.SlopesUp)
            vertLgth2 = vertLgth2 * -1.00;
          double rateChange2 = vertLgth2 / horizLgth2;

          var xx2 = rateChange2 * horizLgth2;
        }

        return rateChange;
#endif

      }
    }


    /// <summary>
    /// return a delegate which calculates y from x using the point-slope formula
    /// form of this line.
    /// </summary>
    /// <returns></returns>
    public Func<double, double> PointSlopeForm()
    {
      var slope = this.Height / this.Width;
      Func<double, double> func = delegate(double X)
      {
        var y = slope * (X - this.Start.X) + this.Start.Y;
        return y;
      };

      return func ;
    }

    public string PointSlopeFormula()
    {
      var slope = this.Height / this.Width;
      return "y = " + Math.Round(slope, 4).ToString() + " * (x - " +
        Math.Round(this.Start.X, 2) + ") + " +
        Math.Round(this.Start.Y, 2);
    }

//  y = slope * ( x - x_origin ) + y_origin  // point slope form
    public Tuple<double, double, double> GetPointSlopeFormulaValues()
    {
      var slope = this.Height / this.Width;
      var originX = this.Start.X;
      var originY = this.Start.Y;

      return new Tuple<double, double, double>(slope, originX, originY);
    }


    public Line DrawLine()
    {
      Line newLine = new Line();
      newLine.Stroke = Brushes.Black;
      newLine.Fill = Brushes.Black;
      newLine.StrokeLineJoin = PenLineJoin.Bevel;
      newLine.X1 = this.Start.X;
      newLine.Y1 = this.Start.Y;
      newLine.X2 = this.End.X;
      newLine.Y2 = this.End.Y;
      newLine.StrokeThickness = 2;

      return newLine;
    }

    /// <summary>
    /// the left to right end position of the line.
    /// </summary>

    Point _End;
    public Point End
    {
      get { return _End; }
      set
      {
        _End = value;
        _Angle = null;
        _AdjacentSideLine = null;
        _OppositeSideLine = null;
        _BoundedRect = null;
      }
    }

    public string GetVisualizationInstructions()
    {
      XDocument xdoc = new XDocument(
            new XDeclaration("1.0", "utf-8", "yes"),
            new XElement("VisualizationInstructions",
              this.ToXElement("LineCoordinates")));

      return xdoc.ToString();
    }

    /// <summary>
    /// the difference between the two Y axis end points of the line as an absolute
    /// value. 
    /// </summary>
    /// <param name="Line"></param>
    /// <returns></returns>
    public double Height
    {
      get
      {
        double y = End.Y - Start.Y;
        if (y < 0)
          y += -1;
        else
          y += 1;
        return y;
      }
    }

    /// <summary>
    /// calc and return info on how two lines horizontally intersect each other.
    /// </summary>
    /// <param name="Line1"></param>
    /// <param name="Line2"></param>
    /// <returns></returns>
    public static HorizontalIntersect HorizontalIntersect(
      LineCoordinates Line1, LineCoordinates Line2)
    {
      HorizontalIntersect hi = null;
      double iw;
      double line1Ofs;
      double line2Ofs;
      if ((Line1.LeftMost <= Line2.LeftMost) && (Line1.RightMost >= Line2.LeftMost))
      {
        line1Ofs = Line2.LeftMost - Line1.LeftMost;
        line2Ofs = 0;
        if (Line1.RightMost < Line2.RightMost)
          iw = Line1.RightMost - Line2.LeftMost + 1.00;
        else
          iw = Line2.Width;
      }
      else if ((Line2.LeftMost <= Line1.LeftMost) && (Line2.RightMost >= Line1.LeftMost))
      {
        line2Ofs = Line1.LeftMost - Line2.LeftMost;
        line1Ofs = 0;
        if (Line2.RightMost < Line1.RightMost)
          iw = Line2.RightMost - Line1.LeftMost + 1.00;
        else
          iw = Line1.Width;
      }
      else
      {
        iw = 0;
        line1Ofs = 0;
        line2Ofs = 0;
      }

      hi = new HorizontalIntersect()
      {
        Length = iw,
        Line1 = Line1,
        Line1Ofs = line1Ofs,
        Line2 = Line2,
        Line2Ofs = line2Ofs
      };

      return hi;
    }
    /// <summary>
    /// Calc the point at which this line intersects with another line.
    /// </summary>
    /// <param name="Other"></param>
    /// <returns></returns>
    public static Point? IntersectPos(LineCoordinates Line1, LineCoordinates Line2)
    {
      // the x-axis range of this line does not intersect at all with the x-axis range
      // of the other line.
      if ((Line1.Start.X > Line2.End.X) || (Line1.End.X < Line2.Start.X))
        return null;

      // the y-axis range of this line does not intersect at all with the y-axis range
      // of the other line.
      else if ((Line1.TopMost > Line2.BottomMost) || (Line1.BottomMost < Line2.TopMost))
        return null;

        // lines are perpendicular. They intersect at the x-axis of the vertical line and
      // the y-axis of the horizontal line.
      else if (Line1.IsVertical && Line2.IsHorizontal)
      {
        return new Point(Line1.Start.X, Line2.Start.Y);
      }
      else if (Line1.IsHorizontal && Line2.IsVertical)
      {
        return new Point(Line1.Start.X, Line2.Start.Y);
      }

        // start pos of this line matches the start or end pos of the other line.
      else if ((Line1.Start.Equals(Line2.Start)) || (Line1.Start.Equals(Line2.End)))
      {
        return Line1.Start;
      }

        // end pos of this line matches the start or end pos of the other line.
      else if ((Line1.End.Equals(Line2.Start)) || (Line1.End.Equals(Line2.End)))
      {
        return Line2.End;
      }

      else
      {


        double i_x = 0.00;
        double i_y = 0.00;
//        char collisionDetected;

        double s1_x, s1_y, s2_x, s2_y;
        s1_x = Line1.End.X - Line1.Start.X;
        s1_y = Line1.End.Y - Line1.Start.Y;
        s2_x = Line2.End.X - Line2.Start.X;
        s2_y = Line2.End.Y - Line2.Start.Y;

        double s, t;
        s = (-s1_y * (Line1.Start.X - Line2.Start.X) + s1_x * (Line1.Start.Y - Line2.Start.Y))
          / (-s2_x * s1_y + s1_x * s2_y);
        t = (s2_x * (Line1.Start.Y - Line2.Start.Y) - s2_y * (Line1.Start.X - Line2.Start.X))
          / (-s2_x * s1_y + s1_x * s2_y);

        if (s >= 0 && s <= 1 && t >= 0 && t <= 1)
        {
          // Collision detected
          i_x = Line1.Start.X + (t * s1_x);
          i_y = Line1.Start.Y + (t * s1_y);
          return new Point(i_x, i_y);
        }
        else
        {
          return null;
        }
      }

#if skip
        var hi = LineCoordinates.HorizontalIntersect(this, Other);

        double leftMostX = hi.Line1.Start.X + hi.Line1Ofs;
        TriangleVertex vertex1;
        TriangleVertex vertex2;

        // calc the location and angle of the vertex of the intersect triangle formed
        // by line1.
        {
          Point? pos;
          double angle;
          if (hi.Line1Ofs > 0)
          {
            var adjLine = new HorizontalLineCoordinates(hi.Line1.Start, hi.Line1Ofs);
            var oppLine = this.CalcOppositeSideLine(hi.Line1Ofs);
            var rtTriangle = new RightTriangle()
            {
              AdjSide = adjLine,
              OppSide = oppLine
            };
            pos = rtTriangle.OppVertex.Location;
          }
          else
          {
            pos = hi.Line1.Start;
          }
          angle = IntersectPos_CalcVertexAngle(hi.Line1.Angle);
          vertex1 = new TriangleVertex()
          {
            Angle = angle,
            Location = pos.Value
          };
        }

        // calc the left side vertex of the intersect triangle formed by line2.
        {
          Point? pos;
          double angle;
          if (hi.Line2Ofs > 0)
          {
            var adjLine = new HorizontalLineCoordinates(hi.Line2.Start, hi.Line2Ofs);
            var oppLine = this.CalcOppositeSideLine(hi.Line2Ofs);
            var rtTriangle = new RightTriangle()
            {
              AdjSide = adjLine,
              OppSide = oppLine
            };
            pos = rtTriangle.OppVertex.Location;
          }
          else
          {
            pos = hi.Line2.Start;
          }
          angle = IntersectPos_CalcVertexAngle(hi.Line2.Angle);
          vertex2 = new TriangleVertex()
          {
            Angle = angle,
            Location = pos.Value
          };
        }

        // build the oblique triangle that is formed by the intersection of the
        // two lines.
        var obTriangle = new ObliqueTriangle()
        {
          Vertex1 = vertex1,
          Vertex2 = vertex2
        };


        return null;

      }
#endif

    }

    private double IntersectPos_CalcVertexAngle(double Angle)
    {
      double vertexAngle;
      if (Angle < 0)
        vertexAngle = 90 + Angle;
      else
        vertexAngle = 90 - Angle;
      return vertexAngle;
    }

    public Point? xIntersectPos(LineCoordinates Other)
    {
      // the x-axis range of this line does not intersect at all with the x-axis range
      // of the other line.
      if ((this.Start.X > Other.End.X) || (this.End.X < Other.Start.X))
        return null;

      // the y-axis range of this line does not intersect at all with the y-axis range
      // of the other line.
      else if ((this.TopMost > Other.BottomMost) || (this.BottomMost < Other.TopMost))
        return null;

        // lines are perpendicular. They intersect at the x-axis of the vertical line and
      // the y-axis of the horizontal line.
      else if (this.IsVertical && Other.IsHorizontal)
      {
        return new Point(this.Start.X, Other.Start.Y);
      }
      else if (this.IsHorizontal && Other.IsVertical)
      {
        return new Point(Other.Start.X, this.Start.Y);
      }

        // start pos of this line matches the start or end pos of the other line.
      else if ((this.Start.Equals(Other.Start)) || (this.Start.Equals(Other.End)))
      {
        return this.Start;
      }

        // end pos of this line matches the start or end pos of the other line.
      else if ((this.End.Equals(Other.Start)) || (this.End.Equals(Other.End)))
      {
        return this.End;
      }

      else
      {
        Point otherStart;
        Point thisStart;
        double otherTan = Other.VerticalToHorizontalRateOfChange;
        double thisTan = this.VerticalToHorizontalRateOfChange;

        if (this.Start.X > Other.Start.X)
        {
          thisStart = this.Start;
          otherStart = Other.CalcHorizontalPointOnLine(this.Start.X - Other.Start.X + 1.00);
        }
        else
        {
          otherStart = Other.Start;
          thisStart = this.CalcHorizontalPointOnLine(Other.Start.X - this.Start.X + 1.00);
        }

        // other line is above this line. The tan of the other line has to be greater than the
        // tangent of this line for the two to ever intersect.
        if (otherStart.Y < thisStart.Y)
        {
          // tan(a - b) = ( tan a - tan b ) / (1 + (tan a * tan b))
          double tanDiff = otherTan - thisTan;
          tanDiff = (otherTan - thisTan) / (1 + (otherTan * thisTan));
          if (tanDiff > 0)
          {
            var oppLgth = LineHeight.CalcHeight(thisStart.Y, otherStart.Y);
            var adjLgth = LineCoordinates.CalcAdjacentLgth(tanDiff, oppLgth);

            var intersectAdjLgth = (thisStart.X - this.Start.X + 1.00) + adjLgth;
            var intersectLine = this.CalcOppositeSideLine(intersectAdjLgth);
            if (this.SlopesDown)
              return intersectLine.End;
            else
              return intersectLine.Start;
          }
          else
          {
            return null;
          }
        }
        else
        {
          return null;
        }
      }
    }

    /// <summary>
    /// calc if the line is above the point.
    /// </summary>
    /// <param name="Pos"></param>
    /// <returns></returns>
    public bool? IsAbove(Point Pos)
    {
      bool? isAbove = null;

      // point is to left of the start of the line or to the right of the end.
      if ((Pos.X < this.Start.X) || (Pos.X > this.End.X))
      {
        isAbove = null;
      }

        // point is entirely above the line.
      else if (Pos.Y < this.TopMost)
      {
        isAbove = false;
      }

        // point is entirely below the line.
      else if (Pos.X < this.Start.X)
      {
        isAbove = true;
      }

      else
      {
        double adjSideLx = Pos.X - this.Start.X + 1.00;
        var oppLine = this.CalcOppositeSideLine(adjSideLx);
        if ((this.SlopesUp == true) && (oppLine.Start.Y < Pos.Y))
          isAbove = true;
        else if ((this.SlopesDown == true) && (oppLine.End.Y < Pos.Y))
          isAbove = true;
      }

      return isAbove;
    }

    /// <summary>
    /// calc if the line is below the point.
    /// </summary>
    /// <param name="Pos"></param>
    /// <returns></returns>
    public bool? IsBelow(Point Pos)
    {
      bool? isBelow = null;

      // point is to left of the start of the line or to the right of the end.
      if ((Pos.X < this.Start.X) || (Pos.X > this.End.X))
      {
        isBelow = null;
      }

        // point is entirely above the line.
      else if (Pos.Y < this.TopMost)
      {
        isBelow = true;
      }

        // point is entirely below the line.
      else if (Pos.X < this.Start.X)
      {
        isBelow = false;
      }

      else
      {
        double adjSideLx = Pos.X - this.Start.X + 1.00;
        var oppLine = this.CalcOppositeSideLine(adjSideLx);
        if ((this.SlopesUp == true) && (oppLine.Start.Y > Pos.Y))
          isBelow = true;
        else if ((this.SlopesDown == true) && (oppLine.End.Y > Pos.Y))
          isBelow = true;
      }

      return isBelow;
    }

    public bool IsHorizontal
    {
      get
      {
        if (this.Start.Y == this.End.Y)
          return true;
        else
          return false;
      }
    }

    /// <summary>
    /// calc if the line is to the left of a point.
    /// </summary>
    /// <param name="Pos"></param>
    /// <returns></returns>
    public bool? IsLeftOf(Point Pos)
    {
      bool? isLeftOf = null;

      // point is above the top of the line or below the bottom. 
      if ((Pos.Y < this.TopMost) || (Pos.Y > this.BottomMost))
      {
        isLeftOf = null;
      }

      else if (Pos.X > this.End.X)
      {
        isLeftOf = true;
      }

      else if (Pos.X < this.Start.X)
      {
        isLeftOf = false;
      }

      else
      {
        double adjSideLx = Pos.X - this.Start.X + 1.00;
        var oppLine = this.CalcOppositeSideLine(adjSideLx);
        if ((this.SlopesUp == true) && (oppLine.Start.Y < Pos.Y))
          isLeftOf = true;
        else if ((this.SlopesDown == true) && (oppLine.End.Y > Pos.Y))
          isLeftOf = true;
      }

      return isLeftOf;
    }

    /// <summary>
    /// calc if the line is to the right of a point.
    /// </summary>
    /// <param name="Pos"></param>
    /// <returns></returns>
    public bool? IsRightOf(Point Pos)
    {
      bool? isRightOf = null;

      // point is above the top of the line or below the bottom. 
      if ((Pos.Y < this.TopMost) || (Pos.Y > this.BottomMost))
      {
        isRightOf = null;
      }

        // the point is to the left of the start of the line. Since this line runs from 
      // left to right, the point is to the left of the line.
      else if (Pos.X < this.Start.X)
      {
        isRightOf = true;
      }

        // the point is to the right of the end of the line. 
      else if (Pos.X > this.End.X)
      {
        isRightOf = false;
      }

      else
      {
        double adjSideLx = Pos.X - this.Start.X + 1.00;
        var oppLine = this.CalcOppositeSideLine(adjSideLx);
        if ((this.SlopesUp == true) && (oppLine.Start.Y > Pos.Y))
          isRightOf = true;
        else if ((this.SlopesDown == true) && (oppLine.End.Y < Pos.Y))
          isRightOf = true;
      }

      return isRightOf;
    }

    public bool IsVertical
    {
      get
      {
        if (this.Start.X == this.End.X)
          return true;
        else
          return false;
      }
    }

    /// <summary>
    /// the left most position of the line.
    /// </summary>
    public double LeftMost
    {
      get
      {
        if (Start.X < End.X)
          return Start.X;
        else
          return End.X;
      }
    }

#if skip
    private LineLeftToRightInfo _LeftToRightInfo;
    public LineLeftToRightInfo LeftToRightInfo
    {
      get
      {
        if (_LeftToRightInfo == null)
        {
          _LeftToRightInfo = new LineLeftToRightInfo(this);
        }
        return _LeftToRightInfo;
      }
    }
#endif

    public double Length
    {
      get
      {
        double height = this.Height;  // the a side of right triangle
        double width = this.Width;    // the b side of right triangle

        // use the formula ( a squared + b squared ) = c squared to calc the length of
        // the line, where the line is the hypotenuse ( c squared ).
        double lgth = Math.Sqrt((height * height) + (width * width));
        return lgth;
      }
    }

    public Point MidPoint
    {
      get
      {
        if (this.IsVertical == true)
        {
          return new Point(this.Start.X, this.Start.Y + (this.Height / 2));
        }
        else if (this.IsHorizontal == true)
        {
          return new Point(this.Start.X + (this.Width / 2), this.Start.Y);
        }
        else
        {
          double adjSideLgth = ((this.End.X - this.Start.X) + 1.00) / 2;
          var oppLine = this.CalcOppositeSideLine(adjSideLgth);
          if (this.SlopesUp == true)
            return oppLine.Start;
          else
            return oppLine.End;
        }
      }
    }

    /// <summary>
    /// compute and return a new line that is located at an offset from the input line.
    /// </summary>
    /// <param name="LineCoor"></param>
    /// <param name="Offset"></param>
    /// <returns></returns>
    public static LineCoordinates MoveByOffset(LineCoordinates LineCoor, Point Offset)
    {
      var start = LineCoor.Start.AddPoint(Offset);
      var end = LineCoor.End.AddPoint(Offset);
      return new LineCoordinates(start, end);
    }

    LineCoordinates _OppositeSideLine;

    /// <summary>
    /// The opposite side line is the line opposite the angle between this line and the
    /// adjacent line. This line, the adjacent line and the opposite line form
    /// a right triangle. Where the right angle of the right triangle is at the 
    /// intersection of the adjacent line and the opposite side line.
    /// </summary>
    public LineCoordinates OppositeSideLine
    {
      get
      {
        if (_OppositeSideLine == null)
        {
          _OppositeSideLine = new LineCoordinates(
            this.AdjacentSideLine.End,
            this.End);
        }
        return _OppositeSideLine;
      }
    }

    /// <summary>
    /// the right most position of the line.
    /// </summary>
    public double RightMost
    {
      get
      {
        if (Start.X > End.X)
          return Start.X;
        else
          return End.X;
      }
    }

    public bool SlopesDown
    {
      get
      {
        if (this.Start.Y < this.End.Y)
          return true;
        else
          return false;
      }
    }

    public bool SlopesUp
    {
      get
      {
        if (this.Start.Y > this.End.Y)
          return true;
        else
          return false;
      }
    }

    /// <summary>
    /// the left to right start position of the line.
    /// </summary>
    Point _Start;
    public Point Start
    {
      get { return _Start; }
      set
      {
        _Start = value;
        _Angle = null;
        _AdjacentSideLine = null;
        _OppositeSideLine = null;
        _BoundedRect = null;
      }
    }

    double? _StrokeThickness;
    public double StrokeThickness
    {
      get
      {
        if (_StrokeThickness == null)
          return 1.00;
        else
          return _StrokeThickness.Value;
      }
      set { _StrokeThickness = value; }
    }

    /// <summary>
    /// the top most position of the line.
    /// </summary>
    public double TopMost
    {
      get
      {
        if (Start.Y < End.Y)
          return Start.Y;
        else
          return End.Y;
      }
    }

    public override string ToString()
    {
      return "Start: " + this.Start.ToString() +
        " End:" + this.End.ToString();
    }

    /// <summary>
    /// calc and return info on how two lines vertically intersect each other.
    /// </summary>
    /// <param name="Line1"></param>
    /// <param name="Line2"></param>
    /// <returns></returns>
    public static VerticalIntersect VerticalIntersect(
      LineCoordinates Line1, LineCoordinates Line2)
    {
      VerticalIntersect vi = null;
      double ih;
      double line1Ofs;
      double line2Ofs;
      if ((Line1.TopMost <= Line2.TopMost) && (Line1.BottomMost >= Line2.TopMost))
      {
        line1Ofs = Line2.TopMost - Line1.TopMost;
        line2Ofs = 0;
        if (Line1.BottomMost < Line2.BottomMost)
          ih = Line1.BottomMost - Line2.TopMost + 1.00;
        else
          ih = Line2.Height;
      }
      else if ((Line2.TopMost <= Line1.TopMost) && (Line2.BottomMost >= Line1.TopMost))
      {
        line2Ofs = Line1.TopMost - Line2.TopMost;
        line1Ofs = 0;
        if (Line2.BottomMost < Line1.BottomMost)
          ih = Line2.BottomMost - Line1.TopMost + 1.00;
        else
          ih = Line1.Height;
      }
      else
      {
        ih = 0;
        line1Ofs = 0;
        line2Ofs = 0;
      }

      vi = new VerticalIntersect()
      {
        Length = ih,
        Line1 = Line1,
        Line1Ofs = line1Ofs,
        Line2 = Line2,
        Line2Ofs = line2Ofs
      };

      return vi;
    }

    /// <summary>
    /// the difference between the two X axis end points of the line as an absolute
    /// value. 
    /// </summary>
    /// <param name="Line"></param>
    /// <returns></returns>
    public double Width
    {
      get
      {
        return Math.Abs(Start.X - End.X) + 1.00;
      }
    }

    public bool WithinHorizontalRange(double Pos)
    {
      if ((Pos >= Start.X) && (Pos <= End.X))
        return true;
      else
        return false;
    }

    public bool WithinVerticalRange(double Pos)
    {
      if ((Pos >= Start.Y) && (Pos <= End.Y))
        return true;
      else
        return false;
    }
  }

  // Create a static "Ext" class within the source file of the user defined class
  // Create two methods. The first creates an XElement from an instance of the class.
  // The seconds creates an instance of the class from the XElement.

  public static class LineCoordinatesExt
  {
    public static XElement ToXElement(this LineCoordinates LineCoor, XName Name)
    {
      if (LineCoor == null)
        return new XElement(Name, null);
      else
      {
        XElement xe = new XElement(Name,
            LineCoor.Start.ToXElement("Start"),
            LineCoor.End.ToXElement("End")
            );
        return xe;
      }
    }

    public static LineCoordinates ToLineCoordinates(
      this XElement Elem, XNamespace Namespace)
    {
      LineCoordinates coor = null;
      if (Elem != null)
      {

        var start = Elem.Element(Namespace + "Start").ToPoint(Namespace);
        var end = Elem.Element(Namespace + "End").ToPoint(Namespace);
        coor = new LineCoordinates(start, end);
      }
      return coor;
    }
  }
}

