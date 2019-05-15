using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AutoCoder.Text;

namespace AutoCoder.Scan
{
  public static class VerbatimLiteral
  {
    public static Tuple<int,string> ScanCloseQuote(
      string Text, VerbatimLiteralPattern Pattern, int Bx)
    {
      StringBuilder sb = new StringBuilder();
      int ex = -1;

      // get the quote char from the pattern that identifies the start of a
      // verbatim literal string. ( the " in @" )
      char quoteChar = Pattern.QuoteChar;

      // the contents of the verbatim literal starts immed after the verbatim
      // literal start pattern.
      int bx = Bx + Pattern.PatternValue.Length;

      // loop looking for the end quote and store the literal contents.
      while (true)
      {
        // look ahead for a quote or end of string.
        int fx;
        if (bx > (Text.Length - 1))
          fx = -1;
        else
          fx = Text.IndexOf(quoteChar, bx);

        // store what was passed over as the literal content.
        {
          int lx;
          if (fx == -1)
            lx = Text.Length - bx;
          else
            lx = fx - bx;
          if (lx > 0)
            sb.Append(Text.Substring(bx, lx));
        }

        // end of string before the closing quote. that is a problem.
        // This method returns an end pos of -1.
        if (fx == -1)
        {
          ex = -1;
          break;
        }

        // next char is not a quote char. This is the end of the quoted string.
        char? nxChar = Text.NextChar(fx);
        if ((nxChar == null) || (nxChar.Value != quoteChar))
        {
          ex = fx;
          break;
        }

        // next char is a quote char. Add as single quote char to the
        // literal content and continue the processing of quoted contents.
        sb.Append(quoteChar);
        bx = fx + 2;
      }

      // return to caller with the position of the closing quote and the contents of
      // the literal.
      if (ex == -1)
        return new Tuple<int, string>(ex, null);
      else
        return new Tuple<int, string>(ex, sb.ToString());
    }
  }
}
