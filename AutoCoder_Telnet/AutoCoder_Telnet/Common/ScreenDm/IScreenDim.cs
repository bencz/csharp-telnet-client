using AutoCoder.Ext;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Xml.Linq;

namespace AutoCoder.Telnet.Common.ScreenDm
{
  public interface IScreenDim
  {
    int Height { get; set; }
    int Width { get; set; }
  }

  public static class IScreenDimExt
  {
    public static bool CompareEquals(this IScreenDim Value1, IScreenDim Value2)
    {
      if ((Value1 == null) && (Value2 == null))
        return true;
      else if ((Value1 == null) || (Value2 == null))
        return false;
      else if (Value1.Height != Value2.Height)
        return false;
      else if (Value1.Width != Value2.Width)
        return false;
      else
        return true;
    }
    public static bool GetIsNormalScreen(this IScreenDim Dim)
    {
      if (Dim == null)
        return false;
      else
        return ((Dim.Height == 24) && (Dim.Width == 80));
    }

    /// <summary>
    /// screen dimensions are either wide screen, 27x132 or not 24x80.
    /// </summary>
    public static bool GetIsWideScreen( this IScreenDim Dim)
    {
      if (Dim == null)
        return false;
      else
        return ((Dim.Height == 27) && (Dim.Width == 132));
    }

    public static void SetIsWideScreen(this IScreenDim Dim, bool IsWide)
    {
        if (IsWide == true)
        {
          Dim.Height = 27;
          Dim.Width = 132;
        }
        else
        {
          Dim.Height = 24;
          Dim.Width = 80;
        }
      }

    public static Size ToCanvasDim(this IScreenDim Dim, Size charBoxDim)
    {
      var x = charBoxDim.Width * Dim.Width;
      var y = charBoxDim.Height * Dim.Height;
      return new Size(x, y);
    }

    public static IScreenDim ToScreenDim(
      this XElement Elem, XNamespace Namespace)
    {
      IScreenDim dim = null;
      if (Elem != null)
      {
        dim = new ScreenDim();
        dim.Height = Elem.Element(Namespace + "Height").IntOrDefault(0).Value;
        dim.Width = Elem.Element(Namespace + "Width").IntOrDefault(0).Value;
      }
      else
        dim = new ScreenDim(24, 80);
      return dim;
    }

    public static string ToText(this IScreenDim Dim)
    {
      return "Height:" + Dim.Height + " Width:" + Dim.Width;
    }

    public static XElement ToXElement(this IScreenDim Dim, XName Name)
    {
      if (Dim == null)
        return new XElement(Name, null);
      else
      {
        XElement xe = new XElement(Name,
            new XElement("Height", Dim.Height),
            new XElement("Width", Dim.Width)
            );
        return xe;
      }
    }
  }
}
