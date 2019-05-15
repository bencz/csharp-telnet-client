using AutoCoder.Ext;
using AutoCoder.Systm;
using AutoCoder.Telnet.Common.ScreenDm;
using AutoCoder.Telnet.Enums;
using AutoCoder.Telnet.IBM5250.Content;
using System;
using System.Windows;
using System.Xml.Linq;

namespace AutoCoder.Telnet.Common.RowCol
{
  public interface IRowCol
  {
    RowColRelative RowColRelative
    { get; set; }
    int RowNum { get; set; }
    int ColNum { get; set; }
    CharPoint ContentStart { get; set; }
    ScreenDim Dim { get; }
    IntPair HorzBounds { get; }
    IntPair VertBounds { get; }
    LocationFrame LocationFrame { get; }
    IRowCol Advance(int Length);
    IRowCol ToOneRowCol();
    IRowCol ToZeroRowCol();
    IRowCol NewRowCol(
      int RowNum = -1, int ColNum = -1, ScreenContent ScreenContent = null,
      bool ForceParentRelative = false);
  }

  public static class IRowColExt
  {

    /// <summary>
    /// compare rowCol to 2nd RowCol value. Return -1 if 1st RowCol is less than
    /// 2nd RowCol. 0 if equal. 1 if 1st RowCol > 2nd RowCol.
    /// </summary>
    /// <param name="RowCol1"></param>
    /// <param name="RowCol2"></param>
    /// <returns></returns>
    public static int CompareTo(this IRowCol RowCol1, IRowCol RowCol2)
    {
      if (RowCol1.RowNum < RowCol2.RowNum)
        return -1;
      else if (RowCol1.RowNum > RowCol2.RowNum)
        return 1;
      else
        return RowCol1.ColNum.CompareTo(RowCol2.ColNum);
    }

    public static int Width(this IRowCol RowCol)
    {
      return RowCol.HorzBounds.b - RowCol.HorzBounds.a + 1;
    }
    public static int Height(this IRowCol RowCol)
    {
      return RowCol.VertBounds.b - RowCol.VertBounds.a + 1;
    }

    public static IRowCol Add(this IRowCol Value1, IRowCol Value2)
    {
      int rowNum = Value1.RowNum + Value2.RowNum;
      int colNum = Value1.ColNum + Value2.ColNum;
      return Value1.NewRowCol(rowNum, colNum);
    }

#if skip
    public static IRowCol Advance(this IRowCol RowCol, int Length)
    {
      int col = RowCol.ColNum + Length;
      int row = RowCol.RowNum;

      // negative advance and column off the charts to the left. 
      while (col < RowCol.HorzBounds.a)
      {
        col += RowCol.Width();
        row -= 1;
        if (row < RowCol.VertBounds.a)
          row = RowCol.VertBounds.b;
      }

      // positive advance and column out of bounds to the right.
      while (col > RowCol.HorzBounds.b)
      {
        col -= RowCol.Width();
        row += 1;
        if (row > RowCol.VertBounds.b)
          row = RowCol.VertBounds.a;
      }

      return new ZeroRowCol(row, col);
    }
#endif

    private static int WrapVertical(this IRowCol RowCol, int Y)
    {
      int y = Y;
      if (y < 0)
        y = RowCol.VertBounds.b;
      if (y > RowCol.VertBounds.b)
      {
        var adjVert = (y - RowCol.VertBounds.b) - 1;
        y = RowCol.VertBounds.a + adjVert;
      }
      return y;
    }

    public static IRowCol AdvanceDown(this IRowCol RowCol)
    {
      int colNum = RowCol.ColNum;
      int rowNum = RowCol.RowNum + 1;
      rowNum = RowCol.WrapVertical(rowNum);
      return RowCol.NewRowCol(rowNum, colNum);
    }
    public static IRowCol AdvanceLeft(this IRowCol RowCol)
    {
      var rowCol = RowCol.Advance(-1);
      return rowCol;
    }
    public static IRowCol AdvanceRight(this IRowCol RowCol)
    {
      var rowCol = RowCol.Advance(1);
      return rowCol;
    }

    public static IRowCol AdvanceUp(this IRowCol RowCol)
    {
      int colNum = RowCol.ColNum;
      int rowNum = RowCol.RowNum - 1;
      rowNum = RowCol.WrapVertical(rowNum);
      return RowCol.NewRowCol(rowNum, colNum);
    }

    /// <summary>
    /// calc the column distance from Loc1 to Loc2. Distance includes the location 
    /// itself. 
    /// </summary>
    /// <param name="Loc1"></param>
    /// <param name="Loc2"></param>
    /// <param name="Dim"></param>
    /// <returns></returns>
    public static int DistanceInclusive(this IRowCol Loc1, IRowCol Loc2, ScreenDim Dim)
    {
      // ensure loc1 is before loc2.
      var loc1 = Loc1;
      var loc2 = Loc2;
      if (loc1.IsAfter(loc2))
      {
        loc1 = Loc2;
        loc2 = Loc1;
      }

      // number of rows difference.
      int rowDiff = loc2.RowNum - loc1.RowNum;

      // number of colums.
      if (loc2.ColNum < loc1.ColNum)
      {
        rowDiff -= 1;
        loc2.ColNum += Dim.Width;
      }
      int colDiff = loc2.ColNum - loc1.ColNum;

      int lgth = (rowDiff * Dim.Width) + colDiff + 1;
      return lgth;
    }
    public static int DistanceInclusive(this IRowCol Loc1, IRowCol Loc2)
    {
      var lgth = Loc1.DistanceInclusive(Loc2, Loc1.Dim);
      return lgth;
    }

    /// <summary>
    /// return true if is row/col loc2 is after the input parm loc1.
    /// </summary>
    /// <param name="loc"></param>
    /// <returns></returns>
    public static bool IsAfter(this IRowCol loc1, IRowCol loc2)
    {
      if (loc1.RowNum > loc2.RowNum)
        return true;
      else if (loc1.RowNum < loc2.RowNum)
        return false;
      else if (loc1.ColNum > loc2.ColNum)
        return true;
      else
        return false;
    }

    /// <summary>
    /// get the column space remaining on the row of this location.
    /// </summary>
    /// <param name="RowCol"></param>
    /// <returns></returns>
    public static int GetRowRemaining(this IRowCol RowCol)
    {
      var remLx = RowCol.HorzBounds.b - RowCol.ColNum + 1;
      return remLx;
    }

    /// <summary>
    /// apply the window position adjustment to convert the position relative to
    /// the window to position relative to parent of the window.
    /// </summary>
    /// <param name="AdjustRowCol"></param>
    /// <param name="LocalPos"></param>
    /// <returns></returns>
    public static IRowCol LocalPosToParentPos(
      this IRowCol AdjustRowCol, IRowCol LocalPos)
    {
      IRowCol parentPos = null;
      if (AdjustRowCol == null)
        parentPos = LocalPos;
      else
        parentPos = LocalPos.Add(AdjustRowCol);
      return parentPos;
    }

    public static IRowCol Negate( this IRowCol RowCol )
    {
      var rowNum = RowCol.RowNum * -1;
      var colNum = RowCol.ColNum * -1;
      return RowCol.NewRowCol(rowNum, colNum);
    }

    /// <summary>
    /// apply the window position adjustment to convert the position relative to
    /// the window to position relative to parent of the window.
    /// </summary>
    /// <param name="AdjustRowCol"></param>
    /// <param name="LocalPos"></param>
    /// <returns></returns>
    public static IRowCol ParentPosToLocalPos(
      this IRowCol AdjustRowCol, IRowCol ParentPos)
    {
      IRowCol localPos = null;
      if (AdjustRowCol == null)
        localPos = ParentPos;
      else
        localPos = ParentPos.Sub(AdjustRowCol);
      return localPos;
    }
    public static IRowCol Sub(this IRowCol Value1, IRowCol Value2)
    {
      int rowNum = Value1.RowNum - Value2.RowNum;
      int colNum = Value1.ColNum - Value2.ColNum;
      return Value1.NewRowCol(rowNum, colNum);
    }

    /// <summary>
    /// convert the RowCol to a CharPoint. 
    /// </summary>
    /// <param name="RowCol"></param>
    /// <returns></returns>
    public static CharPoint ToCharPoint( this IRowCol RowCol )
    {
      return new CharPoint(RowCol.ColNum, RowCol.RowNum);
    }

    public static Point ToCanvasPos(this IRowCol RowCol, Size CharBoxDim)
    {
      double x = RowCol.ColNum * CharBoxDim.Width;
      double y = RowCol.RowNum * CharBoxDim.Height;
      return new Point(x, y);
    }

    public static IRowCol ToContentRelative(
      this IRowCol RowCol, ScreenContent ScreenContent )
    {
      if (RowCol.RowColRelative == ScreenContent.ContentRelative)
        return RowCol;

      // convert RowCol to location relative to a window.
      else if (RowCol.RowColRelative == RowColRelative.Parent)
      {
        var rowNum = RowCol.RowNum - ScreenContent.StartRowCol.RowNum;
        var colNum = RowCol.ColNum - ScreenContent.StartRowCol.ColNum;
        return RowCol.NewRowCol(rowNum, colNum, ScreenContent);
      }

      // convert RowCol from local to window to absolute screen loc.
      else
      {
        var rowNum = RowCol.RowNum + RowCol.ContentStart.Y;
        var colNum = RowCol.ColNum + RowCol.ContentStart.X;
        return RowCol.NewRowCol(rowNum, colNum, ScreenContent);
      }
    }

    /// <summary>
    /// convert RowCol from whatever it is relative to to a value that is
    /// relative to the entire telnet screen.
    /// </summary>
    /// <param name="RowCol"></param>
    /// <param name="WindowContent"></param>
    /// <returns></returns>
    public static IRowCol ToParentRelative(
      this IRowCol RowCol, ScreenContent WindowContent)
    {
      if (RowCol.RowColRelative == RowColRelative.Parent)
        return RowCol;

      // convert RowCol from local to window to absolute screen loc.
      else
      {
        var rowNum = RowCol.RowNum + RowCol.ContentStart.Y;
        var colNum = RowCol.ColNum + RowCol.ContentStart.X;
        bool forceParentRelative = true;
        return RowCol.NewRowCol(rowNum, colNum, null, forceParentRelative);
      }
    }
    public static string ToText(this IRowCol RowCol)
    {
      return RowCol.RowNum + "/" + RowCol.ColNum;
    }
  }
}
