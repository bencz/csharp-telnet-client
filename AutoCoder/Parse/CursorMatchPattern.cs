using System;
using System.Collections.Generic;
using System.Text;
using AutoCoder.Text;
using AutoCoder.Core;
using AutoCoder.Text.Enums;

namespace AutoCoder.Parse
{
  /// <summary>
  /// Used to compare if a stmt word cursor matches a pattern being
  /// searched for.
  /// </summary>
  public class CursorMatchPattern
  {
    WordCompositeCode mCompositeCode = WordCompositeCode.Any;
    WhichEdge mEdge = WhichEdge.Any;
    DelimClassification mDelimClass = DelimClassification.Any;
    RulePolarity mPolarity = RulePolarity.Positive;
    string mOpenBracePattern = null;
    string mFreeFormPattern = null ;
    
    // array of allowed values. 
    string[] mAllowedValues = null;

    // the case to convert WordText to when matching AllowedValues.
    CharCase mCase = CharCase.Upper;

    public CursorMatchPattern(
      WordCompositeCode InCompositeCode, WhichEdge InEdge,
      DelimClassification InDelimClass)
    {
      mCompositeCode = InCompositeCode;
      mEdge = InEdge;
      mDelimClass = InDelimClass;
    }

    public CursorMatchPattern(
      WordCompositeCode InCompositeCode, WhichEdge InEdge )
    {
      mCompositeCode = InCompositeCode;
      mEdge = InEdge;
      mDelimClass = DelimClassification.Any;
    }

    public CursorMatchPattern(WordCompositeCode InCompositeCode)
    {
      mCompositeCode = InCompositeCode;
      mEdge = WhichEdge.Any;
      mDelimClass = DelimClassification.Any;
    }

    public CursorMatchPattern(string InFreeFormPattern)
    {
      FreeFormPattern = InFreeFormPattern;
    }

    public string[] AllowedValues
    {
      get { return mAllowedValues; }
      set { mAllowedValues = value; }
    }

    public CharCase Case
    {
      get { return mCase; }
      set { mCase = value; }
    }

    public WordCompositeCode CompositeCode
    {
      get { return mCompositeCode; }
      set { mCompositeCode = value; }
    }

    public DelimClassification DelimClass
    {
      get { return mDelimClass; }
      set { mDelimClass = value; }
    }

    public bool DoesMatch(StmtWordListCursor InCursor)
    {
      bool doesMatch = true;

      if (InCursor.IsAtEnd == true)
      {
        doesMatch = false;
      }

      // match CompositeCode.
      if ((doesMatch == true) && (mCompositeCode != WordCompositeCode.Any))
      {
        if (mCompositeCode != InCursor.StmtWord.CompositeCode)
        {
          doesMatch = false;
        }
      }

      // brace character.
      if ((doesMatch == true) && ( OpenBracePattern != null ))
      {
        if (InCursor.StmtWord.IsBraced(OpenBracePattern) == false)
          doesMatch = false;
      }

      // match Edge.
      if ((doesMatch == true) && (mEdge != WhichEdge.Any))
      {
        if (mEdge != InCursor.Edge)
        {
          doesMatch = false;
        }
      }

      // match DelimClass.
      if ((doesMatch == true) && (mDelimClass != DelimClassification.Any))
      {
        if (mDelimClass != InCursor.StmtWord.DelimClass)
        {
          doesMatch = false;
        }
      }

      // match WordText against allowed values.
      if (( doesMatch == true ) && ( mAllowedValues != null ))
      {
        string s1 = null ;
        switch(Case)
        {
          case CharCase.Upper:
            s1 = InCursor.StmtWord.UpperWordText ;
            break ;

          case CharCase.Lower:
            s1 = InCursor.StmtWord.WordText.ToLower( ) ;
            break ;

          default:
            s1 = InCursor.StmtWord.WordText ;
            break ;
        }
        int fx = Array.IndexOf<string>( AllowedValues, s1 ) ;
        if ( fx == -1 )
          doesMatch = false ;
      }

      // reverse the polarity of the match result.
      if (Polarity == RulePolarity.Negative)
        doesMatch = !doesMatch;

      return doesMatch;
    }

    public WhichEdge Edge
    {
      get { return mEdge; }
      set { mEdge = value; }
    }

    /// <summary>
    /// Free form pattern is translated into distinct pattern match items.
    /// Sample patterns: not_obc( = not the "(" OpenBracePattern.
    /// </summary>
    public string FreeFormPattern
    {
      get { return mFreeFormPattern; }
      set
      {
        mFreeFormPattern = value;
        string[] parts = mFreeFormPattern.Split(new char[] { '_' });
        foreach (string part in parts)
        {
          int partLx = part.Length ;

          if (part == "not")
            Polarity = RulePolarity.Negative;

            // obp(
          else if ((partLx == 4) && (part.Substring(0, 3) == "obp"))
          {
            OpenBracePattern = part.Substring(3, 1);
          }

          // Lead, Trail, LeadEdge, TrailEdge
          else if ((part == "Lead") || (part == "LeadEdge"))
            Edge = WhichEdge.LeadEdge;
          else if ((part == "Trail") || (part == "TrailEdge"))
            Edge = WhichEdge.TrailEdge;

          // wccAtom, wccSentence, ... : WordCompositeCode xxxx
          else if ((partLx > 3) && (part.Substring(0, 3) == "wcc"))
          {
            string wcc = part.Substring(3);
            if (wcc == "Atom")
              CompositeCode = WordCompositeCode.Atom;
            else if (wcc == "Braced")
              CompositeCode = WordCompositeCode.Braced;
            else if (wcc == "Sentence")
              CompositeCode = WordCompositeCode.Sentence;
            else if (wcc == "General")
              CompositeCode = WordCompositeCode.General;
            else
              throw new ApplicationException("unexpected composit code: " + part); 
          }

          // dcNewLine, dcWhitespace, ... : DelimClassification code.
          else if ((partLx > 2) && (part.Substring(0, 2) == "dc"))
          {
            string dc = part.Substring(2);
            DelimClass = (DelimClassification)Enum.Parse(typeof(DelimClassification), dc);
          }

          // caseMixed, caseLower, ... : case of the text to match to AllowedValues.
          else if ((partLx > 4) && (part.Substring(0, 4) == "case"))
          {
            string s1 = part.Substring(4);
            Case = (CharCase)Enum.Parse(typeof(CharCase), s1);
          }

          // av|V1|v2|v3|v4 : allowed values. separate the values with "|" char.
          else if ((partLx > 3) && (part.Substring(0, 3) == "av|"))
          {
            AllowedValues = part.Substring(3).Split(new char[] { '|' });
          }
        }
      }
    }

    public string OpenBracePattern
    {
      get { return mOpenBracePattern; }
      set { mOpenBracePattern = value; }
    }

    public RulePolarity Polarity
    {
      get { return mPolarity; }
      set { mPolarity = value; }
    }
  }
}
