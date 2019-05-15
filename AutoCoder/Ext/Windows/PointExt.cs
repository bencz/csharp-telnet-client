using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Xml.Linq;
using AutoCoder.Windows;

namespace AutoCoder.Ext.Windows
{
  public static class PointExt
  {
    public static Point AddPoint(this Point Point1, Point Point2)
    {
      var x = Point1.X + Point2.X;
      var y = Point1.Y + Point2.Y;
      return new Point(x, y);
    }


    public static Point EndPoint(Point Start, DrawDim Length)
    {
      double xa, ya;
      if (Length.X < 0)
        xa = 1;
      else
        xa = -1;

      if (Length.Y < 0)
        ya = 1;
      else
        ya = -1;

      double epx = Start.X + Length.X + xa;
      double epy = Start.Y + Length.Y + ya;

      return new Point(epx, epy);
    }

    public static Rect GetRectAtPoint(this Point Pt)
    {
      var rect = new Rect(Pt.X, Pt.Y, 1, 1);
      return rect;
    }

    public static Rect GetCenteredRect(this Point Pt, double Margin)
    {
      var rect = new Rect(
        Pt.X - Margin, Pt.Y - Margin,
        1.00 + Margin + Margin,
        1.00 + Margin + Margin);
      return rect;
    }

    public static string GetVisualizationInstructions(this Point Value)
    {
      XDocument xdoc = new XDocument(
            new XDeclaration("1.0", "utf-8", "yes"),
            new XElement("VisualizationInstructions",
              Value.ToXElement("Point")));

      return xdoc.ToString();
    }

    /// <summary>
    /// return true if the X and Y values of the point are zero.
    /// </summary>
    /// <param name="point"></param>
    /// <returns></returns>
    public static bool IsZeroPoint(this Point point)
    {
      if ((point.X == 0) && (point.Y == 0))
        return true;
      else
        return false;
    }

    /// <summary>
    /// Compute the length, in the form of a vector, between two points.
    /// The length is the distance between two points that includes the points themselves.
    /// ( the length between 1,1 and 2,2 is 2,2. The offset would be 1,1.)
    /// When the 2nd point is to the left or is above the 1st point, the length will be
    /// negative. 
    /// </summary>
    /// <param name="Point1"></param>
    /// <param name="Point2"></param>
    /// <returns></returns>
    public static Vector LengthBetween(Point Point1, Point Point2)
    {
      double xa, ya ;
      if (Point1.X > Point2.X)
        xa = -1;
      else
        xa = 1;
      if (Point1.Y > Point2.Y)
        ya = -1;
      else
        ya = 1;
      var adj = new Vector(xa, ya);
      var lgth = Point2 - Point1 + adj;
      return lgth;
    }

    /// <summary>
    /// round both the x and y position of the point the number of decimal positions.
    /// </summary>
    /// <param name="Pt"></param>
    /// <param name="Decimals"></param>
    /// <returns></returns>
    public static Point Round(this Point Pt, int Decimals)
    {
      double x = Math.Round(Pt.X, Decimals);
      double y = Math.Round(Pt.Y, Decimals);
      return new Point(x, y);
    }


    /// <summary>
    /// subtract Point2 from Point1.
    /// </summary>
    /// <param name="Point1"></param>
    /// <param name="Point2"></param>
    /// <returns></returns>
    public static Point SubPoint(this Point Point1, Point Point2)
    {
      var xd = Point1.X - Point2.X;
      var yd = Point1.Y - Point2.Y;
      return new Point(xd, yd);
    }


    // Create a static "Ext" class within the source file of the user defined class
    public static XElement ToXElement(this Point Point, XName Name)
    {
      XElement xe = new XElement(Name,
          new XElement("X", Point.X),
          new XElement("Y", Point.Y)
          );
      return xe;
    }

    public static Point ToPoint(
      this XElement Elem, XNamespace Namespace)
    {
      double x = 0;
      double y = 0;
      if (Elem != null)
      {
        x = Elem.Element(Namespace + "X").DoubleOrDefault(0).Value;
        y = Elem.Element(Namespace + "Y").DoubleOrDefault(0).Value;
      }
      var point = new Point(x, y);
      return point;
    }

    public static Point? PointOrDefault(
      this XElement Element, Point? Default = null)
    {
      if (Element == null)
        return Default;
      else
      {
        var x = Element.Element("X").DoubleOrDefault(0).Value;
        var y = Element.Element("Y").DoubleOrDefault(0).Value;
        return new Point(x, y);
      }
    }

  }
}
