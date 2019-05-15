using System;
using System.Text ;
using AutoCoder.Text ;

namespace AutoCoder.Network
{
	// ----------------------- MimeEncodedWord -------------------------------
	// as per rfc1522, an encoded-word is Quoted-Printable encoded contents wrapped in the
	// form spcfd in the rfc that enables mail agents to identify and decode the contents.
	public class MimeEncodedWord
	{
		// data members used when class is returned by 
		public int Bx ;
		public int Lx ;
		public string CharSet ;
		public string DecodedValue ;

		// --------------------- CalcQuotedPrintableTraits ------------------------
		// Setup an object containing the traits for the Quoted-Printable encoding of
		// contents of the MimeHeaderLine.
		private static QuotedPrintableTraits CalcQuotedPrintableTraits(
			MimeHeaderLineTraits InTraits )
		{
			string otherEncodeChars = " \t\n\r?" ;
			QuotedPrintableTraits qpTraits = new QuotedPrintableTraits( ) ;
			qpTraits.SetEncoder( InTraits.Encoder ) ;
			if ( InTraits.OtherEncodeChars != null )
				otherEncodeChars = otherEncodeChars + InTraits.OtherEncodeChars ;
			qpTraits.SetOtherEncodeChars( otherEncodeChars ) ;
			return qpTraits ;
		}


		public static string Encode( string InValue, MimeHeaderLineTraits InHdrTraits )
		{
			QuotedPrintableTraits qpTraits =
				CalcQuotedPrintableTraits( InHdrTraits ) ;
			string ec = QuotedPrintable.Encode( InValue, qpTraits ) ;
			string results = "=?" + InHdrTraits.EncoderCharSet + "?Q?" + ec + "?=" ;
			return results ;
		}

		/// <summary>
		/// Crack the component parts of the encoded-word string starting at InBx
		/// in the string. Return an object pair. pair.a is a bool set to false if the
		/// encoded-word string is not correctly formed. pair.b is a MimeEncodedWord object
		/// containing the component parts of the cracked word.
		/// </summary>
		/// <param name="InString"></param>
		/// <param name="InBx"></param>
		/// <returns></returns>
		public static BoolObjectPair CrackEncodedWord( string InString, int InBx )
		{
			int Fx, Ix, Lx, RemLx ;
			string ws = null ;
			MimeEncodedWord ew = new MimeEncodedWord( ) ;
			bool isEncodedWord = true ;

			try
			{

				// isolate the next 80 chars from the string as a workspace ( encoded words are
				// limited to 75 chars or less )
				ew.Bx = InBx ;
				ws = AcCommon.PullString( InString, InBx, 80, null ).ToLower( ) ;
				Ix = 0 ;

				// isolate the charset name
				Ix = 2 ;
				if ( isEncodedWord == true )
				{
					RemLx = ws.Length - Ix ;
					if ( RemLx <= 3 )
						isEncodedWord = false ;
					else
					{
						Fx = ws.IndexOf( "?q?", Ix ) ;
						if ( Fx == -1 )
							isEncodedWord = false ;
						else
						{
							Lx = Fx - Ix ;
							ew.CharSet = InString.Substring( InBx + Ix, Lx ) ;
							Ix = Fx + 3 ;
						}
					}
				}

				// quoted-printable encoded text runs until "?="
				if ( isEncodedWord == true )
				{
					RemLx = ws.Length - Ix ;
					if ( RemLx <= 2 )
						isEncodedWord = false ;
					else
					{
						Fx = ws.IndexOf( "?=", Ix ) ;
						if ( Fx == -1 )
							isEncodedWord = false ;
						else
						{
							Lx = Fx - Ix ;
							string qpEncoded = InString.Substring( InBx + Ix, Lx ) ;
							ew.DecodedValue = QuotedPrintable.DecodeString( qpEncoded ) ;
							ew.Lx = Fx + 2 ;
						}
					}
				}
			}
			catch( Exception )
			{
				isEncodedWord = false ;
			}
			return( new BoolObjectPair( isEncodedWord, ew )) ;
		}

		/// <summary>
		/// Is the position in the string the start of an encoded-word
		/// </summary>
		/// <param name="InString">String to examine</param>
		/// <param name="InBx">Position in string</param>
		/// <returns></returns>
		public static bool IsStartOfEncodedWord( string InString, int InBx )
		{
			bool isEncodedWord = true ;
			if ( AcCommon.PullString( InString, InBx, 2, null ) != "=?" )
				isEncodedWord = false ;
			else
			{
				isEncodedWord = CrackEncodedWord( InString, InBx ).a ;
			}

			return isEncodedWord ;
		}

	}
}
