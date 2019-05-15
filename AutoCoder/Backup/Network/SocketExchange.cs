using System;
using System.IO;
using System.Net;
using System.Net.Sockets ;
using System.Text;
using System.Threading ;
using AutoCoder.Text ;
using Org.Mentalis.Security.Ssl;
using Org.Mentalis.Security.Certificates;

namespace AutoCoder.Network
{
	// possible better name:  ServerExchange ( maybe ServerExchange is a derived class
  //                        of SocketExchange )
	// ServerTransaction - describes a series of ServerExchanges
	// ServerExchangeInfo - basic info, then a logger holding the back and forth history
	//                      of the exchange 
  // ServerTransactionLog or Info - basic info of the transaction, then a list of the
  //      child ServerExchangeLog and ServerTransactionLog 
 
	// ---------------------------- SocketExchange -------------------------
	// a send/response network transaction
	public class SocketExchange
	{
		Socket mSocket = null ;
		SecureSocket mSecureSocket = null ;
		string mSecureSocketType ;
		AnySocket mAnySocket = null ;

		string mServerName ;
		int mServerPortNx ;
		Logger mLogger = null ;
		string mSendMessage ;
		StringBuilder mResponseBuilder ;
		ExpectedResponseCodes mExpectedResponseCodes ;
		string mExpectedResponseCode ;  // see ExpectedResponseCode property.
		int mReadMoreDelay ;		// delay time while wait for more to receive.

		// ------------------------ constructors --------------------------
		public SocketExchange(
			Socket InSocket, string InServerName, int InServerPortNx, Logger InLogger )
		{
			mSocket = InSocket ;
			mServerName = InServerName ;
			mServerPortNx = InServerPortNx ;
			mLogger = InLogger ;
			mReadMoreDelay = 0 ;
			ResponseMessage = null ;
		}
		public SocketExchange( string InServerName, int InServerPortNx, Logger InLogger )
		{
			mSocket = null ;
			mServerName = InServerName ;
			mServerPortNx = InServerPortNx ;
			mLogger = InLogger ;
			mReadMoreDelay = 0 ;
			ResponseMessage = null ;
		}

		// --------------------------- properties ------------------------------
		public AnySocket AnySocket
		{
			get { return mAnySocket ; }
			set { mAnySocket = value ; }
		}
		public Socket ConnectedSocket
		{
			get { return mSocket ; }
			set
			{
				mSocket = value ;
				mAnySocket = new AnySocket( mSocket ) ;
			}
		}
		public SecureSocket ConnectedSecureSocket
		{
			get { return mSecureSocket ; }
			set
			{
				mSecureSocket = value ;
				mAnySocket = new AnySocket( mSecureSocket ) ;
			}
		}

		// collection of possible response codes expected from the remote.
		public ExpectedResponseCodes ExpectedResponseCodes
		{
			get { return mExpectedResponseCodes ; }
			set { mExpectedResponseCodes = value ; }
		}

		// the response code pulled from the ResponseMessage that matches one of the
		// codes in the ExpectedResponseCodes collection.
		public string ExpectedResponseCode
		{
			get
			{
				AssureExpectedResponseCode( ) ;
				return mExpectedResponseCode ;
			}
		}

		public Logger Logger
		{
			get { return mLogger ; }
			set { mLogger = value ; }
		}

		public int ReadMoreDelay
		{
			get { return mReadMoreDelay ; }
			set { mReadMoreDelay = value ; }
		}

		public string ResponseCode
		{
			get { return CalcResponseCode( ) ; }
		}

		public StringBuilder ResponseBuilder
		{
			get { return mResponseBuilder ; }
		}

		/// <summary>
		/// the complete, accumulated response string.
		/// </summary>
		public string ResponseMessage
		{
			get
			{
				if ( mResponseBuilder == null )
					return "" ;
				else
					return mResponseBuilder.ToString( ) ;
			}
			set
			{
				mExpectedResponseCode = null ;
				mResponseBuilder = null ;
				if ( value != null )
				{
					mResponseBuilder = new StringBuilder( ) ;
					mResponseBuilder.Append( value ) ;
				}
			}
		}

		private string ServerPortCombo
		{
			get { return mServerName + ":" + mServerPortNx ; }
		}
		public string ServerName
		{
			get { return mServerName ; }
			set { mServerName = value ; }
		}
		public int ServerPortNx
		{
			get { return mServerPortNx ; }
			set { mServerPortNx = value ; }
		}
		public int SocketAvailable
		{
			get	{ return GetSocketAvailable( ) ; }
		}

		// --------------------------- AddMessage ---------------------------------
		private void AddMessage( NetworkRole InRole, string InFullMsg )
		{
			if ( mLogger != null )
				mLogger.AddMessage( InRole, InFullMsg ) ;
		}

		// --------------------- AssureExpectedResponseCode -------------------
		private void AssureExpectedResponseCode( )
		{
			if ( mExpectedResponseCode == null )
			{
				mExpectedResponseCode = 
					ExpectedResponseCodes.FindResponseCode( ResponseMessage ) ;
			}
		}

		private void AssureResponseBuilder( )
		{
			if ( mResponseBuilder == null )
				mResponseBuilder = new StringBuilder( 2000 ) ;
		}

		// --------------------------- CalcResponseCode ----------------------------
		private string CalcResponseCode( )
		{
			string responseCode = ExpectedResponseCode ;
			if ( responseCode == null )
			{
				int Ix = ResponseMessage.IndexOf( " " ) ;
				if ( Ix == -1 )
					responseCode = ResponseMessage ;
				else
					responseCode = ResponseMessage.Substring( 0, Ix ) ;
			}
			return responseCode ;
		}

		// --------------------------- CloseConnection --------------------------------
		public void CloseConnection( )
		{
			if ( mSecureSocket != null )
			{
				CloseSecureConnection( ) ;
				return ;
			}

			if ( mSocket == null )
				ThrowNetworkException( "CloseConnection error. Not connected." ) ;
			try
			{
				mSocket.Close( ) ;
				mSocket = null ;
			}
			catch( Exception excp )
			{
				ThrowNetworkException( "CloseConnection failed.", excp ) ;
			}
			AddMessage( NetworkRole.Client, "Connection closed." ) ;
		}

		// ------------------------ CloseSecureConnection --------------------------------
		public void CloseSecureConnection( )
		{
			if ( mSecureSocket == null )
				ThrowNetworkException( "CloseSecureConnection error. Not connected." ) ;
			try
			{
				mSecureSocket.Shutdown( SocketShutdown.Both ) ;
				mSecureSocket.Close( ) ;
				mSecureSocket = null ;
				AnySocket = null ;
			}
			catch( Exception excp )
			{
				ThrowNetworkException( "CloseSecureConnection failed.", excp ) ;
			}
			AddMessage( NetworkRole.Client, "SecureConnection closed." ) ;
		}

		// ----------------------------- Connect ---------------------------------
		// connect to the remote socket.
		public void Connect( )
		{
			mSocket = null ;

			// make sure server name and port have been spcfd.
			if ( Stringer.IsEmpty( mServerName ) == true )
				ThrowNetworkException( "Connect error. Server name is empty." ) ;
			if ( mServerPortNx == 0 )
				ThrowNetworkException( "Connect error. Server port number is zero." ) ;

			// resolve and connect to the remote system.
			try
			{
				// resolve to the host server.
				IPHostEntry hostEntry = null;
				hostEntry = Dns.Resolve( mServerName ) ;

				// I guess Resolve returns a list of IP addresses. ( something to do with
				// IPv6 ). This loop is standard stuff pulled from the MS documenation.
				// Try to connect to each address until successful.
				foreach( IPAddress address in hostEntry.AddressList )
				{
					IPEndPoint ep = new IPEndPoint( address, mServerPortNx ) ;
					Socket tempSocket = new Socket(
						ep.AddressFamily, 
						SocketType.Stream,
						ProtocolType.Tcp ) ;

					tempSocket.Connect( ep ) ;

					if ( tempSocket.Connected == true )
					{
						ConnectedSocket = tempSocket ;
						break;
					}

				}
			}
			catch( Exception e )
			{
				ThrowNetworkException(
					"Exception connecting to " + ServerPortCombo, e ) ;
			}

			// throw exception if can't connect ...
			if ( mSocket == null )
				ThrowNetworkException( "Failed to connect to " + ServerPortCombo ) ;
			else
				AddMessage( NetworkRole.Client, "Connected to " + ServerPortCombo ) ;
		}

		// ---------------------- CreateSecureSocket ---------------------------
		private SecureSocket CreateSecureSocket( )
		{
			SecureSocket socket ;

			// Connection type.
			//   0 = normal connection, 1 = direct TLS connection,
			//   2 = indirect TLS connection (using STARTTLS command)]
			mSecureSocketType = "1" ;

			// create a new SecurityOptions instance
			SecurityOptions options = new SecurityOptions(SecureProtocol.None) ;
			options.AllowedAlgorithms = SslAlgorithms.SECURE_CIPHERS ;
			options.Entity = ConnectionEnd.Client ;
			options.VerificationType = CredentialVerification.Manual ;
			options.Verifier = new CertVerifyEventHandler( OnVerify ) ;
			options.Flags = SecurityFlags.Default ;
			options.CommonName = ServerName ;
			if ( mSecureSocketType == "1" ) 
			{
				options.Protocol = SecureProtocol.Tls1 ;
			}
			
			// create the new secure socket
			socket =	new SecureSocket(
				AddressFamily.InterNetwork,
				SocketType.Stream,
				ProtocolType.Tcp,
				options ) ;

			return socket ;
		}

		// ------------------------ DelayThenReadMore ------------------------
		private void DelayThenReadMorex( )
		{
			if ( mSecureSocket != null )
			{
				if ( mSecureSocket.Available == 0 )
				{
					AddMessage( NetworkRole.Client, "Delay " + mReadMoreDelay + " "
						+	DateTime.Now.ToString( )) ;
					Thread.Sleep( mReadMoreDelay ) ;
					AddMessage( NetworkRole.Client, "Delay done. Available: "
						+ mSecureSocket.Available + " " + DateTime.Now.ToString( )) ;
				}
				if ( mSecureSocket.Available > 0 )
				{
					AssureResponseBuilder( ) ;
//					SocketReadMore( ) ;
				}
			}

			else
			{
				if ( mSocket.Available == 0 )
				{
					AddMessage( NetworkRole.Client, "Delay " + mReadMoreDelay + " " + DateTime.Now.ToString( )) ;
					Thread.Sleep( mReadMoreDelay ) ;
					AddMessage( NetworkRole.Client, "Delay done. Available: " + mSocket.Available + " " + DateTime.Now.ToString( )) ;
				}
				if ( mSocket.Available > 0 )
				{
//					AssureResponseBuilder( ) ;
//					SocketReadMore( ) ;
				}
			}
		}

		// ------------------------ DelayThenReadMore ------------------------
		private void DelayThenReadMorex( StringBuilder InOutAccum )
		{
			if ( mSecureSocket != null )
			{
				if ( mSecureSocket.Available == 0 )
				{
					AddMessage( NetworkRole.Client, "Delay " + mReadMoreDelay + " "
						+	DateTime.Now.ToString( )) ;
					Thread.Sleep( mReadMoreDelay ) ;
					AddMessage( NetworkRole.Client, "Delay done. Available: "
						+ mSecureSocket.Available + " " + DateTime.Now.ToString( )) ;
				}
//				if ( mSecureSocket.Available > 0 )
//					SocketReadMore( InOutAccum ) ;
			}

			else
			{
				if ( mSocket.Available == 0 )
				{
					AddMessage( NetworkRole.Client, "Delay " + mReadMoreDelay + " " + DateTime.Now.ToString( )) ;
					Thread.Sleep( mReadMoreDelay ) ;
					AddMessage( NetworkRole.Client, "Delay done. Available: " + mSocket.Available + " " + DateTime.Now.ToString( )) ;
				}
//				if ( mSocket.Available > 0 )
//					SocketReadMore( InOutAccum ) ;
			}
		}

		// ----------------------- GetResponseData ----------------------------
		public string GetResponseData( )
		{
			int Bx = ResponseCode.Length ;
			
			// isolate and return the data after the response code.
			int Lx = ResponseMessage.Length - Bx ;
			if ( Lx > 0 )
				return( ResponseMessage.Substring( Bx, Lx ).Trim( )) ;
			else
				return "" ;
		}

		// ----------------------- GetSocketAvailable --------------------
		private int GetSocketAvailable( )
		{
			if ( mSecureSocket != null )
				return mSecureSocket.Available ;
			else
				return mSocket.Available ;
		}

		// ----------------------- LoadResponseMessage ------------------------
		// load the response string ( instead of calling SocketRead to fill it )
		public void LoadResponseMessage( string InResponse )
		{
			ResponseMessage = InResponse ;
			AddMessage( NetworkRole.Server, ResponseMessage ) ;
		}

		// ------------------------ OnVerify -------------------------------------
		protected void OnVerify(
			SecureSocket socket,
			Certificate remote,
			CertificateChain InChain,
			VerifyEventArgs e ) 
		{
			Certificate[] certs = InChain.GetCertificates( ) ;
			for( int Ix = 0 ; Ix < certs.Length ; Ix++ ) 
			{
				AddMessage( NetworkRole.Client, certs[Ix].ToString( true )) ;
			}

			// print out the result of the chain verification
			AddMessage(
				NetworkRole.Client,
				"Verify certificate: " +
				InChain.VerifyChain( socket.CommonName, AuthType.Server ).ToString( )) ;
		}

		// ----------------------------- Receive ------------------------------
		// receive message from remote without first sending.
		public void Receive( )
		{
			ResponseMessage = null ;
			mSendMessage = null ;
			ResponseMessage = SocketRead( ) ;

			// server may be slow getting the complete message to this client. Delay the
			// spcfd nbr of milliseconds, then if "DataAvailable", read the available data
			// and append to what was already read.
			if ( ReadMoreDelay > 0 )
			{
				ReceiveMore( ReadMoreDelay ) ;
			}
		}

		/// <summary>
		/// Receive more and append to ResponseMessage
		/// </summary>
		/// <returns></returns>
		public bool ReceiveMore( )
		{
			bool gotMore = false ;
			string moreMessage = SocketRead( ) ;
			if ( moreMessage != null )
			{
				AssureResponseBuilder( ) ;
				mResponseBuilder.Append( moreMessage ) ;
				gotMore = true ;
			}
			return gotMore ;
		}
		
		public bool ReceiveMore( int InSleepMilliseconds )
		{
			bool gotMore = false ;
			if ( GetSocketAvailable( ) == 0 )
				Thread.Sleep( InSleepMilliseconds ) ;
			if ( GetSocketAvailable( ) == 0 )
				gotMore = false ;
			else
				gotMore = ReceiveMore( ) ;
			return gotMore ;
		}

		// ---------------------- ResponseMessageContains ---------------------------
		public bool ResponseMessageContains( string InValue )
		{
			if ( ResponseMessage.IndexOf( InValue ) == -1 )
				return false ;
			else
				return true ;
		}

		// ---------------------- ResponseMessageEndsWithCrLf -----------------------
		public bool ResponseMessageEndsWithCrLf( )
		{
			return( Stringer.EndsWith( ResponseMessage, NetworkConstants.CrLf )) ;
		}

		// ----------------------------- SecureConnect -----------------------------
		// secure SSL connect to the remote socket.
		public void SecureConnect( )
		{
			mSecureSocket = null ;

			// make sure server name and port have been spcfd.
			if ( Stringer.IsEmpty( mServerName ) == true )
				ThrowNetworkException( "Connect error. Server name is empty." ) ;
			if ( mServerPortNx == 0 )
				ThrowNetworkException( "Connect error. Server port number is zero." ) ;

			// resolve and connect to the remote system.
			try
			{
				// resolve to the host server.
				IPHostEntry hostEntry = null;
				hostEntry = Dns.Resolve( mServerName ) ;

				// I guess Resolve returns a list of IP addresses. ( something to do with
				// IPv6 ). This loop is standard stuff pulled from the MS documenation.
				// Try to connect to each address until successful.
				foreach( IPAddress address in hostEntry.AddressList )
				{
					IPEndPoint ep = new IPEndPoint( address, mServerPortNx ) ;

					SecureSocket tempSocket = CreateSecureSocket( ) ;

//					Socket tempSocket = new Socket(
//						ep.AddressFamily, 
//						SocketType.Stream,
//						ProtocolType.Tcp ) ;
		
					// connect to the SMTP server
					tempSocket.Connect( ep ) ;

					if ( tempSocket.Connected == true )
					{
						ConnectedSecureSocket = tempSocket;
						break;
					}

				}
			}
			catch( Exception e )
			{
				ThrowNetworkException(
					"Exception connecting to " + ServerPortCombo, e ) ;
			}

			// throw exception if can't connect ...
			if ( mSecureSocket == null )
				ThrowNetworkException( "Failed to connect to " + ServerPortCombo ) ;
			else
				AddMessage( NetworkRole.Client, "Connected to " + ServerPortCombo ) ;
		}

		// ----------------------------- Send ------------------------------
		// send message to remote, log the results. ( dont read the response. )
		public void Send( string InSendMessage )
		{
			ResponseMessage = null ;
			mSendMessage = InSendMessage ;
			SocketWrite( ) ;
		}

		// ----------------------------- SendReceive ------------------------------
		// send message to remote, read back the response.
		// Use TestReponse to process the response message.
		public void SendReceive( string InSendMessage )
		{
			ResponseMessage = null ;
			mSendMessage = InSendMessage ;

			SocketWrite( ) ;
			ResponseMessage = SocketRead( ) ;

			// server may be slow getting the complete message to this client. Delay the
			// spcfd nbr of milliseconds, then if "DataAvailable", read the available data
			// and append to what was already read.
			if ( ReadMoreDelay > 0 )
			{
				ReceiveMore( ReadMoreDelay ) ;
			}
		}

		// -------------------- SleepThenReadMoreAvailableData ------------------------
		// caller suspects there is more data on the way to read, but dont want to block
		// and timeout waiting for it. 
		// Sleep for specified time, then, if DataAvailable, receive the data.
		public bool SleepThenReadMoreAvailableData( int InSleepMilliseconds )
		{
			bool gotData = false ;
			if ( GetSocketAvailable( ) == 0 )
			{
				AddMessage(
					NetworkRole.Client,
					"SleepThenReadMoreAvailableData( " + InSleepMilliseconds + " )" ) ;
				Thread.Sleep( InSleepMilliseconds ) ;
			}
			if ( GetSocketAvailable( ) > 0 )
			{
				string seg = SocketRead( ) ;
				AssureResponseBuilder( ) ;
				mResponseBuilder.Append( seg ) ;
				gotData = true ;
			}
			else
			{
				AddMessage( NetworkRole.Client, "Sleep expire. Still no available data." ) ;
				gotData = false ;
			}
			return gotData ;
		}

		// --------------------------- SocketRead -------------------------------
		// read the bytes from the socket, return its contents in string form.
		private string SocketRead(  )
		{
			string responseSegment = null ;
			try
			{
				int Lx = 0 ;
				byte[] buffer = new byte[4096] ;
				Lx = AnySocket.Receive( buffer ) ;
				responseSegment = Encoding.ASCII.GetString( buffer, 0, Lx ) ;
				AddMessage( NetworkRole.Server, responseSegment ) ;
			}
			catch(System.Exception excp )
			{
				ThrowNetworkException( "Exception receiving from " + ServerPortCombo, excp ) ; 
			}

			return responseSegment ;
		}

		// --------------------------- SocketReadMore -------------------------------
		// read the bytes from the socket, return its contents in string form.
		public void SocketReadMorex( StringBuilder InOutAccum  )
		{
			if ( InOutAccum.Length == 0 )
				InOutAccum.Append( ResponseMessage ) ;
			string responseSegment = SocketRead( ) ;
			InOutAccum.Append( responseSegment ) ;
		}

		public void SocketReadMorex( )
		{
			string responseSegment = SocketRead( ) ;
			mResponseBuilder.Append( responseSegment ) ;
		}

		// --------------------------- SocketWrite ---------------------------
		private void SocketWrite( ) 
		{
			try
			{
				byte[] arrToSend = Encoding.ASCII.GetBytes( mSendMessage ) ;
				AnySocket.Send( arrToSend, SocketFlags.None ) ;
				AddMessage( NetworkRole.Client, mSendMessage  ) ;
			}
			catch( System.Exception excp)
			{
				ThrowNetworkException( "Exception writing to " + ServerPortCombo, excp ) ; 
			}
		}

		// --------------------------- TestResponseCode ----------------------------
		public bool TestResponseCodex( string InCode )
		{
			int Lx ;
			if ( ResponseMessage == null )
				ThrowNetworkException(
					"TestResponseCode method failed. No response code to test" ) ;
			Lx = InCode.Length ;
			if ( ResponseMessage.StartsWith( InCode ) == true )
			{
				return true ;
			}
			else
				return false ;
		}

		// ------------------------- ThrowIfUnexpectedResponse ------------------------
		public void ThrowIfUnexpectedResponse( )
		{
			if ( ResponseMessage == null )
				ThrowNetworkException( "No response message to search" ) ;
			if ( ExpectedResponseCode == null )
				ThrowUnexpectedResponse( ) ;
		}

		// ----------------------- ThrowUnexpectedResponse --------------------------
		public void ThrowUnexpectedResponse( )
		{
			string msg =
				"Unexpected response. Expected: " + 
				ExpectedResponseCodes.ToString( ) +
				" Received: " +
				ResponseCode ;
			ThrowNetworkException( msg ) ;
		}

		// ----------------------- ThrowUnexpectedResponse --------------------------
		public void ThrowUnexpectedResponse( string InExpected )
		{
			string msg =
				"Unexpected response. Expected: " + 
				InExpected +
				" Received: " +
				ResponseMessage ;
			ThrowNetworkException( msg ) ;
		}

		// ------------------------ ThrowNetworkException ------------------------
		public void ThrowNetworkException( string InMessage )
		{
			AddMessage( NetworkRole.Exception, InMessage ) ;
			throw( new NetworkException( InMessage )) ;
		}

		// ------------------------ ThrowNetworkException ------------------------
		public void ThrowNetworkException( string InMessage, System.Exception InCaughtExcp )
		{
			AddMessage( NetworkRole.CaughtException, InCaughtExcp.Message ) ;
			AddMessage( NetworkRole.Exception, InMessage ) ;
			throw( new NetworkException( InMessage, InCaughtExcp )) ;
		}

	} // end class SocketExchange 
}
