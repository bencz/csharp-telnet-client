using System ;
using System.Net.Sockets ;

namespace AutoCoder.Network
{

	// ------------------------- AcTcpClient ------------------------------
	// derived from TcpClient.  Exposes the contained socket object.
	public class AcTcpClient : TcpClient
	{

		// --------------------- constructor --------------------------------
		public AcTcpClient( string InHost, int InPortNx )
			: base( InHost, InPortNx )
		{
		}

		// -------------------- get the underlying socket --------------------
		public Socket ClientSocket
		{
			get { return Client ; }
		}
	}
}
