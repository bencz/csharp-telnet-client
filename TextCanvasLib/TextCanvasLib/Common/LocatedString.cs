using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TextCanvasLib.Common
{
  /// <summary>
  /// a combo of a string and the position at which the string is located.
  /// Using the length of the string, the begin and end position of the string can be
  /// determined. Code can determine if one located string intersects another. And two
  /// located strings can be combined ( unioned ).
  /// </summary>
  public class LocatedString
  {
    public int BeginPos { get; set; }

    public int Length
    {
      get { return this.Text.Length; }
    }

    public string Text { get; set; }

    public int EndPos
    {
      get
      {
        return this.BeginPos + this.Text.Length - 1;
      }
    }

    public LocatedString(int BeginPos, string Text)
    {
      this.BeginPos = BeginPos;
      this.Text = Text;
    }

    public LocatedString GetChar(int Pos)
    {
      LocatedString locStr = null;
      if ((Pos >= this.BeginPos) && (Pos <= this.EndPos))
      {
        int ix = Pos - this.BeginPos;
        var s1 = this.Text.Substring(ix, 1);
        locStr = new LocatedString(Pos, s1);
      }
      return locStr;
    }

    /// <summary>
    /// text of located string is empty.
    /// </summary>
    /// <returns></returns>
    public bool IsEmpty()
    {
      if (this.Text.Length == 0)
        return true;
      else
        return false;
    }

    public bool IsWithin(int Pos)
    {
      if (Pos < this.BeginPos)
        return false;
      else if (Pos > this.EndPos)
        return false;
      else
        return true;
    }

    /// <summary>
    /// remove the located string from this located string.
    /// The string to remove must be an entire subset of the remove from string.
    /// Use the absolute position of each string to calc the actual location of the
    /// string to remove.
    /// </summary>
    /// <param name="RemoveText"></param>
    /// <returns></returns>
    public LocatedString Remove(LocatedString RemoveText)
    {
      LocatedString afterRemoveText;

      // remove text must be entirely within the text of this item.
      if ((this.IsWithin(RemoveText.BeginPos) == true)
        && (this.IsWithin(RemoveText.EndPos) == true))
      {
        var ix = StringPosToTextPos(RemoveText);
        var s1 = this.Text.Remove(ix, RemoveText.Length);
        afterRemoveText = new LocatedString(this.BeginPos, s1);
      }
      else
        throw new ApplicationException("location of text to remove is not within range " +
          "of the text.");

      return afterRemoveText;
    }

    /// <summary>
    /// convert absolute pos of input located string to relative position within the 
    /// Text of this located string.
    /// </summary>
    /// <param name="InString"></param>
    /// <returns></returns>
    private int StringPosToTextPos(LocatedString InString)
    {
      int textPos = InString.BeginPos - this.BeginPos;
      return textPos;
    }

    public override string ToString()
    {
      return "Begin:" + this.BeginPos + " End:" + this.EndPos + " " + this.Text;
    }

    public LocatedString ToUpper( )
    {
      var s1 = this.Text.ToUpper();
      return new LocatedString(this.BeginPos, s1);
    }

    public static LocatedString Union(LocatedString s1, LocatedString s2)
    {
      int unionBx = Math.Min(s1.BeginPos, s2.BeginPos);

      // segment of text in s2 which is before s1.
      string bfrText = "";
      if (s2.BeginPos < s1.BeginPos)
      {
        int lx = s1.BeginPos - s2.BeginPos;
        if (lx > s2.Length)
          lx = s2.Length;
        bfrText = s2.Text.Substring(0, lx);

        // pad before text wih blanks to make sure begin text in s2 reaches to right
        // before begin of s1.
        int padLx = (s1.BeginPos - s2.BeginPos) - bfrText.Length;
        if (padLx > 0)
          bfrText = bfrText.PadRight(bfrText.Length + padLx);
      }

      // segment of text in s2 with is after s1.
      string aftText = "";
      if (s2.EndPos > s1.EndPos)
      {
        int lx = s2.EndPos - s1.EndPos;
        if (lx > s2.Length)
          lx = s2.Length;
        int bx = s2.Length - lx;
        aftText = s2.Text.Substring(bx, lx);

        // pad after text wih before blanks in the case where there is a gap between the
        // end of s1 and the start of s2.
        int padLx = s2.BeginPos - s1.EndPos - 1;
        if (padLx > 0)
          aftText = aftText.PadLeft(aftText.Length + padLx);
      }

      // the union text.
      string unionText = bfrText + s1.Text + aftText;

      return new LocatedString(unionBx, unionText);
    }
  }
}
