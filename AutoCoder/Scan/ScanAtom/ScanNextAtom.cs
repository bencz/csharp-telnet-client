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
    // delimiter and whitespace characters as spcfd in the TextTraits argument )
    // Return null when end of string.
    public static ScanAtomCursor ScanNextAtom(
      ScanStream ScanStream, 
      TextTraits Traits, ScanAtomCursor CurrentWord )
    {
      PatternScanResults nonWord = null;

      AtomText atomText = null;
      List<MatchScanPattern> atomTextList = null;
      AtomText whitespaceText = null;

      ScanAtomCode? tokenCode = null; // ScanAtomCode of this token.
      int? tokenLx = null;
      ScanAtomCode? priorTokenCode = null;

      bool? priorCodeIsWhitespaceSignificant = null;

      // stay at the current location. return copy of the cursor, but with stayatflag
      // turned off.
      if (CurrentWord.StayAtFlag == true)
      {
        atomText = CurrentWord.AtomText;
        tokenCode = atomText.AtomCode;
        priorTokenCode = null;
        nonWord = new PatternScanResults(
          CurrentWord.AtomPattern,
          CurrentWord.StartLoc.ToStreamLocation(ScanStream).Value,
          CurrentWord.AtomPattern.Length);
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
            priorTokenCode = CurrentWord.AtomCode;
            priorCodeIsWhitespaceSignificant = CurrentWord.WhitespaceIsSignificant;
          }

          // calc scan start position
          bx = ScanAtom.CalcScanNextStart(ScanStream, Traits, CurrentWord);

          // advance past whitespace
          if (bx != -1)
          {
            int saveBx = bx;
            bx = Scanner.ScanNotEqual(ScanStream.Stream, Traits.WhitespacePatterns, bx);

            // there is some whitespace. Isolate it as AtomText.
            // This method will return the whitespace as the token. But need to look at 
            // the token before and after to classify the whitespace as significant or
            // not. ( whitespace between identifiers or keywords is significant. 
            // Whitespace between symbols is not significant. 
            // note: even insignificant whitespace is returned as a token because the
            //       whitespace is needed when redisplaying the statement text.
            if (bx != saveBx)
            {
              int whitespaceEx = -1;
              if (bx == -1)
                whitespaceEx = ScanStream.Stream.Length - 1;
              else
                whitespaceEx = bx - 1;
              int whitespaceLx = whitespaceEx - saveBx + 1;

              // split the whitespace between space/tab and EOL
              {
                int fx1 = ScanStream.Stream.IndexOfAny(new char[] { ' ', '\t' }, saveBx);
                int fx2 = ScanStream.Stream.IndexOfAny(new char[] { '\r', '\n' }, saveBx);
                if (fx1 > whitespaceEx)
                  fx1 = -1;
                if (fx2 > whitespaceEx)
                  fx2 = -1;
                if ((fx1 == saveBx) && (fx2 != -1))
                  whitespaceEx = fx2 - 1;
                if ((fx2 == saveBx) && (fx1 != -1))
                  whitespaceEx = fx1 - 1;
                whitespaceLx = whitespaceEx - saveBx + 1;
              }

              string userCode = null;
              whitespaceText = new AtomText(
                ScanAtomCode.Whitespace,
                ScanStream.Stream.Substring(saveBx, whitespaceLx), " ",
                new StreamLocation(saveBx).ToTextLocation(ScanStream),
                new StreamLocation(whitespaceEx).ToTextLocation(ScanStream),
                userCode);
            }
          }
        }
        // end STEP 1.
        #endregion

        #region STEP 2. Isolate either numeric lit, quoted lit or identifier/keyword.
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
            nonWord = rv.Item3;  // the non word pattern immed after numeric literal
            tokenCode = ScanAtomCode.Numeric;
          }

          // got something.  now scan forward for the pattern that delimits the word.
          else if (bx != -1)
          {
            {
              nonWord = Scanner.ScanEqualAny(ScanStream.Stream, bx, Traits.DelimPatterns);
            }

            // a special value starter. scan further for the spcval word.
            // If an identifier follows 
            var startPat = nonWord.FindPattern(DelimClassification.SpecialValueStarter);
            if (startPat != null)
            {
              var csr = new ScanAtomCursor(startPat, ScanStream);
              var nx = ScanAtom.ScanNextAtom(ScanStream, Traits, csr);
              if ((nx.Position == RelativePosition.At)
                && (nx.AtomCode.IsIdentifier() == true))
              {
                atomText = AtomText.Combine(
                  startPat.AtomText, nx.AtomText, ScanAtomCode.SpecialValue);
              }
            }

            // got the AtomText of the token.
            if (atomText != null)
            {
            }

            // word chars all the way to the end.
            else if (nonWord == null)
            {
              tokenCode = ScanAtomCode.Identifier;
              tokenLx = ScanStream.Stream.Length - bx;
            }

            else if (nonWord.FoundAtPosition(DelimClassification.Quote, bx))
            {
              var rv = Scanner.IsolateQuotedWord(ScanStream, Traits, bx);
              litType = rv.Item1;
              litText = rv.Item2;
              nonWord = rv.Item3; // the non word pattern immed after quoted literal
              tokenCode = ScanAtomCode.Quoted;
            }

              // delim pattern found past the start of the scan. That means there are
            // identifier chars from the start of the scan to the found delim.
            else if (bx != nonWord.Position)
            {
              tokenCode = ScanAtomCode.Identifier;
              tokenLx = nonWord.Position - bx;
            }

            else if (nonWord.IsEmpty == false)
              tokenCode =
                nonWord.FirstFoundPattern.MatchPattern.DelimClassification.ToScanAtomCode();

              // should never get here.
            else
              tokenCode = null;
          }

          // attempt to classify the identifier token as a keyword.
          if (atomText == null)
          {
            if ((tokenCode != null) && (tokenCode.Value == ScanAtomCode.Identifier))
            {
              var rv = Traits.KeywordPatterns.MatchPatternToSubstring(
                ScanStream.Stream, bx, tokenLx.Value);
              var kwdResults = rv.Item3;
              var kwdPat = kwdResults.FirstFoundPattern;
              if (kwdPat != null)
              {
                tokenCode = kwdPat.MatchPattern.DelimClassification.ToScanAtomCode();
                nonWord = kwdResults;
              }
            }
          }
        }
        // end STEP 2.
        #endregion

        #region STEP 3 - setup atomText of the found token.
        {

          // got the atomText of the token.
          if (atomText != null)
          {
            nonWord = null;
          }

          // got whitespace. 
          else if (whitespaceText != null)
          {
            ScanAtomCode wstc = ScanAtomCode.Whitespace;

            if (priorTokenCode == null)
              wstc = ScanAtomCode.InsignificantWhitespace;
            else if (tokenCode == null)
              wstc = ScanAtomCode.InsignificantWhitespace;
            else if ((priorTokenCode.Value.WhitespaceIsSignificant() == true)
              && (tokenCode.Value.WhitespaceIsSignificant() == true))
              wstc = ScanAtomCode.Whitespace;
            else
              wstc = ScanAtomCode.InsignificantWhitespace;

            atomText = whitespaceText;
            atomText.AtomCode = wstc;
          }

          // got nothing.
          else if (bx == -1)
          {
          }

          // no delim found. word text all the way to the end.
          else if (nonWord.IsEmpty == true)
          {
            if (whitespaceText != null)
            {
              atomText = whitespaceText;
            }

            else
            {
              // get the text from start of scan to end of string.
              var rv = Scanner.IsolateWordText(
                ScanStream, Traits, litType, litText, bx, null);
              atomText = rv.Item3;
            }
          }

          // got a word followed by non word pattern. return the word.
          else if (nonWord.Position > bx)
          {
            if (whitespaceText != null)
            {
              atomText = whitespaceText;
              nonWord = new PatternScanResults();
            }

            else
            {
              var rv = Scanner.IsolateWordText(
                ScanStream, Traits, litType, litText, bx, nonWord.Position);
              atomText = rv.Item3;
            }
          }

          // no word. just delim. 
          else
          {
            // the delim is comment to end. store as a word.
            if (nonWord.FirstFoundPattern.MatchPattern.DelimClassification ==
              DelimClassification.CommentToEnd)
            {
              var rv = ScanAtom.ClassifyAsComment(ScanStream, Traits, bx);
              atomText = rv.Item2;
              nonWord = rv.Item4;
            }

              // the word found is a non word or keyword pattern.
            else
            {
              // got whitespace followed by keyword. Return the whitespace.
              if ((nonWord.FirstFoundPattern.MatchPattern.DelimClassification
                == DelimClassification.Keyword)
                && (whitespaceText != null))
              {
                atomText = whitespaceText;
                nonWord = new PatternScanResults();
              }

                // there are more than one scan patterns that match.
              else if (nonWord.FoundCount > 1)
              {
                atomTextList = new List<MatchScanPattern>();

                foreach (var pat in nonWord)
                {
                  pat.AssignAtomText(ScanStream);
                  atomTextList.Add(pat);
                }
              }

              else
              {
                var foundPat = nonWord.FirstFoundPattern;
                foundPat.AssignAtomText(ScanStream);
                atomText = foundPat.AtomText;
              }
            }
          }
        }
        #endregion
      }

      // store the results in the return cursor.
      {
        ScanAtomCursor nx = null;
        if (atomText == null)
        {
          nx = new ScanAtomCursor();
          nx.Position = RelativePosition.End;
        }
        else if (atomTextList != null)
        {
          nx = new ScanAtomCursor(atomTextList);
        }
        else
        {
          if ((nonWord == null) || (nonWord.IsEmpty == true))
            nx = new ScanAtomCursor(atomText, null);
          else
            nx = new ScanAtomCursor(atomText, nonWord.FirstFoundPattern.MatchPattern);
          nx.Position = RelativePosition.At;
        }

        return nx;
      }
    }
  }
}
