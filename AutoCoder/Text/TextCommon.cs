using System;

namespace AutoCoder.Text
{

	// ------------------------ TextException ---------------------------
	public class TextException : ApplicationException
	{
		public TextException( string InMethod, string InMessage )
			: base( InMethod + ". " + InMessage )
		{
		}
	}

  // ----------------------- StmtElemForm -----------------------------
  // Sentence            - a space delimited sequence of StmtElem. ex: public class TextException
  // QualifiedSequence   - sequence of elements connected by the qualifying "." operator
  // QualifiedPart       - the individual "." delim word within the QualifiedSequence
  // Function            - element in form xxxxx(yy,bb)
  // ExpressionOperator  - expression symbol such as "+" or "&&" used as an operator
  //                       between factors in an expression statement.
  // lhs, rhs            - left and right hand side of an assignment statement.
  public enum StmtElemForm
  {
    None,
    Sentence, BracedSentence,
    BracedContent,
    Assignment, lhs, rhs,
    ExpressionOperator,
    Function, CSV, Command,
    ValueExpression, BooleanExpression,
    QuotedLiteral, NumericLiteral, Empty, Mixed,
    QualifiedSequence, QualifiedPart,
    TopStmt,
    Variable
  }

  // ------------------------------ StmtForm ---------------------------------
  // the form of a statement/command string stye string.
  // Function   - functionName( arg1, arg2, ... )
  // CSV        - comma seperated values. vlu1, vlu2, vlu3, ...
  // Command    - a command name, followed by args.   CPYF  fromfile tofile
  // Assignment - a stmt with two expressions, seperated by "=".   a = func(b,c)
  // Boolean    - stmt where the elements are seperated by boolean operators
  //                a == 5 or b < 25 
  public enum StmtForm { None, Function, CSV, Command, Assignment, Boolean }

	// ------------------------ QuoteEncapsulation -----------------------------
	// how quoted strings encapsulate contained quotes.
	//   Double - the quotes are doubled up in the string.  "a ""quoted"" word"
	//   Escape - quote chars preceeded by the backslash escape char are part of
	//            the quoted string.   "a \"quoted\" word"
	// todo: possible other name - EnclosedQuoteMethod
	public enum QuoteEncapsulation { Double, Escape }

#if skip
  // --------------------------- WordCompositeCode ----------------------------
  // classifies how words are grouped in the StmtParser first pass.
  // words are either a collection of paren enclosed words, a sequence of words
  // in sentence form, or a standalone word.
  public enum WordCompositeCode
  {
    Atom, Braced, Sentence, General, None, Any
  } 
#endif

#if skip
	// --------------------------- DelimClassification --------------------------
	// VirtualWhitespace : used when a delim is a char ( OpenContentBraced ) which 
  //                     should be processed as a value in the the next word.
  //                     Also, where whitespace between a special marker char like
  //                     OpenContentBraced and the preceeding word is optional, makes
  //                     is easier to process in the parse loop if a whitespace can
  //                     be assumed to always be present.
  // PathSep    - the separator ( "/" or "\" ) within a path or qualified name.
  public enum DelimClassification { 
    Any, None, NotAssigned, 
    Whitespace, VirtualWhitespace,
    NewLine, Quote, ExpressionSymbol,
    OpenContentBraced, OpenNamedBraced, CloseBraced,
    EndStmt, EndOfString, Value,
    PathSep, 
    CommentToEnd, EmbeddedComment
  }
#endif

	// --------------------------- AutoCoder.Text.Chars ----------------------
	// static methods that return various character arrays
	public class Chars
	{

		// ---------------------------- CharArrayChars  ------------------------------
		public static char[] CharArrayChars( char InChar1 )
		{
			char[] ca = { InChar1 } ;
			return ca ;
		}

		// ---------------------------- CharArrayChars  ------------------------------
		public static char[] CharArrayChars( char InChar1, char InChar2 )
		{
			char[] ca = { InChar1, InChar2 } ;
			return ca ;
		}

		// ----------------------- an array of whitespace characters -------------
		public static char[] WhitespaceChars( )
		{
			char[] ws = { ' ', '\t' } ;
			return ws ;
		}

		// ----------------- whitespace characters + the input character -----------
		public static char[] WhitespaceChars( char InAdditionalChar )
		{
			char[] ws = { ' ', '\t', InAdditionalChar } ;
			return ws ;
		}

	}
}
