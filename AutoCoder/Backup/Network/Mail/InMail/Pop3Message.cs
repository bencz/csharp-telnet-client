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

namespace AutoCoder.Network.Mail.InMail
{
	/// <summary>
	/// DLM: Stores the From:, To:, Subject:, body and attachments
	/// within an email. Binary attachments are Base64-decoded
	/// </summary>

	public class Pop3Message
	{
		SocketExchange mSockEx = null ;
		MimeMessagePartList mParts = null ;
		MimeAttachmentList mAttachments = null ;

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
		
		private System.Int64 m_inboxPosition = 0;

		SockThreadState mSockThreadState = null ;

		// manual reset event.  init to "not signaled"
		ManualResetEvent m_manualEvent = new ManualResetEvent(false) ;

		// --------------------------- constructor -----------------------
		public Pop3Message(
			System.Int64 InMailDropPosition,
			SocketExchange InEx )
		{
			m_inboxPosition = InMailDropPosition ;
			mSockEx = InEx ;

			// object used to receive the mail message in a background thread.
			mSockThreadState = new SockThreadState( ) ;
			mSockThreadState.SockEx = InEx ;
			mSockThreadState.sb = new StringBuilder( ) ;

			// load email ...
			LoadEmail( InEx, InMailDropPosition ) ;

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
		public MimeAttachmentList Attachments
		{
			get
			{
				if ( mAttachments == null )
				{
					mAttachments = new MimeAttachmentList( ) ;
					mAttachments.BuildList( mParts ) ;
				}
				return mAttachments ;
			}
		}

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

		public System.Int64 InboxPosition
		{
			get { return m_inboxPosition; }
		}

		// ---------------------- LoadEmail ---------------------------
		private void LoadEmail( SocketExchange InEx, System.Int64 InMailDropPosition )
		{
			// tell the server we want to read the message.
			InEx.Send( "retr " + InMailDropPosition + PopConstants.CrLf ) ;

			InEx.ExpectedResponseCodes =
        new ExpectedResponseCodes( PopConstants.Ok, PopConstants.Error ) ;
			InEx.SendReceive( "retr " + InMailDropPosition + PopConstants.CrLf ) ;
			InEx.ThrowIfUnexpectedResponse( ) ;

			// -ERR response. no such message number.
			if ( InEx.ResponseCode == PopConstants.Error )
			{

			}

			else
			{

				// for now, receive SSL link messages in the same thread.
				if ( InEx.ConnectedSecureSocket != null )
				{
					StringBuilder mail = null ;
					mail = ReceiveMessage_SameThread( InEx ) ;
					InEx.LoadResponseMessage( mail.ToString( )) ;
				}

					// receive in a background thread.
				else
				{
					StartReceive( InEx ) ;
					InEx.LoadResponseMessage( mSockThreadState.sb.ToString( )) ;
				}
			}

			// parse email ...
			mParts = MimeCommon.MessagePartSplitter( InEx.ResponseMessage ) ;

			string[] lines = MimeCommon.MessageLineSplitter( InEx.ResponseMessage ) ;
			if ( 1 == 2 )
			{
				mParts = MimeCommon.MessagePartSplitter( lines ) ;
			}

			mAttachments = null ;
			MimeAttachment attach = (MimeAttachment) Attachments.FirstAttachment( ).Current  ;
			if ( attach != null )
			{
				attach.SaveAs( "c:\\apress\\attachment.txt" ) ;
			}

			if ( 1 == 2 )
			{
				// dump the header lines of the top part.
				InEx.Logger.AddMessage( NetworkRole.Server, "--- start header line dump ---" ) ;
				MimeTopPart topPart = ( MimeTopPart ) mParts.GetTopPart( ) ;
				for( int Ix = 0 ; Ix < topPart.PropertyLines.Length ; ++Ix )
				{
					InEx.Logger.AddMessage( NetworkRole.Server, topPart.PropertyLines[Ix] ) ;
				}
				for( int Ix = 0 ; Ix < topPart.MessageLines.Length ; ++Ix )
				{
					InEx.Logger.AddMessage( NetworkRole.Server, topPart.MessageLines[Ix] ) ;
				}
				InEx.Logger.AddMessage( NetworkRole.Server, "--- end of header line dump ---" ) ;

				// dump the lines of each part.
				foreach( MimeMessagePart part in mParts )
				{
					InEx.Logger.AddMessage( NetworkRole.Server, "** message part **" ) ;
					InEx.Logger.AddMessage( NetworkRole.Server, "** property lines **" ) ;
					foreach( string line in part.PropertyLines )
					{
						InEx.Logger.AddMessage( NetworkRole.Server, line ) ;
					}
					InEx.Logger.AddMessage( NetworkRole.Server, "** message lines **" ) ;
					foreach( string line in part.MessageLines )
					{
						InEx.Logger.AddMessage( NetworkRole.Server, line ) ;
					}
				}
				InEx.Logger.AddMessage( NetworkRole.Server, "---- end of parts ----------" ) ;

			}

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
			System.Int64 startOfBody = ParseHeader( InLines ) ;
			System.Int64 numberOfLines = InLines.Length;

			m_messageComponents = 
				new Pop3MessageComponents(
				InLines,
				startOfBody,
				m_multipartBoundary,
				m_contentType ) ;
		}

		// -------------------------- ParseHeader --------------------------------
		private System.Int64 ParseHeader(string[] lines)
		{
			int numberOfLines = lines.Length;
			System.Int64 bodyStart = 0;

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
				SockThreadState sockThreadState = 
					(SockThreadState) InResult.AsyncState;
				SocketExchange sockEx = sockThreadState.SockEx ;
				Socket client = sockEx.ConnectedSocket ;
				SecureSocket secureSock = sockEx.ConnectedSecureSocket ;
				int BufferSx = SockThreadState.BufferSize ;

				int LoopCx = 0 ;
				while( true )
				{
					// a simple loop count.  dont let it overflow.
					++LoopCx ;
					if ( LoopCx > 1000 )
					{
						LoopCx = 2 ;
					}

					// receive from the socket.
					int ReadCx ;
					if ( LoopCx == 1 )
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
								sockThreadState.buffer, 0, BufferSx, SocketFlags.None ) ;
						else
							ReadCx = secureSock.Receive(
								sockThreadState.buffer, 0, BufferSx, SocketFlags.None ) ;
					}

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

		// ------------------------ StartReceive --------------------------
		private void StartReceive( SocketExchange InEx )
		{
			Socket socket = InEx.ConnectedSocket ;
			
			// set the sync event to non signaled.
			m_manualEvent.Reset( ) ;
			
			// start receiving data ...
			if ( socket != null )
			{
				socket.BeginReceive(
					mSockThreadState.buffer,
					0,
					SockThreadState.BufferSize,
					0,
					new AsyncCallback( ReceiveCallback ),
					mSockThreadState ) ;
			}
			else
			{
				InEx.ConnectedSecureSocket.BeginReceive(
					mSockThreadState.buffer,
					0,
					SockThreadState.BufferSize,
					0,
					new AsyncCallback( ReceiveCallback ),
					mSockThreadState ) ;
			}

			// Block on the event.  The receive thread will signal when the receive is
			// complete.
			m_manualEvent.WaitOne( ) ;
		}

		// ------------------------- ReceiveMessage_SameThread -------------------------
		private StringBuilder ReceiveMessage_SameThread( SocketExchange InSockEx ) 
		{
			StringBuilder sb = new StringBuilder( ) ;
			int BufferSx = 512 ;
			byte[] buffer = new byte[512] ;

			try 
			{
				Socket client = InSockEx.ConnectedSocket ;
				SecureSocket secureSock = InSockEx.ConnectedSecureSocket ;

				int LoopCx = 0 ;
				while( true )
				{
					// a simple loop count.  dont let it overflow.
					++LoopCx ;
					if ( LoopCx > 1000 )
					{
						LoopCx = 2 ;
					}

					// receive from the socket.
					int ReadCx ;
					if ( client != null )
						ReadCx = client.Receive(
							buffer, 0, BufferSx, SocketFlags.None ) ;
					else
						ReadCx = secureSock.Receive(
							buffer, 0, BufferSx, SocketFlags.None ) ;

					// did not receive anything.  probably should leave.
					if ( ReadCx == 0 )
					{
						InSockEx.Logger.AddMessage( NetworkRole.Server, "Got zero bytes" ) ;
						break ;
					}

					// There might be more data, 
					// so store the data received so far.
					string block = Encoding.ASCII.GetString( buffer, 0, ReadCx ) ;
					sb.Append( block ) ;

					// is this the end of the message
					if ( Stringer.Tail( sb, 5 ) == "\r\n.\r\n" ) 
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
			}
			return sb ;
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
