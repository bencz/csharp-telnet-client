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
  public static partial class ScanAtom
  {

    // ------------------------ ScanNextAtom -------------------------
    // Scans to the next atom in the string. ( a word being the text bounded by the
    // delimeter and whitespace characters as spcfd in the TextTraits argument )
    // Return null when end of string.
    public static ScanAtomCursor ScanNextAtom(
      ScanStream ScanStream, 
      TextTraits Traits, ScanAtomCursor CurrentWord )
    {

      // components of the next word.
      TextLocation wordBx = null;
      int nonWordIx = -1;
      int nonWordLx = 0;

      ScanPattern nonWordPat = null;
      List<MatchScanPattern> nonWordPatList = null;
      AtomText atomText = null;
      List<MatchScanPattern> atomTextList = null;
      AtomText whitespaceText = null;
//      ScanAtomCode? priorCode = null;
      bool? priorCodeIsWhitespaceSignificant = null;

      // stay at the current location. return copy of the cursor, but with stayatflag
      // turned off.
      if (CurrentWord.StayAtFlag == true)
      {
        atomText = CurrentWord.AtomText;
        nonWordPat = CurrentWord.AtomPattern;
        wordBx = CurrentWord.StartLoc ;
      }

      else
      {

        #region STEP1 setup the begin pos of the next word.
        // ----------------------------- STEP 1 ------------------------------
        // setup the begin pos of the next word.
        int bx;
        {
          // save the ScanAtomCode of the prior word.
          if ((CurrentWord.Position == RelativePosition.At)
            || (CurrentWord.Position == RelativePosition.After))
          {
            priorCodeIsWhitespaceSignificant = CurrentWord.WhitespaceIsSignificant;
//            priorCode = CurrentWord.AtomText.AtomCode;
          }

          // calc scan start position
          bx = ScanAtom.CalcScanNextStart(ScanStream, Traits, CurrentWord);

          // advance past whitespace
          if (bx != -1)
          {
            int saveBx = bx;
            bx = Scanner.ScanNotEqual(ScanStream.Stream, Traits.WhitespacePatterns, bx);
            
            // there is some whitespace. depending on what preceeds and follows, may 
            // return this as the atom.
            if ((priorCodeIsWhitespaceSignificant != null) 
              && (priorCodeIsWhitespaceSignificant.Value == true))
            {
              if (bx != saveBx)
              {
                int whitespaceEx = -1;
                if (bx == -1)
                  whitespaceEx = ScanStream.Stream.Length - 1;
                else
                  whitespaceEx = bx - 1;
                int whitespaceLx = whitespaceEx - saveBx + 1;

                whitespaceText = new AtomText(
                  ScanAtomCode.Whitespace,
                  ScanStream.Stream.Substring(saveBx, whitespaceLx), " ",
                  new StreamLocation(saveBx).ToTextLocation(ScanStream),
                  new StreamLocation(whitespaceEx).ToTextLocation(ScanStream));
              }
            }
          }
        }
        // end STEP 1.
        #endregion

        #region STEP 2. Isolate either numeric lib, quoted lit or scan to non word pattern
        // ------------------------------- STEP 2 ----------------------------------
        // Isolate either numeric literal, quoted literal or scan to the next non word
        // pattern.
        LiteralType? litType = null;
        string litText = null;
        {
          // got a decimal digit. isolate the numeric literal string.
          if ((bx != -1) && (Char.IsDigit(ScanStream.Stream[bx]) == true))
          {
            var rv = Scanner.IsolateNumericLiteral(ScanStream, Traits, bx);
            litType = rv.Item1;
            litText = rv.Item2;
            nonWordPat = rv.Item3;  // the non word pattern immed after numeric literal
            nonWordIx = rv.Item4;   // pos of foundPat
          }

          // got something.  now scan forward for the pattern that delimits the word.
          else if (bx != -1)
          {
            {
              var rv = Scanner.ScanEqualAny(ScanStream.Stream, bx, Traits.DelimPatterns);
              nonWordPat = rv.Item1;
              nonWordIx = rv.Item2;
              nonWordLx = rv.Item3;
              nonWordPatList = rv.Item4;
            }

            // got a quote char. Isolate the quoted string, then find the delim that follows
            // the quoted string.
            if ((nonWordPat != null)
              && (nonWordPat.DelimClassification == DelimClassification.Quote)
              && (nonWordIx == bx))
            {
              var rv = Scanner.IsolateQuotedWord(ScanStream, Traits, nonWordIx);
              litType = rv.Item1;
              litText = rv.Item2;
              nonWordPat = rv.Item3;  // the non word pattern immed after quoted literal
              nonWordIx = rv.Item4;   // pos of foundPat.
            }
          }
        }
        // end STEP 2.
        #endregion

        #region STEP 3 - setup wordBx and wordPart with the found word.
        {
          // got nothing.
          if (bx == -1)
          {
          }

          // no delim found. word text all the way to the end.
          else if (nonWordIx == -1)
          {
            if (whitespaceText != null)
            {
              atomText = whitespaceText;
              nonWordPat = null;
              nonWordPatList = null;
            }

            else
            {
              var rv = Scanner.IsolateWordText(
                ScanStream, Traits, litType, litText, bx, null);
              atomText = rv.Item3;
              wordBx = atomText.StartLoc;
            }
          }

          // got a word and a non word pattern.
          else if (nonWordIx > bx)
          {
            if (whitespaceText != null)
            {
              atomText = whitespaceText;
              nonWordPat = null;
              nonWordPatList = null;
            }

            else
            {
              var rv = Scanner.IsolateWordText(
                ScanStream, Traits, litType, litText, bx, nonWordIx);
              atomText = rv.Item3;
              wordBx = atomText.StartLoc;
            }
          }

          // no word. just delim. 
          else
          {
            // the delim is comment to end. store as a word.
            if (nonWordPat.DelimClassification == DelimClassification.CommentToEnd)
            {
              var rv = Scanner.ScanEqualAny(ScanStream.Stream, bx, Traits.NewLinePatterns);
              var eolPat = rv.Item1;
              var eolIx = rv.Item2;
              if (eolPat == null)
              {
                int ex = ScanStream.Stream.Length - 1;
                wordBx = new StreamLocation(nonWordIx).ToTextLocation(ScanStream);
                TextLocation wordEx = new StreamLocation(ex).ToTextLocation(ScanStream);
                string commentText = ScanStream.Substring(nonWordIx);

                atomText = new AtomText(
                  ScanAtomCode.CommentToEnd, commentText, null, wordBx, wordEx);

                nonWordPat = null;
                nonWordPatList = null;
              }
              else
              {
                wordBx = new StreamLocation(nonWordIx).ToTextLocation(ScanStream);
                int lx = eolIx - nonWordIx;
                TextLocation wordEx =
                  new StreamLocation(nonWordIx + lx - 1).ToTextLocation(ScanStream);
                string commentText = ScanStream.Substring(nonWordIx, lx);
                atomText = new AtomText(
                  ScanAtomCode.CommentToEnd, commentText, null, wordBx, wordEx);
                var sloc = wordBx.ToStreamLocation(ScanStream);

                nonWordPat = eolPat;
                nonWordPatList = null;
              }
            }

              // the word found is a non word or keyword pattern.
            else
            {

              // got whitespace followed by keyword. Return the whitespace.
              if ((nonWordPat.DelimClassification == DelimClassification.Keyword)
                && (whitespaceText != null))
              {
                atomText = whitespaceText;
                nonWordPat = null;
                nonWordPatList = null;
              }

                // there are more than one scan patterns that match.
              else if (nonWordPatList != null)
              {
                atomTextList = new List<MatchScanPattern>();
                foreach (var pat in nonWordPatList)
                {
                  wordBx = new StreamLocation(nonWordIx).ToTextLocation(ScanStream);
                  int lx = pat.MatchLength;
                  TextLocation wordEx =
                    new StreamLocation(nonWordIx + lx - 1).ToTextLocation(ScanStream);
                  string scanText = ScanStream.Stream.Substring(nonWordIx, lx);

                  atomText = new AtomText(
                    pat.MatchPattern.DelimClassification.ToScanAtomCode().Value,
                    scanText, 
                    pat.MatchPattern.ReplacementValue,
                    wordBx, wordEx);

                  pat.AtomText = atomText;
                  atomTextList.Add(pat);
                }
              }

              else
              {
                wordBx = new StreamLocation(nonWordIx).ToTextLocation(ScanStream);
                int lx = nonWordLx;
                TextLocation wordEx =
                  new StreamLocation(nonWordIx + lx - 1).ToTextLocation(ScanStream);
                string scanText = ScanStream.Stream.Substring(nonWordIx, lx);

                atomText = new AtomText(
                  nonWordPat.DelimClassification.ToScanAtomCode().Value,
                  scanText, nonWordPat.ReplacementValue,
                  wordBx, wordEx);
              }
            }
          }
        }
        #endregion
      }

      // store the results in the return cursor.
      ScanAtomCursor nx = null ;
      if ( atomText == null )
      {
        nx = new ScanAtomCursor( ) ;
        nx.Position = RelativePosition.End ;
      }
      else if (atomTextList != null)
      {
        nx = new ScanAtomCursor(atomTextList);
      }
      else
      {
        nx = new ScanAtomCursor(atomText, nonWordPat);
        nx.Position = RelativePosition.At;
      }

      return nx ;
    }
  }
}
