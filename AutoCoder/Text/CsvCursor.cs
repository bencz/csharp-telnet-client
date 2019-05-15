using System;
using System.Collections.Generic;
using System.Text;
using AutoCoder.Core;
using AutoCoder.Scan;
using AutoCoder.Core.Enums;

namespace AutoCoder.Text
{
  public class CsvCursor
  {
    TextTraits mTraits = null ;
    WordCursor mCursor = null;
    string mCsvString = null;

    public CsvCursor()
    {
      mTraits = new TextTraits();
      mTraits.DividerPatterns.AddDistinct( ",", Enums.DelimClassification.DividerSymbol) ;
      mTraits.OpenNamedBracedPatterns.Replace("[", Enums.DelimClassification.OpenNamedBraced);
    }

    public CsvCursor( string InCsvString )
    {
      mTraits = new TextTraits();
      mTraits.DividerPatterns.AddDistinct(",", Enums.DelimClassification.DividerSymbol);
      mTraits.OpenNamedBracedPatterns.Replace("[", Enums.DelimClassification.OpenNamedBraced);

      mCursor = new WordCursor();
      mCursor.TextTraits = mTraits;
      mCursor.Position = RelativePosition.Begin;
      mCsvString = InCsvString;
    }

    /// <summary>
    /// the current item in the CSV string is a value ( as opposed to
    /// an empty slot ( space followed by a comma )
    /// </summary>
    public bool IsAtItemValue
    {
      get
      {
        if (mCursor.Word == null)
          return false;
        else
          return true;
      }
    }

    /// <summary>
    /// the cursor locates an item value and the value is braced.
    /// </summary>
    public bool IsBraced
    {
      get
      {
        if (mCursor.Word == null)
          return false;
        else
          return mCursor.Word.IsBraced;
      }
    }

    public bool IsEndOfString
    {
      get { return mCursor.IsEndOfString ; }
    }

    /// <summary>
    /// the cursor locates an item value and the value is quoted.
    /// </summary>
    public bool IsQuoted
    {
      get
      {
        if ( mCursor.Word == null )
          return false ;
        else
          return mCursor.Word.IsQuoted ;
      }
    }

    /// <summary>
    /// the item value without quotes or braces.
    /// </summary>
    public string ItemValue
    {
      get
      {
        if (mCursor.Word == null)
          return null;
        else if (mCursor.Word.IsBraced)
          return mCursor.Word.BracedText;
        else if (mCursor.Word.IsQuoted)
          return mCursor.Word.DequotedValue;
        else
          return mCursor.Word.Value;
      }
    }

    public WordCursor ItemCursor
    {
      get { return mCursor; }
    }

    public void NextValue()
    {
      if (mCursor.Position == RelativePosition.Begin)
        mCursor = Scanner.ScanFirstWord(mCsvString, mTraits);
      else
        mCursor = Scanner.ScanNextWord(mCsvString, mCursor);
    }

  }

}
