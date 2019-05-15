using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Xml.Linq;
using AutoCoder.Ext;

namespace AutoCoder.Windows.Ext
{
  public static class SizeExt
  {
    public static XElement ToXElement(this Size Size, string ElemName)
    {
      XElement elem = new XElement(ElemName,
        new XElement("Width", Size.Width),
        new XElement("Height", Size.Height));
      return elem;
    }

    public static Size? SizeOrDefault(
      this XElement Element, Size? Default = null)
    {
      if (Element == null)
        return Default;
      else
      {
        var wd = Element.Element("Width").DoubleOrDefault(0).Value ;
        var ht = Element.Element("Height").DoubleOrDefault(0).Value ;
        return new Size(wd,ht) ;
      }
    }

  }
}
