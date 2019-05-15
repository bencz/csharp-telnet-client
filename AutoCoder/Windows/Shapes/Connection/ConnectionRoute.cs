using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Shapes;
using System.Windows;
using AutoCoder.Ext.Shapes;
using AutoCoder.Core.Enums;
using AutoCoder.Windows.Lines;
using AutoCoder.Windows.Primitives;
using AutoCoder.Text;
using System.Xml.Linq;
using AutoCoder.Ext.System.Text;

namespace AutoCoder.Windows.Shapes.Connection
{
  /// <summary>
  /// a sequence of lines, curves and other shapes that are used to connect two or more
  /// shapes to each other.
  /// </summary>
  public class ConnectionRoute
  {

    public ShapeSide FromSide
    {
      get;
      set;
    }

    public Shape ToShape
    { get; set; }

    public ConnectionRoute( ShapeSide FromSide, Shape ToShape)
    {
      this.FromSide = FromSide;
      this.ToShape = ToShape;
    }

    public void AddLeg(ConnectionLeg Connector)
    {
      var node = new LinkedListNode<ConnectionLeg>(Connector);
      Connector.Node = node;
      this.LegList.AddLast(node);
    }

    public ConnectionLeg DrawInitialLegToShape()
    {
      ConnectionLeg leg = null ;
      LineCoordinates legCoor = null;
      Point? legStart = null;

      // starting side
      var fromSide = this.FromSide;
      var toShapeInfo = new SideToShapeInfo(fromSide, this.ToShape);

      // the direction of the initial line.
      var dir = fromSide.WhichSide.ToDirection();

      // draw horizontal line staight to the target shape.
      if (toShapeInfo.HorizDirection.Equals(dir)
        && (toShapeInfo.VertIntersect != null)
        && (toShapeInfo.VertIntersect.Length > 0))
      {
        var vi = toShapeInfo.VertIntersect;
        double centeredIntersect1 = vi.Line1Ofs + (vi.Length / 2);
        legStart = vi.Line1.CalcVerticalPointOnLine(centeredIntersect1);
        double centeredIntersect2 = vi.Line2Ofs + (vi.Length / 2);
        Point toPt = vi.Line2.CalcVerticalPointOnLine(centeredIntersect2);
        legCoor = new LineCoordinates(legStart.Value, toPt);
      }

      // draw vertical staight to the target shape.
      else if (toShapeInfo.VertDirection.Equals(dir)
        && (toShapeInfo.HorizIntersect != null)
        && (toShapeInfo.HorizIntersect.Length > 0))
      {
        var hi = toShapeInfo.HorizIntersect;
        double centeredIntersect1 = hi.Line1Ofs + (hi.Length / 2);
        legStart = hi.Line1.CalcHorizontalPointOnLine(centeredIntersect1);
        double centeredIntersect2 = hi.Line2Ofs + (hi.Length / 2);
        Point toPt = hi.Line2.CalcHorizontalPointOnLine(centeredIntersect2);
        legCoor = new LineCoordinates(legStart.Value, toPt);
      }

        // drawing a vertical line. 
        // The end destination shape is in the direction of the vertical line.
        // ( ex: drawing the line down and the shape is below the side )
        // Draw the line to the mid point of the vertical side of the to_shape. 
      else if (toShapeInfo.VertDirection.Equals(dir))
      {
        // start the leg at the mid point of the start from side.
        legStart = fromSide.LineCoor.MidPoint;

        // draw the line down ( or up ) to the mid point of the vertical side
        // ( either left or right side ) of the "to shape".
        var toY = toShapeInfo.VertSide.MidPoint.Y;
        var legEnd = new Point(legStart.Value.X, toY);
        legCoor = new LineCoordinates(legStart.Value, legEnd);
      }

        // drawing a horizontal line. 
      // The end destination shape is in the direction of the horizontal line.
      // ( ex: drawing the line to the right and the shape is to the righ of the side )
      // Draw the line to the mid point of the horizonntal side of the to_shape. 
      else if (toShapeInfo.HorizDirection.Equals(dir))
      {
        // start the leg at the mid point of the start from side.
        legStart = fromSide.LineCoor.MidPoint;

        // draw the line left ( or right ) to the mid point of the horiz side
        var toX = toShapeInfo.HorizSide.MidPoint.X;
        var legEnd = new Point(toX, legStart.Value.Y);

        legCoor = new LineCoordinates(legStart.Value, legEnd);
      }

      // create the leg.
      if (legCoor != null)
      {
        leg = new ConnectionLeg()
        {
          Direction = dir,
          LineCoor = legCoor,
          Start = legStart.Value
        };
      }

      // draw a short line from the shape to the next available orbit location
      // around the from shape.
      else
      {
        leg = ConnectionLeg.DrawLegToOrbit(this.FromSide.Shape, this.FromSide.WhichSide) ;
      }

      return leg ;
    }

    /// <summary>
    /// Compute the direction of and then draw the next leg of the connection route that
    /// connects one shape to another.
    /// </summary>
    /// <param name="ToShape"></param>
    /// <param name="Route"></param>
    /// <returns></returns>
    public ConnectionLeg DrawLegToShape( )
    {
      ConnectionLeg leg;

      // the last leg of the route so far.
      var lastLeg = this.LastLeg;

      // the end point of the leg.
      var startPt = lastLeg.End;

      // the direction from the current end point of the route to the ToShape.
      var rv = ToShape.DirectionToShape(startPt);
      var horizDir = rv.HorizDirection;
      var vertDir = rv.VertDirection;

      // eliminate the direction of the last leg of the route
      if (lastLeg.Direction.IsVertical())
        vertDir = null;
      else if (lastLeg.Direction.IsHorizontal())
        horizDir = null;

      // draw a leg in the direction
      if (horizDir != null)
      {
        leg = DrawLegToShape(this.FromSide.Shape, this.ToShape, lastLeg, horizDir.Value);
      }
      else if (vertDir != null)
      {
        leg = DrawLegToShape(this.FromSide.Shape, this.ToShape, lastLeg, vertDir.Value);
      }
      else if (lastLeg.Direction.IsHorizontal())
      {
        leg = DrawLegToShape(
          this.FromSide.Shape, this.ToShape, lastLeg, WhichDirection.Up);
      }
      else
      {
        leg = DrawLegToShape(
          this.FromSide.Shape, this.ToShape, lastLeg, WhichDirection.Left);
      }

      return leg;
    }

    public Tuple<WhichDirection?,WhichDirection?> DrawLegToShape_GetDirection()
    {
      // the last leg of the route so far.
      var lastLeg = this.LastLeg;

      // the end point of the leg.
      var startPt = lastLeg.End;

      // the direction from the current end point of the route to the ToShape.
      var rv = ToShape.DirectionToShape(startPt);
      var horizDir = rv.HorizDirection;
      var vertDir = rv.VertDirection;

      // eliminate the direction of the last leg of the route
      if (lastLeg.Direction.IsVertical())
        vertDir = null;
      else if (lastLeg.Direction.IsHorizontal())
        horizDir = null;

      return new Tuple<WhichDirection?, WhichDirection?>(horizDir, vertDir);
    }

    public static ConnectionLeg DrawLegToShape(
      Shape FromShape, Shape ToShape, ConnectionLeg LastLeg,
      WhichDirection Direction)
    {
      ConnectionLeg leg = null;
      LineCoordinates legCoor = null;

      var toShapeInfo = ToShape.DirectionToShape(LastLeg.End);

      // start point of the leg.
      var legStart = LastLeg.End;

      // attempt to draw a direct line to the shape.
      if (legCoor == null)
      {
        legCoor = ConnectionRoute.TryDrawDirectLineToShape(
          toShapeInfo, legStart, Direction);
      }

      // drawing a vertical line. draw it to the halfway point of the vertical side
      // of the shape.
      if ((legCoor == null) 
        && Direction.IsVertical()
        && (toShapeInfo.VertDirection.Equals(Direction)))
      {
        var toPoint = new Point(legStart.X, toShapeInfo.VertSide.MidPoint.Y);
        legCoor = new LineCoordinates(legStart, toPoint);
      }

      // drawing a horizontal line. draw it to the halfway point of the horiz side of
      // the shape.
      if ((legCoor == null)
        && Direction.IsHorizontal()
        && (toShapeInfo.HorizDirection.Equals(Direction)))
      {
        var toPoint = new Point(toShapeInfo.HorizSide.MidPoint.X, legStart.Y);
        legCoor = new LineCoordinates(legStart, toPoint);
      }

      // leg not drawn. The current leg is part of an orbit around a shape. Have to draw to 
      // the next corner of the orbit.
      if (legCoor == null)
      {
        var rectMore = ShapeMore.Construct( FromShape ) ;
        legCoor = rectMore.DrawLineToOrbitCorner(legStart, Direction);
      }

      leg = new ConnectionLeg()
      {
        Direction = Direction,
        LineCoor = legCoor,
        Start = legStart
      };

      return leg;
    }

    public string GetVisualizationInstructions()
    {
      XDocument xdoc = new XDocument(
            new XDeclaration("1.0", "utf-8", "yes"),
            new XElement("VisualizationInstructions",
              this.ToXElement("ConnectionRoute")));

      return xdoc.ToString();
    }

    public Point End
    { get; set; }


    public ConnectionLeg LastLeg
    {
      get
      {
        var leg = this.LegList.Last;
        return leg.Value;
      }
    }

    LinkedList<ConnectionLeg> _LegList;

    /// <summary>
    ///  the number of legs in the route.
    /// </summary>
    public int LegCount
    {
      get
      {
        return this.LegList.Count;
      }
    }

    /// <summary>
    /// the shapes that constitute the path
    /// </summary>
    public LinkedList<ConnectionLeg> LegList
    {
      get
      {
        if (_LegList == null)
          _LegList = new LinkedList<ConnectionLeg>();
        return _LegList;
      }
    }

    public Point Start
    {
      get;
      set;
    }

    public bool ToShapeReached
    {
      get
      {
        var lastLeg = this.LastLeg;
        if (lastLeg != null)
        {
          if (this.ToShape.Intersects(lastLeg.End))
            return true;
        }
        return false;
      }
    }

    public override string ToString()
    {
      StringBuilder sb = new StringBuilder();
      sb.Append("From " + this.FromSide.WhichSide + " " + this.FromSide.LineCoor.ToString());
      sb.SentenceAppend(this.LegList.Count + " legs.");
      return sb.ToString();
    }

    public XElement ToXElement(XName Name)
    {
      var rv = new XElement(Name,
        this.FromSide.ToXElement("FromSide"),
        this.ToShape.ToXElement("ToShape"),
        new XElement("LegList",
          from x in this.LegList
          select x.ToXElement("Leg")));

      return rv;
    }

    public static LineCoordinates TryDrawDirectLineToShape(
      PointToShapeInfo ToShapeInfo, Point FromPoint, WhichDirection Direction)
    {
      LineCoordinates lineCoor = null;
      Point? toPoint = null;

      // The vertical direction to the shape matches the requested draw direction.
      // And a straight line can be drawn from the point to the shape. 
      if ((ToShapeInfo.VertDirection != null) 
        && (ToShapeInfo.VertDirection.Value == Direction)
        && (ToShapeInfo.HorizSide.WithinHorizontalRange(FromPoint.X)))
      {
        toPoint = new Point(FromPoint.X, ToShapeInfo.HorizSide.Start.Y);
      }

        // same as above, only in the horizontal direction.
      else if ((ToShapeInfo.HorizDirection != null) 
        && (ToShapeInfo.HorizDirection.Value == Direction)
        && (ToShapeInfo.VertSide.WithinVerticalRange(FromPoint.Y)))
      {
        toPoint = new Point(ToShapeInfo.VertSide.Start.X, FromPoint.Y);
      }

      // build the coordinates of the direct line.
      if (toPoint != null)
      {
        lineCoor = new LineCoordinates(FromPoint, toPoint.Value);
      }

      return lineCoor;
    }

    public static LineCoordinates TryDrawDirectLineToShape(
      PointToShapeInfo ToShapeInfo, LineCoordinates FromSide, WhichDirection Direction)
    {
      LineCoordinates lineCoor = null;

#if skip
      Point? toPoint = null;
      // The vertical direction to the shape matches the requested draw direction.
      // And a straight line can be drawn from the point to the shape. 
      if ((ToShapeInfo.VertDirection != null)
        && (ToShapeInfo.VertDirection.Value == Direction)
        && (ToShapeInfo.HorizSide.WithinHorizontalRange(FromPoint.X)))
      {
        toPoint = new Point(FromPoint.X, ToShapeInfo.HorizSide.Start.Y);
      }

        // same as above, only in the horizontal direction.
      else if ((ToShapeInfo.HorizDirection != null)
        && (ToShapeInfo.HorizDirection.Value == Direction)
        && (ToShapeInfo.VertSide.WithinVerticalRange(FromPoint.Y)))
      {
        toPoint = new Point(ToShapeInfo.VertSide.Start.X, FromPoint.Y);
      }

      // build the coordinates of the direct line.
      if (toPoint != null)
      {
        lineCoor = new LineCoordinates(FromPoint, toPoint.Value);
      }
#endif

      return lineCoor;
    }

    /// <summary>
    /// number of turns along the path. Use when determining if one path is more direct
    /// than another path.
    /// </summary>
    public int TurnCount
    {
      get;
      set;
    }
  }
}
