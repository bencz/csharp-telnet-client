using System;
using System.Text ;
using AutoCoder.Text ; 

namespace AutoCoder.Network
{
	// ------------------------- MessageHeaderString -------------------------------
	// a string placed in the header of the internet message.
	public class MessageHeaderString
	{

		protected enum ChunkType { None, Spaces, NonSpace } 

		protected class Chunk
		{
			int mBx = -1 ;
			int mLx = -1 ;
			ChunkType mChunkType ;

			public Chunk( )
			{
			}

			public Chunk( int InBx, int InLx )
			{
				Bx = InBx ;
				Lx = InLx ;
			}

			public int Bx
			{
				get { return mBx ; }
				set { mBx = value ; }
			}
			public int Lx
			{
				get { return mLx ; }
				set { mLx = value ; }
			}
			public ChunkType ChunkType
			{
				get { return mChunkType ; }
				set { mChunkType = value ; }
			}

			public Chunk Empty( )
			{
				Bx = -1 ;
				Lx = -1 ;
				return this ; 
			}

			public bool IsEmpty( )
			{
				return( Bx == -1 ) ;
			}
		}


		// --------------------- Traits ------------------------------
		// how to handle aspects of the encoding contents.
		public class Traits
		{
			Encoding mEncoder ;
			string mEncoderCharSet ;
			string mOtherEncodeChars = null ;
//			int mnLineHardMaxLength ;
//			int mnLineDesiredMaxLength ;

			// ---------------------- Properties ------------------------
			public string EncoderCharSet
			{
				get { return mEncoderCharSet ; }
				set
				{
					mEncoderCharSet = value ;
					mEncoder = Encoding.GetEncoding( mEncoderCharSet ) ;
				}
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
				{
					string encoderCharSet = "ISO-8859-1" ;
					mEncoder = Encoding.GetEncoding( encoderCharSet ) ;
				}
			}

			// ------------------- SetOtherEncodeChars ------------------------
			public Traits SetOtherEncodeChars( string InValue )
			{
				OtherEncodeChars = InValue ;
				return this ; 
			}

			// -------------------- SetEncoderCharSet ----------------------------
			public Traits SetEncoderCharSet( string InEncoderCharSet )
			{
				EncoderCharSet = InEncoderCharSet ;
				return this ; 
			}

		} // end class Traits

		// ---------------------- MessageHeaderString --------------------------------
		public MessageHeaderString( )
		{
		}

		// -------------------------- EncodeAsRequired ------------------------------
		// encode the string if "RequiresEncoding".  Otherwise, return the input string
		// as is.
		public static string EncodeAsRequired(
			string InValue, QuotedPrintableTraits InTraits )
		{
			if ( QuotedPrintable.RequiresEncoding( InValue, InTraits ) == true )
			{
				StringBuilder sb = new StringBuilder( InValue.Length * 2 ) ;
				sb.Append( "=?" + InTraits.CharSet + "?Q?" ) ;
				sb.Append( QuotedPrintable.Encode( InValue, InTraits )) ;
				sb.Append( "?=" ) ;
				return sb.ToString( ) ;
			}
			else
				return InValue ;
		}

		// ------------------------ ToEncodedString --------------------------------
		public static string ToEncodedString( string InValue )
		{
			return "" ;
		}

		// --------------------------- AdvanceNextChunk -----------------------------
		// 
		private static Chunk AdvanceNextChunk( string InValue, Chunk InChunk )
		{
			int Fx = 0, Lx = 0 ;
			Chunk results = null ;
			int Ix ;

			// calc the pos of start of chunk that follows the current chunk.
			if ( InChunk.IsEmpty( ))
				Ix = 0 ;
			else
				Ix = InChunk.Bx + InChunk.Lx ;
			
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
					results = new Chunk( Ix, InValue.Length - Ix ) ;
				}

				else 
				{
					Lx = Fx - Ix ;
					results = new Chunk( Ix, Lx ) ;
				}
			}
			return results ;
		}

	}
}
