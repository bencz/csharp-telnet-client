using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Shapes;
using System.Windows;
using AutoCoder.Ext.Shapes.LineClasses;
using AutoCoder.Windows.Lines;
using AutoCoder.Windows.Primitives;
using AutoCoder.Ext.Windows;

namespace AutoCoder.Ext.Shapes
{
  public static class LineExt
  {
    /// <summary>
    /// return the bottom most position of the line.
    /// </summary>
    /// <param name="Line"></param>
    /// <returns></returns>
    public static double GetBottom(this Line Line)
    {
      if (Line.Y1 >= Line.Y2)
        return Line.Y1;
      else
        return Line.Y2;
    }

    /// <summary>
    /// the difference between the two Y axis end points of the line as an absolute
    /// value. 
    /// </summary>
    /// <param name="Line"></param>
    /// <returns></returns>
    public static double GetHeight(this Line Line)
    {
      return Math.Abs(Line.Y1 - Line.Y2) + 1;
    }

    /// <summary>
    /// the left most position of the line.
    /// </summary>
    /// <param name="Line"></param>
    /// <returns></returns>
    public static double GetLeft(this Line Line)
    {
      if (Line.X1 <= Line.X2)
        return Line.X1;
      else
        return Line.X2;
    }

    public static double GetLength(this Line Line)
    {
      double height = Line.GetHeight();
      double width = Line.GetWidth();
      double lgth = Math.Sqrt((height * height) + (width * width));
      return lgth;
    }

    /// <summary>
    /// the right most position of the line.
    /// </summary>
    /// <param name="Line"></param>
    /// <returns></returns>
    public static double GetRight(this Line Line)
    {
      if (Line.X1 >= Line.X2)
        return Line.X1;
      else
        return Line.X2;
    }

    /// <summary>
    /// return the top most position of the line.
    /// </summary>
    /// <param name="Line"></param>
    /// <returns></returns>
    public static double GetTop(this Line Line)
    {
      if (Line.Y1 <= Line.Y2)
        return Line.Y1;
      else
        return Line.Y2;
    }

    /// <summary>
    /// the difference between the two X axis end points of the line as an absolute
    /// value. 
    /// </summary>
    /// <param name="Line"></param>
    /// <returns></returns>
    public static double GetWidth(this Line Line)
    {
      return Math.Abs(Line.X1 - Line.X2) + 1;
    }

#if skip
    // calc if the line slopes, from left to right, up or down.
    public static LineLeftToRightInfo GetLeftToRightInfo(this Line Line)
    {
      LineLeftToRightInfo info = new LineLeftToRightInfo(Line);

#if skip
      char slopeDir = ' ';
      if (Line.Y1 > Line.Y2)
      {
        if (Line.X1 < Line.X2)
        {
          slopeDir = 'u';
          info.Start = new Point(Line.X1, Line.Y1);
          info.End = new Point(Line.X2, Line.Y2);
        }
        else
        {
          slopeDir = 'd';
          info.Start = new Point(Line.X2, Line.Y2);
          info.End = new Point(Line.X1, Line.Y1);
        }
      }
      else
      {
        if (Line.X1 < Line.X2)
        {
          slopeDir = 'd';
          info.Start = new Point(Line.X1, Line.Y1);
          info.End = new Point(Line.X2, Line.Y2);
        }
        else
        {
          slopeDir = 'u';
          info.Start = new Point(Line.X2, Line.Y2);
          info.End = new Point(Line.X1, Line.Y1);
        }
      }
#endif

      // angle of the line relative to a horizontal line that interesect the start pos
      // of the line.
      {
        Point origin = new Point(info.Start.X, info.Start.Y);
        Point baseHorizontal = new Point(info.End.X, info.Start.Y) ;
        Point lineEndPos = new Point(info.End.X, info.End.Y);

        Vector originToBaseHorizontal = baseHorizontal - origin;
        Vector originToLineEnd = lineEndPos - origin;

//        info.Angle = Vector.AngleBetween(originToBaseHorizontal, originToLineEnd);
      }

      return info;
    }
#endif

    /// <summary>
    /// Compute the angle of the line relative to a vertical line drawn up from the
    /// start point.
    /// </summary>
    /// <param name="Start"></param>
    /// <param name="End"></param>
    /// <returns></returns>
    public static Angle360 GetAngle(Point Start, Point End)
    {
      // draw vertical line from the start to a random location above the start.
      var upVertical = new Point(Start.X, Start.Y - 90);
      
      // vectors to the input line and the up vertical line.
      var startToUpVertical = upVertical - Start;
      var startToEnd = End - Start;

      // the angle between the up vertical and the input line.
      var angle = Vector.AngleBetween(startToUpVertical, startToEnd);

      double angle360;
      if (angle < 0)
        angle360 = 180 + (angle + 180);
      else
        angle360 = angle;

      return new Angle360(angle360);
    }

    public static double GetAngle(this Line Line)
    {
      var xx = Line.GetCoordinates( ) ;
      return xx.Angle ;

#if skip
      double ang = xx.Angle;

      Point origin = new Point(Line.X1, Line.Y1);
      Point downVertical = new Point(Line.X1, Line.Y2);
      Point lineEndPos = new Point(Line.X2, Line.Y2);

      Vector originToDownVertical = downVertical - origin;
      Vector originToLineEnd = lineEndPos - origin;

      var angle = Vector.AngleBetween(originToDownVertical, originToLineEnd);

      return angle;
#endif
    }

    public static Rect GetBoundedRect(this Line Line)
    {
      var coor = new LineCoordinates(
        new Point(Line.X1, Line.Y1), new Point(Line.X2, Line.Y2));
      return coor.BoundedRect;

#if skip
      var left = Line.GetLeft();
      var top = Line.GetTop();
      var width = Math.Abs(Line.X2 - Line.X1) + 1.00;
      var height = Math.Abs(Line.Y2 - Line.Y1) + 1.00;
      var rect = new Rect(left, top, width, height);
      return rect;
#endif
    }

    public static LineCoordinates GetCoordinates(this Line Line)
    {
      var coor = new LineCoordinates(
        new Point(Line.X1, Line.Y1), new Point(Line.X2, Line.Y2));
      return coor;
    }

#if skip
    public static double GetAngleOld(this Line Line)
    {
      var width = Line.X2 - Line.X1;
      var height = Line.Y2 - Line.Y1;
      var angle = width / height;
      return angle;
    }
#endif

    public static bool IntersectsWith(this Line Line, Point Pos)
    {
      bool doesIntersect = false;

      var lineinfo = Line.GetCoordinates();

      if (lineinfo.BoundedRect.Contains(Pos) == true)
      {
        var halfThick = Line.StrokeThickness / 2;

        // how far is the pos from the left side of the line.
        var farOver = Pos.X - lineinfo.Start.X;

        // compute the verticle pos on the line ( based on its angle ) at the
        // x axis position of the Pos.
        var oppLine = lineinfo.CalcOppositeSideLine(farOver + 1.00);

        // the end point of the opposite side line that intersects the 
        // current uiShape line.
        Point pointOnLine;
        if (lineinfo.SlopesUp == true)
          pointOnLine = oppLine.Start;
        else
          pointOnLine = oppLine.End;

#if skip
              debug =
              "Mouse pos:" + Pos.ToString() + Environment.NewLine +
              "Calc mouse pos:" + pointOnLine.ToString() +
              Environment.NewLine +
              "Opposite side line:" + oppLine.ToString();
              if (comboDebug == null)
                comboDebug = debug;
              else
                comboDebug = comboDebug + Environment.NewLine +
                  "2nd line" + Environment.NewLine + debug;
#endif

        // build a Rect centered around the intersection point, where the 
        // distance from the point to the edge of the rect is half the stroke
        // width of the line.
        var centeredRect = pointOnLine.GetCenteredRect(halfThick + 1.00);

        // the Pos is touching the uiLine if it is contained within the 
        // intersection point rect.
        if (centeredRect.Contains(Pos))
        {
          doesIntersect = true;
        }
      }
      return doesIntersect;
    }

    public static bool IsHorizontal(this Line Line)
    {
      if (Line.Y1 == Line.Y2)
        return true;
      else
        return false;
    }

    public static bool IsVertical(this Line Line)
    {
      if (Line.X1 == Line.X2)
        return true;
      else
        return false;
    }

    /// <summary>
    /// move this line by the values of the vector. Add the vector values to the start
    /// and end positions of the line.
    /// </summary>
    /// <param name="Line"></param>
    /// <param name="Adj"></param>
    public static void Move(this Line Line, Vector Adj)
    {
      Line.X1 += Adj.X;
      Line.X2 += Adj.X;
      Line.Y1 += Adj.Y;
      Line.Y2 += Adj.Y;
    }

    public static string ToText(this Line Line)
    {
      string text = null;

      // a horizontal line
      if (Line.Y1 == Line.Y2)
      {
        double lx = Line.X2 - Line.X1;
        Point pt = new Point(Line.X1, Line.Y1);
        text = "Horizontal line. " + "From " + pt.ToString() + " Length " + lx.ToString();
      }

      return text == null ? "" : text;
    }


  }
}

