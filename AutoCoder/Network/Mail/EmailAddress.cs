using System ;
using System.Text ;
using AutoCoder.Text ;
using AutoCoder.Scan;
using AutoCoder.Text.Enums;

namespace AutoCoder.Network.Mail
{
	// ----------------------- EmailAddress ---------------------------------
	public class EmailAddress
	{
		string 	mAddress ;
		string mFriendlyName ;
		string mComment ;

		// ----------------------- constructor ---------------------------------- 
		public EmailAddress( )
		{
			mAddress = null ;
			mFriendlyName = null ;
		}

		public EmailAddress( string InAddress )
		{
			mAddress = InAddress ;
			mFriendlyName = null ;
		}
		
		public EmailAddress( string InAddress, string InFriendlyName )
		{
			mAddress = InAddress ;
			mFriendlyName = InFriendlyName ;
		}

		public string Address
		{
			get { return mAddress ; }
			set { mAddress = value ; }
		}

		public string Comment
		{
			get { return mComment ; }
			set { mComment = value ; }
		}

		public string FriendlyName
		{
			get { return mFriendlyName ; }
			set { mFriendlyName = value ; }
		}

		// ------------------------- ParseAddressString ------------------------
		public static EmailAddress ParseAddressString( string InString )
		{
			TextTraits traits ;
			traits = new TextTraits( )
				.SetQuoteEncapsulation( QuoteEncapsulation.Escape ) ;
      traits.DividerPatterns.AddDistinct( 
        new string[] {" ", "\t"}, Text.Enums.DelimClassification.DividerSymbol);
			WordCursor bgnFriendly = null ;
			WordCursor endFriendly = null ;

			EmailAddress results = new EmailAddress( ) ;

			WordCursor csr = Scanner.PositionBeginWord( InString, traits ) ;
			while( true )
			{
				// advance to the next word in the address string. 
				csr = Scanner.ScanNextWord( InString, csr ) ;
				if ( csr.IsEndOfString )
					break ;

					// the email address itself is <braced>.
				else if (( csr.Word.Class == WordClassification.ContentBraced )
					&& ( csr.Word.BraceChar == '<' ))
					results.Address = csr.Word.BracedText ;
				
					// comment in the email address string.
				else if (( csr.Word.Class == WordClassification.ContentBraced )
					&& ( csr.Word.BraceChar == '(' ))
				{
					results.Comment = csr.Word.BracedText ;
					results.Comment = 
						MimeCommon.DecodeHeaderString_EncodedOnly( results.Comment ) ;
				}

					// word part of the friendly name in the address. extend the word range of
					// the friendly string.
				else
				{
					if ( bgnFriendly == null )
						bgnFriendly = csr ;
					endFriendly = csr ;
				}
			}

			// working from the word range, isolate the full friendly name string.
			string fullFriendly = null ;
			
			if (( bgnFriendly != null ) && ( bgnFriendly == endFriendly ))
				fullFriendly = bgnFriendly.Word.ToString( ) ;
			else if ( bgnFriendly != null )
			{
				int Bx = bgnFriendly.WordBx ;
				int Ex = endFriendly.WordEx ;
				fullFriendly = InString.Substring( Bx, Ex - Bx + 1 ) ;
			}

			// final decode of the friendly name.  name could be quoted, could contain
			// encoded-words.
			if ( fullFriendly != null )
				fullFriendly = MimeCommon.DecodeHeaderString_QuotedEncodedEither( fullFriendly ) ;

			// the friendly name could actually be the email address.
			if ( results.Address == null )
				results.Address = fullFriendly ;
			else
				results.FriendlyName = fullFriendly ;

			return results ;
		}

		// ----------------------- ToMimeString ---------------------------
		public string ToMimeString( string InCharSet )
		{
			string friendlyResults = null ;
			string addressResults = null ;
			string results = null ;

			// message line encode the friendly name.
			if ( Stringer.IsNotEmpty( FriendlyName ))
			{
				MimeHeaderLineBuilder friendly = new MimeHeaderLineBuilder( ) ;
				friendly.Traits
          .SetEncoderCharSet( InCharSet )
          .SetOtherEncodeChars( "<>\"\'" ) ;
				friendly.Append( FriendlyName ) ;
				friendlyResults = friendly.ToEncodedString( ) ;
			}

			// message line encode the email address
			MimeHeaderLineBuilder lb = new MimeHeaderLineBuilder( ) ;
			lb.Traits
				.SetEncoderCharSet( InCharSet ) ;
			lb.Append( " <" + Address + ">" ) ;
			addressResults = lb.ToEncodedString( ) ;

			results =
				MimeCommon.ConcatMessageLine( lb.Traits, friendlyResults, addressResults ) ;
			return results ;
		}

		// -------------------------- ToSendString --------------------------------
		public string ToSendString( )
		{
			StringBuilder sb = new StringBuilder( 512 ) ;
			if ( FriendlyName != null )
				sb.Append(
					Stringer.Enquote( FriendlyName, '"', QuoteEncapsulation.Escape ) +
					" " ) ;
			sb.Append( '<' ) ;
			sb.Append( Address ) ;
			sb.Append( '>' ) ;
			return sb.ToString( ) ;
		}

		// --------------------------- ToMessageHeaderString -------------------------
		// The email address and friendly name in the form specified for an internet 
		// message header. 
		// Use the CharSet to Quoted-Printable encode the friendly name if need be.
		public string ToMessageHeaderString( string InCharSet )
		{
			StringBuilder sb = new StringBuilder( 512 ) ;
			if ( Stringer.IsNotEmpty( FriendlyName ))
			{
				string mhs = null ;
				QuotedPrintableTraits traits =
					new QuotedPrintableTraits( )
					.SetCharSet( InCharSet )
					.SetOtherEncodeChars( "\"" ) ;
				mhs = MessageHeaderString.EncodeAsRequired( FriendlyName, traits ) ;
				sb.Append( mhs ) ;
        sb.Append( " <" + Address + ">" ) ;
			}
			else
			{
				sb.Append( "<" + Address + ">" ) ;
			}
			return sb.ToString( ) ;
		}

	} // end class EmailAddress
}