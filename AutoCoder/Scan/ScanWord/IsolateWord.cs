using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AutoCoder.Text;
using AutoCoder.Text.Enums;
using AutoCoder.Core;

namespace AutoCoder.Scan
{
  public static partial class ScanWord
  {
    // -------------------- IsolateWord ---------------------------
    // We have a word starting at InBx. Scan to the end of the word.
    // Returns the word in the InOutResults parm.
    // Returns the word delim in the return argument.
    private static ScanPatternResults IsolateWord(
      string Text,
      int Bx,
      ref WordCursor Results,
      TextTraits Traits)
    {
      int bx;
      ScanPatternResults spr = null;

      bx = Bx;
      char ch1 = Text[bx];

      // is start of either verbatim string literal or quoted literal.
      if (
        ((Traits.VerbatimLiteralPattern != null) &&
        (Traits.VerbatimLiteralPattern.Match(Text, bx)))
        || (Traits.IsQuoteChar(ch1) == true )
        )
      {
        var rv = ScanWord.IsolateQuotedWord(Text, bx, Traits);
        bx = rv.Item1;
        int? ex = rv.Item2;
        string wordText = rv.Item3;
        WordClassification wc = WordClassification.Quoted;
        var litType = rv.Item4;
        spr = rv.Item5;
        Results.SetWord(wordText, wc, bx);
        Results.Word.LiteralType = litType;
      }

      else
      {

        // Scan the string for any of the non word patterns spcfd in Traits.
        DelimClassification sprdc = DelimClassification.None;
        int remLx = Text.Length - bx;
        spr = Scanner.ScanEqualAny(Text, bx, remLx, Traits.NonWordPatterns);
        if (spr.IsNotFound == false)
          sprdc = spr.FoundPat.DelimClassification;

        // a quote character within the name.  this is an error.
        if (sprdc == DelimClassification.Quote)
        {
          throw new ApplicationException(
            "quote character immed follows name character at position " +
            spr.FoundPos.ToString() + " in " + Text);
        }

        // no delim found. all word to the end of the string.
        else if (spr.IsNotFound)
        {
          string wordText = Text.Substring(Bx);
          Results.SetWord(wordText, WordClassification.Identifier, Bx);
        }

        // found an open named brace char
          // Open named braced words are words that combine the word and the braced contents. 
          // debateable that this feature is needed and should be retained. 
        else if (sprdc == DelimClassification.OpenNamedBraced)
        {
          Scanner.ScanWord_IsolateWord_Braced(
            Text, bx, spr, ref Results, Traits);
        }

        // delim is same position as the word.  so either the word is the delim ( an 
        // expression symbol ) or the word is empty ( the delim is a comma, semicolon,
        // ... a content divider )
        else if (spr.FoundPos == Bx)
        {

          if ((Traits.NonDividerIsWord == true)
            && (Traits.IsDividerDelim(spr.FoundPat.DelimClassification) == false))
          {
            Results.SetWord(
              spr.FoundPat.PatternValue, 
              spr.FoundPat.DelimClassification.ToWordClassification( ).Value,
              Bx,
              spr.FoundPat.LeadChar);
          }

            // start of CommentToEnd comment. This is a word, not a delim. Find the
          // end of the comment and set the delim to that end position.
          else if (sprdc == DelimClassification.CommentToEnd)
          {
            spr = Scanner.ScanWord_IsolateWord_CommentToEnd(
              Text, spr.FoundPos, ref Results, Traits);
          }

          else
            Results.SetNullWord();
        }

            // we have a word that ends with a delim.
        else
        {
          int lx = spr.FoundPos - Bx;
          string wordText = Text.Substring(Bx, lx);
          Results.SetWord(wordText, WordClassification.Identifier, Bx);
        }
      }

      // return ScanPatternResults of the delim that ends the word.
      return spr;
    }
  }
}

