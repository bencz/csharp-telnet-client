using System;
using System.Text ;
using System.Collections.Generic;
using AutoCoder.Text ;

namespace AutoCoder
{

	// --------------------------- AcCommon ---------------------------------
	// kind of a class of general purpose statics.
	public static class AcCommon
	{

    // ------------------------ BoolValue ------------------------------
    public static bool BoolValue(object InValue)
    {
      if (InValue == null)
        return false ;
      else
        return (bool)InValue;
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
		/// Does char array contain a char
		/// </summary>
		/// <param name="InValue"></param>
		/// <param name="InPattern"></param>
		/// <returns></returns>
    public static bool Contains( char[] InValues, char InPattern )
		{
      bool doesContain = false;
      foreach( char vlu in InValues )
      {
        if (vlu == InPattern)
        {
          doesContain = true;
          break;
        }
      }
      return doesContain;
    }

    /// <summary>
    /// Does int[] contain a value.
    /// </summary>
    /// <param name="InValue"></param>
    /// <param name="InPattern"></param>
    /// <returns></returns>
    public static bool Contains(int[] InValues, int InPattern)
    {
      bool doesContain = false;
      foreach (char vlu in InValues)
      {
        if (vlu == InPattern)
        {
          doesContain = true;
          break;
        }
      }
      return doesContain;
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

    /// <summary>
    /// remove all items from the array that compare equal the find char.
    /// </summary>
    /// <param name="InArray"></param>
    /// <param name="InFindChar"></param>
    /// <returns></returns>
    public static char[] FindEqualAndRemoveAll(char[] InArray, char InFindChar)
    {
      char[] res = InArray;

      // first pass. count number of array items to remove.
      int rmvCx = 0 ;
      foreach (char ch1 in InArray)
      {
        if (ch1 == InFindChar)
          ++rmvCx;
      }

      // 2nd pass. remove the found items.
      if (rmvCx > 0)
      {
        res = new char[InArray.Length - rmvCx];
        int tx = 0;
        foreach (char ch1 in InArray)
        {
          if (ch1 != InFindChar)
          {
            res[tx] = ch1;
            ++tx;
          }
        }
      }

      return res;
    }

    /// <summary>
    /// return index of first array item which matches pattern.
    /// </summary>
    /// <param name="InValues"></param>
    /// <param name="InPattern"></param>
    /// <returns></returns>
    public static int IndexOf(int[] InValues, int InPattern)
    {
      int fx = -1;
      for (int ix = 0; ix < InValues.Length; ++ix)
      {
        if (InValues[ix] == InPattern)
        {
          fx = ix;
          break;
        }
      }
      return fx;
    }

    /// <summary>
    /// Return index of first array item which matches any of the
    /// pattern array values.
    /// </summary>
    /// <param name="InValues"></param>
    /// <param name="InPattern"></param>
    /// <returns></returns>
    public static int IndexOfAny(int[] InValues, int[] InPattern)
    {
      int fx = -1;
      for (int ix = 0; ix < InValues.Length; ++ix)
      {
        foreach (int patItem in InPattern)
        {
          if (patItem == InValues[ix])
          {
            fx = ix;
            break;
          }
        }
        if (fx >= 0)
          break;
      }
      return fx;
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

    /// <summary>
    /// parse a string that contains a date in yymmdd form.
    /// </summary>
    /// <param name="InDate"></param>
    /// <returns></returns>
    public static DateTime ParseDate_yymmdd(string InDate)
    {
      int yyyy = 0 ;
      int yy = Int32.Parse(InDate.Substring(0, 2));
      int mm = Int32.Parse(InDate.Substring(2, 2));
      int dd = Int32.Parse(InDate.Substring(4, 2));
      if ((yy == 0) && (mm == 0) && (dd == 0))
      {
        yyyy = 0001;
        mm = 1;
        dd = 1;
      }
      else
      {
        if (yy >= 80)
          yyyy = 1900 + yy;
        else
          yyyy = 2000 + yy;
      }
      return new DateTime(yyyy, mm, dd);
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
    /// <summary>
    /// Pull ( copy ) range of characters from a string
    /// </summary>
    /// <param name="InValue">String to pull from</param>
    /// <param name="InBx">Begin pos of chars to pull</param>
    /// <param name="InLx">Length of chars to pull</param>
    /// <returns>char[] of pulled chars</returns>
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
