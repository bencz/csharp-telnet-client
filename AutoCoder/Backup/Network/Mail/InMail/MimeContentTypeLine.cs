using System ;
using System.Collections ;
using AutoCoder.Text ;

namespace AutoCoder.Network.Mail.InMail
{
	// ----------------------- MimeContentTypeLine ------------------------------
	public class MimeContentTypexLine
	{
		string mLine ;
		string mContentType ;
		string mContentSubType ;
		string mCharSet ;
		string mBoundary ;
		TextTraits mTraits ;

		// --------------------------- properties --------------------------
		public string Boundary
		{
			get { return mBoundary ; }
		}

		// ------------------------ MimeContentTypeLine ----------------------------
		public MimeContentTypexLine( )
		{

		}

		// ------------------------ Load ----------------------------------
		public MimeContentTypexLine Load( string InLine )
		{
			// traits used when stepping word to word in the content-type line.
			mTraits = new TextTraits( )
				.SetDelimChars( "/:; \t=" )
				.SetWhitespaceChars( " \t" )
				.SetQuoteEncapsulation( QuoteEncapsulation.Escape ) ;

			mLine = InLine ;
			Parse( ) ;
			return this ;
		}

		// ------------------------- ParseValue_Boundary -----------------------
		private Scanner.WordCursor ParseValue_Boundary(
			Scanner.WordCursor InWordCsr )
		{
			Scanner.WordCursor csr = null ;

			// advance to the value after the boundary= kwd.
			csr = Scanner.ScanNextWord( mLine, InWordCsr, mTraits ) ;
			if ( csr.IsAtWord == true )
			{
				if ( csr.Word.WordIsQuoted == true )
					mBoundary = csr.Word.DequotedWord ;
				else
					mBoundary = csr.Word.ToString( ) ;
			}
			return csr ;
		}

		// ------------------------- ParseValue_CharSet -----------------------
		private Scanner.WordCursor ParseValue_CharSet(
			Scanner.WordCursor InWordCsr )
		{
			Scanner.WordCursor csr = null ;

			// advance to the value after the charset= kwd.
			csr = Scanner.ScanNextWord( mLine, InWordCsr, mTraits ) ;
			if ( csr.IsAtWord )
			{
				if ( csr.Word.WordIsQuoted == true )
					mCharSet = csr.Word.DequotedWord ;
				else
					mCharSet = csr.Word.ToString( ) ;
			}
			return csr ;
		}

		// ------------------------- ParseValue_ContentType -----------------------
		private Scanner.WordCursor ParseValue_ContentType(
			Scanner.WordCursor InCsr )
		{
			Scanner.WordCursor csr = InCsr ;
			while( true )
			{
				csr = Scanner.ScanNextWord( mLine, csr, mTraits ) ;
				if ( csr.IsEndOfString )
					break ;
				if ( csr.Delim == "/" )
					mContentType = csr.Word.ToString( ) ;
				else if ( csr.Delim == ";" )
				{
					if ( mContentType == null )
						mContentType = csr.Word.ToString( ) ;
					else
						mContentSubType = csr.Word.ToString( ) ;
					break ;
				}
			}
			return csr ;
		}

		// ------------------------------ Parse ----------------------------
		private void Parse( )
		{

			Scanner.WordCursor csr = Scanner.PositionBeginWord( ) ;
			while( true )
			{
				csr = Scanner.ScanNextWord( mLine, csr, mTraits ) ;
				if ( csr.IsEndOfString )
					break ;

				// content-type: type/subtype;
				if (( csr.Word.ToString( ).ToLower( ) == "content-type" )
					&& ( csr.Delim == ":" ))
					csr = ParseValue_ContentType( csr ) ;

				// boundary="value"
				else if (( csr.Word.ToString( ).ToLower( ) == "boundary" )
					&& ( csr.Delim == "=" ))
					csr = ParseValue_Boundary( csr ) ;

				// charset=us-ascii
				else if (( csr.Word.ToString( ).ToLower( ) == "charset" )
					&& ( csr.Delim == "=" ))
					csr = ParseValue_CharSet( csr ) ;
			}
		}

	} // end class MimeContentTypeLine 
}
