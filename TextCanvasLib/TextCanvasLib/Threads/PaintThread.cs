using AutoCoder.Telnet.Common;
using AutoCoder.Telnet.Enums;
using AutoCoder.Telnet.IBM5250.Content;
using AutoCoder.Telnet.IBM5250.WorkstationCommands;
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
using System.Windows.Controls;
using System.Windows.Threading;
using TextCanvasLib.Canvas;
using TextCanvasLib.ContentExt;
using TextCanvasLib.Telnet;
using TextCanvasLib.ThreadMessages;
using TextCanvasLib.ThreadMessages.Hover;
using TextCanvasLib.Threads;

namespace AutoCoder.Telnet.Threads
{
  public class PaintThread : ThreadBase, IThreadBase
  {
    public ConcurrentMessageQueue InputQueue
    {
      get; set;
    }

    private ItemCanvas TelnetCanvas
    { get; set; }

    private Window Window
    { get; set; }

    private TabItem TelnetTabItem
    { get; set; }

    public MasterThread MasterThread
    { get; set; }

    public PaintThread(
      ExtendedManualResetEvent ShutdownFlag, Window Window,
      ItemCanvas TelnetCanvas)
      : base(ShutdownFlag)
    {
      this.InputQueue = new ConcurrentMessageQueue();
      this.Window = Window;
      this.TelnetCanvas = TelnetCanvas;
      this.LogList = new TelnetLogList("Paint");
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
          if ( message != null )
          {
            if (message is PaintCanvasMessage)
            {
              var paintMessage = message as PaintCanvasMessage;
              var screenContent = paintMessage.ScreenContent;
              var bi = Window.Dispatcher.BeginInvoke(
                DispatcherPriority.Input, new ThreadStart(
                  () =>
                  {
                    if (screenContent is WindowScreenContent)
                    {
                      var windowScreenContent = screenContent as WindowScreenContent;

                      // find the existing ItemCanvas associated with the 
                      // WindowScreenContent.
                      var itemCanvas = screenContent.FindItemCanvas(this.TelnetCanvas);
                      //                      var itemCanvas = TelnetCanvas.FindChildCanvas(screenContent.ContentNum);
                      if (itemCanvas == null)
                      {
                        itemCanvas = windowScreenContent.CreateItemCanvas(
                          TelnetCanvas, MasterThread, this);
                      }

                      screenContent.PaintScreenContent(itemCanvas);
                    }
                    else
                    {
                      screenContent.PaintScreenContent(this.TelnetCanvas);
                    }
                  }));
            }

            else if (message is ClearUnitMessage)
            {
              var cu = message as ClearUnitMessage;
              this.TelnetCanvas.ContentNum = cu.ContentNum;
            }

            else if (message is KeyboardInputMessage)
            {
              var kbInput = message as KeyboardInputMessage;
            }

            else if (message is GeneralThreadMessage)
            {
              var generalMessage = message as GeneralThreadMessage;
              var screenContent = generalMessage.ScreenContent;
              switch (generalMessage.MessageCode)
              {
                case ThreadMessageCode.ClearLog:
                  {
                    this.LogList.Clear();
                    break;
                  }
                case ThreadMessageCode.ReportVisualItems:
                  {
                    IEnumerable<string> report = null;

                    var bi = Window.Dispatcher.BeginInvoke(
                      DispatcherPriority.Input, new ThreadStart(
                        () =>
                        {
                          report = screenContent.ReportVisualItems(this.TelnetCanvas);
                        }));
                    bi.Wait();

                    this.LogList.AddReport(Direction.none, report);
                    break;
                  }
              }
            }

            // message sent by TimerTick method of the HoverTimer class.
            // Call draw hover box method on the UI thread.
            // ( should be changed to push processing to a HoverThread. Hover
            //   processing like ODBC calls to server should not be done on the
            //   UI thread. )
            else if (message is HoverMessageBase)
            {
              if (message is CanvasHoverMessage)
              {
                var hoverMessage = message as CanvasHoverMessage;
                var bi = Window.Dispatcher.BeginInvoke(
                  DispatcherPriority.Input, new ThreadStart(
                    () =>
                    {
                      hoverMessage.ItemCanvas.DrawHoverBox(hoverMessage.HoverPos);
                    }));
              }
              else if (message is SuspendHoverMessage)
              {
                this.TelnetCanvas.HoverTimer.SuspendHover();
              }
              else if (message is ResumeHoverMessage)
              {
                this.TelnetCanvas.HoverTimer.ResumeHover();
              }
              else if (message is DelayHoverMessage)
              {
                this.TelnetCanvas.HoverTimer.DelayHover();
              }
            }

            // message from match thread. Telling PaintThread of latest match.
            else if (message is MatchScreenDefnMessage)
            {
              var screenDefn = message as MatchScreenDefnMessage;
              var bi = Window.Dispatcher.BeginInvoke(
                DispatcherPriority.Input, new ThreadStart(
                  () =>
                  {
                    this.TelnetCanvas.MatchScreenDefn = screenDefn.ScreenDefn;
                  }));
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
  }
}
