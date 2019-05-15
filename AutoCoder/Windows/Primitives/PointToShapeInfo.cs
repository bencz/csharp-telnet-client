using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AutoCoder.Core.Enums;
using AutoCoder.Windows.Lines;
using System.Windows;
using System.Windows.Shapes;
using AutoCoder.Ext.Shapes;

namespace AutoCoder.Windows.Primitives
{
  /// <summary>
  /// info on where a point is relative to a shape. Whether the point is above
  /// or below the shape. To the left or right of the shape.
  /// </summary>
  public class PointToShapeInfo
  {
    public WhichDirection? HorizDirection
    { get; set; }

    /// <summary>
    /// the side of the shape, the bottom or the top, that the VertDirection of the point
    /// is either above or below.
    /// </summary>
    public LineCoordinates HorizSide
    { get; set; }

    public WhichDirection? VertDirection
    { get; set; }

    public LineCoordinates VertSide
    { get; set; }


    public PointToShapeInfo(Shape Shape, Point Point)
    {
      WhichDirection? horizDir = null;
      LineCoordinates horizSide = null;
      WhichDirection? vertDir = null;
      LineCoordinates vertSide = null;

      var topSide = Shape.GetSide(WhichSide.Top);
      var leftSide = Shape.GetSide(WhichSide.Left);

      if (Point.Y > leftSide.End.Y)
      {
        vertDir = WhichDirection.Up;
        horizSide = Shape.GetSide(WhichSide.Bottom);
      }
      else if (Point.Y < leftSide.Start.Y)
      {
        vertDir = WhichDirection.Down;
        horizSide = Shape.GetSide(WhichSide.Top);
      }

      if (Point.X > topSide.End.X)
      {
        horizDir = WhichDirection.Left;
        vertSide = Shape.GetSide(WhichSide.Right);
      }
      else if (Point.X < topSide.Start.X)
      {
        horizDir = WhichDirection.Right;
        vertSide = Shape.GetSide(WhichSide.Left);
      }

      this.HorizDirection = horizDir;
      this.HorizSide = horizSide;
      this.VertDirection = vertDir;
      this.VertSide = vertSide;
    }

    public bool MatchesDirection(WhichDirection Direction)
    {
      if (Direction.IsVertical() && (VertDirection != null)
        && (VertDirection.Value == Direction))
        return true;
      else if (Direction.IsHorizontal( ) && ( this.HorizDirection != null )
        && ( this.HorizDirection.Value == Direction ))
        return true ;
      else
        return false ;
    }
  }
}
