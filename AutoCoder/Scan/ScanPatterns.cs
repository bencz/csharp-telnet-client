using System;
using System.Collections.Generic;
using System.Text;
using AutoCoder.Text;
using AutoCoder.Collections;
using AutoCoder.Text.Enums;
using System.Collections.ObjectModel;

namespace AutoCoder.Scan
{
  public delegate void delScanPatternsChanged( ScanPatterns InPatterns ) ;

  // ScanPatterns is a class which enables relatively efficient scanning
  // for any of a list of pattern strings.
  //
  public class ScanPatterns : List<ScanPattern>
  {
    char[] mLeadChars;
    string[] mStringArray;
    StringBuilder mAllStringCombo;

    public ScanPatterns()
    {
      mAllStringCombo = new StringBuilder();
      mStringArray = null;
      mLeadChars = null;
      _ScanPatternsArray = null;
    }

    public ScanPatterns(string[] Patterns, DelimClassification DelimClassification)
      :this( )
    {
      foreach (string pat in Patterns)
      {
        Add(pat, DelimClassification);
      }
    }

    public ScanPatterns(char[] Patterns, DelimClassification DelimClassification)
      : this( )
    {
      foreach (char pat in Patterns)
      {
        Add(pat.ToString( ), DelimClassification);
      }
    }

    public ScanPatterns(string Pattern1, DelimClassification DelimClassification)
      : this( )
    {
      Add(Pattern1, DelimClassification);
    }

    public ScanPatterns(
      string Pattern1, string Pattern2, DelimClassification DelimClassification)
      : this( )
    {
      Add(Pattern1, DelimClassification);
      Add(Pattern2, DelimClassification);
    }

    public ScanPatterns( params ScanPatterns[] Patterns)
      : this( )
    {
      foreach (var pattern in Patterns)
      {
        this.AddDistinct(pattern) ;
      }
    }

    public static ScanPatterns operator +(ScanPatterns InValue1, ScanPatterns InValue2)
    {
      ScanPatterns rv = new ScanPatterns(InValue1);
      rv.AddDistinct(InValue2);
      return rv;
    }

    public char[] LeadChars
    {
      get
      {
        if (mLeadChars == null)
        {
          mLeadChars = new char[ScanPatternsArray.Length];
          for( int ix = 0 ; ix < mLeadChars.Length ; ++ix )
          {
            mLeadChars[ix] = ScanPatternsArray[ix].LeadChar ;
          }
        }
        return mLeadChars;
      }
    }

    public ScanPattern[] ScanPatternsArray
    {
      get
      {
        if (_ScanPatternsArray == null)
        {
          _ScanPatternsArray = new ScanPattern[this.Count];
          int ix = 0;
          foreach (var item in this)
          {
            _ScanPatternsArray[ix] = item;
            ix += 1;
          }
        }
        return _ScanPatternsArray ;
      }
    }
    ScanPattern[] _ScanPatternsArray;

    public void Add(
      string InPattern1, string InPattern2, DelimClassification DelimClassification)
    {
      Add(InPattern1, DelimClassification);
      Add(InPattern2, DelimClassification);
    }

    public void Add(string[] InPatternStrings, DelimClassification DelimClassification)
    {
      foreach (string pat in InPatternStrings)
      {
        Add(pat, DelimClassification);
      }
    }

    public void Add(char[] InPatternChars, DelimClassification DelimClassification)
    {
      foreach (char patChar in InPatternChars)
      {
        Add(patChar.ToString( ), DelimClassification);
      }
    }

    public void Add(char[] InPatternChars, DelimClassification DelimClassification,
      DelimClassification OverlapClassification )
    {
      foreach (char patChar in InPatternChars)
      {
        Add(patChar.ToString(), DelimClassification, OverlapClassification);
      }
    }

    /// <summary>
    /// Add pattern to list of patterns.  Return zero based position of the added
    /// pattern in the pattern list.
    /// </summary>
    /// <param name="InPatternValue"></param>
    /// <returns></returns>
    public int Add(
      string PatternValue, DelimClassification DelimClassification,
      int Priority = 0 )
    {
      ScanPattern sp = new ScanPattern(PatternValue, DelimClassification);
      sp.Priority = Priority;
      var rv = Add_Common(sp);
      return rv;
    }

    public int Add(
      string PatternValue, DelimClassification DelimClassification,
      DelimClassification OverlapClassification,
      int Priority = 0)
    {
      ScanPattern sp = 
        new ScanPattern(PatternValue, DelimClassification, OverlapClassification);
      sp.Priority = Priority;
      var rv = Add_Common(sp);
      return rv;
    }

    private int Add_Common(ScanPattern Pat )
    {
      AddToNextSameLeadCharChain(Pat);
      base.Add(Pat);
      Pat.AddedToPatternList = true;

      // add to combo string. this string is used as a quick way to determine if 
      // a new string being added is distinct or not.
      mAllStringCombo.Append('\u0001' + Pat.PatternValue + '\u0002');

      // force these two arrays to be rebuilt next time accessed thru their
      // property getters.
      mLeadChars = null;
      _ScanPatternsArray = null;
      mStringArray = null;

      // mark the position of the pattern in the list of patterns.
      Pat.ArrayPosition = base.Count - 1;

      return Pat.ArrayPosition;
    }

    public new void Add(ScanPattern Pattern)
    {
      // pattern has already been added to a ScanPatterns collection.
      if (Pattern.AddedToPatternList == true)
        throw new ApplicationException(
          "ScanPattern already added to ScanPatterns collection");

      AddToNextSameLeadCharChain(Pattern);
      base.Add(Pattern);
      Pattern.AddedToPatternList = true;

      // add to combo string. this string is used as a quick way to determine if 
      // a new string being added is distinct or not.
      mAllStringCombo.Append('\u0001' + Pattern.PatternValue + '\u0002');

      // force these two arrays to be rebuilt next time accessed thru their
      // property getters.
      mLeadChars = null;
      _ScanPatternsArray = null;
      mStringArray = null;

      // mark the position of the pattern in the list of patterns.
      Pattern.ArrayPosition = base.Count - 1;
    }

    public void Add(ScanPatterns Patterns)
    {
      foreach (var pattern in Patterns)
      {
        this.Add(pattern.Duplicate( ));
      }
    }

    public void AddDistinct(ScanPattern Pattern)
    {
      Add(Pattern);
      return;
#if skip
      int fx = mAllStringCombo.ToString().IndexOf('\u0001' + Pattern.PatternValue + '\u0002');
      if (fx == -1)
      {
        Add(Pattern);
      }
#endif 
    }

    /// <summary>
    /// Add the pattern string if string does not exist in the collection.
    /// </summary>
    /// <param name="InPatternValue"></param>
    /// <returns></returns>
    public int AddDistinct(string PatternValue, DelimClassification DelimClassification)
    {
      int pxx = Add(PatternValue, DelimClassification);
      return pxx;
#if skip
      int fx = mAllStringCombo.ToString().IndexOf('\u0001' + PatternValue + '\u0002');
      if ( fx == -1 )
      {
        int px = Add(PatternValue, DelimClassification);
        return px;
      }
      else
        return -1;
#endif
    }

    public void AddDistinct(string[] InPatterns, DelimClassification DelimClassification)
    {
      foreach (string pat in InPatterns)
      {
        Add(pat, DelimClassification);
        continue;

//        AddDistinct(pat, DelimClassification);
      }
    }

    public void AddDistinct(ScanPatterns Patterns)
    {
      foreach (var pat in Patterns)
      {
        Add(pat.Duplicate());
        continue;
      }
    }

    private void AddToNextSameLeadCharChain(ScanPattern NewPat)
    {
      ScanPattern pat = FindFirstPattern(NewPat.PatternValue);
      if (pat != null)
      {

        // step thru the trail of patterns with the same lead char.
        // step until the last pattern in the trail.
        while (pat.NextSameLeadChar != null)
        {
          pat = pat.NextSameLeadChar;
        }

        // point the pattern at the end of the trail to this new pattern.
        pat.NextSameLeadChar = NewPat;
      }
    }

    public new void Clear()
    {
      base.Clear();
      mAllStringCombo.Length = 0;

      mLeadChars = null;
      _ScanPatternsArray = null;
      mStringArray = null;

#if skip
      // signal "PatternsChanged" event.
      if (ScanPatternsChanged != null)
        ScanPatternsChanged(this);
#endif

    }

    public bool Contains(string InPatternString)
    {
      bool doesContain = false;
      if (Array.IndexOf<string>(StringArray, InPatternString) != -1)
        doesContain = true;
      return doesContain;
    }

    public bool Contains(char InChar)
    {
      bool doesContain = false;
      if (Array.IndexOf<char>(LeadChars, InChar) != -1)
        doesContain = true;
      return doesContain;
    }

    public new bool Contains(ScanPattern Pattern)
    {
      int fx = Array.IndexOf<char>(this.LeadChars, Pattern.LeadChar);
      if (fx == -1)
        return false;
      else
      {
        foreach (var pat in this)
        {
          if (pat.PatternValue == Pattern.PatternValue)
            return true;
        }
        return false;
      }
    }

    public bool ContainsAll(string[] InPatternStrings)
    {
      bool containsAll = false;
      if (Arrayer.ContainsAll<string>(
        this.StringArray, InPatternStrings) == true)
      {
        containsAll = true;
      }
      return containsAll;
    }

    public bool ContainsAll(ScanPatterns InPatterns)
    {
      bool containsAll = ContainsAll(InPatterns.StringArray);
      return containsAll;
    }

    /// <summary>
    /// find the first pattern in this collection of patterns with a first char which
    /// matches the first char of the search pattern.
    /// </summary>
    /// <param name="InPatternValue"></param>
    /// <returns></returns>
    private ScanPattern FindFirstPattern(string SearchPattern)
    {
      // first char of the search pattern.
      char searchLeadChar = SearchPattern[0];

      ScanPattern foundPat = null;
      foreach (ScanPattern pat in this)
      {
        if (pat.LeadChar == searchLeadChar)
        {
          foundPat = pat;
          break;
        }
      }
      return foundPat;
    }

    /// <summary>
    /// match the first pattern in this collection of patterns which matches the
    /// text and ends at the specified Ex.
    /// </summary>
    /// <param name="Text"></param>
    /// <param name="Ex"></param>
    /// <returns></returns>
    public Tuple<ScanPattern,int> MatchFirstPatternEndsAtStringLocation(
      string Text, int Ex)
    {
      int foundBx = -1;
      ScanPattern foundPat = null;

      foreach (var pat in this)
      {
        int bx = Ex - pat.MinPatternLength + 1;
        if (bx >= 0)
        {
          var rv = pat.EqualsText(Text, bx);
          bool isMatch = rv.Item1;
          int matchLx = rv.Item2;
          if (isMatch == true)
          {
            foundPat = pat;
            foundBx = bx;
            break;
          }
        }
      }

      return new Tuple<ScanPattern, int>(foundPat, foundBx);
    }

    public Tuple<ScanPattern, int, PatternScanResults> MatchPatternToSubstring(
      string Text, int Bx, int Lx)
    {
      ScanPattern foundPat = null;
      PatternScanResults nonWord = new PatternScanResults();
      char ch1 = Text[Bx];
      int foundMatchLx = 0;

      // find first pattern that matches lead char of the substring.
      int patIx = Array.IndexOf<char>(this.LeadChars, ch1);
      if (patIx >= 0)
      {

        ScanPattern pat = this.ScanPatternsArray[patIx];
        while (pat != null)
        {

          // use virtual method to compare if the pattern matches the text in the string.
          var rv = pat.EqualsText(Text, Bx);
          bool isMatch = rv.Item1;
          int matchLx = rv.Item2;

          if ((isMatch == true) && (matchLx < Lx))
          {
            isMatch = false;
          }

          if (isMatch == true)
          {
            nonWord.AddFound(pat, Bx, matchLx);
            foundPat = pat;
            foundMatchLx = matchLx;
            break;
          }
          pat = pat.NextSameLeadChar;
        }
      }

      return new
        Tuple<ScanPattern, int, PatternScanResults>(foundPat, foundMatchLx, nonWord);
    }

    /// <summary>
    /// Search this list of ScanPatterns for all the patterns that match the substring
    /// at the specified position in the string.
    /// </summary>
    /// <param name="InString"></param>
    /// <param name="InStringIx"></param>
    /// <param name="InStringEx"></param>
    /// <returns></returns>
    public Tuple<ScanPattern,int,List<MatchScanPattern>, PatternScanResults> 
      MatchPatternsAtStringLocation(string Text, int Ix, int Ex)
    {
      ScanPattern foundPat = null;
      char ch1 = Text[Ix];
      int foundMatchLx = 0;
      List<MatchScanPattern> foundPats = null;
      PatternScanResults nonWord = new PatternScanResults();

      // find first pattern that matches lead char of the substring.
      int patIx = Array.IndexOf<char>(this.LeadChars, ch1);
      if (patIx >= 0)
      {

        // loop thru the ScanPatterns with a lead char that matches the first char
        // of the text being scanned.
        ScanPattern pat = this.ScanPatternsArray[patIx];
        while (pat != null)
        {
          // use virtual method to compare if the pattern matches the text in the string.
          var rv = pat.EqualsText(Text, Ix);
          bool isMatch = rv.Item1;
          int matchLx = rv.Item2;

          if (isMatch == true)
          {
            nonWord.AddFound(pat, Ix, matchLx);

            // store the first found pattern.
            if ((foundPat == null) && (foundPats == null))
            {
              foundPat = pat;
              foundMatchLx = matchLx;
            }

              // for now, skip any match with length < existing match.
            else if (matchLx < foundMatchLx)
            {
            }

              // length of this match exceeds what is already found.
            // Replace all found ( shorter ) matches with this match.
            else if (matchLx > foundMatchLx)
            {
              foundPats = null;
              foundPat = pat;
              foundMatchLx = matchLx;
            }

            // add the found pattern to the list of found patterns.
            else
            {

              // create the list. add the initial found pattern to the list.
              if (foundPats == null)
              {
                foundPats = new List<MatchScanPattern>();
                MatchScanPattern matchPat = new MatchScanPattern(foundPat, Ix, foundMatchLx);
                foundPats.Add(matchPat);
              }

              {
                MatchScanPattern matchPat = new MatchScanPattern(pat, Ix, matchLx);
                foundPats.Add(matchPat);
              }
            }
          }
          pat = pat.NextSameLeadChar;
        }
      }

      // the found patterns

      return new 
        Tuple<ScanPattern, int, List<MatchScanPattern>,PatternScanResults>(
        foundPat, foundMatchLx, foundPats, nonWord);
    }

    public Tuple<ScanPattern,int> FindPatternAtSubstring(
      BoundedString InString, int InStringIx)
    {
      ScanPattern foundPat = null;
      int foundMatchLx = 0;
      if (InString.IsOutsideBounds(InStringIx) == false)
      {
        var rv = MatchPatternsAtStringLocation(InString.String, InStringIx, InString.Ex);
        foundPat = rv.Item1;
        foundMatchLx = rv.Item2;
      }
      return new Tuple<ScanPattern,int>(foundPat,foundMatchLx);
    }

    public bool IsEmpty()
    {
      if (base.Count == 0)
        return true;
      else
        return false;
    }

    public ScanPattern MatchAt(string Text, int Index)
    {
      ScanPattern foundPat = null;

      if (Index != -1)
      {
        char ch1 = Text[Index];
        char ch2 = ' ';
        int remLx = Text.Length - Index;
        if (remLx > 1)
          ch2 = Text[Index + 1];

        // find first pattern that matches lead char of the substring.
        int patIx = Array.IndexOf<char>(this.LeadChars, ch1);
        if (patIx >= 0)
        {
          var pat = this[patIx];

          foundPat = null;
          while (pat != null )
          {
            if ((foundPat != null) && (foundPat.Length >= pat.Length))
              continue;

            else if ((pat.Length <= remLx) 
              && (Text.Substring(Index, pat.Length) == pat.PatternValue))
            {
              foundPat = pat;
            }
            pat = pat.NextSameLeadChar;
          }

        }
      }

      return foundPat;
    }

    public bool MatchesPattern(char InChar)
    {
      bool matches = false;
      if (Array.IndexOf<char>(LeadChars, InChar) != -1)
        matches = true;
      return matches;
    }

    public bool MatchesPattern(string InPatternString)
    {
      bool doesMatch = false;
      if (Array.IndexOf<string>(StringArray, InPatternString) != -1)
        doesMatch = true;
      return doesMatch;
    }

    public void Replace(
      string Pattern1, string Pattern2, DelimClassification DelimClassification)
    {
      Clear();
      Add(Pattern1, DelimClassification);
      Add(Pattern2, DelimClassification);
    }

    public void Replace(string[] PatternStrings, DelimClassification DelimClassification)
    {
      Clear();
      Add(PatternStrings, DelimClassification);
    }

    /// <summary>
    /// Add pattern to list of patterns.  Return zero based position of the added
    /// pattern in the pattern list.
    /// </summary>
    /// <param name="InPatternValue"></param>
    /// <returns></returns>
    public int Replace(string PatternValue, DelimClassification DelimClassification)
    {
      Clear();
      int pos = Add(PatternValue, DelimClassification);
      return pos;
    }

    public void Replace(ScanPatterns Patterns)
    {
      Clear();
      Add(Patterns);
    }

    public string[] StringArray
    {
      get
      {
        if (mStringArray == null)
        {
          mStringArray = new string[ScanPatternsArray.Length];
          for (int ix = 0; ix < mStringArray.Length; ++ix)
          {
            mStringArray[ix] = ScanPatternsArray[ix].PatternValue;
          }
        }
        return mStringArray;
      }
    }

#region predefined, static patterns

    public static ScanPatterns NewLinePatterns
    {
      get
      {
        ScanPatterns pats = new ScanPatterns();
        pats.Add(new ScanPattern(Environment.NewLine, DelimClassification.NewLine));
        pats.Add(new ScanPattern("\n", DelimClassification.NewLine));
        pats.Add(new ScanPattern("\r", DelimClassification.NewLine));

        return pats;
      }
    }

    public static ScanPatterns WhitespacePatterns
    {
      get
      {
        ScanPatterns pats = new ScanPatterns();
        pats.Add(new ScanPattern(" ", DelimClassification.Whitespace));
        pats.Add(new ScanPattern("\t", DelimClassification.Whitespace));

        return pats;
      }
    }

    public static ScanPatterns DividerPatterns
    {
      get
      {
        ScanPatterns pats = new ScanPatterns();
        pats.Add(new ScanPattern(",", DelimClassification.DividerSymbol));
        pats.Add(new ScanPattern(":", DelimClassification.DividerSymbol));

        return pats;
      }
    }

    public static ScanPatterns EndStmtPatterns
    {
      get
      {
        ScanPatterns pats = new ScanPatterns();
        pats.Add(new ScanPattern(";", DelimClassification.EndStmt));

        return pats;
      }
    }

#endregion

  }
}
