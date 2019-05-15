using AutoCoder.Ext.Windows;
using AutoCoder.Telnet;
using AutoCoder.Telnet.Common;
using AutoCoder.Telnet.Common.RowCol;
using System;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;
using TextCanvasLib.Ext;
using TextCanvasLib.Visual;

namespace TextCanvasLib.Canvas
{
  /// <summary>
  /// CanvasCaret consists of the canvas it is located on, the timer event that fires
  /// to make the caret blink, the text of the caret symbol, and the location of the
  /// caret.
  /// </summary>
  public class CanvasCaret : IDisposable
  {
    public TextBlock CaretTextBlock
    {
      get;
      set;
    }

    System.Threading.Timer BlinkTimer
    {
      get;
      set;
    }

    ItemCanvas Screen { get; set; }

    string _SymbolText = null;
    string SymbolText
    {
      get
      {
        if (_SymbolText == null)
          _SymbolText = "_";
        return _SymbolText;
      }
      set { _SymbolText = value; }
    }

    private IRowCol _Location;
    public IRowCol Location
    {
      get
      {
        return _Location;
      }
      set
      {
        _Location = value;
      }
    }
    public int ObjectNum
    { get; set; }

    public CanvasCaret(ItemCanvas Screen)
    {
      this.Screen = Screen;
      Screen.Odom += 1;
      this.ObjectNum = Screen.Odom;
      CreateTextBlock();
    }

    private void StartTimer()
    {
      TimerCallback timerDelegate =
          new TimerCallback(TimerTick);
      BlinkTimer = new Timer(timerDelegate, null, 0, 500);
    }

    private void TimerTick(object sender)
    {
      BlinkSymbol(this.Location, null);
    }

    private void CreateTextBlock()
    {
      CaretTextBlock = new TextBlock();
      CaretTextBlock.SetFontDefn(this.Screen.CanvasDefn.FontDefn);
      Screen.Canvas.Children.Add(CaretTextBlock);
      CaretTextBlock.Text = "";
    }

    public void RefreshCaret( )
    {
      var ix = Screen.Canvas.Children.IndexOf(this.CaretTextBlock);
      if (ix >= 0)
        Screen.Canvas.Children.Remove(this.CaretTextBlock);
      CreateTextBlock();
    }

    public void RemoveCaret()
    {
      if ( this.BlinkTimer != null)
      {
        this.BlinkTimer.Dispose();
        this.BlinkTimer = null;
      }

      if (this.CaretTextBlock != null)
      {
        var ix = Screen.Canvas.Children.IndexOf(this.CaretTextBlock);
        if (ix >= 0)
          Screen.Canvas.Children.Remove(this.CaretTextBlock);
        this.CaretTextBlock = null;
      }
    }

    /// <summary>
    /// set the location on the canvas of the caret symbol.
    /// </summary>
    /// <param name="Location"></param>
    public void SetCaretTextBlockLocation(
      IRowCol Location, bool TraceBlinkPos = false )
    {
      var pos = Location.ToCanvasPos( Screen.CanvasDefn.CharBoxDim );
      System.Windows.Controls.Canvas.SetLeft(CaretTextBlock, pos.X);
      System.Windows.Controls.Canvas.SetTop(CaretTextBlock, pos.Y);
    }

    // This method is called by the timer delegate.
    public void BlinkSymbol(
      IRowCol Loc, string SymbolText = null, bool TraceBlinkPos = false)
    {
      if (Screen.Canvas.Dispatcher.CheckAccess() == false)
      {
        Screen.Canvas.Dispatcher.BeginInvoke(
          DispatcherPriority.Input, new ThreadStart(
            () =>
            {
              Blink_Actual(Loc, SymbolText, TraceBlinkPos);
            }));
      }
      else
      {
        Blink_Actual(Loc, SymbolText, TraceBlinkPos);
      }
    }

    // This method is called by the timer delegate.
    private void Blink_Actual(
      IRowCol Loc, string SymbolText = null, bool TraceBlinkPos = false)
    {
      if (CaretTextBlock != null)
      {
        if (SymbolText != null)
          CaretTextBlock.Text = SymbolText;
        else if (CaretTextBlock.Text == this.SymbolText)
          CaretTextBlock.Text = " ";
        else
          CaretTextBlock.Text = this.SymbolText;

        SetCaretTextBlockLocation( this.Location, TraceBlinkPos );
      }
    }
    public void StartBlink(IVisualItem Item, IRowCol Loc, string SymbolText)
    {
      this.Location = Loc;
      this.SymbolText = SymbolText;
      if ( BlinkTimer == null )
        StartTimer();
    }
    public void StartBlink(CanvasPositionCursor Cursor, bool TraceBlinkPos = false)
    {
      this.Location = Cursor.RowCol;
      if (BlinkTimer == null)
      {
        StartTimer();
      }
      BlinkSymbol(this.Location, this.SymbolText, TraceBlinkPos);
    }

    public void Dispose()
    {
      // kill the blink timer.
      if ( this.BlinkTimer != null )
      {
        this.BlinkTimer.Dispose();
        this.BlinkTimer = null;
      }

      RemoveCaret();
    }

    public override string ToString()
    {
      string s1 = "CanvasCaret";
      string s2 = null;
      if (this.Location == null)
        s2 = "Location: null";
      else
        s2 = "Location: " + this.Location.ToString();
      return s1 + " " + s2 + " Symbol:" + this.SymbolText;
    }
  }
}

