using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace AutoCoder.Forms
{
  /// <summary>
  /// Experimental. Point that can positioned relative to another point.
  /// Superceded by methods that calc a point relative to a Rectangle.
  /// </summary>
  public class DrawPoint
  {
    int mX = 0 ;
    int mY = 0 ;
    DrawPoint mBaseX;
    DrawPoint mBaseY;

    /// <summary>
    /// DrawPoint built from absoute values.
    /// </summary>
    /// <param name="InAbsoluteX"></param>
    /// <param name="InAbsoluteY"></param>
    public DrawPoint(int InAbsoluteX, int InAbsoluteY)
    {
      mBaseX = null;
      mBaseY = null;
      mX = InAbsoluteX;
      mY = InAbsoluteY;
    }

    /// <summary>
    /// construct DrawPoint from absolute and/or relative values.
    /// </summary>
    /// <param name="InBaseX"></param>
    /// <param name="InOffsetX"></param>
    /// <param name="InBaseY"></param>
    /// <param name="InOffsetY"></param>
    public DrawPoint(
      DrawPoint InBaseX, int InOffsetX,
      DrawPoint InBaseY, int InOffsetY)
    {
      mBaseX = InBaseX;
      mX = InOffsetX;
      mBaseY = InBaseY;
      mY = InOffsetY;
    }

    /// <summary>
    /// the actual, absolute point represented by this DrawPoint.
    /// </summary>
    public Point Point
    {
      get
      {
        int x = 0;
        int y = 0;

        if (mBaseX == null)
          x = mX;
        else
          x = mBaseX.Point.X + mX;

        if (mBaseY == null)
          y = mY;
        else
          y = mBaseY.Point.Y + mY;

        return new Point(x, y);
      }
    }

  }
}
