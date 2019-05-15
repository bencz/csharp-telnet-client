using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoCoder.Html.HtmlParser
{
  // Description: Definition of token types supported by HtmlLexicalAnalyzer

  /// <summary>
  /// types of lexical tokens for html-to-xaml converter
  /// </summary>
  internal enum HtmlTokenType
  {
    OpeningTagStart,
    ClosingTagStart,
    TagEnd,
    EmptyTagEnd,
    EqualSign,
    Name,
    Atom, // any attribute value not in quotes
    Text, //text content when accepting text
    Comment,
    EOF,
  }
}
