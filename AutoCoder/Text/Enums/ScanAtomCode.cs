using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AutoCoder.Text.Enums
{
  public enum ScanAtomCode
  {
    // identifier, Keyword and SpecialValue should probably be combined as Identifer.
    // With IdentifierCode being Keyword, SpecialValue and Name/identifier.
    Identifier,
    Keyword,
    SpecialValue,

    Numeric, Quoted,
    DividerSymbol,
    EndStmt,
    EndOfString,
    EnclosureSymbol,
    Delimeter,
    CommentToEnd, EmbeddedComment,
    
    // the entire line is a comment to end
    EntireLineCommentToEnd,

    ExpressionSymbol,
    Assignment,
    Whitespace,
    InsignificantWhitespace,
    Qualifier
  }

  public static class ScanAtomCodeExt
  {
    public static bool WhitespaceIsSignificant(this ScanAtomCode AtomCode)
    {
      if (AtomCode == ScanAtomCode.Identifier)
        return true;
      else if (AtomCode == ScanAtomCode.Keyword)
        return true;
      else if (AtomCode == ScanAtomCode.Numeric)
        return true;
      else if (AtomCode == ScanAtomCode.Quoted)
        return true;
      else
        return false;
    }

    public static bool IsIdentifier(this ScanAtomCode? AtomCode)
    {
      if (AtomCode == null)
        return false;
      else
        return AtomCode.Value.IsIdentifier();
    }

    public static bool IsIdentifier(this ScanAtomCode AtomCode)
    {
      if (AtomCode == ScanAtomCode.Identifier)
        return true;
      else if (AtomCode == ScanAtomCode.Keyword)
        return true;
      else if (AtomCode == ScanAtomCode.SpecialValue)
        return true;
      else
        return false;
    }
  }
}
