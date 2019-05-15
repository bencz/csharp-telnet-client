using AutoCoder.Ext;
using AutoCoder.Telnet.Common.ScreenDm;
using AutoCoder.Telnet.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace AutoCoder.Telnet.Common.ScreenLoc
{
  public interface IScreenLoc
  {
    int RowNum { get; set; }
    int ColNum { get; }

    /// <summary>
    /// rownum and colnum are zero or one based.
    /// </summary>
    LocationFrame LocationFrame
    { get; set; }

    IScreenLoc NewOneBased(int RowNum, int ColNum);
    IScreenLoc NewZeroBased(int RowNum, int ColNum);
    IScreenLoc NewInstance(int RowNum, int ColNum);
  }

  public static class IScreenLocExt
  {

    /// <summary>
    /// calc absolute location. 
    /// </summary>
    /// <param name="Loc"></param>
    /// <param name="Start"></param>
    /// <returns></returns>
    public static IScreenLoc AbsoluteLoc(this IScreenLoc Loc, IScreenLoc Start)
    {
      var rowNum = Start.RowNum + Loc.RowNum;
      var colNum = Start.ColNum + Loc.ColNum;
      if ( Loc.LocationFrame == LocationFrame.OneBased)
      {
        rowNum -= 1;
        colNum -= 1;
      }
      return Loc.NewInstance(rowNum, colNum);
    }
    public static IScreenLoc Advance(this IScreenLoc Loc, int Length, ScreenDim Dim)
    {
      int col = Loc.ColNum + Length;
      int row = Loc.RowNum;

      // adjust to zero based.
      if ( Loc.LocationFrame == LocationFrame.OneBased)
      {
        col -= 1;
        row -= 1;
      }

      // negative advance and column off the charts to the left. 
      while (col < 0)
      {
        col += Dim.Width;
        row -= 1;
        if (row < 0)
          row = Dim.Height - 1;
      }

      // positive advance and column out of bounds to the right.
      while (col >= Dim.Width)
      {
        col -= Dim.Width;
        row += 1;
        if (row >= Dim.Height)
          row = Dim.Height - 1;
      }

      // back to one based.
      if (Loc.LocationFrame == LocationFrame.OneBased)
      {
        col += 1;
        row += 1;
      }

      return Loc.NewInstance(row, col);
    }

    public static bool CompareEqual(this IScreenLoc RowCol1, IScreenLoc RowCol2)
    {
      if ((RowCol1 == null) && (RowCol2 == null))
        return true;
      else if ((RowCol1 == null) || (RowCol2 == null))
        return false;
      else if (RowCol1.RowNum != RowCol2.RowNum)
        return false;
      else if (RowCol1.ColNum != RowCol2.ColNum)
        return false;
      else
        return true;
    }

    /// <summary>
    /// compare rowCol to 2nd RowCol value. Return -1 if 1st RowCol is less than
    /// 2nd RowCol. 0 if equal. 1 if 1st RowCol > 2nd RowCol.
    /// </summary>
    /// <param name="RowCol1"></param>
    /// <param name="RowCol2"></param>
    /// <returns></returns>
    public static int CompareTo(this IScreenLoc RowCol1, IScreenLoc RowCol2)
    {
      if (RowCol1.RowNum < RowCol2.RowNum)
        return -1;
      else if (RowCol1.RowNum > RowCol2.RowNum)
        return 1;
      else
        return RowCol1.ColNum.CompareTo(RowCol2.ColNum);
    }

    public static IScreenLoc ToZeroBased(this IScreenLoc ScreenLoc)
    {
      if ( ScreenLoc.LocationFrame == LocationFrame.OneBased)
      {
        int rowNum = ScreenLoc.RowNum - 1;
        int colNum = ScreenLoc.ColNum - 1;
        return ScreenLoc.NewZeroBased(rowNum, colNum);
      }
      else
      {
        return ScreenLoc;
      }
    }

    public static IScreenLoc ToOneBased(this IScreenLoc ScreenLoc)
    {
      if (ScreenLoc.LocationFrame == LocationFrame.ZeroBased)
      {
        int rowNum = ScreenLoc.RowNum + 1;
        int colNum = ScreenLoc.ColNum + 1;
        return ScreenLoc.NewOneBased(rowNum, colNum);
      }
      else
      {
        return ScreenLoc;
      }
    }

    /// <summary>
    /// return the value as text. Different from ToString in that ToString
    /// includes the class name.
    /// </summary>
    /// <param name="RowCol"></param>
    /// <returns></returns>
    public static string ToText(this IScreenLoc RowCol)
    {
      return RowCol.RowNum + "/" + RowCol.ColNum;
    }


  }
}
