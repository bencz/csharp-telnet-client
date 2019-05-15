using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AutoCoder.Text.Enums;
using AutoCoder.Text.Location;
using AutoCoder.Text;
using AutoCoder.Scan;

namespace AutoCoder.Text
{
	public static partial class Scanner
	{
    // --------------------------------- IsolateNumericLiteral ------------------------
    public static Tuple<LiteralType, string, PatternScanResults>
      IsolateNumericLiteral(
      ScanStream ScanStream,
      TextTraits Traits,
      int Bx)
    {

      // for now, all numeric literals are simple integers. 
      // have to expand to determine if a float, decimal, what the sign is,
      // what the precision is.
      LiteralType litType = LiteralType.Integer;

      string litText = null;
      PatternScanResults nonWord = null;

      // step from char to char. Look for a char that is not part of the
      // numeric literal.
      int ix = Bx;
      int litEx = Bx;
      while (true)
      {
        if (ix >= ScanStream.Stream.Length)
          break;
        char ch1 = ScanStream.Stream[ix];
        if (Char.IsDigit(ch1) == false)
          break;

        litEx = ix;
        ix += 1;
      }

      // isolate the numeric literal.
      int lx = litEx - Bx + 1;
      litText = ScanStream.Substring(Bx, lx);

      // isolate the delim that follows that numeric literal.
      int bx = litEx + 1;
      if (bx < ScanStream.Stream.Length)
      {
        nonWord = Scanner.ScanEqualAny(ScanStream.Stream, bx, Traits.DelimPatterns);
//        foundPat = rv.Item1;
//        foundIx = rv.Item2;
//        foundLx = rv.Item3;
//        nonWord = rv.Item3;
      }

      return new Tuple<LiteralType, string, PatternScanResults>(
        litType, litText, nonWord);
    }
  }
}
