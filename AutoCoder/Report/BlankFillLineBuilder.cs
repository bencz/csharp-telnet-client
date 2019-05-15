using AutoCoder.Ext.System;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AutoCoder.Report
{
  /// <summary>
  /// class used to build a text line. Where text put onto the line at a specified
  /// position is blank filled to the right or overlays existing text.
  /// </summary>
  public class BlankFillLineBuilder
  {
    StringBuilder sb
    { get; set; }

    /// <summary>
    /// when appending to the line there is a gap, a number of blanks, which are
    /// appended after the appended text. The default value of the Append 
    /// method is -1, meaning use the DefaultGap. Which is initially set to zero.
    /// Meaning, by default, no gap space is appended.
    /// </summary>
    public int DefaultGap
    {
      get; set;
    }

    public BlankFillLineBuilder(int DefaultGap = 0)
    {
      this.sb = new StringBuilder();
      this.DefaultGap = DefaultGap;
    }

    public BlankFillLineBuilder Empty()
    {
      this.sb.Length = 0;
      return this;
    }

    public bool IsEmpty( )
    {
      var lx = sb.Length;
      if (lx == 0)
        return true;
      else
        return false;
    }

    public int Length
    {
      get { return sb.Length; }
    }

    public void Append(string Text)
    {
      var text = Text.EmptyIfNull();
      this.sb.Append(text);
    }

    public void Append(string Text, int Width, int Gap = -1)
    {
      var text = Text.EmptyIfNull();
      this.sb.Append(text.PadRight(Width));

      // setup gap size.
      if (Gap == -1)
        Gap = this.DefaultGap;

      // append gap space to the line.
      if ( Gap > 0)
      {
        string gapText = " ";
        this.sb.Append(gapText.PadRight(Gap));
      }
    }

    public void Put(string Text, int Pos, int Width)
    {
      // clip the length of the text.
      var text = Text.EmptyIfNull();
      if ((Width != -1 ) && (text.Length > Width))
        text = Text.Substring(0, Width);
      Put(text, Pos);
    }

    public void Put(string Text, int Pos)
    {
      var text = Text.EmptyIfNull();

      // pad line with blanks.
      if (Pos > this.sb.Length)
      {
        int padLx = Pos - this.sb.Length;
        this.sb.Append(' ', padLx);
      }

      if (Pos == this.sb.Length)
      {
        this.sb.Append(text);
      }
      else
      {
        int resultLx = Pos + text.Length;
        if (resultLx > this.sb.Length)
        {
          int padLx = resultLx - this.sb.Length;
          this.sb.Append(' ', padLx);
        }

        int px = Pos;
        foreach (var ch1 in text)
        {
          this.sb[px] = ch1;
          px += 1;
        }
      }
    }

    public void PutDisplayableText(string Text, int Pos)
    {
      var text = Text.EmptyIfNull();

      var sb = new StringBuilder();
      foreach (var ch1 in text)
      {
        if (ch1.IsPrintable())
          sb.Append(ch1);
        else
        {
          sb.Append("/" + ch1.ToHex() + ' ');
        }
      }
      Put(sb.ToString(), Pos);
    }

    public override string ToString()
    {
      return this.sb.ToString();
    }
  }
}
