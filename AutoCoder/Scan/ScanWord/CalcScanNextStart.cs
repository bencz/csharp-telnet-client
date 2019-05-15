using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AutoCoder.Text;
using AutoCoder.Core.Enums;
using AutoCoder.Text.Enums;
using AutoCoder.Text.Location;

namespace AutoCoder.Scan
{
  public static partial class ScanWord
  {

    // ----------------------- CalcScanNextStart ---------------------------
    // calc start position from which to start scan to the next word.
    private static int CalcScanNextStart(
      ScanStream ScanStream,
      TextTraits Traits, ScanWordCursor Cursor)
    {
      int bx;
      switch (Cursor.Position)
      {
        case RelativePosition.Begin:
          bx = 0;
          break;

        case RelativePosition.Before:
          bx = Cursor.CursorBx.ToStreamLocation(ScanStream).Value;
          break;

        case RelativePosition.After:
        case RelativePosition.At:
          if (Traits.IsNonWordPattern(Cursor.DelimPattern))
            bx = Cursor.DelimEx.ToStreamLocation(ScanStream).Value + 1;
          else if ( Cursor.WordIsDelim == true )
            bx = Cursor.WordEx.ToStreamLocation(ScanStream).Value + 1;
          else
            bx = Cursor.DelimBx.ToStreamLocation(ScanStream).Value ;
          break;

        case RelativePosition.End:
          bx = ScanStream.Stream.Length;
          break;

        case RelativePosition.None:
          bx = -1;
          break;

        default:
          bx = -1;
          break;
      }

      if (bx > (ScanStream.Stream.Length - 1))
        bx = -1;

      return bx;
    }
  }
}
