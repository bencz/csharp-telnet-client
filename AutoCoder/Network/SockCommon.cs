using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;

namespace AutoCoder.Network
{
  public static class SockCommon
  {

    /// <summary>
    /// return a Socket that is connected to an ip port.
    /// </summary>
    /// <param name="Addr"></param>
    /// <param name="PortNx"></param>
    /// <returns></returns>
    public static Socket GetConnectSocket(IPAddress Addr, int PortNx)
    {
      Socket s = null;
      Socket tempSocket = null;
      IPEndPoint ipe = null;

      ipe = new IPEndPoint(Addr, PortNx);
      tempSocket =
          new Socket(ipe.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

      tempSocket.Connect(ipe);

      if (tempSocket.Connected)
      {
        s = tempSocket;
      }
      return s;
    }

    public static IPAddress GetLocalIpAddress( string SubNetStub = null)
    {
      IPAddress localAddr = null;
      string hostName = Dns.GetHostName();
      IPHostEntry he = Dns.GetHostEntry(hostName);

      IPAddress[] addrs = he.AddressList;
      if (addrs.Length == 0)
        throw new ApplicationException(
          "local system " + hostName + " does not have an IP address");

      // get the ip4 address from the address list 
      foreach (IPAddress addr in addrs)
      {
        if (addr.AddressFamily == AddressFamily.InterNetwork)
        {
          if ( SubNetStub != null )
          {
            if (addr.ToString().IndexOf(SubNetStub) != -1)
            {
              localAddr = addr;
              break;
            }
          }
          else
          {
            localAddr = addr;
            break;
          }
        }
      }

      return localAddr;
    }
  }
}
