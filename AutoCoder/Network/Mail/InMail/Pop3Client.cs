using System;
using System.Collections;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Text;
using System.Text.RegularExpressions;
using System.Diagnostics;
using AutoCoder.Network ;
using AutoCoder.Network.Mail ;
using AutoCoder.Text ;

namespace AutoCoder.Network.Mail.InMail
{
	public class Pop3Client
	{
		AutoCoder.Network.Logger mLogger ;
		ServerConnection mServerConnection ;
		private Pop3Credential m_credential ;
		bool		mUseSecureConnection = false ; 

		int mPortNx = 110 ;
		private const int MAX_BUFFER_READ_SIZE = 256 ;
		
		private long m_inboxPosition = 0 ;
		private long m_directPosition = -1 ;

		SocketExchange		mSockEx ;
 
		private Pop3Message m_pop3Message = null;

		public Pop3Credential UserDetails
		{
			set { m_credential = value; }
			get { return m_credential; }
		}

		public string From
		{
			get { return m_pop3Message.From; }
		}

		public int Port
		{
			get { return mPortNx ; }
			set { mPortNx = value ; }
		}

		public string To
		{
			get { return m_pop3Message.To; }
		}

		public string Subject
		{
			get { return m_pop3Message.Subject; }
		}

		public SocketExchange SockEx
		{
			get
			{
				if ( mSockEx == null )
					throw( new MailException(
						"No connection to server. Use ConnectAndLogin method." )) ;
				else
					return mSockEx ;
			}
		}

		public string Body
		{
			get { return m_pop3Message.Body; }
		}

		public bool UseSecureConnection
		{
			get { return mUseSecureConnection ; }
			set { mUseSecureConnection = value ; }
		}

		public IEnumerator MultipartEnumerator
		{
			get { return m_pop3Message.MultipartEnumerator; }
		}

		public bool IsMultipart
		{
			get { return m_pop3Message.IsMultipart; }
		}

		// logging property set by the caller. 
		public AutoCoder.Network.Logger Logger
		{
			get { return mLogger ; }
			set { mLogger = value ; }
		}

		// ----------------------- events -----------------------------------
		public event MailEventHandler Connected ;
		public event MailEventHandler Authenticated ;

		// ------------------------- Pop3Client constructor --------------------
		public Pop3Client(string user, string pass, string server)
		{
			m_credential = new Pop3Credential(user,pass,server);
		}

		public Pop3Client( ServerConnection InServerConn )
		{
			mServerConnection = InServerConn ;
			m_credential = new Pop3Credential(
				mServerConnection.AccountName,
				mServerConnection.Password,
				mServerConnection.ServerName ) ;
			UseSecureConnection = mServerConnection.SecureConnect ;
			Port = mServerConnection.PortNx ;
		}

		private Socket GetClientSocket( )
		{
			Socket s = null;
			
			try
			{
				IPHostEntry hostEntry = null;
        
				// Get host related information.
				hostEntry = Dns.GetHostEntry(m_credential.Server);

				// Loop through the AddressList to obtain the supported 
				// AddressFamily. This is to avoid an exception that 
				// occurs when the host IP Address is not compatible 
				// with the address family 
				// (typical in the IPv6 case).
				
				foreach(IPAddress address in hostEntry.AddressList)
				{
					IPEndPoint ipe = new IPEndPoint(address, Port ) ;
				
					Socket tempSocket = 
						new Socket(ipe.AddressFamily, 
						SocketType.Stream, ProtocolType.Tcp);

					tempSocket.Connect(ipe);

					if(tempSocket.Connected)
					{
						// we have a connection.
						// return this socket ...
						s = tempSocket;
						break;
					}
					else
					{
						continue;
					}
				}
			}
			catch(Exception e)
			{
				throw new Pop3ConnectException(e.ToString());
			}

			// throw exception if can't connect ...
			if(s == null)
			{
				throw new Pop3ConnectException("Error : connecting to "
					+m_credential.Server);
			}
			
			return s;
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

		// -------------------- MailServerLogin --------------------------------
		private void MailServerLogin( )
		{
			// send username ...
			mSockEx.ExpectedResponseCodes = new ExpectedResponseCodes( PopConstants.Ok ) ;
			mSockEx.SendReceive( "user " + m_credential.User + PopConstants.CrLf ) ;
			if ( mSockEx.ExpectedResponseCode == null )
				mSockEx.ThrowUnexpectedResponse( ) ;

			// send password
			mSockEx.ExpectedResponseCodes = new ExpectedResponseCodes( PopConstants.Ok ) ;
			mSockEx.SendReceive( "pass " + m_credential.Pass + PopConstants.CrLf ) ;
			mSockEx.ThrowIfUnexpectedResponse( ) ;
		}

		public long MessageCount
		{
			get 
			{
				long count = 0 ;

				SockEx.ExpectedResponseCodes =	new ExpectedResponseCodes( PopConstants.Ok ) ;
				SockEx.SendReceive( "stat" ) ;
				SockEx.ThrowIfUnexpectedResponse( ) ;
				string returned = SockEx.ResponseMessage ;

				// if values returned ...
				if( Regex.Match(returned,
					@"^.*\+OK[ |	]+([0-9]+)[ |	]+.*$").Success )
				{
						// get number of emails ...
						count = long.Parse( Regex
						.Replace(returned.Replace("\r\n","")
						, @"^.*\+OK[ |	]+([0-9]+)[ |	]+.*$" ,"$1") );
				}

				return(count);
			}
		}

		public void CloseConnection()
		{			
			SockEx.Send( "quit" ) ;
			mSockEx = null ;
			m_pop3Message = null;
		}

		// --------------------------- ConnectAndLogin -----------------------------
		public void ConnectAndLogin( )
		{
			// connect to the mail server.
			mSockEx = new SocketExchange(
				m_credential.Server,
				Port,
				Logger ) ;

			if ( UseSecureConnection == true )
				mSockEx.SecureConnect( ) ;
			else
				mSockEx.Connect( ) ;

			SignalConnectedEvent( new MailEventArgs( MailEvent.Connected )) ;

			// receive initial connection response from mail server.
			mSockEx.ExpectedResponseCodes = new ExpectedResponseCodes( PopConstants.Ok ) ;
			mSockEx.Receive( ) ;
			mSockEx.ThrowIfUnexpectedResponse( ) ;

			// send login details ...
			MailServerLogin( ) ;
		}

		public bool DeleteEmail()
		{
			bool ret = false;

			SockEx.ExpectedResponseCodes =	new ExpectedResponseCodes( PopConstants.Ok ) ;
			SockEx.SendReceive( "dele " + m_inboxPosition ) ;
			SockEx.ThrowIfUnexpectedResponse( ) ;
			string returned = SockEx.ResponseMessage ;

			if( Regex.Match(returned,
				@"^.*\+OK.*$").Success )
			{
				ret = true;
			}

			return ret;
		}

		// ---------------------- ResponseIsPopTerminated -----------------------
		public bool ResponseIsPopTerminated( StringBuilder InResponse )
		{
			return( Stringer.EndsWith(
				InResponse, NetworkConstants.CrLf + "." + NetworkConstants.CrLf )) ;
		}

		// --------------------------- RunList --------------------------------
		// run the List command on the server
		public MailDropMessages RunList( )
		{
			MailDropMessages messages = new MailDropMessages( ) ;
			string[] listLines = null ;
			SockEx.SendReceive( "LIST" + PopConstants.CrLf ) ;
			while( true )
			{
				if ( ResponseIsPopTerminated( SockEx.ResponseBuilder ) == true )
					break ;
				SockEx.SleepThenReadMoreAvailableData( 1000 ) ;
			}
			listLines = Stringer.Split( SockEx.ResponseMessage, NetworkConstants.CrLf ) ;

			// parse the list line output into an arraylist of MailDropMessages. 
			for( int Ix = 0 ; Ix < listLines.Length ; ++Ix )
			{
				string line = listLines[Ix] ;
				if (( line[0] == '+' ) || ( line[0] == '.' ))
					continue ;
				messages.AddMessage( new MailDropMessage( line )) ;
			}

			return messages ;
		}

		public Pop3Message RunRetr( Int64 InMessageNx )
		{
			Pop3Message msg = null ;
			msg = new Pop3Message( InMessageNx, mSockEx ) ;
			return msg ;
		}

		public string RunStat( )
		{
			SockEx.SendReceive( "STAT" + PopConstants.CrLf ) ;
			if ( SockEx.ResponseMessageEndsWithCrLf( ) == false )
        SockEx.SleepThenReadMoreAvailableData( 1000 ) ;
			return SockEx.ResponseMessage ;
		}

		public bool NextEmail( long directPosition)
		{
			bool ret;

			if( directPosition >= 0 )
			{
				m_directPosition = directPosition;
				ret = NextEmail( ) ;
			}
			else
			{
				throw new Pop3MessageException("Position less than zero");
			}

			return ret;
		}

		// ----------------------- NextEmail ----------------------------------
		public bool NextEmail( )
		{
			string returned;

			long pos;

			if ( m_directPosition == -1 )
			{
				if( m_inboxPosition == 0 )
				{
					pos = 1;
				}
				else
				{
					pos = m_inboxPosition + 1;
				}
			}
			else
			{
				pos = m_directPosition+1;
				m_directPosition = -1;
			}

			SockEx.ExpectedResponseCodes =
				new ExpectedResponseCodes( PopConstants.Ok, PopConstants.Error ) ;
			SockEx.SendReceive( "list " + pos.ToString( ) + PopConstants.CrLf ) ;
			if ( SockEx.ExpectedResponseCode == PopConstants.Error )
				return false ;
			else
				SockEx.ThrowIfUnexpectedResponse( ) ;

			// positive response: +ok MessagePosition MessageSize.
			returned = SockEx.ResponseMessage ; 

			m_inboxPosition = pos;

			// strip out CRLF ...
			string[] noCr = returned.Split(new char[]{ '\r' });

			// get size ...
			string[] elements = noCr[0].Split(new char[]{ ' ' });

			System.Int64 size = System.Int64.Parse( elements[2] ) ;

			// ... else read email data
			m_pop3Message = new Pop3Message( m_inboxPosition, mSockEx ) ;

			return true;
		}
	}
}
