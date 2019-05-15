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

  public class SideToShapeInfo
  {

    public Shape Shape
    { get; set; }

    public ShapeSide Side
    { get; set; }

    public WhichDirection? HorizDirection
    { get; set; }

    public HorizontalIntersect HorizIntersect
    { get; set; }

    public LineCoordinates HorizSide
    { get; set; }

    public WhichDirection? VertDirection
    { get; set; }


    /// <summary>
    /// how the side vertically intersects VertSide of the shape
    /// </summary>
    public VerticalIntersect VertIntersect
    { get; set; }

    public LineCoordinates VertSide
    { get; set; }

    /// <summary>
    /// return the direction from a point to the shape.
    /// </summary>
    /// <param name="Shape"></param>
    /// <param name="FromPoint"></param>
    /// <returns></returns>
    public SideToShapeInfo(ShapeSide Side, Shape Shape)
    {
      this.Side = Side;
      this.Shape = Shape;

      WhichDirection? horizDir = null;
      LineCoordinates horizSide = null;
      HorizontalIntersect horizIntersect = null;
      WhichDirection? vertDir = null;
      LineCoordinates vertSide = null;
      VerticalIntersect vertIntersect = null;

      var br = Shape.GetBoundedRect();

      if (Side.LineCoor.TopMost > br.Bottom)
      {
        vertDir = WhichDirection.Up;
        horizSide = Shape.GetSide(WhichSide.Bottom);
        if (Side.LineCoor.IsHorizontal)
        {
          horizIntersect = LineCoordinates.HorizontalIntersect(Side.LineCoor, horizSide) ;
        }
      }
      else if (Side.LineCoor.BottomMost < br.Top)
      {
        vertDir = WhichDirection.Down;
        horizSide = Shape.GetSide(WhichSide.Top);
        if (Side.LineCoor.IsHorizontal)
        {
          horizIntersect = LineCoordinates.HorizontalIntersect(Side.LineCoor, horizSide) ;
        }
      }

      if (Side.LineCoor.LeftMost > br.Right)
      {
        horizDir = WhichDirection.Left;
        vertSide = Shape.GetSide(WhichSide.Right);
        if (Side.LineCoor.IsVertical)
        {
          vertIntersect = LineCoordinates.VerticalIntersect(Side.LineCoor, vertSide);
        }
      }

      else if (Side.LineCoor.RightMost < br.Left)
      {
        horizDir = WhichDirection.Right;
        vertSide = Shape.GetSide(WhichSide.Left);
        if (Side.LineCoor.IsVertical)
        {
          vertIntersect = LineCoordinates.VerticalIntersect(Side.LineCoor, vertSide) ;
        }
      }

      // setup the vertical side of the shape that faces the side.
      if (vertSide == null)
      {
        var rightSide = new ShapeSide(Shape, WhichSide.Right);
        var leftSide = new ShapeSide(Shape, WhichSide.Left);
        if (Side.RightMost > rightSide.RightMost)
          vertSide = rightSide.LineCoor;
        else if (Side.LeftMost < leftSide.LeftMost)
          vertSide = leftSide.LineCoor;
      }

      // setup the horizontal side of the shape that faces the side.
      if (horizSide == null)
      {
        var bottomSide = new ShapeSide(Shape, WhichSide.Bottom);
        var topSide = new ShapeSide(Shape, WhichSide.Top);
        if (Side.BottomMost > bottomSide.BottomMost)
          horizSide = bottomSide.LineCoor;
        else if (Side.TopMost < topSide.TopMost)
          horizSide = topSide.LineCoor;
      }

      this.HorizDirection = horizDir;
      this.HorizSide = horizSide;
      this.HorizIntersect = horizIntersect;
      this.VertDirection = vertDir;
      this.VertSide = vertSide;
      this.VertIntersect = vertIntersect;
    }
  }
}
