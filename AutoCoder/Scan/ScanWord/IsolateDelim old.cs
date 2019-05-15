using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AutoCoder.Text;
using AutoCoder.Text.Enums;

namespace AutoCoder.Scan
{
  public static partial class ScanWord
  {

    // -------------------- IsolateDelim ---------------------------
    private static void IsolateDelim(
      string Text,
      ScanPatternResults PatternResults,
      ref WordCursor Results,
      TextTraits Traits)
    {
      // did not find a nonword char.  must have hit end of string.
      if (PatternResults.IsNotFound)
        Results.DelimClass = DelimClassification.EndOfString;

      // we have a delimiter of some kind.
      else
      {
        DelimClassification sprdc = PatternResults.FoundPat.DelimClassification;

        // delim is whitespace of some sort. Continue to look ahead for a non 
        // whitespace pattern. 
        if (Traits.IsWhitespaceDelimClass(sprdc) == true)
        {
          int bx = PatternResults.FoundPos;
          var spr = Scanner.ScanNotEqual(
             Text, bx, Text.Length - 1,
             Traits.WhitespacePatterns);
          if ( spr.FoundPat != null )
          {
          }
        }

        Results.WhitespaceFollowsWord = false;
        Results.WhitespaceFollowsDelim = false;
        Results.DelimIsWhitespace = false;

        // the delim is a hard delim ( not whitespace )
        if (sprdc != DelimClassification.Whitespace)
        {
          // Want the openContent brace to be processed as a standalone word. Use
          // virtual whitespace so the word that this open brace is the delim of will
          // have what appears to be a whitespace delim. Then the following word will
          // be the standalone open content brace char.
          if ((sprdc == DelimClassification.OpenContentBraced)
            && ( Traits.VirtualWhitespace == true ))
          {
            Results.SetDelim(
              Text,
              null, PatternResults.FoundPos, DelimClassification.VirtualWhitespace);
          }
          else
          {
            // delim is either as classified in the collection of NonWords or is
            // a PathPart delim.
            ScanPattern pat = Traits.GetPathPartDelim(
              Text, PatternResults.FoundPos);
            if (pat != null)
            {
              Results.SetDelim(
                Text,
                pat.PatternValue,
                PatternResults.FoundPos,
                DelimClassification.PathSep);
            }
            else
            {
              Results.SetDelim(
                Text,
                PatternResults.FoundPat.PatternValue,
                PatternResults.FoundPos,
                sprdc);
            }
          }
        }

          // whitespace immed follows the word text
        else
        {
          ScanWord.IsolateDelim_WhitespaceFollows(
            Text, PatternResults, ref Results, Traits);
        }
      }
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
    private static void IsolateDelim_WhitespaceFollows(
      string Text,
      ScanPatternResults PatternResults,
      ref WordCursor Results,
      TextTraits Traits)
    {
      Results.WhitespaceFollowsWord = true;
      ScanPattern nwPat = null;

      // Look for hard delim after the ws.
      ScanPatternResults scanResults =
        Scanner.ScanNotEqual(
        Text, PatternResults.FoundPos, Text.Length - 1,
        Traits.WhitespacePatterns);

      // look that the char after the ws is a nonword.
      if (scanResults.FoundPos != -1)
      {
        nwPat = Traits.NonWordPatterns.FindPatternAtSubstring(
          Text, scanResults.FoundPos, Text.Length - 1);
      }

      // the char after the whitespace is a non word (delim) char.
      if (nwPat != null)
      {
        DelimClassification nwdc = nwPat.DelimClassification;

        // is the delim actually a sep char in a path name.
        // so the delim is the whitespace.
        if (Traits.IsPathPartDelim(Text, scanResults.FoundPos))
        {
          ScanWord.IsolateDelim_SetDelimIsWhitespace(
            Text, Traits, Results, PatternResults.FoundPos);
        }

        // is a content open brace char. delim stays as whitespace because
        // content braces are considered standalone words.
        else if (nwPat.DelimClassification.IsOpenBraced( ))
        {
          ScanWord.IsolateDelim_SetDelimIsWhitespace(
            Text, Traits, Results, PatternResults.FoundPos);
        }

          // is a quote char. the quoted string is considered a word.
        else if (nwdc == DelimClassification.Quote)
        {
          ScanWord.IsolateDelim_SetDelimIsWhitespace(
            Text, Traits, Results, PatternResults.FoundPos);
        }

          // is an actual delim. 
        else
        {
          Results.SetDelim(
            Text,
            nwPat.PatternValue, scanResults.FoundPos, nwdc);
        }
      }

        // the whitespace char is the delim of record.
      else
      {
        ScanWord.IsolateDelim_SetDelimIsWhitespace(
          Text, Traits, Results, PatternResults.FoundPos);
      }
    }

    // --------------------------- IsolateDelim_SetDelimIsWhitespace ----------
    private static void IsolateDelim_SetDelimIsWhitespace(
      string Text, TextTraits Traits,
      WordCursor Results, int WsIx)
    {

      // store the actual string of whitespace characters. ( the whitespace can be
      // checked later to see if it contains tabs or newlines )
      ScanPatternResults spr = Scanner.ScanNotEqual(
        Text, WsIx, Text.Length - 1,
        Traits.WhitespacePatterns);

      string delimVlu = spr.ScannedOverString;
      Results.SetDelim(
        Text, delimVlu, WsIx, DelimClassification.Whitespace);

      Results.DelimIsWhitespace = true;
    }
  }
}
