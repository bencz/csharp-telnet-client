using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using TextCanvasLib.xml;
using AutoCoder.Ext.System;
using AutoCoder.Core.Enums;
using System.Windows;
using System.Windows.Input;
using TextCanvasLib.Enums;
using System.Windows.Shapes;
using System.Windows.Media;
using TextCanvasLib.Common;
using AutoCoder.Ext.Windows;
using TextCanvasLib.Main;

namespace TextDemo
{
  /// <summary>
  /// item canvas renders a list of ScreenVisualItems on the screen. Cursor is
  /// located in an item and advances from one item to the next. Keyboard entry
  /// is applied to the visual item where the cursor is located.
  /// </summary>
  public class TestCanvas : ScreenCanvasBase
  {
    public delegate void
    ExitScreenEventHandler(object o, EventArgs args);

    public event ExitScreenEventHandler ScreenExitEvent;

    List<ShowItemBase> ShowItemList { get; set; }

    public ItemCanvasCursor CaretCursor
    { get; set; }

    public TestCanvas(Canvas canvas, double charWidth, double charHeight,
      int ColWidth, int RowHeight)
      : base(canvas, charWidth, charHeight, ColWidth, RowHeight)
    {
      this.Canvas.PreviewKeyDown += Item_PreviewKeyDown;
      this.Canvas.MouseLeftButtonDown += Canvas_MouseLeftButtonDown;
    }

    private void Canvas_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
      var pos = e.GetPosition(this.Canvas);
      var loc = pos.CanvasPosToScreenLoc(this.CharDim);
      var canvasCursor = VisualItems.FindVisualItem(loc);
      if ( canvasCursor.VisualCursor != null )
      {
        this.CaretCursor = canvasCursor;
        this.CanvasCaret.StartBlink(this.CaretCursor);
        e.Handled = true;
      }
    }

    private void Item_PreviewKeyDown(object sender, KeyEventArgs e)
    {
      string keyText = TextCanvasLib.Ext.KeyExt.ToTextInput(e.Key);

      if (keyText != null)
      {
        this.CaretCursor = PutText_AdvanceCaret(this.CaretCursor, keyText);
        this.CanvasCaret.StartBlink(this.CaretCursor);
        e.Handled = true;
      }
      else if (e.Key == Key.Tab)
      {
        if ( Keyboard.IsKeyDown(Key.LeftShift) == false )
          this.CaretCursor = this.CaretCursor.NextInputItem();
        else
          this.CaretCursor = this.CaretCursor.PrevInputItem();

        var item = this.CaretCursor.GetVisualItem();
        CanvasCaret.SetLocation(item.StrLoc);
        e.Handled = true;
      }
      else if (e.Key == Key.Down)
      {
        e.Handled = true;
      }
      else if (e.Key == Key.Enter)
      {
        CanvasCaret.BlinkSymbol(this.CaretLoc, "_");
      }
      else if (e.Key == Key.Right)
      {
        this.CaretCursor.AdvanceRight(HowAdvance.NextColumn);
        this.CanvasCaret.StartBlink(this.CaretCursor);
        e.Handled = true;
      }
      else if (e.Key == Key.Left)
      {
        this.CaretCursor.AdvanceLeft(HowAdvance.NextColumn);
        this.CanvasCaret.StartBlink(this.CaretCursor);
        e.Handled = true;
      }
      else if (e.Key == Key.Up)
      {
        e.Handled = true;
      }
      else if (e.Key == Key.Back)
      {
        PutText(" ", this.CaretLoc);
        CanvasCaret.BlinkSymbol(this.CaretLoc, "_");
        e.Handled = true;
      }

      else if (e.Key == Key.Delete)
      {
        CanvasCaret.BlinkSymbol(this.CaretLoc, "_");
        e.Handled = true;
      }

      // f3-exit.  Signal the screen exit event.
      else if (e.Key == Key.F3)
      {
        if (this.ScreenExitEvent != null)
          this.ScreenExitEvent(this, new EventArgs());
      }
    }
    public override void PaintScreen(List<ShowItemBase> ShowItemList)
    {
      this.ShowItemList = ShowItemList;

      base.PaintScreen(ShowItemList);

      // find the first field on the screen.
      var cursor = VisualItems.FirstInputItem();
      if (cursor != null)
      {
        this.CaretCursor = new ItemCanvasCursor(cursor);
        var item = cursor.GetVisualItem();
        CanvasCaret.StartBlink(item, item.StrLoc, "_");
      }
    }

    public ItemCanvasCursor PutText_AdvanceCaret(
      ItemCanvasCursor Cursor, string Text, bool Reuse = true)
    {
      ItemCanvasCursor cursor = Cursor;
      VisualItem item = null;
      var vi = cursor.GetVisualItem();
      if (vi?.IsInputItem == true)
      {
        item = PutTextToVisualItem(cursor, Text, cursor.Position);
        cursor = cursor.AdvanceRight(HowAdvance.NextEntryField, Reuse);
      }
      return cursor;
    }

    private VisualItem PutTextToVisualItem(ItemCanvasCursor CanvasCursor, string Text, int Pos)
    {
      var item = CanvasCursor.GetVisualItem();
      var absPos = item.StrColNum + Pos - 1;
      var locText = new LocatedString(absPos, Text);
      item.ApplyText(locText);
      return item;
    }
  }
}
