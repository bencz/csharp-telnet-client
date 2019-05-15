using System;

namespace AutoCoder.TextStmt
{

	// ------------------------ TextScanException ---------------------------
	public class TextScanException : ApplicationException
	{
		public TextScanException( string InMethod, string InMessage )
			: base( InMethod + ". " + InMessage )
		{
		}
	}

	// -------------------------- AssignStmtContents -------------------
	// the contents of an assignment statement. 
	public class AssignStmtContents
	{
		public string Lhs ;
		public string Rhs ;
		public string Operator ;
	}

	// -------------------------- TextStmtWord ------------------------
	public class TextStmtWord
	{
		TextStmt mCmds ;
		int mWordBx ;
		int mWordLx ;

		public TextStmtWord( TextStmt InCmds, int InWordBx, int InWordLx )
		{
			mCmds = InCmds ;
			mWordBx = InWordBx ;
			mWordLx = InWordLx ;
		}

		public int WordBx
		{
			get { return mWordBx ; }
		}
		public int WordLx
		{
			get { return mWordLx ; }
		}

		public override string ToString( )
		{
			return mCmds.Substring( mWordBx, mWordLx ) ;
		}
	}

}
