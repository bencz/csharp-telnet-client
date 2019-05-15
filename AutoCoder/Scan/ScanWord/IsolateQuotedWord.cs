using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AutoCoder.Text;
using AutoCoder.Text.Enums;
using AutoCoder.Text.Location;

namespace AutoCoder.Scan
{
  public static partial class ScanWord
  {

#if skip

    // replace with Scanner.IsolateQuotedWord

    private static Tuple<LiteralType, string, ScanPattern, int>
      IsolateQuotedWord(
      ScanStream ScanStream,
      TextTraits Traits,
      int Bx)
    {
      LiteralType litType = LiteralType.none;
      string litText = null;
      char ch1 = ScanStream.Stream[Bx];

      ScanPattern foundPat = null;
      int foundIx = -1;
      int quoteEx = -1;

      // is start of a verbatim string literal
      if ((Traits.VerbatimLiteralPattern != null) &&
        (Traits.VerbatimLiteralPattern.Match(ScanStream.Stream, Bx)))
      {
        var rv = VerbatimLiteral.ScanCloseQuote(
          ScanStream.Stream, Traits.VerbatimLiteralPattern, Bx);
        quoteEx = rv.Item1;
        litText = rv.Item2;
        litType = LiteralType.VerbatimString;
      }

        // is a quoted literal
      else if (Traits.IsQuoteChar(ch1) == true)
      {
        quoteEx = Scanner.ScanCloseQuote(ScanStream.Stream, Bx, Traits.QuoteEncapsulation);
        if (quoteEx != -1)
        {
          int lx = quoteEx - Bx + 1;
          litText = ScanStream.Substring(Bx, lx);

          // correct the following at some point. Should be either string or
          // char lit.
          litType = LiteralType.String;
        }
      }

      // isolate the delim that follows that quoted word.
      {
        int bx = quoteEx + 1;
        var rv = Scanner.ScanEqualAny(ScanStream.Stream, bx, Traits.DelimPatterns);
        foundPat = rv.Item1;
        foundIx = rv.Item2;
      }

      return new Tuple<LiteralType, string, ScanPattern, int>(
        litType, litText, foundPat, foundIx);
    }

#endif

    // --------------------------------- IsolateQuotedWord ------------------------------------

    private static Tuple<int, int?, string, LiteralType, ScanPatternResults>
      IsolateQuotedWord(
      string Text, int Bx, TextTraits Traits)
    {
      ScanPatternResults spr = null;
      int? ex = null;
      string wordText = null;
      char ch1 = Text[Bx];
      LiteralType litType = LiteralType.none;

      // is start of a verbatim string literal
      if ((Traits.VerbatimLiteralPattern != null) &&
        (Traits.VerbatimLiteralPattern.Match(Text, Bx)))
      {
        var rv = VerbatimLiteral.ScanCloseQuote(
          Text, Traits.VerbatimLiteralPattern, Bx);
        ex = rv.Item1;
        wordText = rv.Item2;
        litType = LiteralType.VerbatimString;
      }

        // is a quoted literal
      else if (Traits.IsQuoteChar(ch1) == true)
      {
        ex = Scanner.ScanCloseQuote(Text, Bx, Traits.QuoteEncapsulation);
        if (ex.Value != -1)
        {
          int lx = ex.Value - Bx + 1;
          wordText = Text.Substring(Bx, lx);

          // correct the following at some point. Should be either string or
          // char lit.
          litType = LiteralType.String; 
        }
      }

      // not a quoted literal
      if ((ex == null) || (ex.Value == -1))
      {
        throw (new ApplicationException(
          "Closing quote not found starting at position " +
          Bx.ToString() + " in " + Text));
      }

      else
      {
        // setup the non word which follows the closing quote.
        int ix = ex.Value + 1;
        if (Text.IsPastEnd(ix))
          spr = new ScanPatternResults(-1);
        else
        {
          // the char that follows the closing quote must be a delim
          int remLx = Text.Length - ix;
          spr = Scanner.ScanEqualAny(Text, ix, remLx, Traits.NonWordPatterns);
          if (spr.FoundPos != ix)
            throw new ApplicationException(
              "invalid char follows close quote at pos " + ix.ToString() +
              " in " + Stringer.Head(Text, 80));
        }
      }

      return new Tuple<int, int?, string, LiteralType, ScanPatternResults>
        (Bx, ex, wordText, litType, spr);
    }


  }
}
