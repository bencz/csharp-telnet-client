using AutoCoder.Telnet.Common;
using AutoCoder.Telnet.Connect;
using AutoCoder.Telnet.Enums;
using AutoCoder.Telnet.IBM5250.Header;
using AutoCoder.Telnet.ThreadMessages;
using AutoCoder.Threading;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace AutoCoder.Telnet.Threads
{
  public class ConnectThread : ThreadBase, IThreadBase
  {
    private SessionSettings SessionSettings
    { get; set; }

    /// <summary>
    /// queue thru with messages to this thread are sent.
    /// </summary>
    public ConcurrentMessageQueue InputQueue
    {
      get; set;
    }

    private FromThread FromThread
    { get; set; }
    private ToThread ToThread
    { get; set; }

    /// <summary>
    /// the Exception received when the connection failed.
    /// </summary>
    public Exception ConnectionFailedException
    { get; private set; }


    public ConnectThread(
      ExtendedManualResetEvent ShutdownFlag,
      FromThread FromThread, ToThread ToThread,
      SessionSettings Settings)
      : base(ShutdownFlag)
    {
      this.FromThread = FromThread;
      this.ToThread = ToThread;
      this.SessionSettings = Settings;
      this.InputQueue = new ConcurrentMessageQueue();
    }

    public void EntryPoint()
    {
      this.ThreadEndedEvent.Reset();
      try
      {
        // loop receiving from the server until:
        //   - the foreground thread wants to shutdown the connection. It has set
        //     the ShutdownFlag event.
        while ((ShutdownFlag.State == false))
        {
          var message = InputQueue.WaitAndDequeue(this.ShutdownFlag.EventObject);
          if (message != null)
          {
            // startup message sent by MainWindow after the threads have been
            // started. Once startup is completed ( telnet negotiation ) the
            // startup completed event is signaled back on the MainWindow.
            if ( message is TelnetStartupMessage)
            {
              var startupMessage = message as TelnetStartupMessage;
              if ( startupMessage.TypeTelnetDevice == TypeTelnetDevice.Terminal)
              {
                TelnetDisplayStartup(
                  startupMessage.ServerConnectPack, 
                  startupMessage.NegotiateSettings,
                  startupMessage.TelnetQueue,
                  FromThread, ToThread,
                  startupMessage.ClientWindow, 
                  startupMessage.TelnetStartupComplete);
              }

              else if (startupMessage.TypeTelnetDevice == TypeTelnetDevice.Printer)
              {
                TelnetPrinterStartup(
                  startupMessage.ServerConnectPack, 
                  startupMessage.NegotiateSettings,
                  startupMessage.TelnetQueue,
                  FromThread, ToThread,
                  startupMessage.ClientWindow, 
                  startupMessage.TelnetStartupComplete);
              }
            }

            else if (message is GeneralThreadMessage)
            {
              var generalMessage = message as GeneralThreadMessage;
              switch (generalMessage.MessageCode)
              {
              }
            }
          }
        }
      }
      finally
      {
        // in case anyone waiting for this thread to end. Signal the ended event.
        ThreadEndedEvent.Set();
      }
    }

    private void TelnetDisplayStartup(
      ServerConnectPack ConnectPack, NegotiateSettings NegotiateSettings,
      ConcurrentMessageQueue TelnetQueue,
      FromThread FromThread, ToThread ToThread,
      Window ClientWindow, Action<bool, TypeTelnetDevice?> TelnetStartupComplete)
    {

      // initial telnet back and forth negotiation with server.
      var rv = TelnetConnection.TelnetConnectAndNegotiate(
        ConnectPack.HostName, NegotiateSettings, 
        TelnetQueue, ToThread);
      var sessionSettings = rv.Item1;
      ConnectPack.Settings = sessionSettings;

      // read the dataStreamHeader from the Telnet thread.
      // this message contains the startup up code ( I902 ), system name and
      // printer name.  The next message from the server will be sent when there
      // is a spooled file ready to print.
      var attrMsg = TelnetQueue.WaitAndDequeue() as TelnetDeviceAttrMessage;
      if (attrMsg != null)
      {
        // signal startup is complete to the telnet window on the UI thread.
        var bi = ClientWindow.Dispatcher.BeginInvoke(
          DispatcherPriority.Input, new ThreadStart(
            () =>
            {
              TelnetStartupComplete(true, attrMsg.TypeDevice);
            }));
      }
    }

    private void TelnetPrinterStartup(
      ServerConnectPack ConnectPack, NegotiateSettings NegotiateSettings,
      ConcurrentMessageQueue TelnetQueue,
      FromThread FromThread, ToThread ToThread,
      Window ClientWindow, Action<bool, TypeTelnetDevice?> TelnetStartupComplete)
    {
      // initial telnet back and forth negotiation with server.
      // method returns when a non telnet command is received from the input 
      // queue.
      var rv = TelnetConnection.TelnetConnectAndNegotiate(
        ConnectPack.HostName, NegotiateSettings,
        TelnetQueue, ToThread);
      var sessionSettings = rv.Item1;
      ConnectPack.Settings = sessionSettings;

      // read the dataStreamHeader from the Telnet thread.
      // this message contains the startup up code ( I902 ), system name and
      // printer name.  The next message from the server will be sent when there
      // is a spooled file ready to print.
      var attrMsg = TelnetQueue.WaitAndDequeue() as TelnetDeviceAttrMessage;
      if ( attrMsg != null)
      {
        // signal startup is complete to the telnet window on the UI thread.
        var bi = ClientWindow.Dispatcher.BeginInvoke(
          DispatcherPriority.Input, new ThreadStart(
            () =>
            {
              TelnetStartupComplete(true, attrMsg.TypeDevice);
            }));
      }

#if skip
      var dsh = TelnetQueue.WaitAndDequeue() as DataStreamHeader;
      if (( dsh != null ) && ( dsh is PrinterStartupDataStreamHeader))
      {
        var startupHeader = dsh as PrinterStartupDataStreamHeader;
      }
#endif
    }
  }
}
