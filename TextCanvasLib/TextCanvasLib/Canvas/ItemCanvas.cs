using System;
using System.Collections.Generic;
using System.Linq;
using TextCanvasLib.xml;
using System.Windows;
using System.Windows.Input;
using TextCanvasLib.Enums;
using TextCanvasLib.Common;
using TextCanvasLib.Visual;
using AutoCoder.Ext.Windows.Input;
using AutoCoder.Telnet.IBM5250.WorkstationCommands;
using AutoCoder.Telnet.IBM5250.WtdOrders;
using AutoCoder.Telnet.Common;
using System.ComponentModel;
using AutoCoder.Telnet.Common.RowCol;
using AutoCoder.Telnet.Enums.IBM5250;
using AutoCoder.Telnet.Enums;
using AutoCoder.Telnet.IBM5250.Content;
using TextCanvasLib.Threads;
using AutoCoder.Telnet.ThreadMessages;
using AutoCoder.Telnet;
using AutoCoder.Systm;
using System.Windows.Controls;
using AutoCoder.Ext.Windows;
using AutoCoder.Ext.Controls;
using AutoCoder.Telnet.LogFiles;
using AutoCoder.Telnet.Threads;
using System.Windows.Media;
using TextCanvasLib.Hover;
using ScreenDefnLib.Defn;
using AutoCoder.Telnet.Common.ScreenDm;
using AutoCoder.Telnet.Common.ScreenLoc;
using AutoCoder.Enums;
using TextCanvasLib.ThreadMessages.Hover;
using TextCanvasLib.Keybd;

namespace TextCanvasLib.Canvas
{
  /// <summary>
  /// item canvas renders a list of ScreenVisualItems on the screen. Cursor is
  /// located in an item and advances from one item to the next. Keyboard entry
  /// is applied to the visual item where the cursor is located.
  /// </summary>
  public class ItemCanvas : INotifyPropertyChanged, IDisposable
  {
    public ItemCanvas Parent
    { get; set; }

    /// <summary>
    /// window canvases which are placed on the main canvas.
    /// </summary>
    public ItemCanvas[] Children
    { get; set; }

    /// <summary>
    /// the  ContentNum of the corresponding ScreenContent.
    /// </summary>
    public int ContentNum
    { get; set;  }

    public CharPoint ContentStart
    { get; set; }
    public int Odom { get; set; }
    public CanvasDefn CanvasDefn
    { get; set; }
    public double FontPointSize
    {
      get { return CanvasDefn.FontDefn.PointSize; }
    }

    /// <summary>
    /// the actual canvas on which the textblocks are drawn from 5250 display 
    /// orders.
    /// </summary>
    public System.Windows.Controls.Canvas Canvas
    { get; set; }

    public LogFile LogFile { get; set; }

    protected IRowCol CaretLoc { get; set; }

    public ScreenVisualItems VisualItems
    { get; set; }

    public IScreenDefn MatchScreenDefn
    { get; set; }

    private WriteToDisplayCommand _WriteToDisplayCommand;

    public WriteToDisplayCommand WriteToDisplayCommand
    {
      get { return _WriteToDisplayCommand; }
      set
      {
        if (_WriteToDisplayCommand != value)
        {
          _WriteToDisplayCommand = value;
          RaisePropertyChanged("WriteToDisplayCommand");
        }
      }
    }

    protected CanvasCaret CanvasCaret { get; set; }

    public delegate void
    ExitScreenEventHandler(object o, EventArgs args);

    public event ExitScreenEventHandler ScreenExitEvent;
    List<ShowItemBase> ShowItemList { get; set;  }

    /// <summary>
    /// for now. When the Canvas is drawn in a window, this is the border
    /// control that is the parent of the Canvas.
    /// </summary>
    public Border BorderControl
    { get; set; }

    public event Action<ItemCanvas> CanvasChanged;

    /// <summary>
    /// the visual item the caret is located in. And the position in the visual item.
    /// ( or if located outside of visualItem, the RowCol position on the canvas. )
    /// </summary>
    private CanvasPositionCursor _CaretCursor;
    public CanvasPositionCursor CaretCursor
    {
      get { return _CaretCursor; }
      set
      {
        _CaretCursor = value;

        // send the keyboard input to the master thread. That thread applies input
        // and WTD commands to the master screenContent block.
        if ((this.MasterThread != null) && ( this.DoHandleUserInput == true ))
        {
          var caretMove = new CaretMoveMessage()
          {
            RowCol = this.CaretCursor.RowCol as ZeroRowCol
          };
          this.MasterThread.PostInputMessage(caretMove);
//          this.MasterThread.MasterQueue.Enqueue(caretMove);
        }
      }
    }

    /// <summary>
    /// see MouseMove event handler. Store the current location of the mouse on
    /// the canvas.
    /// </summary>
    public Point MousePosOnCanvas
    { get; set; }


    /// <summary>
    /// determine if this ItemCanvas should handle user input.  Depends on if
    /// this item canvas is canvas of main screen or a window.
    /// ( If this canvas has child canvases, then input is processed by the
    ///   item canvas of those child canvases. Not this parent canvas. )
    /// </summary>
    public bool DoHandleUserInput
    {
      get
      {
        bool doHandle = false;

        // ItemCanvas is in a child window. Any clicks within that canvas is
        // handled by this ItemCanvas.
        if (this.IsChild == true)
          doHandle = true;

        // nothing on the item canvas.
        else if (this.MasterScreenContent == null)
          doHandle = false;

        // this is the main screen item canvas. If no windows on the canvas
        // then the canvas is accepting input. Handle the input.
        else if (this.MasterScreenContent.HasChildren == false)
          doHandle = true;

        // the main canvas has windows on it. The main canvas accepts no input
        // while there is a window on it.
        else
          doHandle = false;

        return doHandle;
      }
    }

    /// <summary>
    /// timer  ticks every second. Used to determine that mouse is hovering on
    /// the screen.
    /// </summary>
    public HoverTimer HoverTimer
    {
      get;
      set;
    }

    public HowReadScreen? HowRead
    { get; set; }

    /// <summary>
    /// this ItemCanvas is the canvas of a child canvas.  That is a window on
    /// the main canvas.
    /// </summary>
    public bool IsChild
    {
      get { return (this.ContentStart != null); }
    }

    public ScreenDim ScreenDim
    {
      get { return this.CanvasDefn.ScreenDim; }
    }

    public MasterThread MasterThread
    { get; set; }
    public PaintThread PaintThread
    { get; set; }

    public HoverBox HoverBox
    { get; set; }

    /// <summary>
    /// Mouse is hovering over an item in the 5250 canvas. Call into the hover box
    /// class to fill the hover window and make it visible.
    /// Called from PaintThread. PaintThread handles the CanvasHoverMessage.
    /// </summary>
    /// <param name="hoverPos"></param>
    public void DrawHoverBox(Point hoverPos)
    {
      // calc the row/col the hover position.
      var rowCol = hoverPos.CanvasPosToRowCol(
        this.CanvasDefn.CharBoxDim, this.ContentStart);

      this.HoverBox.DrawHoverBox(
        hoverPos, rowCol as IScreenLoc, this.MatchScreenDefn, 
        this.MasterScreenContent);
    }

    /// <summary>
    /// a reference to the ScreenContent of the master thread. Used for reading
    /// only. Used when ItemCanvas code has to determine if the ScreenContent of
    /// the master thread has a window or not.
    /// </summary>
    public ScreenContent MasterScreenContent
    {
      get
      {
        if (this.MasterThread == null)
          return null;
        else
          return this.MasterThread.BaseMaster;
      }
    }

    public ItemCanvas(System.Windows.Controls.Canvas Canvas,
      ScreenDim ScreenDim, double FontPointSize, MasterThread MasterThread,
      PaintThread PaintThread, int ContentNum)
    {
      this.Canvas = Canvas;
      this.CanvasDefn = new CanvasDefn(ScreenDim, FontPointSize);

      this.CaretLoc = new ZeroRowCol(0, 0);
      this.VisualItems = new ScreenVisualItems();
      this.CanvasCaret = new CanvasCaret(this);
      this.MasterThread = MasterThread;
      this.PaintThread = PaintThread;

      this.HoverBox = new HoverBox(true);
      this.HoverBox.HoverWindowMouseEnterLeave += HoverBox_HoverWindowMouseEnterLeave;
      this.HoverBox.HoverWindowChanged += HoverBox_HoverWindowChanged;

      // the ContentNum of the ScreenContent associated with this ItemCanvas.
      this.ContentNum = ContentNum;

      HookUserInputEvents();
      // this.HoverTimer = new HoverTimer(650, this);
    }

    /// <summary>
    /// the hover window is being dragged on the screen or its border being 
    /// dragged to resize it. While that is going on, prevent mouse hover 
    /// detection from happening on the canvas.
    /// </summary>
    /// <param name="arg1"></param>
    /// <param name="arg2"></param>
    private void HoverBox_HoverWindowChanged(HoverBox arg1, WindowChangedCode arg2)
    {
      var msg = new DelayHoverMessage();
      this.PaintThread.PostInputMessage(msg);
    }

    /// <summary>
    /// the mouse has entered or left the hover window. From this item canvas, 
    /// signal the paint thread that the hover timer should be suspended or 
    /// resumed.
    /// </summary>
    /// <param name="arg1"></param>
    /// <param name="arg2"></param>
    private void HoverBox_HoverWindowMouseEnterLeave(HoverBox arg1, EnterLeaveCode arg2)
    {
      if ( arg2 == EnterLeaveCode.Enter )
      {
        var msg = new SuspendHoverMessage();
        this.PaintThread.PostInputMessage(msg);
      }
      else if (arg2 == EnterLeaveCode.Leave)
      {
        var msg = new ResumeHoverMessage();
        this.PaintThread.PostInputMessage(msg);
      }
    }

    public ItemCanvas(
      System.Windows.Controls.Canvas canvas, double charWidth, double charHeight,
      ScreenDim ScreenDim, double FontPointSize, PaintThread PaintThread)
      : this(canvas, ScreenDim, FontPointSize, null, PaintThread, 0 )
    {
    }

    public void HookBorderDrag( )
    {
      if (this.BorderControl != null)
      {
        this.BorderControl.MouseLeftButtonDown += WindowBorder_MouseLeftButtonDown;
        this.BorderControl.MouseLeftButtonUp += WindowBorder_MouseLeftButtonUp;
        this.BorderControl.MouseMove += WindowBorder_MouseMove;
      }
    }

    Point? BorderMouseDownPos;
    private void WindowBorder_MouseMove(object sender, MouseEventArgs e)
    {
      if ( this.BorderMouseDownPos != null)
      {
        var pos = e.GetPosition(this.BorderControl.Parent as UIElement);
        var diff = pos.SubPoint(this.BorderMouseDownPos.Value);

        var borderPos = CanvasExt.GetUpperLeft(this.BorderControl);
        borderPos = borderPos.AddPoint(diff);
        CanvasExt.SetUpperLeft(this.BorderControl, borderPos);
        this.BorderMouseDownPos = pos;
      }
    }

    private void WindowBorder_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
    {
      this.BorderMouseDownPos = null;
    }

    private void WindowBorder_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
      this.BorderMouseDownPos = e.GetPosition(this.BorderControl.Parent as UIElement);
    }

    public void HookUserInputEvents( )
    {
      this.Canvas.PreviewKeyDown -= Item_PreviewKeyDown;
      this.Canvas.MouseLeftButtonDown -= Canvas_MouseLeftButtonDown;

      this.Canvas.PreviewKeyDown += Item_PreviewKeyDown;
      this.Canvas.MouseLeftButtonDown += Canvas_MouseLeftButtonDown;
      this.Canvas.MouseMove += Canvas_MouseMove;
      this.Canvas.Focusable = true;
    }

    /// <summary>
    /// add the Child ItemCanvas to Children collection of this parent.
    /// </summary>
    /// <param name="Child"></param>
    public void AddChild(ItemCanvas Child)
    {
      // the child points up to this parent.
      Child.Parent = this;

      // add child to end of list of Children.
      if (this.Children == null)
      {
        this.Children = new ItemCanvas[1];
        this.Children[0] = Child;
      }
      else
      {
        var childList = this.Children.ToList();
        childList.Add(Child);
        this.Children = childList.ToArray();
      }
    }

    /// <summary>
    /// important function. Places the actual frameworkElement of the VisualItem on
    /// the canvas.
    /// </summary>
    /// <param name="visualItem"></param>
    /// <param name="Elem"></param>
    public Point AddItemToCanvas(IVisualItemMore visualItem, FrameworkElement Elem)
    {
      // start column is beyond screen size. Advance to column 0 of the next row.
      var rowCol = visualItem.ShowRowCol();

      // calc the x,y dot position of the visual item.
      var pos = rowCol.ToCanvasPos(this.CanvasDefn.CharBoxDim);

      this.Canvas.Children.Add(Elem);
      System.Windows.Controls.Canvas.SetLeft(Elem, pos.X);
      System.Windows.Controls.Canvas.SetTop(Elem, pos.Y);

      visualItem.IsOnCanvas = true;

      return pos;
    }

    public Point AddItemToCanvas(IRowCol RowCol, FrameworkElement Elem)
    {
      // calc the x,y dot position of the visual item.
      var pos = RowCol.ToCanvasPos(this.CanvasDefn.CharBoxDim);

      this.Canvas.Children.Add(Elem);
      System.Windows.Controls.Canvas.SetLeft(Elem, pos.X);
      System.Windows.Controls.Canvas.SetTop(Elem, pos.Y);

      return pos;
    }

    private void WriteToItemCanvasLog(IVisualItemMore visualItem)
    {
      var rowCol = visualItem.ShowRowCol( );
      SpecialLogFile.AppendTextLine(
        "write to canvas. " + rowCol.ToString() + " " + visualItem.ToString());
    }

    private void Canvas_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
      var pos = e.GetPosition((IInputElement)this.Canvas);
      var rowCol = pos.CanvasPosToRowCol(this.CanvasDefn.CharBoxDim, this.ContentStart);

      //      rowCol = this.AdjustShowRowCol.LocalPosToParentPos(rowCol);
      if (this.DoHandleUserInput == true)
      {
        this.CaretCursor = VisualItems.FindVisualItemCanvas(rowCol);
        this.CanvasCaret.StartBlink(this.CaretCursor);
        this.Canvas.Focus();
      }
    }

    private void Canvas_MouseMove(object sender, MouseEventArgs e)
    {
      // pos of mouse on the canvas.
      var pos = e.GetPosition(this.Canvas);
      this.MousePosOnCanvas = pos;
    }

    public void SetFocus( )
    {
      if ( this.Canvas != null)
      {
        this.Canvas.Focus();
      }
    }

    private static Key[] EnterKeyCodeArray = 
      new Key[] {
        Key.Enter, Key.PageDown, Key.PageUp, Key.F1, Key.F2, Key.F3, Key.F4,
        Key.F5, Key.F6, Key.F7, Key.F8, Key.F9, Key.F10, Key.F11, Key.F12
      };

    /// <summary>
    /// erase the screen. Clear the collection of visual items.
    /// </summary>
    public void EraseScreen()
    {
      RemoveCaret();
      this.Canvas.Children.Clear();
      this.VisualItems.Clear();
      this.CanvasCaret = new CanvasCaret(this);
      this.DisposeChildren();
    }

    public void RemoveCaret( )
    {
      if (this.CanvasCaret != null)
        this.CanvasCaret.Dispose();
      this.CanvasCaret = new CanvasCaret(this);
    }

    public ItemCanvas FindChildCanvas(int contentNum)
    {
      ItemCanvas found = null;
      if (this.Children != null)
      {
        foreach (var child in this.Children)
        {
          if (child.ContentNum == contentNum)
          {
            found = child;
            break;
          }
        }
      }
      return found;
    }

    private void Item_PreviewKeyDown(object sender, KeyEventArgs e)
    {

      string keyText = e.Key.ToTextInput();

      if (this.DoHandleUserInput == true)
      {
        var modkey = e.ToModifiedKey();
        if (keyText != null)
        {
          this.CaretCursor = PutKeyboardText_AdvanceCaret(this.CaretCursor, keyText);
          this.CanvasCaret.StartBlink(this.CaretCursor, true);
        }

        else if (e.Key == Key.Tab)
        {
          if (Keyboard.IsKeyDown(Key.LeftShift) == false)
            this.CaretCursor = this.CaretCursor.NextInputItemCircular(
              this.VisualItems, VisualFeature.tabTo);
          else
            this.CaretCursor = this.CaretCursor.PrevInputItemCircular(
              this.VisualItems, VisualFeature.tabTo);
          this.CanvasCaret.StartBlink(this.CaretCursor, true);
          e.Handled = true;
        }
        else if (e.Key == Key.Enter)
        {
          if (KeyboardExt.IsShiftDown())
          {
            this.CaretCursor = this.CaretCursor.NextInputItemNextLine(this.VisualItems);
            this.CanvasCaret.StartBlink(this.CaretCursor);
          }
          else
          {
            PostAidKeyResponseMessage(e.Key);
          }
        }
        else if (e.Key == Key.Right)
        {
          this.CaretCursor = this.CaretCursor.AdvanceRight(
            this.VisualItems, HowAdvance.NextColumn);
          this.CanvasCaret.StartBlink(this.CaretCursor);
          e.Handled = true;
        }
        else if (e.Key == Key.Left)
        {
          this.CaretCursor = this.CaretCursor.AdvanceLeft(
            this.VisualItems, HowAdvance.NextColumn);
          this.CanvasCaret.StartBlink(this.CaretCursor);
          e.Handled = true;
        }
        else if (e.Key == Key.Down)
        {
          this.CaretCursor = this.CaretCursor.AdvanceDown(this.VisualItems);
          this.CanvasCaret.StartBlink(this.CaretCursor);
          e.Handled = true;
        }
        else if (e.Key == Key.Up)
        {
          this.CaretCursor = this.CaretCursor.AdvanceUp(this.VisualItems);
          this.CanvasCaret.StartBlink(this.CaretCursor);
          e.Handled = true;
        }
        else if (e.Key == Key.Back)
        {
          this.CaretCursor = this.CaretCursor.AdvanceLeft(
            this.VisualItems, HowAdvance.NextColumn);
          if (this.CaretCursor.IsInputItem() == true)
          {
            PutKeyboardText(this.CaretCursor, " ");
          }
          this.CanvasCaret.StartBlink(this.CaretCursor);
          e.Handled = true;
        }

        else if (e.Key == Key.Delete)
        {
          PutKeyboardText(this.CaretCursor, " ");
          this.CanvasCaret.StartBlink(this.CaretCursor);
          e.Handled = true;
        }

        // escape key. delete hoverbox.
        else if (e.Key == Key.Escape)
        {
          if (this.HoverBox != null)
          {
            this.HoverBox.RemoveHoverBox();
          }
        }

        else if (Array.IndexOf(EnterKeyCodeArray, e.Key) != -1)
        {
          PostAidKeyResponseMessage(e.Key);
        }

#if skip
        else if (e.Key == Key.System)
        {
          PostAidKeyResponseMessage(e.Key);
          e.Handled = true;
        }
#endif

        SignalCanvasChanged();
      }
    }

    private void PostAidKeyResponseMessage(Key key)
    {
      var aidKey = key.ToAidKey( KeyboardExt.IsShiftDown());
      var msg = new AidKeyResponseMessage(aidKey.Value);
      this.MasterThread.PostInputMessage(msg);
    }

    private void PostKeyboardInput(Key key)
    {
      // send the keyboard input to the master thread. That thread applies input
      // and WTD commands to the master screenContent block.
      if (this.MasterThread != null)
      {
        var kbInput = new KeyboardInputMessage()
        {
          KeyCode = key,
          ShiftDown = KeyboardExt.IsShiftDown( )
        };
        this.MasterThread.PostInputMessage(kbInput);
//        this.MasterThread.MasterQueue.Enqueue(kbInput);
      }
    }

    /// <summary>
    /// see PreviewKeyDown. After keypress is handled, call this method to signal
    /// the canvas changed event.
    /// </summary>
    private void SignalCanvasChanged( )
    {
      if (this.CanvasChanged != null)
      {
        this.CanvasChanged(this);
      }
    }
    public void PaintScreen(
      List<ShowItemBase> ShowItemList, OneRowCol CaretRowCol, bool Erase = true)
    {
      if (Erase == true)
        this.EraseScreen();

      this.ShowItemList = ShowItemList;

      PaintScreen_Actual(ShowItemList);
    }
    public OneRowCol PaintScreen(
      WriteToDisplayCommand WTD_command, bool Erase = true, TelnetLogList LogList = null)
    {
      if (Erase == true)
      {
        this.EraseScreen();
        if (LogList != null)
          LogList.AddItem(Direction.Read, "PaintScreen. EraseScreen.");
      }

      this.ShowItemList = null;

      var caret = PaintScreen_Actual(WTD_command, LogList);

      return caret;
    }

    /// <summary>
    /// create visual items from the show items and place those visual items on the
    /// canvase.
    /// </summary>
    /// <param name="ShowItemList"></param>
    private void PaintScreen_Actual(List<ShowItemBase> ShowItemList)
    {
      foreach (IVisualItem iShowItem in ShowItemList)
      {
        // a literal value. look first for any fields on the screen which the 
        // literal is applied to.
        if (iShowItem is ShowLiteralItem)
        {
          var findNode = VisualItems.FindFieldItem(iShowItem);
          if (findNode != null)
          {
            // replace textbock segment with the spanner.
            var visualItem = findNode.Value as IVisualItem;
            if (visualItem is VisualTextBlockSegment)
            {
              var seg = visualItem as VisualTextBlockSegment;
              visualItem = seg.Parent;
            }

            var vim = visualItem as IVisualItemMore;

            // pos within the canvas item at which to place literal text.
            int bx = iShowItem.ShowRowCol( ).ColNum - visualItem.ShowRowCol().ColNum;

            // apply the literal text.
            vim.ApplyText(iShowItem.ShowText, bx);

            visualItem.TailAttrByte = iShowItem.TailAttrByte;
            continue;
          }
        }

        {
          var visualItem = VisualItemFactory(iShowItem);
          visualItem.CreateFromItem = (ShowItemBase)iShowItem;

          var iMore = visualItem as IVisualItemMore;
          var node = iMore.InsertIntoVisualItemsList(this.VisualItems);

          // var node = InsertIntoVisualItemsList(visualItem as IVisualItem);
          (visualItem as IVisualItemMore).AddToCanvas(this);

          if (iShowItem is ShowFieldItem)
          {
            var fieldItem = iShowItem as ShowFieldItem;
            iMore.SetupFieldItem(
              fieldItem, this.CanvasDefn.CharBoxDim, this.CanvasDefn.KernDim);
            iMore.CreateFromItem = fieldItem;
          }
        }
      }
    }

    /// <summary>
    /// create visual items from the show items and place those visual items on the
    /// canvase.
    /// </summary>
    /// <param name="ShowItemList"></param>
    private OneRowCol PaintScreen_Actual(
      WriteToDisplayCommand WTD_command, TelnetLogList LogList = null)
    {
      IRowCol curRowCol = new ZeroRowCol(0, 0);
      OneRowCol caret = null;
      foreach( var order in WTD_command.OrderList )
      {
        if ( LogList != null)
        {
          LogList.AddItem(Direction.Read, "PaintScreen. " + order.ToString());
        }

        if (order is TextDataOrder)
        {
          var tdo = order as TextDataOrder;
          if (tdo.IsEmpty() == false)
          {
            PaintScreen_ApplyTextDataOrder(tdo, curRowCol);
          }
          curRowCol = tdo.Advance(curRowCol);
        }

        else if (order is SetBufferAddressOrder)
        {
          var sba = order as SetBufferAddressOrder;
          curRowCol = sba.GetRowCol(this.ScreenDim).ToZeroRowCol();
        }

        else if (order.OrderCode == WtdOrder.InsertCursor)
        {
          var ico = order as InsertCursorOrder;
          caret = ico.RowCol;
        }

        else if (order is StartFieldOrder)
        {
          var sfo = order as StartFieldOrder;
          {
            var showText = new String(' ', sfo.LL_Length);
            var vtb = new VisualTextBlock(
              showText, (ZeroRowCol)curRowCol, sfo.AttrByte,
              this.CanvasDefn.CharBoxDim, this.CanvasDefn.KernDim,
              this.CanvasDefn.FontDefn);
            var iMore = vtb as IVisualItemMore;
            var node = iMore.InsertIntoVisualItemsList(this.VisualItems);
            iMore.AddToCanvas(this);

            vtb.SetupFieldItem(
              sfo, this.CanvasDefn.CharBoxDim, this.CanvasDefn.KernDim);
          }
          curRowCol = curRowCol.Advance(1);
        }

        else if ( order is RepeatToAddressOrder)
        {
          var rao = order as RepeatToAddressOrder;
          this.VisualItems.ApplyRepeatToAddressOrder(curRowCol, rao, this);
          var lx = rao.RepeatLength((ZeroRowCol) curRowCol);
          curRowCol = curRowCol.Advance(lx);
        }

        else if (order is EraseToAddressOrder)
        {
          var eao = order as EraseToAddressOrder;
          this.VisualItems.ApplyEraseToAddressOrder(curRowCol, eao, this);
          var lx = eao.EraseLength((ZeroRowCol)curRowCol);
          curRowCol = curRowCol.Advance(lx);
        }
      }
      return caret;
    }

    private void PaintScreen_ApplyTextDataOrder(TextDataOrder tdo, IRowCol curRowCol)
    {
      var showText = tdo.ShowText;

      var showRowCol = tdo.ShowRowCol(curRowCol);
      var itemEndRowCol = tdo.ItemEndRowCol(showRowCol);
      var itemRange = new RowColRange(curRowCol, itemEndRowCol);

      IVisualItem ivi = null;

      // range if visual items which overlap this tdo.
      var overlapItems = VisualItems.GetOverlapItems(itemRange);
      if ( overlapItems != null)
      {
        foreach( var cursor in overlapItems)
        {
          var item = cursor.GetVisualItem();
          var itemRowCol = item.ItemRowCol;
          var tailAttrByte = tdo.TailAttrByte;
          if ((tdo.TailAttrByte != null)
            && (item.AttrByte != null)
            && (tdo.ItemEndRowCol(curRowCol).CompareTo(item.ItemRowCol) == 0))
          {
             int xx = 4;
          }
        }

      }
      else
      {

      }

      if (itemEndRowCol != null)
        ivi = VisualItems.FindFieldItem(curRowCol, showRowCol, itemEndRowCol);
      if (ivi != null)
      {
        // replace textbock segment with the spanner.
        if (ivi is VisualTextBlockSegment)
        {
          var seg = ivi as VisualTextBlockSegment;
          ivi = seg.Parent;
        }

        var vim = ivi as IVisualItemMore;

        // pos within the canvas item at which to place literal text.
        int bx = showRowCol.ColNum - ivi.ShowRowCol().ColNum;

        // apply the literal text.
        vim.ApplyText(showText, bx);
        if (tdo.TailAttrByte != null)
          ivi.TailAttrByte = tdo.TailAttrByte;
      }
      else if (itemEndRowCol != null)
      {
        var visualItem = VisualItemFactory(
          tdo.ShowText, (ZeroRowCol)curRowCol, tdo.AttrByte,
          tdo.TailAttrByte);
        visualItem.FromOrder = tdo;

        var iMore = visualItem as IVisualItemMore;
        var node = iMore.InsertIntoVisualItemsList(this.VisualItems);
        iMore.AddToCanvas(this);
      }
    }

    public void PositionCaret(OneRowCol CaretRowCol)
    {
      // position the caret cursor.
      VisualItemCursor ic = null;
      if (CaretRowCol != null)
      {
        var rowCol = CaretRowCol.ToZeroRowCol();
        this.CaretCursor = VisualItems.FindVisualItemCanvas(rowCol);
      }
      else
      {
        // find the first field on the screen.
        ic = VisualItems.InputItemList().FirstOrDefault();

        this.CaretCursor = new CanvasPositionCursor(ic);
      }

      // by default. place the caret at first input field on screen.
      if (this.CaretCursor.RowCol == null)
      {
        var rowCol = new ZeroRowCol(0, 0);
        this.CaretCursor = VisualItems.FindVisualItemCanvas(rowCol);
      }

      // position the caret cursor at the visual item.
      this.CanvasCaret.StartBlink(this.CaretCursor, true);
    }

    public void RepaintScreen()
    {
      // first, remove from canvas.
      foreach (var cursor in VisualItems.ItemList())
      {
        var visualItem = cursor.GetVisualItem();
        var iMore = visualItem as IVisualItemMore;
        if (iMore != null)
        {
          iMore.RemoveFromCanvas(this);
        }
      }

      // loop adding the draw controls of the visual items back onto the canvas.
      foreach (var cursor in VisualItems.ItemList())
      {
        var visualItem = cursor.GetVisualItem();

        // set the font of the textBlock.
        if (visualItem is VisualTextBlock)
        {
          var iTextBlock = visualItem as VisualTextBlock;
          iTextBlock.SetPaintDefn(this.CanvasDefn.CharBoxDim, this.CanvasDefn.KernDim,
            this.CanvasDefn.FontDefn);

          if (iTextBlock.ShowItem != null)
          {
            iTextBlock.SetupFieldItem(
              iTextBlock.ShowItem, this.CanvasDefn.CharBoxDim, this.CanvasDefn.KernDim);
          }

          var iMore = visualItem as IVisualItemMore;
          iMore.AddToCanvas(this);
        }
      }

      this.CanvasCaret.RefreshCaret();
    }

    /// <summary>
    /// apply the text entered from the keyboard onto the visual item located by 
    /// the canvas cursor. Handle error where the canvas cursor item is not input
    /// capable.
    /// </summary>
    /// <param name="Cursor"></param>
    /// <param name="Text"></param>
    /// <param name="Reuse"></param>
    /// <returns></returns>
    public Tuple<VisualItem,string> PutKeyboardText(
      CanvasPositionCursor Cursor, string Text, bool Reuse = true)
    {
      string errmsg = null;
      VisualItem visualItem = null;
        if ( Cursor.IsInputItem( ) == true )
      {
        visualItem = Cursor.PutKeyboardText(Text);

        // send the keyboard input to the master thread. That thread applies input
        // and WTD commands to the master screenContent block.
        if ( this.MasterThread != null)
        {
          var kbInput = new KeyboardInputMessage()
          {
            RowCol = Cursor.RowCol as ZeroRowCol,
            Text = Text
          };
          this.MasterThread.PostInputMessage(kbInput);
        }
      }
      else
        errmsg = "not entry field";

      return new Tuple<VisualItem, string>(visualItem, errmsg);
    }

    public CanvasPositionCursor PutKeyboardText_AdvanceCaret(
      CanvasPositionCursor Cursor, string Text, bool Reuse = true )
    {
      CanvasPositionCursor cursor = Cursor;
      var rv = PutKeyboardText(cursor, Text, Reuse);
      var errmsg = rv.Item2;
      if ( errmsg == null)
      {
        cursor = cursor.AdvanceRight(this.VisualItems, HowAdvance.NextEntryField, Reuse);
      }
      return cursor;
    }

    /// <summary>
    /// create VisualTextBlock or VisualSpanner from string to display on the 
    /// canvase. If string spans multiple rows, create VisualSpanner which in turn
    /// contains multiple VisualTextBlock segments.
    /// </summary>
    /// <param name="ShowText"></param>
    /// <param name="ItemRowCol"></param>
    /// <param name="AttrByte"></param>
    /// <param name="TailAttrByte"></param>
    /// <returns></returns>
    public VisualItem VisualItemFactory(string ShowText, ZeroRowCol ItemRowCol, 
      byte? AttrByte, byte? TailAttrByte )
    {
      VisualItem visualItem = null;
      if (CanvasCommon.DoesSpanMultipleRows(
        ItemRowCol, AttrByte, ShowText.Length) == true)
      {
        visualItem = 
          new VisualSpanner( ShowText, ItemRowCol, AttrByte, this.CanvasDefn);
      }
      else
      {
        visualItem = new VisualTextBlock(
          ShowText, ItemRowCol, AttrByte,
          this.CanvasDefn.CharBoxDim, this.CanvasDefn.KernDim,
          this.CanvasDefn.FontDefn);
        visualItem.TailAttrByte = TailAttrByte;
      }
      return visualItem;
    }

    public VisualItem VisualItemFactory(ScreenContent ScreenContent, ContentField contentField)
    {
      VisualItem visualItem = null;
      if ( contentField is ContinuedContentField)
      {
        var contField = contentField as ContinuedContentField;
        if (contField.ContinuedFieldSegmentCode == ContinuedFieldSegmentCode.First)
        {
          visualItem = 
            new VisualSpanner(ScreenContent, contField, this.CanvasDefn);
        }
      }
      else
      {
      visualItem = this.VisualItemFactory(
        contentField.GetShowText(ScreenContent),
        contentField.RowCol, 
        contentField.GetAttrByte(ScreenContent),
        contentField.GetTailAttrByte(ScreenContent));
      }

      return visualItem;
    }

    private VisualItem VisualItemFactory(IVisualItem ShowItem)
    {
      VisualItem visualItem = null;
      if (ShowItem.DoesShowSpanMultipleRows() == true)
      {
        visualItem = new VisualSpanner(
          ShowItem.ShowText, ShowItem.ItemRowCol, ShowItem.AttrByte,
          this.CanvasDefn);
        visualItem.CreateFromItem = (ShowItemBase)ShowItem;
      }
      else
      {
        visualItem = new VisualTextBlock(
          ShowItem.ShowText, ShowItem.ItemRowCol, ShowItem.AttrByte,
          this.CanvasDefn.CharBoxDim, this.CanvasDefn.KernDim,
          this.CanvasDefn.FontDefn);
        visualItem.CreateFromItem = (ShowItemBase)ShowItem;
      }
      return visualItem;
    }

    public void ChangeFontSize(double? fontSize)
    {
      this.CanvasDefn.ChangeFontSize(fontSize.Value);
      this.RepaintScreen();
    }

    public event PropertyChangedEventHandler PropertyChanged;
    private void RaisePropertyChanged(string propName)
    {

      PropertyChangedEventHandler eh = PropertyChanged;

      if (eh != null)
      {
        eh(this, new PropertyChangedEventArgs(propName));
      }
    }

    /// <summary>
    /// unhook event handlers.  Dispose of timers.
    /// </summary>
    public void Dispose()
    {
      this.Canvas.PreviewKeyDown -= Item_PreviewKeyDown;
      this.Canvas.MouseLeftButtonDown -= Canvas_MouseLeftButtonDown;
      this.Canvas.MouseMove -= Canvas_MouseMove;

			if ( this.HoverTimer != null)
			{
				this.HoverTimer.Dispose();
			}

			this.HoverBox.Dispose();

      if (this.BorderControl != null)
      {
        this.BorderControl.MouseLeftButtonDown -= WindowBorder_MouseLeftButtonDown;
        this.BorderControl.MouseLeftButtonUp -= WindowBorder_MouseLeftButtonUp;
        this.BorderControl.MouseMove -= WindowBorder_MouseMove;
      }

      // clear the canvas of its children.
      this.Canvas.Children.Clear();

    }

    public void DisposeChildren( )
    {
      if (this.Children != null)
      {
        foreach (var child in this.Children)
        {
          child.Dispose();
        }

        this.Children = null;
      }
    }
  }
}
