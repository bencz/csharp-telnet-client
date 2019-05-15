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

	// ------------------------ QuoteEncapsulation -----------------------------
	// how quoted strings encapsulate contained quotes.
	//   Double - the quotes are doubled up in the string.  "a ""quoted"" word"
	//   Escape - quote chars preceeded by the backslash escape char are part of
	//            the quoted string.   "a \"quoted\" word"
	// todo: possible other name - EnclosedQuoteMethod
	public enum QuoteEncapsulation { Double, Escape }

	// ---------------------------- WordClassification -----------------------------
	/// <summary>
	/// classification of word in a string.
	/// </summary>
	public enum WordClassification {
		None, Name, MixedText, Numeric, Quoted, Braced, NameBraced, Delimeter }

	// --------------------------- DelimClassification --------------------------
	public enum DelimClassification { None, Whitespace, End, Value }

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
