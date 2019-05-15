using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AutoCoder.Text;
using AutoCoder.Core.Enums;
using AutoCoder.Text.Enums;

namespace AutoCoder.Scan
{
  public static partial class ScanWord
  {

    // ----------------------- CalcStartBx ---------------------------
    // calc start position from which to start scan to the next word.
    private static int CalcStartBx(
      string Text, WordCursor Word)
    {
      int Bx;
      switch (Word.Position)
      {
        case RelativePosition.Begin:
          Bx = 0;
          break;
        case RelativePosition.Before:
          Bx = Word.ScanBx;
          break;

        case RelativePosition.After:
          if (Word.TextTraits.IsDividerDelim(Word.DelimClass) == true)
            Bx = Word.ScanEx + 1;
          else if ( Word.WordIsDelim == true )
            Bx = Word.ScanEx + 1;
          else
            Bx = Word.DelimBx;
          break;

          // the delim of the current word is itself considered a standalone
          // word. ( it is a symbol, an open or close enclosure, ... )
          // position so the next word is the delim itself. 
        case RelativePosition.At:
          if (Word.TextTraits.IsDividerDelim(Word.DelimClass) == true)
            Bx = Word.ScanEx + 1;
          else if ( Word.WordIsDelim == true )
            Bx = Word.ScanEx + 1;
          else
            Bx = Word.DelimBx;
          break;

        case RelativePosition.End:
          Bx = Text.Length;
          break;

        case RelativePosition.None:
          Bx = -1;
          break;

        default:
          Bx = -1;
          break;
      }

      if (Bx > (Text.Length - 1))
        Bx = -1;

      return Bx;
    }

  }
}
