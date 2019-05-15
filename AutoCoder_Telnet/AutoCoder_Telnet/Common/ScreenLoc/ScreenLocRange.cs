using AutoCoder.Enums;
using AutoCoder.Telnet.Common.ScreenDm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoCoder.Telnet.Common.ScreenLoc
{
  /// <summary>
  /// from and to range of space on the canvas.
  /// intentded as lightweight RowCol and RowColRange. ScreenLoc only contains
  /// </summary>
  public class ScreenLocRange
  {
    public IScreenLoc From
    { get; private set; }

    public IScreenLoc To
    { get; private set; }

    /// <summary>
    /// does the range grow as a rectangle. Or does it wrap from line to line.
    /// </summary>
    public RangeForm RangeForm
    { get; set; }

    public ScreenLocRange(IScreenLoc RowCol, RangeForm RangeForm = RangeForm.Rectangular)
    {
      this.From = RowCol;
      this.To = RowCol;
      this.RangeForm = RangeForm;
    }

    public ScreenLocRange(IScreenLoc From, IScreenLoc To, RangeForm rangeForm = RangeForm.Rectangular)
    {

      // make sure from preceeds to.
      if (To.CompareTo(From) < 0)
      {
        this.From = To;
        this.To = From;
      }
      else
      {
        this.From = From;
        this.To = To;
      }

      this.RangeForm = rangeForm;
    }

    public ScreenLocRange(IScreenLoc From, int Length, ScreenDim Dim)
    {
      this.From = From;
      if (Length == 0)
        this.To = From;
      else
        this.To = From.Advance(Length - 1, Dim);
    }

    public bool Contains(IScreenLoc RowCol)
    {
      bool contains = false;
      if ((RowCol.CompareTo(this.From) >= 0)
        && (RowCol.CompareTo(this.To) <= 0))
        contains = true;
      return contains;
    }
    public bool CompletelyContains(ScreenLocRange Range)
    {
      bool contains = false;
      if (this.From.CompareTo(Range.From) <= 0)
      {
        if (this.To.CompareTo(Range.To) >= 0)
        {
          contains = true;
        }
      }
      return contains;
    }

    /// <summary>
    /// return true if the input range fully or partially overlaps this range.
    /// </summary>
    /// <param name="Range"></param>
    /// <returns></returns>
    public bool ContainsAny(ScreenLocRange Range)
    {
      bool contains = false;

      // the input range starts somewhere within this range.
      if (this.Contains(Range.From))
        contains = true;

      // the input range ends somewhere within this range.
      else if (this.Contains(Range.To))
        contains = true;

      // the input range starts before and ends after this range. That is, the 
      // input range completely overlaps this range.
      else if ((Range.From.CompareTo(this.From) < 0)
        && (Range.To.CompareTo(this.To) > 0))
        contains = true;

      return contains;
    }

    /// <summary>
    /// the inclusive length of the range.
    /// </summary>
    /// <returns></returns>
    public int GetLength(ScreenDim Dim)
    {
      int lx = 0;
      if (To.RowNum == From.RowNum)
        lx = To.ColNum - From.ColNum + 1;
      else
      {
        var from = From.ToZeroBased();
        var to = To.ToZeroBased();

        int row = From.RowNum;
        int col = From.ColNum;
        while (row < to.RowNum)
        {
          lx = lx + (Dim.Width - col);
          col = 0;
          row += 1;
        }

        lx += (to.ColNum - col + 1);
      }
      return lx;
    }

    /// <summary>
    /// recalc range as an absolute range. 
    /// </summary>
    /// <param name="Start"></param>
    /// <returns></returns>
    public ScreenLocRange ToAbsolute(IScreenLoc Start)
    {
      var from = this.From.AbsoluteLoc(Start);
      var to = this.To.AbsoluteLoc(Start);
      return new ScreenLocRange(from, to, this.RangeForm);
    }

    public IntPair ToDim( )
    {
      // width
      var wx = this.To.ColNum - this.From.ColNum + 1;

      // height
      var ht = this.To.RowNum - this.From.RowNum + 1;

      return new IntPair(ht, wx);
    }

    public ScreenLocRange Union(ScreenLocRange range2)
    {
      var r1 = this.Extend(range2.From);
      var r2 = r1.Extend(range2.To);
      return r2;
    }

    public ScreenLocRange Extend(IScreenLoc loc)
    {
      var fromRow = this.From.RowNum;
      var from = this.From;
      var to = this.To;

      if ( this.RangeForm == RangeForm.Linear)
      {
        if (loc.RowNum < from.RowNum)
          from = loc;
        else if ((loc.RowNum == from.RowNum) && (loc.ColNum < from.ColNum))
          from = loc;

        if (loc.RowNum > to.RowNum)
          to = loc;
        else if ((loc.RowNum == to.RowNum) && (loc.ColNum > to.ColNum))
          to = loc;
      }

      else if (this.RangeForm == RangeForm.Rectangular)
      {
        if (loc.RowNum < from.RowNum)
          from = from.NewInstance(loc.RowNum, from.ColNum);
        if (loc.ColNum < from.ColNum)
          from = from.NewInstance(from.RowNum, loc.ColNum);

        if (loc.RowNum > to.RowNum)
          to = to.NewInstance(loc.RowNum, to.ColNum);
        if (loc.ColNum > to.ColNum)
          to = to.NewInstance(to.RowNum, loc.ColNum);
      }

      return new ScreenLocRange(from, to);
    }

    /// <summary>
    /// extend the range by the number of repeats.
    /// </summary>
    /// <param name="RepeatCount"></param>
    /// <returns></returns>
    public ScreenLocRange ApplyRepeat( int RepeatCount)
    {
      var from = this.From;
      var to = this.To;
      var dim = this.ToDim();
      for(int ix = 1; ix < RepeatCount; ++ix)
      {
        to = to.NewInstance(to.RowNum + dim.a, to.ColNum);
      }
      return new ScreenLocRange(from, to);
    }

    public override string ToString()
    {
      return "from:" + this.From.ToString() + " to:" + this.To.ToString();
    }
  }
}
