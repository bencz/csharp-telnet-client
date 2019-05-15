using System;
using System.Collections.Generic;
using System.Text;
using AutoCoder.Text.Enums;
using AutoCoder.Ext.System.Text;

namespace AutoCoder.Text
{
  /// <summary>
  /// An isolated and classified word in a text string
  /// </summary>
  public class TextWord
  {
    string mValue;
    WordClassification mClass;
    TextTraits mTraits = null;
    char? _BraceChar = null;
    LiteralType _LiteralType = LiteralType.none;

    public TextWord(string InValue, TextTraits InTraits)
    {
      CharObjectPair results = Stringer.CalcWordClassification(InValue, InTraits);
      CommonConstruct(
        InValue, (WordClassification)results.b, InTraits, results.a);
    }

    public TextWord(string Value, WordClassification WordClass, TextTraits Traits)
    {
      char? braceChar = null;
      
      if (WordClass.IsOpenBraced())
      {
        braceChar = Value[0] ;
        if (Traits.IsOpenBraceChar(braceChar.Value) == false)
          throw new ApplicationException("not an open brace char");
      }

      CommonConstruct(Value, WordClass, Traits, braceChar);
    }

    public TextWord(string Text, LiteralType LiteralType, TextTraits Traits)
    {
      mValue = Text;
      _LiteralType = LiteralType;
      
      if (this.LiteralType.IsNumeric( ))
        mClass = WordClassification.Numeric;
      else if (this.LiteralType.IsQuoted( ))
        mClass = WordClassification.Quoted;
      else
        throw new ApplicationException(
          "LiteralType is not supported. " + LiteralType.ToString());

      mTraits = Traits;
      _BraceChar = null;
    }

    public TextWord(
      string InValue, WordClassification InWordClass, TextTraits InTraits, 
      char? BraceChar)
    {
      CommonConstruct(InValue, InWordClass, InTraits, BraceChar);
    }

    public char BraceChar
    {
      get
      {
        if ((mClass == WordClassification.ContentBraced)
          || (mClass == WordClassification.NamedBraced)
          || (mClass == WordClassification.OpenNamedBraced )
          || (mClass == WordClassification.OpenContentBraced))
          return _BraceChar.Value;
        else
          throw (new ApplicationException("Get BraceChar failed. Not a braced word."));
      }
      set { _BraceChar = value; }
    }

    /// <summary>
    /// The BraceName is the word immed prior to the enclosing braces of the
    /// NameBraced word.  ex: the xxx in the word xxx( yyy, zzz )
    /// </summary>
    public string BraceName
    {
      get
      {
        if (mClass == WordClassification.NamedBraced)
        {
          int Ix = mValue.IndexOf(BraceChar);
          return mValue.Substring(0, Ix);
        }
        else if (mClass == WordClassification.OpenNamedBraced)
        {
          return mValue;
        }
        else
          throw (new ApplicationException(
            "word is not name braced. BraceName property is not available."));
      }
    }

    /// <summary>
    /// the string contained within the braces of the braced value.
    /// </summary>
    public string BracedText
    {
      get
      {
        int bx = BracedTextBx;
        if (bx == -1)
          return "";
        else
        {
          string text = mValue.Substring(bx, BracedTextLx);
          return text;
        }
      }
    }

    /// <summary>
    /// begin pos of the braced text rltv to start of braced word.
    /// </summary>
    public int BracedTextBx
    {
      get
      {
        int bx = 0 ;
        if (mClass == WordClassification.ContentBraced)
        {
          bx = 1;
        }
        else if (mClass == WordClassification.NamedBraced)
        {
          bx = mValue.IndexOf(BraceChar) + 1;
        }
        else if ((mClass == WordClassification.OpenContentBraced)
          || (mClass == WordClassification.OpenNamedBraced))
          throw new ApplicationException(
            "open braced word does not contain braced text. " +
            "( the braced text follows in the next textword. )");
        else
          throw (new ApplicationException("Get BracedValue failed. Not a braced word."));

        // advance past whitespace
        int valueEx = mValue.Length - 1 ;
        bx = Scanner.ScanNotEqual(
          mValue, bx, valueEx, mTraits.WhitespacePatterns ).FoundPos ;
        return bx;
      }
    }

    /// <summary>
    /// length of the text within the braces of the braced value.
    /// </summary>
    public int BracedTextLx
    {
      get
      {
        int bx = BracedTextBx;
        if (bx == -1)
          return -1;
        else
        {
          int ex = mValue.Length - 2;
          ex = Scanner.ScanReverseNotEqual(
            mValue, 0, mValue.Length - 1, ex, mTraits.WhitespacePatterns);
          int lx = ex - bx + 1;
          return lx;
        }
      }
    }

    public WordClassification Class
    {
      get { return mClass; }
      set { mClass = value; }
    }

    private void CommonConstruct(
      string InValue, WordClassification InClass, TextTraits InTraits, char? BraceChar)
    {
      mValue = InValue;
      mClass = InClass;
      mTraits = InTraits;
      _BraceChar = BraceChar;

      // depending on word class, brace char must be specified or be blanks.
      if ((mClass == WordClassification.ContentBraced)
        || (mClass == WordClassification.NamedBraced)
        || (mClass == WordClassification.OpenContentBraced)
        || (mClass == WordClassification.OpenNamedBraced))
      {
        if (_BraceChar == null)
          throw (new ApplicationException("Brace character of braced word is missing"));
      }
    }

    // have to decide if the Dequote should be done here or should require the use
    // of Stringer.Dequote method.  Want a property that will get the simple, atomic
    // value of the word. if the word is an element with sub members, should not be
    // returning a mixed value:
    //       SimpleValue        // string form of the simple ( no sub elements ) value.
    //       NonQuotedSimpleValue   // 
    // -------------------------- DequotedWord ------------------------------
    // return the word value, its enclosing quotes removed.
    // will throw an ApplicationException if the word is not quoted.
    public string DequotedWord
    {
      get
      {
        if (mClass == WordClassification.Quoted)
          return Stringer.Dequote(mValue, mTraits.QuoteEncapsulation);
        else if (mClass == WordClassification.Identifier)
          return mValue;
        else
          throw new ApplicationException("word value is not quotable");
      }
    }

    /// <summary>
    /// The string value of the word without the enclosing quotes.
    /// To simplify, this should be the property to use instead of DequotedWord
    /// and NonQuotedWord.
    /// </summary>
    public string DequotedValue
    {
      get
      {
        if (mClass == WordClassification.Quoted)
          return Stringer.Dequote(mValue, mTraits.QuoteEncapsulation);
        else
          throw new ApplicationException("word value is not quoted");
      }
    }

    public bool IsBraced
    {
      get
      {
        if ((mClass == WordClassification.ContentBraced)
          || (mClass == WordClassification.NamedBraced)
          || (mClass == WordClassification.OpenNamedBraced)
          || (mClass == WordClassification.OpenContentBraced))
          return true;
        else
          return false;
      }
    }

    /// <summary>
    /// the value of the word is in quoted string form.
    /// </summary>
    public bool IsQuoted
    {
      get
      {
        if (mClass == WordClassification.Quoted)
          return true;
        else
          return false;
      }
    }

    /// <summary>
    /// the sub classification of word class Quoted. String, char, VerbatimString.
    /// </summary>
    public LiteralType LiteralType
    {
      get { return _LiteralType; }
      set { _LiteralType = value; }
    }

    /// <summary>
    /// return the word value.  if quoted, dequote it. otherwise, return as is.
    /// </summary>
    public string NonQuotedWordx
    {
      get
      {
        if (IsQuoted == true)
          return (DequotedWord);
        else
          return ToString();
      }
    }

    /// <summary>
    /// string form of the simple ( no sub members ) WordValue.
    /// If the value is quoted, dequote it.
    /// </summary>
    public string NonQuotedSimpleValue
    {
      get
      {
        if (mClass == WordClassification.Quoted)
          return Stringer.Dequote(mValue, mTraits.QuoteEncapsulation);
        else
          return SimpleValue;
      }
    }

    /// <summary>
    /// The quote char that encloses the quoted word. 
    /// </summary>
    public char QuoteChar
    {
      get
      {
        if (mClass == WordClassification.Quoted)
          return mValue[0];
        else
          throw (new ApplicationException("Get QuoteChar failed. Not a quoted word."));
      }
    }

    /// <summary>
    /// The value in string form of the simple ( no sub values ) WordValue
    /// </summary>
    public string SimpleValue
    {
      get
      {
        switch (mClass)
        {
          case WordClassification.Identifier:
            return mValue;
          case WordClassification.Numeric:
            return mValue;
          case WordClassification.Quoted:
            return mValue;
          case WordClassification.Delimeter:
            return null;
          default:
            throw new ApplicationException("WordValue value is not simple");
        }
      }
    }

    /// <summary>
    /// The the front to end, delim excluded, string value of the word.
    /// The SimpleValue property and ToString method should be eliminated and this
    /// property used in their place. Use the BraceName and BracedText properties
    /// to access components of a mixed format name.
    /// </summary>
    public string Value
    {
      get { return mValue; }
    }


    // ----------------------- SetValue ---------------------------------
    /// <summary>
    /// Set the value of the word and its position in the string.  
    /// </summary>
    /// <param name="InWord"></param>
    /// <param name="InWordBx"></param>
    /// <returns></returns>
    private void SetValue(
      string InValue, WordClassification InClass, char? BraceChar)
    {
      mValue = InValue;
      mClass = InClass;
      _BraceChar = null;

      // brace character.
      if ((mClass == WordClassification.ContentBraced)
        || (mClass == WordClassification.OpenContentBraced))
        _BraceChar = mValue[0];
      else if ((InClass == WordClassification.NamedBraced)
        || (mClass == WordClassification.OpenNamedBraced))
      {
        if (BraceChar == null)
          throw (new ApplicationException("brace char of NameBraced word is missing"));
        _BraceChar = BraceChar;
      }
    }

    /// <summary>
    /// The text value of the word.
    /// </summary>
    /// <returns></returns>
    public override string ToString()
    {
      StringBuilder sb = new StringBuilder();
      sb.Append("TextWord.");
      sb.SentenceAppend(mClass.ToString());
      if (_LiteralType != Enums.LiteralType.none)
        sb.SentenceAppend(_LiteralType.ToString());
      sb.SentenceAppend(mValue);
      return sb.ToString();
    }
  } // end class WordValue

}
