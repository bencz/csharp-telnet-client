using System;
using System.Collections;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace AutoCoder.Network.Mail.InMail
{

	// ----------------------- SockThreadState ----------------------
	// class used to receive MailMessage in a background thread.
	public class SockThreadState
	{
		// Client socket.
		SocketExchange mSockEx = null ;
		
		// Size of receive buffer.
		public const int BufferSize = 512 ;
		
		// Receive buffer.
		public byte[] buffer = new byte[BufferSize];
		
		// Received data string.
		public StringBuilder sb = new StringBuilder( ) ;

		// -------------------- properties ------------------------
		public SocketExchange SockEx
		{
			get { return mSockEx ; }
			set { mSockEx = value ; }
		}
	}
}
