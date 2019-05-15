using System ;
using System.Collections ;
using AutoCoder.Text ;
using AutoCoder.Scan;

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
      mTraits = new TextTraits()
        .SetQuoteEncapsulation(QuoteEncapsulation.Escape);

      mTraits.DividerPatterns.AddDistinct(new ScanPatterns(
        new string[] { "/", ":", ";", " ", "\t", "=" }, 
        Text.Enums.DelimClassification.DividerSymbol));

      mTraits.WhitespacePatterns.Replace(" ", "\t", Text.Enums.DelimClassification.Whitespace ) ;
			
      mLine = InLine ;
			Parse( ) ;
			return this ;
		}

		// ------------------------- ParseValue_Boundary -----------------------
		private WordCursor ParseValue_Boundary(
			WordCursor InWordCsr )
		{
			WordCursor csr = null ;

			// advance to the value after the boundary= kwd.
			csr = Scanner.ScanNextWord( mLine, InWordCsr ) ;
			if ( csr.IsDelimOnly == false )
			{
				if ( csr.Word.IsQuoted == true )
					mBoundary = csr.Word.DequotedWord ;
				else
					mBoundary = csr.Word.ToString( ) ;
			}
			return csr ;
		}

		// ------------------------- ParseValue_CharSet -----------------------
		private WordCursor ParseValue_CharSet(
			WordCursor InWordCsr )
		{
			WordCursor csr = null ;

			// advance to the value after the charset= kwd.
			csr = Scanner.ScanNextWord( mLine, InWordCsr ) ;
			if ( csr.IsAtWord )
			{
				if ( csr.Word.IsQuoted == true )
					mCharSet = csr.Word.DequotedWord ;
				else
					mCharSet = csr.Word.ToString( ) ;
			}
			return csr ;
		}

		// ------------------------- ParseValue_ContentType -----------------------
		private WordCursor ParseValue_ContentType(
			WordCursor InCsr )
		{
			WordCursor csr = InCsr ;
      csr.TextTraits = mTraits;
			while( true )
			{
				csr = Scanner.ScanNextWord( mLine, csr ) ;
				if ( csr.IsEndOfString )
					break ;
				if ( csr.DelimValue == "/" )
					mContentType = csr.Word.ToString( ) ;
				else if ( csr.DelimValue == ";" )
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

			WordCursor csr = Scanner.PositionBeginWord( mLine, mTraits ) ;
			while( true )
			{
				csr = Scanner.ScanNextWord( mLine, csr ) ;
				if ( csr.IsEndOfString )
					break ;

				// content-type: type/subtype;
				if (( csr.Word.ToString( ).ToLower( ) == "content-type" )
					&& ( csr.DelimValue == ":" ))
					csr = ParseValue_ContentType( csr ) ;

				// boundary="value"
				else if (( csr.Word.ToString( ).ToLower( ) == "boundary" )
					&& ( csr.DelimValue == "=" ))
					csr = ParseValue_Boundary( csr ) ;

				// charset=us-ascii
				else if (( csr.Word.ToString( ).ToLower( ) == "charset" )
					&& ( csr.DelimValue == "=" ))
					csr = ParseValue_CharSet( csr ) ;
			}
		}

	} // end class MimeContentTypeLine 
}
