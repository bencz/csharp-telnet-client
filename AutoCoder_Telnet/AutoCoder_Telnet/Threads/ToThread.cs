using AutoCoder.Ext.System;
using AutoCoder.Systm;
using AutoCoder.Telnet.Common;
using AutoCoder.Telnet.Common.RowCol;
using AutoCoder.Telnet.Common.ScreenDm;
using AutoCoder.Telnet.Enums;
using AutoCoder.Telnet.Enums.IBM5250;
using AutoCoder.Telnet.IBM5250.Common;
using AutoCoder.Telnet.IBM5250.Content;
using AutoCoder.Telnet.IBM5250.Header;
using AutoCoder.Telnet.IBM5250.Response;
using AutoCoder.Telnet.IBM5250.WorkstationCommands;
using AutoCoder.Telnet.IBM5250.WtdOrders;
using AutoCoder.Telnet.TelnetCommands;
using AutoCoder.Telnet.ThreadMessages;
using AutoCoder.Threading;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AutoCoder.Telnet.Threads
{
  public class ToThread : ThreadBase, IThreadBase
  {
    public TcpClient Client
    { get; set; }

    /// <summary>
    /// event signaled when the connection to the telnet server has been lost.
    /// </summary>
    public AutoResetEvent ConnectionFailedEvent
    { get; private set; }

    /// <summary>
    /// the Exception received when the connection failed.
    /// </summary>
    public Exception ConnectionFailedException
    { get; private set; }

    public ConcurrentMessageQueue InputQueue
    { get; set; }

    public ToThread(
      ExtendedManualResetEvent ShutdownFlag, TcpClient Client,
      ScreenDim ScreenDim)
      : base(ShutdownFlag)
    {
      this.Client = Client;
      this.InputQueue = new ConcurrentMessageQueue();
      this.ConnectionFailedEvent = new AutoResetEvent(false);
      this.LogList = new TelnetLogList("To");
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
            if (message is AidKeyResponseMessage)
            {
              var msg = message as AidKeyResponseMessage;
              var content = msg.ScreenContent.GetWorkingContentBlock();
              BuildAndSendResponseDataStream(msg.AidKey, content, this.LogList);
            }

            else if (message is SaveScreenMessage)
            {
              var msg = message as SaveScreenMessage;
              var ra = 
                SaveScreenCommand.BuildSaveScreenResponse(msg.ScreenContent);
              TelnetConnection.WriteToHost(
                null, ra, this.Client.GetStream());
            }

            else if (message is ReadScreenMessage)
            {
              var msg = message as ReadScreenMessage;
              var ra =
                ReadScreenCommand.BuildReadScreenResponse(msg.ScreenContent);
              TelnetConnection.WriteToHost(LogList, ra, this.Client.GetStream());
            }

            else if (message is Query5250ResponseMessage)
            {
              var msg = message as Query5250ResponseMessage;
              var ra = Query5250Response.BuildQuery5250Response();
              TelnetConnection.WriteToHost(LogList, ra, this.Client.GetStream());
            }

            // send the DataBytes contained in the message to the server.
            else if ( message is SendDataMessage)
            {
              var msg = message as SendDataMessage;
              TelnetConnection.WriteToHost(
                LogList, msg.DataBytes, this.Client.GetStream());
            }

            else if (message is GeneralThreadMessage)
            {
              var generalMessage = message as GeneralThreadMessage;
              switch( generalMessage.MessageCode)
              {
                case ThreadMessageCode.ClearLog:
                  {
                    this.LogList.Clear();
                    break;
                  }
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

    private void BuildAndSendResponseDataStream(
      AidKey AidKey, ScreenContent ScreenContent, TelnetLogList LogList = null)
    {

      // send response data stream up to the server. 
      {
        var ra = BuildResponseByteStream(
          ScreenContent,
          AidKey, ScreenContent.CaretRowCol, ScreenContent.HowRead);
        ra = ra.Append(EOR_Command.Build());

        // send response stream back to server.
        TelnetConnection.WriteToHost( LogList, ra, this.Client.GetStream());

        // blank line in the log file.
        var logItem = new LogListItem(Direction.Read, "", true);
        this.LogList.AddItem(logItem);
      }
    }

    private byte[] BuildResponseByteStream(
      ScreenContent ScreenContent,
      AidKey AidKey, OneRowCol CaretRowCol, HowReadScreen? HowRead = null)
    {
      var ba = new ByteArrayBuilder();

      // what to read from the screen.
      HowReadScreen howRead = HowReadScreen.ReadAllInput;
      if (HowRead != null)
        howRead = HowRead.Value;

      {
        var buf = DataStreamHeader.Build(99, TerminalOpcode.PutGet);
        ba.Append(buf);
      }

      // location of caret on the canvas.
      {
        var rowCol = CaretRowCol.ToParentRelative(ScreenContent);
        var buf = ResponseHeader.Build(rowCol as OneRowCol, AidKey);
        ba.Append(buf);
      }

      foreach (var dictItem in ScreenContent.FieldDict)
      {
        ContentField responseField = null;
        var contentItem = dictItem.Value;

        // process only ContentField.
        if (contentItem is ContentField)
        {
          var contentField = contentItem as ContentField;
          responseField = contentField;
        }

        // only process the first of a continued entry field.
        if ((responseField !=null) && ( responseField is ContinuedContentField))
        {
          var contContentField = responseField as ContinuedContentField;
          if (contContentField.ContinuedFieldSegmentCode != ContinuedFieldSegmentCode.First)
            responseField = null;
        }

        if ((howRead == HowReadScreen.ReadMdt)
          && (responseField != null) && (responseField.GetModifiedFlag(ScreenContent) == false))
        {
          responseField = null;
        }

        if (responseField != null)
        {
          IRowCol rowCol = responseField.RowCol.ToOneRowCol().AdvanceRight();
          {
            rowCol = rowCol.ToParentRelative(ScreenContent);
            var buf = SetBufferAddressOrder.Build(rowCol as OneRowCol);
            ba.Append(buf);
          }
          {
            var buf = TextDataOrder.Build(responseField.GetFieldShowText(ScreenContent)) ;
            ba.Append(buf);
          }
        }
      }

      // update length of response data stream.
      var ra = ba.ToByteArray();
      {
        var wk = new ByteArrayBuilder();
        wk.AppendBigEndianShort((short)ra.Length);
        Array.Copy(wk.ToByteArray(), ra, 2);
      }

      return ra;
    }
  }
}
