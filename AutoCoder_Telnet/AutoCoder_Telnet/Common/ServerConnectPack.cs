using AutoCoder.Telnet.Connect;
using AutoCoder.Telnet.Threads;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace AutoCoder.Telnet.Common
{
  public class ServerConnectPack :IDisposable
  {
    public TcpClient TcpClient
    { get; set; }

   public SessionSettings Settings
    { get; set; }

    public FromThread FromThread
    { get; set; }

    public string HostName
    { get; set; }

#if skip
    /// <summary>
    /// property defined for convenience. more direct access to the comm blocks 
    /// read from the server.
    /// </summary>
    public ConcurrentMessageQueue ReadBlocks
    {
      get { return FromThread.TelnetQueue; }
    }
#endif

    public ServerConnectPack(FromThread FromThread, TcpClient Client, string HostName)
    {
      this.TcpClient = Client;
      this.FromThread = FromThread;
      this.HostName = HostName;
    }
    public void Dispose()
    {
      this.TcpClient?.Close();
    }

    public bool IsConnected()
    {
      if (this.TcpClient?.Connected == true)
        return true;
      else
        return false;
    }

  }
}
