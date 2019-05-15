using System;
using System.Collections ;

namespace AutoCoder.Network
{
	/// <summary>
	/// A ServerTransaction is a sequence of ServerExchanges ( SocketExchange ) ????
	/// </summary>
	public class ServerTransaction
	{
		AnySocket mSocket ;
//		ArrayList mExchanges = null ;

		public ServerTransaction( AnySocket InSock )
		{
			mSocket = InSock ;
		}

	}

}
