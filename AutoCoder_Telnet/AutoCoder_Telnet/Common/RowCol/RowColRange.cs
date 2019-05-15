using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoCoder.Telnet.Common.RowCol
{
  /// <summary>
  /// from and to range of space on the canvas.
  /// </summary>
  public class RowColRange
  {
    public IRowCol From
    { get; private set; }

    public IRowCol To
    { get; private set; }

    public RowColRange( IRowCol RowCol )
    {
      this.From = RowCol;
      this.To = RowCol;
    }

    public RowColRange( IRowCol From, IRowCol To)
    {

      // make sure from preceeds to.
      if ( To.CompareTo(From) < 0)
      {
        this.From = To;
        this.To = From;
      }
      else
      {
        this.From = From;
        this.To = To;
      }
    }

    public RowColRange( IRowCol From, int Length )
    {
      this.From = From;
      if (Length == 0)
        this.To = From;
      else
        this.To = From.Advance(Length - 1);
    }

    public bool Contains( IRowCol RowCol )
    {
      bool contains = false;
      if ((RowCol.CompareTo(this.From) >= 0) 
        && (RowCol.CompareTo(this.To) <= 0))
          contains = true;
      return contains;
    }
    public bool CompletelyContains(RowColRange Range)
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
    public bool ContainsAny(RowColRange Range)
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
    public int Length
    {
      get
      {
        var dim = From.Dim;
        int lx = 0;
        if (To.RowNum == From.RowNum)
          lx = To.ColNum - From.ColNum + 1;
        else
        {
          var from = From.ToZeroRowCol();
          var to = To.ToZeroRowCol();

          int row = From.RowNum;
          int col = From.ColNum;
          while (from.RowNum < to.RowNum)
          {
            lx = lx + (dim.Width - from.ColNum);
            from.ColNum = 0;
            from.RowNum += 1;
          }

          lx += (to.ColNum - from.ColNum + 1);
        }
        return lx;
      }
    }

  }
}
