using AutoCoder.Ext;
using AutoCoder.Systm;
using AutoCoder.Telnet.Common.ScreenDm;
using AutoCoder.Telnet.Common.ScreenLoc;
using AutoCoder.Telnet.Enums;
using System.Xml.Linq;

namespace AutoCoder.Telnet.Common.RowCol
{
  public abstract class RowColBase
  {
    public int RowNum
    { get; set; }

    public int ColNum
    { get; set; }

    public IntPair HorzBounds
    { get; set; }
    public IntPair VertBounds
    { get; set; }
    public ScreenDim Dim
    { get; set; }

    /// <summary>
    /// the start x,y pos of the content this RowCol is relative to.
    /// </summary>
    public CharPoint ContentStart
    { get; set; }

    public LocationFrame LocationFrame
    { get; set; }

    public RowColRelative RowColRelative
    { get; set; }

    protected RowColBase( 
      int RowNum, int ColNum, LocationFrame LocationFrame, 
      RowColRelative RowColRelative, CharPoint ContentStart)
      : this(RowNum, ColNum, LocationFrame, new ScreenDim(24,80),
          RowColRelative, ContentStart)
    {
    }
    protected RowColBase(
      int RowNum, int ColNum, LocationFrame LocationFrame, ScreenDim Dim,
      RowColRelative RowColRelative, CharPoint ContentStart)
    {
      this.RowNum = RowNum;
      this.ColNum = ColNum;
      this.Dim = Dim;
      this.LocationFrame = LocationFrame;
      this.RowColRelative = RowColRelative;
      this.ContentStart = ContentStart;

      if (LocationFrame == LocationFrame.OneBased)
      {
        this.HorzBounds = new IntPair(1, this.Dim.Width);
        this.VertBounds = new IntPair(1, this.Dim.Height);
      }
      else
      {
        this.HorzBounds = new IntPair(0, this.Dim.Width - 1);
        this.VertBounds = new IntPair(0, this.Dim.Height - 1);
      }
    }
    protected RowColBase(
      LocationFrame LocationFrame, ScreenDim Dim, RowColRelative RowNumRelative)
    {
      this.RowNum = RowNum;
      this.ColNum = ColNum;
      int width = Dim.Width;
      int height = Dim.Height;
      this.RowColRelative = RowNumRelative;
      if (LocationFrame == LocationFrame.OneBased)
      {
        this.HorzBounds = new IntPair(1, width);
        this.VertBounds = new IntPair(1, height);
      }
      else
      {
        this.HorzBounds = new IntPair(0, width - 1);
        this.VertBounds = new IntPair(0, height - 1);
      }

      // init row and col to values that are out of bounds.
      this.RowNum = this.VertBounds.a - 1;
      this.ColNum = this.HorzBounds.a - 1;
    }

    public void Apply( IScreenLoc Value)
    {
      this.RowNum = Value.RowNum;
      this.ColNum = Value.ColNum;
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
    public static IRowCol Factory(LocationFrame frame, int RowNum, int ColNum, int Height, int Width)
    {
      IRowCol rc = null;
      if (frame == LocationFrame.OneBased)
        rc = new OneRowCol(RowNum, ColNum, new ScreenDim(Height, Width));
      else
        rc = new ZeroRowCol(RowNum, ColNum, new ScreenDim(Height, Width));
      return rc;
    }

    public override int GetHashCode()
    {
      var hashCode = (this.RowNum * 100) + this.ColNum;
      return hashCode;
    }

    public IScreenLoc NewZeroBased(int RowNum, int ColNum)
    {
      return new ZeroRowCol(RowNum, ColNum, Dim, RowColRelative, ContentStart);
    }
    public IScreenLoc NewOneBased(int RowNum, int ColNum)
    {
      return new OneRowCol(RowNum, ColNum, Dim, RowColRelative, ContentStart);
    }
  }

  public static class RowColBaseExt
  {
    public static XElement ToXElement(this RowColBase RowCol)
    {
      var xe = RowCol.ToXElement("RowCol");
      return xe;
    }
    public static XElement ToXElement(this RowColBase RowCol, XName Name)
    {
      if (RowCol == null)
        return new XElement(Name, null);
      else
      {
        XElement xe = new XElement(Name,
            new XElement("RowNum", RowCol.RowNum),
            new XElement("ColNum", RowCol.ColNum),
            new XElement("Height", RowCol.Dim.Height),
            new XElement("Width", RowCol.Dim.Width),
            new XElement("Frame", RowCol.LocationFrame)
            );
        return xe;
      }
    }
    public static IRowCol ToRowCol(this XElement Elem, XNamespace Namespace)
    {
      IRowCol item = null;
      if (Elem != null)
      {
        int rowNum = Elem.Element(Namespace + "RowNum").IntOrDefault(0).Value;
        int colNum = Elem.Element(Namespace + "ColNum").IntOrDefault(0).Value;
        int height = Elem.Element(Namespace + "Height").IntOrDefault(24).Value;
        int width = Elem.Element(Namespace + "Width").IntOrDefault(80).Value;
        var frame =
          Elem.Element(Namespace + "Frame").StringOrDefault("").TryParseLocationFrame(LocationFrame.ZeroBased).Value;

        item = RowColBase.Factory(frame, rowNum, colNum, height, width);
      }
      return item;
    }

  }
}
