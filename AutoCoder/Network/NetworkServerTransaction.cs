using System;
using System.Collections ;

namespace AutoCoder.Network
{
	/// <summary>
	/// A ServerTransaction is a sequence of ServerExchanges ( SocketExchange ) ????
	/// </summary>
	public class NetworkServerTransaction
	{
		AnySocket mSocket ;
//		ArrayList mExchanges = null ;

		public NetworkServerTransaction( AnySocket InSock )
		{
			mSocket = InSock ;
		}

	}

}
