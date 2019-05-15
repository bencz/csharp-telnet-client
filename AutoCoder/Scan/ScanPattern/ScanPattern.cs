using System;
using System.Collections.Generic;
using System.Text;
using AutoCoder.Core;
using AutoCoder.Text;
using AutoCoder.Text.Enums;
using AutoCoder.Ext.System.Text;

namespace AutoCoder.Scan
{
  // consider strings that are delimited, not by single characters, but by patterns of
  // characters. the "->" pointer in c++.  

  // the NonWordLookup array has a side by side, entry to entry relationship
  // with LeadChars. Where the array index to LeadChar is also the index
  // to the NonWordLookup array, which contains the full non word string.

  // In the event there are more than one nonword strings with the same starting
  // char, use the NextSameLeadChar member to access the other nonword string with 
  // the same starting char.

  // KeywordPattern class is a class that uses ScanPattern as a base class.
  public class ScanPattern
  {

    public bool AddedToPatternList
    {
      get;
      set;
    }

    // the full string value of the scan pattern.
    string mPatternValue;
    public virtual string PatternValue
    {
      get
      {
        return mPatternValue;
      }
      set
      {
        mPatternValue = value;
        if (mPatternValue.Length == 0)
          throw new ApplicationException("ScanPattern string is empty");
        this.LeadChar = mPatternValue[0];
      }
    }

    public virtual int MinPatternLength
    {
      get
      {
        if (this.PatternValue == null)
          return 0;
        else
          return this.PatternValue.Length;
      }
    }

    // position of this ScanPattern within the lists maintained by the
    // ScanPatterns class.
    int mArrayPosition = -1;
    public int ArrayPosition
    {
      get { return mArrayPosition; }
      set { mArrayPosition = value; }
    }

    DelimClassification _DelimClassification;
    public DelimClassification DelimClassification
    {
      get { return _DelimClassification; }
      set { _DelimClassification = value; }
    }

    /// <summary>
    /// Overlap classification is used when multiple DelimClassifications are desired
    /// to compare the same in terms of returning the patterns that match at a 
    /// location in a string. When multiple patterns of the same classification match
    /// the same text in a string, the pattern which is longest is returned. If the 
    /// delim class is different, multipl matching ScanPatterns are returned. Use
    /// OverlapClassification to enable the longest of multiple matching patterns of
    /// different classifications to be returned.
    /// </summary>
    public Nullable<DelimClassification> OverlapClassification
    { get; set; }

    /// <summary>
    /// Compare delim class between patterns, where OverlapClassification where first
    /// OverlapClassification and the DelimClassification is used for the compare.
    /// See the Add method of MatchScanPatternList.
    /// </summary>
    public DelimClassification CompareClassification
    {
      get
      {
        if (this.OverlapClassification != null)
          return this.OverlapClassification.Value;
        else
          return this.DelimClassification;
      }
    }

    /// <summary>
    /// string that replaces the found pattern text.
    /// When a keyword has multiple words with whitespace between the word, the
    /// text that matches the pattern may contain more than one space between the
    /// words. This replacement value provides a consistent return value that 
    /// replaces what was actually found.
    /// </summary>
    public string ReplacementValue
    {
      get { return _ReplacementValue; }
      set
      {
        _ReplacementValue = value;
      }
    }
    string _ReplacementValue;

    /// <summary>
    /// an adhoc user classification code of the pattern.
    /// When there are patterns to scan for with the same PatternValue, but different
    /// UserCode, the scan method returns a match for each pattern.
    /// </summary>
    public string UserCode
    { get; set; }


    /// <summary>
    /// when more than one pattern is a match, the pattern with the highest priority
    /// is the one returned by the scan method.
    /// </summary>
    public int Priority
    { get; set; }

    #region constructors 

    private ScanPattern()
    {
      this._ReplacementValue = null;
      this.mPatternValue = null;
      this.mNextSameLeadChar = null;
      this.AddedToPatternList = false;
      this.Priority = 0;
    }

    public ScanPattern(string PatternValue, DelimClassification DelimClassification)
      : this( )
    {
      this.PatternValue = PatternValue;
      this.DelimClassification = DelimClassification;
    }

    public ScanPattern(string PatternValue, DelimClassification DelimClassification,
      Nullable<DelimClassification> OverlapClassification )
      : this()
    {
      this.PatternValue = PatternValue;
      this.DelimClassification = DelimClassification;
      this.OverlapClassification = OverlapClassification; 
    }

    public ScanPattern(ScanPattern Pattern)
      : this( )
    {
      this.ReplacementValue = Pattern.ReplacementValue;
      this.PatternValue = Pattern.PatternValue;
      this.DelimClassification = Pattern.DelimClassification;
      this.OverlapClassification = Pattern.OverlapClassification;
    }

    public virtual ScanPattern Duplicate()
    {
      ScanPattern dup = new ScanPattern(this);
      return dup;
    }
    
    #endregion

#if skip
    public ScanCompareRule CompareRule
    {
      get { return mCompareRule; }
      set { mCompareRule = value; }
    }
#endif

    public override bool Equals(object obj)
    {
      ScanPattern compareTo = obj as ScanPattern;
      if ((compareTo.DelimClassification == this.DelimClassification)
        && (compareTo.PatternValue == this.PatternValue)
        && (compareTo.UserCode == this.UserCode ))
        return true;
      else
        return false;
    }

    public virtual Tuple<bool, int> EqualsText(string Text, int Ix)
    {
      int matchLx = 0;
      bool doesMatch = false;
      int remLx = Text.Length - Ix + 1;
      int patLx = this.PatternValue.Length;
      if (remLx < patLx)
        doesMatch = false;
      else if (this.PatternValue == Text.Substring(Ix, patLx))
      {
        doesMatch = true;
        matchLx = patLx;
      }
      else
        doesMatch = false;

      return new Tuple<bool, int>(doesMatch, matchLx);
    }

    /// <summary>
    /// evaluate if this pattern matches the current location in the string.
    /// </summary>
    /// <param name="InString"></param>
    /// <param name="InBx"></param>
    /// <param name="InEx"></param>
    /// <param name="InIx"></param>
    /// <returns></returns>
    public bool Evaluate(string Text, int Bx, int Ex, int Ix)
    {
      if (this.LeadChar == Text[Ix])
        return true;
      else
      {
        bool rv = Stringer.CompareSubstringEqual(
            Text, Ix, Ex, this.PatternValue);
        return rv;
      }
    }

    public override int GetHashCode()
    {
      return base.GetHashCode();
    }

    public char LeadChar
    {
      get ;
      private set ;
    }

    // The next ScanPattern object which has the same lead char as this
    // pattern. 
    // Used in the FindInSameLeadCharChain method to return the
    // ScanPattern which correctly matches the pattern in the string
    // being scanned.
    ScanPattern mNextSameLeadChar;

    public int Length
    {
      get
      {
        if (this.PatternValue == null)
          return 0;
        else
          return this.PatternValue.Length;
      }
    }

    public bool Match(BoundedString Text, int Ix)
    {
      bool rv = false;
      if (Text.IsOutsideBounds(Ix, this.PatternValue.Length) == true)
        rv = false;
      else if (this.LeadChar != Text[Ix])
        rv = false;
      else if (this.PatternValue == Text.Substring(Ix, this.PatternValue.Length))
        rv = true;
      else
        rv = false;
      return rv;
    }

    public bool Match(string Text, int Ix)
    {
      bool rv = false;
      int ex = Ix + this.PatternValue.Length;
      if ( ex > (Text.Length - 1))
        rv = false;
      else if (this.LeadChar != Text[Ix])
        rv = false;
      else if (this.PatternValue == Text.Substring(Ix, this.PatternValue.Length))
        rv = true;
      else
        rv = false;
      return rv;
    }

    public ScanPattern NextSameLeadChar
    {
      get { return mNextSameLeadChar; }
      set 
      { 
        mNextSameLeadChar = value; 
      }
    }

    public override string ToString()
    {
      StringBuilder sb = new StringBuilder();
      sb.Append("ScanPattern");

      string patText = "";
      if (this.PatternValue == " ")
        patText = "[space]";
      else if (this.PatternValue == "\t")
        patText = "\\t";
      else if (this.PatternValue == Environment.NewLine)
        patText = "\\r\\n";
      else if (this.PatternValue == "\r")
        patText = "\\r";
      else if (this.PatternValue == "\n")
        patText = "\\n";
      else
        patText = this.PatternValue;

      sb.SentenceAppend(patText);
      sb.SentenceAppend(this.DelimClassification.ToString());

      if (this.UserCode != null)
        sb.SentenceAppend(this.UserCode);

      return sb.ToString();
    }
  }
}
