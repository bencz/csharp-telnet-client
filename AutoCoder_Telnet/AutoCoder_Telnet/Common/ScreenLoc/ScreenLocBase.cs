using AutoCoder.Ext;
using AutoCoder.Telnet.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace AutoCoder.Telnet.Common.ScreenLoc
{
  abstract public class ScreenLocBase
  {
    public int RowNum
    { get; set; }
    public int ColNum
    { get; set; }
    public LocationFrame LocationFrame
    { get; set; }

    public ScreenLocBase(LocationFrame LocationFrame, int RowNum, int ColNum)
    {
      this.LocationFrame = LocationFrame;
      this.ColNum = ColNum;
      this.RowNum = RowNum;
    }

    /// <summary>
    /// create a new RowCol from component parts. Either a ZeroRowCol or OneRowCol, 
    /// depending on LocationFrame.
    /// </summary>
    /// <param name="frame"></param>
    /// <param name="RowNum"></param>
    /// <param name="ColNum"></param>
    /// <param name="Height"></param>
    /// <param name="Width"></param>
    /// <returns></returns>
    public static IScreenLoc Factory(LocationFrame frame, int RowNum, int ColNum)
    {
      IScreenLoc rc = null;
      if (frame == LocationFrame.OneBased)
        rc = new OneScreenLoc(RowNum, ColNum);
      else
        rc = new ZeroScreenLoc(RowNum, ColNum);
      return rc;
    }

    public IScreenLoc NewZeroBased(int RowNum, int ColNum )
    {
      return new ZeroScreenLoc(RowNum, ColNum);
    }
    public IScreenLoc NewOneBased(int RowNum, int ColNum)
    {
      return new OneScreenLoc(RowNum, ColNum);
    }
  }

  public static class ScreenLocBaseExt
  {
    public static XElement ToXElement(this ScreenLocBase RowCol)
    {
      var xe = RowCol.ToXElement("RowCol");
      return xe;
    }
    public static XElement ToXElement(this ScreenLocBase RowCol, XName Name)
    {
      if (RowCol == null)
        return new XElement(Name, null);
      else
      {
        XElement xe = new XElement(Name,
            new XElement("RowNum", RowCol.RowNum),
            new XElement("ColNum", RowCol.ColNum),
            new XElement("Frame", RowCol.LocationFrame)
            );
        return xe;
      }
    }
    public static IScreenLoc ToScreenLoc(this XElement Elem, XNamespace Namespace)
    {
      IScreenLoc item = null;
      if (Elem != null)
      {
        int rowNum = Elem.Element(Namespace + "RowNum").IntOrDefault(0).Value;
        int colNum = Elem.Element(Namespace + "ColNum").IntOrDefault(0).Value;
        var frame =
          Elem.Element(Namespace + "Frame").StringOrDefault("").TryParseLocationFrame(LocationFrame.ZeroBased).Value;

        item = ScreenLocBase.Factory(frame, rowNum, colNum);
      }
      return item;
    }
  }
}
