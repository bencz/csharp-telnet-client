using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AutoCoder.Text;
using AutoCoder.Text.Location;
using AutoCoder.Text.Enums;

namespace AutoCoder.Scan
{
  public static partial class ScanAtom
  {

    /// <summary>
    /// sub method of ScanAtom.ScanNextAtom.
    /// A start of a comment token has been found. This method classifies what type of 
    /// comment it is and locates the end of the comment.
    /// </summary>
    /// <param name="ScanStream"></param>
    /// <param name="Traits"></param>
    /// <param name="Bx"></param>
    /// <returns></returns>
    private static Tuple<TextLocation, AtomText, ScanPattern, PatternScanResults> 
      ClassifyAsComment(ScanStream ScanStream, TextTraits Traits, int Bx)
    {
      TextLocation wordBx = null;
      AtomText atomText = null;
      ScanPattern nonWordPat = null;
      PatternScanResults nonWord = null;

      // look prior to see if this comment to the end of the line is the first non 
      // blank on the line.
      bool isFirstNonBlankOnLine = false;
      if (Bx == 0)
      {
        isFirstNonBlankOnLine = true;
      }
      else
      {
        // go back to the first non blank.
        int ix = Scanner.ScanReverseNotEqual(
          ScanStream.Stream, Bx - 1, Traits.WhitespaceWithoutNewLinePatterns);
        if (ix == -1) // nothing but blanks to start of string.
        {
          isFirstNonBlankOnLine = true;
        }

        else
        {
          var rv = Traits.NewLinePatterns.MatchFirstPatternEndsAtStringLocation(
            ScanStream.Stream, ix);
          var pat = rv.Item1;
          var patBx = rv.Item2;

          // is a new line pattern. there is nothing but spaces between this new line
          // and the start of the comment.
          if (pat != null)
          {
            isFirstNonBlankOnLine = true;
          }
        }
      }

      // set the atomCode of this atom depending on if the comment starts the line.
      ScanAtomCode atomCode = ScanAtomCode.CommentToEnd;
      if (isFirstNonBlankOnLine == true)
        atomCode = ScanAtomCode.EntireLineCommentToEnd;

      // scan for a new line. That is the end of the comment.
      {
        nonWord = Scanner.ScanEqualAny(ScanStream.Stream, Bx, Traits.NewLinePatterns);
//        eolPat = rv.Item1;
//        eolIx = rv.Item2;
//        nonWord = rv.Item3;
      }

      // no newline pattern found. Comment to the end of the text stream.
      if (nonWord.IsEmpty == true)
//      if (eolPat == null)
      {
        int ex = ScanStream.Stream.Length - 1;
        wordBx = new StreamLocation(Bx).ToTextLocation(ScanStream);
        TextLocation wordEx = new StreamLocation(ex).ToTextLocation(ScanStream);
        string commentText = ScanStream.Substring(Bx);

        string userCode = null;
        atomText = new AtomText(
          atomCode, commentText, null, wordBx, wordEx,
          userCode);

//        nonWordPat = eolPat;
      }

      else
      {
        wordBx = new StreamLocation(Bx).ToTextLocation(ScanStream);
        int lx = nonWord.Position - Bx;
//        int lx = eolIx - Bx;
        TextLocation wordEx =
          new StreamLocation(Bx + lx - 1).ToTextLocation(ScanStream);
        string commentText = ScanStream.Substring(Bx, lx);
        string userCode = null;
        atomText = new AtomText(
          atomCode, commentText, null, wordBx, wordEx,
          userCode);
        var sloc = wordBx.ToStreamLocation(ScanStream);

//        nonWordPat = eolPat;
      }
      return new Tuple<TextLocation, AtomText, ScanPattern, PatternScanResults>
        (wordBx, atomText, nonWordPat, nonWord);
    }
  }
}
