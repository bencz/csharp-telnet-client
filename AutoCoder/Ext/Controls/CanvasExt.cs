using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows;
using AutoCoder.Text;
using System.Windows.Shapes;
using System.Windows.Media;
using AutoCoder.Ext.Shapes;
using AutoCoder.Ext.System;
using AutoCoder.Core.Enums;
using AutoCoder.Ext.Shapes.LineClasses;
using AutoCoder.Windows.Lines;
using AutoCoder.Windows.Shapes.Connection;
using AutoCoder.Windows.Primitives;
using AutoCoder.Ext.Windows;
using System.Diagnostics;

namespace AutoCoder.Ext.Controls
{
  public static class CanvasExt
  {

    /// <summary>
    /// Draw a circle of the specified width centered at the specified point.
    /// </summary>
    /// <param name="AtPoint"></param>
    /// <param name="Width"></param>
    /// <returns></returns>
    public static Ellipse DrawCircle(
      this Canvas Canvas1, Point AtPoint, double Width,
      Brush Brush)
    {
      Ellipse elip = new Ellipse();
      elip.Fill = Brush;
      elip.Width = Width;
      elip.Height = Width;

      // circle is centered at the specified point. Calc the left and top edge from the
      // center position and circle width.
      Canvas.SetLeft(elip, AtPoint.X - (Width / 2));
      Canvas.SetTop(elip, AtPoint.Y - (Width / 2));

      Canvas1.Children.Add(elip);

      return elip;
    }

    public static void DrawGridLines(
      this Canvas Canvas1, double Spacing, string Tag = null)
    {
      var lineChar = new LineCharacteristics()
      {
        StrokeThickness = 1,
      };

      // draw horizontal grid lines.
      double ht = Canvas1.ActualHeight;
      for (var ix = 0.0; ix < Canvas1.ActualHeight; ix = ix + Spacing)
      {
        var start = new Point(0, ix);
        var coor = new LineCoordinates(start, new Point(Canvas1.ActualWidth, ix));
        var line = Canvas1.DrawLine(coor, lineChar);
        if (Tag != null)
          line.Tag = Tag;
      }

      // draw vertical grid lines.
      for (var ix = 0.0; ix < Canvas1.ActualWidth; ix += Spacing)
      {
        var start = new Point(ix, 0);
        var coor = new VerticalLineCoordinates(start, Canvas1.ActualHeight);
        var line = Canvas1.DrawLine(coor, lineChar);
        if (Tag != null)
          line.Tag = Tag;
      }
    }

    /// <summary>
    /// Draw a rectangle on the canvas to highlight something.
    /// </summary>
    /// <param name="DrawOnCanvas"></param>
    /// <param name="UpperLeft"></param>
    /// <param name="Dim"></param>
    /// <param name="TagName"></param>
    public static void DrawHighlightRectangle( 
      this Canvas DrawOnCanvas, Point UpperLeft, Size Dim, string TagName)
    {

      // remove the current highlight rect from the canvas surface.
      if (TagName.IsNullOrEmpty() == false)
        DrawOnCanvas.Children.RemoveByTag(TagName);

      // draw the highlight rect.
      Rectangle rect = new Rectangle();
      rect.Stroke = Brushes.Green;
      rect.StrokeThickness = 3;
      rect.Fill = Brushes.Transparent;
      rect.Width = Dim.Width;
      rect.Height = Dim.Height;
      rect.Tag = TagName;

      DrawOnCanvas.Children.Add(rect);
      int zindex = DrawOnCanvas.Children.Count;
      Canvas.SetZIndex(rect, zindex);
      Canvas.SetLeft(rect, UpperLeft.X);
      Canvas.SetTop(rect, UpperLeft.Y);
    }

    public static Line DrawLine(this Canvas InCanvas, LineCoordinates LineCoor)
    {
      Line newLine = new Line();
      newLine.Stroke = Brushes.Black;
      newLine.Fill = Brushes.Black;
      newLine.StrokeLineJoin = PenLineJoin.Bevel;
      newLine.X1 = LineCoor.Start.X;
      newLine.Y1 = LineCoor.Start.Y;
      newLine.X2 = LineCoor.End.X;
      newLine.Y2 = LineCoor.End.Y;
      newLine.StrokeThickness = 2;
      InCanvas.Children.Add(newLine);

      return newLine;
    }

    public static Line DrawLine(this Canvas InCanvas, LineCoordinates LineCoor, 
      LineCharacteristics LineChar)
    {
      Line newLine = new Line();
      newLine.Stroke = LineChar.Stroke;
      newLine.Fill = LineChar.Fill;
      newLine.StrokeLineJoin = LineChar.StrokeLineJoin;
      newLine.X1 = LineCoor.Start.X;
      newLine.Y1 = LineCoor.Start.Y;
      newLine.X2 = LineCoor.End.X;
      newLine.Y2 = LineCoor.End.Y;
      newLine.StrokeThickness = LineChar.StrokeThickness;
      InCanvas.Children.Add(newLine);

      return newLine;
    }

    public static void DrawLinesOfRoute(this Canvas InCanvas, ConnectionRoute Route)
    {
      foreach (var leg in Route.LegList)
      {
        var line = InCanvas.DrawLine(leg.LineCoor);
        leg.DrawnLine = line ;
      }
    }

    public static TextBlock DrawText( this Canvas Canvas, string Text, Point Pos )
    {
      var tb = new TextBlock() ;
      tb.Text = Text;

      Canvas.Children.Add(tb);

      Canvas.SetLeft(tb, Pos.X);
      Canvas.SetTop(tb, Pos.Y);

      return tb;
    }

    public static void RemoveLinesOfRoute(this Canvas InCanvas, ConnectionRoute Route)
    {
      foreach( var leg in Route.LegList)
      {
        var line = leg.DrawnLine ;
        InCanvas.Children.Remove(line) ;
      }
    }

    public static ConnectionRoute DrawRouteBetweenShapes(
      this Canvas InCanvas, Shape Shape1, Shape Shape2)
    {
      ConnectionRoute selectedRoute = null;

      // for each direction from the from shape.
      var possibleRoutes = new List<ConnectionRoute>();
      foreach (var dir in WhichDirectionExt.Directions())
      {
        var side = new ShapeSide(Shape1, dir.ToSide());

        var route = new ConnectionRoute(side, Shape2);

        {
          var leg = route.DrawInitialLegToShape();
          if (leg == null)
            continue;
          route.AddLeg(leg);
        }
        while (true)
        {
          if (Shape2.Intersects(route.LastLeg.End))
            break;

          var leg = route.DrawLegToShape();

          route.AddLeg(leg);
        }

        // add the route to the possible routes list.
        possibleRoutes.Add(route);
      }

      // select the route with the fewest number of legs.
      foreach (var route in possibleRoutes)
      {
        if (selectedRoute == null)
          selectedRoute = route;
        else if (route.LegCount < selectedRoute.LegCount)
          selectedRoute = route;
      }

      if (selectedRoute != null)
      {
        InCanvas.DrawLinesOfRoute(selectedRoute);
        Shape1.StoreConnection(selectedRoute, Shape2);
        Shape2.StoreConnection(selectedRoute, Shape1);
      }

      return selectedRoute;
    }

    public static ConnectionRoute StartConnectionRouteToShape(
      Shape FromShape, WhichSide StartSide, Shape ToShape)
    {
      var fromSide = new ShapeSide(FromShape, StartSide) ;
      ConnectionRoute route = new ConnectionRoute(fromSide, ToShape);

      return route;
    }

    public static LineCoordinates DrawLineBetweenShapes(
      this Canvas InCanvas, Shape Shape1, WhichSide WhichSide1, 
      Shape Shape2, WhichSide WhichSide2 )
    {
      LineCoordinates betLine = null ;

      var coor1 = Shape1.GetSide(WhichSide1);
      var coor2 = Shape2.GetSide(WhichSide2);

      // line runs from the midpoint of one side to the mid point of the other.
      betLine = new LineCoordinates(coor1.MidPoint, coor2.MidPoint);

      // draw a vertical line between the shapes.
      if ((WhichSide1 == WhichSide.Bottom) && (WhichSide2 == WhichSide.Top))
      {
        var hi = LineCoordinates.HorizontalIntersect(coor1, coor2);
        if (hi.Length > 0)
        {
          double centeredIntersect1 = hi.Line1Ofs + (hi.Length / 2) ;
          Point fromPt = coor1.CalcHorizontalPointOnLine(centeredIntersect1);
          double centeredIntersect2 = hi.Line2Ofs + (hi.Length / 2);
          Point toPt = coor2.CalcHorizontalPointOnLine(centeredIntersect2);
          betLine = new LineCoordinates(fromPt, toPt);
        }
      }

      // draw a horizontal line between the shapes.
      else if ((WhichSide1 == WhichSide.Right) && (WhichSide2 == WhichSide.Left))
      {
        var vi = LineCoordinates.VerticalIntersect(coor1, coor2);
        if (vi.Length > 0)
        {
          double centeredIntersect1 = vi.Line1Ofs + (vi.Length / 2);
          Point fromPt = coor1.CalcVerticalPointOnLine(centeredIntersect1);
          double centeredIntersect2 = vi.Line2Ofs + (vi.Length / 2);
          Point toPt = coor2.CalcVerticalPointOnLine(centeredIntersect2);
          betLine = new LineCoordinates(fromPt, toPt);
        }
      }

      // get horizontal intersect between the two lines
      // return the intersect as from and to pos, 
      // get center pos of the horizontal intersect

      return betLine;
    }

    /// <summary>
    /// Draw a rectangle on the canvas.
    /// </summary>
    /// <param name="Canvas1"></param>
    /// <param name="UpperLeft"></param>
    /// <param name="Dim"></param>
    /// <param name="FillBrush"></param>
    /// <param name="StrokeBrush"></param>
    /// <param name="StrokeThickness"></param>
    /// <returns></returns>
    public static Rectangle DrawRectangle(
      this Canvas Canvas1, Point UpperLeft, Size Dim, 
      Brush FillBrush, Brush StrokeBrush,
      double StrokeThickness = 1.00)
    {
      Rectangle rect = new Rectangle();
      rect.Stroke = StrokeBrush;
      rect.StrokeThickness = StrokeThickness;
      rect.Fill = FillBrush;
      rect.Width = Dim.Width;
      rect.Height = Dim.Height;

      Canvas1.Children.Add(rect);
      Canvas.SetLeft(rect, UpperLeft.X);
      Canvas.SetTop(rect, UpperLeft.Y);

      return rect;
    }

    /// <summary>
    /// map out a route from shape1 to shape2, starting from a specific side of shape1.
    /// </summary>
    /// <param name="InCanvas"></param>
    /// <param name="Shape1"></param>
    /// <param name="Shape2"></param>
    /// <param name="DepartureSide"></param>
    /// <param name="Route"></param>
    public static void DrawRouteBetweenShapes(
      this Canvas InCanvas, Shape Shape1, Shape Shape2,
      WhichSide DepartureSide,
      ConnectionRoute Route)
    {

      // vertical and horizontal intersection of the two shapes.
      var vi = ShapeExt.VerticalIntersect(Shape1, Shape2);
      var hi = ShapeExt.HorizontalIntersect(Shape1, Shape2);

      if (DepartureSide == WhichSide.Left)
      {
        LineCoordinates coor = null;
        if (Shape1.IsEntirelyToTheRightOf(Shape2).GetValueOrDefault() && (vi.Length > 0))
          coor = InCanvas.DrawLineBetweenShapes(
            Shape1, WhichSide.Left, Shape2, WhichSide.Right);
        else
        {
          // draw a short line from the shape to the next available orbit location
          // around the from shape.
          var leg = ConnectionLeg.DrawLegToOrbit(Shape1, DepartureSide);
          Route.AddLeg(leg);
        }
      }

    }

    public static List<Shape> FindShapesThatIntersectLine(
      this Canvas Canvas, LineCoordinates Line)
    {
      List<Shape> shapes = new List<Shape>( ) ;
      foreach (var elem in Canvas.Children)
      {
        var uiShape = elem as Shape;
        if (uiShape != null)
        {
          bool doesIntersect = Line.DoesIntersect(uiShape);
          if (doesIntersect == true)
            shapes.Add(uiShape);
        }
      }

      return shapes;
    }

    /// <summary>
    /// find the element located at the Pos within the Canvas.
    /// Return a reference to that element. Also return the vector between the
    /// upper left of the Canvas and the upper left of the found element.
    /// </summary>
    /// <param name="Canvas"></param>
    /// <param name="Pos"></param>
    /// <returns></returns>
    public static Tuple<FrameworkElement,Vector?> FindFrameworkElementAtPoint(
      this Canvas Canvas, Point Pos)
    {
      FrameworkElement found = null;
      Vector? rltvToElement = null;

      var posRect = Pos.GetRectAtPoint();
      Debug.Print("mouse click pos. " + posRect.ToString()) ;

      foreach (var elem in Canvas.Children)
      {
        var fe = elem as FrameworkElement;
        if (fe != null)
        {
          var rect = fe.GetCanvasRect();
          Debug.Print("frameworkElement:" + fe.ToString() + " location:" + rect.ToString());

          if ( rect.Contains(posRect))
          {
            found = fe;
            rltvToElement = posRect.TopLeft - rect.TopLeft;
          }
        }
      }

      return new Tuple<FrameworkElement, Vector?>(found, rltvToElement);
    }

    public static Tuple<Shape, string> FindShapeAtPoint(this Canvas Canvas, Point Pos)
    {
      Shape found = null;
      var posRect = Pos.GetRectAtPoint();
      string comboDebug = null;

      foreach (var elem in Canvas.Children)
      {
        var uiShape = elem as Shape;
        if (uiShape != null)
        {

          if (uiShape is Line)
          {
            var uiLine = uiShape as Line;
            if (uiLine.IntersectsWith(Pos))
            {
              found = uiShape;
              break;
            }
          }

          else if (uiShape is Path)
          {
            var uiPath = uiShape as Path;

            var rectGeom = new RectangleGeometry();
            rectGeom.Rect = new Rect(Pos, new Size(1, 1));
            var rp = new Path();
            rp.Fill = Brushes.Black;
            rp.Stroke = Brushes.Black;
            rp.StrokeThickness = 1;
            rp.Data = rectGeom;

            var xx = uiPath.RenderedGeometry.FillContainsWithDetail(rectGeom);
            if (xx == IntersectionDetail.Intersects)
            {
              found = uiShape;
              break;
            }
          }

          else
          {
            var rect = CanvasExt.GetShapeRect(uiShape);
            if (rect.IntersectsWith(posRect))
            {
              found = uiShape;
              break;
            }
          }
        }
      }

      return new Tuple<Shape, string>(found, comboDebug);
    }

    /// <summary>
    /// Find the first shape on the canvas at the point. If no shape found, look for
    /// shapes that are between the PriorPos and the Pos. Shapes between the two
    /// positions are passed over shapes. Return the estimated pass over point.
    /// </summary>
    /// <param name="Canvas"></param>
    /// <param name="Pos"></param>
    /// <param name="PriorPos"></param>
    /// <returns></returns>
    public static Tuple<Shape, Point?> FindShapeAtPoint(
      this Canvas Canvas, Point Pos, Point? PriorPos)
    {
      Shape found = null;
      Point? estPassOverPoint = null;
      var posRect = Pos.GetRectAtPoint();

      foreach (var elem in Canvas.Children)
      {
        var uiShape = elem as Shape;
        if (uiShape != null)
        {

          if (uiShape is Line)
          {
            var uiLine = uiShape as Line;
            if (uiLine.IntersectsWith(Pos))
            {
              found = uiShape;
              break;
            }
          }

          else
          {
            var rect = CanvasExt.GetShapeRect(uiShape);
            if (rect.IntersectsWith(posRect))
            {
              found = uiShape;
              break;
            }
          }
        }
      }

      return new Tuple<Shape, Point?>(found, estPassOverPoint);
    }

    /// <summary>
    /// return the upper left position of the child UIElement within the Canvas
    /// parent of that element.
    /// </summary>
    /// <param name="Child"></param>
    /// <returns></returns>
    public static Point GetUpperLeft(UIElement Child)
    {
      var x = Canvas.GetLeft(Child);
      var y = Canvas.GetTop(Child);
      Point pos = new Point(x, y);
      return pos;
    }

    public static void SetUpperLeft(UIElement Child, Point UpperLeft)
    {
      Canvas.SetLeft(Child, UpperLeft.X);
      Canvas.SetTop(Child, UpperLeft.Y);
    }

#if skip
    public static Rect GetLineRect(Line Elem)
    {
      var left = Elem.GetLeft();
      var top = Elem.GetTop();
      var width = Math.Abs(Elem.X2 - Elem.X1) + 1.00;
      var height = Math.Abs(Elem.Y2 - Elem.Y1) + 1.00;
      var rect = new Rect(left, top, width, height);
      return rect;
    }
#endif

    public static Rect GetShapeRect(Shape Elem)
    {
      if (Elem is Line)
        return (Elem as Line).GetBoundedRect( ) ;
      else
      {
        var left = Canvas.GetLeft(Elem);
        var top = Canvas.GetTop(Elem);
        var width = Elem.ActualWidth;
        var height = Elem.ActualHeight;
        var rect = new Rect(left, top, width, height);
        return rect;
      }
    }

    /// <summary>
    /// return a list of all the children of the canvas which are shapes and which have
    /// a Tag value that matches the search value. 
    /// </summary>
    /// <param name="Canvas1"></param>
    /// <param name="Tag"></param>
    /// <returns></returns>
    public static List<Shape> GetShapesByTag(this Canvas Canvas1, string Tag)
    {
      List<Shape> elems = new List<Shape>();

      foreach (var item in Canvas1.Children)
      {
        if (item is Shape)
        {
          var shapeItem = item as Shape;

          if ((shapeItem.Tag != null) && (shapeItem.Tag is String))
          {
            var shapeTag = shapeItem.Tag as String;
            if (shapeTag == Tag)
            {
              elems.Add(shapeItem);
            }
          }
        }
      }

      return elems;
    }

    public static void MoveShape(this Canvas Canvas, Shape Elem, Vector Adj)
    {
      var x = Canvas.GetLeft(Elem) + Adj.X;
      var y = Canvas.GetTop(Elem) + Adj.Y;
      Canvas.SetLeft(Elem, x);
      Canvas.SetTop(Elem, y);

      // shape contains DrawMore info. Redraw any connection lines from this shape to
      // other shapes.
      if ((Elem.Tag != null) && (Elem.Tag is DrawMore))
      {
        (Elem.Tag as DrawMore).RedrawConnections(Canvas);
      }
    }

    /// <summary>
    /// remove all the children of the canvas which are shapes and which have a tag
    /// value that matches the search value.
    /// </summary>
    /// <param name="Canvas1"></param>
    /// <param name="Tag"></param>
    /// <returns></returns>
    public static int RemoveShapesByTag(this Canvas Canvas1, string Tag)
    {
      var items = Canvas1.GetShapesByTag(Tag);
      if (items.Count > 0)
      {
        foreach (var item in items)
        {
          Canvas1.Children.Remove(item);
        }
      }
      return items.Count;
    }
  }
}
