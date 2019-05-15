using System;
using System.Collections.Generic;
using System.Text;

#if skip

namespace AutoCoder.Text
{
  public partial class Scanner
  {

    public class ScanNonWordResults
    {
      string mNonWordString;
      Nullable<char> mNonWordChar;
      int mPosition;
      DelimClassification mDelimClass;

      public ScanNonWordResults( 
        int InPosition, char InNonWordChar, DelimClassification InDelimClass )
      {
        mPosition = InPosition;
        mNonWordChar = InNonWordChar;
        mNonWordString = null;
        mDelimClass = InDelimClass;
      }

      public ScanNonWordResults(
        int InPosition, string InNonWordString, DelimClassification InDelimClass )
      {
        mPosition = InPosition;
        mNonWordChar = null;
        mNonWordString = InNonWordString;
        mDelimClass = InDelimClass;
      }

      public ScanNonWordResults(ScanCharResults InScanChar)
      {
        mPosition = InScanChar.ResultPos;
        
        if (InScanChar.IsNotFound == true)
          mNonWordChar = null;
        else
          mNonWordChar = InScanChar.ResultChar;

        mDelimClass = DelimClassification.None;
        mNonWordString = null;
      }

      public ScanNonWordResults(int InPosition)
      {
        if (InPosition != -1)
          throw new ApplicationException("only not found value, -1, allowed in this constructor");
        mPosition = InPosition;
        mNonWordChar = null;
        mNonWordString = null;
        mDelimClass = DelimClassification.None;
      }

      public Nullable<char> NonWordChar
      {
        get { return mNonWordChar; }
        set { mNonWordChar = value; }
      }

      public string NonWordString
      {
        get
        {
          if (mNonWordString != null)
            return mNonWordString;
          else if (mNonWordChar != null)
            return mNonWordChar.Value.ToString();
          else
            return null;
        }
      }

      public int Position
      {
        get { return mPosition; }
      }

      public int ResultPos
      {
        get { return mPosition; }
      }

      public DelimClassification DelimClass
      {
        get { return mDelimClass; }
      }

      public bool IsNotFound
      {
        get
        {
          if (mPosition == -1)
            return true;
          else
            return false;
        }
      }

      public void Apply(ScannerNonWord InNonWord)
      {
        if (InNonWord.IsNonWordChar == true)
        {
          mNonWordChar = InNonWord.NonWordChar;
          mNonWordString = null;
        }
        else
        {
          mNonWordChar = null;
          mNonWordString = InNonWord.NonWordString;
        }
        mDelimClass = InNonWord.DelimClass;
      }
    }






    public class ScanNonWordxResults
    {
      ScannerNonWord mNonWord;
      ScanCharResults mCharResults;
      TextTraits mTextTraits;

      public ScanNonWordxResults(
        TextTraits InTextTraits, ScannerNonWord InNonWord, ScanCharResults InCharResults)
      {
        mTextTraits = InTextTraits;
        mNonWord = InNonWord;
        mCharResults = InCharResults;
      }

      public ScanNonWordxResults( TextTraits InTextTraits )
      {
        mTextTraits = InTextTraits;
        mNonWord = null;
        mCharResults = new ScanCharResults();
      }

      public ScanCharResults CharResults
      {
        get { return mCharResults; }
        set { mCharResults = value; }
      }

      public DelimClassification DelimClass
      {
        get
        {
          if (mNonWord == null)
            return DelimClassification.None;
          else
            return mNonWord.DelimClass;
        }
      }

      public bool IsNotFound
      {
        get
        {
          if (mCharResults.ResultPos == -1)
            return true;
          else
            return false;
        }
      }

      public ScannerNonWord NonWord
      {
        get { return mNonWord; }
        set { mNonWord = value; }
      }

      public int ResultPos
      {
        get { return mCharResults.ResultPos; }
      }

      public TextTraits TextTraits
      {
        get { return mTextTraits; }
      }
    }
  }
}

#endif

