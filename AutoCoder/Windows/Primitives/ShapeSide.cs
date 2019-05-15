using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AutoCoder.Core.Enums;
using AutoCoder.Windows.Lines;
using System.Windows.Shapes;
using AutoCoder.Ext.Shapes;
using System.Xml.Linq;
using System.Windows;

namespace AutoCoder.Windows.Primitives
{
  public class ShapeSide
  {
    public ShapeSide( Shape Shape, WhichSide WhichSide)
    {
      this.Shape = Shape;
      this.WhichSide = WhichSide;
    }

    public Shape Shape
    { get; set; }

    public WhichSide WhichSide
    { get; set; }

    public double BottomMost
    {
      get
      {
        return this.LineCoor.BottomMost;
      }
    }

    public double LeftMost
    {
      get
      {
        return this.LineCoor.LeftMost;
      }
    }

    LineCoordinates _LineCoor;
    public LineCoordinates LineCoor
    {
      get
      {
        if (_LineCoor == null)
          _LineCoor = Shape.GetSide(this.WhichSide);
        return _LineCoor;
      }
    }

    public double RightMost
    {
      get
      {
        return this.LineCoor.RightMost;
      }
    }

    public double TopMost
    {
      get
      {
        return this.LineCoor.TopMost;
      }
    }

    public XElement ToXElement(XName Name)
    {
      XElement elem = new XElement(Name,
        new XElement("WhichSide", this.WhichSide.ToString( )),
        this.Shape.ToXElement("Shape")) ;

      return elem;
    }
  }
}
