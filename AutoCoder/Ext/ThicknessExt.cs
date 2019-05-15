using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Xml.Linq;
using AutoCoder.Xml;

namespace AutoCoder.Ext
{
  public static class ThicknessExt
  {
    public static XElement ToXElement(this Thickness Thick, string ElemName)
    {
      XElement elem = new XElement(ElemName,
        new XElement("Left", Thick.Left),
        new XElement("Right", Thick.Right),
        new XElement("Top", Thick.Top),
        new XElement("Bottom", Thick.Bottom));
      return elem;
    }

    public static Thickness Parse(XElement Elem, XNamespace Namespace)
    {
      Thickness ld = new Thickness();
      ld.Left = Elem.Element(Namespace + "Left").DoubleOrDefault(0).Value;
      ld.Right = Elem.Element(Namespace + "Right").DoubleOrDefault(0).Value;
      ld.Top = Elem.Element(Namespace + "Top").DoubleOrDefault(0).Value;
      ld.Bottom = Elem.Element(Namespace + "Bottom").DoubleOrDefault(0).Value;
      return ld;
    }

    public static Thickness ThicknessOrDefault(
      this XElement Elem, XNamespace Namespace, Thickness Default)
    {
      if (Elem == null)
        return Default;
      else
      {
        var ld = ThicknessExt.Parse(Elem, Namespace);
        return ld;
      }
    }
  }
}

