using AutoCoder.Ext.System;
using AutoCoder.Ext.System.Collections.Generic;
using AutoCoder.Ext.Windows;
using AutoCoder.Telnet.LogFiles;
using AutoCoder.Telnet.Threads;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using TextCanvasLib.Canvas;
using TextCanvasLib.ThreadMessages;
using TextCanvasLib.ThreadMessages.Hover;

namespace TextCanvasLib.Hover
{
  /// <summary>
  /// record mousepos every second.  Detect when mousePos has not changed
  /// for the last 3 seconds and initiate hover processing.
  /// 
  /// HoverTimer object is owned by the ItemCanvas. ItemCanvas calls 
  /// </summary>
  public class HoverTimer : IDisposable
  {
    public ItemCanvas ItemCanvas
    { get; set; }

    /// <summary>
    /// timer  ticks every second. Used to determine that mouse is hovering on
    /// the screen.
    /// </summary>
    System.Threading.Timer Timer
    {
      get;
      set;
    }

    Point CurrentHoverPos
    { get; set; }
    Point[] RecentMousePos
    { get; set; }

    public int? TimerInterval
    {
      get
      {
        if (_TimerInterval == null)
          _TimerInterval = 500;
        return _TimerInterval;
      }
      set { _TimerInterval = value; }
    }
    private int? _TimerInterval;


    public HoverTimer(int Interval, ItemCanvas ItemCanvas)
    {
      this.ItemCanvas = ItemCanvas;

      StartTimer(Interval);
    }

    private void KillTimer( )
    {
      if (this.Timer != null)
      {
        this.Timer.Dispose();
        this.Timer = null;
      }
    }

    public void DelayHover( )
    {
      if ( this.Timer != null)
      {
        this.RecentMousePos.ArrayInit(new Point(0, 0));
      }
    }
    public void ResumeHover( )
    {
      if ( this.Timer == null)
      {
        StartTimer(this.TimerInterval.Value);
      }
    }
    public void SuspendHover()
    {
      KillTimer();
    }

    private void StartTimer(int Interval)
    {
      // save the interval value. Use when suspend and resume the timer.
      this.TimerInterval = Interval;

      TimerCallback timerDelegate =
          new TimerCallback(TimerTick);
      this.Timer = new Timer(timerDelegate, null, 0, this.TimerInterval.Value );

      this.RecentMousePos = new Point[5];
      this.RecentMousePos.ArrayInit(new Point(0, 0));
      this.CurrentHoverPos = new Point(0, 0);
    }

    private void TimerTick(object sender)
    {
      if (this.RecentMousePos != null)
      {
        // push MousePos onto array of most recent mousepos.
        this.RecentMousePos.ArrayPush(this.ItemCanvas.MousePosOnCanvas);

        // detect if the mouse is hovering.
        var hoverPos = this.GetHoverPos(3);
        if ((hoverPos != null) 
          && (hoverPos.Value != this.CurrentHoverPos) 
          && (this.ItemCanvas.PaintThread != null))
        {

          // send hover message to the paint thread.
          this.CurrentHoverPos = hoverPos.Value;
          var msg = new CanvasHoverMessage(this.ItemCanvas, hoverPos.Value);
          this.ItemCanvas.PaintThread.PostInputMessage(msg);
        }
      }
    }

    public void Dispose()
    {
      KillTimer();
    }

    /// <summary>
    /// determine if mouse is hovering in the same location the last 3 seconds.
    /// </summary>
    public Point? GetHoverPos(int depth)
    {
      Point? hoverPos = null;
      hoverPos = this.RecentMousePos[0];
      for (int ix = 0; ix <= depth - 1; ++ix)
      {
        if (this.RecentMousePos[ix].IsZeroPoint() == true)
        {
          hoverPos = null;
          break;
        }
        else if (this.RecentMousePos[ix] != hoverPos.Value)
        {
          hoverPos = null;
          break;
        }
      }
      return hoverPos;
    }
  }
}
