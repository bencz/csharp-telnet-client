using System;
using System.Collections ;
using System.Text ;

namespace AutoCoder.Network
{
	// --------------------------- LogLine ---------------------------
	public class LogLine
	{
		string mMessage ;
		DateTime mLogTs ;

		public LogLine( )
		{
		}
		public LogLine( string InMessage )
		{
			Assign( InMessage ) ;
		}

		public string Message
		{
			get { return mMessage ; }
		}
		public string MessageShowCodes
		{
			get { return CalcMessageShowCodes( ) ; }
		}
		public DateTime LogTs
		{
			get { return mLogTs ; }
		}

		public LogLine Assign( string InMessage )
		{
			mMessage = InMessage ;
			mLogTs = DateTime.UtcNow ;
			return this ; 
		}

		// ------------------- CalcMessageShowCodes ---------------------------
		// return the message string in a form that shows the cr, lf, and tab codes  
		private string CalcMessageShowCodes( )
		{
			string msg = Message ;
			StringBuilder sb = new StringBuilder( msg.Length ) ;
			for( int Ix = 0 ; Ix < msg.Length ; ++Ix )
			{
				char ch1 = msg[Ix] ;
				if ( ch1 == '\t' )
					sb.Append( "<TAB>" ) ;


				// a crlf followed by a space in a message header is a FOLD. mail agents 
				// interpret folds as whitespace.
				else if (( ch1 == '\r' )
					&& ( AcCommon.PullString( msg, Ix, 3, null ) == "\r\n " ))
				{
					sb.Append( "<FOLD>" ) ;
					Ix += 2 ;
				}

				else if (( ch1 == '\r' )
					&& ( AcCommon.PullString( msg, Ix, 2, null ) == NetworkConstants.CrLf ))
				{
					sb.Append( "<CRLF>" ) ;
					++Ix ;
				}
				else if ( ch1 == '\r' )
					sb.Append( "<CR>" ) ;
				else if ( ch1 == '\n' )
					sb.Append( "<LF>" ) ;
				else
					sb.Append( ch1 ) ;
			}
			return sb.ToString( ) ;
		}
	}
  
	// ------------------------- Logger --------------------------
	// used to provide a history log in ArrayList form.
	public class Logger : ArrayList
	{
		public Logger( )
		{
		}

		public Logger Add( LogLine InLine )
		{
			base.Add( InLine ) ;
			return this ;
		}
		public Logger Add( string InMessage )
		{
			LogLine line = new LogLine( InMessage ) ;
			Add( line ) ;
			return this ;
		}

		// --------------------------- AddMessage ---------------------------------
		public void AddMessage( NetworkRole InRole, string InFullMsg )
		{
			if ( InFullMsg.Length > 512 )
				Add( InRole.ToString( ) + " " + InFullMsg.Substring( 0, 512 ) + "..." ) ;
			else
				Add( InRole.ToString( ) + " " + InFullMsg ) ;
		}

	} // end class Logger
} // end namespace Network
