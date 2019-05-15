using AutoCoder.Systm;
using AutoCoder.Telnet.Common.RowCol;
using AutoCoder.Telnet.Common.ScreenDm;
using AutoCoder.Telnet.Common.ScreenLoc;
using AutoCoder.Telnet.Enums;
using AutoCoder.Telnet.IBM5250.Content;

namespace AutoCoder.Telnet.Common.RowCol
{
  /// <summary>
  /// row and column location. Values are 1 based.
  /// </summary>
  public class OneRowCol :  RowColBase, IRowCol, IScreenLoc
  {
    public OneRowCol(int RowNum, int ColNum, 
      RowColRelative RowColRelative = RowColRelative.Parent,
      CharPoint ContentStart = null)
      : base(RowNum, ColNum, LocationFrame.OneBased, new ScreenDim(24, 80), 
          RowColRelative, ContentStart)
    {
    }
    public OneRowCol(
      int RowNum, int ColNum, ScreenDim Dim, 
      RowColRelative RowColRelative = RowColRelative.Parent,
      CharPoint ContentStart = null)
      : base(RowNum, ColNum, LocationFrame.OneBased, Dim, RowColRelative,
          ContentStart)
    {
    }

    public OneRowCol( int RowNum, int ColNum, ScreenContent ScreenContent )
      : base(RowNum, ColNum, LocationFrame.OneBased,
          ScreenContent.ScreenDim, ScreenContent.ContentRelative,
          ScreenContent.StartCharPoint)
    {
//      this.ContentStart = ScreenContent.StartRowCol.ToCharPoint();
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
        if (ForceParentRelative == true)
        {
          rowCol = new OneRowCol(
            RowNum, ColNum, this.Dim, RowColRelative.Parent, null);
        }
        else
        {
          rowCol = new OneRowCol(
            RowNum, ColNum, this.Dim, this.RowColRelative, this.ContentStart);
        }
      }
      else
        rowCol = new OneRowCol(RowNum, ColNum, ScreenContent);
      return rowCol;
    }

    public override string ToString()
    {
      return "OneRowCol. Row:" + this.RowNum + " Col:" + this.ColNum +
        " " + this.RowColRelative.ToString();
    }

    public IRowCol ToOneRowCol()
    {
      return NewRowCol(this.RowNum, this.ColNum);
    }

    /// <summary>
    /// convert from one base row/col address to zero based row/col.
    /// </summary>
    /// <returns></returns>
    public IRowCol ToZeroRowCol()
    {
      var rowNum = this.RowNum - 1;
      var colNum = this.ColNum - 1;
      return new ZeroRowCol(rowNum, colNum, this.Dim, this.RowColRelative, this.ContentStart);
    }

    public override bool Equals(object obj)
    {
      if (obj is OneRowCol)
      {
        var objKey = obj as OneRowCol;
        if ((objKey.RowNum == this.RowNum) && (objKey.ColNum == this.ColNum))
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
}
