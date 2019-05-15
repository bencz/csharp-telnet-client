using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AutoCoder.Text.Enums
{

  // todo: remove the sub classifications of open, close and fully braced words.
  //       Replace with a BraceClassification enum. 

  // note : some WordClassification values exist only to map to DelimClassification.
  //        It is possible for a delim 

  /// <summary>
  /// classification of word in a string.
  /// </summary>
  public enum WordClassification
  {
    None, Any, Braced, 
    Identifier, 
    MixedText, Numeric, Quoted,
    ContentBraced, NamedBraced, OpenNamedBraced, OpenContentBraced,
    Delimeter,
    CloseBraced,
    CommentToEnd, EmbeddedComment,
    Sentence,
    ExpressionSymbol,
    Assignment,
    LambdaSymbol,
    DividerSymbol
  }

  public static class WordClassificationExt
  {
    public static bool IsOpenBraced(this WordClassification WordClass)
    {
      if (WordClass == WordClassification.OpenContentBraced)
        return true;
      else if (WordClass == WordClassification.OpenNamedBraced)
        return true;
      else
        return false;
    }
  }

}
