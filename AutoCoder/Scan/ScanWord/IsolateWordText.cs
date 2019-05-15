using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AutoCoder.Text.Enums;
using AutoCoder.Text.Location;
using AutoCoder.Text;

namespace AutoCoder.Scan
{
  public static partial class ScanWord
  {

    private static Tuple<TextLocation,TextWord> IsolateWordText( 
      ScanStream ScanStream,
      TextTraits Traits,
      LiteralType? LiteralType,
      string LitText,
      int Bx, int? NonWordBx )
    {
      TextLocation wordBx = null;
      TextWord wordPart = null;

      // not a literal. A word that runs from Bx to immed before NonWordBx. 
      if (LiteralType == null)
      {
        wordBx = new StreamLocation(Bx).ToTextLocation(ScanStream);
        int lx;
        if (NonWordBx == null)
          lx = ScanStream.Stream.Length - Bx;
        else
          lx = NonWordBx.Value - Bx;
        wordPart = new TextWord(
          ScanStream.Substring(Bx, lx), WordClassification.Identifier, Traits);
      }
      
      // a quoted or numeric literal
      else
      {
        wordBx = new StreamLocation(Bx).ToTextLocation(ScanStream);
        wordPart = new TextWord(LitText, LiteralType.Value, Traits);
      }

      return new Tuple<TextLocation, TextWord>(wordBx, wordPart);
    }
  }
}
