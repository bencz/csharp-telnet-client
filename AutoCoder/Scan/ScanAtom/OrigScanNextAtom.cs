using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AutoCoder.Text.Location;
using AutoCoder.Text;
using AutoCoder.Text.Enums;
using AutoCoder.Core.Enums;

namespace AutoCoder.Scan
{
  public static partial class ScanAtom
  {

    // ------------------------ ScanNextAtom -------------------------
    // Scans to the next atom in the string. ( a word being the text bounded by the
    // delimiter and whitespace characters as spcfd in the TextTraits argument )
    // Return null when end of string.
    public static ScanAtomCursor OrigScanNextAtom(
      ScanStream ScanStream,
      TextTraits Traits, ScanAtomCursor CurrentWord)
    {
      // components of the next word.
      TextLocation wordBx = null;
      int nonWordIx = -1;
      int nonWordLx = 0;

      ScanPattern nonWordPat = null;
      PatternScanResults nonWord = null;

      AtomText atomText = null;
      List<MatchScanPattern> atomTextList = null;
      AtomText whitespaceText = null;

      ScanAtomCode? tokenCode = null; // ScanAtomCode of this token.
      int? tokenLx = null;
      ScanAtomCode? priorTokenCode = null;

      //      ScanAtomCode? priorCode = null;
      bool? priorCodeIsWhitespaceSignificant = null;

      // stay at the current location. return copy of the cursor, but with stayatflag
      // turned off.
      if (CurrentWord.StayAtFlag == true)
      {
        atomText = CurrentWord.AtomText;
        tokenCode = atomText.AtomCode;
        priorTokenCode = null;
        nonWordPat = CurrentWord.AtomPattern;
        wordBx = CurrentWord.StartLoc;
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
            nonWord = rv.Item3;
//            nonWordPat = rv.Item4;  // the non word pattern immed after numeric literal
//            nonWordIx = rv.Item5;   // pos of foundPat
            tokenCode = ScanAtomCode.Numeric;
          }

          // got something.  now scan forward for the pattern that delimits the word.
          else if (bx != -1)
          {
            {
              nonWord = Scanner.ScanEqualAny(ScanStream.Stream, bx, Traits.DelimPatterns);
//              nonWordPat = rv.Item1;
//              nonWordIx = rv.Item2;
//              nonWordLx = rv.Item3;
//              nonWord = rv.Item3;
            }

            // a special value starter. scan further for the spcval word.
            var startPat = nonWord.FindPattern(DelimClassification.SpecialValueStarter);
            if (startPat != null)
            {

            }

            // word chars all the way to the end.
            //            if (nonWordPat == null)
            if (nonWord == null)
            {
              tokenCode = ScanAtomCode.Identifier;
              tokenLx = ScanStream.Stream.Length - bx;
            }

            else if (nonWord.FoundAtPosition(DelimClassification.Quote, bx))
            {
              var rv = Scanner.IsolateQuotedWord(ScanStream, Traits, bx);
              litType = rv.Item1;
              litText = rv.Item2;
//              nonWordPat = rv.Item3;  // the non word pattern immed after quoted literal
//              nonWordIx = rv.Item4;   // pos of foundPat.
              nonWord = rv.Item3;
              tokenCode = ScanAtomCode.Quoted;
            }

#if skip
            // got a quote char. Isolate the quoted string, then find the delim that follows
            // the quoted string.
            else if ((nonWordPat.DelimClassification == DelimClassification.Quote)
              && (nonWordIx == bx))
            {
              var rv = Scanner.IsolateQuotedWord(ScanStream, Traits, nonWordIx);
              litType = rv.Item1;
              litText = rv.Item2;
              nonWordPat = rv.Item3;  // the non word pattern immed after quoted literal
              nonWordIx = rv.Item4;   // pos of foundPat.
              nonWord = rv.Item5;
              tokenCode = ScanAtomCode.Quoted;
            }
#endif
            // delim pattern found past the start of the scan. That means there are
            // identifier chars from the start of the scan to the found delim.
            else if (bx != nonWord.Position)
            //            else if (bx != nonWordIx)
            {
              tokenCode = ScanAtomCode.Identifier;
              tokenLx = nonWord.Position - bx;
              //              tokenLx = nonWordIx - bx;
            }

            else if (nonWordPat != null)
              tokenCode = nonWordPat.DelimClassification.ToScanAtomCode();

              // should never get here.
            else
              tokenCode = null;
          }

          // attempt to classify the identifier token as a keyword.
          if ((tokenCode != null) && (tokenCode.Value == ScanAtomCode.Identifier))
          {
            var rv = Traits.KeywordPatterns.MatchPatternToSubstring(
              ScanStream.Stream, bx, tokenLx.Value);
            var kwdResults = rv.Item3;
            var kwdPat = kwdResults.FirstFoundPattern;
            if (kwdPat != null)
            {
              tokenCode = kwdPat.MatchPattern.DelimClassification.ToScanAtomCode();
              nonWordPat = kwdPat.MatchPattern;
              nonWord = kwdResults;
              nonWordIx = bx;
              nonWordLx = kwdPat.MatchLength;
            }

#if skip
            var matchPat = rv.Item1 ;
            var keywordTextLx = rv.Item2;  // the actual lgth of matched text.
            if (matchPat != null)
            {
              tokenCode = matchPat.DelimClassification.ToScanAtomCode();
              nonWordPat = matchPat;
              nonWordPatList = null;
              nonWord = null;
              nonWordIx = bx;
              nonWordLx = keywordTextLx;
            }
#endif
          }
        }
        // end STEP 2.
        #endregion

        #region STEP 3 - setup wordBx and wordPart with the found word.
        {

          // got whitespace. 
          if (whitespaceText != null)
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
          //          else if (nonWordIx == -1)
          {
            if (whitespaceText != null)
            {
              atomText = whitespaceText;
              nonWordPat = null;
            }

            else
            {
              // get the text from start of scan to end of string.
              var rv = Scanner.IsolateWordText(
                ScanStream, Traits, litType, litText, bx, null);
              atomText = rv.Item3;
              wordBx = atomText.StartLoc;
            }
          }

          // got a word and a non word pattern.
          else if (nonWord.Position > bx)
          //          else if (nonWordIx > bx)
          {
            if (whitespaceText != null)
            {
              atomText = whitespaceText;
              nonWord = new PatternScanResults();
              nonWordPat = null;
            }

            else
            {
              var rv = Scanner.IsolateWordText(
                ScanStream, Traits, litType, litText, bx, nonWord.Position);
              //              var rv = Scanner.IsolateWordText(
              //                ScanStream, Traits, litType, litText, bx, nonWordIx);
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
              var rv = ScanAtom.ClassifyAsComment(ScanStream, Traits, bx);
              wordBx = rv.Item1;
              atomText = rv.Item2;
              nonWordPat = rv.Item3;
              nonWord = rv.Item4;
#if skip
              var rv = Scanner.ScanEqualAny(ScanStream.Stream, bx, Traits.NewLinePatterns);
              var eolPat = rv.Item1;
              var eolIx = rv.Item2;

              // no newline pattern found. Comment to the end of the text stream.
              if (eolPat == null)
              {
                int ex = ScanStream.Stream.Length - 1;
                wordBx = new StreamLocation(nonWordIx).ToTextLocation(ScanStream);
                TextLocation wordEx = new StreamLocation(ex).ToTextLocation(ScanStream);
                string commentText = ScanStream.Substring(nonWordIx);

                string userCode = null;
                atomText = new AtomText(
                  ScanAtomCode.CommentToEnd, commentText, null, wordBx, wordEx,
                  userCode);

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
                string userCode = null;
                atomText = new AtomText(
                  ScanAtomCode.CommentToEnd, commentText, null, wordBx, wordEx,
                  userCode);
                var sloc = wordBx.ToStreamLocation(ScanStream);

                nonWordPat = eolPat;
                nonWordPatList = null;
              }
#endif
            }

              // the word found is a non word or keyword pattern.
            else
            {
              // got whitespace followed by keyword. Return the whitespace.
              if ((nonWordPat.DelimClassification == DelimClassification.Keyword)
                && (whitespaceText != null))
              {
                atomText = whitespaceText;
                nonWord = new PatternScanResults();
                nonWordPat = null;
              }

                // there are more than one scan patterns that match.
              else if (nonWord.FoundCount > 1)
              //              else if (nonWordPatList != null)
              {
                atomTextList = new List<MatchScanPattern>();

                foreach (var pat in nonWord)
                {
                  wordBx = new StreamLocation(nonWord.Position).ToTextLocation(ScanStream);
                  int lx = pat.MatchLength;
                  TextLocation wordEx =
                    new StreamLocation(nonWord.Position + lx - 1).ToTextLocation(ScanStream);
                  string scanText = ScanStream.Stream.Substring(nonWord.Position, lx);

                  atomText = new AtomText(
                    pat.MatchPattern.DelimClassification.ToScanAtomCode().Value,
                    scanText,
                    pat.MatchPattern.ReplacementValue,
                    wordBx, wordEx,
                    pat.MatchPattern.UserCode);

                  pat.AtomText = atomText;
                  atomTextList.Add(pat);
                }
#if skip
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
                    wordBx, wordEx,
                    pat.MatchPattern.UserCode);

                  pat.AtomText = atomText;
                  atomTextList.Add(pat);
                }
#endif
              }

              else
              {
                var foundPat = nonWord.FirstFoundPattern;
                wordBx = new StreamLocation(nonWord.Position).ToTextLocation(ScanStream);
                int lx = foundPat.MatchLength;
                TextLocation wordEx =
                  new StreamLocation(nonWord.Position + lx - 1).ToTextLocation(ScanStream);
                string scanText = ScanStream.Stream.Substring(nonWord.Position, lx);

                atomText = new AtomText(
                  foundPat.MatchPattern.DelimClassification.ToScanAtomCode().Value,
                  scanText, foundPat.MatchPattern.ReplacementValue,
                  wordBx, wordEx,
                  foundPat.MatchPattern.UserCode);

#if skip
                wordBx = new StreamLocation(nonWordIx).ToTextLocation(ScanStream);
                int lx = nonWordLx;
                TextLocation wordEx =
                  new StreamLocation(nonWordIx + lx - 1).ToTextLocation(ScanStream);
                string scanText = ScanStream.Stream.Substring(nonWordIx, lx);

                atomText = new AtomText(
                  nonWordPat.DelimClassification.ToScanAtomCode().Value,
                  scanText, nonWordPat.ReplacementValue,
                  wordBx, wordEx,
                  nonWordPat.UserCode);
#endif

              }
            }
          }
        }
        #endregion
      }

      // store the results in the return cursor.
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
        //        nx = new ScanAtomCursor(atomText, nonWordPat);
        if ((nonWord == null) || (nonWord.IsEmpty == true))
          nx = new ScanAtomCursor(atomText, nonWordPat);
        else
          nx = new ScanAtomCursor(atomText, nonWord.FirstFoundPattern.MatchPattern);
        nx.Position = RelativePosition.At;
      }

      return nx;
    }
  }

}
