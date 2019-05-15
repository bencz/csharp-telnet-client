using System ;
using System.Text ;
using AutoCoder.Text ;
using AutoCoder.Network.Mail.InMail ;

namespace AutoCoder.Network.Mail
{

	// -------------------------- MimeCommon -----------------------------
	public class MimeCommon
	{
		public MimeCommon( )
		{
		}

		// ------------------------ CharIsWhitespace ---------------------------
		public static bool CharIsWhitespace( char InChar )
		{
			if (( InChar == ' ' )
				|| ( InChar == '\t' ))
				return true ;
			else
				return false ;
		}

		// ------------------------ ConcatMessageLine ---------------------------
		public static string ConcatMessageLine(
			MimeHeaderLineTraits InTraits,
			string InValue1,
			string InValue2 )
		{
			return( ConcatMessageLine( InTraits, InValue1, InValue2, null, null )) ;
		}

		// ------------------------ ConcatMessageLine ---------------------------
		public static string ConcatMessageLine(
			MimeHeaderLineTraits InTraits,
			string InValue1,
			string InValue2,
			string InValue3 )
		{
			return( ConcatMessageLine( InTraits, InValue1, InValue2, InValue3, null )) ;
		}

		// ------------------------ ConcatMessageLine ---------------------------
		public static string ConcatMessageLine(
			MimeHeaderLineTraits InTraits,
			string InValue1,
			string InValue2,
			string InValue3,
			string InValue4 )
		{
			StringBuilder sb = new StringBuilder( 2000 ) ;

			if ( InValue1 != null )
				ConcatMessageLine_Value( InTraits, sb, InValue1 ) ;
			if ( InValue2 != null )
				ConcatMessageLine_Value( InTraits, sb, InValue2 ) ;
			if ( InValue3 != null )
				ConcatMessageLine_Value( InTraits, sb, InValue3 ) ;
			if ( InValue4 != null )
				ConcatMessageLine_Value( InTraits, sb, InValue4 ) ;

			return sb.ToString( ) ;
		}

		// ---------------------- ConcatMessageLine_Value ---------------------
		private static void ConcatMessageLine_Value(
			MimeHeaderLineTraits InTraits,
			StringBuilder InMessageLine,
			string InConcatValue )
		{
			int Lx = InMessageLine.Length + 1 + InConcatValue.Length ;
			if ( InConcatValue == MimeConstants.CrLf )
				InMessageLine.Append( InConcatValue ) ;
			else if ( InMessageLine.Length == 0 )
				InMessageLine.Append( InConcatValue ) ;
			else if ( Lx > InTraits.LineDesiredMaxLength )
				InMessageLine.Append( MimeConstants.Fold + InConcatValue ) ;
			else
				InMessageLine.Append( " " + InConcatValue ) ;
		}

		// ---------------- DecodeHeaderString_EncodedOnly --------------------------
		// decode the string that is a mix of encoded-words and asis text.
		public static string DecodeHeaderString_EncodedOnly( string InString )
		{
			StringBuilder sb = new StringBuilder( ) ;
			int Lx = InString.Length ;
			for( int Ix = 0 ; Ix < Lx ; ++Ix )
			{
				char ch1 = InString[Ix] ;
				if (( ch1 == '=' )
					&& ( MimeEncodedWord.IsStartOfEncodedWord( InString, Ix ) == true ))
				{
					MimeEncodedWord ew =
						(MimeEncodedWord) MimeEncodedWord.CrackEncodedWord( InString, Ix ).b ;
					sb.Append( ew.DecodedValue ) ;
					Ix += ew.Lx ;
					--Ix ;
				}
				else
					sb.Append( ch1 ) ;
			}
			return sb.ToString( ) ;
		}

		// ---------------- DecodeHeaderString_QuotedEncodedBoth --------------------------
		// decode the string that can be both quoted and contain encoded-words.
		public static string DecodeHeaderString_QuotedEncodedBoth( string InString )
		{
			string results = DecodeHeaderString_QuotedOnly( InString ) ;
			results = DecodeHeaderString_EncodedOnly( results ) ;
			return results ;
		}

		// ---------------- DecodeHeaderString_QuotedEncodedEither --------------------------
		// decode the string that can be either quoted or contain encoded-words.
		public static string DecodeHeaderString_QuotedEncodedEither( string InString )
		{
			string results = null ;
			QuoteEncapsulation qem = QuoteEncapsulation.Escape ;
			if ( Stringer.IsQuoted( InString, qem ) == true )
				results = DecodeHeaderString_QuotedOnly( InString ) ;
			else
				results = DecodeHeaderString_EncodedOnly( InString ) ;
			return results ;
		}

		// ---------------- DecodeHeaderString_QuotedOnly --------------------------
		// decode the header string.  If it is quoted, dequote it. Otherwise, return as is.
		public static string DecodeHeaderString_QuotedOnly( string InString )
		{
			QuoteEncapsulation qem = QuoteEncapsulation.Escape ;
			string results = null ;
			if ( Stringer.IsQuoted( InString, qem ) == true )
			{
				results = Stringer.Dequote( InString, qem ) ;
			}
			else
				results = InString ;
			return results ;
		}

		// ------------------------- MessageLineSplitter --------------------------
		// split the mime encoded message into its text line contents.
		public static string[] MessageLineSplitter( string InStream )
		{
			string unfoldedStream = Unfold( InStream ) ;
			string[] results = Stringer.Split( unfoldedStream, "\r\n" ) ;
			return results ;
		}
 
		// -------------------------- MessagePartSplitter -----------------------
		// input is an array holding the unfolded lines of the message.
		// Build and return a list of MimeMessagePart objects.  Each part object holds the
		// lines of a part of the message. 
		// ( a part is either the header part or the boundary string delimited content parts.
		public static InMail.MimeMessagePartList MessagePartSplitter( string[] InLines )
		{
			InMail.MimeMessagePartList parts = new InMail.MimeMessagePartList( ) ;
			int curPartBx = -1 ;
			int curPartCx = 0 ;
			MimePartCode curPartCode = MimePartCode.Top ;
			bool currentlyBetweenParts = false ;
			bool partIsJustStarting = true ;
			StringStack bdryStack = new StringStack( ) ;

			for( int Ix = 0 ; Ix < InLines.Length ; ++Ix )
			{
				string line = InLines[Ix] ;
				MimeLineCode lc = MimeParser.CalcLineCode( line, bdryStack.GetTop( )) ;

				switch( lc )
				{
					case MimeLineCode.Property:
					{
						StringPair propPair = MimeParser.SplitPropertyLine( line ) ;

						// the content-type property.  Could have an boundary="xxxxx" element. 
						// Boundary strings in a mime document have scope. That is, within one 
						// boundary ( section ) of the document, there can be sub boundaries
						// ( sub sections )
						if ( propPair.a.ToLower( ) == "content-type" )
						{
							PartProperty.ContentType ct = 
								MimeParser.ParseContentType( propPair.b ) ;
							if (( ct.Boundary != null ) && ( ct.Boundary != "" ))
								bdryStack.Push( ct.Boundary ) ;
						}
						break ;
					}

						// part boundary line. load the lines of the current part and reset the line
						// counter of this new part.
					case MimeLineCode.PartBdry:
						if ( curPartBx != -1 )
						{
							parts.AddNewPart( curPartCode )
								.LoadLines( InLines, curPartBx, curPartCx ) ;
						}
						curPartCx = 0 ;
						curPartBx = -1 ;
						curPartCode = MimePartCode.Content ;
						currentlyBetweenParts = false ;
						partIsJustStarting = true ;
						break ;

					case MimeLineCode.PartEndBdry:
						if ( curPartBx != -1 )
						{
							parts.AddNewPart( curPartCode )
								.LoadLines( InLines, curPartBx, curPartCx ) ;
						}
						curPartCx = 0 ;
						curPartBx = -1 ;
						if ( bdryStack.IsNotEmpty )
							bdryStack.Pop( ) ;
						currentlyBetweenParts = true ;
						break ;

					default:
						break ;
				}

				// add to range of lines in the current part.
				if (( currentlyBetweenParts == false ) && ( lc != MimeLineCode.PartBdry ))
				{
					if (( partIsJustStarting == true ) && ( line == "" ))
					{
					}
					else
					{
						++curPartCx ;
						if ( curPartBx == -1 )
							curPartBx = Ix ;
					}
					partIsJustStarting = false ;
				}
			}

			// load the lines of the final in progress part.
			if ( curPartBx != -1 )
			{
				parts.AddNewPart( curPartCode )
					.LoadLines( InLines, curPartBx, curPartCx ) ;
			}

			return parts ;
		}
 
		// -------------------------- MessagePartSplitter -----------------------
		// input is an array holding the unfolded lines of the message.
		// Build and return a list of MimeMessagePart objects.  Each part object holds the
		// lines of a part of the message. 
		// ( a part is either the header part or the boundary string delimited content parts.
		public static InMail.MimeMessagePartList MessagePartSplitter( string InStream )
		{
			InMail.MimeMessagePartList parts = new InMail.MimeMessagePartList( ) ;
			bool currentlyBetweenParts = false ;
			StringStack bdryStack = new StringStack( ) ;

			int Ex = -1 ;
			string line = null ;
			MimeMessagePart.PartInProgress pip =
				new MimeMessagePart.PartInProgress( MimePartCode.Top ) ;

			while( true )
			{
				// advance in string. 
				int lineBx = Ex + 1 ;
				if ( lineBx >= InStream.Length )
					break ;

				// the next line in the stream. recognize folds depending on if currently within
				// a part header or within the message lines of the part ( no folding there )
				if ( pip.CurrentlyAmoungPropertyLines == true )
				{
					IntStringPair rv = ScanEndUnfoldedLine( InStream, lineBx ) ;
					Ex = rv.a ;
					line = rv.b ;
				}
				else
				{
					IntStringPair rv = ScanEndLine( InStream, lineBx ) ;
					Ex = rv.a ;
					line = rv.b ;
				}

				// calc what kind of a line in the mime document we have here. 
				MimeLineCode lc = MimeParser.CalcLineCode( line, bdryStack.GetTop( )) ;
				if (( lc == MimeLineCode.Property )
					&& ( pip.CurrentlyAmoungMessageLines == true ))
					lc = MimeLineCode.Text ;

				switch( lc )
				{
					case MimeLineCode.Property:
					{
						StringPair propPair = MimeParser.SplitPropertyLine( line ) ;

						// the content-type property.  Could have an boundary="xxxxx" element. 
						// Boundary strings in a mime document have scope. That is, within one 
						// boundary ( section ) of the document, there can be sub boundaries
						// ( sub sections )
						if ( propPair.a.ToLower( ) == "content-type" )
						{
							PartProperty.ContentType ct = 
								MimeParser.ParseContentType( propPair.b ) ;
							if (( ct.Boundary != null ) && ( ct.Boundary != "" ))
								bdryStack.Push( ct.Boundary ) ;
						}
						pip.AddLine( lineBx, line ) ;
						break ;
					}

						// part boundary line. load the lines of the current part and reset the line
						// counter of this new part.
					case MimeLineCode.PartBdry:
						if ( pip.HasLines == true )
							parts.AddNewPart( InStream, pip ) ;
						pip = new MimeMessagePart.PartInProgress( MimePartCode.Content ) ;
						currentlyBetweenParts = false ;
						break ;

					case MimeLineCode.PartEndBdry:
						if ( pip.HasLines == true )
							parts.AddNewPart( InStream, pip ) ;
						pip = new MimeMessagePart.PartInProgress( MimePartCode.Content ) ;
						if ( bdryStack.IsNotEmpty )
							bdryStack.Pop( ) ;
						currentlyBetweenParts = true ;
						break ;

						// we have a line which is not a property line, not a boundary line.
					default:
					{
						// just starting out.  discard the "+OK" response sent by the server immed 
						// before the start of the message.
						if (( pip.PartCode == MimePartCode.Top )
							&& ( pip.PartIsJustStarting == true )
							&& ( line.Length >= 3 )
							&& ( line.Substring( 0, 3 ) == "+OK" ))
						{
						}

							// currently handling property lines.
						else if ( pip.CurrentlyAmoungPropertyLines == true )
						{
							// a blank line switches from property lines to message lines.
							if ( line.Length == 0 )
							{
								pip.CurrentlyAmoungPropertyLines = false ;
								pip.CurrentlyAmoungMessageLines = true ;
							}

								// what is this text line doing amoung the property lines ??
								// for now, just ignore it.
							else
							{
							}
						}

						else if ( currentlyBetweenParts == false )
						{
							pip.AddLine( lineBx, line ) ;
						}
						break ;
					}
				}
			}

			// load the lines of the final in progress part.
			if ( pip.HasLines == true )
				parts.AddNewPart( InStream, pip ) ;

			return parts ;
		}

		// ------------------------ ParsePropertyLine -----------------------------
		public static StringPair ParsePropertyLine( string InLine )
		{
			StringPair pair = new StringPair( ) ;
			int Fx = InLine.IndexOf( ':' ) ;
			if ( Fx == -1 )
				throw( new MimeContentException( "Header property line not correct form" )) ;

			// the property name extends up to the ":"
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

		/// <summary>
		/// Scan in stream to the next end of line which is not a mime message header
		/// fold sequence ( <cr><lf><lwsp>)
		/// </summary>
		/// <param name="InStream"></param>
		/// <param name="InBx"></param>
		/// <returns></returns>
		public static IntStringPair ScanEndUnfoldedLine( string InStream, int InBx )
		{
			StringBuilder sb = new StringBuilder( 256 ) ;
			int Lx = InStream.Length ;
			int Ex = 0 ;
			int Ix = InBx - 1 ;
			while( true )
			{
				++Ix ;

				// past end of stream
				if ( Ix >= Lx )
				{
					Ex = Lx - 1 ;
					break ;
				}

				char ch1 = InStream[Ix] ;

				// this is a fold sequence. 
				if (( ch1 == '\r' )
					&& (( Ix + 2 ) < Lx )
					&& ( InStream[Ix+1] == '\n' )
					&& (( InStream[Ix+2] == ' ' ) || ( InStream[Ix+2] == '\t' )))
				{
					sb.Append( InStream[Ix+2]) ;
					Ix += 2 ;
				}

					// this is the end of the line.
				else if (( ch1 == '\r' )
					&& (( Ix + 1 ) < Lx )
					&& ( InStream[Ix+1] == '\n' ))
				{
					Ex = Ix + 1 ;
					break ;
				}

					// not a fold.  add to the output stream.
				else
					sb.Append( ch1 ) ;
			}

			return( new IntStringPair( Ex, sb.ToString( ))) ;
		}

		/// <summary>
		/// Scan in stream to the next end of line ( crlf )
		/// </summary>
		/// <param name="InStream"></param>
		/// <param name="InBx"></param>
		/// <returns></returns>
		public static IntStringPair ScanEndLine( string InStream, int InBx )
		{
			StringBuilder sb = new StringBuilder( 256 ) ;
			int Lx = InStream.Length ;
			int Ex = 0 ;
			int Ix = InBx - 1 ;
			while( true )
			{
				++Ix ;

				// past end of stream
				if ( Ix >= Lx )
				{
					Ex = Lx - 1 ;
					break ;
				}

				char ch1 = InStream[Ix] ;

					// this is the end of the line.
				if (( ch1 == '\r' )
					&& (( Ix + 1 ) < Lx )
					&& ( InStream[Ix+1] == '\n' ))
				{
					Ex = Ix + 1 ;
					break ;
				}

				else
					sb.Append( ch1 ) ;
			}

			return( new IntStringPair( Ex, sb.ToString( ))) ;
		}

		// --------------------------- Unfold ---------------------------------
		// folds in the stream are crlf followed by a whitespace character. Folds are used
		// to limit the length of the text lines in mime messages as they  
		// This method removes the folds from the stream, replacing them with the single
		// whitespace char that is the last char of the fold sequence.
		public static string Unfold( string InStream )
		{
			StringBuilder sb = new StringBuilder( InStream.Length ) ;

			int Lx = InStream.Length ;
			for( int Ix = 0 ; Ix < Lx ; ++Ix )
			{
				char ch1 = InStream[Ix] ;

				// this is a fold sequence. 
				if (( ch1 == '\r' )
					&& ( Ix <= ( Lx - 3 ))
					&& ( InStream[Ix+1] == '\n' )
					&& (( InStream[Ix+2] == ' ' ) || ( InStream[Ix+2] == '\t' )))
				{
					sb.Append( InStream[Ix+2]) ;
					Ix += 2 ;
				}

					// not a fold.  add to the output stream.
				else
					sb.Append( ch1 ) ;
			}
			return sb.ToString( ) ;
		}

	}
}
