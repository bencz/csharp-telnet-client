using System;
using System.Collections.Generic;
using System.Text;
using AutoCoder.Text;
using AutoCoder.Core;
using AutoCoder.Scan;
using AutoCoder.Text.Enums;
using AutoCoder.Core.Enums;
using AutoCoder.Ext.System.Text;

namespace AutoCoder.Text
{
  public class WordCursor
  {
    public enum enumVirtualCursor { None, WhitespaceOnly } ;

    // The value of the word itself. holds other info such as the WordClassification
    // ( is it quoted, braced, numeric, ... )
    TextWord mWord;

    int mWordBx;
    int mDelimBx;
    string mDelim;
    DelimClassification mDelimClass;
		bool? _TrapFlag;
    bool mDelimIsWhitespace;

    // these properties are not yet used.
    bool mWhitespaceFollowsWord;
    bool mWhitespaceFollowsDelim;
    
    RelativePosition mRltv;
    TextTraits mTraits = null;
    string mString;  // the string the word is in.

    // A virtual value for the cursor that overrides the actual value.
    // Used when  
    enumVirtualCursor mVirtualCursor;

    // when true, Next( ) and Prev( ) will stay at same location and
    // clear the stay flag.
    bool mStayAtFlag;

    // ----------------------------- constructor ---------------------------
    public WordCursor()
    {
      EmptyWordParts();
      mTraits = null;
      mString = null;
      mStayAtFlag = false;
      mVirtualCursor = enumVirtualCursor.None;
      this.DelimClass = DelimClassification.NotAssigned;
    }

    public WordCursor(WordCursor InWord)
    {
      mWord = InWord.mWord;
      mWordBx = InWord.WordBx;
      mDelimBx = InWord.DelimBx;
      mDelim = InWord.mDelim;
      mDelimIsWhitespace = InWord.mDelimIsWhitespace;
      this.DelimClass = InWord.mDelimClass;
      mWhitespaceFollowsWord = InWord.mWhitespaceFollowsWord;
      mWhitespaceFollowsDelim = InWord.mWhitespaceFollowsDelim;
      mRltv = InWord.mRltv;
      mTraits = InWord.mTraits;
      mString = InWord.mString;
      mStayAtFlag = InWord.mStayAtFlag;
      mVirtualCursor = InWord.mVirtualCursor;
    }

    public int CursorPos
    {
      get
      {
        if (mRltv == RelativePosition.Begin)
          return 0;
        else if (mRltv == RelativePosition.At)
        {
          if (mWordBx != -1)
            return mWordBx;
          else if (mDelimBx != -1)
            return mDelimBx;
          else
            return 0;
        }
        else
          return 0;
      }
    }

    public int DelimBx
    {
      get { return mDelimBx; }
    }

    public DelimClassification DelimClass
    {
      get
      {
        if (mVirtualCursor == enumVirtualCursor.WhitespaceOnly)
          this.DelimClass = DelimClassification.Whitespace;
        if (mDelimClass == DelimClassification.NotAssigned)
        {
          if (mRltv != RelativePosition.At)
            this.DelimClass = DelimClassification.None;
          else if (DelimIsWhitespace == true)
            this.DelimClass = DelimClassification.Whitespace;
          else if (DelimValue.Length == 0)
            throw new ApplicationException("cannot calc delim class");
          else if (mTraits.IsCloseBraceChar(DelimValue))
            this.DelimClass = DelimClassification.CloseBraced;
          else
            this.DelimClass = DelimClassification.DividerSymbol;
        }
        return mDelimClass;
      }
      set
			{
				mDelimClass = value; 
			}
    }

    public int DelimEx
    {
      get
      {
        if (mDelim == null)
          return -1;
        else
          return (mDelim.Length + mDelimBx - 1);
      }
    }

    public bool DelimIsAssignmentSymbol
    {
      get
      {
        if (DelimClass != DelimClassification.ExpressionSymbol)
          return false;
        else if (DelimValue.Length != 1)
          return false;
        else if (DelimValue == "=")
          return true;
        else
          return false;
      }
    }

    public bool DelimIsCloseBrace
    {
      get
      {
        if (DelimClass == DelimClassification.CloseBraced)
          return true;
        else
          return false;
      }
    }

    public bool DelimIsOpenBrace
    {
      get
      {
        if (DelimClass == DelimClassification.OpenContentBraced)
          return true;
        else if (DelimClass == DelimClassification.OpenNamedBraced)
          return true;
        else
          return false;
      }
    }

    public bool DelimIsWhitespace
    {
      get
      {
        if (mVirtualCursor == enumVirtualCursor.WhitespaceOnly)
          return true;
        else if (mDelimClass == DelimClassification.Whitespace)
          return true;
        else if (mDelimClass == DelimClassification.VirtualWhitespace)
          return true;
        else
          return mDelimIsWhitespace;
      }
      set { mDelimIsWhitespace = value; }
    }

    public bool DelimIsExpressionSymbol
    {
      get
      {
        if (DelimClass == DelimClassification.ExpressionSymbol)
          return true;
        else
          return false;
      }
    }

    public bool DelimIsPathPart
    {
      get
      {
        if (mVirtualCursor == enumVirtualCursor.WhitespaceOnly)
          return false;
        else if (mWhitespaceFollowsDelim == true)
          return false;
        else if (mTraits.IsPathSepChar(DelimValue) == true)
          return true;
        else
          return false;
      }
    }

    public string DelimValue
    {
      get
      {
        if (mVirtualCursor == enumVirtualCursor.WhitespaceOnly)
          return " ";
        else if (mDelimClass == DelimClassification.VirtualWhitespace)
          return " ";
        else if (mDelim == null)
          return "";
        else
          return mDelim;
      }
      set { mDelim = value; }
    }

    /// <summary>
    /// Word cursor is positioned at a word.
    /// </summary>
    public bool IsAtWord
    {
      get
      {
        if ((mRltv == RelativePosition.At) && ( mWord != null ))
          return true;
        else
          return false;
      }
    }

    /// <summary>
    /// Cursor has advanced to a delimiter without a preceeding word value.
    /// </summary>
    public bool IsDelimOnly
    {
      get
      {
        if (mVirtualCursor == enumVirtualCursor.WhitespaceOnly)
          return true;
        else if ((mWord == null) && (mDelimBx != -1))
          return true;
        else
          return false;
      }
    }

    // ------------------------- AssignDelimPart ---------------------------
    // set the delim properies of the cursor to the corr values in another
    // cursor.
    public void AssignDelimPart(WordCursor InWord)
    {
      mDelimBx = InWord.DelimBx;
      mDelim = InWord.mDelim;
      mDelimIsWhitespace = InWord.mDelimIsWhitespace;
      this.DelimClass = InWord.mDelimClass;
      mWhitespaceFollowsWord = InWord.mWhitespaceFollowsWord;
      mWhitespaceFollowsDelim = InWord.mWhitespaceFollowsDelim;
    }

    public string DelimToString()
    {
      string s1 = null;
      s1 = "DelimValue( " + DelimValue + " )" +
        " DelimClass( " + DelimClass.ToString() + " )";
      return s1;
    }

    /// <summary>
    /// Empty the word and delim parts of the cursor.
    /// </summary>
    public void EmptyWordParts()
    {
      mWord = null;
      mWordBx = -1;
      mDelimBx = -1;
      mDelim = null;
      mRltv = RelativePosition.None;
      mDelimIsWhitespace = false;
      this.DelimClass = DelimClassification.NotAssigned;
      mWhitespaceFollowsWord = false;
      mWhitespaceFollowsDelim = false;
      mVirtualCursor = enumVirtualCursor.None;
      mStayAtFlag = false;
    }

    // ----------------------- IsEmpty -----------------------
    private bool IsEmpty
    {
      get
      {
        if (mVirtualCursor == enumVirtualCursor.WhitespaceOnly)
          return false;
        else if ((mWordBx == -1) && (mDelimBx == -1))
          return true;
        else
          return false;
      }
    }

    /// <summary>
    /// Word cursor is at end of string. No word, no delimiter.
    /// </summary>
    public bool IsEndOfString
    {
      get
      {
        if (mVirtualCursor == enumVirtualCursor.WhitespaceOnly)
          return false;
        else if (mRltv == RelativePosition.End)
          return true;
        else
          return false;
      }
    }

    // word is part of a path when the delim is a PathSepChar ( see
    // TextTraits )
    public bool IsPathPart
    {
      get
      {
        if (mVirtualCursor == enumVirtualCursor.WhitespaceOnly)
          return false;
        else if (mWhitespaceFollowsWord == true)
          return false;
        else if (mTraits.IsPathSepChar(DelimValue) == true)
          return true;
        else
          return false;
      }
    }

    /// <summary>
    /// scan to the word in string after this one.
    /// </summary>
    /// <returns></returns>
    public WordCursor NextWord()
    {
      if (mString == null)
        throw (new ApplicationException(
          "Scan next word exception. String is not assigned."));
      return (Scanner.ScanNextWord(mString, this));
    }

    public char OpenBracedChar
    {
      get
      {
        if ((mWord == null) || ( mVirtualCursor == enumVirtualCursor.WhitespaceOnly ))
          throw new ApplicationException("OpenBraceChar not available. Not a braced word.");
        else return mWord.BraceChar;
      }
    }

    public int OpenBracedIx
    {
      get
      {
        if ((mWord != null)
          && ( mVirtualCursor != enumVirtualCursor.WhitespaceOnly )          
          && ((mWord.Class == WordClassification.OpenContentBraced)
          || (mWord.Class == WordClassification.OpenNamedBraced)))
          return mDelimBx;
        else
          throw new ApplicationException("not an OpenBraced word");
      }
    }

    // -------------------- RelativePostion -------------------------
    // use when calc the start position when scanning to next word.
    public RelativePosition Position
    {
      get
      {
        if (mVirtualCursor == enumVirtualCursor.WhitespaceOnly)
          return RelativePosition.At;
        else
          return mRltv;
      }
      set { mRltv = value; }
    }

    // ---------------------- ScanBx -------------------------
    // Begin position of the scan. either the bgn pos of the word or delim. 
    // Whichever is present.
    public int ScanBx
    {
      get
      {
        if (IsEmpty == true)
          return 0;
        else if (mVirtualCursor == enumVirtualCursor.WhitespaceOnly)
          return DelimBx;
        else if (WordBx != -1)
          return WordBx;
        else if (DelimBx >= 0)
          return DelimBx;
        else
          return 0;
      }
    }

    // ---------------------- ScanEx -------------------------
    // End position of the scan. either the end pos of the word or delim. 
    // Whichever is greater.
    public int ScanEx
    {
      get
      {
        if (IsEmpty == true)
          return -1;
        else if (mVirtualCursor == enumVirtualCursor.WhitespaceOnly)
          return DelimEx;
        else if (mDelimClass == DelimClassification.VirtualWhitespace)
          return WordEx;
        else if (DelimEx >= 0)
          return DelimEx;
        else if (Word == null)
          return mWordBx;
        else
          return WordEx;
      }
    }

    /// <summary>
    /// Stay at current location on next advance of cursor.
    /// </summary>
    public bool StayAtFlag
    {
      get { return mStayAtFlag; }
      set { mStayAtFlag = value; }
    }

    // -------------------------- Traits ----------------------------
    public TextTraits TextTraits
    {
      get
      {
        if (mTraits == null)
          mTraits = new TextTraits();
        return mTraits;
      }
      set { mTraits = value; }
    }

		public bool? TrapFlag
		{
			get { return _TrapFlag; }
			set { _TrapFlag = value; }
		}

    public int ValueBx
    {
      get
      {
        if (IsEmpty == true)
          return -1;
        else if (IsDelimOnly == true)
          return mDelimBx;
        else
          return mWordBx;
      }
    }

    public int ValueEx
    {
      get
      {
        if (IsEmpty == true)
          return -1;
        else if (IsDelimOnly == true)
          return DelimEx;
        else
          return WordEx;
      }
    }

    public enumVirtualCursor VirtualCursor
    {
      get { return mVirtualCursor; }
      set { mVirtualCursor = value; }
    }

    public bool WhitespaceContainsNewLine
    {
      get 
      {
        if (mDelimClass != DelimClassification.Whitespace)
          throw new ApplicationException("word delim is not whitespace");
        var spr = Scanner.ScanEqualAny(DelimValue, mTraits.NewLinePatterns);
        if (spr.IsFound == true)
          return true;
        else
          return false;
      }
    }

    public bool WhitespaceFollowsDelim
    {
      get { return mWhitespaceFollowsDelim; }
      set { mWhitespaceFollowsDelim = value; }
    }

    public bool WhitespaceFollowsWord
    {
      get { return mWhitespaceFollowsWord; }
      set { mWhitespaceFollowsWord = value; }
    }

    // ------------------------------ WordBx ---------------------------
    /// <summary>
    /// Begin position of word located by this cursor.
    /// </summary>
    public int WordBx
    {
      get { return mWordBx; }
      set { mWordBx = value; }
    }

    // ----------------------------- WordClassification -----------------------
    public WordClassification WordClassification
    {
      get
      {
        if (mWord == null)
          return WordClassification.None;
        else
          return mWord.Class;
      }
    }

    // ------------------------------- WordEx -------------------------------
    public int WordEx
    {
      get
      {
        if (mWordBx == -1)
          return -1;
        else
          return (mWordBx + Word.ToString().Length - 1);
      }
    }

    /// <summary>
    /// this word is an open brace word.
    /// </summary>
    public bool WordIsOpenBrace
    {
      get
      {
        if (WordClassification == WordClassification.OpenContentBraced)
          return true;
        else if (WordClassification == WordClassification.OpenNamedBraced)
          return true;
        else
          return false;
      }
    }

    public bool WordIsDelim
    {
      get
      {
        if (mWord == null)
          return false;
        else if (this.WordEx == this.DelimEx)
          return true;
        else
          return false;
      }
    }

    public int WordLx
    {
      get
      {
        if (IsDelimOnly == true)
          return 0;
        else
          return Word.ToString().Length;
      }
    }

    // ------------------------------ Word -----------------------------
    public TextWord Word
    {
      get { return mWord ; }
    }

    // ----------------------------------- SetDelim ------------------------------------
    public void SetDelim(
      BoundedString InBoundedString,
      ScanPatternResults InScanResults, DelimClassification InDelimClass )
    {
      if (InScanResults.IsNotFound == true)
      {
        mDelim = null;
        mDelimBx = -1;
        this.DelimClass = DelimClassification.EndOfString;
      }
      else
      {
        SetDelim(
          InBoundedString, InScanResults.FoundPat.PatternValue, InScanResults.FoundPos, 
          InDelimClass);
      }
    }

    // ----------------------------------- SetDelim ------------------------------------
    public void SetDelim(
      string Text,
      ScanPatternResults ScanResults, DelimClassification DelimClass)
    {
      if (ScanResults.IsNotFound == true)
      {
        mDelim = null;
        mDelimBx = -1;
        this.DelimClass = DelimClassification.EndOfString;
      }
      else
      {
        SetDelim(
          Text, ScanResults.FoundPat.PatternValue, ScanResults.FoundPos,
          DelimClass);
      }
    }

    // ------------------------------ SetDelim ---------------------------------
    public WordCursor SetDelim(
      BoundedString InBoundedString, 
      string InDelim, int InDelimBx, DelimClassification InDelimClass )
    {
      mDelim = InDelim;
      mDelimBx = InDelimBx;
      this.DelimClass = InDelimClass;

      // check if whitespace after the delim.
      if ((InDelim != null) 
        && (InDelimClass != DelimClassification.Whitespace)
        && (InDelimClass != DelimClassification.EndOfString))
      {
        int Nx = InDelimBx + InDelim.Length;
        if ((Nx <= InBoundedString.Ex)
          && (mTraits.IsWhitespace(InBoundedString, Nx)))
          mWhitespaceFollowsDelim = true;
      }

      return this;
    }

    // ------------------------------ SetDelim ---------------------------------
    public WordCursor SetDelim(
      string Text,
      string Delim, int DelimBx, DelimClassification DelimClass)
    {
      mDelim = Delim;
      mDelimBx = DelimBx;
      this.DelimClass = DelimClass;

      // check if whitespace after the delim.
      if ((Delim != null)
        && (DelimClass != DelimClassification.Whitespace)
        && (DelimClass != DelimClassification.EndOfString))
      {
        int Nx = DelimBx + Delim.Length;
        if ((Nx <= (Text.Length - 1))
          && (mTraits.IsWhitespace(Text, Nx, Text.Length - 1)))
          mWhitespaceFollowsDelim = true;
      }

      return this;
    }

    /// <summary>
    /// The word is delimiter only.
    /// </summary>
    /// <returns></returns>
    public WordCursor SetNullWord()
    {
      mWord = null;
      mWordBx = -1;
      this.Position = RelativePosition.At;
      return this;
    }

    // ------------------------- SetString ---------------------------------
    // ( this property should probably be removed. in the event the word spans
    //   multiple lines in a text file ( a braced word ), this property would be
    //   incomplete. )
    public WordCursor SetString(string InString)
    {
      mString = InString;
      return this;
    }

    // ----------------------- SetTraits ---------------------------------
    public WordCursor SetTraits(TextTraits InTraits)
    {
      mTraits = InTraits;
      return this;
    }

    public WordCursor SetVirtualCursor_WhitespaceOnly(int InDelimIx)
    {
      VirtualCursor = enumVirtualCursor.WhitespaceOnly;
      mDelimBx = InDelimIx;
      return this;
    }

    // ----------------------- SetWord ---------------------------------
    /// <summary>
    /// Set the value of the word and its position in the string.  
    /// </summary>
    /// <param name="InWord"></param>
    /// <param name="InWordBx"></param>
    /// <returns></returns>
    public WordCursor SetWord(string InWord, WordClassification InClass, int InWordBx)
    {
      return (SetWord(InWord, InClass, InWordBx, ' '));
    }

    // ----------------------- SetWord ---------------------------------
    /// <summary>
    /// Set the value of the word and its position in the string.  
    /// </summary>
    /// <param name="InWord"></param>
    /// <param name="InWordBx"></param>
    /// <returns></returns>
    public WordCursor SetWord(
      string InWordText, WordClassification InWordClass, int InWordBx, char InBraceChar)
    {
      mWord = new TextWord(InWordText, InWordClass, mTraits, InBraceChar);
      mWordBx = InWordBx;
      this.Position = RelativePosition.At;
      return this;
    }

    /// <summary>
    /// throw an ApplicationException if the WordCursor is not positioned at a word.
    /// </summary>
    public void Throw_NotPositionAt()
    {
      if (this.Position != RelativePosition.At)
        throw (new ApplicationException("word cursor not positioned at a word"));
    }

    public string ToDelimPresentationString()
    {
      StringBuilder sb = new StringBuilder();
      sb.Append("dBx: " + DelimBx.ToString());
      sb.Append(" dc: " + DelimClass.ToString());

      // delim value
      switch (DelimClass)
      {
        case DelimClassification.EndOfString:
          break;

        case DelimClassification.Whitespace:
          if ((this.TextTraits.NewLineIsWhitespace == true)
            && ( this.TextTraits.NewLinePatterns.Contains( this.DelimValue )))
            sb.Append(" dv: " + "CRLF");
          break;

        case DelimClassification.DividerSymbol:
          string s1 = this.DelimValue;
          if (this.DelimValue.IndexOf(",") != -1)
            s1 = "COMMA";
          else if ( this.DelimValue.IndexOf(";") != -1)
            s1 = "SEMICOLON" ;
          sb.Append(" dv: " + s1);
          break;

        default:
          sb.Append(" dv: " + this.DelimValue);
          break;
      }
      
      return sb.ToString();
    }

    public string ToPresentationString()
    {
      StringBuilder sb = new StringBuilder();
      sb.Append("dBx: " + DelimBx.ToString());
      sb.Append(" dc: " + DelimClass.ToString());
      if ((DelimClass != DelimClassification.Whitespace)
        && ( DelimClass != DelimClassification.EndOfString ))
        sb.Append(" dv: " + this.DelimValue);
      sb.Append(" " + ToValuePresentationString());
      return sb.ToString();
    }

    public override string ToString()
    {
      StringBuilder sb = new StringBuilder();
      sb.Append(this.WordClassification.ToString());
      sb.SentenceAppend(this.DelimClass.ToString());
      if ( this.Word != null )
        sb.SentenceAppend(this.Word.Value);
      return sb.ToString();
    }

    public string ToValuePresentationString()
    {
      string s1 = null;
      if (WordClassification == WordClassification.None)
        s1 = WordClassification.ToString();
      else
        s1 =
          " wc: " + WordClassification.ToString() +
          " wBx: " + WordBx.ToString( ) +
          " wv: " + Word.ToString();
      return s1;
    }

  } // end class WordCursor
}
