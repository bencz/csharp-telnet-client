using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Linq;
using AutoCoder.Ehllapi.Common;
using AutoCoder.Ext;

namespace AutoCoder.Ehllapi
{
  public class PresentationSpaceDim
  {
    int mHeight;
    int mWidth;

    public PresentationSpaceDim(int InHeight, int InWidth)
    {
      mHeight = InHeight;
      mWidth = InWidth;
    }

    public int Width
    {
      get { return mWidth; }
      set { mWidth = value; }
    }

    public int Height
    {
      get { return mHeight; }
      set { mHeight = value; }
    }

    /// <summary>
    /// advance location to column 1 of the next row.
    /// </summary>
    /// <param name="InLoc"></param>
    /// <returns></returns>
    public DisplayLocation CrLf(DisplayLocation InLoc)
    {
      int rx = InLoc.Row + 1;
      int cx = 1;
      return new DisplayLocation(rx, cx);
    }

    public DisplayLocation IncDisplayLocation(DisplayLocation InLoc)
    {
      DisplayLocation loc = new DisplayLocation(InLoc.Row, InLoc.Column);
      loc.Column += 1;
      if (loc.Column > Width)
      {
        loc.Row += 1;
        loc.Column = 1;
      }
      return loc;
    }

    public DisplayLocation LinearToDisplayLocation(int InLinear)
    {
      int colNx = 0;
      int rowNx = Math.DivRem(InLinear, Width, out colNx);
      if (colNx == 0)
      {
        colNx = Width;
      }
      else
      {
        rowNx += 1;
      }
      return new DisplayLocation(rowNx, colNx);
    }

    public DisplayLocation CalcEndLocation(DisplayLocation InStartLoc, int InLgth)
    {
      int bx = InStartLoc.ToLinear(this);
      int ex = bx + InLgth - 1;
      DisplayLocation endLoc = LinearToDisplayLocation(ex);
      return endLoc;
    }
  }

  public static class PresentationSpaceDimExt
  {
    public static XElement ToXElement(this PresentationSpaceDim Dim, XName Name)
    {
      if (Dim == null)
        return new XElement(Name, null);
      else
      {
        XElement xe = new XElement(Name,
            new XElement("Width", Dim.Width),
            new XElement("Height", Dim.Height));
        return xe;
      }
    }

    public static PresentationSpaceDim PresentationSpaceDimOrDefault(
      this XElement Elem, XNamespace ns, PresentationSpaceDim Default = null)
    {
      if (Elem == null)
        return Default;
      else
      {
        var wd = Elem.Element("Width").IntOrDefault(0).Value;
        var ht = Elem.Element("Height").IntOrDefault(0).Value;
        return new PresentationSpaceDim(ht, wd);
      }
    }


  }
}
