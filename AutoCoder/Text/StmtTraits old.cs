using System;
using System.Collections.Generic;
using System.Text;
using AutoCoder ;
using AutoCoder.Text ;
using AutoCoder.Scan;
using AutoCoder.Collections;
using AutoCoder.Text.Enums;

namespace AutoCoder.Text
{
  /// <summary>
  /// The traits that govern the parsing of a statement string.
  /// </summary>
  public class StmtTraits : TextTraits 
  {
    string mFunctionOpenBracedChar = null ;
    string mFunctionCloseBracedChar = null ;
    string[] mSentenceDelimStrings = null;
    string[] mSentenceWordDelimStrings = new string[] { " ", "\t", Environment.NewLine };

    ScanPatterns mSentenceOpenBracedPatterns = 
      new ScanPatterns("{", Enums.DelimClassification.OpenContentBraced);
    ScanPatterns mComboOpenContentBracedPatterns = null;
    ScanPatterns mSentenceCloseBracedPatterns = null ;

    string[] mSentencePartDelimStrings = new string[] { ":" };

    bool mCommandCapable = false;

    public StmtTraits()
      : base()
    {
      Construct_Common();
    }

    public StmtTraits(TextTraits InTextTraits)
      : base(InTextTraits)
    {
      Construct_Common();
    }

    /// <summary>
    /// statement can contain Command type statements  
    /// </summary>
    public bool CommandCapable
    {
      get { return mCommandCapable; }
      set { mCommandCapable = value; }
    }

    public string FunctionOpenBraceChar
    {
      get
      {
        if (mFunctionOpenBracedChar == null)
          mFunctionOpenBracedChar = "(";
        return mFunctionOpenBracedChar;
      }
      set
      { 
        mFunctionOpenBracedChar = value;
        mFunctionCloseBracedChar = null ;
      }
    }

    public string FunctionCloseBraceChar
    {
      get
      {
        if (mFunctionCloseBracedChar == null)
          mFunctionCloseBracedChar =
            AcCommon.CalcCloseBraceChar(FunctionOpenBraceChar[0]).ToString();
        return mFunctionCloseBracedChar;
      }
    }

    public bool HasSentenceDelimStrings
    {
      get
      {
        if (SentenceDelimStrings.Length == 0)
          return false;
        else
          return true;
      }
    }

    /// <summary>
    /// override of the OpenContentBracedPatterns property designed to include
    /// SentenceOpenBracedPatterns in the contents of the base TextTraits class 
    /// OpenContentBracedPatterns property.
    /// </summary>
    public override ScanPatterns OpenContentBracedPatterns
    {
      get
      {
        if ( mComboOpenContentBracedPatterns == null )
        {
          base.OpenContentBracedPatterns.AddDistinct( SentenceOpenBracedPatterns ) ;
        }
        return base.OpenContentBracedPatterns ;
      }
    }

    public ScanPatterns SentenceCloseBracedPatterns
    {
      get
      {
        if (mSentenceCloseBracedPatterns == null)
        {
          char[] cbChars = 
            Stringer.GetCorrCloseBraceChars(SentenceOpenBracedPatterns.LeadChars);
          mSentenceCloseBracedPatterns = 
            new ScanPatterns( cbChars, Enums.DelimClassification.CloseBraced );
        }
        return mSentenceCloseBracedPatterns;
      }
    }

    public string[] SentenceDelimStrings
    {
      get
      {
        if (mSentenceDelimStrings == null)
        {
          mSentenceDelimStrings = Arrayer.Concat<string>(
            SentencePartDelimStrings,
            SentenceWordDelimStrings,
            SentenceOpenBracedPatterns.StringArray) ;
        }
        return mSentenceDelimStrings;
      }
    }

    public ScanPatterns SentenceOpenBracedPatterns
    {
      get
      {
        return mSentenceOpenBracedPatterns;
      }
    }

    public string[] SentencePartDelimStrings
    {
      get
      { 
        return mSentencePartDelimStrings ;
      }
      set
      {
        mSentencePartDelimStrings = value;

        // add to DelimStrings.
        if ((Arrayer.IsEmpty<string>(mSentencePartDelimStrings) == false)
          && ( DividerPatterns.ContainsAll(mSentencePartDelimStrings) == false))
        {
          DividerPatterns.AddDistinct( 
            mSentencePartDelimStrings, Enums.DelimClassification.DividerSymbol);
        }

        mSentenceDelimStrings = null;
      }
    }

    public string[] SentenceWordDelimStrings
    {
      get
      {
        return mSentenceWordDelimStrings;
      }
      set
      {
        mSentenceWordDelimStrings = value;
        mSentenceDelimStrings = null;

        // add the sentence word delims to DelimStrings.
        DividerPatterns.AddDistinct(mSentenceWordDelimStrings, Enums.DelimClassification.DividerSymbol);

      }
    }

    private void Construct_Common()
    {
      mSentenceOpenBracedPatterns.ScanPatternsChanged += 
        new delScanPatternsChanged(SentenceOpenBracedPatterns_ScanPatternsChanged);

      if (ExpressionPatterns.IsEmpty())
        ExpressionPatterns.Add(
        new char[] { '<', '>', '&', '|', '+', '-', '/', '*', '!', '=' },
         DelimClassification.ExpressionSymbol );

      if (EndStmtPatterns.IsEmpty( ))
        this.EndStmtPatterns.Add(";", DelimClassification.EndStmt);

      if (NewLinePatterns.IsEmpty())
        NewLinePatterns.Add(Environment.NewLine, DelimClassification.NewLine);

      // default sentence delim chars are whitespace and the ":".
      // ( the ":" delimits sentence parts. the whitespace delimits sentence words. )
    }

    /// <summary>
    /// disable the classification of stmts as sentences. Will be command
    /// strings instead.
    /// </summary>
    public void EmptySentenceDelimChars()
    {
      SentenceWordDelimStrings = null;
      SentencePartDelimStrings = null;
    }

    public bool IsFunctionCloseBrace( string InValue )
    {
      if ( FunctionCloseBraceChar == InValue )
        return true ;
      else
        return false ;
    }

    public bool IsSentenceCloseBraceChar(string InDelim)
    {
      if (SentenceCloseBracedPatterns.Contains(InDelim) == true)
        return true;
      else
        return false;
    }

    public bool IsSentenceOpenBraceChar(string InDelim)
    {
      if ( SentenceOpenBracedPatterns.Contains(InDelim) == true )
        return true;
      else
        return false;
    }

    public bool IsSentenceDelim(string InDelim)
    {
      if (Array.IndexOf<string>(SentenceDelimStrings, InDelim) >= 0)
        return true;
      else
        return false;
    }

    void SentenceOpenBracedPatterns_ScanPatternsChanged(ScanPatterns InPatterns)
    {
      mComboOpenContentBracedPatterns = null;
    }

  }
}
