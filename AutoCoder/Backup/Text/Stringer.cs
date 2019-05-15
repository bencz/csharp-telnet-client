using System;
using System.Text ;
using AutoCoder ;

namespace AutoCoder.Text
{

	// -------------------------- Stringer -----------------------------
	// class containing static functions that scan a text string
	public class Stringer
	{

		/// <summary>
		/// Examine the string for its WordClassification content.
		/// </summary>
		/// <param name="InWord"></param>
		/// <param name="InTraits"></param>
		/// <returns></returns>
		public static CharObjectPair CalcWordClassification(
			string InWord, TextTraits InTraits )
		{
			int Fx = 0, Ix = 0 ;
			WordClassification wc = WordClassification.None ;
			char braceChar = ' ' ;
			char ch1 = AcCommon.PullChar( InWord, 0 ) ;
			int Ex = InWord.Length - 1 ;

			// is quoted. the word runs to the closing quote.
			if ( Scanner.IsOpenQuoteChar( ch1 ) == true )
			{
				Ix = Scanner.ScanCloseQuote( InWord, 0, InTraits.QuoteEncapsulation ) ;
				if ( Ix == Ex )
					wc = WordClassification.Quoted ;
				else
					wc = WordClassification.MixedText ;
			}

			// check if the string is a Braced or NameBraced word.
			if ( wc == WordClassification.None )
			{
				char[] combo = AcCommon.Concat( InTraits.DelimChars, InTraits.BraceChars ) ;
				Scanner.ScanCharResults results = Scanner.ScanEqual( InWord, 0, combo ) ;
				Fx = results.ResultPos ;
				ch1 = results.ResultChar ;

				// found a brace char
				if (( InTraits.IsOpenBraceChar( ch1 ) == true )
					&& ( InTraits.IsDelimChar( ch1 ) == false )) 
				{
					Ix = Scanner.ScanCloseBrace( InWord, Fx ) ;
					if ( Ix == Ex )
					{
						braceChar = ch1 ;
						if ( Fx == 0 )
							wc = WordClassification.Braced ;
						else
							wc = WordClassification.NameBraced ;
					}
				}
			}

			// word is all delimeter.
			if ( wc == WordClassification.None )
			{
				Fx = Scanner.ScanNotEqual( InWord, 0, InTraits.DelimChars ).a ;
				if ( Fx >= 0 )
					wc = WordClassification.Delimeter ;
			}

			// check if a numeric string.
			if ( wc == WordClassification.None )
			{
				char[] digitChars =
					new char[] {'0', '1', '2', '3', '4', '5', '6', '7', '8', '9', '.', '+', '-' } ;
				Fx = Scanner.ScanNotEqual( InWord, 0, digitChars ).a ;
				if ( Fx == -1 )
				{
					wc = WordClassification.Numeric ;
					try
					{
						double vx = double.Parse( InWord ) ;
					}
					catch( Exception )
					{
						wc = WordClassification.None ;
					}
				}
			}

			// any delim chars in the string.  if not, the string is a name.  otherwise, it is
			// mixed.
			if ( wc == WordClassification.None )
			{
				Fx = Scanner.ScanEqual( InWord, 0, InTraits.DelimChars ).a ;
				if ( Fx == -1 )
					wc = WordClassification.Name ;
				else
					wc = WordClassification.MixedText ;
			}

			return new CharObjectPair( braceChar, wc ) ;
		}

		// --------------------------- Dequote ------------------------------
		public static string Dequote( string InText, QuoteEncapsulation InQem )
		{
			int Lx = InText.Length + 2 ;	// 2=quote chars. 
			char QuoteChar = InText[0] ;
			StringBuilder sb = new StringBuilder( Lx ) ;
			int Ix = 0 ;
			int EndIx = InText.Length - 2 ;
			while( true )
			{
				++Ix ;
				if ( Ix > EndIx )
					break ;
				char ch1 = InText[Ix] ;

				// using the escape method to enclose quote chars. This means the escape char
				// is used to encapsulate other special characters in the quoted string.
				// todo: dequote using "QuotedStringTraits" rules.
				if (( ch1 == '\\' )
					&& ( InQem == QuoteEncapsulation.Escape )
					&& ( Ix < ( EndIx - 1 )))
				{
					sb.Append( MaterializeEscapeChar( InText, Ix )) ;
					++Ix ;
				}

					// quote char enquoted using the "double the char" method.
				else if (( ch1 == QuoteChar ) &&
					( InQem == QuoteEncapsulation.Double ) &&
					( AcCommon.PullChar( InText, Ix + 1 ) == QuoteChar ))
				{
					sb.Append( ch1 ) ;
					++Ix ;
				}

					// any other character.  append to result string.
				else
					sb.Append( ch1 ) ;
			}
			return sb.ToString( ) ;
		}

		// ------------------------- EndsWith ------------------------------------
		// test if string ends with sub string
		public static bool EndsWith( string InValue, string InEndsWith )
		{
			int EndsLx = InEndsWith.Length ;
			int Bx = InValue.Length - EndsLx ;
			if (( Bx >= 0 ) &&
				( InValue.Substring( Bx, EndsLx ) == InEndsWith ))
				return true ;
			else
				return false ;
		}

		// ------------------------- EndsWith ------------------------------------
		// test if StringBuilder ends with sub string
		public static bool EndsWith( StringBuilder InValue, string InEndsWith )
		{
			int EndsLx = InEndsWith.Length ;
			int Bx = InValue.Length - EndsLx ;
			if (( Bx >= 0 ) &&
				( InValue.ToString( Bx, EndsLx ) == InEndsWith ))
				return true ;
			else
				return false ;
		}

		// ------------------------- Enquote ------------------------------
		public static string Enquote(	string InValue, char InQuoteChar )
		{
			return( Enquote( InValue, InQuoteChar, QuoteEncapsulation.Escape )) ;
		}

		// ------------------------- Enquote ------------------------------
		public static string Enquote(
			string InValue, char InQuoteChar, QuoteEncapsulation InQem )
		{
			int Lx = InValue.Length ;
			int Sx = ( Lx * 2 ) + 2 ;
			StringBuilder sb = new StringBuilder( Sx ) ;
			sb.Append( InQuoteChar ) ;
			for( int Ix = 0 ; Ix < Lx ; ++Ix )
			{
				char ch1 = InValue[Ix] ;
				if ( ch1 == InQuoteChar )
				{
					if ( InQem == QuoteEncapsulation.Double )
					{
						sb.Append( ch1 ) ;
						sb.Append( ch1 ) ;
					}
					else
					{
						sb.Append( @"\" ) ;
						sb.Append( ch1 ) ;
					}
				}
				else
          sb.Append( ch1 ) ;
			}
			sb.Append( InQuoteChar ) ;
			return sb.ToString( ) ;
		}

		// ------------------------------ GetNonNull -----------------------------
		// return the string value that is empty string if null value.
		public static string GetNonNull( string InValue )
		{
			if ( InValue == null )
				return "" ;
			else
				return InValue ;
		}

		/// <summary>
		/// Scan in string for any of the pattern characters.
		/// </summary>
		/// <param name="InValue"></param>
		/// <param name="InPatternChars"></param>
		/// <returns></returns>
		public static int IndexOf( string InValue, char[] InPatternChars )
		{
			return( Scanner.ScanEqual( InValue, 0, InPatternChars ).a ) ;
		}

		/// <summary>
		/// Scan in string for any of the pattern characters.
		/// </summary>
		/// <param name="InValue"></param>
		/// <param name="InBx"></param>
		/// <param name="InPatternChars"></param>
		/// <returns></returns>
		public static int IndexOf( string InValue, int InBx, char[] InPatternChars )
		{
			return( Scanner.ScanEqual( InValue, InBx, InPatternChars ).a ) ;
		}

		// ----------------------------- IsBlank ------------------------------
		// string is empty or all blanks
		public static bool IsBlank( string InValue )
		{
			if ( InValue == null )
				return true ;
			else if ( InValue.Length == 0 )
				return true ;
			else if ( InValue == "" )
				return true ;
			else
				return false ;
		}

		// --------------------------- IsNotBlank ---------------------------------
		public static bool IsNotBlank( string InValue )
		{
			return( !IsBlank( InValue )) ;
		}

		// ----------------------------- IsEmpty ------------------------------
		// string is null or zero length
		public static bool IsEmpty( string InValue )
		{
			if ( InValue == null )
				return true ;
			else if ( InValue.Length == 0 )
				return true ;
			else
				return false ;
		}

		// -------------------------- IsNotEmpty -----------------------------
		public static bool IsNotEmpty( string InValue )
		{
			return( !IsEmpty( InValue )) ;
		}

		// --------------------------- IsQuoted --------------------------------
		// is the string enclosed in quotes.  the quoted string must be properly formed.
		// 1st char is quote, last char is closing quote, and any enclosed quotes are
		// properly encapsulated as per the QuoteEncapsulationMethod.
		public static bool IsQuoted( string InString, QuoteEncapsulation InQem )
		{
			if ( InString.Length < 2 )
				return false ;
			char ch1 = InString[0] ;
			if (( ch1 != '"' ) && ( ch1 != '\'' ))
				return false ;
			int Fx = Scanner.ScanCloseQuote( InString, 0, InQem ) ;
			if (( Fx != -1 ) && ( Fx == InString.Length - 1 ))
				return true ;
			else
				return false ;
		}

		// ----------------------- MaterializeEscapeChar ---------------------
		// used by the Dequote method. unpacks the standard escape sequences used in
		// quoted strings.
		// returns an int/char pair holding the length of the escape sequence and the
		// materialized character value.
		private static IntCharPair MaterializeEscapeChar( string InString, int InIx )
		{
			char nx = AcCommon.PullCharArray( InString, InIx, 2 )[1] ;
			if ( nx == 't' )
				return new IntCharPair( 2, '\t' ) ;
			else if ( nx == 'r' )
				return new IntCharPair( 2, '\r' ) ;
			else if ( nx == 'n' )
				return new IntCharPair( 2, '\n' ) ;
			else if ( nx == '\'' )
				return new IntCharPair( 2, '\'' ) ;
			else if ( nx == '\\' )
				return new IntCharPair( 2, '\\' ) ;
			else if ( nx == '"' )
				return new IntCharPair( 2, '"' ) ;
			else if ( nx == '0' )
				return new IntCharPair( 2, '\0' ) ;
			else
				throw( new ApplicationException( "Unexpected escape sequence starting at " +
					"position " + InIx + " in string: " + InString )) ;
		}

		// ---------------------- Split --------------------------------
		// same as string.Split, only splits based on the occurance of string.
		public static string[] Split( string InString, string InPattern )
		{
			int Bx, Fx, Tx, Lx ;

			// first, talley the number of lines which will be split out.
			int Cx = Talley( InString, InPattern ) ;
			if ( Tail( InString, InPattern.Length ) != InPattern )
				++Cx ;

			// now we know how many lines will be needed.  alloc the string array.
			string[] results = new string[Cx] ;
			Tx = 0 ;

			// split out the strings.
			Bx = 0 ;
			while( true )
			{
				if ( Bx >= InString.Length )
					break ;

				// remaining string to split is shorter than the split pattern. Add this 
				// remainder to the results array and leave. 
				if ( Bx > ( InString.Length - InPattern.Length ))
				{
					results[Tx] = InString.Substring( Bx ) ;
					break ;
				}

				// no more pattern. from here to the end is the final string.
				Fx = InString.IndexOf( InPattern, Bx ) ;
				if ( Fx == -1 )
				{
					results[Tx] = InString.Substring( Bx ) ;
					break ;
				}

				// split out the string up to the split pattern.
				Lx = Fx - Bx ;
				if ( Lx == 0 )
					results[Tx] = "" ;
				else
					results[Tx] = InString.Substring( Bx, Lx ) ;
				++Tx ;

				// advance to the next split start position.
				Bx = Fx + InPattern.Length ;
			}

			return results ;
		}

		// ------------------------ Tail -----------------------------
		// return the ending xx number of chars from the StringBuilder
		public static string Tail( StringBuilder InString, int InTailLx )
		{
			int Lx = InTailLx ;
			if ( InString.Length < InTailLx )
				Lx = InString.Length ;
			if ( Lx == 0 )
				return "" ;
			else
			{
				int Bx = InString.Length - Lx ;
				return( InString.ToString( Bx, Lx )) ;
			}
		}

		// ------------------------ Tail -----------------------------
		// return the ending xx number of chars from the string.
		public static string Tail( string InString, int InTailLx )
		{
			int Lx = InTailLx ;
			if ( InString.Length < InTailLx )
				Lx = InString.Length ;
			if ( Lx == 0 )
				return "" ;
			else
			{
				int Bx = InString.Length - Lx ;
				return( InString.Substring( Bx, Lx )) ;
			}
		}

		// ---------------------------- Talley -----------------------------
		// count the number of occurances of pattern in string.
		// ( there is an issue of how much to advance the cursor after each find of the
		//   pattern. Either advance by 1 char, or advance by the length of the pattern.
		//   This method advances by length of the pattern. Use TalleyOverlap to advance
		//   by one char after each find. )
		public static int Talley( string InString, string InPattern )
		{
			int talleyCx = 0 ;
			int Bx = 0 ;
			while( true )
			{
				if ( Bx > ( InString.Length - InPattern.Length ))
					break ;
				int Fx = InString.IndexOf( InPattern, Bx ) ;
				if ( Fx == -1 )
					break ;
				++talleyCx ;
				Bx = Fx + InPattern.Length ;
			}
			return talleyCx ;
		}
		// ---------------------------- Talley -----------------------------
		// count the number of occurances of pattern in string.
		public static int Talley( string InString, char[] InPattern )
		{
			int talleyCx = 0 ;
			for( int Ix = 0 ; Ix < InString.Length ; ++Ix )
			{
				char ch1 = InString[Ix] ;
				for( int Jx = 0 ; Jx < InPattern.Length ; ++Jx )
				{
					if ( InPattern[Jx] == ch1 )
					{
						++talleyCx ;
						break ;
					}
				}
			}
			return talleyCx ;
		}

		// ----------------------- QuotedPrintableEncode --------------------
		public static string QuotedPrintableEncode( Encoding InEncoding, char InChar )
		{
			byte[] bytes = new byte[10] ;
			StringBuilder sb = new StringBuilder( 10 ) ;
			int ByteCx = InEncoding.GetBytes( "" + InChar, 0, 1, bytes, 0 ) ;
			for( int Ix = 0 ; Ix < ByteCx ; ++Ix )
			{
				if ( bytes[Ix] < 16 )
					sb.Append( "=0" + Convert.ToString( bytes[Ix], 16 ).ToUpper( )) ;
				else
					sb.Append( "=" + Convert.ToString( bytes[Ix], 16 ).ToUpper( )) ;
			}
			return sb.ToString( ) ;
		}

		// ----------------------- TrimStartAndEnd -----------------------
		public static string TrimStartAndEnd(
			string InString,
			char[] InTrimStartChars,
			char[] InTrimEndChars )
		{
			string OutString = InString.TrimStart( InTrimStartChars ) ;
			if ( OutString == null )
				return "" ;
			OutString = OutString.TrimEnd( InTrimEndChars ) ;
			if ( OutString == null )
				return "" ;
			else
				return OutString ;
		}
		
	} // end class Stringer
} // end namespace AutoCoder.Text
