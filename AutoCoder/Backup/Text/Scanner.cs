using System;
using AutoCoder ;

namespace AutoCoder.Text
{

	public class WordValue
	{
		string mValue ;
		WordClassification mClass ;
		TextTraits mTraits = null ;
		char mBraceChar = ' ' ;

		public WordValue( string InValue, TextTraits InTraits )
		{
			CharObjectPair results = Stringer.CalcWordClassification( InValue, InTraits ) ;
			CommonConstruct(
				InValue, (WordClassification) results.b, InTraits, results.a ) ;
		}

		public WordValue( string InValue, WordClassification InClass, TextTraits InTraits )
		{
			CommonConstruct( InValue, InClass, InTraits, ' ' ) ;
		}

		public WordValue(
			string InValue, WordClassification InClass, TextTraits InTraits, char InBraceChar )
		{
			CommonConstruct( InValue, InClass, InTraits, InBraceChar ) ;
		}

		public char BraceChar
		{
			get {
				if (( mClass == WordClassification.Braced )
					|| ( mClass == WordClassification.NameBraced ))
					return mBraceChar ;
				else
					throw( new ApplicationException( "Get BraceChar failed. Not a braced word." )) ;
			}
			set { mBraceChar = value ; }
		}

		/// <summary>
		/// The BraceName is the word immed prior to the enclosing braces of the
		/// NameBraced word.  ex: the xxx in the word xxx( yyy, zzz )
		/// </summary>
		public string BraceName
		{
			get
			{
				if ( mClass != WordClassification.NameBraced )
					throw( new ApplicationException(
						"word is not name braced. BraceName property is not available." )) ;
				else
				{
					int Ix = mValue.IndexOf( BraceChar ) ;
					return mValue.Substring( 0, Ix ) ;
				}
			}
		}

		public WordValue BracedValue
		{
			get
			{
			int Bx = 0, Ex = 0, Fx = 0, Lx = 0 ;
				if ( mClass == WordClassification.Braced )
				{
					Bx = 1 ;
					Lx = mValue.Length - 2 ;
				}
				else if ( mClass == WordClassification.NameBraced )
				{
					Bx = mValue.IndexOf( BraceChar ) + 1 ;
					Lx = mValue.Length - Bx - 1 ; 
				}
				else
					throw( new ApplicationException( "Get BracedValue failed. Not a braced word." )) ;
				
				// trim whitespace from the front.
				Bx = Scanner.ScanNotEqual( mValue, Bx, mTraits.WhitespaceChars ).a ;
				Ex = Bx + Lx - 1 ;
				if ( Bx != -1 )
				{
					Fx = Scanner.ScanReverseNotEqual( mValue, Ex, mTraits.WhitespaceChars ).a ;
					if ( Fx == -1 )
						Lx = 0 ;
					else
						Lx = Fx - Bx + 1 ;
				}

				// isolate the braced word value.
				WordValue bracedValue = null ;
				if (( Bx != -1 ) && ( Lx > 0 )) 
					bracedValue = new WordValue( mValue.Substring( Bx, Lx ), mTraits ) ;
				else
					bracedValue = new WordValue( "", WordClassification.MixedText, mTraits ) ;

				return bracedValue ;
			}
		}

		public WordClassification Class
		{
			get { return mClass ; }
			set { mClass = value ; }
		}

		private void CommonConstruct(
			string InValue, WordClassification InClass, TextTraits InTraits, char InBraceChar )
		{
			mValue = InValue ;
			mClass = InClass ;
			mTraits = InTraits ;
			mBraceChar = InBraceChar ;

			// depending on word class, brace char must be specified or be blanks.
			if (( mClass == WordClassification.Braced )
				|| ( mClass == WordClassification.NameBraced ))
			{
				if ( mBraceChar == ' ' )
					throw( new ApplicationException( "Brace character of braced word is missing" )) ;
			}
		}

		// -------------------------- DequotedWord ------------------------------
		// return the word value, its enclosing quotes removed.
		// will throw an ApplicationException if the word is not quoted.
		public string DequotedWord
		{
			get { return Stringer.Dequote( mValue, mTraits.QuoteEncapsulation ) ; }
		}

		/// <summary>
		/// return the word value.  if quoted, dequote it. otherwise, return as is.
		/// </summary>
		public string NonQuotedWord
		{
			get
			{
				if ( WordIsQuoted == true )
					return( DequotedWord ) ;
				else
					return ToString( ) ;
			}
		}

		/// <summary>
		/// The quote char that encloses the quoted word. 
		/// </summary>
		public char QuoteChar
		{
			get
			{
				if ( mClass == WordClassification.Quoted )
					return mValue[0] ;
				else
					throw( new ApplicationException( "Get QuoteChar failed. Not a quoted word." )) ;
			}
		}

		// ----------------------- SetValue ---------------------------------
		/// <summary>
		/// Set the value of the word and its position in the string.  
		/// </summary>
		/// <param name="InWord"></param>
		/// <param name="InWordBx"></param>
		/// <returns></returns>
		private void SetValue(
			string InValue, WordClassification InClass, char InBraceChar )
		{
			mValue = InValue ;
			mClass = InClass ;
			mBraceChar = ' ' ;
			
			// brace character.
			if ( mClass == WordClassification.Braced )
				mBraceChar = mValue[0] ;
			else if ( InClass == WordClassification.NameBraced )
			{
				if ( InBraceChar == ' ' )
					throw( new ApplicationException( "brace char of NameBraced word is missing" )) ;
				mBraceChar = InBraceChar ;
			}
		}

		/// <summary>
		/// The text value of the word.
		/// </summary>
		/// <returns></returns>
		public override string ToString( )
		{
			return mValue ;
		}

		public bool WordIsQuoted
		{
			get
			{
				if ( mClass == WordClassification.Quoted )
					return true ;
				else
					return false ;
			}
		}
	}

	// ------------------------- AutoCoder.Text.Scanner -----------------------
	public class Scanner
	{

		// -------------------------- ScanCharResults ----------------------------
		public struct ScanCharResults
		{
			public int a ;		// the resulting pos of the char
			public char b ;		// the resulting char

			public ScanCharResults( int InNotFoundIx )
			{
				a = InNotFoundIx ;
				b = '\0' ;
			}
			public ScanCharResults( char InResultChar, int InResultPos )
			{
				a = InResultPos ;
				b = InResultChar ;
			}
			public char ResultChar 
			{
				get { return b ; }
			}
			public int ResultPos
			{
				get { return a ; }
			}
		}

		// -------------------------- WordCursor ------------------------
		public class WordCursor
		{
			// The value of the word itself. holds other info such as the WordClassification
			// ( is it quoted, braced, numeric, ... )
			WordValue mWordValue ;

			int mWordBx ;
			int mDelimBx ;
			string mDelim ;
			bool mDelimIsWhitespace ;
			AcRelativePosition mRltv ;
			TextTraits mTraits = null ;
			string mString ;  // the string the word is in.

			// ----------------------------- constructor ---------------------------
			public WordCursor( )
			{
				EmptyWord( ) ;
				mTraits = null ;
				mString = null ;
			}

			public WordCursor( WordCursor InWord )
			{
				mWordValue = InWord.mWordValue ;
				mWordBx = InWord.WordBx ;
				mDelimBx = InWord.DelimBx ;
				mDelim = InWord.mDelim ;
				mDelimIsWhitespace = InWord.mDelimIsWhitespace ;
				mRltv = InWord.mRltv ;
				mTraits = InWord.mTraits ;
				mString = InWord.mString ;
			}

			/// <summary>
			/// advance this cursor to the next word.
			/// ( the NextWord method returns a new cursor object that is advance to the
			/// next word. )
			/// </summary>
			/// <returns></returns>
			public void AdvanceNextWord( )
			{
				if ( mString == null )
					throw( new ApplicationException(
						"Scan next word exception. String is not assigned." )) ;
				Scanner.AdvanceNextWord( mString, this, Traits ) ;
			}

			// -------------------------- properties --------------------------

			public string Delim
			{
				get
				{
					if ( mDelim == null )
						return "" ;
					else
						return mDelim ;
				}
				set { mDelim = value ; }
			}
			public int DelimBx
			{
				get { return mDelimBx ; }
			}

			public DelimClassification DelimClass
			{
				get
				{
					DelimClassification dc ;

					if ( DelimIsWhitespace == true )
						dc = DelimClassification.Whitespace ;
					else if ( Delim.Length > 0 )
						dc = DelimClassification.Value ;
					else if ( Word == null )
						dc = DelimClassification.None ;
					else
						dc = DelimClassification.End ;
					return dc ;
				}
			}

			public int DelimEx
			{
				get
				{
					if ( mDelim == null )
						return -1 ;
					else
						return( mDelim.Length + mDelimBx - 1 ) ;
				}
			}

			public bool DelimIsWhitespace
			{
				get { return mDelimIsWhitespace ; }
				set { mDelimIsWhitespace = value ; }
			}

			// -------------------------- Clone ------------------------------
			public WordCursor Clone( )
			{
				WordCursor word = new WordCursor( this ) ;
				return word ;
			}

			/// <summary>
			/// Word cursor is positioned at a word.
			/// </summary>
			public bool IsAtWord
			{
				get
				{
					if ( mRltv == AcRelativePosition.At )
						return true ;
					else
						return false ;
				}
			}

			/// <summary>
			/// Empty the word attributes of the cursor.
			/// </summary>
			public void EmptyWord( )
			{
				mWordValue = null ;
				mWordBx = -1 ;
				mDelimBx = -1 ;
				mDelim = null ;
				mRltv = AcRelativePosition.None ;
				mDelimIsWhitespace = false ;
			}

			// ----------------------- IsEmptyWord -----------------------
			public bool IsEmptyWord
			{
				get
				{
					if (( mWordBx == -1 ) && ( mDelimBx == -1 ))
						return true ;
					else
						return false ;
				}
			}

			/// <summary>
			/// Word cursor is at end of string.
			/// </summary>
			public bool IsEndOfString
			{
				get
				{
					if ( mRltv == AcRelativePosition.End )
						return true ;
					else
						return false ;
				}
			}

			/// <summary>
			/// scan to the word in string after this one.
			/// </summary>
			/// <returns></returns>
			public WordCursor NextWord( )
			{
				if ( mString == null )
					throw( new ApplicationException(
						"Scan next word exception. String is not assigned." )) ;
				return( Scanner.ScanNextWord( mString, this, Traits )) ;
			}

			// -------------------- RelativePostion -------------------------
			// use when calc the start position when scanning to next word.
			public AcRelativePosition RelativePosition
			{
				get { return mRltv ; }
				set { mRltv = value ; }
			}

			// ---------------------- ScanBx -------------------------
			// Begin position of the scan. either the bgn pos of the word or delim. 
			// Whichever is present.
			public int ScanBx
			{
				get
				{
					if ( IsEmptyWord == true )
						return 0 ;
					else if ( WordBx != -1 )
						return WordBx ;
					else if ( DelimBx >= 0 )
						return DelimBx ;
					else
						return 0 ;
				}
			}

			// ---------------------- ScanEx -------------------------
			// End position of the scan. either the end pos of the word or delim. 
			// Whichever is greater.
			public int ScanEx
			{
				get
				{
					if ( IsEmptyWord == true )
						return -1 ;
					else if ( DelimEx >= 0 )
						return DelimEx ;
					else
						return WordEx ;
				}
			}

			// ------------------------------ SetDelim ---------------------------------
			public WordCursor SetDelim( string InDelim, int InDelimBx )
			{
				mDelim = InDelim ;
				mDelimBx = InDelimBx ;
				return this ;
			}

			/// <summary>
			/// The word is delimeter only.
			/// </summary>
			/// <returns></returns>
			public WordCursor SetNullWord( )
			{
				mWordValue = null ;
				mWordBx = -1 ;
				RelativePosition = AcRelativePosition.At ;
				return this ;
			}

			// ------------------------- SetString ---------------------------------
			public WordCursor SetString( string InString )
			{
				mString = InString ;
				return this ;
			}

			// ----------------------- SetTraits ---------------------------------
			public WordCursor SetTraits( TextTraits InTraits )
			{
				mTraits = InTraits ;
				return this ;
			}

			// ----------------------- SetWord ---------------------------------
			/// <summary>
			/// Set the value of the word and its position in the string.  
			/// </summary>
			/// <param name="InWord"></param>
			/// <param name="InWordBx"></param>
			/// <returns></returns>
			public WordCursor SetWord( string InWord, WordClassification InClass, int InWordBx )
			{
				return( SetWord( InWord, InClass, InWordBx, ' ' )) ;
			}

			// ----------------------- SetWord ---------------------------------
			/// <summary>
			/// Set the value of the word and its position in the string.  
			/// </summary>
			/// <param name="InWord"></param>
			/// <param name="InWordBx"></param>
			/// <returns></returns>
			public WordCursor SetWord(
				string InWord, WordClassification InClass, int InWordBx, char InBraceChar )
			{
				mWordValue = new WordValue( InWord, InClass, mTraits, InBraceChar ) ;
				mWordBx = InWordBx ;
				RelativePosition = AcRelativePosition.At ;
				return this ;
			}

			// -------------------------- Traits ----------------------------
			public TextTraits Traits
			{
				get
				{
					if ( mTraits == null )
						mTraits = new TextTraits( ) ;
					return mTraits ;
				}
				set { mTraits = value ; }
			}

			/// <summary>
			/// the string value of the word.
			/// </summary>
			public WordValue Word
			{
				get
				{
					if ( IsAtWord == false )
						return null ;
					else
						return mWordValue ;		// could be null if word is delim only. 
				}
			}
			public int WordBx
			{
				get { return mWordBx ; }
			}


			public int WordEx
			{
				get
				{
					if ( mWordBx == -1 )
						return -1 ;
					else
						return ( mWordBx + Word.ToString( ).Length - 1 ) ;
				}
			}

			public int WordLx
			{
				get
				{
					if ( IsAtWord == false )
						return 0 ;
					else
						return Word.ToString( ).Length ;
				}
			}
		}
		// ---------------------- end of class Scanner.WordCursor ------------------

		// ----------------------- IsOpenBraceChar ----------------------------
		public static bool IsOpenBraceChar( char InValue )
		{
			if (( InValue == '(' ) ||
				( InValue == '{' ) ||
				( InValue == '[' ) ||
				( InValue == '<' ))
				return true ;
			else
				return false ;
		}

		// ----------------------- IsOpenQuoteChar ----------------------------
		public static bool IsOpenQuoteChar( char InValue )
		{
			if (( InValue == '\'' ) ||
				( InValue == '"' ))
				return true ;
			else
				return false ;
		}

		// ------------------------ PositionBeforeWord ----------------------
		public static WordCursor PositionBeforeWord( WordCursor InWord )
		{
			WordCursor word = new WordCursor( InWord ) ;
			word.RelativePosition = AcRelativePosition.Before ;
			return word ;
		}

		// ----------------------- PositionBeginWord -----------------------------
		// position at begin of string to scan.
		public static WordCursor PositionBeginWord( )
		{
			WordCursor word = new WordCursor( ) ;
			word.RelativePosition = AcRelativePosition.Begin ;
			return word ;
		}

		// ------------------------ ScanCloseBrace -------------------------
		public static int ScanCloseBrace( string InString, int InBx )
		{
			return( ScanCloseBrace( InString, InBx, QuoteEncapsulation.Escape )) ;
		}

		// ------------------------ ScanCloseBrace -------------------------
		// todo: pass TextTraits to a version of this method. Use recursion for each brace
		//       char found.
		public static int ScanCloseBrace(
			string InString, int InBx, QuoteEncapsulation InQem )
		{
			char openBraceChar = InString[InBx] ;
			char closeBraceChar = AcCommon.CalcCloseBraceChar( openBraceChar ) ;
			int Ix = InBx, Fx = 0 ;
			int Lx = InString.Length ;
			int ParenLevel = 1 ;
			while( true )
			{
				++Ix ;
				if ( Ix >= Lx )
					throw( new TextException(
						"ScanCloseBrace",	"Close brace not found." )) ;
        
				char ch1 = InString[Ix] ;
				if ( ch1 == openBraceChar )
					++ParenLevel ;
				else if ( ch1 == closeBraceChar )
				{
					--ParenLevel ;
					if ( ParenLevel == 0 )
						break ;
				}
				else if ( IsOpenQuoteChar( ch1 ) == true )
				{
					Fx = ScanCloseQuote( InString, Ix, InQem ) ;
					Ix = Fx ;
				}
			}
			return Ix ;
		}

		// --------------------------- ScanCloseQuote ------------------------------
		public static int ScanCloseQuote(
			string InString, int InBx, QuoteEncapsulation InQem )
		{
			char QuoteChar = InString[InBx] ;
			int cloqIx = -1 ;

			for( int Ix = InBx + 1 ; Ix < InString.Length ; ++Ix )
			{
				char ch1 = InString[Ix] ;

				// using the escape method to enclose quote chars. This means the escape char
				// is used to encapsulate other special characters in the quoted string.
				// todo: dequote using "QuotedStringTraits" rules.
				if (( ch1 == '\\' )
					&& ( InQem == QuoteEncapsulation.Escape )
					&& ( Ix < ( InString.Length - 1 )))
				{
					++Ix ;
				}

					// quote char enquoted using the "double the char" method.
				else if (( ch1 == QuoteChar ) &&
					( InQem == QuoteEncapsulation.Double ) &&
					( AcCommon.PullChar( InString, Ix + 1 ) == QuoteChar ))
				{
					++Ix ;
				}

					// found the closing quote char.
				else if ( ch1 == QuoteChar )
				{
					cloqIx = Ix ;
					break ;
				}
			}

			return cloqIx ;
		}

		// --------------------------- ScanEqual-------------------------------
		// scan for the first char equal to any of the pattern characters.
		public static ScanCharResults ScanEqual( string InValue, char[] InPatternChars )
		{
			return( ScanEqual( InValue, 0, InPatternChars )) ;
		}

		// --------------------------- ScanEqual-------------------------------
		// scan for the first char equal to any of the pattern characters.
		public static ScanCharResults ScanEqual(
			string InValue,
			int InBx,
			char[] InPatternChars )
		{
			int PatLx = InPatternChars.Length ;
			int VluLx = InValue.Length ;
			for( int Ix = InBx ; Ix < VluLx ; ++Ix )
			{
				char ch1 = InValue[Ix] ;
				for( int Jx = 0 ; Jx < PatLx ; ++Jx )
				{
					if ( ch1 == InPatternChars[Jx] )
						return( new ScanCharResults( ch1, Ix )) ;
				}
			}
			return( new ScanCharResults( -1 )) ;
		}

		// ------------------------ ScanNextWord -------------------------
		// Scans to the next word in the string. ( a word being the text bounded by the
		// delimeter and whitespace characters as spcfd in the TextTraits argument )
		// Return null when end of string.
		public static WordCursor ScanNextWord(
			string InString,
			WordCursor InCurrentWord,
			TextTraits InTraits )
		{
			int Bx ;
			WordCursor results =	new WordCursor( )
				.SetString( InString )
				.SetTraits( InTraits ) ;

			// calc scan start position
			Bx = ScanWord_CalcStartBx( InString, InCurrentWord ) ;

			// advance past whitespace
			if ( Bx < InString.Length )
				Bx = ScanNotEqual( InString, Bx, InTraits.WhitespaceChars ).ResultPos ;

			// got the start of something. scan for the delimeter ( could be the current char )
			if ( Bx < InString.Length )
			{
				ScanWord_IsolateWord( InString, Bx, ref results, InTraits ) ;
			}

			// depending on the word, isolate and store the delim that follows.
			ScanWord_IsolateDelim( InString, Bx, ref results, InTraits ) ;

			// current word position.
			if ( results.ScanEx == -1 )
				results.RelativePosition = AcRelativePosition.End ;
			else
				results.RelativePosition = AcRelativePosition.At ;

			return results ;
		}

		// ----------------------------- ScanNotEqual -----------------------------
		public static ScanCharResults ScanNotEqual(
			string InValue,
			int InBx,
			char InPattern )
		{
			int Fx = -1 ;
			int Ix = InBx - 1 ;
			while( true )
			{
				++Ix ;
				if ( Ix >= InValue.Length )
					break ;
				char ch1 = InValue[Ix] ;
				if ( ch1 != InPattern )
				{
					Fx = Ix ;
					break ;
				}
			}
			if ( Fx == -1 )
				return( new ScanCharResults( -1 )) ;
			else
				return( new ScanCharResults( InValue[Fx], Fx )) ;
		}

		// --------------------------- ScanNotEqual-------------------------------
		// scan for the first char not equal to all of the pattern characters.
		public static ScanCharResults ScanNotEqual(
			string InValue,
			int InBx,
			char[] InPatternChars )
		{
			int PatLx = InPatternChars.Length ;
			int VluLx = InValue.Length ;
			for( int Ix = InBx ; Ix < VluLx ; ++Ix )
			{
				char ch1 = InValue[Ix] ;
				bool IsEqual = false ;
				for( int Jx = 0 ; Jx < PatLx ; ++Jx )
				{
					if ( ch1 == InPatternChars[Jx] )
					{
						IsEqual = true ;
						break ;
					}
				}
				if ( IsEqual == false )
					return( new ScanCharResults( ch1, Ix )) ;
			}
			return( new ScanCharResults( -1 )) ;
		}

		// --------------------------- ScanReverseEqual-------------------------------
		// scan for the first char equal to any of the pattern characters.
		public static ScanCharResults ScanReverseEqual(
			string InValue,
			int InBx,
			char[] InPatternChars )
		{
			int PatLx = InPatternChars.Length ;
			for( int Ix = InBx ; Ix > 0 ; --Ix )
			{
				char ch1 = InValue[Ix] ;
				for( int Jx = 0 ; Jx < PatLx ; ++Jx )
				{
					if ( ch1 == InPatternChars[Jx] )
						return( new ScanCharResults( ch1, Ix )) ;
				}
			}
			return( new ScanCharResults( -1 )) ;
		}

		// --------------------------- ScanReverseNotEqual-------------------------------
		// scan reverse for the first char not equal to all of the pattern characters.
		public static ScanCharResults ScanReverseNotEqual(
			string InValue,
			int InBx,
			char[] InPatternChars )
		{
			int PatLx = InPatternChars.Length ;
			for( int Ix = InBx ; Ix > 0 ; --Ix )
			{
				char ch1 = InValue[Ix] ;
				bool IsEqual = false ;
				for( int Jx = 0 ; Jx < PatLx ; ++Jx )
				{
					if ( ch1 == InPatternChars[Jx] )
					{
						IsEqual = true ;
						break ;
					}
				}
				if ( IsEqual == false )
					return( new ScanCharResults( ch1, Ix )) ;
			}
			return( new ScanCharResults( -1 )) ;
		}

		// ----------------------- ScanWord_CalcStartBx ---------------------------
		// calc start position from which to start scan to the next word.
		private static int ScanWord_CalcStartBx( string InString, WordCursor InWord )
		{
			int Bx ;
			switch( InWord.RelativePosition )
			{
				case AcRelativePosition.Begin:
					Bx = 0 ;
					break ;
				case AcRelativePosition.Before:
					Bx = InWord.ScanBx ;
					break ;
				case AcRelativePosition.After:
					Bx = InWord.ScanEx + 1 ;
					break ;
				case AcRelativePosition.End:
					Bx = InString.Length ;
					break ;
				case AcRelativePosition.None:
					Bx = 0 ;
					break ;
				case AcRelativePosition.At:
					Bx = InWord.ScanEx + 1 ;
					break ;
				default:
					Bx = 0 ;
					break ;
			}
			return Bx ;
		}

		// -------------------- ScanWord_IsolateDelim ---------------------------
		private static void ScanWord_IsolateDelim(
			string InString,
			int InBx,
			ref WordCursor InOutResults,
			TextTraits InTraits )
		{
			int Bx, Lx ;
			string delim ;

			// setup the start of the delim.
			if ( InOutResults.WordBx == -1 )
				Bx = InBx ; 
			else
				Bx = InOutResults.WordEx + 1 ;

			// word went to the end of the string.
			if ( Bx >= InString.Length )
			{
				Bx = -1 ;
			}

			// we have a delimiter of some kind.
			if ( Bx != -1 )
			{
				InOutResults.DelimIsWhitespace = false ;

				// the delim is a hard delim ( not whitespace )
				char ch1 = InString[Bx] ;
				if ( AcCommon.Contains( InTraits.WhitespaceChars, ch1 ) == false )
				{
					Lx = 1 ;
					delim = InString.Substring( Bx, Lx ) ;
					InOutResults.SetDelim( delim, Bx ) ;
				}

					// is a soft delim ( whitespace ). Look for hard delim after the ws.
				else
				{
					ScanCharResults scanResults =
						ScanNotEqual( InString, Bx, InTraits.WhitespaceChars ) ;
					if (( scanResults.ResultPos != -1 )
						&& ( AcCommon.Contains( InTraits.DelimChars, scanResults.ResultChar )))
					{
						Lx = 1 ;
						delim = AcCommon.CharToString( scanResults.ResultChar ) ;
						InOutResults.SetDelim( delim, scanResults.ResultPos ) ;
					}

						// the whitespace char is the delim of record.
					else
					{
						Lx = 1 ;
						delim = InString.Substring( Bx, Lx ) ;
						InOutResults.SetDelim( delim, Bx ) ;
						InOutResults.DelimIsWhitespace = true ;
					}
				}
			}
		}

		// -------------------- ScanWord_IsolateWord ---------------------------
		private static void ScanWord_IsolateWord(
			string InString,
			int InBx,
			ref WordCursor InOutResults,
			TextTraits InTraits )
		{
			int Bx, Fx, Ix, Lx ;
			string word ;

			Bx = InBx ;
			char ch1 = InString[Bx] ;

			// is quoted. the word runs to the closing quote.
			if ( IsOpenQuoteChar( ch1 ) == true )
			{
				Ix = ScanCloseQuote( InString, Bx, InTraits.QuoteEncapsulation ) ;
				if ( Ix == -1 )
					throw( new ApplicationException( "Closing quote not found starting at position " +
						Bx + " in " + InString )) ;
				Lx = Ix - Bx + 1 ;
				word = InString.Substring( Bx, Lx ) ;
				InOutResults.SetWord( word, WordClassification.Quoted, Bx ) ;
				return ;
			}

			// look for a brace or delim character.
			char[] combo = AcCommon.Concat( InTraits.DelimChars, InTraits.BraceChars ) ;
			ScanCharResults results = ScanEqual( InString, Bx, combo ) ;
			Fx = results.ResultPos ;
			ch1 = results.ResultChar ;

			// found a brace char
			if (( InTraits.IsOpenBraceChar( ch1 ) == true )
				&& ( InTraits.IsDelimChar( ch1 ) == false )) 
			{
				Ix = ScanCloseBrace( InString, Fx ) ;
				if ( Ix == -1 )
					throw( new ApplicationException( "Closing brace not found starting at position " +
						Fx + " in " + InString )) ;
				Lx = Ix - Bx + 1 ;
				word = InString.Substring( Bx, Lx ) ;
				if ( Bx == Fx )
					InOutResults.SetWord( word, WordClassification.Braced, Bx ) ;
				else
					InOutResults.SetWord( word, WordClassification.NameBraced, Bx, ch1 ) ;
			}
				
			// no delim found. all word to the end of the string.
			else if ( Fx == -1 )
			{
				word = InString.Substring( Bx ) ;
				InOutResults.SetWord( word, WordClassification.Name, Bx ) ;
			}

				// delim is same position as the word.  so there is no word, only a delim.
			else if ( Fx == Bx )
			{
				InOutResults.SetNullWord( ) ;
			}

				// we have a word that ends with a delim.
			else
			{
				Lx = Fx - Bx ;
				word = InString.Substring( Bx, Lx ) ;
				InOutResults.SetWord( word, WordClassification.Name, Bx ) ;
			}
		}
	
		// ------------------------ ScanFirstWord -------------------------
		public static WordCursor ScanFirstWord( string InString, TextTraits InTraits )
		{
			return( ScanNextWord( InString, new WordCursor( ), InTraits )) ;
		}

		// ------------------------ AdvanceNextWord -------------------------
		// Scans to the next word in the string. ( a word being the text bounded by the
		// delimeter and whitespace characters as spcfd in the TextTraits argument )
		// Return null when end of string.
		public static void AdvanceNextWord(
			string InString,
			WordCursor InOutWordCursor,
			TextTraits InTraits )
		{
			int Bx ;

			// calc scan start position
			Bx = ScanWord_CalcStartBx( InString, InOutWordCursor ) ;

			// empty the word parts of the cursor.
			InOutWordCursor.EmptyWord( ) ;

			// advance past whitespace
			if ( Bx < InString.Length )
				Bx = ScanNotEqual( InString, Bx, InTraits.WhitespaceChars ).ResultPos ;

			// got the start of something. scan for the delimeter ( could be the current char )
			if ( Bx < InString.Length )
			{
				ScanWord_IsolateWord( InString, Bx, ref InOutWordCursor, InTraits ) ;
			}

			// depending on the word, isolate and store the delim that follows.
			ScanWord_IsolateDelim( InString, Bx, ref InOutWordCursor, InTraits ) ;

			// current word position.
			if ( InOutWordCursor.ScanEx == -1 )
				InOutWordCursor.RelativePosition = AcRelativePosition.End ;
			else
				InOutWordCursor.RelativePosition = AcRelativePosition.At ;
		}

	} // end class Scanner
} // end namespace AutoCoder.Text
