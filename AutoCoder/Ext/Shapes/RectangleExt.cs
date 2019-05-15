using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Shapes;
using AutoCoder.Core.Enums;
using System.Windows.Controls;
using System.Windows;
using AutoCoder.Ext.Shapes.LineClasses;
using AutoCoder.Windows.Lines;
using System.Xml.Linq;
using System.Windows.Media;

namespace AutoCoder.Ext.Shapes
{
  public static class RectangleExt
  {
    public static Rect GetBoundedRect(this Rectangle Rect)
    {
      double x1 = Canvas.GetLeft(Rect);
      double y1 = Canvas.GetTop(Rect);

      var wd = Rect.ActualWidth;
      if (wd == 0)
        wd = Rect.Width;
      var ht = Rect.ActualHeight;
      if (ht == 0)
        ht = Rect.Height;

      return new Rect(x1, y1, wd, ht);
    }

    public static LineCoordinates GetSide(this Rectangle Rect, WhichSide Side)
    {
      var left = Canvas.GetLeft(Rect);
      var right = left + Rect.ActualWidth - 1.00;
      var top = Canvas.GetTop(Rect);
      var bottom = top + Rect.ActualHeight - 1.00;

      switch (Side)
      {
        case WhichSide.Left:
          return new LineCoordinates(new Point(left, top), new Point(left, bottom));
        case WhichSide.Right:
          return new LineCoordinates(new Point(right, top), new Point(right, bottom));
        case WhichSide.Top:
          return new LineCoordinates(new Point(left, top), new Point(right, top));
        case WhichSide.Bottom:
          return new LineCoordinates(new Point(left, bottom), new Point(right, bottom));
        default:
          throw new ApplicationException("unsupported enum value");
      }
    }

    public static XElement ToXElement(this Rectangle Rect, XName Name)
    {
      var strokeBrush = Rect.Stroke;
      var fillBrush = Rect.Fill;
      var br = Rect.GetBoundedRect();

      XElement elem = new XElement(Name,
        new XElement("Width", br.Width),
        new XElement("Height", br.Height),
        new XElement("Left", br.Left),
        new XElement("Top", br.Top),
        new XElement("Stroke", strokeBrush.ToString( )),
        new XElement("Fill", fillBrush.ToString( ))
        ) ;
      
      return elem;
    }

    public static Rectangle RectangleOrDefault(
      this XElement Elem, XNamespace Namespace, Rectangle Default)
    {
      Rectangle model = null;
      if (Elem == null)
        model = Default;
      else
      {
        model = new Rectangle();

        model.Width = Elem.Element(Namespace + "Width").DoubleOrDefault(0.00).Value;
        model.Height = Elem.Element(Namespace + "Height").DoubleOrDefault(0.00).Value;

        var left = Elem.Element(Namespace + "Left").DoubleOrDefault(0.00).Value;
        var top = Elem.Element(Namespace + "Top").DoubleOrDefault(0.00).Value;
        Canvas.SetLeft(model, left);
        Canvas.SetTop(model, top);

        {
          string s1 = Elem.Element(Namespace + "Stroke").StringOrDefault("");
          model.Stroke = new BrushConverter().ConvertFromString(s1) as Brush;
        }

        {
          string s1 = Elem.Element(Namespace + "Fill").StringOrDefault("");
          model.Fill = new BrushConverter().ConvertFromString(s1) as Brush;
        }
      }
      return model;
    }

  }
}
