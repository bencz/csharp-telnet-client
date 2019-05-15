using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Xml.Linq;
using AutoCoder.Ext.Shapes.LineClasses;
using AutoCoder.Windows.Lines;
using AutoCoder.Ext;
using AutoCoder.Ext.Windows;

namespace AutoCoder.Windows.Triangles
{
  /// <summary>
  /// the location and angle of the vertex of a triangle.
  /// </summary>
  public class TriangleVertex
  {
    public TriangleVertex()
    {
    }

    public double Angle
    { get; set; }

    public Point Location
    { get; set; }

    /// <summary>
    /// the line of the triangle that is opposite the vertex
    /// </summary>
    public LineCoordinates Line
    { get; set; }

    public override string ToString()
    {
      return "Angle:" + this.Angle + " Location:" + this.Location;
    }
  }

  public static class TriangleVertexExt
  {
    public static XElement ToXElement(this TriangleVertex Vertex, XName Name)
    {
      if (Vertex == null)
        return new XElement(Name, null);
      else
      {
        XElement xe = new XElement(Name,
          new XElement("Angle", Vertex.Angle),
            Vertex.Location.ToXElement("Location"),
            Vertex.Line.ToXElement("Line")
            );
        return xe;
      }
    }

    public static TriangleVertex ToTriangleVertex(
      this XElement Elem, XNamespace Namespace)
    {
      TriangleVertex vertex = null;
      if (Elem != null)
      {
        var angle = Elem.Element(Namespace + "Angle").DoubleOrDefault(0).Value;
        var location = Elem.Element(Namespace + "Location").ToPoint(Namespace);
        var line = Elem.Element(Namespace + "Line").ToLineCoordinates(Namespace);
        vertex = new TriangleVertex( )
        {
          Angle = angle,
          Location = location,
          Line = line
        };
      }
      return vertex;
    }
  }
}
