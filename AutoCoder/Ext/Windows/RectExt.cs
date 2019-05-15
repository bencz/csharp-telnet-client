using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Xml.Linq;
using AutoCoder.Ext;

namespace AutoCoder.Ext.Windows
{
  public static class RectExt
  {
    public static XElement ToXElement(this Rect Rect, string ElemName)
    {
      XElement elem = new XElement(ElemName,
        new XElement("Width", Rect.Width),
        new XElement("Height", Rect.Height),
        new XElement("Left", Rect.Left),
        new XElement("Top", Rect.Top)) ;

      return elem;
    }

    public static Rect? RectOrDefault(
      this XElement Element, Rect? Default = null)
    {
      if (Element == null)
        return Default;
      else
      {
        var wd = Element.Element("Width").DoubleOrDefault(0).Value ;
        var ht = Element.Element("Height").DoubleOrDefault(0).Value ;
        var lf = Element.Element("Left").DoubleOrDefault(0).Value;
        var tp = Element.Element("Top").DoubleOrDefault(0).Value;
        return new Rect(lf, tp, wd, ht);
      }
    }
  }
}
