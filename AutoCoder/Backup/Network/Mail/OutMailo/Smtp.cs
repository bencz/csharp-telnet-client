using System ;
using System.Net ;
using System.Text ;
using System.Collections ;
using AutoCoder.Network ;
using AutoCoder.Network.Mail ;
using AutoCoder.Text ;

namespace AutoCoder.Network.Mail.OutMailo
{

	// ------------------------ Smtp ------------------------------
	public class Smtp
	{
		string mServerName ;
		int mPortNx ;
		string mUsername ;
		string mPassword ;
		object mServerRequiresAuthentication ;

		// ----------------- members used in the history log -----------------
		Logger mDetailLogger = null ;

		// --------------------- constructors -----------------------------
		public Smtp()
		{
			ConstructCommon( ) ;
		}

		public Smtp( string InServerName, string InUsername, string InPassword )
		{
			ConstructCommon( ) ;
			ServerName = InServerName ;
			Username = InUsername ;
			Password = InPassword ;
		}

		// ------------------------- Properties ---------------------------------
		public string ServerName
		{
			get { return mServerName ; }
			set { mServerName = value ; }
		}
		public bool ServerRequiresAuthentication
		{
			get { return CalcServerRequiresAuthentication( ) ; }
			set { mServerRequiresAuthentication = value ; }
		}
		public int Port
		{
			get { return mPortNx ; }
			set { mPortNx = value ; }
		}
		public string Username
		{
			get { return mUsername ; }
			set { mUsername = value ; }
		}
		public string Password
		{
			get { return mPassword ; }
			set { mPassword = value ; }
		}

		// ---------------- log to an ArrayList -------------------------
		public Logger DetailLogger
		{
			get { return mDetailLogger ; }
			set { mDetailLogger = value ; }
		}

		// ----------------------- events -----------------------------------
		public event MailEventHandler Connected ;
		public event MailEventHandler Authenticated ;
		public event MailEventHandler StartDataSend ;
		public event MailEventHandler EndDataSend ;
		public event MailEventHandler Disconnected ;

		// -------------------------- AuthenticateToServer -------------------------
		private void AuthenticateToServer( SocketExchange InExchange )
		{
			string message = null ;

			// log the base64 encoded string that follows the 334 challenge.
			string ResponseData = InExchange.GetResponseData( ) ;
			byte[] msgBytes =	Convert.FromBase64String( ResponseData ) ;
			System.Text.Encoding encoding = System.Text.Encoding.UTF8 ;
			string msg = encoding.GetString( msgBytes ) ;
			LogMessage( NetworkRole.Client, msg ) ;
			
			// send the user name, expect back a challenge for the password.
			int LoopCx = 0 ;
			while( true )
			{
				message =
					Convert.ToBase64String(
					Encoding.ASCII.GetBytes( Username.ToCharArray( ))) +
					SmtpConstants.CrLf ;
				InExchange.ExpectedResponseCodes = new ExpectedResponseCodes(
					SmtpConstants.Challenge, SmtpConstants.SyntaxError ) ;
				InExchange.SendReceive( message ) ;
				if ( InExchange.ExpectedResponseCode == SmtpConstants.Challenge )
					break ;
				else if ( InExchange.ExpectedResponseCode == SmtpConstants.SyntaxError )
				{
					++LoopCx ;
					if ( LoopCx >= 3 )
						InExchange.ThrowUnexpectedResponse( SmtpConstants.Challenge ) ;
				}
				else
					InExchange.ThrowIfUnexpectedResponse( ) ;
			}

			// send the password. expect back 235 = login successful.
			message =
				Convert.ToBase64String(
				Encoding.ASCII.GetBytes( Password.ToCharArray( ))) +
				SmtpConstants.CrLf ;
			InExchange.ExpectedResponseCodes =
				new ExpectedResponseCodes( SmtpConstants.Authenticated ) ;
			InExchange.SendReceive( message ) ;
			InExchange.ThrowIfUnexpectedResponse( ) ;

			// is authenticated.
			SignalAuthenticatedEvent( new MailEventArgs( MailEvent.Authenticated )) ;
		}

		// ------------------------ CalcServerRequiresAuthentication ----------------
		// Server requires authentication if the property is explicitly set or if the
		// Username and Password properties are set.
		private bool CalcServerRequiresAuthentication( )
		{
			if ( mServerRequiresAuthentication != null )
				return (bool) mServerRequiresAuthentication ;
			else if (( Stringer.IsEmpty( Username ) == false ) &&
				( Stringer.IsEmpty( Password ) == false ))
				return true ;
			else
				return false ;
		}

		// -------------------------- ConstructCommon ----------------------------
		private void ConstructCommon( )
		{
			mPortNx = 25 ;
		}

		// --------------------------- LogMessage ---------------------------------
		private void LogMessage( NetworkRole InRole, string InFullMsg )
		{
			if ( DetailLogger != null )
			{
				DetailLogger.Add( InRole.ToString( ) + " " + InFullMsg ) ;
			}
		}
		
		// ------------------------------ SendMail --------------------------------
		public void SendMail( MailMessage msg)
		{
			// connect to the mail server.
			SocketExchange xchg = new SocketExchange( ServerName, Port, DetailLogger ) ;
			xchg.Connect( ) ;
			SignalConnectedEvent( new MailEventArgs( MailEvent.Connected )) ;

			// receive the initial server hello message.
			xchg.ExpectedResponseCodes = new ExpectedResponseCodes( SmtpConstants.ServerReady ) ; 
			xchg.Receive( ) ;
			xchg.ThrowIfUnexpectedResponse( ) ;

			// hello message exchange.
			xchg.ExpectedResponseCodes = new ExpectedResponseCodes( SmtpConstants.Ok ) ; 
			if ( ServerRequiresAuthentication == true )
				xchg.SendReceive( "EHLO " + Dns.GetHostName() + SmtpConstants.CrLf ) ;
			else
				xchg.SendReceive( "HELO " + Dns.GetHostName() + SmtpConstants.CrLf ) ;
			xchg.ThrowIfUnexpectedResponse( ) ;

			// got a 250, but did not get a 250-AUTH. receive more.
			if (( xchg.ResponseCode == SmtpConstants.Ok  ) &&
				( xchg.ResponseMessageContains( "AUTH" ) == false ))
			{
				LogMessage( NetworkRole.Client, "AUTH not received. Receive more." ) ;
				xchg.SleepThenReadMoreAvailableData( 300 ) ;
			}

			// authentication loop
			xchg.ReadMoreDelay = 0 ;
			if ( ServerRequiresAuthentication == true )
			{
				int okCount = 0 ;
				while( true )
				{
					xchg.ExpectedResponseCodes =
						new ExpectedResponseCodes( SmtpConstants.Ok, SmtpConstants.Challenge ) ; 
					xchg.SendReceive( "AUTH LOGIN" + SmtpConstants.CrLf ) ;
					xchg.ThrowIfUnexpectedResponse( ) ;
					if ( xchg.ResponseCode == SmtpConstants.Challenge )
					{
						AuthenticateToServer( xchg ) ;
						break ;
					}
					else if ( xchg.ResponseCode == SmtpConstants.Ok )
					{
						++okCount ;
						continue ;
					}
				}
			}

			// send the MAIL FROM message to the server.  This is the start of the sending
			// of the email message.
			xchg.ExpectedResponseCodes = 
				new ExpectedResponseCodes( SmtpConstants.Challenge, SmtpConstants.Ok ) ;
			xchg.SendReceive( "MAIL FROM: <" + msg.From.Address + ">" + SmtpConstants.CrLf ) ;
			xchg.ThrowIfUnexpectedResponse( ) ;
			if ( xchg.ResponseCode == SmtpConstants.Challenge )
				AuthenticateToServer( xchg ) ;

			// send "rcpt to" message to the mail server to validate the recipients.
			ServerValidateRecipients( xchg, msg.ToRecipients ) ;
			ServerValidateRecipients( xchg, msg.CCRecipients ) ;
			ServerValidateRecipients( xchg, msg.BCCRecipients ) ;

			// send the DATA message to the server.
			xchg.ExpectedResponseCodes = new ExpectedResponseCodes( SmtpConstants.DataReady ) ;
			xchg.SendReceive( "DATA" + SmtpConstants.CrLf ) ;
			xchg.ThrowIfUnexpectedResponse( ) ;

			// send the message itself.
			SignalStartDataSendEvent( new MailEventArgs( MailEvent.StartDataSend )) ;
			xchg.ExpectedResponseCodes = new ExpectedResponseCodes( SmtpConstants.Ok ) ;
			string dataMessage =
				msg.ToString( ) + SmtpConstants.CrLf + "." + SmtpConstants.CrLf ;
			xchg.SendReceive( dataMessage ) ;
			xchg.ThrowIfUnexpectedResponse( ) ;
			SignalEndDataSendEvent( new MailEventArgs( MailEvent.EndDataSend )) ;

			// send quit message.
			xchg.ExpectedResponseCodes = new ExpectedResponseCodes( SmtpConstants.Quit ) ;
			xchg.SendReceive( "QUIT" + SmtpConstants.CrLf ) ;
			xchg.ThrowIfUnexpectedResponse( ) ;

			// close the connection.
			SignalDisconnectedEvent( new MailEventArgs( MailEvent.Disconnected )) ;
			xchg.CloseConnection( ) ;
		}
				
		// ------------------------- ServerValidateRecipients ------------------------
		// Send "rcpt to" message to the mail server to validate each recipient.
		private void ServerValidateRecipients(
			SocketExchange InSock, ArrayList InRecipients )
		{
			IEnumerator it = InRecipients.GetEnumerator( ) ;
			while( it.MoveNext( ) == true )
			{
				EmailAddress recipient = (EmailAddress) it.Current ;
				string message = 
					"RCPT TO: <" + recipient.Address + ">" + SmtpConstants.CrLf ;
				InSock.ExpectedResponseCodes = new ExpectedResponseCodes( SmtpConstants.Ok ) ;
				InSock.SendReceive( message ) ;
				InSock.ThrowIfUnexpectedResponse( ) ;
			}	
		}

		// ---------------------- signal events ----------------------------------
		// ( if user code contains an "OnEvent" method. )
		void SignalConnectedEvent( MailEventArgs InArgs )
		{
			if ( Connected != null )
				Connected( this, InArgs ) ;
		}
		
		void SignalAuthenticatedEvent( MailEventArgs InArgs )
		{
			if ( Authenticated != null )
				Authenticated( this, InArgs ) ;
		}
		
		void SignalStartDataSendEvent( MailEventArgs InArgs )
		{
			if ( StartDataSend != null )
				StartDataSend( this, InArgs ) ;
		}
		
		void SignalEndDataSendEvent( MailEventArgs InArgs )
		{
			if ( EndDataSend != null )
				EndDataSend( this, InArgs ) ;
		}
		
		void SignalDisconnectedEvent( MailEventArgs InArgs )
		{
			if ( Disconnected != null )
				Disconnected( this, InArgs ) ;
		}
		
		// ------------------------ ThrowException ------------------------------
		private void ThrowException( string InMessage )
		{
			LogMessage( NetworkRole.Client, InMessage ) ;
			throw( new MailException( InMessage )) ;
		}
		private void ThrowException( string InMessage, Exception InCaughtExcp )
		{
			LogMessage( NetworkRole.CaughtException, InCaughtExcp.Message ) ;
			LogMessage( NetworkRole.Exception, InMessage ) ;
			throw( new MailException( InMessage, InCaughtExcp )) ;
		}

		private bool CheckMailMessage( MailMessage InMessage )
		{
			if (( InMessage.ToRecipients == null ) || ( InMessage.ToRecipients.Count <= 0 ))
				ThrowException( "MailMessage is missing \"To\" recipients" ) ;
			return true ;
		}

	}
}