using System ;
using System.Collections ;
using AutoCoder ;
using AutoCoder.Text ;
using AutoCoder.Scan;
using AutoCoder.Text.Enums;

namespace AutoCoder.Network.Mail.InMail
{

	// ------------------------ MimeParser --------------------------------
	// ( should move these methods to MimeCommon. )
	public class MimeParser
	{

		// ------------------------ CalcLineCode --------------------------------
		// calc the LineCode of the mime message line.  see the MimeLineCode enum.
		public static MimeLineCode CalcLineCode(
			string InLine, string InBdryText )
		{
			MimeLineCode code = MimeLineCode.None ;

			// a part boundary line
			if (( InLine.Length > 2 )
				&& ( InLine.Substring(0,2) == "--" )
				&& ( InBdryText != null ))
			{
				string beginBdry = "--" + InBdryText ;
				string endBdry = beginBdry + "--" ;
				if (( InLine.Length >= endBdry.Length )
					&& ( InLine.Substring( 0, endBdry.Length ) == endBdry ))
					code = MimeLineCode.PartEndBdry ;
				else if (( InLine.Length >= beginBdry.Length )
					&& ( InLine.Substring( 0, beginBdry.Length ) == beginBdry ))
					code = MimeLineCode.PartBdry ;
			}

			// check for the remaining line codes.
			if ( code != MimeLineCode.None )
			{	}

			else if ( InLine.Length == 0 )
				code = MimeLineCode.None ;

				// a property line starts with non blank, then that word ends with ":"
			else if (( MimeCommon.CharIsWhitespace( InLine[0] ) == false )
				&& ( ScanPropertyNameDelimChar( InLine ).ResultChar == ':' ))
				code = MimeLineCode.Property ;

			else
				code = MimeLineCode.Text ;

			return code ;
		}

		/// <summary>
		/// Parse the Received: property of the message part.
		/// </summary>
		/// <param name="InRawValue"></param>
		/// <returns></returns>
		public static PartProperty.Received ParsePartProperty_Received ( string InRawValue )
		{
			return( new PartProperty.Received( "", "", "", "", "" )) ; 
		}

		/// <summary>
		/// Split the string of mail addresses on the "," that separates them.
		/// </summary>
		/// <param name="InString"></param>
		/// <returns></returns>
		public static ArrayList SplitStringOfMailAddresses( string InString )
		{
			ArrayList addrList = new ArrayList( ) ;

      TextTraits traits;
      traits = new TextTraits()
        .SetQuoteEncapsulation(QuoteEncapsulation.Escape);
      traits.DividerPatterns.Replace(new string[] { ",", "\t" }, DelimClassification.DividerSymbol);
      traits.WhitespacePatterns.Replace( " ", "\t", DelimClassification.Whitespace ) ;

			WordCursor word = Scanner.PositionBeginWord( InString, traits ) ;
			while( true )
			{
				ObjectPair pair = ScanNextAddress( InString, word ) ;
				word = (WordCursor ) pair.b ;

				// got nothing. end of string.
				if ( pair.a == null )
					break ;

				// isolate the mail address string.
				string mailAddr = PullMailAddress( InString, pair ) ;
				
				// add the address string to list of such strings.
				addrList.Add( mailAddr ) ;
			}

			// return the split list of address strings.
			return addrList ;
		}

		// --------------------------- PullMailAddress ------------------------------
		private static string PullMailAddress( string InString, ObjectPair InPair )
		{
			WordCursor bgnAddrWord = (WordCursor) InPair.a ;
			WordCursor endAddrWord = (WordCursor) InPair.b ;

			int Bx = bgnAddrWord.WordBx ;
			int Ex = endAddrWord.WordEx ;

			if (( Bx == -1 ) || ( Ex == -1 ))
				throw( new ApplicationException(
					"email address not properly formed: " + InString )) ;

			return( InString.Substring( Bx, Ex - Bx + 1 )) ;
		}

		// ------------------------------ ScanNextAddress ---------------------------
		private static ObjectPair ScanNextAddress(
			string InString, WordCursor InWord )
		{
			TextTraits traits ;
      traits = new TextTraits()
        .SetQuoteEncapsulation(QuoteEncapsulation.Escape);
      traits.DividerPatterns.Replace(new string[] { ",", "\t" }, DelimClassification.DividerSymbol);
      traits.WhitespacePatterns.Replace( " ", "\t", DelimClassification.Whitespace );
      WordCursor bgnAddrWord = null;
			WordCursor endAddrWord = null ;

			// advance from word to word in the string until the comma between addresses 
			// or the end of the string.
			WordCursor word = InWord ;
      word.TextTraits = traits;
			while( true )
			{
				word = Scanner.ScanNextWord( InString, word ) ;
				if ( word.IsEndOfString )
					break ;
				
				// expand the word range of the current mail address string.
				if ( bgnAddrWord == null )
					bgnAddrWord = word ;
				endAddrWord = word ;

				if ( word.DelimValue == "," )	// end of this address.
					break ;
				if ( word.DelimValue == "" )		// end of the string.
					break ;
			}

			return( new ObjectPair( bgnAddrWord,endAddrWord )) ;
		}

		/// <summary>
		/// split and parse a string of comma separated email addresses. Return as a
		/// list of EmailAddress objects.  
		/// </summary>
		/// <param name="InString"></param>
		/// <returns></returns>
		public static ArrayList ParseStringOfEmailAddresses( string InString )
		{
			ArrayList addrList = new ArrayList( ) ;

			ArrayList addrStrings = SplitStringOfMailAddresses( InString ) ;
			foreach( string addrString in addrStrings )
			{
				EmailAddress addr = EmailAddress.ParseAddressString( addrString ) ;
				addrList.Add( addr ) ;
			}

			return addrList ;
 		}

		// ------------------------- ParseContentDisposition ----------------------------
		public static PartProperty.ContentDisposition
			ParseContentDisposition( string InString )
		{
      TextTraits traits = new TextTraits()
        .SetQuoteEncapsulation(QuoteEncapsulation.Escape);
      traits.DividerPatterns.Replace(
        new string[] { ";", " ", "\t", "=" }, DelimClassification.DividerSymbol);
      traits.WhitespacePatterns.Replace(" ", "\t", DelimClassification.Whitespace );
      PartProperty.ContentDisposition results = new PartProperty.ContentDisposition();

			WordCursor csr = Scanner.PositionBeginWord( InString, traits ) ;
			while( true )
			{
				csr = Scanner.ScanNextWord( InString, csr ) ;
				if ( csr.IsEndOfString )
					break ;

				// content disposition
				if ( csr.DelimValue == ";" )
					results.Disposition = csr.Word.ToString( ).ToLower( ) ;

					// a kwd
				else if ( csr.DelimValue == "=" )
				{
					WordCursor nxCsr = csr.NextWord( ) ;
					if (( nxCsr.DelimClass == DelimClassification.EndOfString )
						|| ( nxCsr.DelimClass == DelimClassification.Whitespace ))
					{
						string kwd = csr.Word.ToString( ).ToLower( ) ;
						csr = nxCsr ;
						if ( kwd == "filename" )
							results.FileName = csr.Word.NonQuotedSimpleValue ;
					}				}
			}
			return results ;
		}

		// ------------------------- ParseContentType ----------------------------
		public static PartProperty.ContentType ParseContentType( string InString )
		{
      TextTraits traits = new TextTraits()
        .SetQuoteEncapsulation(QuoteEncapsulation.Escape);
      traits.DividerPatterns.Replace(
        new string[] { "/", ":", ";", " ", "\t", "=" }, DelimClassification.DividerSymbol);
      traits.WhitespacePatterns.Replace( " ", "\t", DelimClassification.Whitespace );
      PartProperty.ContentType results = new PartProperty.ContentType();

			WordCursor csr = Scanner.PositionBeginWord( InString, traits ) ;
			while( true )
			{
				csr = Scanner.ScanNextWord( InString, csr ) ;
				if ( csr.IsEndOfString )
					break ;

				// content type
				if ( csr.DelimValue == "/" )
					results.Type = csr.Word.ToString( ).ToLower( ) ;

					// content sub type.
				else if ( csr.DelimValue == ";" )
					results.SubType = csr.Word.ToString( ).ToLower( ) ;

					// a kwd
				else if ( csr.DelimValue == "=" )
				{
					WordCursor nxCsr = csr.NextWord( ) ;
					if (( nxCsr.DelimClass == DelimClassification.EndOfString )
						|| ( nxCsr.DelimClass == DelimClassification.Whitespace ))
					{
						string kwd = csr.Word.ToString( ).ToLower( ) ;
						csr = nxCsr ;
						if ( kwd == "charset" )
							results.CharSet = csr.Word.NonQuotedSimpleValue ;
						else if ( kwd == "boundary" )
							results.Boundary = csr.Word.NonQuotedSimpleValue ;
						else if ( kwd == "name" )
							results.Name = csr.Word.NonQuotedSimpleValue ;
					}
				}
			}
			return results ;
		}

		// ------------------------- ScanPropertyNameDelimChar -------------------------
		private static Scanner.ScanCharResults ScanPropertyNameDelimChar( string InLine )
		{
			char[] pattern = { ' ', '\t', ':' } ;
			return( Scanner.ScanEqualAny( InLine, pattern )) ;
		}

		// ------------------------ SplitPropertyLine -----------------------------
		public static StringPair SplitPropertyLine( string InLine )
		{
			StringPair pair = new StringPair( ) ;
			int Fx = ScanPropertyNameDelimChar( InLine ).ResultPos ;

			// the property name are the characters up to the ":"
			pair.a = InLine.Substring( 0, Fx ).Trim( ) ;
			int Bx = Fx + 1 ;

			// everything after the ":" is the property value. 
			if ( Bx >= InLine.Length )
				pair.b = "" ;
			else
			{
				pair.b = InLine.Substring( Bx ).Trim( ) ;
			}

			return pair ;
		}
		
	}
}
