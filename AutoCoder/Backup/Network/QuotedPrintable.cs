using System ;
using System.Text ;
using AutoCoder.Text ;

namespace AutoCoder.Network
{

	// --------------------- QuotedPrintableTraits ------------------------------
	// how to handle aspects of the encoding contents.
	public class QuotedPrintableTraits
	{
		QuotedPrintable.LinebreakTreatment mLbTreatment = QuotedPrintable.LinebreakTreatment.Break ;
		string mCharSet = "ISO-8859-1" ;
		Encoding mEncoder ;
		string mOtherEncodeChars = null ;
		int mRequiresEncodingTriggerLength = -1 ;
		bool mEncodeAlways = false ;

		// ---------------------- Properties ------------------------
		public QuotedPrintable.LinebreakTreatment LinebreakTreatment
		{
			get { return mLbTreatment ; }
			set { mLbTreatment = value ; }
		}
		public string CharSet
		{
			get { return mCharSet ; }
			set
			{
				mCharSet = value ;
				mEncoder = Encoding.GetEncoding( mCharSet ) ;
			}
		}

		public bool EncodeAlways
		{
			get { return mEncodeAlways ; }
			set { mEncodeAlways = value ; }
		}

		public Encoding Encoder
		{
			get
			{
				AssureEncoder( ) ;
				return mEncoder ; 
			}
			set { mEncoder = value ; }
		}

		// --------------------- RequiresEncodingTriggerLength ---------------------
		// length, which when exceeded, triggers a "RequiresEncoding" condition.
		public int RequiresEncodingTriggerLength
		{
			get { return mRequiresEncodingTriggerLength ; }
			set { mRequiresEncodingTriggerLength = value ; }
		}

		// ------------------ property - OtherEncodeChars --------------------
		// chars in addition to the standard characters to encode. 
		public string OtherEncodeChars
		{
			get { return mOtherEncodeChars ; }
			set { mOtherEncodeChars = value ; }
		}

		// -------------------------- AssureEncoder ---------------------
		private void AssureEncoder( )
		{
			if ( mEncoder == null )
				mEncoder = Encoding.GetEncoding( mCharSet ) ;
		}

		// -------------------------- SetEncoder ------------------------------
		public QuotedPrintableTraits SetEncoder( Encoding InEncoder )
		{
			mEncoder = InEncoder ;
			return this ; 
		}

		// --------------------- SetEncodeAlways ------------------------
		public QuotedPrintableTraits SetEncodeAlways( bool InValue )
		{
			EncodeAlways = InValue ;
			return this ;
		}

		// -------------------- SetLinebreakTreatment ----------------------
		public QuotedPrintableTraits SetLinebreakTreatment( QuotedPrintable.LinebreakTreatment InValue )
		{
			mLbTreatment = InValue ;
			return this ;
		}

		// ------------------- SetOtherEncodeChars ------------------------
		public QuotedPrintableTraits SetOtherEncodeChars( string InValue )
		{
			OtherEncodeChars = InValue ;
			return this ; 
		}

		// -------------------- SetCharSet ----------------------------
		public QuotedPrintableTraits SetCharSet( string InCharSet )
		{
			CharSet = InCharSet ;
			return this ; 
		}

		// ---------------- SetRequiresEncodingTriggerLength -----------------
		public QuotedPrintableTraits SetRequiresEncodingTriggerLength( int InValue )
		{
			RequiresEncodingTriggerLength = InValue ;
			return this ;
		}

	} // end class QuotedPrintableTraits

	// ------------------------- QuotedPrintable -------------------------
	// build string in QuotedPrintable form. 
	public class QuotedPrintable
	{
		public enum LinebreakTreatment { Break, Encode }  

		string mCharSet ;
		Encoding mEncoder ;

		// ------------------------- AssureEncoder -------------------------------
		void AssureEncoder( )
		{
			if ( mEncoder == null )
			{
				mEncoder = Encoding.GetEncoding( mCharSet ) ;
			}
		}

		// --------------------------- ConstructCommon -----------------------------
		void ConstructCommon( )
		{
			mCharSet = "ISO-8859-1" ;
			mEncoder = null ;
		}

		// -------------------------- CalcEncodedByteBounds ------------------------
		// An encoded byte can either be in form "=xx" or a single literal byte character.
		// The bounds of the encoded byte are either the single char, or run from the 
		// "=" character to the 2nd octet external form character.
		// This method is passed and encoded string and a position in that string. It returns,
		// by reference, the bounds of the encoded byte at that position.
		private static void CalcEncodedByteBounds(
			ref int OutBx,
			ref int OutEx,
			string InEncodedChars,
			int InIx )
		{
			// isolate chars prior to InIx.
			char[] chars = AcCommon.PullCharArray( InEncodedChars, InIx - 2, 3 ) ;
			
			// find the begin position of the encoded byte.
			if ( chars[2] == '=' )
				OutBx = InIx ;
			else if ( chars[1] == '=' )
				OutBx = InIx - 1 ;
			else if ( chars[0] == '=' )
				OutBx = InIx - 2 ;
			else
				OutBx = InIx ;

			// calc the end position of the encoded byte.
			if ( InEncodedChars[OutBx] == '=' )
				OutEx = OutBx + 2 ;
			else
				OutEx = OutBx ;
		}

		/// <summary>
		/// decode the quoted-printable encoded array of lines. Account for the QP
		/// line continuation character "=" found at the end of some lines.  
		/// </summary>
		/// <param name="InLines"></param>
		/// <returns></returns>
		public static string[] DecodeLines( string[] InLines )
		{
			// talley the number of resulting lines.
			int Sx = 0 ;
			for( int Ix = 0 ; Ix < InLines.Length ; ++Ix )
			{
				if ( Stringer.Tail( InLines[Ix], 1 ) != "=" )
					++Sx ;
			}

			// now we know how many lines will result from the decoding.
			string[] results = new string[Sx] ;
			int Tx = 0 ;

			// quoted-printable decode the lines.
			string wip = null ;
			string line = null ;
			for( int Ix = 0 ; Ix < InLines.Length ; ++Ix )
			{
				if ( wip == null )
					line = InLines[Ix] ;
				else
					line = wip + InLines[Ix] ;

				// does this line continue to the next
				if ( Stringer.Tail( line, 1 ) == "=" )
				{
					int Lx = line.Length - 1 ;
					wip = line.Substring( 0, Lx ) ;
				}
				else
				{

					// BgnTemp
					try
					{
						results[Tx] = DecodeString( line ) ;
					}
					catch( ApplicationException excp )
					{
						throw( new ApplicationException( "Quoted-Printable lines decode error. " +
							"Line " + Ix + " of " + InLines.Length, excp )) ;
					}
					// EndTemp

					++Tx ;
					wip = null ;
				}
			}

			// an odd case where the last line ended with an "=".  This line was not added
			// to the result array. This is badly formed data.
			if ( wip != null )
				throw( new ApplicationException(
					"Final quoted-printable encoded line is not correctly formed" )) ;

			return results ;
			}

		/// <summary>
		/// Decode the quoted-printable encoded string.
		/// Note: use DecodeLines to decode lines of text that include the QP line
		/// continuation character ( "=" ).  
		/// </summary>
		/// <param name="InString"></param>
		/// <returns></returns>
		public static string DecodeString( string InString )
		{
			StringBuilder sb = new StringBuilder( InString.Length ) ;
			for( int Ix = 0 ; Ix < InString.Length ; ++Ix )
			{
				char ch1 = InString[Ix] ;
				if ( ch1 != '=' )
					sb.Append( ch1 ) ;
				else
				{
					string hex = AcCommon.PullString( InString, Ix + 1, 2, " " ) ;

					// note: not an error if a lone "=" in a QP encoded string.
					if ( AcCommon.IsHexExternalForm( hex ) == false )
					{
						sb.Append( ch1 ) ;
					}

					else
					{
//						sb.Append( (char) Convert.ToInt32( hex, 16 )) ;
//						Ix += 2 ;

						byte singleByte = System.Convert.ToByte( hex, 16 ) ;
						byte[] bytes = AcCommon.ToByteArray( singleByte ) ;
						string hexChar = System.Text.Encoding.ASCII.GetString( bytes ) ;
						sb.Append( hexChar ) ;
						Ix += 2 ;
					}
				}
			}
			return sb.ToString( ) ;
		}

		// ------------------------- Encode -----------------------------------
		// Quoted-Printable encode a string.
		public static string Encode( string InValue, QuotedPrintableTraits InTraits )
		{
			// first pass.  encode one char at a time, without regard to 76 char line
			// limit.
			string im = EncodeChars( InValue, InTraits ) ;

			// split into max 76 char lines.
			string result = SplitEncodedResultsIntoLines( im ) ;

			return result ;
		}

		// ----------------------- EncodeByte --------------------
		// the basic primitive. Quoted-Printable encode a single byte.  
		public static string EncodeByte( byte InByte )
		{
			string result ;
			if ( InByte < 16 )
				result = "=0" + Convert.ToString( InByte, 16 ).ToUpper( ) ;
			else
				result = "=" + Convert.ToString( InByte, 16 ).ToUpper( ) ;
			return result ;
		}

		// ----------------------- EncodeChar ----------------------------------
		// Quoted-Printable encode a single unicode character.
		// ( depending on the charset, the unicode char could be encoded as more than
		//   one byte. )
		public static string EncodeChar( QuotedPrintableTraits InTraits, char InChar )
		{
			byte[] bytes = new byte[10] ;
			StringBuilder sb = new StringBuilder( 10 ) ;
			int ByteCx = InTraits.Encoder.GetBytes( "" + InChar, 0, 1, bytes, 0 ) ;
			for( int Ix = 0 ; Ix < ByteCx ; ++Ix )
			{
				sb.Append( EncodeByte( bytes[Ix] )) ;
			}
			return sb.ToString( ) ;
		}

		// ------------------------- EncodeChars -----------------------------------
		// Quoted-Printable encode the chars of a string without regard for the line
		// length maximum.
		private static string EncodeChars(
			string InValue, QuotedPrintableTraits InTraits )
		{
			char[] newLineChars = Environment.NewLine.ToCharArray( ) ;
			StringBuilder sb = new StringBuilder( InValue.Length * 2 ) ;

			// first pass.  encode one char at a time, without regard to 76 char line
			// limit.
			for( int Ix = 0 ; Ix < InValue.Length ; ++Ix )
			{
				char ch1 = InValue[Ix] ;

				// one of the "other" chars to encode.
				if (( InTraits.OtherEncodeChars != null )
					&& ( InTraits.OtherEncodeChars.IndexOf( ch1 ) != -1 ))
				{
					sb.Append( EncodeChar( InTraits, ch1 )) ;
				}

					// space or tab.  encoding depends on if followed by crlf or not.
				else if (( ch1 == 9 ) || ( ch1 == 32 ))
				{
					char[] nxChars =
						AcCommon.PullCharArray( InValue, Ix + 1, newLineChars.Length ) ;
					if (( InTraits.LinebreakTreatment == LinebreakTreatment.Break ) &&
						( AcCommon.CompareEqual( nxChars, newLineChars ) == true ))
						sb.Append( EncodeChar( InTraits, ch1 )) ;
					else
						sb.Append( ch1 ) ;
				}

					// Linebreak sequence handled as a line break.
				else if (( ch1 == newLineChars[0] ) &&
					( InTraits.LinebreakTreatment == LinebreakTreatment.Break ) &&
					( AcCommon.CompareEqual(
					newLineChars,
					AcCommon.PullCharArray( InValue, Ix, newLineChars.Length ))
					== true ))
					sb.Append( "\r\n" ) ;

					// a basic ascii char. literal representation.
				else if ((( ch1 >= 33 ) && ( ch1 <= 60 )) ||
					(( ch1 >= 62 ) && ( ch1 <= 126 )))
					sb.Append( ch1 ) ;

				else
					sb.Append( EncodeChar( InTraits, ch1 )) ;
			}

			return sb.ToString( ) ;
		}

		// ------------------------ RequiresEncoding -------------------------
		// evaluate if the string requires encoding according to the QuotedPrintableTraits.
		public static bool RequiresEncoding( string InValue, QuotedPrintableTraits InTraits )
		{
			bool requiresEncoding = false ;
			char[] newLineChars = Environment.NewLine.ToCharArray( ) ;

			// encode always.
			if ( InTraits.EncodeAlways == true )
			{
				requiresEncoding = true ;
			}

			// length of string exceeds the "RequiresEncodingTriggerLength"
			else if (( InTraits.RequiresEncodingTriggerLength != -1 )
				&& ( InValue.Length > InTraits.RequiresEncodingTriggerLength ))
			{
				requiresEncoding = true ;
			}

			// loop for each character in the string.  Test each to determine if the
			// string requires Quoted-Printable encoding.
			for( int Ix = 0 ; Ix < InValue.Length ; ++Ix )
			{
				if ( requiresEncoding == true )		break ;
				char ch1 = InValue[Ix] ;

				// one of the "other" chars to encode.
				if (( InTraits.OtherEncodeChars != null )
					&& ( InTraits.OtherEncodeChars.IndexOf( ch1 ) != -1 ))
				{
					requiresEncoding = true ;
				}

					// space or tab.  encoding depends on if followed by crlf or not.
				else if (( ch1 == 9 ) || ( ch1 == 32 ))
				{
					char[] nxChars =
						AcCommon.PullCharArray( InValue, Ix + 1, newLineChars.Length ) ;
					if (( InTraits.LinebreakTreatment == LinebreakTreatment.Break ) &&
						( AcCommon.CompareEqual( nxChars, newLineChars ) == true ))
						requiresEncoding = true ;
				}

					// LineBreak sequence handled as a line break.
				else if (( ch1 == newLineChars[0] ) &&
					( AcCommon.CompareEqual( newLineChars,
					AcCommon.PullCharArray( InValue, Ix, newLineChars.Length )) == true )) 
				{
					if ( InTraits.LinebreakTreatment == LinebreakTreatment.Encode )
						requiresEncoding = true ;
				}

					// a basic ascii char. literal representation.
				else if ((( ch1 >= 33 ) && ( ch1 <= 60 )) ||
					(( ch1 >= 62 ) && ( ch1 <= 126 )))
				{
				}

					// an equal sign.  by itself, does not trigger QP encoding.
				else if ( ch1 == '=' )
				{
				}

					// an encode required character.
				else
					requiresEncoding = true ;
			}

			return requiresEncoding ;
		}

		// ----------------------- SplitEncodedResultsIntoLines -----------------------
		static string SplitEncodedResultsIntoLines( string InResults )
		{
			StringBuilder sb = new StringBuilder( InResults.Length * 2 ) ;

			// split into max 76 char lines.
			int Ix = 0 ;
			while( Ix < InResults.Length )
			{
				int SegLx ;
				int RemLx = InResults.Length - Ix ;

				// remaining section of the encoded string fits on a single line.  
				if ( RemLx <= 76 )
				{
					SegLx = RemLx ;
					sb.Append( InResults.Substring( Ix, RemLx )) ;
				}

					// current section has to be split.  find encoded byte boundary and
					// add an "=\r\n" to the end of the line.
				else
				{
					int Fx = 0, Tx = 0 ;	// from and to of the encoded byte.
					int Ex = Ix + 75 - 1 ; // position of end of the encoded line.
					CalcEncodedByteBounds( ref Fx, ref Tx, InResults, Ex ) ;
					if ( Tx > Ex )	// the encoded byte runs past segment end. 
						Ex = Fx - 1 ; // the segment ends immed before this overhand encoded byte.
 
					// add the segment with a soft break to the results
					SegLx = Ex - Ix + 1 ;
					sb.Append( InResults.Substring( Ix, SegLx )) ;
					sb.Append( "=\r\n" ) ;
				}
				Ix += SegLx ;
			}
			return sb.ToString( ) ;
		}
	}
}
