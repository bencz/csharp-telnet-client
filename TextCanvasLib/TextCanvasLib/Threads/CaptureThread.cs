using AutoCoder.Ext.System.Data;
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
using System.Data;
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
  public class CaptureThread : ThreadBase, IThreadBase
  {
    public ConcurrentMessageQueue InputQueue
    {
      get; set;
    }

    public IThreadBase MasterThread
    { get; set; }

    public CaptureThread(
      ExtendedManualResetEvent ShutdownFlag, IThreadBase MasterThread)
      : base(ShutdownFlag)
    {
      this.InputQueue = new ConcurrentMessageQueue();
      this.MasterThread = MasterThread;
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
            if (message is CaptureContentMessage)
            {
              var captureMessage = message as CaptureContentMessage;
              DoCapture(captureMessage.ScreenDefn, captureMessage.ScreenContent,
                captureMessage.CaptureFolderPath);
            }
            if (message is GeneralThreadMessage)
            {
              var generalMessage = message as GeneralThreadMessage;
              var screenContent = generalMessage.ScreenContent;
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
    private DataTable CaptureReport
    { get; set; }

    private void DoCapture(IScreenDefn screenDefn, ScreenContent content,
      string CaptureFolderPath )
    {
      var itemReport = screenDefn.Capture(content);

      // accum capture data if post aid key is pagedown.
      if ((content.PostAidKey != null)
        && (this.CaptureReport != null)
        && (content.PostAidKey.Value == AidKey.RollUp))
      {
        var r2 = DataTableExt.CombineVertically(this.CaptureReport, itemReport);
        this.CaptureReport = r2;
      }
      else
      {
        this.CaptureReport = itemReport;
      }

      var htmlText = this.CaptureReport.ToHtmlTableText();

      var fileName = screenDefn.ScreenName + "." + "html";
      var captureFilePath = System.IO.Path.Combine(CaptureFolderPath, fileName);
      System.IO.File.WriteAllText(captureFilePath, htmlText);
    }
  }
}
