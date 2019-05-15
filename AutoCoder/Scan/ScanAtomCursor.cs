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
  public class ScanAtomCursor
  {
    
    #region public get/set properties

    public AtomText AtomText
    { get; set; }

    /// <summary>
    /// property set when more than 1 scan patterns match the text at the
    /// scan found begin position.
    /// List items contain the AtomText of the found pattern. Also the ScanPattern
    /// which made the match.
    /// </summary>
    public List<MatchScanPattern> ManyAtomText
    { get; set; }

    RelativePosition _Position;
    public RelativePosition Position
    {
      get { return _Position ; }
      set { _Position = value ; }
    }

    ScanPattern _AtomPattern ;
    public ScanPattern AtomPattern
    {
      get { return _AtomPattern ; }
      private set 
      {
        _AtomPattern = value ;
      }
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
    public ScanAtomCursor()
    {
      this.AtomText = null;
      this.Position = RelativePosition.None ;
      this.AtomPattern = null ;
      this.StayAtFlag = null;
      this.ManyAtomText = null;
    }

    public ScanAtomCursor(ScanAtomCursor Cursor)
    {
      this.AtomText = Cursor.AtomText ;
      this.Position = Cursor.Position ;
      this.AtomPattern = Cursor.AtomPattern ;
      this.StayAtFlag = Cursor.StayAtFlag;
      this.ManyAtomText = Cursor.ManyAtomText;
    }

    public ScanAtomCursor(
      AtomText AtomText, ScanPattern AtomPattern,
      RelativePosition Position = RelativePosition.At)
      : this()
    {
      this.AtomText = AtomText;
      this.AtomPattern = AtomPattern;
      this.Position = Position;
    }

    public ScanAtomCursor(List<MatchScanPattern> ManyAtomText)
      : this()
    {
      this.ManyAtomText = ManyAtomText;
      this.Position = RelativePosition.At;
    }

    public ScanAtomCursor(MatchScanPattern MatchPattern, ScanStream ScanStream)
    {
      MatchPattern.AssignAtomText(ScanStream);
      this.AtomText = MatchPattern.AtomText;
      this.AtomPattern = MatchPattern.MatchPattern;
      this.Position = RelativePosition.At;
    }

    #endregion

    #region  public methods

    public override string ToString()
    {
      StringBuilder sb = new StringBuilder();

      sb.SentenceAppend(this.Position.ToString());
      if (this.AtomText != null)
        sb.SentenceAppend(this.AtomText.ToString( ));

      return sb.ToString();
    }

    #endregion

    #region public get properties

    public ScanAtomCode? AtomCode
    {
      get
      {
        if (this.AtomText != null)
          return this.AtomText.AtomCode;
        else if (this.ManyAtomText != null)
          return this.ManyAtomText[0].AtomText.AtomCode;
        else
          return null;
      }
    }

    public TextLocation StartLoc
    {
      get
      {
        if (this.AtomText != null)
          return this.AtomText.StartLoc ;
        else
          return null;
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

    public bool WhitespaceIsSignificant
    {
      get
      {
        if ((this.Position == RelativePosition.At)
          || (this.Position == RelativePosition.After))
        {
          if (this.AtomText != null)
            return this.AtomText.AtomCode.WhitespaceIsSignificant();
          else
          {
            foreach (var at in this.ManyAtomText)
            {
              bool isSig = at.AtomText.AtomCode.WhitespaceIsSignificant();
              if (isSig == true)
                return true;
            }
            return false;
          }
        }
        else
        {
          return false;
        }
      }
    }

    public TextLocation EndLoc
    {
      get
      {
        if (this.AtomText != null)
          return this.AtomText.EndLoc;
        else if (this.ManyAtomText != null)
        {
          TextLocation maxEndLoc = null;
          int maxMatchLength = -1 ;
          foreach (var matchPat in this.ManyAtomText)
          {
            if ( matchPat.MatchLength > maxMatchLength )
            {
              maxEndLoc = matchPat.AtomText.EndLoc ;
              maxMatchLength = matchPat.MatchLength ;
            }
          }
          return maxEndLoc;
        }
        else
          return null;
      }
    }

    #endregion

    #region public static methods

    public static ScanAtomCursor PositionBegin()
    {
      ScanAtomCursor cursor = new ScanAtomCursor();
      cursor.Position = RelativePosition.Begin;
      return cursor;
    }

    #endregion
  } 
}
