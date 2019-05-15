using System;
using AutoCoder.Text ;

namespace AutoCoder.TextStmt
{

	// --------------------- class TextStmt ------------------------------
	public class TextStmt
	{
		string mCmds ;
		char[] mWhitespaceChars ;
		char[] mDelimChars ;

		public TextStmt( )
		{
			ConstructCommon( ) ;
		}
		public TextStmt( string InCmds )
		{
			ConstructCommon( ) ;
			AssignString( InCmds ) ;
		}

		public char[] WhitespaceChars
		{
			set { mWhitespaceChars = value ; }
		}
		public char[] DelimChars
		{
			set { mDelimChars = value ; }
		}
		
		public TextStmt AssignString( string InCmds )
		{
			mCmds = InCmds ;
			return this ;
		}

		private void ConstructCommon( )
		{
			mWhitespaceChars = new char[] { ' ', '\t' } ;
			mDelimChars = new char[] { ';', ' ', ',' } ;
		}

		// ----------------------- IsOpenParenChar ----------------------------
		public bool IsOpenParenChar( int InIx )
		{
			if ( InIx >= mCmds.Length )
				throw( new TextScanException(
					"IsOpenParenChar",
					"Start pos " + InIx + " is past end of string." )) ;
			char ch1 = mCmds[InIx] ;
			if (( ch1 == '(' ) ||
				( ch1 == '{' ) ||
				( ch1 == '[' ) ||
				( ch1 == '<' ))
				return true ;
			else
				return false ;
		}

		// ----------------------- IsOpenQuoteChar ----------------------------
		public bool IsOpenQuoteChar( int InIx )
		{
			if ( InIx >= mCmds.Length )
				throw( new TextScanException(
					"IsOpenQuoteChar",
					"Start pos " + InIx + " is past end of string." )) ;
			char ch1 = mCmds[InIx] ;
			if (( ch1 == '\'' ) ||
				( ch1 == '"' ))
				return true ;
			else
				return false ;
		}

		// ------------------- ParseAssignCommandString ----------------------
		public AssignStmtContents ParseAssignCommandString( )
		{
			return( new AssignStmtContents( )) ;
		}

		// ----------------------- RemoveWord ------------------------------
		// remove the word from the stmt string.
		public void RemoveWord( StmtWord InWord )
		{
			mCmds = mCmds.Remove( InWord.WordBx, InWord.WordLx ) ;
		}

		// ------------------------ ScanCharEqual ------------------------
		// scan for character in the command string
		public int ScanCharEqual( char InChar, int Bx )
		{
			int Ix = Bx - 1 ;
			int Lx = mCmds.Length ;
			while( true )
			{
				++Ix ;
				if ( Ix >= Lx )
				{
					Ix = -1 ;
					break ;
				}
				char ch1 = mCmds[Ix] ;
				if (( ch1 == '"' ) || ( ch1 == '\'' ))
				{
					Ix = ScanCloseQuote( Ix ) ;
					if ( Ix == -1 )
						break ;
				}
				else if ( ch1 == InChar )
					break ;
			}
			return Ix ;
		}

		// ------------------------ ScanCloseQuote ------------------------
		public int ScanCloseQuote( int InBx )
		{
			char QuoteChar = mCmds[InBx] ;
			int Lx = mCmds.Length ;
			int Ix = InBx ;
			while( true )
			{
				++Ix ;
				if ( Ix >= Lx )
				{
					Ix = -1 ;
					break ;
				}
				if ( mCmds[Ix] == QuoteChar )
					break ;
			}
			return Ix ;
		}

		// ------------------------ ScanCloseParen -------------------------
		public int ScanCloseParen( int InBx )
		{
			char OpenParenChar = mCmds[InBx] ;
			char CloseParenChar = AcCommon.CalcCloseBraceChar( mCmds[InBx] ) ;
			int Ix = InBx, Fx = 0 ;
			int Lx = mCmds.Length ;
			int ParenLevel = 1 ;
			while( true )
			{
				++Ix ;
				if ( Ix >= Lx )
					throw( new TextScanException(
						"ScanCloseParen",	"Close paren not found." )) ;
        
				char ch1 = mCmds[Ix] ;
				if ( ch1 == OpenParenChar )
					++ParenLevel ;
				else if ( ch1 == CloseParenChar )
				{
					--ParenLevel ;
					if ( ParenLevel == 0 )
						break ;
				}
				else if ( IsOpenParenChar( Ix ) == true )
				{
					Fx = ScanCloseParen( Ix ) ;
					Ix = Fx ;
				}
				else if ( IsOpenQuoteChar( Ix ) == true )
				{
					Fx = ScanCloseQuote( Ix ) ;
					Ix = Fx ;
				}
			}
			return Ix ;
		}

		// ------------------------ ScanFirstWord -------------------------
		public StmtWord ScanFirstWord( )
		{
			int Ex = 0, Fx = 0 ; 
			int Bx = Scanner.ScanNotEqual( mCmds, 0, mWhitespaceChars ).ResultPos ;
			if ( Bx == -1 )
				return( null ) ;

			char ch1 = mCmds[Bx] ;
			if ( IsOpenParenChar( Bx ) == true )
			{
				Ex = ScanCloseParen( Bx ) ;
			}
			else if ( IsOpenQuoteChar( Bx ) == true )
			{
				Ex = ScanCloseQuote( Bx ) ;
			}
			else
			{
				Fx = Scanner.ScanEqual( mCmds, Bx, mDelimChars ).ResultPos ;
				if ( Fx == -1 )
					Ex = mCmds.Length - 1 ;
				else
					Ex = Fx - 1 ;
			}

			int Lx = Ex - Bx + 1 ;
			return( new StmtWord( this, Bx, Lx )) ;
		}

		// -------------------------- Split ------------------------------
		public SplitResults Split( char InDelimChar )
		{
			// locate each occuration of the delimeter.
			int[] DelimIx = new int[99] ;
			int Dx = -1 ;
			int Ix = 0, Fx ;
			int Lx = mCmds.Length ; 
			while( true )
			{
				++Ix ;
				if ( Ix >= Lx )
					break ;
				char ch1 = mCmds[Ix] ;
				if ( IsOpenParenChar( Ix ) == true )
				{
					Fx = ScanCloseParen( Ix ) ;
					Ix = Fx ;
				}
				else if ( IsOpenQuoteChar( Ix ) == true )
				{
					Fx = ScanCloseQuote( Ix ) ;
					Ix = Fx ;
				}
				else if ( mCmds[Ix] == InDelimChar )
				{
					++Dx ;
					DelimIx[Dx] = Ix ;
				}
			} // end while look for every delimeter.

			// calc content array lx. depends on whether any content follows the last delimeter
			int ArraySx = Dx + 1 ; 
			int TailContentLx = ( mCmds.Length - DelimIx[Dx] ) - 1 ;
			if ( TailContentLx > 0 )
				++ArraySx ; 

			// allocate the split result arrays.
			string[] content = new string[ArraySx] ;
			string[] delim = new string[ArraySx] ;

			// break out the content that preceeds the delimeters.
			int PvDelimEx = -1 ;
			for ( Ix = 0 ; Ix <= Dx ; ++Ix )
			{
				int DelimBx = DelimIx[Ix] ;
				int DelimLx = 1 ;
				
				int ContentBx = PvDelimEx + 1 ;
				int ContentLx = DelimBx - ContentBx ;
				content[Ix] = mCmds.Substring( ContentBx, ContentLx ) ;
				delim[Ix] = mCmds.Substring( DelimBx, DelimLx ) ;

				PvDelimEx = DelimBx + DelimLx - 1 ;
			}

			// break out the content that follows the last delimeter.
			int FollowLx = ( mCmds.Length - PvDelimEx ) - 1 ;
			if ( TailContentLx > 0 )
			{
				content[ArraySx-1] = mCmds.Substring( PvDelimEx + 1, TailContentLx ) ;
				delim[ArraySx-1] = "" ;
			}

			return( new SplitResults( content, delim )) ; 
		}

		// ------------------------ Substring ---------------------------
		public string Substring( int InBx, int InLx )
		{
			return( mCmds.Substring( InBx, InLx )) ;
		}

		// ------------------------- ToString -----------------------------
		public override string ToString( )
		{
			return mCmds ;
		}

	} // end class TextStmt


}
