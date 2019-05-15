using System;
using System.Collections ;
using System.Text ;
using AutoCoder.Text ; 

namespace AutoCoder.Network
{

	// ------------------------- MimeHeaderLineBuilder -------------------------------
	// a line placed in the header of the internet message.
	public class MimeHeaderLineBuilder
	{
		protected enum ChunkType
		{ None, Spaces, LiteralChars, EncodedChars, Fold, EndOfLine } 

		// ------------------------ EncodedLines --------------------------
		protected class EncodedLines : ArrayList
		{
			MimeHeaderLineBuilder mParentLineBuilder = null ;

			// ------------------- constructor ------------------------------
			public EncodedLines( MimeHeaderLineBuilder InLineBuilder )
			{
				mParentLineBuilder = InLineBuilder ;
			}

			// ----------------------- properties -----------------------
			public LineChunkList CurrentLine
			{
				get { return LastLine( ) ; }
			}

			public MimeHeaderLineBuilder ParentLineBuilder
			{
				get { return mParentLineBuilder ; }
			}

			// -------------------------- AddNewLine -------------------------
			public LineChunkList AddNewLine( )
			{
				LineChunkList line = new LineChunkList( this ) ;
				base.Add( line ) ;
				return line ;
			}

			// ------------------------ FirstLine -------------------------
			// return the first encoded line.
			public LineChunkList FirstLine( )
			{
				LineChunkList line = null ;
				if ( base.Count > 0 )
					line = (LineChunkList) base[0] ;
				return line ;
			}

			// ------------------------ FoldNewLine ----------------------------
			// insert a fold at the end of the current line
			public Chunk FoldNewLine( )
			{
				// end the current line.
				Chunk eolChunk = new Chunk( ChunkType.EndOfLine ) ;
				CurrentLine.AddChunk( eolChunk ) ;

				// add the fold to the current empty line.
				Chunk fold = new Chunk( ChunkType.Fold ) ;
				CurrentLine.AddChunk( fold ) ;

				return fold ;
			}

			// ------------------------- FoldNewLine ---------------------------
			// add a fold ( crlf-space ) to the current line.  This will 
			public ChunkPair FoldNewLine( Chunk InSpaceChunk )
			{
				// must be a space chunk
				if ( InSpaceChunk.ChunkType != ChunkType.Spaces )
					throw( new ApplicationException( "FoldNewLine method must be passed spaces" )) ;
				
				// end the current line.
				Chunk eolChunk = new Chunk( ChunkType.EndOfLine ) ;
				CurrentLine.AddChunk( eolChunk ) ;

				// split the space chunk. one char for the fold chunk. the remainder to be 
				// placed as processing continues.
				ChunkPair pair = InSpaceChunk.Split( ParentLineBuilder.Traits, 1 ) ;

				// the fold chunk is a single space.  ( this is a single space from the
				// unicode string so it has a Bx. )
				Chunk foldChunk = pair.a ;
				CurrentLine.AddChunk( foldChunk ) ;
				
				return pair ;
			}

			// ------------------------ LastLine -------------------------
			// return the last encoded line.
			public LineChunkList LastLine( )
			{
				LineChunkList line = null ;
				int Cx = base.Count ;
				if ( Cx > 0 )
					line = (LineChunkList) base[ Cx - 1 ] ;
				return line ;
			}

			// ----------------------- ToMimeMessageString -----------------------
			// assemble and return the encoded string in form for the mime message stream.
			public string ToMimeMessageString( )
			{
				StringBuilder sb = new StringBuilder( 2000 ) ;
				foreach( LineChunkList line in this )
				{
					sb.Append( line.ToMimeMessageString( )) ;
				}
				return sb.ToString( ) ;
			}

		} // end EncodedLines class


		// ---------------------------- Chunk ---------------------------
		// character block from the unicode string to be mime header line encoded.
		protected class Chunk
		{
			int mBx = -1 ;
			ChunkType mChunkType ;
			string mValue ;
			string mEncodedValue ;
			int mEncodedBx = -1 ;
			bool mEncodeAlways = false ;

			// length consumed from "string to encode" by this chunk. 
			// Normally length consumed is the length of the chunk value. 
			// However, if the chunk is a Fold inserted into the output to comply with a
			// mime message line maximum, the consumed length will be zero.
			int mConsumedLx = -1 ; 

			// --------------------- constructors ----------------------------
			public Chunk( )
			{
			}

			public Chunk( string InValue, int InBx )
			{
				Bx = InBx ;
				mValue = InValue ;
			}

			public Chunk( ChunkType InChunkType, string InValue )
			{
				ChunkType = InChunkType ;
				Value = InValue ;
			}

			public Chunk( ChunkType InChunkType )
			{
				ChunkType = InChunkType ;
			}

			// ---------------------- properties -----------------------------
			public int Bx
			{
				get { return mBx ; }
				set { mBx = value ; }
			}
			public ChunkType ChunkType
			{
				get { return mChunkType ; }
				set { mChunkType = value ; }
			}
			public int ConsumedLx
			{
				get
				{
					if ( mConsumedLx != -1 )
						return mConsumedLx ;
					else if ( ChunkType == ChunkType.Fold )
						return 1 ;
					else if ( ChunkType == ChunkType.EndOfLine )
						return 1 ;
					else
						return Value.Length ;
				}
				set { mConsumedLx = value ; }
			}

			public bool EncodeAlways
			{
				get { return mEncodeAlways ; }
				set { mEncodeAlways = value ; }
			}

			public int EncodedBx
			{
				get
				{
					if ( mEncodedBx == -1 )
						throw( new ApplicationException( "begin position of encoded value not yet assigned." )) ;
					else
						return mEncodedBx ;
				}
				set	{	mEncodedBx = value ; }
			}
			public int EncodedEx
			{
				get { return ( EncodedBx + EncodedValue.Length - 1 ) ; }
			}
			public string EncodedValue
			{
				get { return mEncodedValue ; }
				set { mEncodedValue = value ; }
			}
			public int EndIx
			{
				get { return Bx + ConsumedLx - 1 ; }
			}
			public string Value
			{
				get
				{
					if ( ChunkType == ChunkType.Fold )
						return " " ;
					else if ( ChunkType == ChunkType.EndOfLine )
						return "\n\r" ; 
					else
						return mValue ;
				}
				set { mValue = value ; }
			}

			// -------------------------- Empty --------------------------
			public Chunk Empty( )
			{
				Bx = -1 ;
				mValue = null ;
				return this ; 
			}

			// ------------------------- Chunk.Encode -------------------------------
			// encode the chunk as it will be placed in the mime header.
			public string Encode( MimeHeaderLineTraits InTraits )
			{
				EncodedValue = null ;
				if (( ChunkType == ChunkType.EncodedChars )
					|| ( EncodeAlways == true ))
				{
					EncodedValue = MimeEncodedWord.Encode( Value, InTraits ) ;
				}
				else if ( ChunkType == ChunkType.EndOfLine )
				{
					EncodedValue = "\r\n" ;
				}
				else 
				{
					EncodedValue = Value ;
				}
				return EncodedValue ;
			}

			// ---------------------- EvalChunkType ----------------------
			// eval the ChunkType based on examination of the chunk value
			public Chunk EvalChunkType( MimeHeaderLineTraits InTraits )
			{
				// isolate the chunk value.
				string chunkValue = Value ;
				int Lx = chunkValue.Length ;
			
				// classify the chunk. is it a literal chunk or an encoded-word required one.
				bool mustEncode = false ;
				bool allSpaces = true ;
				for( int Ix = 0 ; Ix < Lx ; ++Ix )
				{
					char ch1 = chunkValue[Ix] ;
					if (( ch1 < 33 ) || ( ch1 > 126 ))
					{
						mustEncode = true ;
						break ;
					}
					else if (( InTraits.OtherEncodeChars != null )
						&& ( InTraits.OtherEncodeChars.IndexOf( ch1 ) != -1 ))
					{
						mustEncode = true ;
						break ;
					}

					if (( allSpaces == true ) && ( ch1 != ' ' ))
						allSpaces = false ;
				}

				// assign the chunk type.
				if ( mustEncode == true )
					ChunkType = ChunkType.EncodedChars ;
				else if ( allSpaces == true )
					ChunkType = ChunkType.Spaces ;
				else
					ChunkType = ChunkType.LiteralChars ;

				return this ;
			}

			// -------------------------- IsEmpty -------------------------
			public bool IsEmpty( )
			{
				return( mValue == null ) ;
			}

			// -------------------- NewSpacesChunk -------------------------
			public static Chunk NewSpacesChunk( int InSpaceCx )
			{
				Chunk chunk = new Chunk( ChunkType.Spaces ) ;
				string spaces = " " ;
				if ( InSpaceCx > 1 )
					spaces = spaces.PadRight( InSpaceCx - 1 ) ;
				chunk.Value = spaces ;
				return chunk ;
			}

			// ------------------------- SetEncodeAlways --------------------
			public Chunk SetEncodeAlways( bool InValue )
			{
				EncodeAlways = InValue ;
				return this ;
			}

			// ----------------------- Split -----------------------------------
			public ChunkPair Split( MimeHeaderLineTraits InTraits, int InFirstPartLx )
			{
				string part1Value = null ;
				string part2Value = null ;
				Chunk part1 = null ;
				Chunk part2 = null ;

				int part1Lx = InFirstPartLx ;
				int part2Lx = Value.Length - part1Lx ;
				int part1Bx = Bx ;
				int part2Bx = Bx + part1Lx ;

				if ( part1Lx > 0 )
				{
					part1Value = Value.Substring( 0, part1Lx ) ;
					part1 = new Chunk( ) ;
					part1.Bx = part1Bx ;
					part1.Value = part1Value ;
					part1.EvalChunkType( InTraits ) ;
				}

				if ( part2Lx > 0 )
				{
					part2Value = Value.Substring( part1Lx, part2Lx ) ;
					part2 = new Chunk( ) ;
					part2.Bx = part2Bx ;
					part2.Value = part2Value ;
					part2.EvalChunkType( InTraits ) ;
				}

				return( new ChunkPair( part1, part2 )) ;
			}

			// ------------------------ TrimLeft --------------------------------
			// return a new chunk which if effectively a copy of this chunk but with
			// the starting characters clipped and the start position moved to the right.
			public Chunk TrimLeft( int InTrimCharCx )
			{
				Chunk chunk = new Chunk( ChunkType ) ;
				int Lx = ConsumedLx - InTrimCharCx ;
				chunk.Bx = Bx + InTrimCharCx ;
				if ( Lx > 0 )
					chunk.Value = Value.Substring( InTrimCharCx, Lx ) ;
				return chunk ;
			}

		} // end class Chunk


		// ------------------------ ChunkPair -------------------------
		protected class ChunkPair
		{
			public Chunk a ;
			public Chunk b ;

			public ChunkPair( )
			{
				a = null ;
				b = null ;
			}
			public ChunkPair( Chunk Ina, Chunk Inb )
			{
				a = Ina ;
				b = Inb ;
			}
		}

		// --------------------------- LineChunkList ---------------------------
		// list of chunks on the line. 
		// Used to decide whether current line should be folded or not.
		protected class LineChunkList : ArrayList
		{
			EncodedLines mParentLines = null ;

			// -------------------------- constructor -------------------------
			public LineChunkList( EncodedLines InLines )
			{
				mParentLines = InLines ;
			}

			// ------------------------- properties -----------------------------
			public int EncodedEx
			{
				get
				{
					if ( IsEmpty( ) == true )
						return 0 ;
					else
						return( LastChunk( ).EncodedEx ) ; 
				}
			}

			public EncodedLines ParentLines
			{
				get { return mParentLines ; }
			}

			// ------------------------ AddChunk ------------------------------
			// add the chunk to this list of chunks on the encoded line.
			public LineChunkList AddChunk( Chunk InChunk )
			{
				// Fold chunk must be at the start of the line.
				if (( InChunk.ChunkType == ChunkType.Fold )
					&& ( base.Count > 0 ))
					throw( new ApplicationException( "Fold being added to line that is not new" )) ;

				// encode the chunk
				InChunk.Encode( ParentLines.ParentLineBuilder.Traits ) ;

				// the encoded begin position of this chunk is immed after the end of the 
				// prior one.
				if ( base.Count == 0 )
					InChunk.EncodedBx = 0 ;
				else
					InChunk.EncodedBx = LastChunk( ).EncodedEx + 1 ;

				// add to this list of line of encoded values.
				base.Add( InChunk ) ;

				return this ;
			}

			// --------------------- CalcDesiredMaxRemLx -------------------------------
			// how much open space remains on the encoded output line.
			public int CalcDesiredMaxRemLx( MimeHeaderLineTraits InTraits )
			{
				int remLx = InTraits.LineDesiredMaxLength - EncodedEx ;
				return remLx ;
			}

		public bool IsEmpty( )
		{
			return( base.Count == 0 ) ;
		}

			// ---------------------- IsNewLine --------------------------
			// line is either empty or has only a Fold chunk on it.
			public bool IsNewLine( )
			{
				if ( base.Count == 0 )
					return true ;
				else if (( base.Count == 1 )
					&& ( FirstChunk( ).ChunkType == ChunkType.Fold ))
					return true ;
				else
					return false ;
			}

			// ------------------------ FirstChunk -------------------------
			// return the first Chunk on the line.
			public Chunk FirstChunk( )
			{
				Chunk chunk = null ;
				if ( base.Count > 0 )
					chunk = (Chunk) base[0] ;
				return chunk ;
			}

			// ------------------------ LastChunk -------------------------
			// return the last Chunk on the line.
			public Chunk LastChunk( )
			{
				Chunk chunk = null ;
				int Cx = base.Count ;
				if ( Cx > 0 )
					chunk = (Chunk) base[ Cx - 1 ] ;
				return chunk ;
			}


			// ----------------------- ToMimeMessageString -----------------------
			// assemble and return the encoded string in form for the mime message stream.
			public string ToMimeMessageString( )
			{
				StringBuilder sb = new StringBuilder( 2000 ) ;
				foreach( Chunk chunk in this )
				{
					sb.Append( chunk.EncodedValue ) ;
				}
				return sb.ToString( ) ;
			}
		} // end class LineChunkList


		// --------------------------- LinePart -----------------------------


		// ------------------------ data members ----------------------------
		StringBuilder mLine = null ;
		MimeHeaderLineTraits mTraits ;

		// ---------------------- MimeHeaderLineBuilder --------------------------------
		public MimeHeaderLineBuilder( )
		{
		}

		// ---------------------------- properties --------------------------------
		public MimeHeaderLineTraits Traits
		{
			get
			{
				if ( mTraits == null )
					mTraits = new MimeHeaderLineTraits( ) ;
				return mTraits ;
			}
			set { mTraits = value ; }
		}

		private StringBuilder UnencodedText
		{
			get
			{
				if ( mLine == null )
					mLine = new StringBuilder( 5000 ) ;
				return mLine ;
			}
		}

		// ----------------------------- Append ---------------------------------
		// add unencoded unicode text to the line. Once complete adding all the text,
		// and the Traits are set, use ToString to get the encoded result.
		public MimeHeaderLineBuilder Append( string InValue )
		{
			UnencodedText.Append( InValue ) ;
			return this ; 
		}

		// --------------------- CalcQuotedPrintableTraits ------------------------
		// Setup an object containing the traits for the Quoted-Printable encoding of
		// contents of the MimeHeaderLine.
		public static QuotedPrintableTraits CalcQuotedPrintableTraits(
			MimeHeaderLineTraits InMimeLineTraits )
		{
			QuotedPrintableTraits qpTraits = new QuotedPrintableTraits( ) ;
			qpTraits.SetCharSet( InMimeLineTraits.EncoderCharSet ) ;
			return qpTraits ;
		}

		// ------------------------------ Clear ------------------------------------
		// clear the unencoded unicode string of the line.
		public MimeHeaderLineBuilder Clear( )
		{
			mLine = null ;
			return this ;
		}

		// ----------------- MimeHeaderLineBuilder.ToEncodedString -------------------------
		// encode the loaded unicode string as one or more mime message lines.
		public string ToEncodedString( )
		{
			string ucText = UnencodedText.ToString( ) ;
			StringBuilder sb = new StringBuilder( 5000 ) ;
			EncodedLines lines = new EncodedLines( this ) ;

			// for each chunk of text in the string. The chunks are either contiguous spaces,
			// text that can be placed literally ( unencoded ) on the mime header line, or
			// text that has to be encoded in Quoted-Printable form.
			Chunk chunk = new Chunk( ) ;
			lines.AddNewLine( ) ;
			while( true )
			{
				chunk = AdvanceNextChunk( ucText, chunk ) ;
				if ( chunk == null )
					break ;

				// a spaces chunk. before adding the spaces to the encoded output, look ahead
				// to the next chunk.  Will it fit on the current line?  If not, use folding to 
				// start the space chunk on the next line.
				if ( chunk.ChunkType == ChunkType.Spaces )
				{
					chunk = ToEncodedString_SpaceChunk( chunk, lines, ucText ) ;
				}

				else
					lines.CurrentLine.AddChunk( chunk ) ;
			}

			// final piece. The mime header line term char sequence.
//			lines.CurrentLine.AddChunk( new Chunk( ChunkType.EndOfLine )) ;
			return( lines.ToMimeMessageString( )) ;
		}

		// Encode a spaces chunk for placement in the mime output stream.
		// Before adding the spaces to the encoded output, look ahead
		// to the next chunk.  Will it fit on the current line?  If not, use folding to 
		// start the space chunk on the next line.
		private Chunk ToEncodedString_SpaceChunk(
			Chunk InChunk,
			EncodedLines InLines,
			string InUcText )
		{
			Chunk chunk = InChunk ;
			Chunk nxChunk = AdvanceNextChunk( InUcText, chunk ) ;

			// how much space will this space chunk, AND the chunk that comes next, occupy in
			// the encoded output stream. ( cant end a line with spaces. they are trimmed by
			// the mail agents )
			int LxNeeded = chunk.Encode( Traits ).Length ;
			if ( nxChunk != null )
				LxNeeded += nxChunk.Encode( Traits ).Length ;

			// length needed exceeds the desired max size of the line ( about 80 characters )
			// Place the space chunk in the encoded output stream in such a way that as many
			// spaces will be placed on the current line, but at least one will remain and be
			// folded onto the next line.
			if ( InLines.CurrentLine.CalcDesiredMaxRemLx( Traits ) < LxNeeded )
			{
				chunk = SpaceChunk_ToEndOfEncodedLine( chunk, InLines ) ;
			}

				// This space chunk and the chunk that follows fit on the line.  place the
				// space chunk. Next cycle will pickup and place the next chunk.
			else
			{
				InLines.CurrentLine.AddChunk( chunk ) ;
			}

			return chunk ;
		}

		// ------------------------- SpaceChunk_ToEndOfEncodedLine --------------------
		// spaces being placed at the last chunk on the line. That means the last char
		// of the spaces must be QP encoded as =20 in order to prevent the spaces from
		// being trimmed by mail transfer agents.
		private Chunk SpaceChunk_ToEndOfEncodedLine(
			Chunk InChunk,
			EncodedLines InLines )
		{
			ChunkPair pair = null ;

			// isolate the chunk we are working with.
			Chunk chunk = InChunk ;

			// space remaining on the line.
			int RemLx = InLines.CurrentLine.CalcDesiredMaxRemLx( Traits ) ;
			
			// how much room is used by single space in encoded-word form.
			int SingleEncodedSpaceUsedLx =
				new Chunk( ChunkType.Spaces, " " )
				.SetEncodeAlways( true )
				.Encode( Traits )
				.Length ;
			int maxSeg1SpaceCx = RemLx - SingleEncodedSpaceUsedLx - 1 ; 

			// not enough room to encode a single space. fold right here.
			if ( SingleEncodedSpaceUsedLx > RemLx )
			{
				pair = InLines.FoldNewLine( chunk ) ;
				chunk = pair.a ;
			}

				// only one space in the chunk. that space is needed for the fold.  fold
				// right away.
			else if ( chunk.Value.Length == 1 )
			{
				pair = InLines.FoldNewLine( chunk ) ;
				chunk = pair.a ;
			}

				// place spaces on the line, then a space in encoded-word form, then fold the
				// line.  
			else
			{

				// place as many spaces before the encoded-word space.
				int seg1SpaceCx = chunk.Value.Length - 2 ;
				if ( seg1SpaceCx > maxSeg1SpaceCx )
					seg1SpaceCx = maxSeg1SpaceCx ;
				if ( seg1SpaceCx > 0 )
				{
					pair = chunk.Split( Traits, seg1SpaceCx ) ;
					InLines.CurrentLine.AddChunk( pair.a ) ;
					chunk = pair.b ;
				}
				
				// there should be at least 2 spaces remaining. 
				if ( chunk.Value.Length < 2 )
					throw( new ApplicationException( "Space chunk is too small" )) ;

				// split a space from the chunk and add it to the line in forced encoded form.
				// this is the last chunk on the line.
				pair = chunk.Split( Traits, 1 ) ;
				pair.a.SetEncodeAlways( true ) ;
				InLines.CurrentLine.AddChunk( pair.a ) ;
				chunk = pair.b ;

				// fold the line, using another space as the fold char on the new line.
				pair = InLines.FoldNewLine( chunk ) ;
				chunk = pair.a ;
			}

			// return with the last chunk placed on the line.
			return chunk ;
		}

		// ------------------------ ToEncodedString --------------------------------
		public static string ToEncodedStringx( string InValue )
		{
			return "" ;
		}

		// --------------------------- AdvanceNextChunk -----------------------------
		private Chunk AdvanceNextChunk( string InValue, Chunk InChunk )
		{
			int Fx = 0, Lx = 0 ;
			Chunk results = null ;
			int Ix ;

			// calc the pos of start of chunk that follows the current chunk.
			if ( InChunk.IsEmpty( ))
				Ix = 0 ;
			else
				Ix = InChunk.Bx + InChunk.ConsumedLx ;
			
			// past end of string.
			if ( Ix >= InValue.Length )
			{
				results = null ;
			}

				// starting a space chunk.  find the end of the chunk, the last space.
			else if ( InValue[Ix] == ' ' )
			{
				Fx = Scanner.ScanNotEqual( InValue, Ix, ' ' ).ResultPos ;
				if ( Fx == -1 )
				{
					results = new Chunk( InValue.Substring( Ix ), Ix ) ;
				}
				else 
				{
					Lx = Fx - Ix ;
					results = new Chunk( InValue.Substring( Ix, Lx ), Ix ) ;
				}
				results.ChunkType = ChunkType.Spaces ;
			}
			
				// chunk runs from this non space until a space is found.
			else
			{
				Fx = InValue.IndexOf( ' ', Ix ) ;
				if ( Fx == -1 )
					Lx = InValue.Length - Ix ;
				else
					Lx = Fx - Ix ;
				results = new Chunk( InValue.Substring( Ix, Lx ), Ix ) ;

				// classify the chunk. is it a literal chunk or an encoded-word required one.
				bool mustEncode = false ;
				for( Ix = 0 ; Ix < results.Value.Length ; ++Ix )
				{
					char ch1 = results.Value[Ix] ;
					if (( ch1 < 33 ) || ( ch1 > 126 ))
					{
						mustEncode = true ;
						break ;
					}
					else if (( Traits.OtherEncodeChars != null )
						&& ( Traits.OtherEncodeChars.IndexOf( ch1 ) != -1 ))
					{
						mustEncode = true ;
						break ;
					}
				}

				// an all literal chunk.
				if ( mustEncode == false )
					results.ChunkType = ChunkType.LiteralChars ;

				// a relatively short chunk.  Classify as encoded-word required, no matter how
				// many of the characters need to be encoded. 
				else if ( results.Value.Length <= 10 )
					results.ChunkType = ChunkType.EncodedChars ;

				// the chunk starts with literal characters.
				else
					results.ChunkType = ChunkType.EncodedChars ;
   			}

			return results ;
		}

	}


}
