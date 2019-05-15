using System;
using System.Collections.Generic;
using System.Text;
using AutoCoder;
using AutoCoder.File;
using AutoCoder.Core;
using AutoCoder.Scan;
using AutoCoder.Text.Enums;
using AutoCoder.Core.Enums;

namespace AutoCoder.Text
{
  public partial class Scanner
  {

    // --------------------------- ScanNextWord -------------------------------
    public static TextLinesWordCursor ScanNextWord(
      TextLines Lines, TextLinesWordCursor CurrentWord)
    {
      TextLinesWordCursor res = null;
      TextLinesCursor csr = null;
      TextLinesScanResults sr = null;

      // calc scan start position
      csr = ScanWord_CalcStartBx(Lines, CurrentWord);

      // advance past whitespace
      if ((csr.Position != RelativePosition.None) &&
        (csr.Position != RelativePosition.End))
      {
        sr = Lines.ScanNotEqual(
          csr, CurrentWord.TextTraits.WhitespacePatterns,
          ScannerEndOfLineConsider.ContinueScan);
        csr = sr.Pos;
      }

      // got the start of something. scan for the delimeter ( could be the current char )
      if (csr.Position == RelativePosition.At)
      {
      //ScanWord_IsolateWord(InBoundedString, Bx, ref results, InCurrentWord.TextTraits);
      }

      return res;
    }

    // ------------------------ ScanNextWord -------------------------
    // Scans to the next word in the string. ( a word being the text bounded by the
    // delimeter and whitespace characters as spcfd in the TextTraits argument )
    // Return null when end of string.
    public static WordCursor ScanNextWord(
      string Text,
      WordCursor CurrentWord)
    {
      BoundedString boundedString = new BoundedString(Text);
      WordCursor res = ScanNextWord(boundedString, CurrentWord);
      return res;
    }

    // ------------------------ ScanNextWord -------------------------
    // Scans to the next word in the string. ( a word being the text bounded by the
    // delimeter and whitespace characters as spcfd in the TextTraits argument )
    // Return null when end of string.
    public static WordCursor ScanNextWord(
      BoundedString BoundedString,
      WordCursor CurrentWord)
    {
      int Bx;
      WordCursor results = null;
      ScanPatternResults spr = null;

      // stay at the current location.
      if (CurrentWord.StayAtFlag == true)
      {
        if ((CurrentWord.Position != RelativePosition.At)
          && (CurrentWord.Position != RelativePosition.End))
          throw new ApplicationException("cursor not position at location to stay at");
        results = new WordCursor(CurrentWord);
        results.StayAtFlag = false;
      }

      else
      {
        results = new WordCursor()
          .SetString(BoundedString.String)
          .SetTraits(CurrentWord.TextTraits);
        results.VirtualCursor = WordCursor.enumVirtualCursor.None;

        // calc scan start position
        Bx = ScanWord_CalcStartBx(BoundedString, CurrentWord);

        // advance past whitespace
        if ((Bx != -1) && (Bx <= BoundedString.Ex))
         Bx = ScanNotEqual(
            BoundedString.String, Bx, BoundedString.Ex,
            CurrentWord.TextTraits.WhitespacePatterns ).FoundPos ;

        // got the start of something. scan for the delimeter (could be the current char)
        spr = null;
        DelimClassification sprdc = DelimClassification.None;
        if ((Bx != -1) && (Bx <= BoundedString.Ex))
        {
          spr = 
            ScanWord_IsolateWord(
            BoundedString, Bx, ref results, CurrentWord.TextTraits);
          if (spr.IsNotFound == true)
            sprdc = DelimClassification.EndOfString;
          else
            sprdc = spr.FoundPat.DelimClassification;
        }

        if (spr == null)
        {
          results.Position = RelativePosition.End;
          results.SetDelim(BoundedString, null, -1, DelimClassification.EndOfString);
        }

        else
        {

          // depending on the word, isolate and store the delim that follows.

          // OpenNamedBraced. delim is the open brace char. 
          if (results.WordClassification == WordClassification.OpenNamedBraced)
          {
            ScanPatternResults spr2;
            spr2 = ScanEqualAny(
              BoundedString, Bx, CurrentWord.TextTraits.OpenNamedBracedPatterns);
            results.SetDelim(
              BoundedString,
              spr2.FoundPat.PatternValue,
              spr2.FoundPos, DelimClassification.OpenNamedBraced);
          }

            // OpenContentBraced. word and delim are the same.
          else if (results.WordClassification == WordClassification.OpenContentBraced)
          {
            results.SetDelim(
              BoundedString,
              results.Word.Value, results.WordBx, DelimClassification.OpenContentBraced);
          }

            // word is CommentToEnd. delim is end of line.
          else if (results.WordClassification == WordClassification.CommentToEnd)
          {
            results.SetDelim(BoundedString, spr, sprdc ) ;
          }

            // process the NonWordResults returned by "ScanWord_IsolateWord"
          else 
            ScanWord_IsolateDelim(
              BoundedString, spr, ref results, CurrentWord.TextTraits);
        }

        // current word position.
        if (results.ScanEx == -1)
        {
          results.Position = RelativePosition.End;
          results.SetDelim(BoundedString, null, -1, DelimClassification.EndOfString);
        }
        else
          results.Position = RelativePosition.At;
      }

      return results;
    }

    // ----------------------- ScanWord_CalcStartBx ---------------------------
    // calc start position from which to start scan to the next word.
    private static int ScanWord_CalcStartBx(
      BoundedString BoundedString, WordCursor Word)
    {
      int Bx;
      switch (Word.Position)
      {
        case RelativePosition.Begin:
          Bx = BoundedString.Bx;
          break;
        case RelativePosition.Before:
          Bx = Word.ScanBx;
          break;

        case RelativePosition.After:
          Bx = Word.ScanEx + 1;
          break;

        case RelativePosition.At:
          Bx = Word.ScanEx + 1;
          break;
        
        case RelativePosition.End:
          Bx = BoundedString.Ex + 1;
          break;
        case RelativePosition.None:
          Bx = -1;
          break;
        
        default:
          Bx = -1;
          break;
      }

      if (Bx > BoundedString.Ex)
        Bx = -1;

      return Bx;
    }

    // ----------------------- ScanWord_CalcStartBx ---------------------------
    // calc start position from which to start scan to the next word.
    private static TextLinesCursor ScanWord_CalcStartBx(
      TextLines InLines, TextLinesWordCursor InWord)
    {
      //      TextLinesNode linesNode = null;
      TextLinesCursor bx = null;
      switch (InWord.Position)
      {
        case RelativePosition.Begin:
          bx = InLines.PosBgn();
          break;
        case RelativePosition.Before:
          break;
        case RelativePosition.After:
          break;
        case RelativePosition.End:
          bx = InLines.PosEnd();
          break;
        case RelativePosition.None:
          break;
        case RelativePosition.At:
          break;
        default:
          break;
      }
      return bx;
    }

    // -------------------- ScanWord_IsolateDelim ---------------------------
    private static void ScanWord_IsolateDelim(
      BoundedString InBoundedString,
      ScanPatternResults InPatternResults,
      ref WordCursor InOutResults,
      TextTraits InTraits)
    {
      // did not find a nonword char.  must have hit end of string.
      if (InPatternResults.IsNotFound)
        InOutResults.DelimClass = DelimClassification.EndOfString;
      
      // we have a delimiter of some kind.
      else
      {
        DelimClassification sprdc = InPatternResults.FoundPat.DelimClassification;

        InOutResults.WhitespaceFollowsWord = false;
        InOutResults.WhitespaceFollowsDelim = false;
        InOutResults.DelimIsWhitespace = false;

        // the delim is a hard delim ( not whitespace )
        if (sprdc != DelimClassification.Whitespace)
        {
          // Want the openContent brace to be processed as a standalone word. Use
          // virtual whitespace so the word that this open brace is the delim of will
          // have what appears to be a whitespace delim. Then the following word will
          // be the standalone open content brace char.
          if (sprdc == DelimClassification.OpenContentBraced)
          {
            InOutResults.SetDelim(
              InBoundedString,
              null, InPatternResults.FoundPos, DelimClassification.VirtualWhitespace);
          }
          else
          {
            // delim is either as classified in the collection of NonWords or is
            // a PathPart delim.
            ScanPattern pat = InTraits.GetPathPartDelim(
              InBoundedString, InPatternResults.FoundPos);
            if (pat != null)
            {
              InOutResults.SetDelim(
                InBoundedString,
                pat.PatternValue,
                InPatternResults.FoundPos,
                DelimClassification.PathSep);
            }
            else
            {
              InOutResults.SetDelim(
                InBoundedString,
                InPatternResults.FoundPat.PatternValue,
                InPatternResults.FoundPos,
                sprdc) ;
            }
          }
        }

          // whitespace immed follows the word text
        else
        {
          ScanWord_IsolateDelim_WhitespaceFollows(
            InBoundedString, InPatternResults, ref InOutResults, InTraits);
        }
      }
    }

    /// <summary>
    /// calc if the word starting at InBx is part of a path.
    /// </summary>
    /// <param name="InBoundedString"></param>
    /// <param name="InTraits"></param>
    /// <param name="InBx"></param>
    /// <returns></returns>
    private static bool ScanWord_IsolateDelim_IsPathPart(
      BoundedString InBoundedString, TextTraits InTraits, int InBx)
    {
      bool rc = false;

      WordCursor csr = InBoundedString.PositionBefore(InBx, InTraits);
      csr = ScanNextWord(InBoundedString, csr);

      if ((csr.IsPathPart == true))
        rc = true;

      return rc;
    }

    /// <summary>
    /// The delim after the word is whitspace. If what follows the whitespace
    /// is a delim char, then this whitspace is disregarded as the delim, and 
    /// the delim is what follows the whitespace.
    /// </summary>
    /// <param name="InBoundedString"></param>
    /// <param name="InNonWordResults"></param>
    /// <param name="InOutResults"></param>
    /// <param name="InTraits"></param>
    private static void ScanWord_IsolateDelim_WhitespaceFollows(
      BoundedString InBoundedString,
      ScanPatternResults InPatternResults,
      ref WordCursor InOutResults,
      TextTraits InTraits)
    {
      InOutResults.WhitespaceFollowsWord = true;
      ScanPattern nwPat = null;
      int nwMatchLx = 0;

      // Look for hard delim after the ws.
      ScanPatternResults scanResults =
        ScanNotEqual(
        InBoundedString.String, InPatternResults.FoundPos, 
        InBoundedString.Ex, InTraits.WhitespacePatterns);

      // look that the char after the ws is a nonword.
      if (scanResults.FoundPos != -1)
      {
        var rv = InTraits.NonWordPatterns.FindPatternAtSubstring(
          InBoundedString, scanResults.FoundPos ) ;
        nwPat = rv.Item1;
        nwMatchLx = rv.Item2;
      }

      // the char after the whitespace is a non word (delim) char.
      if ( nwPat != null )
      {
        DelimClassification nwdc = nwPat.DelimClassification;

        // is the delim actually a sep char in a path name.
        // so the delim is the whitespace.
        if (InTraits.IsPathPartDelim(InBoundedString, scanResults.FoundPos))
        {
          ScanWord_IsolateDelim_SetDelimIsWhitespace(
            InBoundedString, InTraits, InOutResults, InPatternResults.FoundPos);
        }

        // is a content open brace char. delim stays as whitespace because
        // content braces are considered standalone words.
        else if ( nwPat.DelimClassification.IsOpenBraced( ))
        {
          ScanWord_IsolateDelim_SetDelimIsWhitespace(
            InBoundedString, InTraits, InOutResults, InPatternResults.FoundPos);
        }

          // is a quote char. the quoted string is considered a word.
        else if (nwdc == DelimClassification.Quote)
        {
          ScanWord_IsolateDelim_SetDelimIsWhitespace(
            InBoundedString, InTraits, InOutResults, InPatternResults.FoundPos);
        }

          // is an actual delim. 
        else
        {
          InOutResults.SetDelim(
            InBoundedString,
            nwPat.PatternValue, scanResults.FoundPos, nwdc ) ;
        }
      }

        // the whitespace char is the delim of record.
      else
      {
        ScanWord_IsolateDelim_SetDelimIsWhitespace(
          InBoundedString, InTraits, InOutResults, InPatternResults.FoundPos);
      }
    }

    // --------------------------- ScanWord_IsolateDelim_SetDelimIsWhitespace ----------
    private static void ScanWord_IsolateDelim_SetDelimIsWhitespace(
      BoundedString InBoundedString, TextTraits InTraits,
      WordCursor InOutResults, int InWsIx)
    {

      // store the actual string of whitespace characters. ( the whitespace can be
      // checked later to see if it contains tabs or newlines )
      ScanPatternResults spr = ScanNotEqual(
        InBoundedString.String, InWsIx, InBoundedString.Ex,
        InTraits.WhitespacePatterns);

      string delimVlu = spr.ScannedOverString;
      InOutResults.SetDelim(
        InBoundedString, delimVlu, InWsIx, DelimClassification.Whitespace);
      
      InOutResults.DelimIsWhitespace = true;
    }

#if skip
    // -------------------- ScanWord_IsolatexWord ---------------------------
    private static TextLinesWordCursor ScanWord_IsolatexWord(
      TextLines InLines,
      TextLinesCursor InBxCsr,
      TextTraits InTraits)
    {
      TextLinesWordCursor tlwc = null;
      TextLinesCursor csr = null;
      TextLinesCursor endcsr = null;
      ScanPatternResults spr = null;

      csr = new TextLinesCursor(InBxCsr);
      char ch1 = InBxCsr.CursorChar;

      // look ahead to see if this word is braced.
      if (IsOpenQuoteChar(ch1) == false)
        spr = 
          ScanEqualAny( csr.LineData, csr.LineOx, csr.LineData.Length - 1, InTraits.NonWordPatterns ) ;
      else
        spr = new ScanPatternResults( -1 );

      // the rule is only braced words can span multiple lines. so if the word is
      // not braced, it can be parsed by the more general purpose IsolatexWord method.
      if ((IsOpenQuoteChar(ch1) == true) ||
        (InTraits.IsOpenBraceChar(spr.FoundChar.Value) == false))
      {
        ScanBoundedString bs = new ScanBoundedString(csr.LineData);
        WordCursor wc = new WordCursor();
        wc.SetTraits(InTraits);
        ScanWord_IsolateWord(bs, csr.LineOx, ref wc, InTraits);
        endcsr =
          new TextLinesCursor(csr.LinesNode, wc.ScanEx, AcRelativePosition.At);
        tlwc = new TextLinesWordCursor(wc, csr, endcsr);
      }

      else
      {

      }

      return tlwc;
    }
#endif

    // -------------------- ScanWord_IsolateWord ---------------------------
    // We have a word starting at InBx. Scan to the end of the word.
    // Returns the word in the InOutResults parm.
    // Returns the word delim in the return argument.
    private static ScanPatternResults ScanWord_IsolateWord(
      BoundedString InBoundedString,
      int InBx,
      ref WordCursor InOutResults,
      TextTraits Traits)
    {
      int Bx, Ix, Lx;
      string wordText;
      ScanPatternResults spr = null;

      Bx = InBx;
      char ch1 = InBoundedString.String[Bx];

      // is start of a verbatim string literal
      if (( Traits.VerbatimLiteralPattern != null ) &&
        ( Traits.VerbatimLiteralPattern.Match(InBoundedString, Bx )))
      {
      }
       
      // is quoted. the word runs to the closing quote.
      else if (IsOpenQuoteChar(ch1) == true)
      {
        Ix = ScanCloseQuote(InBoundedString.String, Bx, Traits.QuoteEncapsulation);
        if ((Ix == -1) || (Ix > InBoundedString.Ex))
          throw (new ApplicationException(
            "Closing quote not found starting at position " +
            Bx.ToString() + " in " + InBoundedString.String));
        Lx = Ix - Bx + 1;
        wordText = InBoundedString.String.Substring(Bx, Lx);
        InOutResults.SetWord(wordText, WordClassification.Quoted, Bx);

        // setup the non word which follows the closing quote.
        Bx = Ix + 1;
        if (InBoundedString.IsOutsideBounds(Bx))
          spr = new ScanPatternResults(-1) ;
        else
        {
          // the char that follows the closing quote must be a delim
          spr = ScanEqualAny(InBoundedString, Bx, Traits.NonWordPatterns);
          if (spr.FoundPos != Bx)
            throw new ApplicationException(
              "invalid char follows close quote at pos " + Ix.ToString() +
              " in " + Stringer.Head(InBoundedString.String, 80));
        }
      }
      else
      {

        // Scan the string for any of the non word patterns spcfd in Traits.
        DelimClassification sprdc = DelimClassification.None;
        spr = ScanEqualAny(InBoundedString, Bx, Traits.NonWordPatterns);
        if (spr.IsNotFound == false)
          sprdc = spr.FoundPat.DelimClassification;

        // a quote character within the name.  this is an error.
        if (sprdc == DelimClassification.Quote)
        {
          throw new ApplicationException(
            "quote character immed follows name character at position " +
            spr.FoundPos.ToString() + " in " + InBoundedString.String);
        }

        // no delim found. all word to the end of the string.
        else if (spr.IsNotFound)
        {
          wordText = InBoundedString.Substring(Bx);
          InOutResults.SetWord(wordText, WordClassification.Identifier, InBx);
        }

        // found an open named brace char
        else if (sprdc == DelimClassification.OpenNamedBraced)
        {
          ScanWord_IsolateWord_Braced(
            InBoundedString, Bx, spr, ref InOutResults, Traits);
        }

          // delim is same position as the word.  so there is no word, only a delim.
        else if (spr.FoundPos == InBx)
        {
          if ( Scanner.IsOpenBraced(sprdc ))
          {
            InOutResults.SetWord(
              spr.FoundPat.PatternValue, WordClassification.OpenContentBraced, Bx, 
              spr.FoundPat.LeadChar ) ;
          }

            // start of CommentToEnd comment. This is a word, not a delim. Find the
            // end of the comment and set the delim to that end position.
          else if (sprdc == DelimClassification.CommentToEnd)
          {
            spr = ScanWord_IsolateWord_CommentToEnd(
              InBoundedString, spr.FoundPos, ref InOutResults, Traits);
          }

          else
            InOutResults.SetNullWord();
        }

            // we have a word that ends with a delim.
        else
        {
          Lx = spr.FoundPos - InBx;
          wordText = InBoundedString.Substring(InBx, Lx);
          InOutResults.SetWord(wordText, WordClassification.Identifier, InBx);
        }
      }

      // return ScanPatternResults of the delim that ends the word.
      return spr;
    }

    // ----------------------- ScanWord_IsolatedWord_Braced -----------------------
    private static void ScanWord_IsolateWord_Braced(
      BoundedString InBoundedString,
      int InWordBx,
      ScanPatternResults InNonWordResults,
      ref WordCursor InOutResults,
      TextTraits InTraits)
    {
      string wordText;
      int Lx, Ix;

      int braceIx = InNonWordResults.FoundPos;

      char braceChar = InNonWordResults.FoundPat.LeadChar ;
      if (InTraits.BracedTreatment == ScannerBracedTreatment.Parts)
      {

        // a standalone open brace char. the brace char is the word ( and it will
        // also be the delim )
        if (InWordBx == braceIx)
          InOutResults.SetWord(
            InNonWordResults.FoundPat.PatternValue,
            WordClassification.OpenContentBraced, InWordBx, braceChar);
        else
        {
          wordText =
            InBoundedString.String.Substring(InWordBx, braceIx - InWordBx);
          InOutResults.SetWord(
            wordText, WordClassification.OpenNamedBraced, InWordBx, braceChar);
        }
      }

        // the whole braced word.  braced word runs all the way to the closing brace.
      else if (InTraits.BracedTreatment == ScannerBracedTreatment.Whole)
      {
        Ix = ScanCloseBrace(
          InBoundedString.String, 
          braceIx, InBoundedString.Ex, InTraits.QuoteEncapsulation);
        if (Ix == -1)
          throw new ApplicationException(
            "Closing brace not found starting at position " +
            braceIx + " in " + InBoundedString.String);
        Lx = Ix - InWordBx + 1;
        wordText = InBoundedString.String.Substring(InWordBx, Lx);
        if (InWordBx == braceIx)
          InOutResults.SetWord(
            wordText, WordClassification.ContentBraced, InWordBx, braceChar);
        else
          InOutResults.SetWord(
            wordText, WordClassification.NamedBraced, InWordBx, braceChar);
      }
    }

    // ----------------------- ScanWord_IsolatedWord_Braced -----------------------
    public static void ScanWord_IsolateWord_Braced(
      string Text,
      int WordBx,
      ScanPatternResults NonWordResults,
      ref WordCursor Results,
      TextTraits Traits)
    {
      string wordText;
      int Lx, Ix;

      int braceIx = NonWordResults.FoundPos;

      char braceChar = NonWordResults.FoundPat.LeadChar;
      if (Traits.BracedTreatment == ScannerBracedTreatment.Parts)
      {

        // a standalone open brace char. the brace char is the word ( and it will
        // also be the delim )
        if (WordBx == braceIx)
          Results.SetWord(
            NonWordResults.FoundPat.PatternValue,
            WordClassification.OpenContentBraced, WordBx, braceChar);
        else
        {
          wordText =
            Text.Substring(WordBx, braceIx - WordBx);
          Results.SetWord(
            wordText, WordClassification.OpenNamedBraced, WordBx, braceChar);
        }
      }

        // the whole braced word.  braced word runs all the way to the closing brace.
      else if (Traits.BracedTreatment == ScannerBracedTreatment.Whole)
      {
        int remLx = Text.Length - braceIx;
        Ix = ScanCloseBrace(
          Text,
          braceIx, remLx, Traits.QuoteEncapsulation);
        if (Ix == -1)
          throw new ApplicationException(
            "Closing brace not found starting at position " +
            braceIx + " in " + Text);
        Lx = Ix - WordBx + 1;
        wordText = Text.Substring(WordBx, Lx);
        if (WordBx == braceIx)
          Results.SetWord(
            wordText, WordClassification.ContentBraced, WordBx, braceChar);
        else
          Results.SetWord(
            wordText, WordClassification.NamedBraced, WordBx, braceChar);
      }
    }

    // ------------------ ScanWord_IsolatedWord_CommentToEnd -------------------
    private static ScanPatternResults ScanWord_IsolateWord_CommentToEnd(
      BoundedString InBoundedString,
      int InWordBx,
      ref WordCursor InOutResults, TextTraits InTraits)
    {
      string wordText;
      ScanPatternResults spr = null;

      // look for end of comment. ( either end of line or end of string )
      int fx = Scanner.ScanEqual(InBoundedString, InWordBx, Environment.NewLine).ResultPos ;
 
      if ( fx >= 0 )
      {
        int Lx = fx - InWordBx ;
        wordText = InBoundedString.Substring( InWordBx, Lx ) ;
        var rv = 
          InTraits.NonWordPatterns.FindPatternAtSubstring(InBoundedString, fx);
        var pat = rv.Item1;
        var matchLx = rv.Item2;
        spr = new ScanPatternResults(fx, pat);
      }
      else
      {
        wordText = InBoundedString.Substring( InWordBx ) ;
        spr = new ScanPatternResults(-1);
      }

      // store info on the word found in the return WordCursor argument.
      InOutResults.SetWord(wordText, WordClassification.CommentToEnd, InWordBx);

      // return value of method contains info on the word delim.
      return spr;
    }

    // ------------------ ScanWord_IsolatedWord_CommentToEnd -------------------
    public static ScanPatternResults ScanWord_IsolateWord_CommentToEnd(
      string Text,
      int WordBx,
      ref WordCursor Results, TextTraits Traits)
    {
      string wordText;
      ScanPatternResults spr = null;

      // look for end of comment. ( either end of line or end of string )
      int fx = Text.IndexOf(Environment.NewLine, WordBx);

      if (fx >= 0)
      {
        int Lx = fx - WordBx;
        wordText = Text.Substring(WordBx, Lx);
        var rv = Traits.NonWordPatterns.MatchPatternsAtStringLocation(Text, fx, 2);
        var pat = rv.Item1;
        var patMatchLx = rv.Item2;
        spr = new ScanPatternResults(fx, pat);
      }
      else
      {
        wordText = Text.Substring(WordBx);
        spr = new ScanPatternResults(-1);
      }

      // store info on the word found in the return WordCursor argument.
      Results.SetWord(wordText, WordClassification.CommentToEnd, WordBx);

      // return value of method contains info on the word delim.
      return spr;
    }

  }
}
