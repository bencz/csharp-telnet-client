using System;
using System.Text ;
using AutoCoder.Text ;

namespace AutoCoder
{

	public enum AcRelativePosition { Begin, Before, After, End, At, None }
	public enum AcReturnCode { Ok, Exception, None } ;

	// --------------------------- AcCommon ---------------------------------
	// kind of a class of general purpose statics.
	public class AcCommon
	{
		public AcCommon()
		{
		}

		// ---------------------- CalcCloseBraceChar -------------------------
		public static char CalcCloseBraceChar( char InOpenBraceChar )
		{
			switch( InOpenBraceChar )
			{
				case '(':
					return ')' ;
				case '{':
					return '}' ;
				case '<':
					return '>' ;
				case '[':
					return ']' ;
				default:
					throw(
						new ApplicationException(
						InOpenBraceChar + " is not a supported close brace character" )) ;
			}
		}

		// ------------------------- CharToString --------------------------
		public static string CharToString( char InValue )
		{
			char[] array = { InValue } ;
			return( new string( array )) ;
		}

		// --------------------- CompareEqual --------------------------------
		// compare two character arrays for equality
		public static bool CompareEqual( char[] InValue1, char[] InValue2 )
		{
			bool IsEqual = true ;
			int Lx = InValue1.Length ;
			if ( Lx != InValue1.Length )
				IsEqual = false ;
			else
			{
				for( int Ix = 0 ; Ix < Lx ; ++Ix )
				{
					if ( InValue1[Ix] != InValue2[Ix] )
					{
						IsEqual = false ;
						break ;
					}
				}
			}
			return IsEqual ;
		}

		/// <summary>
		/// Concatenate one char array to another.
		/// </summary>
		/// <param name="InValue1"></param>
		/// <param name="InValue2"></param>
		/// <returns></returns>
		public static char[] Concat( char[] InValue1, char[] InValue2 )
		{
			int Lx = InValue1.Length + InValue2.Length ;
			char[] result = new char[Lx] ;
			InValue1.CopyTo( result, 0 ) ;
			InValue2.CopyTo( result, InValue1.Length ) ;
			return result ;
		}

		/// <summary>
		/// Concatenate set of char arrays.
		/// </summary>
		/// <param name="InValue1"></param>
		/// <param name="InValue2"></param>
		/// <param name="InValue3"></param>
		/// <returns></returns>
		public static char[] Concat( char[] InValue1, char[] InValue2, char[] InValue3 )
		{
			int Lx = InValue1.Length + InValue2.Length + InValue3.Length ;
			char[] result = new char[Lx] ;
			InValue1.CopyTo( result, 0 ) ;
			InValue2.CopyTo( result, InValue1.Length ) ;
			InValue3.CopyTo( result, InValue1.Length + InValue2.Length ) ;
			return result ;
		}

		// ------------------------- Contains -----------------------------
		// does char array contain a char
		public static bool Contains( char[] InValue, char InPattern )
		{
			for ( int Ix = 0 ; Ix < InValue.Length ; ++Ix )
			{
				if ( InValue[Ix] == InPattern )
					return true ;
			}
			return false ;
		}

		// ---------------------------- EqualAny -------------------------------
		public static bool EqualAny( char InChar, char[] InAnyChar )
		{
			bool IsEqual = false ;
			int Sx = InAnyChar.Length ;
			for( int Ix = 0 ; Ix < Sx ; ++Ix )
			{
				if ( InChar == InAnyChar[Ix] )
				{
					IsEqual = true ;
					break ;
				}
			}
			return IsEqual ;
		}

		// ------------------------ IntValue ------------------------------
		public static int IntValue( object InValue )
		{
			if ( InValue == null )
				return 0 ;
			else
				return (int) InValue ;
		}

		/// <summary>
		/// Is the character a hex external form character, that is, is its value from
		/// 0-9 or A-F.
		/// </summary>
		/// <param name="InValue"></param>
		/// <returns></returns>
		public static bool IsHexExternalForm( char InValue )
		{
			if (( InValue >= '0' ) && ( InValue <= '9' ))
				return true ;
			else if (( InValue >= 'A' ) && ( InValue <= 'F' ))
				return true ;
			else
				return false ;
		}

		/// <summary>
		/// is the string compose entirely of an even number of hex external form
		/// characters. ( 0-9, A-F ).  
		/// </summary>
		/// <param name="InValue"></param>
		/// <returns></returns>
		public static bool IsHexExternalForm( string InValue )
		{
			if ( InValue.Length == 0 )
				return false ;
			else if (( InValue.Length % 2 ) == 1 )
				return false ;
			else
			{
				for( int Ix = 0 ; Ix < InValue.Length ; ++Ix )
				{
					if ( IsHexExternalForm( InValue[Ix] ) == false )
						return false ;
				}
				return true ;
			}
		}

		// -----------------------------NotEqualAll -----------------------------
		public static bool NotEqualAll( char InChar, char[] InAllChars )
		{
			bool IsNotEqual = true ;
			int Sx = InAllChars.Length ;
			for( int Ix = 0 ; Ix < Sx ; ++Ix )
			{
				if ( InChar == InAllChars[Ix] )
				{
					IsNotEqual = false ;
					break ;
				}
			}
			return IsNotEqual ;
		}

		// ------------------------ PullByteArray -------------------------
		// pull a set of bytes from byte[] array. 
		public static byte[] PullByteArray( byte[] InBytes, int InBx, int InLx )
		{
			byte[] result = new byte[InLx] ;
			int Ix = InBx ;
			for( int Tx = 0 ; Tx < InLx ; ++Tx )
			{
				if ( Ix < InBytes.Length )
					result[Tx] = InBytes[Ix] ;
				else
					result[Tx] = 0 ;
				++Ix ;
			}
			return result ;
		}

		// ------------------------ PullCharArray -------------------------
		// pull range of characters from string.
		// If individual character is out of bounds of the string, return null char.
		public static char[] PullCharArray( string InValue, int InBx, int InLx )
		{
			char[] result = new char[InLx] ;
			int Ix = InBx ;
			for( int Tx = 0 ; Tx < InLx ; ++Tx )
			{
				if ( Ix < 0 )
					result[Tx] = '\0' ;
				else if ( Ix >= InValue.Length )
					result[Tx] = '\0' ;
				else
					result[Tx] = InValue[Ix] ;
				++Ix ;
			}
			return result ;
		}

		// ------------------------ PullChar -------------------------
		// pull single character from string.
		// If individual character is out of bounds of the string, return null char.
		public static char PullChar( string InValue, int InIx )
		{
			char pullChar ;
			if ( InIx < 0 )
				pullChar = '\0' ;
			else if ( InIx >= InValue.Length )
				pullChar = '\0' ;
			else
				pullChar = InValue[InIx] ;
			return pullChar ;
		}

		// ------------------------ PullString -------------------------
		// pull substring from string.
		// Use the null substitution string in place of each out of bounds character.
		public static string PullString(
			string InValue, int InBx, int InLx, string InNull )
		{
			StringBuilder sb = new StringBuilder( InLx ) ;
			int Ix = InBx - 1 ;
			for( int x = 0 ; x < InLx ; ++x )
			{
				++Ix ;
				if (( Ix < 0 ) || ( Ix >= InValue.Length ))
				{
					if ( InNull != null )
						sb.Append( InNull ) ;
				}
				else
					sb.Append( InValue[Ix] ) ;
			}
			return sb.ToString( ) ;
		}

		// ------------------------ StringValue ------------------------------
		public static string StringValue( string InValue )
		{
			if ( InValue == null )
				return "" ;
			else
				return InValue ;
		}

		// ------------------------ StringValue ------------------------------
		public static string StringValue( object InValue )
		{
			if ( InValue == null )
				return "" ;
			else
				return (string) InValue ;
		}

		/// <summary>
		/// build byte array from single byte.
		/// </summary>
		/// <param name="InValue"></param>
		/// <returns></returns>
		public static byte[] ToByteArray( byte InValue )
		{
			byte[] byteArray = new byte[1] { InValue } ;
			return byteArray ;
		}

		/// <summary>
		/// Convert string array to comma sep value string form. 
		/// </summary>
		/// <param name="InArray"></param>
		/// <returns></returns>
		public static CsvString ToCsvString( string[] InArray )
		{
			CsvString tos = new CsvString( ) ;
			for( int Ix = 0 ; Ix < InArray.Length ; ++Ix )
			{
				tos.Add( InArray[Ix] ) ;
			}
			return tos ;
		}
	}
}
