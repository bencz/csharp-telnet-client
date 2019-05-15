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
  public class ScanPatterns : ObservableCollection<ScanPattern>
  {
    char[] mLeadChars;
    string[] mStringArray;
    ScanPattern[] mScanPatternsArray;
//    List<ScanPattern> mScanPatternsList;
    StringBuilder mAllStringCombo;

    public event delScanPatternsChanged ScanPatternsChanged;

    public ScanPatterns()
    {
      ConstructCommon();
    }

    public ScanPatterns(string[] Patterns, DelimClassification DelimClassification)
    {
      ConstructCommon();
      foreach (string pat in Patterns)
      {
        Add(pat, DelimClassification);
      }
    }

    public ScanPatterns(char[] Patterns, DelimClassification DelimClassification)
    {
      ConstructCommon();
      foreach (char pat in Patterns)
      {
        Add(pat.ToString( ), DelimClassification);
      }
    }

    public ScanPatterns(string Pattern1, DelimClassification DelimClassification)
    {
      ConstructCommon();
      Add(Pattern1, DelimClassification);
    }

    public ScanPatterns(
      string Pattern1, string Pattern2, DelimClassification DelimClassification)
    {
      ConstructCommon();
      Add(Pattern1, DelimClassification);
      Add(Pattern2, DelimClassification);
    }

    public ScanPatterns( params ScanPatterns[] Patterns)
    {
      ConstructCommon();
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
        ScanPattern[] rv = new ScanPattern[this.Count] ;
        int ix = 0 ;
        foreach( var item in this)
        {
          rv[ix] = item ;
          ix += 1 ;
        }
        return rv ;
      }
    }

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

    /// <summary>
    /// Add pattern to list of patterns.  Return zero based position of the added
    /// pattern in the pattern list.
    /// </summary>
    /// <param name="InPatternValue"></param>
    /// <returns></returns>
    public int Add(string PatternValue, DelimClassification DelimClassification )
    {
      ScanPattern sp = new ScanPattern(PatternValue, DelimClassification);
      AddToNextSameLeadCharChain(sp);
      base.Add(sp);

      // add to combo string. this string is used as a quick way to determine if 
      // a new string being added is distinct or not.
      mAllStringCombo.Append(PatternValue + '\0');

      // force these two arrays to be rebuilt next time accessed thru their
      // property getters.
      mLeadChars = null;
      mScanPatternsArray = null;
      mStringArray = null;

      // signal "PatternsChanged" event.
      if (ScanPatternsChanged != null)
        ScanPatternsChanged(this);

      // mark the position of the pattern in the list of patterns.
      sp.ArrayPosition = base.Count - 1;

      return sp.ArrayPosition;
    }

    public new void Add(ScanPattern Pattern)
    {

#if skip
      this.Add(Pattern.PatternValue, Pattern.DelimClassification);

      ScanPattern sp = new ScanPattern(PatternValue, DelimClassification);
#endif

      AddToNextSameLeadCharChain(Pattern);
      base.Add(Pattern);

      // add to combo string. this string is used as a quick way to determine if 
      // a new string being added is distinct or not.
      mAllStringCombo.Append(Pattern.PatternValue + '\0');

      // force these two arrays to be rebuilt next time accessed thru their
      // property getters.
      mLeadChars = null;
      mScanPatternsArray = null;
      mStringArray = null;

      // signal "PatternsChanged" event.
      if (ScanPatternsChanged != null)
        ScanPatternsChanged(this);

      // mark the position of the pattern in the list of patterns.
      Pattern.ArrayPosition = base.Count - 1;
    }

    public void Add(ScanPatterns Patterns)
    {
      foreach (var pattern in Patterns)
      {
        this.Add(pattern);
      }
    }

    public void AddDistinct(ScanPattern Pattern)
    {
      int fx = mAllStringCombo.ToString().IndexOf(Pattern.PatternValue + '\0');
      if (fx == -1)
      {
        Add(Pattern);
      }
    }

    /// <summary>
    /// Add the pattern string if string does not exist in the collection.
    /// </summary>
    /// <param name="InPatternValue"></param>
    /// <returns></returns>
    public int AddDistinct(string PatternValue, DelimClassification DelimClassification)
    {
      int fx = mAllStringCombo.ToString().IndexOf(PatternValue + '\0');
      if ( fx == -1 )
      {
        int px = Add(PatternValue, DelimClassification);
        return px;
      }
      else
        return -1;
    }

    public void AddDistinct(string[] InPatterns, DelimClassification DelimClassification)
    {
      foreach (string pat in InPatterns)
      {
        AddDistinct(pat, DelimClassification);
      }
    }

    public void AddDistinct(ScanPatterns Patterns)
    {
      foreach (var pat in Patterns)
      {
        AddDistinct(pat);
      }
    }

    private void AddToNextSameLeadCharChain(ScanPattern InNew)
    {
      ScanPattern pat = FindFirstPattern(InNew.PatternValue);
      if (pat != null)
      {
        while (pat.NextSameLeadChar != null)
        {
          pat = pat.NextSameLeadChar;
        }

        pat.NextSameLeadChar = InNew;
      }
    }

    public new void Clear()
    {
      base.Clear();
      mAllStringCombo.Length = 0;

      mLeadChars = null;
      mScanPatternsArray = null;
      mStringArray = null;

      // signal "PatternsChanged" event.
      if (ScanPatternsChanged != null)
        ScanPatternsChanged(this);
    }

    void ConstructCommon()
    {
      mAllStringCombo = new StringBuilder();
      mStringArray = null;
      mLeadChars = null;
      mScanPatternsArray = null;
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

    private ScanPattern FindFirstPattern(string InPatternValue)
    {
      ScanPattern foundPat = null;
      foreach (ScanPattern pat in this)
      {
        if (pat.PatternValue == InPatternValue)
        {
          foundPat = pat;
          break;
        }
      }
      return foundPat;
    }

    /// <summary>
    /// Find the pattern that matches the substring at the spcfd position
    /// in the string.
    /// </summary>
    /// <param name="InString"></param>
    /// <param name="InStringIx"></param>
    /// <param name="InStringEx"></param>
    /// <returns></returns>
    public ScanPattern FindPatternAtSubstring(
      string InString, int InStringIx, int InStringEx)
    {
      ScanPattern foundPat = null;
      char ch1 = InString[InStringIx];

      // find first pattern that matches lead char of the substring.
      int patIx = Array.IndexOf<char>(this.LeadChars, ch1);
      if (patIx >= 0)
      {

        ScanPattern pat = this.ScanPatternsArray[patIx];
        while (pat != null)
        {
//          bool rv = ScanPattern.Evaluate(
          bool rv = Stringer.CompareSubstringEqual(
            InString, InStringIx, InStringEx, pat.PatternValue);
          if (rv == true)
          {
            if (foundPat == null)
              foundPat = pat;

            // Matching pattern already found, but this pattern also matches and it is
            // longer. Always return the longer pattern.
            else if (pat.PatternValue.Length > foundPat.PatternValue.Length)
              foundPat = pat;
          }
          pat = pat.NextSameLeadChar;
        }
      }

      return foundPat;
    }

    public ScanPattern FindPatternAtSubstring(
      BoundedString InString, int InStringIx)
    {
      ScanPattern foundPat = null;
      if (InString.IsOutsideBounds(InStringIx) == false)
      {
        foundPat = FindPatternAtSubstring(
          InString.String, InStringIx, InString.Ex);
      }
      return foundPat;
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
        int remLx = Text.Length - Index;

        // find first pattern that matches lead char of the substring.
        int patIx = Array.IndexOf<char>(this.LeadChars, ch1);
        if (patIx >= 0)
        {
          foreach (var pat in this)
          {
            if (pat.LeadChar == ch1)
            {
              // fix this. once pattern is found, have to keep looking. the pattern found
              // is the longest pattern.
              if ((pat.Length <= remLx) && (Text.Substring(Index, pat.Length) == pat.PatternValue))
              {
                foundPat = pat;
                break;
              }
            }
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
