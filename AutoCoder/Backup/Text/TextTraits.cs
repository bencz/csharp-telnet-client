using System;

namespace AutoCoder.Text
{

	// ---------------------------- TextTraits --------------------------
	public class TextTraits
	{
		char[]	mWhitespaceChars = new char[] { ' ', '\t' } ;
		char[]	mDelimChars = null ;
		char[]	mBraceChars = null ;
		QuoteEncapsulation mQem = QuoteEncapsulation.Escape  ;

		public TextTraits()
		{
			mDelimChars = mWhitespaceChars ;
		}

		// ------------------------ properties ----------------------------
		public char[] WhitespaceChars
		{
			get
			{
				if ( mWhitespaceChars != null )
					return mWhitespaceChars ;
				else
				{
					mWhitespaceChars = StandardWhitespaceChars( ) ;
					return mWhitespaceChars ;
				}
			}
			set { mWhitespaceChars = value ; }
		}
		public char[] DelimChars
		{
			get
			{
				if ( mDelimChars != null )
					return mDelimChars ;
				else
				{
					mDelimChars = StandardDelimChars( ) ;
					return mDelimChars ;
				}
			}
			set { mDelimChars = value ; }
		}
		public char[] BraceChars
		{
			get
			{
				if ( mBraceChars != null )
					return mBraceChars ;
				else
				{
					mBraceChars = StandardBraceChars( ) ;
					return mBraceChars ;
				}
			}
			set { mBraceChars = value ; }
		}
		public QuoteEncapsulation QuoteEncapsulation
		{
			get { return mQem ; }
			set { mQem = value ; }
		}

		// ------------------------ AssureOpenBraceChars -----------------------
		private void AssureOpenBraceChars( )
		{
			if ( mBraceChars == null )
			{
				mBraceChars = StandardBraceChars( ) ;
			}
		}

		// ------------------------- IsDelimChar ---------------------------
		public bool IsDelimChar( char InChar )
		{
			for( int Ix = 0 ; Ix < mDelimChars.Length ; ++Ix )
			{
				if ( mDelimChars[Ix] == InChar )
					return true ;
			}
			return false ;
		}

		// ------------------------- IsOpenBraceChar ---------------------------
		public bool IsOpenBraceChar( char InChar )
		{
			AssureOpenBraceChars( ) ;
			for( int Ix = 0 ; Ix < mBraceChars.Length ; ++Ix )
			{
				if ( mBraceChars[Ix] == InChar )
					return true ;
			}
			return false ;
		}

		// ------------------------- set methods -------------------------------
		public TextTraits SetDelimChars( string InChars )
		{
			mDelimChars = InChars.ToCharArray( ) ;
			return this ; 
		}
		public TextTraits SetQuoteEncapsulation( QuoteEncapsulation InValue )
		{
			mQem = InValue ;
			return this ; 
		}
		public TextTraits SetWhitespaceChars( string InChars )
		{
			mWhitespaceChars = InChars.ToCharArray( ) ;
			return this ; 
		}

		/// <summary>
		/// standard set of open brace characters
		/// </summary>
		/// <returns></returns>
		public static char[] StandardBraceChars( )
		{
			char[] ch4 = new char[] { '{', '[', '(', '<' } ;
			return ch4 ;
		}

		/// <summary>
		/// standard set of delimeter characters.
		/// </summary>
		/// <returns></returns>
		public static char[] StandardDelimChars( )
		{
			char[] ch3 = new char[] { ' ', '\t', ',' } ;
			return AcCommon.Concat( ch3, StandardBraceChars( )) ; 
		}

		/// <summary>
		/// standard set of whitespace characters.
		/// </summary>
		/// <returns></returns>
		public static char[] StandardWhitespaceChars( )
		{
			char[] ch2 = new char[] { ' ', '\t' } ;
			return ch2 ;
		}

	}
}
