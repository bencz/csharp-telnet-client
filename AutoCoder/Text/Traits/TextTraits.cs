using System;
using AutoCoder.Core;
using System.Collections.Generic;
using AutoCoder.Scan;
using AutoCoder.Text.Enums;
using System.Collections.Specialized;
using System.Linq;

namespace AutoCoder.Text
{
  // ---------------------------- TextTraits --------------------------
  public class TextTraits
  {

    // In the event there are more than one nonword strings with the same starting
    // char, use the NextLookup member to access the other nonword string with 
    // the same starting char.
    ScanPatterns mWhitespacePatterns = null;
    ScanPatterns mNewLinePatterns = null;

    // the delim classifications which are categorized as whitespace.
    DelimClassification[] _WhitespaceDelimClasses =
      new DelimClassification[] 
      {
        DelimClassification.Whitespace,
        DelimClassification.NewLine,
      } ;
    
    ScanPatterns mDividerPatterns = null;
    bool _NonDividerIsWord = false;

    // the delim classifications which are categorized as a divider.
    DelimClassification[] _DividerDelimClasses = 
      new DelimClassification[] {DelimClassification.Whitespace,
      DelimClassification.NewLine, DelimClassification.DividerSymbol,
      DelimClassification.EndStmt};

    ScanPatterns mQuotePatterns = 
      new ScanPatterns(new char[] { '"', '\'' }, DelimClassification.Quote);
    VerbatimLiteralPattern _VerbatimLiteralPattern = null;

    ScanPatterns mExpressionPatterns = new ScanPatterns();

    // the ";" at end of a stmt is example of end string char.
    ScanPatterns mEndStmtPatterns = new ScanPatterns();

    // the string patterns which mark a comment that runs to the end of the line.
    ScanPatterns mCommentToEndPatterns = null;

    ScanPatterns mOpenNamedBracedPatterns = 
      new ScanPatterns(new char[] { '(', '[' }, DelimClassification.OpenNamedBraced);
    ScanPatterns mOpenContentBracedPatterns = 
      new ScanPatterns(new char[] { '{', '<' }, DelimClassification.OpenContentBraced);
    ScanPatterns mOpenBracedPatterns = null;
    ScanPatterns mCloseBracedPatterns = null;

    KeywordPatterns _KeywordPatterns = null;

    bool _VirtualWhitespace = true;

    ScanPatterns mPathSepPatterns = new ScanPatterns();
    //    ScanPatterns mPathSepPatterns = 
//      new ScanPatterns(new char[] { '/', '\\' }, DelimClassification.PathSep);

    // used when cracking a statement string into StmtWords. A sentence word is a
    // sequence of words separated by whitespace. 
    bool mFormSentencesFromWhitespaceDelimWords = false;
    
    // combo of delim, ws, brace, quote, expr chars.
    protected ScanPatterns mNonWordPatterns = null;

    QuoteEncapsulation mQem = QuoteEncapsulation.Escape  ;

    // how are braced word scanned and parsed. as a whole word or in pieces.
    ScannerBracedTreatment mBracedTreatment = ScannerBracedTreatment.Whole;

    public TextTraits()
    {
      mWhitespacePatterns = 
        new ScanPatterns(new string[] { " ", "\t" }, DelimClassification.Whitespace);
      mNewLinePatterns = new ScanPatterns();
      mDividerPatterns = new ScanPatterns(
        new string[] { ",", ":", "." }, DelimClassification.DividerSymbol);
      mCommentToEndPatterns = new ScanPatterns();

      // chars which separate names in a path.
      mPathSepPatterns.Add("/", DelimClassification.PathSep, DelimClassification.CommentToEnd);
      mPathSepPatterns.Add("\\", DelimClassification.PathSep);
    }

    public TextTraits(TextTraits Traits)
      : this( ) 
    {
      mWhitespacePatterns.Replace(Traits.mWhitespacePatterns);
      mNewLinePatterns.Replace(Traits.mNewLinePatterns);

      mDividerPatterns.Replace(Traits.mDividerPatterns);
      this.DividerDelimClasses = Traits.DividerDelimClasses; 
      mCommentToEndPatterns.Replace(Traits.mCommentToEndPatterns);
      
      mQuotePatterns.Replace(Traits.mQuotePatterns);
      this.VerbatimLiteralPattern = Traits.VerbatimLiteralPattern;
      
      mExpressionPatterns.Replace(Traits.mExpressionPatterns);
      mEndStmtPatterns.Replace(Traits.mEndStmtPatterns);
      mOpenNamedBracedPatterns.Replace(Traits.mOpenNamedBracedPatterns);
      mOpenContentBracedPatterns.Replace(Traits.mOpenContentBracedPatterns);
      mOpenBracedPatterns = null;
      mCloseBracedPatterns = null;

      mPathSepPatterns.Replace(Traits.mPathSepPatterns);

      mNonWordPatterns = null;
      mQem = Traits.mQem;
      mFormSentencesFromWhitespaceDelimWords = 
        Traits.mFormSentencesFromWhitespaceDelimWords;
    }

    // ------------------------ properties ----------------------------
    public ScannerBracedTreatment BracedTreatment
    {
      get { return mBracedTreatment; }
      set { mBracedTreatment = value; }
    }

    public ScanPatterns CloseBracedPatterns
    {
      get
      {
        if (mCloseBracedPatterns == null)
        {
          char[] cbChars = Stringer.GetCorrCloseBraceChars(OpenBracedPatterns.LeadChars);
          mCloseBracedPatterns = new ScanPatterns(cbChars, DelimClassification.CloseBraced);
        }
        return mCloseBracedPatterns;
      }
    }

    public ScanPatterns CommentToEndPatterns
    {
      get { return mCommentToEndPatterns; }
    }

    // the delim classifications which are categorized as a divider.
    public DelimClassification[] DividerDelimClasses
    {
      get { return _DividerDelimClasses ; }
      set { _DividerDelimClasses = value ; }
    }

    public bool IsDividerDelim(DelimClassification DelimClass)
    {
      if (_DividerDelimClasses == null)
        return false;
      else if (
        Array.IndexOf<DelimClassification>(this.DividerDelimClasses, DelimClass) != -1)
        return true;
      else
        return false;
    }

    virtual public ScanPatterns DividerPatterns
    {
      get { return mDividerPatterns; }
      set 
      { 
        mDividerPatterns = value;
        mNonWordPatterns = null;
      }
    }

    /// <summary>
    /// example is the ";" that marks the end of a C# language stmt.
    /// </summary>
    public ScanPatterns EndStmtPatterns
    {
      get { return mEndStmtPatterns; }
      set 
      { 
        mEndStmtPatterns = value;
        mNonWordPatterns = null;
      }
    }

    string _SpecialValueStarter;
    public string SpecialValueStarter
    {
      get { return _SpecialValueStarter; }
      set
      {
        _SpecialValueStarter = value;
        mNonWordPatterns = null;
      }
    }

    public ScanPatterns ExpressionPatterns
    {
      get { return mExpressionPatterns; }
    }

    public bool FormSentencesFromWhitespaceDelimWords
    {
      get { return mFormSentencesFromWhitespaceDelimWords; }
      set { mFormSentencesFromWhitespaceDelimWords = value; }
    }

    public KeywordPatterns KeywordPatterns
    {
      get
      {
        if (_KeywordPatterns == null)
          _KeywordPatterns = new KeywordPatterns();
        return _KeywordPatterns;
      }
      set
      {
        _KeywordPatterns = value;
      }
    }

    // consider a newline pattern to be whitespace in the statement string.
    bool? _NewLineIsWhitespace = null;
    public bool NewLineIsWhitespace
    {
      get 
      {
        if (_NewLineIsWhitespace == null)
        {
          _NewLineIsWhitespace = false;
          foreach (var item in this.WhitespacePatterns)
          {
            if (item.DelimClassification == DelimClassification.NewLine)
            {
              _NewLineIsWhitespace = true;
              break;
            }
          }
        }
        return _NewLineIsWhitespace.Value;
      }
    }

    public ScanPatterns NewLinePatterns
    {
      get { return mNewLinePatterns; }
      set 
      { 
        mNewLinePatterns = value;
        mNonWordPatterns = null;
      }
    }

    public bool NonDividerIsWord
    {
      get { return _NonDividerIsWord; }
      set { _NonDividerIsWord = value; }
    }

    // delim patterns that are non words.
    // When scanning to next word, scan for any of the delim patterns.
    // The delim patterns collection contain all the delims. Whitespace, new lines,
    // divider symbols, path seperators, open brace, ... Everything.
    // If any text preceeds the found delim pattern, the next word method returns that 
    // text as the word and the delim pattern as the delim.
    // When calculating the start position of the scan for the next word, first check if the 
    // delim pattern of the current word is a NonWordPattern.
    // If the delim pattern is a NonWordPattern, ( whitespace, divider, ... ) start the 
    // scan for next word after the end of that pattern. 
    // If the delim pattern is not a NonWordPattern, consider it a word itself, and return
    // that delim pattern as the next word.

    ScanPatterns _DelimPatternsThatAreNonWords;
    public ScanPatterns DelimPatternsThatAreNonWords
    {
      get 
      {
        return _DelimPatternsThatAreNonWords;
      }
      set
      {
        _DelimPatternsThatAreNonWords = value;
      }
    }

    ScanPatterns _DelimPatterns;
    public ScanPatterns DelimPatterns
    {
      get
      {
        if (_DelimPatterns == null)
        {
          _DelimPatterns = AssembleNonWordPatterns();
        }
        return _DelimPatterns;
      }
    }

    /// <summary>
    /// a concatenation of BraceChars, DividerPatterns and QuotePatterns.
    /// </summary>
    public virtual ScanPatterns NonWordPatterns
    {
      get
      {
        if (mNonWordPatterns == null)
        {
          mNonWordPatterns = AssembleNonWordPatterns();
        }
        return mNonWordPatterns;
      }
    }

    public ScanPatterns OpenBracedPatterns
    {
      get
      {
        if (mOpenBracedPatterns == null)
        {
          mOpenBracedPatterns = new ScanPatterns(OpenContentBracedPatterns);
          mOpenBracedPatterns.AddDistinct(OpenNamedBracedPatterns);
        }
        return mOpenBracedPatterns;
      }
    }

    public virtual ScanPatterns OpenContentBracedPatterns
    {
      get { return mOpenContentBracedPatterns ; }
    }

    public virtual ScanPatterns OpenNamedBracedPatterns
    {
      get{ return mOpenNamedBracedPatterns; }
    }

    public ScanPatterns PathSepPatterns
    {
      get { return mPathSepPatterns ; }
    }

    public ScanPatterns QuotePatterns
    {
      get { return mQuotePatterns; }
    }

    public QuoteEncapsulation QuoteEncapsulation
    {
      get { return mQem ; }
      set { mQem = value ; }
    }

    public VerbatimLiteralPattern VerbatimLiteralPattern
    {
      get { return _VerbatimLiteralPattern; }
      set
      {
        _VerbatimLiteralPattern = value;
      }
    }

    public bool VirtualWhitespace
    {
      get { return _VirtualWhitespace; }
      set { _VirtualWhitespace = value; }
    }

    public ScanPatterns WhitespacePatterns
    {
      get 
      {
        return mWhitespacePatterns;
      }
      set
      {
        mWhitespacePatterns = value;
      }
    }

    ScanPatterns _WhitespaceWithoutNewLinePatterns;
    public ScanPatterns WhitespaceWithoutNewLinePatterns
    {
      get
      {
        if (_WhitespaceWithoutNewLinePatterns == null)
        {
          _WhitespaceWithoutNewLinePatterns = new ScanPatterns();
          foreach (var pat in this.WhitespacePatterns)
          {
            if (pat.DelimClassification != DelimClassification.NewLine)
            {
              _WhitespaceWithoutNewLinePatterns.Add(pat.Duplicate());
            }
          }
        }
        return _WhitespaceWithoutNewLinePatterns;
      }
    }

    private ScanPatterns AssembleNonWordPatterns()
    {
      ScanPatterns pats = new ScanPatterns();

      pats.Add(WhitespacePatterns);
      pats.AddDistinct(NewLinePatterns);

      pats.Add(CommentToEndPatterns);
      
      pats.AddDistinct(QuotePatterns);
      if ( this.VerbatimLiteralPattern != null )
      {
        pats.AddDistinct(this.VerbatimLiteralPattern) ;
      }

      pats.AddDistinct(ExpressionPatterns);
      pats.AddDistinct(EndStmtPatterns);
      pats.AddDistinct(OpenContentBracedPatterns);
      pats.AddDistinct(OpenNamedBracedPatterns);
      pats.AddDistinct(CloseBracedPatterns);
      pats.AddDistinct(PathSepPatterns);

      // standard delimeters like "," and ":"
      pats.AddDistinct(DividerPatterns);

      // special value starter pattern.  *LIBL is the special value. * is the starter
      // value.
      if (this.SpecialValueStarter != null)
      {
        ScanPattern pat = 
          new ScanPattern(this.SpecialValueStarter, DelimClassification.SpecialValueStarter);
        pats.AddDistinct(pat);
      }

      // note. dont add keyword patterns to this collection of non word patterns.
      //       Keyword patterns are a subset of the text ( identifier ) tokens found
      //       between non word tokens. 

      // BgnTemp
//      if (_KeywordPatterns != null)
//      {
//        pats.AddDistinct(KeywordPatterns);
//      }
      // EndTemp

      // sort the list of patterns by patternValue and DelimClass.
      var sortedPats = from a in pats
                       orderby a.PatternValue, a.DelimClassification, a.UserCode
                       select a;

      // build a list of patterns that are distinct on patternValue, delimClass, and
      // keyword code.
      ScanPatterns distinctPats = new ScanPatterns();
      ScanPattern pvPat = null;
      foreach (var pat in sortedPats)
      {
        if ((pvPat == null) || (pat.Equals(pvPat) == false))
          distinctPats.Add(pat.Duplicate( ));
        pvPat = pat;
      }

      return distinctPats;
    }

    /// <summary>
    /// return the NonWord info of path sep char which is classified as being a
    /// path part delimiter ( it could be a division expression symbol )
    /// </summary>
    /// <param name="InBoundedString"></param>
    /// <param name="InIx"></param>
    /// <returns></returns>
    public ScanPattern GetPathPartDelim(BoundedString InBoundedString, int InIx)
    {
      ScanPattern pat = null;
      int matchLx = 0;

      if (InBoundedString.IsOutsideBounds(InIx))
      {
      }

      else if (IsPathSepChar(InBoundedString[InIx]) == true)
      {
        var rv = NonWordPatterns.MatchPatternsAtStringLocation(
          InBoundedString.String, InIx, InBoundedString.Ex);
        pat = rv.Item1;
        matchLx = rv.Item2;

        // the nonword is a path sep char. but is it a pathPart delim?
        // It is if there is a word char before or after the path sep char.
        // ex: /abc/efg vs. a = b / c ;  
          if ((IsWordChar(InBoundedString, InIx - 1) == false)
            && ( IsWordChar( InBoundedString, InIx + 1 ) == false ))
          {
            pat = null ;
          }
      }
        return pat ;
    }

    public ScanPattern GetPathPartDelim(string Text, int Ix)
    {
      ScanPattern pat = null;
      int matchLx = 0;

      if (Ix > (Text.Length -1))
      {
      }

      else if (IsPathSepChar(Text[Ix]) == true)
      {
        var rv = NonWordPatterns.MatchPatternsAtStringLocation(Text, Ix, Text.Length - 1);
        pat = rv.Item1;
        matchLx = rv.Item2;

        // the nonword is a path sep char. but is it a pathPart delim?
        // It is if there is a word char before or after the path sep char.
        // ex: /abc/efg vs. a = b / c ;  
        if ((IsWordChar(Text, Ix - 1) == false)
          && (IsWordChar(Text, Ix + 1) == false))
        {
          pat = null;
        }
      }
      return pat;
    }

    public bool IsCloseBraceChar(string InValue)
    {
      if (CloseBracedPatterns.Contains(InValue) == true)
        return true;
      else
        return false;
    }

    // ------------------------- IsDividerString ---------------------------
    public bool IsDividerString( string InDelim )
    {
      if (DividerPatterns.Contains(InDelim))
        return true;
      else
        return false;
    }

    public bool IsDividerPattern(ScanPattern Pattern)
    {
      return false;
    }

    public bool IsEndStmtDelim(string InDelim)
    {
      if (EndStmtPatterns.IsEmpty( ))
        return false;
      else if (EndStmtPatterns.Contains(InDelim) == true )
        return true;
      else
        return false;
    }

    public bool IsExpressionChar(char InChar)
    {
      if (ExpressionPatterns.IsEmpty( ) == true)
        return false;
      else if ( ExpressionPatterns.Contains(InChar) == true)
        return true;
      else
        return false;
    }

    public bool IsNonWordPattern(ScanPattern Pattern)
    {
      if (this.DelimPatternsThatAreNonWords.Contains(Pattern))
        return true;
      else
        return false;
    }

    // ------------------------- IsOpenBraceChar ---------------------------
    public bool IsOpenBraceChar(string InDelim)
    {
      if (OpenBracedPatterns.Contains(InDelim) == true)
        return true;
      else
        return false;
    }

    public bool IsOpenBraceChar( char InChar)
    {
      if (OpenBracedPatterns.Contains(InChar) == true)
        return true;
      else
        return false;
    }

    public bool IsOpenContentBracedChar(char InChar)
    {
      if (OpenContentBracedPatterns.Contains(InChar) == true)
        return true;
      else
        return false;
    }

    public bool IsOpenContentBracedChar(string InValue)
    {
      if (OpenContentBracedPatterns.Contains(InValue) == true)
        return true;
      else
        return false;
    }

    public bool IsOpenNamedBraceChar(char InChar)
    {
      if (OpenNamedBracedPatterns.Contains(InChar) == true)
        return true;
      else
        return false;
    }

    /// <summary>
    /// calc if the 
    /// </summary>
    /// <param name="InBoundedString"></param>
    /// <param name="InBx"></param>
    /// <returns></returns>
    public bool IsPathPartDelim(BoundedString InBoundedString, int InIx)
    {
      if (InBoundedString.IsOutsideBounds(InIx))
        return false;
      else if (IsPathSepChar(InBoundedString[InIx]) == false)
        return false;
      else if (IsWordChar(InBoundedString, InIx - 1) == true)
        return true;
      else if (IsWordChar(InBoundedString, InIx + 1) == true)
        return true;
      else
        return false ;
    }

    public bool IsPathPartDelim(string Text, int Ix)
    {
      if ((Ix < 0 ) || ( Ix > (Text.Length - 1)))
        return false;
      else if (IsPathSepChar(Text[Ix]) == false)
        return false;
      else if (IsWordChar(Text, Ix - 1) == true)
        return true;
      else if (IsWordChar(Text, Ix + 1) == true)
        return true;
      else
        return false;
    }

    public bool IsPathSepChar( char InValue)
    {
      bool rv = PathSepPatterns.MatchesPattern(InValue);
      return rv;
    }

    public bool IsPathSepChar(string InValue)
    {
      bool rv = PathSepPatterns.MatchesPattern(InValue);
      return rv;
    }

    public bool IsQuoteChar( char Char )
    {
      foreach( char ch1 in mQuotePatterns.LeadChars )
      {
        if (ch1 == Char)
          return true;
      }
      return false;
    }

    public bool IsWhitespace(ScanPattern Pattern)
    {
      if (this.WhitespacePatterns.Contains(Pattern))
        return true;
      else
        return false;
    }

    public bool IsWhitespace(string InString, int InIx, int InEx)
    {
      if (Stringer.CompareSubstringEqualAny(
        InString, InIx, InEx, WhitespacePatterns.StringArray) != -1)
        return true;
      else
        return false;
    }

    public bool IsWhitespace(
      BoundedString InBoundedString, int InIx )
    {
      return IsWhitespace(InBoundedString.String, InIx, InBoundedString.Ex);
    }

    public bool IsWhitespaceDelimClass(DelimClassification DelimClass)
    {
      if (this.WhitespaceDelimClasses == null)
        return false;
      else if (Array.IndexOf<DelimClassification>(
        this.WhitespaceDelimClasses, DelimClass) != -1)
        return true;
      else
        return false;
    }

    /// <summary>
    /// calc if the char at location in string is a word char or not ( is not a
    /// NonWordChar )
    /// </summary>
    /// <param name="InBoundedString"></param>
    /// <param name="InIx"></param>
    /// <returns></returns>
    public bool IsWordChar(
      BoundedString InBoundedString, int InIx)
    {
      bool isWordChar = false;

      if (InBoundedString.IsOutsideBounds(InIx))
        isWordChar = false;

      else
      {
        var rv = NonWordPatterns.FindPatternAtSubstring(
          InBoundedString, InIx);
        var pat = rv.Item1;
        var matchLx = rv.Item2;

        if (pat == null)
          isWordChar = true;
        else
          isWordChar = false;
      }
      return isWordChar ;
    }

    /// <summary>
    /// calc if the char at location in string is a word char or not ( is not a
    /// NonWordChar )
    /// </summary>
    /// <param name="InBoundedString"></param>
    /// <param name="InIx"></param>
    /// <returns></returns>
    public bool IsWordChar(
      string Text, int Ix)
    {
      bool isWordChar = false;

      if (Ix > (Text.Length - 1))
        isWordChar = false;

      else
      {
        var rv = NonWordPatterns.MatchPatternsAtStringLocation(Text, Ix, Text.Length - 1);
        var pat = rv.Item1;
        var matchLx = rv.Item2;
        if (pat == null)
          isWordChar = true;
        else
          isWordChar = false;
      }
      return isWordChar;
    }

    public TextTraits SetQuoteEncapsulation( QuoteEncapsulation Value )
    {
      mQem = Value ;
      return this ; 
    }

    // method called from a ScanPatterns object when the pattern is changed. Changing
    // any of the text patterns requires the consolidated "NonWordPatterns" to be
    // rebuilt on next use.
    void WhitespacePatterns_ScanPatternsChanged(ScanPatterns Patterns)
    {
      mNonWordPatterns = null;

      // Changes to OpenContent and OpenNamedBracedPatterns is signaled to this method
      // along with all the other patterns. Assume that all changes will take place at
      // startup, so repeatedly setting these dependent patterns to null will not cause
      // any unnecessary rebuilds.
      mOpenBracedPatterns = null;
      mCloseBracedPatterns = null;
    }

    public DelimClassification[] WhitespaceDelimClasses
    {
      get { return _WhitespaceDelimClasses; }
      set { _WhitespaceDelimClasses = value; }
    }

  }
}
