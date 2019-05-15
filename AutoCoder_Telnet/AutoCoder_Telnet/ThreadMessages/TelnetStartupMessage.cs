using AutoCoder.Telnet.Common;
using AutoCoder.Telnet.Connect;
using AutoCoder.Telnet.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace AutoCoder.Telnet.ThreadMessages
{
  public class TelnetStartupMessage : ThreadMessageBase
  {
    public ServerConnectPack ServerConnectPack
    { get; set;  }

    public NegotiateSettings NegotiateSettings
    { get; set; }

    public TypeTelnetDevice TypeTelnetDevice
    { get; set; }

    public ConcurrentMessageQueue TelnetQueue
    { get; set; }

    /// <summary>
    /// client window object. use this 
    /// </summary>
    public Window ClientWindow
    { get; set; }

    public Action<bool, TypeTelnetDevice?> TelnetStartupComplete;

    public TelnetStartupMessage( 
      ConcurrentMessageQueue TelnetQueue,
      ServerConnectPack ServerConnectPack, NegotiateSettings NegotiateSettings, 
      Window ClientWindow, Action<bool, TypeTelnetDevice?> TelnetStartupComplete,
      TypeTelnetDevice TypeTelnetDevice )
    {
      this.TelnetQueue = TelnetQueue;
      this.ServerConnectPack = ServerConnectPack;
      this.NegotiateSettings = NegotiateSettings;
      this.ClientWindow = ClientWindow;
      this.TypeTelnetDevice = TypeTelnetDevice;
      this.TelnetStartupComplete = TelnetStartupComplete;
    }

    public override string ToString()
    {
      return "TelnetStartupMessage.";
    }
  }
}
