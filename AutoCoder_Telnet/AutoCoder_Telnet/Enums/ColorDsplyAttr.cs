using AutoCoder.Ext.Windows.Media;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace AutoCoder.Telnet.Enums
{
  public enum ColorDsplyAttr
  {
    ND = 1,
    UL = 2,
    BL = 4,
    RI = 5,
    White = 6,
    Red = 7,
    Blue = 8,
    Green = 9,
    Yellow = 10,
    Turquoise = 11,
    Pink = 12
  }

  public static class ColorDsplyAttrExt
  {
    public static ColorDsplyAttr[] ToColorDsplyAttr(this byte? AttrByte)
    {
      var da = new List<ColorDsplyAttr>();
      if (AttrByte != null)
      {
        var ab = AttrByte.Value;

        if (ab == 0x20)
          da.Add(ColorDsplyAttr.Green);
        else if (ab == 0x21)
        {
          da.Add(ColorDsplyAttr.Green);
          da.Add(ColorDsplyAttr.RI);
        }
        else if (ab == 0x22)
        {
          da.Add(ColorDsplyAttr.White);
        }
        else if (ab == 0x23)
        {
          da.Add(ColorDsplyAttr.White);
          da.Add(ColorDsplyAttr.RI);
        }
        else if (ab == 0x24)
        {
          da.Add(ColorDsplyAttr.Green);
          da.Add(ColorDsplyAttr.UL);
        }
        else if (ab == 0x25)
        {
          da.Add(ColorDsplyAttr.Green);
          da.Add(ColorDsplyAttr.UL);
          da.Add(ColorDsplyAttr.RI);
        }
        else if (ab == 0x26)
        {
          da.Add(ColorDsplyAttr.White);
          da.Add(ColorDsplyAttr.UL);
        }
        else if (ab == 0x28)
        {
          da.Add(ColorDsplyAttr.Red);
        }
        else if (ab == 0x38)
        {
          da.Add(ColorDsplyAttr.Pink);
        }
        else if (ab == 0x3a)
        {
          da.Add(ColorDsplyAttr.Blue);
        }
      }
      return da.ToArray();
    }

    public static Brush GetForegroundBrush(this byte? AttrByte)
    {
      Brush brush = null;
      var daArray = AttrByte.ToColorDsplyAttr();
      foreach( var da in daArray)
      {
        if (da == ColorDsplyAttr.Green)
        {
          var color = Colors.Green;
          color = color.AdjustColor(-0.2f);
          brush = new SolidColorBrush(color);
        }

        else if (da == ColorDsplyAttr.White)
          brush = Brushes.White;
        else if (da == ColorDsplyAttr.Red)
          brush = Brushes.Red;
        else if (da == ColorDsplyAttr.Pink)
          brush = Brushes.Pink;
        else if (da == ColorDsplyAttr.Blue)
        {
          var color = Colors.LightBlue;
          color = color.AdjustColor(+0.2f);
          brush = new SolidColorBrush(color);
        }
      }
      return brush;
    }

    public static string ToText( this ColorDsplyAttr[] AttrArray)
    {
      var sb = new StringBuilder();

      foreach( var ab in AttrArray)
      {
        if (sb.Length > 0)
          sb.Append(' ');
        if ( ab != ColorDsplyAttr.Green)
          sb.Append(ab.ToString());
      }

      return sb.ToString();
    }
  }
}

