using AutoCoder.Ext.System;
using AutoCoder.Systm;
using AutoCoder.Telnet.Common;
using AutoCoder.Telnet.Common.RowCol;
using AutoCoder.Telnet.Common.ScreenDm;
using AutoCoder.Telnet.Connect;
using AutoCoder.Telnet.Enums;
using AutoCoder.Telnet.IBM5250;
using AutoCoder.Telnet.IBM5250.Content;
using AutoCoder.Telnet.IBM5250.WorkstationCommands;
using AutoCoder.Telnet.IBM5250.WtdOrders;
using AutoCoder.Telnet.LogFiles;
using AutoCoder.Telnet.TelnetCommands;
using AutoCoder.Telnet.ThreadMessages;
using AutoCoder.Telnet.Threads;
using AutoCoder.Threading;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AutoCoder.Telnet.Threads
{
  /// <summary>
  /// thread continuously reads from telnet server. Data is provided in complete
  /// blocks that end with EOR or telnet handshake end of data marker.
  /// 
  /// Thread does not establish the connection to the server. Pass the connected
  /// TcpClient to the class thru its constructor.
  /// 
  /// Pass the ShutdownFlag manual reset event in the constructor. Thread uses 
  /// ShutdownFlag as one of the handles to wait on when waiting for data to arrive
  /// from the server. Once the ShutdownFlag event is signalled this thread will 
  /// end.
  /// </summary>
  public class FromThread : ThreadBase, IThreadBase
  {
    public TcpClient Client
    { get; set; }

    private SessionSettings SessionSettings
    { get; set; }

    /// <summary>
    /// queue thru with messages to this thread are sent.
    /// </summary>
    public ConcurrentMessageQueue InputQueue
    {
      get; set;
    }

    /// <summary>
    /// send telnet commands to the telnetQueue. The ConnectThread will receive
    /// these messages.
    /// </summary>
    private ConcurrentMessageQueue TelnetQueue
    { get; set; }

    /// <summary>
    /// event signaled when the connection to the telnet server has been lost.
    /// </summary>
    public ExtendedManualResetEvent ConnectionFailedEvent
    { get; private set; }

    /// <summary>
    /// the Exception received when the connection failed.
    /// </summary>
    public Exception ConnectionFailedException
    { get; private set; }

    public IThreadBase MasterThread
    { get; set; }
    public IThreadBase PrinterThread
    { get; set; }

    public bool ConnectionComplete
    { get; set; }

    public TypeTelnetDevice? TypeDevice
    { get; set; }

    public FromThread(
      ExtendedManualResetEvent ShutdownFlag, TcpClient Client, 
      ConcurrentMessageQueue TelnetQueue,
      SessionSettings Settings, ScreenDim ScreenDim)
      : base(ShutdownFlag)
    {
      this.Client = Client;
      this.InputQueue = new ConcurrentMessageQueue();
      this.TelnetQueue = TelnetQueue;
      this.ConnectionFailedEvent = new ExtendedManualResetEvent(false);
      this.SessionSettings = Settings;
      this.LogList = new TelnetLogList("From");
    }

    public void EntryPoint()
    {
      this.ThreadEndedEvent.Reset();

      try
      {
        byte[] buf = new byte[99999];
        this.ConnectionFailedException = null;
        var gotRead = new ManualResetEvent(false);
        IAsyncResult br = null;
        int bytesRead = 0;
        WaitHandle[] handles = new WaitHandle[3];

        // loop receiving from the server until:
        //   - the foreground thread wants to shutdown the connection. It has set
        //     the ShutdownFlag event.
        //   - the connection to the server has failed. 
        while ((ShutdownFlag.State == false) 
          && ( ConnectionFailedException == null))
        {
          AsyncCallback callBack = null;
          callBack = ar =>
          {
            try
            {
              bytesRead = this.Client.GetStream().EndRead(ar);
              gotRead.Set();  // set the gotRead event. one of the events the
                              // WaitAny is waiting on.
            }
            catch (ObjectDisposedException)
            {
              bytesRead = 0;
            }
            catch (InvalidOperationException)
            {
              bytesRead = 0;
            }
          };

          // dequeue and process any message in the InputQueue.
          if (this.InputQueue.Count > 0)
          {
            ReadAndProcessInputQueue();
            continue;
          }

          try
          {
            // start a socket read. ( do not start a new read on every iteration
            // of this loop. Something may have arrived on the InputQueue. 
            // Meaning the read of the prior iteration is active and is waiting 
            // to complete. )
            if (br == null)
            {
              br = this.Client.GetStream().BeginRead(
                buf, 0, buf.Length, callBack, null);
            }

            // wait for socket read to complete ( gotRead event is set. ) Or for
            // something to arrive in the InputQueue.
            handles[0] = this.ShutdownFlag.EventObject;
            handles[1] = gotRead;
            handles[2] = this.InputQueue.GotMessageEvent;
            var ix = WaitHandle.WaitAny(handles);

            if (ix == 1)
            {
              br = null;
              gotRead.Reset();
              if (bytesRead > 0)
              {
                byte[] recvBytes = LoadReceivedBytes(buf, bytesRead);

                TrafficLogFile.Write(Direction.Read, recvBytes);

                // print to printer data stream has started. route direct to 
                // printer thread.
                if ((this.TypeDevice != null ) 
                  && ( this.TypeDevice.Value == TypeTelnetDevice.Printer))
                {
                  var printerMessage = new PrinterDataBytesMessage(recvBytes);
                  PostToProcessQueue(printerMessage);
                }

                else
                {
                  ProcessAndPostBytesReceived(recvBytes);
                }
              }
            }
          }
          catch (Exception excp)
          {
            this.ConnectionFailedException = excp;
            this.ConnectionFailedEvent.Set();
          }
        }
      }
      finally
      {
        // in case anyone waiting for this thread to end. Signal the ended event.
        ThreadEndedEvent.Set();
      }
    }

    /// <summary>
    /// copy bytes received from the socket to a standalone byte array. While 
    /// doing the copy replace 0xFF pairs with a single 0xFF. ( the telnet server
    /// doubles up the 0xFF in order to not confuse an 0xFF byte in the data 
    /// stream as an IAC ( 0xFF )) 
    /// way 
    /// </summary>
    /// <param name="buf"></param>
    /// <param name="bytesRead"></param>
    /// <returns></returns>
    private byte[] LoadReceivedBytes(byte[] buf, int bytesRead)
    {
      var recvBytes = buf.SubArray(0, bytesRead);

      // look for FF FF in the byte stream.
      var fx = recvBytes.IndexOf(0, doubleFF);
      if ( fx >= 0 )
      {
        recvBytes = recvBytes.ReplaceAll(doubleFF, singleFF) ;
      }

      return recvBytes;
    }
    static readonly byte[] doubleFF = { 0xff, 0xff };
    static readonly byte[] singleFF = { 0xff };

    private void ReadAndProcessInputQueue()
    {
      object message = null;
      bool rc = this.InputQueue.TryDequeue(out message);
      if ((rc == true) && (message is GeneralThreadMessage))
      {
        var generalMessage = message as GeneralThreadMessage;
        switch (generalMessage.MessageCode)
        {
          case ThreadMessageCode.ClearLog:
            {
              this.LogList.Clear();
              break;
            }
        }
      }

      else if (message is TelnetDeviceAttrMessage)
      {
        var attrMessage = message as TelnetDeviceAttrMessage;
        this.TypeDevice = attrMessage.TypeDevice;
        this.ConnectionComplete = true;
      }
    }

    private void ProcessAndPostBytesReceived(byte[] recvBytes)
    {
      // load bytes received from the server into InputArray.
      var inputArray = new InputByteArray(recvBytes);

      WorkstationCommandList wipCmdList = null;
      bool badDataLogged = false;
      while (inputArray.IsEof() == false)
      {
        var rv = ParseAndPostInputArray(inputArray, wipCmdList);
        wipCmdList = rv.Item1;
        var gotSomething = rv.Item2;

        // nothing parsed. And there is data remaining. This is an error.
        if ((gotSomething == false) && (inputArray.IsEof() == false))
        {
          if (badDataLogged == false)
          {
            LogBadData(inputArray);
            badDataLogged = true;
          }
          inputArray.GetByte();   // advance by 1 byte in byte stream.
        }
      }
    }
    private void LogBadData(InputByteArray inputArray)
    {
      SpecialLogFile.AppendTextLine("Invalid data received from telnet server. " +
        "Bytes remaining:" + inputArray.RemainingLength);
      SpecialLogFile.AppendTextLine(inputArray.ToString());

      SpecialLogFile.AppendTextLine("input buffer bytes:");
      var buf = inputArray.DataBytes;
      foreach (var textLine in buf.ToHexReport(16))
      {
        SpecialLogFile.AppendTextLine(textLine);
      }
    }

    private void ThrowInvalidDataReceivedException(InputByteArray inputArray)
    {
      SpecialLogFile.AppendTextLine("Invalid data received from telnet server.");
      SpecialLogFile.AppendTextLine(inputArray.ToString());

      SpecialLogFile.AppendTextLine("input buffer bytes:");
      var buf = inputArray.DataBytes;
      foreach (var textLine in buf.ToHexReport(16))
      {
        SpecialLogFile.AppendTextLine(textLine);
      }
      throw new Exception("Invalid data received from telnet server. " + 
        "Data dumped to c:\\downloads\\SpecialLog.txt.");
    }

    /// <summary>
    /// parse from current byte forward in the InputArray. Will either parse a 
    /// telnet command or a workstation command. 
    /// Route what was parsed to either the FromQueue or the MasterThread. Send to
    /// the FromQueue if still receiving telnet commands. Otherwise, send to the
    /// MasterThread.
    /// </summary>
    /// <param name="InputArray"></param>
    /// <param name="WipCmdList"></param>
    /// <returns></returns>
    private Tuple<WorkstationCommandList,bool> ParseAndPostInputArray( 
      InputByteArray InputArray, WorkstationCommandList WipCmdList)
    {
      WorkstationCommandList wipCmdList = null;
      bool gotSomething = false;

      if (WipCmdList == null)
      {
        var telCmd = InputArray.NextTelnetCommand();
        if (telCmd != null)
        {
          this.TelnetQueue.Enqueue(telCmd);
          gotSomething = true;
        }
      }

      if (gotSomething == false)
      {

        // peek at the input stream from server. Classify the data that is next 
        // to receive.
        var typeData = ServerDataStream.PeekServerCommand(InputArray);
        if (typeData != null)
        {
          var rv = Process5250.ParseWorkstationCommandList(
            InputArray, this.SessionSettings);

          var dsh = rv.Item1;
          var workstationCmdList = rv.Item2;
          var gotEOR = rv.Item3;
          var needMoreBytes = rv.Item4;

          // update connectionComplete flag and typeDevice depending on the stream
          // code of the datastream header.
          if ((this.ConnectionComplete == false) && (dsh != null) && (dsh.StreamCode != null))
          {
            this.ConnectionComplete = true;
            if (dsh.StreamCode.Value == DataStreamCode.Terminal)
              this.TypeDevice = TypeTelnetDevice.Terminal;
            else
              this.TypeDevice = TypeTelnetDevice.Printer;

            // post message to telnet queue so that the ConnectionThread on the
            // other end will know to shutdown.
            var message = new TelnetDeviceAttrMessage(this.TypeDevice.Value);
            this.TelnetQueue.Enqueue(message);
          }

          // got data stream header.
          if (dsh != null)
          {
            if (this.ConnectionComplete == false)
              this.TelnetQueue.Enqueue(dsh);
            else
            {
              var message = new DataStreamHeaderMessage(dsh);
              PostToProcessQueue(message);
            }
            gotSomething = true;
          }

          if (workstationCmdList != null)
            gotSomething = true;

          // accum the workstationCmdList
          if (WipCmdList == null)
            wipCmdList = workstationCmdList;
          else
          {
            wipCmdList = WipCmdList;
            wipCmdList.AddRange(workstationCmdList);
          }

          // got EOR.  store the now completed workstationCmdList.
          if ((gotEOR == true) && (wipCmdList != null))
          {
            var msg = new WorkstationCommandListMessage(wipCmdList);
            PostToProcessQueue(msg);
            gotSomething = true;
            wipCmdList = null;
          }
        }
      }

      return new Tuple<WorkstationCommandList, bool>(wipCmdList, gotSomething);
    }

    private void PostToProcessQueue(ThreadMessageBase Message)
    {
      if ((this.TypeDevice != null) 
        && (this.TypeDevice.Value == TypeTelnetDevice.Printer))
        this.PrinterThread.PostInputMessage(Message);
      else
        this.MasterThread.PostInputMessage(Message);
    }
  }
}
