using AutoCoder.Systm;
using AutoCoder.Telnet.Common.ScreenDm;
using AutoCoder.Telnet.Common.ScreenLoc;
using AutoCoder.Telnet.Enums;
using AutoCoder.Telnet.IBM5250.Content;
using System.Windows;

namespace AutoCoder.Telnet.Common.RowCol
{
  /// <summary>
  /// zero base row/column address.
  /// </summary>
  public class ZeroRowCol : RowColBase, IRowCol, IScreenLoc
  {

    public ZeroRowCol(
      int RowNum, int ColNum, 
      RowColRelative RowColRelative = RowColRelative.Parent,
      CharPoint ContentStart = null)
      : base(RowNum, ColNum, LocationFrame.ZeroBased, new ScreenDim(24, 80), 
          RowColRelative, ContentStart)
    {
    }
    public ZeroRowCol(
      int RowNum, int ColNum, ScreenDim Dim, 
      RowColRelative RowColRelative = RowColRelative.Parent,
      CharPoint ContentStart = null)
      : base(RowNum, ColNum, LocationFrame.ZeroBased, Dim, RowColRelative,
          ContentStart)
    {
    }

    public ZeroRowCol(int RowNum, int ColNum, ScreenContent ScreenContent)
      : base(RowNum, ColNum, LocationFrame.ZeroBased,
          ScreenContent.ScreenDim, ScreenContent.ContentRelative,
          ScreenContent.StartCharPoint)
    {
    }

    public IRowCol Advance(int Length)
    {
      int col = this.ColNum + Length;
      int row = this.RowNum;

      // negative advance and column off the charts to the left. 
      while (col < this.HorzBounds.a)
      {
        col += this.Width();
        row -= 1;
        if (row < this.VertBounds.a)
          row = this.VertBounds.b;
      }

      // positive advance and column out of bounds to the right.
      while (col > this.HorzBounds.b)
      {
        col -= this.Width();
        row += 1;
        if (row > this.VertBounds.b)
          row = this.VertBounds.a;
      }

      return this.NewRowCol(row, col);
    }

    public IScreenLoc NewInstance(int RowNum, int ColNum)
    {
      return new ZeroRowCol(RowNum, ColNum, Dim, RowColRelative, ContentStart);
    }

    public IRowCol NewRowCol(
      int RowNum = -1, int ColNum = -1, ScreenContent ScreenContent = null,
      bool ForceParentRelative = false)
    {
      IRowCol rowCol = null;

      if (RowNum == -1)
        RowNum = this.RowNum;
      if (ColNum == -1)
        ColNum = this.ColNum;

      if (ScreenContent == null)
      {
        if ( ForceParentRelative == true )
        {
          rowCol = new ZeroRowCol(
            RowNum, ColNum, this.Dim, RowColRelative.Parent, null);
        }
        else
        {
          rowCol = new ZeroRowCol(
            RowNum, ColNum, this.Dim, this.RowColRelative, this.ContentStart);
        }
      }
      else
        rowCol = new ZeroRowCol(RowNum, ColNum, ScreenContent);
      return rowCol;
    }

    public IRowCol ToZeroRowCol()
    {
      return NewRowCol(this.RowNum, this.ColNum);
    }

    /// <summary>
    /// convert from zero based row/col address to one based row/col.
    /// </summary>
    /// <returns></returns>
    public IRowCol ToOneRowCol()
    {
      int rowNum = this.RowNum + 1;
      int colNum = this.ColNum + 1;
      return new OneRowCol(rowNum, colNum, this.Dim, this.RowColRelative, this.ContentStart);
    }

    public override string ToString()
    {
      return "ZeroRowCol. Row:" + this.RowNum + " Col:" + this.ColNum +
        " " + this.RowColRelative.ToString();
    }
    public override bool Equals(object obj)
    {
      if (obj is ZeroRowCol)
      {
        var objKey = obj as ZeroRowCol;
        if ((objKey.RowNum == this.RowNum) && (objKey.ColNum == this.ColNum))
          return true;
      }
      else if (obj is IScreenLoc)
      {
        var key = obj as IScreenLoc;
        if ( key.LocationFrame == LocationFrame.OneBased)
        {
          key = key.ToZeroBased();
        }
        if ((key.RowNum == this.RowNum) && (key.ColNum == this.ColNum))
          return true;
      }
      return false;
    }
    public override int GetHashCode()
    {
      var hashCode = (this.RowNum * 100) + this.ColNum;
      return hashCode;
    }
  }

  public static class ZeroRowColExt
  {

    /// <summary>
    /// convert the dot based position to a character based location.
    /// </summary>
    /// <param name="Pos"></param>
    /// <param name="CharDim"></param>
    /// <returns></returns>
    public static IRowCol CanvasPosToRowCol(
      this Point Pos, Size CharDim, CharPoint ContentStart )
    {
      double x = Pos.X / CharDim.Width;
      double y = Pos.Y / CharDim.Height;
      int colNum = (int)x;
      int rowNum = (int)y;

      if (ContentStart == null)
        return new ZeroRowCol(rowNum, colNum);
      else
        return new ZeroRowCol(
          rowNum, colNum, RowColRelative.Local, ContentStart);
    }
  }
}
