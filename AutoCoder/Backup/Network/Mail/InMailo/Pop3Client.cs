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
using Org.Mentalis.Security.Ssl;
using Org.Mentalis.Security.Certificates;

namespace AutoCoder.Network.Mail.InMailo
{
	public class Pop3Client
	{
		AutoCoder.Network.Logger mLogger ;
		private Pop3Credential m_credential;
		bool		mUseSecureConnection = false ; 

		int mPortNx = 110 ;
		private const int MAX_BUFFER_READ_SIZE = 256;
		
		private long m_inboxPosition = 0;
		private long m_directPosition = -1;

		private Socket m_socket = null ;
		private SecureSocket mSecureSocket = null ;
 
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

		private Socket GetClientSocket( )
		{
			Socket s = null;
			
			try
			{
				IPHostEntry hostEntry = null;
        
				// Get host related information.
				hostEntry = Dns.Resolve(m_credential.Server);

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
		private void MailServerLogin( SocketExchange InEx )
		{
			// send username ...
			InEx.ExpectedResponseCodes = new ExpectedResponseCodes( PopConstants.Ok ) ;
			InEx.SendReceive( "user " + m_credential.User + PopConstants.CrLf ) ;
			if ( InEx.ExpectedResponseCode == null )
				InEx.ThrowUnexpectedResponse( ) ;

			// send password
			InEx.ExpectedResponseCodes = new ExpectedResponseCodes( PopConstants.Ok ) ;
			InEx.SendReceive( "pass " + m_credential.Pass + PopConstants.CrLf ) ;
			InEx.ThrowIfUnexpectedResponse( ) ;
		}

		// ------------------------- Send ----------------------------
		//send the data to server
		private void Send( string InData ) 
		{
			if ( m_socket == null )
			{
				throw new Pop3MessageException( "Pop3 connection is closed" ) ;
			}

			try
			{
				// Convert the string data to byte data 
				// using ASCII encoding.
				byte[] byteData = Encoding.ASCII.GetBytes( InData + PopConstants.CrLf ) ;
				
				// Begin sending the data to the remote device.
				m_socket.Send( byteData ) ;
			}
			catch(Exception e)
			{
				throw new Pop3SendException(e.ToString());
			}
		}

		private string GetPop3String()
		{
			if(m_socket == null)
			{
				throw new 
					Pop3MessageException("Connection to POP3 server is closed");
			}

			byte[] buffer = new byte[MAX_BUFFER_READ_SIZE];
			string line = null;

			try
			{
				int byteCount = 
					m_socket.Receive(buffer,buffer.Length,0);

				line = 
					Encoding.ASCII.GetString(buffer, 0, byteCount);
			}
			catch(Exception e)
			{
				throw new Pop3ReceiveException(e.ToString());
			}

			return line;
		}

		public long MessageCount
		{
			get 
			{
				long count = 0;
			
				if(m_socket==null)
				{
					throw new Pop3MessageException("Pop3 server not connected");
				}

				Send("stat");

				string returned = GetPop3String();

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
			Send("quit");

			m_socket = null;
			m_pop3Message = null;
		}

		public bool DeleteEmail()
		{
			bool ret = false;

			Send("dele "+m_inboxPosition);

			string returned = GetPop3String();

			if( Regex.Match(returned,
				@"^.*\+OK.*$").Success )
			{
				ret = true;
			}

			return ret;
		}

		public bool NextEmail( SocketExchange InEx, long directPosition)
		{
			bool ret;

			if( directPosition >= 0 )
			{
				m_directPosition = directPosition;
				ret = NextEmail( InEx ) ;
			}
			else
			{
				throw new Pop3MessageException("Position less than zero");
			}

			return ret;
		}

		// ----------------------- NextEmail ----------------------------------
		public bool NextEmail( SocketExchange InEx )
		{
			string returned;

			long pos;

			if(m_directPosition == -1)
			{
				if(m_inboxPosition == 0)
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

			InEx.ExpectedResponseCodes =
				new ExpectedResponseCodes( PopConstants.Ok, PopConstants.Error ) ;
			InEx.SendReceive( "list " + pos.ToString( ) + PopConstants.CrLf ) ;
			if ( InEx.ExpectedResponseCode == PopConstants.Error )
				return false ;
			else
				InEx.ThrowIfUnexpectedResponse( ) ;
			returned = InEx.ResponseMessage ; 

			m_inboxPosition = pos;

			// strip out CRLF ...
			string[] noCr = returned.Split(new char[]{ '\r' });

			// get size ...
			string[] elements = noCr[0].Split(new char[]{ ' ' });

			long size = long.Parse(elements[2]);

			// ... else read email data
			m_pop3Message = new Pop3Message( m_inboxPosition, size, m_socket, InEx ) ;

			return true;
		}

		// --------------------------- OpenInBox -----------------------------
		public SocketExchange OpenInbox( )
		{
			// connect to the mail server.
			SocketExchange sockEx = new SocketExchange(
				m_credential.Server,
				Port,
				Logger ) ;

			if ( UseSecureConnection == true )
			{
				sockEx.SecureConnect( ) ;
				mSecureSocket = sockEx.ConnectedSecureSocket ;
			}
			else
			{
				sockEx.Connect( ) ;
				m_socket = sockEx.ConnectedSocket ;
			}
			SignalConnectedEvent( new MailEventArgs( MailEvent.Connected )) ;

			// receive initial connection response from mail server.
			sockEx.ExpectedResponseCodes = new ExpectedResponseCodes( PopConstants.Ok ) ;
			sockEx.Receive( ) ;
			sockEx.ThrowIfUnexpectedResponse( ) ;

			// send login details ...
			MailServerLogin( sockEx ) ;

			return sockEx ;
		}
	}
}
