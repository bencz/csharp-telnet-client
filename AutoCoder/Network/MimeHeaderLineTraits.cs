using System ;
using System.Text ;

namespace AutoCoder.Network
{

	// --------------------- MimeHeaderLineTraits ------------------------------
	// how to handle aspects of the encoding contents.
	public class MimeHeaderLineTraits
	{
		Encoding mEncoder = null ;
		string mEncoderCharSet = "ISO-8859-1" ;
		string mOtherEncodeChars = null ;
		int mnLineHardMaxLength = 998 ;
		int mnLineDesiredMaxLength = 78 ;

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

		// -------------------------- properties ----------------------------
		public int LineHardMaxLength
		{
			get { return mnLineHardMaxLength ; }
			set { mnLineHardMaxLength = value ; }
		}
		public int LineDesiredMaxLength
		{
			get { return mnLineDesiredMaxLength ; }
			set { mnLineDesiredMaxLength = value ; }
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
		public MimeHeaderLineTraits SetOtherEncodeChars( string InValue )
		{
			OtherEncodeChars = InValue ;
			return this ; 
		}

		// -------------------- SetEncoderCharSet ----------------------------
		public MimeHeaderLineTraits SetEncoderCharSet( string InEncoderCharSet )
		{
			EncoderCharSet = InEncoderCharSet ;
			return this ; 
		}
	} // end class MimeHeaderLineTraits
}
