using AutoCoder.Telnet.Common;
using AutoCoder.Telnet.Common.ScreenDm;
using AutoCoder.Telnet.Enums;
using AutoCoder.Telnet.Enums.IBM5250;
using AutoCoder.Telnet.IBM5250.Content;
using AutoCoder.Telnet.IBM5250.WorkstationCommands;
using AutoCoder.Telnet.ThreadMessages;
using AutoCoder.Telnet.Threads;
using AutoCoder.Threading;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TextCanvasLib.Canvas;
using TextCanvasLib.ContentExt;
using TextCanvasLib.ThreadMessages;

namespace TextCanvasLib.Threads
{
  /// <summary>
  /// master ScreenContent thread.
  /// Thread receives input that is applied to the master ScreenContent block.
  /// MasterThread sends signal to PaintThread when the ScreenContent block is
  /// ready to be painted.
  /// </summary>
  public class MasterThread : ThreadBase, IThreadBase
  {
    public ConcurrentMessageQueue InputQueue
    {
      get; set;
    }
    public ToThread ToThread
    { get; set; }

    public PaintThread PaintThread
    { get; set; }
    public IThreadBase MatchThread
    { get; set; }
    private ScreenDim ScreenDim
    { get; set; }

    private ExtendedManualResetEvent ConnectionFailedEvent
    { get; set; }

    public ConcurrentOdom ContentOdom
    {
      get; set;
    }
    private ItemCanvas TelnetCanvas
    { get; set; }


    public ScreenContent BaseMaster
    { get; set; }

    /// <summary>
    /// the aid key that resulted in the prior screen being posted to the server.
    /// </summary>
    private AidKey? PostAidKey
    { get; set; }

    public MasterThread(
      ExtendedManualResetEvent ShutdownFlag, ExtendedManualResetEvent ConnectionFailedEvent,
      ScreenDim ScreenDim, ItemCanvas TelnetCanvas)
      : base(ShutdownFlag)
    {
      this.ContentOdom = new ConcurrentOdom();
      this.InputQueue = new ConcurrentMessageQueue();
      this.ConnectionFailedEvent = ConnectionFailedEvent;
      this.ScreenDim = ScreenDim;
      this.TelnetCanvas = TelnetCanvas;
      this.LogList = new TelnetLogList("Master");
    }

    public void EntryPoint()
    {
      this.ThreadEndedEvent.Reset();
      var master = BaseMaster_InitialSetup();

      try
      {
        // loop receiving from the server until:
        //   - the foreground thread wants to shutdown the connection. It has set
        //     the ShutdownFlag event.
        while ((ShutdownFlag.State == false) 
          && ( this.ConnectionFailedEvent.State == false ))
        {
          var message = InputQueue.WaitAndDequeue(
            this.ShutdownFlag.EventObject, this.ConnectionFailedEvent.EventObject);
          if (message != null)
          {
            if (message is WorkstationCommandListMessage)
            {
              var cmdList = (message as WorkstationCommandListMessage).WorkstationCommandList;

              var rv = this.BaseMaster.Apply(
                cmdList, this.ToThread, this.PaintThread, this.LogList);
              bool wtdApplied = rv.Item1;
              this.BaseMaster = rv.Item2;

              // signal the paint thread to paint the canvas with the screen
              // content block.
              if (wtdApplied == true)
              {
                this.BaseMaster.PostAidKey = this.PostAidKey;
                this.PostAidKey = null;
                var masterCopy = this.BaseMaster.Copy();
                var content = masterCopy.GetWorkingContentBlock();
                var paintMessage = new PaintCanvasMessage(content);
                this.PaintThread.PostInputMessage(paintMessage);
              }

              // send another copy of the screenContent to the match thread.
              // Match thread will match the screen to the screen definitions.
              // Looking for screen id, hover code, help text, etc.
              if (wtdApplied == true)
              {
                var masterCopy = this.BaseMaster.Copy();
                var content = masterCopy.GetWorkingContentBlock();
                this.MatchThread.PostInputMessage(
                  ThreadMessageCode.MatchScreenContentToScreenDefn, content);
              }
            }

            else if (message is KeyboardInputMessage)
            {
              var kbInput = message as KeyboardInputMessage;
              if ( kbInput.Text != null)
              {
                master = this.BaseMaster.GetWorkingContentBlock();
                master.ApplyInput(kbInput);
              }
            }

            // message sent from UI thread when the caret is moved. See the 
            // ItemCanvas class.
            else if (message is CaretMoveMessage)
            {
              var caretMove = message as CaretMoveMessage;
              master = this.BaseMaster.GetWorkingContentBlock();
              master.ApplyCaretMove(caretMove);
            }

            // enter key has been pressed. UI thread sent the message to this
            // Master thread. Master thread relays the message to the
            // ToThread along with a copy of the master ScreenContent. ToThread 
            // creates and sends the response data stream based on the master
            // screen content.
            else if ( message is AidKeyResponseMessage)
            {
              var msg = message as AidKeyResponseMessage;
              this.PostAidKey = msg.AidKey;
              msg.ScreenContent = this.BaseMaster.Copy();
              ToThread.PostInputMessage(msg);
            }

            else if ( message is GeneralThreadMessage)
            {
              var generalMessage = message as GeneralThreadMessage;
              switch(generalMessage.MessageCode)
              {
                case ThreadMessageCode.ClearLog:
                  {
                    this.LogList.Clear();
                    break;
                  }

                  // report visual items. Add screenContent to the message and
                  // send it to PaintThread.
                case ThreadMessageCode.ReportVisualItems:
                  {
                    generalMessage.ScreenContent = this.BaseMaster.Copy();
                    PaintThread.PostInputMessage(generalMessage);
                    break;
                  }
              }
            }

            else if ( message is ExchangeMessage)
            {
              var exchangeMessage = message as ExchangeMessage;
              if ( exchangeMessage.MessageCode == ThreadMessageCode.GetScreenContent)
              {
                master = this.BaseMaster.GetWorkingContentBlock();
                var masterCopy = master.Copy();
                exchangeMessage.PostReplyMessage(masterCopy);
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

    private ScreenContent BaseMaster_InitialSetup()
    {
      this.BaseMaster = new ScreenContent(null, this.ScreenDim, this.ContentOdom);

      // send message to paint thread telling it the contentNum of the master 
      // screen content.
      {
        var cu = new ClearUnitMessage(this.BaseMaster.ContentNum);
        this.PaintThread.PostInputMessage(cu);
      }

      return this.BaseMaster;
    }
  }
}
