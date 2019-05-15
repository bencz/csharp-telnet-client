using AutoCoder.Ext.System;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AutoCoder.Systm
{
  /// <summary>
  /// class used to build a text line. Where text put onto the line at a specified
  /// position is blank filled to the left or overlays existing text.
  /// </summary>
  public class xTextLineBuilder
  {
    StringBuilder sb
    { get; set; }

    public xTextLineBuilder( )
    {
      this.sb = new StringBuilder();
    }

    public xTextLineBuilder Empty( )
    {
      this.sb.Length = 0;
      return this;
    }

    public int Length
    {
      get { return sb.Length; }
    }

    public void Append(string Text)
    {
      this.sb.Append(Text);
    }

    public void Append(string Text, int Width )
    {
      this.sb.Append(Text.PadRight(Width));
    }

    public void Put(string Text, int Pos, int Width )
    {
      // clip the length of the text.
      var text = Text;
      if (text.Length > Width)
        text = Text.Substring(0, Width);
      Put(text, Pos);
    }

    public void Put(string Text, int Pos)
    {
      // pad line with blanks.
      if (Pos > this.sb.Length)
      {
        int padLx = Pos - this.sb.Length;
        this.sb.Append(' ', padLx);
      }

      if ( Pos == this.sb.Length )
      {
        this.sb.Append(Text);
      }
      else
      {
        int resultLx = Pos + Text.Length;
        if (resultLx > this.sb.Length)
        {
          int padLx = resultLx - this.sb.Length;
          this.sb.Append(' ', padLx);
        }

        int px = Pos;
        foreach( var ch1 in Text)
        {
          this.sb[px] = ch1;
          px += 1;
        }
      }
    }

    public void PutDisplayableText(string Text, int Pos )
    {
      var sb = new StringBuilder();
      foreach( var ch1 in Text )
      {
        if (ch1.IsPrintable())
          sb.Append(ch1);
        else
        {
          sb.Append("/" + ch1.ToHex( ) + ' ');
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
