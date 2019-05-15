using AutoCoder.Ext.System;
using AutoCoder.Telnet.Connect;
using AutoCoder.Telnet.Enums;
using AutoCoder.Telnet.Enums.IBM5250;
using AutoCoder.Telnet.IBM5250;
using AutoCoder.Telnet.IBM5250.Response;
using AutoCoder.Telnet.IBM5250.WorkstationCommands;
using AutoCoder.Telnet.Threads;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace AutoCoder.Telnet.Common
{
#if skip
  /// <summary>
  /// store the telnet connection socket and stream.
  /// </summary>
  public class ConnectedSocketPack : IDisposable
  {
    public TcpClient TcpClient
    { get; set; }

    public NetworkStream TcpStream
    { get; set; }

    public SessionSettings Settings
    { get; set; }

    public xNetworkStreamBackedInputByteArray InputArray
    { get; set; }

    private FromThread FromThread
    { get; set; }

    public ConnectedSocketPack( 
      TcpClient Client, NetworkStream Stream, 
      xNetworkStreamBackedInputByteArray InputArray,
      FromThread FromThread)
    {
      this.TcpClient = Client;
      this.TcpStream = Stream;
      this.InputArray = InputArray;
      this.FromThread = this.InputArray.FromThread;
    }
    public ConnectedSocketPack(xNetworkStreamBackedInputByteArray InputArray)
    {
      this.InputArray = InputArray;
      this.TcpClient = InputArray.Client;
      this.TcpStream = InputArray.NetStream;
      this.FromThread = InputArray.FromThread;
    }

    public void Dispose()
    {
      this.TcpStream?.Close();
      this.TcpClient?.Close();
    }

    public bool IsConnected( )
    {
      if (this.TcpClient?.Connected == true)
        return true;
      else
        return false;
    }
  }
#endif
}
