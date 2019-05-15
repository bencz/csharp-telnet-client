using System;
using AutoCoder ;
using AutoCoder.File;
using AutoCoder.Core;
using AutoCoder.Scan;
using AutoCoder.Collections;
using AutoCoder.Text.Enums;
using AutoCoder.Core.Enums;

namespace AutoCoder.Text
{

	// ------------------------- AutoCoder.Text.Scanner -----------------------
	public static partial class Scanner
	{
    public class ScanEnclosedResults
    {
      public int PreBx = -1 ;
      public string PreText = null ;
      public int FoundBx = -1 ;
      public string FoundText = null ;
      public int PostBx = -1 ;
      public string PostText = null ;
    }

		// -------------------------- ScanCharResults ----------------------------
		public struct ScanCharResults
		{
			public int a  ;		// the resulting pos of the char
			public char b ;		// the resulting char

			public ScanCharResults( int InNotFoundIx )
			{
				a = InNotFoundIx ;
				b = '\0' ;
			}
			public ScanCharResults( char InResultChar, int InResultPos )
			{
				a = InResultPos ;
				b = InResultChar ;
			}
			public char ResultChar 
			{
				get { return b ; }
			}
			public int ResultPos
			{
				get { return a ; }
			}

      public bool IsNotFound
      {
        get
        {
          if (ResultPos == -1)
            return true;
          else
            return false;
        }
      }
		}

    /// <summary>
    /// results of either scanning for a pattern string in the larger string.
    /// Or scanning for string which ends when delim is found.
    /// </summary>
    public class ScanStringResults
    {
      public int ResultPos = -1 ;
      public string ResultString = null ;
      Nullable<char> mDelimChar = null;
      int mDelimPos = -1;

      public ScanStringResults( int InNotFoundIx )
      {
        ResultPos = InNotFoundIx ;
        ResultString = null ;
      }

      public ScanStringResults( string InResultString, int InResultPos )
      {
        ResultString = InResultString ;
        ResultPos = InResultPos ;
      }

      public ScanStringResults(
        string InResultString, int InResultPos, 
        Nullable<char> InDelimChar, int InDelimPos )
      {
        ResultString = InResultString;
        ResultPos = InResultPos;
        mDelimChar = InDelimChar;
        mDelimPos = InDelimPos;
      }

      public Nullable<char> DelimChar
      {
        get { return mDelimChar; }
        set { mDelimChar = value; }
      }

      public int DelimPos
      {
        get { return mDelimPos; }
        set { mDelimPos = value; }
      }
    }

    // ------------------------ AdvanceNextWord -------------------------
    // Scans to the next word in the string. ( a word being the text bounded by the
    // delimeter and whitespace characters as spcfd in the TextTraits argument )
    // Return null when end of string.
    public static void AdvancexNextWord(
      string InString,
      WordCursor InOutWordCursor,
      TextTraits InTraits)
    {
      int Bx;
      BoundedString boundedString = new BoundedString(InString);
      ScanPatternResults spr = null;

      // calc scan start position
      Bx = ScanWord_CalcStartBx(boundedString, InOutWordCursor);

      // empty the word parts of the cursor.
      InOutWordCursor.EmptyWordParts();

      // advance past whitespace
      if (Bx <= boundedString.Ex)
        Bx = ScanNotEqual(
          boundedString.String, Bx, boundedString.Ex, 
          InTraits.WhitespacePatterns).FoundPos;

      // got the start of something. scan for the delimeter ( could be the current char )
      spr = null;
      if (Bx <= boundedString.Ex)
      {
        spr = ScanWord_IsolateWord(boundedString, Bx, ref InOutWordCursor, InTraits);
      }

      // depending on the word, isolate and store the delim that follows.
      ScanWord_IsolateDelim(boundedString, spr, ref InOutWordCursor, InTraits);

      // current word position.
      if (InOutWordCursor.ScanEx == -1)
        InOutWordCursor.Position = RelativePosition.End;
      else
        InOutWordCursor.Position = RelativePosition.At;
    }

    // --------------------------- IndexOf -------------------------------
    // scan for the first char equal to any of the pattern characters.
    public static ScanCharResults IndexOf(char[] InText, char InFindChar )
    {
      for ( int ix = 0 ; ix < InText.Length ; ++ix )
      {
        if ( InText[ix] == InFindChar )
          return new ScanCharResults( InText[ix], ix ) ;
      }
      return new ScanCharResults( -1 ) ;
    }

    // ------------------------- IsOpenBraced ----------------------------------
    public static bool IsOpenBraced(DelimClassification InDelimClass)
    {
      if ((InDelimClass == DelimClassification.OpenNamedBraced)
        || (InDelimClass == DelimClassification.OpenContentBraced))
        return true;
      else
        return false;
    }

		// ----------------------- IsOpenBraceChar ----------------------------
		public static bool IsOpenBraceChar( char InValue )
		{
			if (( InValue == '(' ) ||
				( InValue == '{' ) ||
				( InValue == '[' ) ||
				( InValue == '<' ))
				return true ;
			else
				return false ;
		}

		// ----------------------- IsOpenQuoteChar ----------------------------
		public static bool IsOpenQuoteChar( char InValue )
		{
			if (( InValue == '\'' ) ||
				( InValue == '"' ))
				return true ;
			else
				return false ;
		}

    public static WordCursor PositionAfter(string InText, int InIx, TextTraits InTraits)
    {
      WordCursor csr = new WordCursor();
      csr.Position = RelativePosition.After;
      csr.WordBx = InIx;
      csr.TextTraits = InTraits;
      return csr;
    }

    // ------------------------ PositionAfterWord ----------------------
    public static WordCursor PositionAfterWord(WordCursor InWord)
    {
      WordCursor word = new WordCursor(InWord);
      word.Position = RelativePosition.After;
      return word;
    }

    /// <summary>
    /// position a cursor before a character location in the string.
    /// </summary>
    /// <param name="InText"></param>
    /// <param name="InIx"></param>
    /// <returns></returns>
    public static WordCursor PositionBefore(string InText, int InIx, TextTraits InTraits )
    {
      WordCursor csr = new WordCursor();
      csr.Position = RelativePosition.Before;
      csr.WordBx = InIx;
      csr.TextTraits = InTraits;
      return csr;
    }

		// ------------------------ PositionBeforeWord ----------------------
		public static WordCursor PositionBeforeWord( WordCursor InWord )
		{
			WordCursor word = new WordCursor( InWord ) ;
			word.Position = RelativePosition.Before ;
			return word ;
		}

		// ----------------------- PositionBeginWord -----------------------------
		// position at begin of string to scan.
		public static WordCursor PositionBeginWord( string Text, TextTraits Traits )
		{
			WordCursor word = new WordCursor( ) ;
			word.Position = RelativePosition.Begin ;
      word.SetTraits( Traits ) ;
			return word ;
		}

    /// <summary>
    /// scan for a string which is enclosed by text before and after it.
    /// </summary>
    /// <param name="InString"></param>
    /// <param name="InBx"></param>
    /// <param name="InPreText"></param>
    /// <param name="InPostText"></param>
    /// <returns></returns>
    public static ScanEnclosedResults ScanEnclosed(
      string InString, int InBx, string InPreText, string InPostText)
    {
      int ex = 0;
      int lx = 0;
      int fx = -1;
      ScanEnclosedResults res = new ScanEnclosedResults( ) ;
      char[] ws = new char[] { ' ', '\t' };
      
      // the enclosing pre text.
      if ((InPreText == null) || (InPreText.Length == 0))
        res.FoundBx = InBx;
      else
      {
        fx = InString.IndexOf(InPreText, InBx);
        if (fx >= 0)
        {
          res.PreText = InPreText;
          res.PreBx = fx;
          res.FoundBx = res.PreBx + res.PreText.Length;
        }
      }

      // advance past whitespace to the actual enclosed text.
      if (res.FoundBx >= 0)
      {
        res.FoundBx = Scanner.ScanNotEqual(InString, res.FoundBx, ws).ResultPos;
      }

      // find the enclosing post text.
      if ((InPostText != null) && (InPostText.Length > 0))
      {
        if (res.FoundBx >= 0)
        {
          fx = InString.IndexOf(InPostText, res.FoundBx);
          if (fx >= 0)
          {
            res.PostText = InPostText;
            res.PostBx = fx;
          }
        }
      }

      // isolate the actual found text.
      if (res.FoundBx >= 0)
      {
        if (res.PostBx == -1)
          ex = InString.Length - 1;
        else
          ex = res.PostBx - 1;

        lx = ex - res.FoundBx + 1;
        res.FoundText = InString.Substring(res.FoundBx, lx).TrimEnd(ws);
      }

      // return the results.
      return res;
    }

		// ------------------------ ScanCloseBrace -------------------------
		/// <summary>
    /// scan for closing brace char that matches the start at open brace char. 
		/// </summary>
		/// <param name="InString"></param>
		/// <param name="InBx"></param>
		/// <returns></returns>
    public static int ScanCloseBrace( string InString, int InBx )
		{
			return( 
        ScanCloseBrace( InString, InBx, InString.Length - 1, QuoteEncapsulation.Escape )) ;
		}

		/// <summary>
    /// scan for closing brace char that matches the start at open brace char. 
		/// </summary>
		/// <param name="InString"></param>
		/// <param name="InBx"></param>
		/// <param name="InQem"></param>
		/// <returns></returns>
    public static int ScanCloseBrace(
			string InString, int InBx, QuoteEncapsulation InQem )
		{
      return ScanCloseBrace(InString, InBx, InString.Length - 1, InQem);
    }

    // ------------------------ ScanCloseBrace -------------------------
    // todo: pass TextTraits to a version of this method. Use recursion for each brace
    //       char found.
    /// <summary>
    /// scan for closing brace char that matches the start at open brace char. 
    /// </summary>
    /// <param name="InString"></param>
    /// <param name="InBx">scan start position</param>
    /// <param name="InEx">end position in string</param>
    /// <param name="InQem"></param>
    /// <returns></returns>
    public static int ScanCloseBrace(
      string InString, int InBx, int InEx, QuoteEncapsulation InQem)
    {
      char openBraceChar = InString[InBx];
      char closeBraceChar = AcCommon.CalcCloseBraceChar(openBraceChar);
      int Ix = InBx, Fx = 0;
      int ParenLevel = 1;
      
      if (InEx >= InString.Length)
        throw new ApplicationException("ScanCloseBrace end pos exceeds string length");

      while (true)
      {
        ++Ix;
        if (Ix > InEx)
        {
          Ix = -1;
          break;
        }

        char ch1 = InString[Ix];
        if (ch1 == openBraceChar)
          ++ParenLevel;
        else if (ch1 == closeBraceChar)
        {
          --ParenLevel;
          if (ParenLevel == 0)
            break;
        }
        else if (IsOpenQuoteChar(ch1) == true)
        {
          Fx = ScanCloseQuote(InString, Ix, InQem);
          Ix = Fx;
        }
      }
      return Ix;
    }

		// --------------------------- ScanCloseQuote ------------------------------
		public static int ScanCloseQuote(
			string InString, int InBx, QuoteEncapsulation InQem )
		{
			char QuoteChar = InString[InBx] ;
			int cloqIx = -1 ;

			for( int Ix = InBx + 1 ; Ix < InString.Length ; ++Ix )
			{
				char ch1 = InString[Ix] ;

				// using the escape method to enclose quote chars. This means the escape char
				// is used to encapsulate other special characters in the quoted string.
				// todo: dequote using "QuotedStringTraits" rules.
				if (( ch1 == '\\' )
					&& ( InQem == QuoteEncapsulation.Escape )
					&& ( Ix < ( InString.Length - 1 )))
				{
					++Ix ;
				}

					// quote char enquoted using the "double the char" method.
				else if (( ch1 == QuoteChar ) &&
					( InQem == QuoteEncapsulation.Double ) &&
					( AcCommon.PullChar( InString, Ix + 1 ) == QuoteChar ))
				{
					++Ix ;
				}

					// found the closing quote char.
				else if ( ch1 == QuoteChar )
				{
					cloqIx = Ix ;
					break ;
				}
			}

			return cloqIx ;
		}

    // --------------------------- ScanCloseQuote ------------------------------
    public static int ScanCloseQuote(
      BoundedString InBounded, int InBx, QuoteEncapsulation InQem)
    {
      char QuoteChar = InBounded.String[InBx];
      int cloqIx = -1;

      for (int Ix = InBx + 1; Ix <= InBounded.Ex; ++Ix)
      {
        char ch1 = InBounded.String[Ix];

        // using the escape method to enclose quote chars. This means the escape char
        // is used to encapsulate other special characters in the quoted string.
        // todo: dequote using "QuotedStringTraits" rules.
        if ((ch1 == '\\')
          && (InQem == QuoteEncapsulation.Escape)
          && (Ix < InBounded.Ex))
        {
          ++Ix;
        }

          // quote char enquoted using the "double the char" method.
        else if (
          (ch1 == QuoteChar) &&
          (InQem == QuoteEncapsulation.Double) &&
          ( Ix < InBounded.Ex ) &&
          ( InBounded.String[Ix+1] == QuoteChar ))
        {
          ++Ix;
        }

          // found the closing quote char.
        else if (ch1 == QuoteChar)
        {
          cloqIx = Ix;
          break;
        }
      }

      return cloqIx;
    }

    /// <summary>
    /// scan for pattern string within string.
    /// </summary>
    /// <param name="InBounded"></param>
    /// <param name="InBx"></param>
    /// <param name="InPattern"></param>
    /// <returns></returns>
    public static ScanStringResults ScanEqual(
      BoundedString InBounded, int InBx, string InPattern)
    {
      int Ix;
      InBounded.ThrowBeforeBounds(InBx);
      int remLx = InBounded.GetRemainingLength(InBx);

      if (remLx < InPattern.Length)
        Ix = -1;
      else
        Ix = InBounded.String.IndexOf(InPattern, InBx ) ;

      if (Ix == -1)
        return (new ScanStringResults(-1));
      else
      {
        char ch1 = InBounded.String[Ix];
        return (new ScanStringResults(InPattern, Ix));
      }
    }

		// --------------------------- ScanEqualAny -------------------------------
		// scan for the first char equal to any of the pattern characters.
		public static ScanCharResults ScanEqualAny( string InValue, char[] InPatternChars )
		{
			return( ScanEqualAny( InValue, 0, InValue.Length - 1, InPatternChars )) ;
		}

		// --------------------------- ScanEqualAny -------------------------------
		// scan for the first char equal to any of the pattern characters.
		public static ScanCharResults ScanEqualAny(
			string InValue,
			int InBx,
			char[] InPatternChars )
		{
      int remLx = InValue.Length - InBx;
      return ScanEqualAny(InValue, InBx, remLx, InPatternChars);
		}

    public static ScanPatternResults ScanEqualAny(
      string InString, ScanPatterns InPatterns)
    {
      ScanPatternResults spr = ScanEqualAny(InString, 0, InString.Length, InPatterns);
      return spr;
    }

    public static ScanPatternResults ScanEqualAny(
      BoundedString InString, int InIx, ScanPatterns InPatterns)
    {
      int lx = InString.Ex - InIx + 1;
      ScanPatternResults spr = ScanEqualAny( InString.String, InIx, lx, InPatterns);
      return spr;
    }

    /// <summary>
    /// Scan string for any of the pattern strings in ScanPatterns.
    /// </summary>
    /// <param name="InString"></param>
    /// <param name="InBx"></param>
    /// <param name="InLx"></param>
    /// <param name="InPatterns"></param>
    /// <returns></returns>
    public static ScanPatternResults ScanEqualAny(
      string InString, int InIx, int InLx, ScanPatterns InPatterns)
    {
      ScanPattern pat = null ;
      ScanPatternResults spr = null;
      int ix = InIx;
      int ex = InIx + InLx - 1;

      while (true)
      {
        spr = null;
        int remLx = ex - ix + 1;
        if (remLx <= 0)
          break;

        ScanCharResults scr = ScanEqualAny(
          InString, ix, remLx, InPatterns.LeadChars);
        if (scr.IsNotFound == true)
        {
          spr = new ScanPatternResults(-1);
          break;
        }

        pat = InPatterns.FindPatternAtSubstring(InString, scr.ResultPos, ex);
        if (pat != null)
        {
          spr = new ScanPatternResults(scr.ResultPos, pat);
          break;
        }

        // advance ix to resume scan after the found lead char.
        ix = scr.ResultPos + 1;
      }

      return spr;
    }

    public static Tuple<ScanPattern,int> ScanEqualAny(
      string Text, int Start, ScanPatterns Patterns)
    {
      ScanPattern pat = null;
      int ix = Start;

      while (true)
      {
        int remLx = Text.Length - ix;
        if (remLx <= 0)
        {
          ix = -1;
          break;
        }

        ScanCharResults scr = ScanEqualAny(
          Text, ix, remLx, Patterns.LeadChars);
        if (scr.IsNotFound == true)
        {
          ix = -1;
          break;
        }

        ix = scr.ResultPos;
        pat = Patterns.FindPatternAtSubstring(Text, ix, Text.Length - 1);
        if (pat != null)
        {
          break;
        }

        // advance ix to resume scan after the found lead char.
        ix = ix + 1;
      }

      return new Tuple<ScanPattern, int>(pat, ix);
    }

    // --------------------------- ScanEqualAny -------------------------------
    // scan for the first char equal to any of the pattern characters.
    /// <summary>
    ///  scan for the first char equal to any of the pattern characters. ( bounded version )
    /// </summary>
    /// <param name="InValue"></param>
    /// <param name="InBx">scan start position</param>
    /// <param name="InLx">count of characters to scan</param>
    /// <param name="InPatternChars"></param>
    /// <returns></returns>
    public static ScanCharResults ScanEqualAny(
      string InValue,
      int InBx,
      int InLx,
      char[] InPatternChars)
    {
      int Ix = InValue.IndexOfAny(InPatternChars, InBx, InLx);
      if (Ix == -1)
        return (new ScanCharResults(-1));
      else
      {
        char ch1 = InValue[Ix];
        return (new ScanCharResults(ch1, Ix));
      }
    }

    /// <summary>
    /// Scan with the bounded string for a char equal any of the pattern characters.
    /// </summary>
    /// <param name="InBounded"></param>
    /// <param name="InBx"></param>
    /// <param name="InPatternChars"></param>
    /// <returns></returns>
    public static ScanCharResults ScanEqualAny(
      BoundedString InBounded,
      int InBx,
      char[] InPatternChars)
    {
      int Ix;
      InBounded.ThrowBeforeBounds(InBx);
      
      int Lx = InBounded.Ex - InBx + 1;
      if (Lx <= 0)
        Ix = -1;
      else
        Ix = InBounded.String.IndexOfAny(InPatternChars, InBx, Lx);
      
      if (Ix == -1)
        return (new ScanCharResults(-1));
      else
      {
        char ch1 = InBounded.String[Ix];
        return (new ScanCharResults(ch1, Ix));
      }
    }

    /// <summary>
    /// Scan forward in string for any of the pattern strings. Use the associated 
    /// pattern lead chars array to quicken the scan.
    /// </summary>
    /// <param name="InString"></param>
    /// <param name="InBx"></param>
    /// <param name="InLx"></param>
    /// <param name="InPattern"></param>
    /// <param name="InPatternLeadChars"></param>
    /// <returns></returns>
    public static ScanPatternResults ScanEqualAnyStrings(
      string InString, int InBx, int InLx,
      string[] InPattern, char[] InPatternLeadChars )
    {
      string pat = null;
      int patIx = -1;

      int fx = -1;
      int scanLx = InLx;
      int stringIx = InBx;
      int stringEx = InBx + InLx - 1;

      while (pat == null)
      {
        pat = null;
        patIx = -1;

        // scan forward in the string for any of the pattern lead chars.
        scanLx = stringEx - stringIx + 1;
        fx = InString.IndexOfAny(InPatternLeadChars, stringIx, scanLx);
        if (fx == -1)
          break;
        char ch1 = InString[fx];

        // length remaining in search string. from found location to end.
        int remLx = stringEx - fx + 1;

        // one of the pattern lead chars is found in the string being scanned.
        // For each instance of that lead char, check that the full pattern
        // string matches the substring at the found pos.
        patIx = -1;
        while (true)
        {
          pat = null;
          int mx = patIx + 1;
          if (mx > InPatternLeadChars.Length)
          {
            patIx = -1;
            break;
          }

          patIx = Array.IndexOf(InPatternLeadChars, ch1, mx);
          if (patIx == -1)
            break;

          // test if the substring starting at the char found position matches the full
          // pattern of the found char.
          pat = InPattern[patIx];
          if ((pat.Length <= remLx) && (InString.Substring(fx, pat.Length) == pat))
            break;
        }

        // pattern string is found.
        if (patIx != -1)
        {
          break;
        }
      }

      // return the results.
      if (fx == -1)
        return new ScanPatternResults(-1);
      else
        return new ScanPatternResults(fx, pat, patIx);
    }


    /// <summary>
    /// Scan forward in string for any of the pattern strings.
    /// </summary>
    /// <param name="InString"></param>
    /// <param name="InBx"></param>
    /// <param name="InLx"></param>
    /// <param name="InPattern"></param>
    /// <returns></returns>
    public static ScanPatternResults ScanEqualAnyStrings(
      string InString, int InBx, int InLx,
      string[] InPattern)
    {

      // build array of pattern leading characters.
      char[] patChars = Arrayer.StringArrayToLeadingCharArray(InPattern);

      ScanPatternResults spr = ScanEqualAnyStrings(
        InString, InBx, InLx, InPattern, patChars);

      return spr;
    }


    /// <summary>
    /// Scan for any of the pattern strings in the search string.
    /// </summary>
    /// <param name="InString"></param>
    /// <param name="InBx"></param>
    /// <param name="InLx"></param>
    /// <param name="InPattern"></param>
    /// <returns></returns>
    public static ScanStringResults ScanEqualAnyxStrings(
      string InString, int InBx, int InLx,
      string[] InPattern)
    {
      string foundPat = null;
      int fx = -1;
      int lx = InLx;
      int ix = InBx;
      int Ex = InBx + InLx - 1;

      // build array of pattern leading characters.
      char[] patChars = Arrayer.StringArrayToLeadingCharArray(InPattern);
      
      while (foundPat == null)
      {
        lx = Ex - ix + 1;
        fx = InString.IndexOfAny(patChars, ix, lx);
        if (fx == -1)
          break;

        // length remaining in search string. from found location to end.
        int remLx = Ex - fx + 1;

        char ch1 = InString[fx];
        int mx = 0;
        while (true)
        {
          int nx = Array.IndexOf(patChars, ch1, mx);
          if (nx == -1)
            break;
          
          // test if the substring starting at the char found position matches the full
          // pattern of the found char.
          string pat = InPattern[nx];
          if ((pat.Length <= remLx) && (InString.Substring(fx, pat.Length) == pat))
            break;

          // full pattern does not match. Setup in patChars array to search for the next
          // pattern search string with the found leading char.
          mx = nx + 1;

        }


        foreach (string s1 in InPattern)
        {
          if ((s1.Length <= remLx)
            && (InString.Substring(fx, s1.Length) == s1))
          {
            foundPat = s1;
            break;
          }
        }
      }

      return new ScanStringResults(foundPat, fx);
    }

    /// <summary>
    /// Scan the bounded string for any of the pattern characters, 
    /// bypassing quoted strings within the scan space.
    /// </summary>
    /// <param name="InBounded"></param>
    /// <param name="InBx"></param>
    /// <param name="InPatternChars"></param>
    /// <param name="InQem"></param>
    /// <returns></returns>
    public static ScanCharResults ScanEqualAny_BypassQuoted(
      BoundedString InBounded,
      int InBx,
      char[] InPatternChars,
      QuoteEncapsulation InQem)
    {
      ScanCharResults res;
      int Fx;
      char[] patternChars = Arrayer.Concat<char>(InPatternChars, new char[] { '"', '\'' });
      int Ix = InBx;
      while (true)
      {
        res = ScanEqualAny(InBounded, Ix, patternChars);
        if (res.ResultPos == -1)
          break;
        else if (IsOpenQuoteChar(res.ResultChar) == true)
        {
          Fx = ScanCloseQuote(InBounded, res.ResultPos, InQem);
          if (Fx == -1)
            throw new ApplicationException("End of quoted string not found");
          else if (Fx == InBounded.Ex)
          {
            res = new ScanCharResults(-1);
            break;
          }
          else
            Ix = Fx + 1;
        }
        else
          break;
      }

      return res;
    }

    // ------------------------ ScanFirstWord -------------------------
    public static WordCursor ScanFirstWord(string InString, TextTraits InTraits)
    {
      BoundedString boundedString = new BoundedString(InString);
      WordCursor res = ScanFirstWord(boundedString, InTraits);
      return res;
    }

    /// <summary>
    /// scan for the first sequence of chars in string after initial whitespace.
    /// </summary>
    /// <param name="InString"></param>
    /// <param name="InWhitespace"></param>
    /// <param name="InMaxReturnChars"></param>
    /// <returns></returns>
    public static ScanStringResults ScanFirstChars(
      string InString, Whitespace InWhitespace, int InMaxReturnChars)
    {
      int ix = 0 ;
      ScanStringResults res = null;

      ScanCharResults cr = Scanner.ScanNotEqual(InString, 0, InWhitespace.WhitespaceChars);
      if ( cr.ResultPos == -1 )
        ix = 0 ;
      else
        ix = cr.ResultPos ;

      if (ix >= InString.Length)
      {
        res = new ScanStringResults(-1);
      }
      else
      {
        string fsChars = Stringer.SubstringLenient(InString, ix, InMaxReturnChars ) ;
        res = new ScanStringResults(fsChars, ix ) ;
      }

      return res;
    }

    // ------------------------ ScanFirstWord -------------------------
    public static WordCursor ScanFirstWord(
      BoundedString InBoundedString, TextTraits InTraits)
    {
      WordCursor csr = new WordCursor();
      csr.Position = RelativePosition.Begin;
      csr.TextTraits = InTraits;
      WordCursor res = ScanNextWord(InBoundedString, csr);
      return res;
    }

    public static TextLinesWordCursor ScanFirstWord(
      TextLines InLines, TextTraits InTraits)
    {
      TextLinesWordCursor csr = new TextLinesWordCursor();
      csr.Position = RelativePosition.Begin;
      csr.TextTraits = InTraits;

      TextLinesWordCursor res = ScanNextWord(InLines, csr);

      return res;
    }

    /// <summary>
    /// scan in string until delim character is found.
    /// </summary>
    /// <param name="InString"></param>
    /// <param name="InBx"></param>
    /// <param name="InWhitespace"></param>
    /// <param name="InDelim"></param>
    /// <returns></returns>
    public static ScanStringResults ScanNextString(
      string InString, int InBx, char[] InWhitespace, char[] InDelim )
    {
      string nextString = null ;
      int nextBx = -1 ;
      Nullable<char> foundDelim = null ;
      int delimPos = -1 ;
      int Ix = 0;
      int Lx = 0 ;
      ScanCharResults res ;

      // advance past whitespace.
      if (InWhitespace.Length == 0)
        Ix = InBx;
      else
      {
        res = ScanNotEqual(InString, InBx, InWhitespace);
        Ix = res.ResultPos;
      }

      // scan until delimiter.
      if (Ix != -1)
      {
        nextBx = Ix;
        res = ScanEqualAny(InString, Ix, InDelim);

        // delim not found. string goes until end of string.
        if (res.ResultPos == -1)
        {
          nextString = InString.Substring(nextBx);
        }

        // found string ends immed before the found delim char.
        else
        {
          delimPos = res.ResultPos;
          foundDelim = res.ResultChar;
          Lx = delimPos - nextBx;
          nextString = InString.Substring(nextBx, Lx);
        }
      }

      return new ScanStringResults(nextString, nextBx, foundDelim, delimPos ) ;
    }

    // -------------------------------- ScanNotEqual ------------------------
    public static int ScanNotEqual(
      string Text, ScanPatterns Patterns, int Start)
    {

      // step thru the string 1 char at a time.
      int ix = Start;
      int ex = Text.Length - 1;
      while (true)
      {
        if (ix > ex)
        {
          ix = -1;
          break;
        }
        char ch1 = Text[ix];

        // the current char is not equal any of the pattern lead chars.
        int fx = Array.IndexOf<char>(Patterns.LeadChars, ch1);
        if (fx == -1)
          break;

        ScanPattern equalPat = null;
        int remLx = ex - ix + 1 ;
        foreach (var pat in Patterns)
        {
          if (pat.PatternValue.Length <= remLx)
          {
            if (pat.PatternValue == Text.Substring(ix, pat.PatternValue.Length))
            {
              equalPat = pat;
              break;
            }
          }
        }

        // text at the current location is not equal any of the patterns.
        if (equalPat == null)
          break;

        ix += equalPat.Length ;
      }

      return ix ;
    }

    // -------------------------------- ScanNotEqual ------------------------
    public static ScanPatternResults ScanNotEqual(
      string InString, int InIx, int InEx, ScanPatterns InScanPatterns )
    {

      // step thru the string 1 char at a time.
      int stringIx = InIx;
      while (true)
      {
        if (stringIx > InEx)
        {
          stringIx = -1;
          break;
        }
        char ch1 = InString[stringIx];

        // the current char is not equal any of the pattern lead chars.
        int patIx = Array.IndexOf<char>(InScanPatterns.LeadChars, ch1) ;
        if ( patIx == -1)
          break;

        ScanPattern equalPat = null;
        ScanPattern pat = InScanPatterns.ScanPatternsArray[patIx];
        while (pat != null)
        {
          bool rv = Stringer.CompareSubstringEqual(InString, stringIx, InEx, pat.PatternValue);
          if (rv == true)
          {
            if (equalPat == null)
              equalPat = pat;

              // Matching pattern already found, but this pattern also matches and it is
              // longer. Always return the longer pattern.
            else if (pat.PatternValue.Length > equalPat.PatternValue.Length)
              equalPat = pat;

          }
          pat = pat.NextSameLeadChar;
        }

        // check for the substring at the current location in string as not equal any
        // of the ScanNotEqual pattern strings.
        if (equalPat == null)
        {
          break;
        }

        // advance past the whitespace string.
        stringIx += equalPat.PatternValue.Length;
      }

      // return the scan results
      ScanPatternResults spr = null;
      if (stringIx == -1)
        spr = new ScanPatternResults(-1);
      else
        spr = new ScanPatternResults(stringIx, InString[stringIx]);
      
      spr.ScannedString = InString;
      spr.ScanStartIx = InIx;
      spr.ScanBoundsEx = InEx;

      return spr;
    }

		// ----------------------------- ScanNotEqual -----------------------------
		/// <summary>
		/// scan for character not equal to single pattern character
		/// </summary>
		/// <param name="InValue"></param>
		/// <param name="InBx"></param>
		/// <param name="InPattern"></param>
		/// <returns></returns>
    public static ScanCharResults ScanNotEqual(
			string InValue,
			int InBx,
			char InPattern )
		{
      return ScanNotEqual(InValue, InBx, InValue.Length - 1, InPattern);
		}

    // ----------------------------- ScanNotEqual -----------------------------
    /// <summary>
    /// scan for character not equal to single pattern character ( bounded version )
    /// </summary>
    /// <param name="InValue"></param>
    /// <param name="InBx"></param>
    /// <param name="InEx">scan end position</param>
    /// <param name="InPattern"></param>
    /// <returns></returns>
    public static ScanCharResults ScanNotEqual(
      string InValue,
      int InBx, int InEx,
      char InPattern)
    {
      int Fx = -1;
      int Ix = InBx - 1;
      while (true)
      {
        ++Ix;
        if (Ix > InEx)
          break;
        char ch1 = InValue[Ix];
        if (ch1 != InPattern)
        {
          Fx = Ix;
          break;
        }
      }
      if (Fx == -1)
        return (new ScanCharResults(-1));
      else
        return (new ScanCharResults(InValue[Fx], Fx));
    }

		// --------------------------- ScanNotEqual-------------------------------
    /// <summary>
    /// Scan for character not equal to any of the pattern characters.
    /// </summary>
    /// <param name="InValue"></param>
    /// <param name="InBx"></param>
    /// <param name="InPatternChars"></param>
    /// <returns></returns>
		public static ScanCharResults ScanNotEqual(
			string InValue,
			int InBx,
			char[] InPatternChars )
		{
      return ScanNotEqual(InValue, InBx, InValue.Length - 1, InPatternChars);
		}

    // --------------------------- ScanNotEqual-------------------------------
    /// <summary>
    /// scan for character not equal to any of the pattern characters ( bounded version )
    /// </summary>
    /// <param name="InValue"></param>
    /// <param name="InBx"></param>
    /// <param name="InEx"></param>
    /// <param name="InPatternChars"></param>
    /// <returns></returns>
    public static ScanCharResults ScanNotEqual(
      string InValue,
      int InBx, int InEx,
      char[] InPatternChars)
    {
      int PatLx = InPatternChars.Length;
      for (int Ix = InBx; Ix <= InEx; ++Ix)
      {
        char ch1 = InValue[Ix];
        bool IsEqual = false;
        for (int Jx = 0; Jx < PatLx; ++Jx)
        {
          if (ch1 == InPatternChars[Jx])
          {
            IsEqual = true;
            break;
          }
        }
        if (IsEqual == false)
          return (new ScanCharResults(ch1, Ix));
      }
      return (new ScanCharResults(-1));
    }

    /// <summary>
    /// scan for location in string which does not contain all of the pattern 
    /// strings.
    /// </summary>
    /// <param name="InString"></param>
    /// <param name="InBx"></param>
    /// <param name="InLx"></param>
    /// <param name="InPattern"></param>
    /// <returns></returns>
    public static ScanCharResults ScanNotEqualStrings(
      string InString, int InBx, int InLx,
      string[] InPattern)
    {
      string foundPat = null;
      int ix = InBx;
      int Ex = InBx + InLx - 1;

      while (foundPat == null)
      {
        // test if current location is equal any of the pattern strings.
        foundPat = null;
        int remLx = Ex - ix + 1;
        foreach (string s1 in InPattern)
        {
          if ((s1.Length <= remLx)
            && (InString.Substring(ix, s1.Length) == s1))
          {
            foundPat = s1;
            break;
          }
        }

        // pattern not found. 
        if (foundPat == null)
          break;

          // advance in string for the length of what was found.
        else
          ix += foundPat.Length;
      }

      if (ix <= Ex)
        return new ScanCharResults(InString[ix], ix);
      else
        return new ScanCharResults(-1);
    }

    /// <summary>
    /// Scan for keyword followed by enclosign parenthesis.
    /// Return the string within the encoding paren.
    /// Return null if keyword is not found.
    /// </summary>
    /// <param name="InText"></param>
    /// <param name="InKwd"></param>
    /// <returns></returns>
    public static string ScanParenEnclosedValue(this string InText, string InKwd)
    {
      string foundValue = null;
      string findText = InKwd + "(";
      int fx = InText.IndexOf(findText);
      if (fx >= 0)
      {
        int braceBx = fx + InKwd.Length;
        int ex = Scanner.ScanCloseBrace(InText, braceBx);
        if (ex >= 0)
        {
          int valueBx = braceBx + 1;
          int valueLx = ex - valueBx;
          if (valueLx > 0)
            foundValue = InText.Substring(valueBx, valueLx).Trim();
          else
            foundValue = "";
        }
      }

      return foundValue;
    }


		// --------------------------- ScanReverseEqual-------------------------------
		// scan for the first char equal to any of the pattern characters.
		public static ScanCharResults ScanReverseEqual(
			string InValue,
			int InBx,
			char[] InPatternChars )
		{
			int PatLx = InPatternChars.Length ;
			for( int Ix = InBx ; Ix > 0 ; --Ix )
			{
				char ch1 = InValue[Ix] ;
				for( int Jx = 0 ; Jx < PatLx ; ++Jx )
				{
					if ( ch1 == InPatternChars[Jx] )
						return( new ScanCharResults( ch1, Ix )) ;
				}
			}
			return( new ScanCharResults( -1 )) ;
		}

    /// <summary>
    /// Scan from end to start in string for the pattern character.
    /// </summary>
    /// <param name="InValue"></param>
    /// <param name="InPatternChar"></param>
    /// <returns></returns>
    public static ScanCharResults ScanReverseEqual(
      string InValue, char InPatternChar)
    {
      int bx = InValue.Length - 1;
      if (bx < 0)
        return new ScanCharResults(-1);
      else
      {
        for (int Ix = bx; Ix >= 0; --Ix)
        {
          if (InValue[Ix] == InPatternChar)
            return (new ScanCharResults( InValue[Ix], Ix));
        }
      }
      return (new ScanCharResults(-1));
    }

		// --------------------------- ScanReverseNotEqual-------------------------------
		// scan reverse for the first char not equal to all of the pattern characters.
		public static ScanCharResults ScanReverseNotEqual(
			string InValue,
			int InBx,
			char[] InPatternChars )
		{
			int PatLx = InPatternChars.Length ;
			for( int Ix = InBx ; Ix > 0 ; --Ix )
			{
				char ch1 = InValue[Ix] ;
				bool IsEqual = false ;
				for( int Jx = 0 ; Jx < PatLx ; ++Jx )
				{
					if ( ch1 == InPatternChars[Jx] )
					{
						IsEqual = true ;
						break ;
					}
				}
				if ( IsEqual == false )
					return( new ScanCharResults( ch1, Ix )) ;
			}
			return( new ScanCharResults( -1 )) ;
		}

    // --------------------------- ScanReverseNotEqual-------------------------------
    // scan reverse for the first location in string not equal to all of the 
    // pattern characters.
    public static int ScanReverseNotEqual(
      string InString, int InBx, int InEx,
      int InIx, ScanPatterns InNotEqualPatterns )
    {
      int foundIx = InIx + 1;
      while (true)
      {
        foundIx -= 1;
        if (foundIx < InBx)
        {
          foundIx = -1;
          break;
        }

        int remLx = InEx - foundIx + 1;
        char ch1 = InString[foundIx];

        // the current char is not equal any of the pattern lead chars.
        int patIx = Array.IndexOf<char>(InNotEqualPatterns.LeadChars, ch1);
        if (patIx == -1)
          break;

        // lead char matches. check the entire pattern string for equality. 
        ScanPattern equalPat = null;
        ScanPattern pat = InNotEqualPatterns.ScanPatternsArray[patIx];
        while (pat != null)
        {
          bool rv = Stringer.CompareSubstringEqual(InString, foundIx, InEx, pat.PatternValue);
          if (rv == true)
          {
            equalPat = pat;
            break;
          }
          pat = pat.NextSameLeadChar;
        }

        // none of the patterns fully match the substring at the current location. 
        if (equalPat == null)
          break;
      }

      return foundIx;
    }

	} // end class Scanner
} // end namespace AutoCoder.Text
