using System;
using System.Collections.Generic;
using System.Text;
using AutoCoder.Core;
using AutoCoder.Text;
using AutoCoder.Text.Enums;

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
  public class ScanPattern
  {

    // the full string value of the scan pattern.
    string mPatternValue;
    
    char mLeadChar;

    // The next ScanPattern object which has the same lead char as this
    // pattern. 
    // Used in the FindInSameLeadCharChain method to return the
    // ScanPattern which correctly matches the pattern in the string
    // being scanned.
    ScanPattern mNextSameLeadChar;

    // position of this ScanPattern within the lists maintained by the
    // ScanPatterns class.
    int mArrayPosition = -1;

    // compare rule to evaluate for when scanning for this pattern.
    ScanCompareRule mCompareRule = ScanCompareRule.Equal;

    DelimClassification _DelimClassification;

    #region constructors 

#if skip
    public ScanPattern()
    {
      mPatternValue = null;
      mNextSameLeadChar = null;
      mLeadChar = '\0' ;
      this.DelimClassification = Text.Enums.DelimClassification.None;
    }
#endif

    public ScanPattern(string PatternValue, DelimClassification DelimClassification)
    {
      this.PatternValue = PatternValue;
      mNextSameLeadChar = null;
      this.DelimClassification = DelimClassification;
    }
    
    #endregion

    public int ArrayPosition
    {
      get { return mArrayPosition; }
      set { mArrayPosition = value; }
    }

    public ScanCompareRule CompareRule
    {
      get { return mCompareRule; }
      set { mCompareRule = value; }
    }

    public DelimClassification DelimClassification
    {
      get { return _DelimClassification; }
      set { _DelimClassification = value; }
    }

    public char LeadChar
    {
      get
      {
        return mLeadChar;
      }
    }

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

    public string PatternValue
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
        mLeadChar = mPatternValue[0];
      }
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
      if (mLeadChar == Text[Ix])
        return true;
      else
      {
        bool rv = Stringer.CompareSubstringEqual(
            Text, Ix, Ex, this.PatternValue);
        return rv;
      }
    }

    public bool Match(BoundedString Text, int Ix)
    {
      bool rv = false;
      if (Text.IsOutsideBounds(Ix, this.PatternValue.Length) == true)
        rv = false;
      else if (mLeadChar != Text[Ix])
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
      else if (mLeadChar != Text[Ix])
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
      set { mNextSameLeadChar = value; }
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
      return sb.ToString();
    }
  }
}
