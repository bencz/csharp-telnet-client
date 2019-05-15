using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Shapes;
using AutoCoder.Windows.Lines;

namespace AutoCoder.Ext.Shapes.LineClasses
{

#if skip

  /// <summary>
  /// info about a line, from a left to right perspective.
  /// The start pos, end pos and angle of the line from the horizontal.
  /// </summary>
  public class LineLeftToRightInfo
  {

    public LineLeftToRightInfo(Line Line)
    {
      if (Line.Y1 > Line.Y2)
      {
        if (Line.X1 < Line.X2)
        {
          this.Start = new Point(Line.X1, Line.Y1);
          this.End = new Point(Line.X2, Line.Y2);
        }
        else
        {
          this.Start = new Point(Line.X2, Line.Y2);
          this.End = new Point(Line.X1, Line.Y1);
        }
      }
      else
      {
        if (Line.X1 <= Line.X2)
        {
          this.Start = new Point(Line.X1, Line.Y1);
          this.End = new Point(Line.X2, Line.Y2);
        }
        else
        {
          this.Start = new Point(Line.X2, Line.Y2);
          this.End = new Point(Line.X1, Line.Y1);
        }
      }
    }

    public LineLeftToRightInfo(LineCoordinates Coor)
    {
      if (Coor.Start.Y > Coor.End.Y)
      {
        if (Coor.Start.X < Coor.End.X)
        {
          this.Start = new Point(Coor.Start.X, Coor.Start.Y);
          this.End = new Point(Coor.End.X, Coor.End.Y);
        }
        else
        {
          this.Start = new Point(Coor.End.X, Coor.End.Y);
          this.End = new Point(Coor.Start.X, Coor.Start.Y);
        }
      }
      else
      {
        if (Coor.Start.X <= Coor.End.X)
        {
          this.Start = new Point(Coor.Start.X, Coor.Start.Y);
          this.End = new Point(Coor.End.X, Coor.End.Y);
        }
        else
        {
          this.Start = new Point(Coor.End.X, Coor.End.Y);
          this.End = new Point(Coor.Start.X, Coor.Start.Y);
        }
      }
    }

    private double? _Angle;

    /// <summary>
    /// The angle of the line relative to a horizontal line that intersects the Start
    /// position of the line. A positive angle means the line slopes down. A negative 
    /// angle means it slopes down. A vertical line has an angle of -90. A horizontal
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
          Point adjacentSide = new Point(this.End.X, this.Start.Y);
          Point lineEndPos = new Point(this.End.X, this.End.Y);

          Vector originToAdjacentSide = adjacentSide - origin;
          Vector originToLineEnd = lineEndPos - origin;

          _Angle = Vector.AngleBetween(originToAdjacentSide, originToLineEnd);
        }
        return _Angle.Value;
      }
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
 
    public double Bottom
    {
      get
      {
        if (this.Start.Y > this.End.Y)
          return this.Start.Y;
        else
          return this.End.Y;
      }
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
    /// The adjacent side line is the x axis line extending to the right from the 
    /// starting point of this line.
    /// </summary>
    /// <param name="LineLength"></param>
    /// <returns></returns>
    /// 
#if skip
    public LineCoordinates CalcAdjacentSideLine(double LineLength)
    {
      Point origin = new Point(this.Start.X, this.Start.Y);
      Point adjLineEnd = new Point(this.Start.X + LineLength - 1.00, this.Start.Y);
      return new LineCoordinates(origin, adjLineEnd);
    }
#endif

#if skip
    public LineCoordinates ComputeAdjacentSideLine(double LineLength)
    {
      Point origin = new Point(this.Start.X, this.Start.Y);
      Point adjLineEnd = new Point(this.Start.X + LineLength - 1.00, this.Start.Y);
      return new LineCoordinates(origin, adjLineEnd);
    }
#endif

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
      var adjSide = new HorizontalLineCoordinates(this.Start, AdjacentSideLength);

      var angle = Math.Abs(this.Angle);

      var radians = angle * (Math.PI / 180);
      var tan = Math.Tan(radians);

      // length of the opposite side line.
      var oppLgth = AdjacentSideLength * tan;

      // the end point of the opposite side line. Where the end point is either
      // above the adjacent line or below it. Depending on if the line ( the 
      // hypotenuse ) goes up or down.
      Point oppEnd;
      if (this.SlopesUp == true)
      {
        oppEnd = new Point(adjSide.End.X, adjSide.End.Y - oppLgth + 1.00);
        _OppositeSideLine = new LineCoordinates(oppEnd, adjSide.End);
      }
      else
      {
        oppEnd = new Point(adjSide.End.X, adjSide.End.Y + oppLgth - 1.00);
        _OppositeSideLine = new LineCoordinates(adjSide.End, oppEnd);
      }

      return _OppositeSideLine;
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
        _BoundedRect = null;
        _OppositeSideLine = null;
      }
    }

    private double Height
    {
      get
      {
        double ht = Math.Abs(this.End.Y - this.Start.Y) + 1.00;
        return ht;
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
      else if (Pos.Y < this.Top)
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
      else if (Pos.Y < this.Top)
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

    public bool? IsBetween(Point Pos, Point? PriorPoint)
    {
      bool? isBetween = null;

      if (PriorPoint == null)
        isBetween = false;

      return isBetween;
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
      if ((Pos.Y < this.Top) || (Pos.Y > this.Bottom))
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
      if ((Pos.Y < this.Top) || (Pos.Y > this.Bottom))
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
    /// 
    /// </summary>
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
    /// Calc the length of the 
    /// </summary>
    public double OppositeSideLength
    {
      get
      {
        return this.OppositeSideLine.Length;

#if skip
        var angle = Math.Abs(this.Angle);

        var radians = angle * (Math.PI / 180);
        var tan = Math.Tan(radians);

        var oppLgth = (End.X - Start.X + 1) * tan;

        // proof. this line is hypotenuse (C). Base horizontal line is A. Opposite 
        // side is B.
        var bLgth = Math.Sqrt(
          (AdjacentSideLine.Width * AdjacentSideLine.Width) +
          (AdjacentSideLine.Length * AdjacentSideLine.Length));

        return oppLgth;
#endif
      }
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

    public bool SlopesDown
    {
      get
      {
        if ( this.Start.Y < this.End.Y )
          return true;
        else
          return false;
      }
    }

    public bool SlopesUp
    {
      get
      {
        if ( this.Start.Y > this.End.Y )
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
        _BoundedRect = null;
        _OppositeSideLine = null;
      }
    }

    public double Top
    {
      get
      {
        if (this.Start.Y < this.End.Y)
          return this.Start.Y;
        else
          return this.End.Y;
      }
    }

    public override string ToString()
    {
      return
        "Start:" + this.Start.ToString() +
        " End:" + this.End.ToString();
    }

    private double Width
    {
      get
      {
        double wd = Math.Abs(this.End.X - this.Start.X) + 1.00;
        return wd;
      }
    }
  }

#endif

}
