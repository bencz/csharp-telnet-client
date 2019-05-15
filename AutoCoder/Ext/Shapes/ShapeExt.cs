using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Shapes;
using AutoCoder.Core.Enums;
using AutoCoder.Ext.Shapes.LineClasses;
using AutoCoder.Ext.Controls;
using System.Windows;
using AutoCoder.Windows.Lines;
using AutoCoder.Windows.Primitives;
using System.Xml.Linq;
using AutoCoder.Windows.Ext;
using AutoCoder.Ext.Windows;
using AutoCoder.Windows.Shapes.Connection;

namespace AutoCoder.Ext.Shapes
{
  public static class ShapeExt
  {
    /// <summary>
    /// return the direction from a point to the shape.
    /// </summary>
    /// <param name="Shape"></param>
    /// <param name="FromPoint"></param>
    /// <returns></returns>
    public static PointToShapeInfo DirectionToShape(this Shape Shape, Point Point)
    {
      var info = new PointToShapeInfo(Shape, Point);
      return info;
    }

    /// <summary>
    /// Fill and return a SideToShapeInfo object with info on 
    /// </summary>
    /// <param name="Shape"></param>
    /// <param name="Side"></param>
    /// <returns></returns>
    public static SideToShapeInfo DirectionToShape(this Shape Shape, ShapeSide Side)
    {
      var info = new SideToShapeInfo(Side, Shape);
      return info;
    }

    public static Rect GetBoundedRect(this Shape Shape)
    {
      if (Shape is Line)
        return (Shape as Line).GetBoundedRect();
      else if (Shape is Rectangle)
      {
        return (Shape as Rectangle).GetBoundedRect();
      }
      else throw new ApplicationException("type is not supported");
    }

    public static Tuple<WhichSide, LineCoordinates> GetFacingSide(
      this Shape Shape, Point Point)
    {
      return null;
    }

#if skip
    public static string GetShapeInfo(this Shape Shape)
    {
      if (Shape is Rectangle)
      {
        var shapeRect = Shape as Rectangle;
        var br = shapeRect.GetBoundedRect();

        XDocument xdoc = new XDocument(
              new XDeclaration("1.0", "utf-8", "yes"),
              br.ToXElement("Rect")
              );
        return xdoc.ToString();
      }
      else
        return "Shape is not rectangle";
    }
#endif

#if skip
    public static bool IsOppositeDirection(
      this Shape FromShape, WhichDirection Direction, Shape ToShape)
    {
      WhichSide side = Direction.ToSide();


      if (side == WhichSide.Left)
      {
        if ( FromShape
      }

      switch (side)
      {
        case WhichDirection.Down:

          break;

        case WhichDirection.Up:
          break;

        case WhichDirection.Left:
          break;

        case WhichDirection.Right:
          break;

        default:
          throw new ApplicationException("unexpected direction");
      }

      return side;
    }
#endif

    public static WhichSide? GetSide(
      this Shape FromShape, WhichDirection Direction, Shape ToShape)
    {
      WhichSide? side = null;

      switch (Direction)
      {
        case WhichDirection.Down:

          break;

        case WhichDirection.Up:
          break;

        case WhichDirection.Left:
          break;

        case WhichDirection.Right:
          break;

        default:
          throw new ApplicationException("unexpected direction");
      }

      return side;
    }

    public static LineCoordinates GetSide(this Shape Shape, WhichSide Side)
    {
      LineCoordinates coor = null;

      // break out the shape as a specific type of shape.
      Line lineShape = null;
      Rectangle rectShape = null;
      if (Shape is Line)
        lineShape = Shape as Line;
      if (Shape is Rectangle)
        rectShape = Shape as Rectangle;

      switch (Side)
      {
        case WhichSide.Left:
          {
            if (lineShape != null)
            {
              if (lineShape.IsHorizontal() == true)
                coor = null;
              else
                coor = lineShape.GetCoordinates();
            }
            else if (rectShape != null)
            {
              coor = rectShape.GetSide(Side);
            }
            break;
          }

        case WhichSide.Right:
          if (lineShape != null)
          {
            if (lineShape.IsHorizontal() == true)
              coor = null;
            else
              coor = lineShape.GetCoordinates();
          }
          else if (rectShape != null)
          {
            coor = rectShape.GetSide(Side);
          }
          break;

        case WhichSide.Top:
          if (lineShape != null)
          {
            if (lineShape.IsVertical() == true)
              coor = null;
            else
              coor = lineShape.GetCoordinates();
          }
          else if (rectShape != null)
          {
            coor = rectShape.GetSide(Side);
          }
          break;

        case WhichSide.Bottom:
          if (lineShape != null)
          {
            if (lineShape.IsVertical() == true)
              coor = null;
            else
              coor = lineShape.GetCoordinates();
          }
          else if (rectShape != null)
          {
            coor = rectShape.GetSide(Side);
          }
          break;

        default:
          throw new ApplicationException("unsupported side");
      }

      if (Shape is Line)
      {
        var line = Shape as Line;
      }

      return coor;
    }

    public static string GetVisualizationInstructions(this Shape Shape)
    {
      if (Shape is Rectangle)
      {
        var shapeRect = Shape as Rectangle;

        XDocument xdoc = new XDocument(
              new XDeclaration("1.0", "utf-8", "yes"),
              new XElement("VisualizationInstructions",
                shapeRect.ToXElement("Rectangle")));

        return xdoc.ToString();
      }
      else
        return "Shape is not rectangle";
    }

    /// <summary>
    /// Determine how much the two shapes horizontally intersect each other.
    /// </summary>
    /// <param name="Shape1"></param>
    /// <param name="Shape2"></param>
    /// <returns></returns>
    public static HorizontalIntersect HorizontalIntersect(Shape Shape1, Shape Shape2)
    {
      var side1 = Shape1.GetSide(WhichSide.Top);
      var side2 = Shape2.GetSide(WhichSide.Top);

      var hi = LineCoordinates.HorizontalIntersect(side1, side2);

      return hi;
    }

    /// <summary>
    /// return true if the point is anywhere within the bounds of the shape.
    /// </summary>
    /// <param name="Shape"></param>
    /// <param name="Pos"></param>
    /// <returns></returns>
    public static bool Intersects(this Shape Shape, Point Pos)
    {
      bool intersects = false;
      var posRect = Pos.GetRectAtPoint();

      if (Shape is Line)
      {
        var uiLine = Shape as Line;
        if (uiLine.IntersectsWith(Pos))
        {
          intersects = true;
        }
      }

      else
      {
        var rect = Shape.GetBoundedRect();
        if (rect.IntersectsWith(posRect))
        {
          intersects = true;
        }
      }
      return intersects;
    }

    public static bool? IsEntirelyAbove(this Shape Shape1, Shape Shape2)
    {
      var bottomSide = Shape1.GetSide(WhichSide.Bottom);
      var topSide = Shape2.GetSide(WhichSide.Top);

      if ((bottomSide == null) || (topSide == null))
        return null;

      if (bottomSide.BottomMost < topSide.TopMost)
        return true;
      else
        return false;
    }

    public static bool? IsEntirelyBelow(this Shape Shape1, Shape Shape2)
    {
      var topSide = Shape1.GetSide(WhichSide.Top);
      var bottomSide = Shape2.GetSide(WhichSide.Bottom);

      if ((bottomSide == null) || (topSide == null))
        return null;

      if (topSide.TopMost > bottomSide.BottomMost)
        return true;
      else
        return false;
    }

    public static bool? IsEntirelyToTheLeftOf(this Shape Shape1, Shape Shape2)
    {
      var rightSide = Shape1.GetSide(WhichSide.Right);
      var leftSide = Shape2.GetSide(WhichSide.Left);

      if ((rightSide == null) || (leftSide == null))
        return null;

      if (rightSide.RightMost < leftSide.LeftMost)
        return true;
      else
        return false;
    }

    public static bool? IsEntirelyToTheRightOf(this Shape Shape1, Shape Shape2)
    {
      var leftSide = Shape1.GetSide(WhichSide.Left);
      var rightSide = Shape2.GetSide(WhichSide.Right);

      if ((rightSide == null) || (leftSide == null))
        return null;

      if (leftSide.LeftMost > rightSide.RightMost)
        return true;
      else
        return false;
    }

    public static bool IsLeftOf(this Shape Shape1, Shape Shape2)
    {
      var side1 = Shape1.GetSide(WhichSide.Left);
      var side2 = Shape2.GetSide(WhichSide.Left);

      if (side1.RightMost < side2.LeftMost)
        return true;
      else
        return false;
    }

    /// <summary>
    /// remove the record of the connection from this shape to the ToShape.
    /// </summary>
    /// <param name="Shape"></param>
    /// <param name="ToShape"></param>
    public static void RemoveConnection(this Shape Shape, Shape ToShape)
    {
      if ((Shape.Tag != null) && (Shape.Tag is DrawMore))
      {
        (Shape.Tag as DrawMore).RemoveConnection(ToShape);
      }
    }

    public static void StoreConnection(this Shape Shape, Line ConnectLine, Shape ToShape)
    {
      if (Shape.Tag == null)
        Shape.Tag = new DrawMore(Shape);

      // error if the Tag does not contain a DrawMore object.
      if ((Shape.Tag is DrawMore) == false)
      {
        throw new ApplicationException("shape tag is not DrawMore object");
      }

      // ShapeConnection object stores the shape connected to and the line that makes
      // the connection.
      ShapeConnection sc = new ShapeConnection()
      {
        ConnectLine = ConnectLine,
        ToShape = ToShape
      };

      // add to the list of connected to shapes.
      (Shape.Tag as DrawMore).ConnectedToShapes.Add(sc);
    }

    public static void StoreConnection(
      this Shape Shape, ConnectionRoute Route, Shape ToShape)
    {
      if (Shape.Tag == null)
        Shape.Tag = new DrawMore(Shape);

      // error if the Tag does not contain a DrawMore object.
      if ((Shape.Tag is DrawMore) == false)
      {
        throw new ApplicationException("shape tag is not DrawMore object");
      }

      // ShapeConnection object stores the shape connected to and the line that makes
      // the connection.
      ShapeConnection sc = new ShapeConnection()
      {
        ConnectRoute = Route, 
        ToShape = ToShape
      };

      // add to the list of connected to shapes.
      (Shape.Tag as DrawMore).ConnectedToShapes.Add(sc);
    }

    public static XElement ToXElement(this Shape Shape, XName Name)
    {
      XElement rv = null;
      string shapeType = null;
      XElement actElem = null;

      if (Shape is Rectangle)
      {
        shapeType = "Rectangle";
        actElem = (Shape as Rectangle).ToXElement("Rectangle");
      }
      else
      {
        shapeType = "none";
        actElem = new XElement("none");
      }

      rv = new XElement(Name,
        new XElement("ShapeType", shapeType),
        actElem) ;

      return rv;
    }

    /// <summary>
    /// Determine how much the two shapes vertically intersect each other.
    /// </summary>
    /// <param name="Shape1"></param>
    /// <param name="Shape2"></param>
    /// <returns></returns>
    public static VerticalIntersect VerticalIntersect(Shape Shape1, Shape Shape2)
    {
      var side1 = Shape1.GetSide(WhichSide.Left);
      var side2 = Shape2.GetSide(WhichSide.Left);

      var vi = LineCoordinates.VerticalIntersect(side1, side2);

      return vi;
    }
  }
}
