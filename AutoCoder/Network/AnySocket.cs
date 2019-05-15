using System;
using System.IO;
using System.Net;
using System.Net.Sockets ;

namespace AutoCoder.Network
{
  /// <summary>
  /// Class contains either a secure or standard socket.
  /// Used as means to direct socket I/O to whichever socket is in use.
  /// </summary>
  public class AnySocket
  {
    Socket mSocket = null ;
    Socket mSecureSocket = null;

    public AnySocket( Socket InSock )
    {
      mSocket = InSock ;
    }

    public int Receive( byte[] OutBuffer )
    {
      int Lx = 0 ;
      byte[] buffer = new byte[4096] ;
      if ( mSecureSocket != null )
        Lx = mSecureSocket.Receive( OutBuffer ) ;
      else
        Lx = mSocket.Receive( OutBuffer ) ;
      return Lx ;
    }

    public int Send( byte[] InBytes, SocketFlags InFlag )
    {
      int Lx ;
      if ( mSecureSocket != null )
        Lx = mSecureSocket.Send( InBytes, InBytes.Length, InFlag ) ;
      else
        Lx = mSocket.Send( InBytes, InBytes.Length, InFlag ) ;
      return Lx ;
    }
  }
}
