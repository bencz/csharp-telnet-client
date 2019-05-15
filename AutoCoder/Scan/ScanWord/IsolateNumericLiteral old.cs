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
    private static Tuple<LiteralType, string, ScanPattern, int>
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
      ScanPattern foundPat = null;
      int foundIx = -1;

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
        var rv = Scanner.ScanEqualAny(ScanStream.Stream, bx, Traits.DelimPatterns);
        foundPat = rv.Item1;
        foundIx = rv.Item2;
      }

      return new Tuple<LiteralType, string, ScanPattern, int>(
        litType, litText, foundPat, foundIx);
    }
  }
}
