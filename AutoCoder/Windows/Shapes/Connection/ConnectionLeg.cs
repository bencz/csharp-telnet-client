using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AutoCoder.Windows.Lines;
using System.Windows.Shapes;
using AutoCoder.Core.Enums;
using AutoCoder.Ext.Shapes;
using AutoCoder.Text;
using System.Windows;
using System.Xml.Linq;
using AutoCoder.Ext.System.Text;

namespace AutoCoder.Windows.Shapes.Connection
{
  /// <summary>
  /// a shape ( most commonly a line ) that is a part of a connection route
  /// between shapes.
  /// </summary>
  public class ConnectionLeg
  {

    /// <summary>
    /// the direction of this leg, from the from-shape to the to-shape.
    /// </summary>
    public WhichDirection Direction
    { get; set; }

    /// <summary>
    /// the line draw to represent this connection leg.
    /// </summary>
    public Line DrawnLine
    {
      get;
      set;
    }

    public Point End
    {
      get
      {
        return this.LineCoor.OtherPoint(this.Start);
      }
    }

    /// <summary>
    /// the LineCoordinates of the connection leg line.
    /// </summary>
    public LineCoordinates LineCoor
    { get; set; }

    /// <summary>
    /// the linked list node of this ConnectionLeg within the ConnectionRoute.
    /// </summary>
    public LinkedListNode<ConnectionLeg> Node
    { get; set; }

    public Point Start
    { get; set; }

    /// <summary>
    /// Return a ConnectionLeg which contains draw instructions for a line that runs 
    /// from the DepartureSide of a shape to the location of the orbit that runs 
    /// around the shape.
    /// </summary>
    /// <param name="FromShape"></param>
    /// <param name="DepartureSide"></param>
    /// <returns></returns>
    public static ConnectionLeg DrawLegToOrbit(Shape FromShape, WhichSide DepartureSide)
    {
      var sideCoor = FromShape.GetSide(DepartureSide);
      var midPt = sideCoor.MidPoint;
      double lgthToOrbit = 30;
      ConnectionLeg leg = null ;

      // draw line depending on the side of the shape.
      LineCoordinates legCoor = null;
      var whichDir = DepartureSide.ToDirection();
      legCoor = new LineCoordinates(midPt, new LineVector(lgthToOrbit, whichDir));

      // line is off the canvas. do not create a connection leg.
      if ((legCoor.Start.X < 0) || (legCoor.Start.Y < 0) || (legCoor.End.X < 0)
        || (legCoor.End.Y < 0))
      {
        leg = null;
      }

      else
      {
        leg = new ConnectionLeg()
        {
          Start = midPt,
          LineCoor = legCoor,
          Direction = whichDir
        };
      }

      return leg;
    }

    public static ConnectionLeg DrawLegFromPoint(Point FromPoint, LineVector Vector)
    {
      // draw line depending on the side of the shape.
      var legCoor = new LineCoordinates(FromPoint, Vector) ;

      var leg = new ConnectionLeg()
      {
        Start = FromPoint,
        LineCoor = legCoor,
        Direction = Vector.Direction
      };

      return leg;
    }

    public string GetVisualizationInstructions()
    {
      XDocument xdoc = new XDocument(
            new XDeclaration("1.0", "utf-8", "yes"),
            new XElement("VisualizationInstructions",
              this.ToXElement("ConnectionLeg")));

      return xdoc.ToString();
    }

    public override string ToString()
    {
      StringBuilder sb = new StringBuilder();
      sb.Append(this.Direction.ToString());
      sb.SentenceAppend("Start:" + this.Start.ToString());
      if (this.LineCoor != null)
        sb.SentenceAppend("End:" + this.LineCoor.OtherPoint(this.Start));
      return sb.ToString();
    }

    public XElement ToXElement(XName Name)
    {
      var rv = new XElement(Name,
        new XElement("Direction", this.Direction.ToString( )),
        this.LineCoor.ToXElement("LineCoordinates")) ;

      return rv;
    }
  }
}
