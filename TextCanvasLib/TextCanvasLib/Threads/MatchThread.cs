using AutoCoder.Serialize;
using AutoCoder.Telnet.Common;
using AutoCoder.Telnet.Enums;
using AutoCoder.Telnet.Enums.IBM5250;
using AutoCoder.Telnet.IBM5250.Content;
using AutoCoder.Telnet.IBM5250.WorkstationCommands;
using AutoCoder.Telnet.LogFiles;
using AutoCoder.Telnet.ThreadMessages;
using AutoCoder.Threading;
using ScreenDefnLib.Defn;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using TextCanvasLib.Canvas;
using TextCanvasLib.ContentExt;
using TextCanvasLib.Telnet;
using TextCanvasLib.ThreadMessages;
using TextCanvasLib.Threads;

namespace AutoCoder.Telnet.Threads
{
  public class MatchThread : ThreadBase, IThreadBase
  {
    public ConcurrentMessageQueue InputQueue
    {
      get; set;
    }

    public ScreenDefnList ScreenDefnList
    { get; set; }

    public IThreadBase MasterThread
    { get; set; }
    public IThreadBase PaintThread
    { get; set; }

    // see TelnetWindowInputSignalHandler in MainWindow.xaml.cs
    private Action<ThreadMessageBase> TelnetWindowInputSignal;

    // copy of capture attributes from the client window.
    public string CaptureFolderPath { get; set; }
    public bool CaptureAuto { get; set; }


    public MatchThread( 
      ExtendedManualResetEvent ShutdownFlag, IThreadBase MasterThread, 
      IThreadBase PaintThread,
      Action<ThreadMessageBase> TelnetWindowInputSignal)
      : base(ShutdownFlag)
    {
      this.InputQueue = new ConcurrentMessageQueue();
      this.MasterThread = MasterThread;
      this.PaintThread = PaintThread;
      this.TelnetWindowInputSignal = TelnetWindowInputSignal;
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

            // list of defined screens. Message sent by the UI thread. Idea being
            // that as a new screen is defined want to send message to this match
            // thread so it will update itself with the current list.
            if (message is AssignScreenDefnListMessage)
            {
              var assignMessage = message as AssignScreenDefnListMessage;
              this.ScreenDefnList = assignMessage.ScreenDefnList;
            }

            else if (message is GeneralThreadMessage)
            {
              var generalMessage = message as GeneralThreadMessage;
              var screenContent = generalMessage.ScreenContent;
              switch (generalMessage.MessageCode)
              {

                // message sent by MasterThread. MasterThread has completed the
                // screen content. It sends message to PaintThread. And this msg
                // to this MatchThread.
                case ThreadMessageCode.MatchScreenContentToScreenDefn:
                  {
                    var matchDefn = FindMatchingScreenDefn(
                      screenContent, this.ScreenDefnList);

                    // send match results to paint thread and tnClient window.
                    // ( send results whether a match found or not. )
                    var matchMessage =
                      new MatchScreenDefnMessage(matchDefn, screenContent);
                    this.PaintThread.PostInputMessage(matchMessage);

                    // signal back to tnClient window.
                    this.TelnetWindowInputSignal(matchMessage);
                    break;
                  }
              }
            }

            else if ( message is CaptureAttributesMessage)
            {
              var captureMessage = message as CaptureAttributesMessage;
              this.CaptureAuto = captureMessage.CaptureAuto;
              this.CaptureFolderPath = captureMessage.CaptureFolderPath;
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

    /// <summary>
    /// the most recent screen capture report. Save the report so that report
    /// data can be accumulated with the postAidKey if pagedown.
    /// </summary>
    private DataItemReport CaptureReport
    { get; set; }

    private IScreenDefn FindMatchingScreenDefn(
      ScreenContent Content, ScreenDefnList ScreenDefnList)
    {
      IScreenDefn matchDefn = null;
      foreach ( var screenDefn in ScreenDefnList)
      {
        var isMatch = screenDefn.Match(Content);
        if ( isMatch == true )
        {
          matchDefn = screenDefn;
          break;
        }
      }
      return matchDefn;
    }
  }
}
