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

    // ------------------------ ScanNextWord -------------------------
    // Scans to the next word in the string. ( a word being the text bounded by the
    // delimeter and whitespace characters as spcfd in the TextTraits argument )
    // Return null when end of string.
    public static ScanWordCursor ScanNextWord(
      ScanStream ScanStream, 
      TextTraits Traits, ScanWordCursor CurrentWord )
    {

      // components of the next word.
      TextWord wordPart = null ;
      TextLocation wordBx = null;
      ScanPattern nonWordPat = null;
      TextLocation nonWordLoc = null;
      int nonWordIx = -1;

      // stay at the current location. return copy of the cursor, but with stayatflag
      // turned off.
      if (CurrentWord.StayAtFlag == true)
      {
        nonWordPat = CurrentWord.DelimPattern;
        nonWordLoc = CurrentWord.DelimBx;
        wordPart = CurrentWord.Word;
        wordBx = CurrentWord.WordBx;
      }

      else
      {

        #region STEP1 setup the begin pos of the next word.
        // ----------------------------- STEP 1 ------------------------------
        // setup the begin pos of the next word.
        int bx;
        {
          // calc scan start position
          bx = ScanWord.CalcScanNextStart(ScanStream, Traits, CurrentWord);

          // advance past whitespace
          if (bx != -1)
          {
            bx = Scanner.ScanNotEqual(ScanStream.Stream, Traits.WhitespacePatterns, bx);
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
            var rv = ScanWord.IsolateNumericLiteral(ScanStream, Traits, bx);
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
            }

            // got a quote char. Isolate the quoted string, then find the delim that follows
            // the quoted string.
            if ((nonWordPat != null)
              && (nonWordPat.DelimClassification == DelimClassification.Quote)
              && (nonWordIx == bx))
            {
              var rv = IsolateQuotedWord(ScanStream, Traits, nonWordIx);
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
            var rv = ScanWord.IsolateWordText(
              ScanStream, Traits, litType, litText, bx, null);
            wordBx = rv.Item1;
            wordPart = rv.Item2;

#if skip
          wordBx = new StreamLocation(bx).ToTextLocation(ScanStream);
          if (litType != null)
          {
            wordPart = new TextWord(litText, WordClassification.Quoted, Traits);
          }
          else
          {
            wordPart = new TextWord(
              ScanStream.Substring(bx), WordClassification.Identifier, Traits);
          }
#endif
          }

          // got a word and a non word pattern.
          else if (nonWordIx > bx)
          {
            var rv = ScanWord.IsolateWordText(
              ScanStream, Traits, litType, litText, bx, nonWordIx);
            wordBx = rv.Item1;
            wordPart = rv.Item2;

#if skip
          wordBx = new StreamLocation(bx).ToTextLocation(ScanStream);
          int lx = foundIx - bx;
          wordPart = new TextWord(
            ScanStream.Substring(bx, lx), WordClassification.Identifier, Traits);
#endif

            nonWordLoc = new StreamLocation(nonWordIx).ToTextLocation(ScanStream);
          }

          // no word. just delim. 
          else
          {
            nonWordLoc = new StreamLocation(nonWordIx).ToTextLocation(ScanStream);

            // the delim is comment to end. store as a word.
            if (nonWordPat.DelimClassification == DelimClassification.CommentToEnd)
            {
              var rv = Scanner.ScanEqualAny(ScanStream.Stream, bx, Traits.NewLinePatterns);
              var eolPat = rv.Item1;
              var eolIx = rv.Item2;
              if (eolPat == null)
              {
                wordBx = new StreamLocation(nonWordIx).ToTextLocation(ScanStream);
                wordPart = new TextWord(
                  ScanStream.Substring(nonWordIx), WordClassification.CommentToEnd, Traits);
                nonWordLoc = null;
                nonWordPat = null;
              }
              else
              {
                wordBx = new StreamLocation(nonWordIx).ToTextLocation(ScanStream);
                int lx = eolIx - nonWordIx;
                var sloc = wordBx.ToStreamLocation(ScanStream);
                wordPart = new TextWord(
                  ScanStream.Substring(sloc.Value, lx), WordClassification.CommentToEnd, Traits);
                nonWordLoc = new StreamLocation(eolIx).ToTextLocation(ScanStream);
                nonWordPat = eolPat;
              }
            }

              // if the delim pattern is not non word ( a divider ), store the pattern also
            // as the word.
            else if (Traits.DelimPatternsThatAreNonWords.Contains(nonWordPat) == false)
            {
              wordBx = nonWordLoc;
              wordPart = new TextWord(
                nonWordPat.PatternValue,
                nonWordPat.DelimClassification.ToWordClassification().Value,
                Traits);
            }
          }
        }
        #endregion

        // delim is whitespace. scan ahead for something more meaningful than whitespace.
        if ((nonWordPat != null) && (Traits.IsWhitespace(nonWordPat)))
        {
          StreamLocation dx = nonWordLoc.ToStreamLocation(ScanStream);
          int fx = Scanner.ScanNotEqual(
            ScanStream.Stream, Traits.WhitespacePatterns, dx.Value + nonWordPat.Length);
          var pat = Traits.DelimPatterns.MatchAt(ScanStream.Stream, fx);
          if (pat != null)
          {
            nonWordLoc = new StreamLocation(fx).ToTextLocation(ScanStream);
            nonWordPat = pat;
          }
        }
      }

      // store the results in the return cursor.
      ScanWordCursor nx = null ;
      if (( wordPart == null ) && ( nonWordPat == null ))
      {
        nx = new ScanWordCursor( ) ;
        nx.Position = RelativePosition.End ;
      }
      else
      {
        nx = new ScanWordCursor(wordPart, wordBx, nonWordLoc, nonWordPat) ;
        nx.Position = RelativePosition.At ;
      }

      return nx ;
    }

    // ------------------------ ScanNextWord -------------------------
    // Scans to the next word in the string. ( a word being the text bounded by the
    // delimeter and whitespace characters as spcfd in the TextTraits argument )
    // Return null when end of string.
    public static WordCursor ScanNextWord(
      string Text, TextTraits Traits, WordCursor CurrentWord )
    {
      int Bx;
      WordCursor results = null;
      ScanPatternResults spr = null;

      // stay at the current location. return copy of the cursor, but with stayatflag
      // turned off.
      if (CurrentWord.StayAtFlag == true)
      {
        WordCursor nx = new WordCursor(CurrentWord) ;
        nx.StayAtFlag = false ;
      }

      else
      {
        // calc scan start position
        Bx = ScanWord.CalcStartBx(Text, CurrentWord);

        // advance past whitespace
        if ((Bx != -1) && (Bx <= (Text.Length - 1)))
        {
          Bx = Scanner.ScanNotEqual(
             Text, Bx, Text.Length - 1,
             CurrentWord.TextTraits.WhitespacePatterns).FoundPos;
        }

        // got the start of something. scan for the delimeter (could be the current char)
        spr = null;
        DelimClassification sprdc = DelimClassification.None;
        if ((Bx != -1) && (Bx <= (Text.Length - 1)))
        {
          spr =
            ScanWord.IsolateWord(Text, Bx, ref results, CurrentWord.TextTraits);
          if (spr.IsNotFound == true)
            sprdc = DelimClassification.EndOfString;
          else
            sprdc = spr.FoundPat.DelimClassification;
        }

        if (spr == null)
        {
          results.Position = RelativePosition.End;
          results.SetDelim(Text, null, -1, DelimClassification.EndOfString);
        }

        else
        {

          // depending on the word, isolate and store the delim that follows.

          // OpenNamedBraced. delim is the open brace char. 
          if (results.WordClassification == WordClassification.OpenNamedBraced)
          {
            ScanPatternResults spr2;
            int remLx = Text.Length - Bx;
            spr2 = Scanner.ScanEqualAny(
              Text, Bx, remLx, CurrentWord.TextTraits.OpenNamedBracedPatterns);
            results.SetDelim(
              Text,
              spr2.FoundPat.PatternValue,
              spr2.FoundPos, DelimClassification.OpenNamedBraced);
          }

            // OpenContentBraced. word and delim are the same.
          else if (results.WordClassification == WordClassification.OpenContentBraced)
          {
            results.SetDelim(
              Text,
              results.Word.Value, results.WordBx, DelimClassification.OpenContentBraced);
          }

            // word is CommentToEnd. delim is end of line.
          else if (results.WordClassification == WordClassification.CommentToEnd)
          {
            results.SetDelim(Text, spr, sprdc);
          }

            // process the NonWordResults returned by "ScanWord_IsolateWord"
          else
            ScanWord.IsolateDelim(
              Text, spr, ref results, CurrentWord.TextTraits);
        }

        // current word position.
        if (results.ScanEx == -1)
        {
          results.Position = RelativePosition.End;
          results.SetDelim(Text, null, -1, DelimClassification.EndOfString);
        }
        else
          results.Position = RelativePosition.At;
      }

      return results;
    }

  }
}
