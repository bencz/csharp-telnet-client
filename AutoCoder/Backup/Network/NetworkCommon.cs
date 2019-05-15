using System;
using System.Collections ;
using System.Text ;

namespace AutoCoder.Network
{

	// ------------------------- enum NetworkRole ------------------------------
	// possible name:  NetworkWho
	// other possible values:  .Info  ( info message placed in the NetworkLogger )
	public enum NetworkRole { Client, Server, Exception, CaughtException, None } ;

	// --------------------------- enum ConnectProtocol -------------------------
	/// <summary>
	/// connection protocol
	/// </summary>
 	public enum ConnectProtocol { Pop3, Smtp, Ftp, None } ;
 
	// -------------------------- NetworkCommon --------------------------
	public class NetworkCommon
	{
		public NetworkCommon( )
		{
		}

		/// <summary>
		/// Dump the stream like value of a string in a way that makes its contents
		/// more visible. 
		/// </summary>
		/// <param name="InValue"></param>
		/// <returns></returns>
		public static ArrayList DumpNetworkStream( string InValue )
		{
			StringBuilder sb = new StringBuilder( ) ;
			int Lx = InValue.Length ;
			ArrayList results = new ArrayList( ) ;
			for( int Ix = 0 ; Ix < Lx ; ++Ix )
			{
				char ch1 = InValue[Ix] ;
				if ( ch1 == '\r' )
					sb.Append( "<cr>" ) ;
				else if ( ch1 == '\n' )
					sb.Append( "<lf>" ) ;
				else if ( ch1 == '\t' )
					sb.Append( "<tab>" ) ;
				else if ( ch1 == ' ' )
					sb.Append( "< >" ) ;
				else 
					sb.Append( ch1 ) ;

				if (( sb.Length > 80 )
					|| ( ch1 == '\n' ))
				{
					results.Add( sb.ToString( )) ;
					sb = new StringBuilder( ) ;
				}
			}

			if ( sb.Length > 0 )
				results.Add( sb.ToString( )) ;
			return results ;
		}
	}

	// ------------------------ ExpectedResponseCodes -------------------------
	public class ExpectedResponseCodes : ArrayList
	{
		// ----------------------- constructor --------------------------
		public ExpectedResponseCodes( )
		{
		}
		public ExpectedResponseCodes( string InCode )
		{
			Add( InCode ) ;
		}
		public ExpectedResponseCodes( string InCode0, string InCode1 )
		{
			Add( InCode0 ) ;
			Add( InCode1 ) ;
		}

		public ExpectedResponseCodes Add( string InCode )
		{
			base.Add( InCode ) ;
			return this ;
		}

		// ----------------------- FindResponseCode --------------------------
		// find the expected response code in the message.
		public string FindResponseCode( string InMessage )
		{
			string foundCode = null ;
			foreach( string code in this )
			{
				if ( InMessage.StartsWith( code ) == true )
				{
					foundCode = code ;
					break ;
				}
			}
			return foundCode ;
		}

		// ---------------------------- ToString ---------------------------
		public override string ToString( )
		{
			StringBuilder sb = new StringBuilder( 80 ) ;
			foreach( string code in this )
			{
				if ( sb.Length > 0 )
					sb.Append( ", " ) ;
				sb.Append( code ) ;
			}
			return sb.ToString( ) ;
		}

	}

	// ---------------------------- NetworkConstants ---------------------------
	public class NetworkConstants
	{
		public static readonly string CrLf = "\r\n" ;
	}

	// ------------------------- NetworkException ---------------------------
	public class NetworkException : ApplicationException 
	{
		public NetworkException( string InMessage )
			:	base( InMessage )
		{
		}
		public NetworkException( string InMessage, System.Exception InCaughtExcp )
			: base( InMessage, InCaughtExcp )
		{
		}   
	}
}
