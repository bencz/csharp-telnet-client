using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AutoCoder.Text.Enums
{

  // --------------------------- DelimClassification --------------------------
  // VirtualWhitespace : used when a delim is a char ( OpenContentBraced ) which 
  //                     should be processed as a value in the the next word.
  //                     Also, where whitespace between a special marker char like
  //                     OpenContentBraced and the preceeding word is optional, makes
  //                     is easier to process in the parse loop if a whitespace can
  //                     be assumed to always be present.
  // PathSep    - the separator ( "/" or "\" ) within a path or qualified name.
  // DividerSymbol    : a divider is delim used to divide/split values in the string.
  //                    dividers could be comma, semicolon, colon. 
  //                    Whitespace is also a divider, but it is further specialized
  //                    as whitespace ( and is not an actual symbol ) 
  public enum DelimClassification
  {
    Any, None, NotAssigned,
    Whitespace, VirtualWhitespace,
    NewLine, Quote, ExpressionSymbol,
    Assignment,
    OpenContentBraced, OpenNamedBraced, CloseBraced,
    EndStmt, EndOfString, 
    DividerSymbol,
    PathSep,
    CommentToEnd, EmbeddedComment,
    Keyword,
    SpecialValueStarter
  }

  public static class DelimClassificationExt
  {

    /// <summary>
    /// test if the enum value is equal any of the compare to enum values.
    /// </summary>
    /// <param name="Position"></param>
    /// <param name="Values"></param>
    /// <returns></returns>
    public static bool EqualAny(this DelimClassification Delim, params DelimClassification[] Values)
    {
      bool rc = false;
      foreach (var vlu in Values)
      {
        if (vlu == Delim)
        {
          rc = true;
          break;
        }
      }
      return rc;
    }

    public static bool IsOpenBraced(this DelimClassification Delim)
    {
      if (Delim == DelimClassification.OpenContentBraced)
        return true;
      else if (Delim == DelimClassification.OpenNamedBraced)
        return true;
      else
        return false;
    }

    public static WordClassification? ToWordClassification(this DelimClassification Delim)
    {
      switch (Delim)
      {
        case DelimClassification.CloseBraced:
          return WordClassification.CloseBraced;
        case DelimClassification.CommentToEnd:
          return WordClassification.CommentToEnd;

        // dividers, whitespace, end of line - never considered a word.
        case DelimClassification.DividerSymbol:
          return null;

        case DelimClassification.EmbeddedComment:
          return WordClassification.EmbeddedComment;

        case DelimClassification.EndOfString:
          return null;

        case DelimClassification.EndStmt:
          return null;

        case DelimClassification.ExpressionSymbol:
          return WordClassification.ExpressionSymbol;

        case DelimClassification.Assignment:
          return WordClassification.Assignment;

        case DelimClassification.NewLine:
          return null;

        case DelimClassification.OpenContentBraced:
          return WordClassification.OpenContentBraced;
        case DelimClassification.OpenNamedBraced:
          return WordClassification.OpenNamedBraced;

        case DelimClassification.PathSep:
          return null;

        case DelimClassification.Quote:
          return null;

        case DelimClassification.VirtualWhitespace:
          return null;
        case DelimClassification.Whitespace:
          return null;
        case DelimClassification.None:
          return null;
        default:
          throw new ApplicationException(
            "unexpected DelimClassification " + Delim.ToString());
      }
    }

    public static ScanAtomCode? ToScanAtomCode(this DelimClassification Delim)
    {
      switch (Delim)
      {
        case DelimClassification.CloseBraced:
          return ScanAtomCode.EnclosureSymbol;
        case DelimClassification.CommentToEnd:
          return ScanAtomCode.CommentToEnd;

        case DelimClassification.DividerSymbol:
          return ScanAtomCode.DividerSymbol;

        case DelimClassification.EmbeddedComment:
          return ScanAtomCode.EmbeddedComment;

        case DelimClassification.EndOfString:
          return ScanAtomCode.EndOfString;

        case DelimClassification.EndStmt:
          return ScanAtomCode.EndStmt;

        case DelimClassification.ExpressionSymbol:
          return ScanAtomCode.ExpressionSymbol;

        case DelimClassification.Assignment:
          return ScanAtomCode.Assignment;

        case DelimClassification.NewLine:
          return null;

        case DelimClassification.OpenContentBraced:
          return ScanAtomCode.EnclosureSymbol;
        case DelimClassification.OpenNamedBraced:
          return ScanAtomCode.EnclosureSymbol;

        case DelimClassification.PathSep:
          return null;

        case DelimClassification.Quote:
          return null;

        case DelimClassification.VirtualWhitespace:
          return null;
        case DelimClassification.Whitespace:
          return ScanAtomCode.Whitespace;

        case DelimClassification.Keyword:
          return ScanAtomCode.Keyword;

        case DelimClassification.SpecialValueStarter:
          return ScanAtomCode.SpecialValue;

        case DelimClassification.None:
          return null;
        default:
          throw new ApplicationException(
            "unexpected DelimClassification " + Delim.ToString());
      }
    }
  }
}
