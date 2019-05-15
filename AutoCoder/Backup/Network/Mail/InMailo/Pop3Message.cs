using System;
using System.Collections;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Text;
using System.Text.RegularExpressions;
using System.Diagnostics;
using AutoCoder.Text ;
using AutoCoder.Network ;
using Org.Mentalis.Security.Ssl;
using Org.Mentalis.Security.Certificates;

namespace AutoCoder.Network.Mail.InMailo
{
	/// <summary>
	/// DLM: Stores the From:, To:, Subject:, body and attachments
	/// within an email. Binary attachments are Base64-decoded
	/// </summary>

	public class Pop3Message
	{
		private Socket mSocket ;
		SocketExchange mSockEx ;

		private Pop3MessageComponents m_messageComponents;

		private string m_from;
		private string m_to;
		private string m_subject;
		private string m_contentType;
		private string m_body;

		private bool m_isMultipart = false;

		private string m_multipartBoundary;
		
		private const int m_fromState=0;
		private const int m_toState=1;
		private const int m_subjectState = 2;
		private const int m_contentTypeState = 3;
		private const int m_notKnownState = -99;
		private const int m_endOfHeader = -98;

		// this array corresponds with above
		// enumerator ...

		private string[] m_lineTypeString =
		{
			"From",
			"To",
			"Subject",
			"Content-Type"
		};
		
		private long m_messageSize = 0;
		private long m_inboxPosition = 0;

		Pop3StateObject mSockThreadState = null ;

		// manual reset event.  init to "not signaled"
		ManualResetEvent m_manualEvent = new ManualResetEvent(false) ;

		// --------------------------- constructor -----------------------
		public Pop3Message(
			long position, long size, Socket client,
			SocketExchange InEx )
		{
			m_inboxPosition = position;
			m_messageSize = size;
			mSocket = client;
			mSockEx = InEx ;

			// object used to receive the mail message in a background thread.
			mSockThreadState = new Pop3StateObject( ) ;
			mSockThreadState.SockEx = InEx ;
			mSockThreadState.sb = new StringBuilder( ) ;

			// load email ...
			LoadEmail( InEx ) ;

			// get body (if it exists) ...
			IEnumerator multipartEnumerator =	MultipartEnumerator ;

			while( multipartEnumerator.MoveNext() )
			{
				Pop3Component multipart = (Pop3Component)
					multipartEnumerator.Current;

				if( multipart.IsBody )
				{
					m_body = multipart.Data;
					break;
				}
			}
		}

		// ------------------------- Properties ------------------------------
		public IEnumerator MultipartEnumerator
		{
			get { return m_messageComponents.ComponentEnumerator; }
		}

		public bool IsMultipart
		{
			get { return m_isMultipart; }
		}

		public string From
		{
			get { return m_from; }
		}

		public string To
		{
			get { return m_to; }
		}

		public string Subject
		{
			get { return m_subject; }
		}

		public string Body
		{
			get { return m_body; }
		}

		public long InboxPosition
		{
			get { return m_inboxPosition; }
		}

		// ---------------------- LoadEmail ---------------------------
		private void LoadEmail( SocketExchange InEx )
		{
			// tell the server we want to read the message.
			InEx.Send( "retr " + m_inboxPosition + PopConstants.CrLf ) ;

			// receive in a background thread.
			StartReceive( InEx ) ;
			InEx.LoadResponseMessage( mSockThreadState.sb.ToString( )) ;

			// parse email ...
			string[] lines = InEx.ResponseMessage.Split( Chars.CharArrayChars( '\r' )) ;
			for( int ix = 0 ; ix < lines.Length ; ++ix )
			{
				InEx.Logger.AddMessage( NetworkRole.Server, lines[ix] ) ;
			}
			ParseEmail( lines ) ;

			// remove reading pop3State ...
			mSockThreadState = null ;
		}

		private int GetHeaderLineType(string line)
		{
			int lineType = m_notKnownState;

			for(int i=0; i<m_lineTypeString.Length; i++)
			{
				string match = m_lineTypeString[i];

				if( Regex.Match(line,"^"+match+":"+".*$").Success )
				{
					lineType = i;
					break;
				}
				else
				if( line.Length == 0 )
				{
					lineType = m_endOfHeader;
					break;
				}
			}

			return lineType;
		}
		
		// ------------------------ ParseEmail ----------------------------------
		// the lines of the email message are contained in the input array.
		// Parse and pull the parts of the message of relvance to the user.
		private void ParseEmail( string[] InLines )
		{
			long startOfBody = ParseHeader( InLines ) ;
			long numberOfLines = InLines.Length;

			m_messageComponents = 
				new Pop3MessageComponents(
				InLines,
				startOfBody,
				m_multipartBoundary,
				m_contentType ) ;
		}

		// -------------------------- ParseHeader --------------------------------
		private long ParseHeader(string[] lines)
		{
			int numberOfLines = lines.Length;
			long bodyStart = 0;

			for(int i=0; i<numberOfLines; i++)
			{
				string currentLine = lines[i].Replace("\n","");

				int lineType = GetHeaderLineType(currentLine);

				switch(lineType)
				{
						// From:
					case m_fromState:
						m_from = Pop3Parse.From(currentLine);	
						break;

						// Subject:
					case m_subjectState:
						m_subject =	Pop3Parse.Subject(currentLine);
						break;

						// To:
					case m_toState:
						m_to = Pop3Parse.To(currentLine);
						break;

						// Content-Type
					case m_contentTypeState:
							
						m_contentType = 
							Pop3Parse.ContentType(currentLine);
						
						m_isMultipart = 
							Pop3Parse.IsMultipart(m_contentType);

						if(m_isMultipart)
						{
							// if boundary definition is on next
							// line ...

							if(m_contentType
								.Substring(m_contentType.Length-1,1).
								Equals(";"))
							{
								++i;

								m_multipartBoundary
									= Pop3Parse.
									MultipartBoundary(lines[i].
									Replace("\n",""));
							}
							else
							{
								// boundary definition is on same
								// line as "Content-Type" ...

								m_multipartBoundary =
									Pop3Parse
									.MultipartBoundary(m_contentType);
							}
						}

						break;

					case m_endOfHeader:
						bodyStart = i+1;
						break;
				}

				if(bodyStart>0)
				{
					break;
				}
			}

			return(bodyStart);
		}

		// ------------------------- ReceiveCallback -------------------------
		// receive MailMessage in background thread.
		private void ReceiveCallback( IAsyncResult InResult ) 
		{
			try 
			{
				// Retrieve the state object and the client socket 
				// from the asynchronous state object.
				Pop3StateObject sockThreadState = 
					(Pop3StateObject) InResult.AsyncState;
				SocketExchange sockEx = sockThreadState.SockEx ;
				Socket client = sockEx.ConnectedSocket ;
				SecureSocket secureSock = sockEx.ConnectedSecureSocket ;

				bool InlRcv = true ;
				while( true )
				{
					// receive from the socket.
					int ReadCx ;
					if ( InlRcv == true )
					{
						if ( client != null )
							ReadCx = client.EndReceive( InResult ) ;
						else
							ReadCx = secureSock.EndReceive( InResult ) ;
					}
					else
					{
						if ( client != null )
							ReadCx = client.Receive(
								sockThreadState.buffer, 0, Pop3StateObject.BufferSize, SocketFlags.None ) ;
						else
							ReadCx = secureSock.Receive(
								sockThreadState.buffer, 0, Pop3StateObject.BufferSize, SocketFlags.None ) ;
					}
					InlRcv = false ;

					// did not receive anything.  probably should leave.
					if ( ReadCx == 0 )
						break ;

					// There might be more data, 
					// so store the data received so far.
					string block = Encoding.ASCII.GetString( sockThreadState.buffer, 0, ReadCx ) ;
					sockThreadState.sb.Append( block ) ;

					// is this the end of the message
					if ( Stringer.Tail( sockThreadState.sb, 5 ) == "\r\n.\r\n" ) 
						break ;
				}
			} 
			catch (Exception e) 
			{
				throw new 
					Pop3ReceiveException(
					"ReceiveCallback error" + 
					e.ToString( )) ;
			}
			finally
			{
				m_manualEvent.Set( ) ;
			}
		}

		// -------------------------------- Send ------------------------------
		//send the data to server
		private void Send(String data) 
		{
			try
			{
				// Convert the string data to byte data 
				// using ASCII encoding.
				
				byte[] byteData = Encoding.ASCII.GetBytes(data+"\r\n");
				
				// Begin sending the data to the remote device.
				mSocket.Send(byteData);
			}
			catch(Exception e)
			{
				throw new Pop3SendException(e.ToString());
			}
		}

		// ------------------------ StartReceive --------------------------
		private void StartReceive( SocketExchange InEx )
		{
			// set the sync event to non signaled.
			m_manualEvent.Reset( ) ;
			
			// start receiving data ...
			mSocket.BeginReceive(
				mSockThreadState.buffer,
				0,
				Pop3StateObject.BufferSize,
				0,
				new AsyncCallback( ReceiveCallback ),
				mSockThreadState ) ;

			// Block on the event.  The receive thread will signal when the receive is
			// complete.
			m_manualEvent.WaitOne( ) ;
		}

		// -------------------------- ToString ----------------------------
		public override string ToString()
		{
			IEnumerator enumerator = MultipartEnumerator;

			string ret = 
				"From    : "+m_from+ "\r\n"+
				"To      : "+m_to+ "\r\n"+
				"Subject : "+m_subject+"\r\n";

			while( enumerator.MoveNext() )
			{
				ret += ((Pop3Component)enumerator.Current).ToString()+"\r\n";
			}
	
			return ret;
		}
	}
}
