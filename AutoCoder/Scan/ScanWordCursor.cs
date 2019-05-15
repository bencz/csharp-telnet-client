using System;
using System.Collections.Generic;
using System.Text;
using AutoCoder.Text;
using AutoCoder.Core;
using AutoCoder.Scan;
using AutoCoder.Text.Enums;
using AutoCoder.Core.Enums;
using AutoCoder.Text.Location;
using AutoCoder.Ext.System.Text;

namespace AutoCoder.Scan
{
  public class ScanWordCursor
  {
    
    #region public get/set properties

    // The value of the word itself. holds other info such as the WordClassification
    // ( is it quoted, braced, numeric, ... )
    TextWord _Word;
    public TextWord Word
    {
      get { return _Word ; }
      set { _Word = value ; }
    }

    TextLocation _WordBx;
    public TextLocation WordBx
    {
      get { return _WordBx; }
      set { _WordBx = value; }
    }

    RelativePosition _Position;
    public RelativePosition Position
    {
      get { return _Position ; }
      set { _Position = value ; }
    }

    TextLocation _DelimBx ;
    public TextLocation DelimBx 
    {
      get { return _DelimBx ; }
      set { _DelimBx = value ; }
    }

    ScanPattern _DelimPattern ;
    public ScanPattern DelimPattern
    {
      get { return _DelimPattern ; }
      set { _DelimPattern = value ; }
    }

    // when true, Next( ) and Prev( ) will stay at same location and
    // clear the stay flag.
    bool? _StayAtFlag;
    public bool? StayAtFlag 
    {
      get { return _StayAtFlag ; }
      set { _StayAtFlag = value ; }
    }

#endregion

    #region constructors

    // ----------------------------- constructor ---------------------------
    public ScanWordCursor()
    {
      this.Word = null ;
      this.WordBx = null ;
      this.Position = RelativePosition.None ;
      this.DelimBx = null ;
      this.DelimPattern = null ;
      this.StayAtFlag = null;
    }

    public ScanWordCursor(ScanWordCursor Cursor)
    {
      this.Word = Cursor.Word ;
      this.WordBx = Cursor.WordBx ;
      this.Position = Cursor.Position ;
      this.DelimBx = Cursor.DelimBx ;
      this.DelimPattern = Cursor.DelimPattern ;
      this.StayAtFlag = Cursor.StayAtFlag;
    }

    public ScanWordCursor(
      TextWord Word, TextLocation WordBx, TextLocation DelimBx, ScanPattern DelimPattern)
      : this()
    {
      this.Word = Word;
      this.WordBx = WordBx;
      this.DelimBx = DelimBx;
      this.DelimPattern = DelimPattern;
    }

    #endregion

    #region  public methods

    public override string ToString()
    {
      StringBuilder sb = new StringBuilder();

      if (this.Word != null)
        sb.SentenceAppend(this.Word.Class.ToString());
      
      if (this.DelimPattern != null)
        sb.SentenceAppend(this.DelimPattern.DelimClassification.ToString());

      if (this.Word != null)
        sb.SentenceAppend(this.Word.Value);

      return sb.ToString();
    }

    #endregion

    #region public get properties

    public TextLocation CursorBx
    {
      get
      {
        if (this.Word != null)
          return this.WordBx;
        else if (this.DelimPattern != null)
          return this.DelimBx;
        else
          return null;
      }
    }

    public TextLocation DelimEx
    {
      get
      {
        if (this.DelimBx == null)
          throw new ApplicationException("cursor does not have a delimiter");
        else
          return this.DelimBx.EndColumnIndex(this.DelimPattern.Length);
      }
    }

    public bool IsEndOfString
    {
      get
      {
        if (this.Position == RelativePosition.End)
          return true;
        else if (this.Position == RelativePosition.None)
          throw new ApplicationException("invalid position");
        else
          return false;
      }
    }

    TextLocation _WordEx;
    public TextLocation WordEx
    {
      get
      {
        if (_WordEx == null)
        {
          if (this.Word == null)
            throw new ApplicationException("Cursor does not have a word");
          else
            _WordEx = this.WordBx.EndColumnIndex(this.Word.Value.Length);
        }
        return _WordEx;
      }
      set { _WordEx = value; }
    }

    /// <summary>
    /// the word and the delim are the same text.
    /// </summary>
    public bool WordIsDelim
    {
      get
      {
        if ((this.DelimBx != null) && (this.WordBx.Equals(this.DelimBx)))
          return true;
        else
          return false;
      }
    }

    #endregion

    #region public static methods

    public static ScanWordCursor PositionBegin()
    {
      ScanWordCursor cursor = new ScanWordCursor();
      cursor.Position = RelativePosition.Begin;
      return cursor;
    }

    #endregion
  } 
}

