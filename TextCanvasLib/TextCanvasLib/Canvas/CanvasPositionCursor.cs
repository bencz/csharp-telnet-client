using AutoCoder.Telnet.Common;
using AutoCoder.Telnet.Common.RowCol;
using AutoCoder.Telnet.IBM5250.Content;
using System.Linq;
using TextCanvasLib.Enums;
using TextCanvasLib.Visual;

namespace TextCanvasLib.Canvas
{
  /// <summary>
  /// stores the VisualItemCursor and the position within that item.
  /// When the position is outside a visualItem the class stores an OutsideRowCol
  /// location.
  /// </summary>
  public class CanvasPositionCursor
  {
    public VisualItemCursor ItemCursor
    { get; set; }

    /// <summary>
    /// character position within the visualItem.
    /// </summary>
    public int Position { get; set; }

    public IRowCol RowCol
    {
      get
      {
        if (this.ItemCursor == null)
          return this.OutsideRowCol;
        else
        {
          var ivi = ItemCursor.GetVisualItem();
          int rowNum = ivi.ShowRowCol().RowNum;
          int colNum = ivi.ShowRowCol( ).ColNum + this.Position - 1;
          var zeroRowCol = ivi.ShowRowCol().NewRowCol(rowNum, colNum);
//          var zeroRowCol = new ZeroRowCol(ivi.ShowRowCol( ).RowNum, colNum);
          return zeroRowCol;
        }
      }
    }

    /// <summary>
    /// location outside of a visual item on the canvas.
    /// </summary>
    public ZeroRowCol OutsideRowCol
    {
      get;
      set;
    }

    /// <summary>
    /// construct cursor from position within a visual item.
    /// </summary>
    /// <param name="ItemCursor"></param>
    /// <param name="Position"></param>
    public CanvasPositionCursor(VisualItemCursor VisualCursor, int Position = 1)
    {
      this.ItemCursor = VisualCursor;
      this.Position = Position;
    }

    /// <summary>
    /// construct from absolute row and column positions. Cursor is located outside of
    /// a VisualItem on the canvas.
    /// </summary>
    /// <param name="RowNum"></param>
    /// <param name="ColNum"></param>
    public CanvasPositionCursor( ZeroRowCol RowCol)
    {
      this.ItemCursor = null;
      this.Position = 0;
      this.OutsideRowCol = RowCol;
    }

    public CanvasPositionCursor( ScreenVisualItems VisualItems, ZeroRowCol RowCol)
    {
      var itemCursor = VisualItems.FindVisualItem(RowCol);
      if (itemCursor == null)
      {
        this.ItemCursor = null;
        this.Position = 0;
        this.OutsideRowCol = RowCol;
      }
      else
      {
        var item = itemCursor.GetVisualItem();
        var iItem = item as IVisualItem;
        int pos = RowCol.ColNum - iItem.ShowRowCol().ColNum + 1;
        this.ItemCursor = itemCursor;
        this.Position = pos;
      }
    }

    /// <summary>
    /// add the row/col adjustment factor to the canvas cursor.
    /// </summary>
    /// <param name="RowCol"></param>
    /// <returns></returns>
    public CanvasPositionCursor Add(IRowCol RowCol)
    {
      CanvasPositionCursor canvasCursor = null;
      if (RowCol == null)
        canvasCursor = this;
      else
      {
        var itemCursor = this.ItemCursor;
        var pos = this.Position;
        IRowCol outsideRowCol = null;
        if (this.OutsideRowCol != null)
        {
          outsideRowCol = this.OutsideRowCol.Add(RowCol);
        }
        if (itemCursor == null)
          canvasCursor = new CanvasPositionCursor(outsideRowCol as ZeroRowCol);
        else
          canvasCursor = new CanvasPositionCursor(itemCursor, pos);
      }
      return canvasCursor;
    }

    public CanvasPositionCursor AdvanceDown(ScreenVisualItems VisualItems )
    {
      var rowCol = this.RowCol.AdvanceDown( ) ;
      return new CanvasPositionCursor(VisualItems, rowCol as ZeroRowCol);
    }
    public CanvasPositionCursor AdvanceLeft( ScreenVisualItems VisualItems,
      HowAdvance HowAdvance = HowAdvance.NextEntryField,
      bool Reuse = true)
    {
      var cursor = this;

      if (cursor.IsOutsideVisualItem() == true)
      {
        var rowCol = cursor.RowCol.AdvanceLeft();
        cursor = new CanvasPositionCursor(VisualItems, rowCol as ZeroRowCol);
      }

      else
      {
        var vi = cursor.GetVisualItem();
        var pos = cursor.Position;

        // cursor located in entry field. and cursor is at the end of the input field. 
        if (((vi as VisualItem)?.IsInputItem == true) && (this.Position == 1)
          && (HowAdvance == HowAdvance.NextEntryField))
        {
          cursor = this.PrevInputItem(VisualItems, null, Reuse);
          // need to set cursor position to the last position in the visual item.
        }

        else
        {
          if (Reuse == true)
          {
            cursor.Position -= 1;
          }
          else
          {
            cursor = new CanvasPositionCursor(this.ItemCursor, this.Position - 1);
          }
        }
      }
      return cursor;
    }

    /// <summary>
    /// advance the cursor one column to the right. The advanced to location depends
    /// if the cursor is located in an input field, if the cursor is at the last
    /// column of the input field and if it is, whether the code should advance to the
    /// next input field or to the screen location to the right which is outside the
    /// input field.
    /// </summary>
    /// <param name="HowAdvance"></param>
    /// <param name="Reuse"></param>
    /// <returns></returns>
    public CanvasPositionCursor AdvanceRight(ScreenVisualItems VisualItems,
      HowAdvance HowAdvance = HowAdvance.NextEntryField,
      bool Reuse = true)
    {
      var cursor = this;

      if (cursor.IsOutsideVisualItem() == true)
      {
        var rowCol = cursor.RowCol.AdvanceRight();
        cursor = new CanvasPositionCursor(VisualItems, rowCol as ZeroRowCol);
      }

      else
      {
        var vi = cursor.GetVisualItem();
        var pos = cursor.Position;

        // cursor located in entry field. and cursor is at the end of the input field. 
        if (((vi as VisualItem)?.IsInputItem == true) 
          && ((vi as VisualItem).IsLastColumn(this.Position)))
        {
          if (HowAdvance == HowAdvance.NextEntryField)
            cursor = this.NextInputItemCircular(VisualItems, null, Reuse);
        }

        else
        {
          if (Reuse == true)
          {
            cursor.Position += 1;
          }
          else
          {
            cursor = new CanvasPositionCursor(this.ItemCursor, this.Position + 1);
          }
        }
      }

      return cursor;
    }


    public CanvasPositionCursor AdvanceUp(ScreenVisualItems VisualItems)
    {
      var rowCol = this.RowCol.AdvanceUp();
      return new CanvasPositionCursor(VisualItems, rowCol as ZeroRowCol);
    }

    /// <summary>
    /// return the VisualItem pointed to by the VisualCursor of this ItemCanvasCursor. 
    /// </summary>
    /// <returns></returns>
    public IVisualItem GetVisualItem( )
    {
      if (this.ItemCursor == null)
        return null;
      else
        return this.ItemCursor.GetVisualItem();
    }

    public bool IsInputItem()
    {
      var vi = this.GetVisualItem();
      if ((vi != null) && ((vi as VisualItem).IsInputItem == true))
        return true;
      else
        return false;
    }

    public bool IsOutsideVisualItem( )
    {
      if (this.ItemCursor == null)
        return true;
      else
        return false;
    }

    public VisualItem PutKeyboardText(string Text)
    {
      var ivi = this.GetVisualItem() as IVisualItem;
      var vim = ivi as IVisualItemMore;
      vim.ApplyText(Text, this.Position - 1);
      vim.ModifiedFlag = true;

      return (ivi as VisualItem);
    }

    public override string ToString()
    {
      if (this.RowCol == null)
        return "CanvasPositionCursor. null.";
      else
        return "CanvasPositionCursor. " + this.RowCol.ToString();
    }
  }

  public static class ItemCanvasCursorExt
  {
    public static CanvasPositionCursor NextInputItem(
      this CanvasPositionCursor Cursor, ScreenVisualItems VisualItems, 
      VisualFeature? Feature = null, bool Reuse = true)
    {
      VisualItemCursor vc = null;
      var cursor = Cursor;

      if ( cursor == null)
      {
        cursor = new CanvasPositionCursor(new ZeroRowCol(0, 0));
      }

      if (cursor.IsOutsideVisualItem() == true)
      {
        vc = VisualItems.FindNextInputItem(cursor.RowCol, Feature);
      }

      else
      {
        vc = cursor.ItemCursor.NextInputItem(Feature);
      }

      if (vc == null)
        cursor = null;
      else if (Reuse == true)
      {
        cursor.ItemCursor = vc;
        cursor.Position = 1;
      }
      else
      {
        cursor = new CanvasPositionCursor(vc, 1);
      }

      return cursor;
    }

    public static CanvasPositionCursor NextInputItemCircular(
      this CanvasPositionCursor Cursor, ScreenVisualItems VisualItems, 
      VisualFeature? Feature = null, bool Reuse = true)
    {
      var posCursor = Cursor.NextInputItem(VisualItems, Feature, Reuse);
      if (posCursor == null)
      {
        var ic = VisualItems.InputItemList(Feature).First();
        if (ic != null)
          posCursor = new CanvasPositionCursor(ic);
      }

      return posCursor;
    }

    public static CanvasPositionCursor PrevInputItem(
      this CanvasPositionCursor Cursor, ScreenVisualItems VisualItems,
      VisualFeature? Feature = null, bool Reuse = true)
    {
      VisualItemCursor vc = null;
      var cursor = Cursor;
      if (cursor.IsOutsideVisualItem() == true)
      {
        vc = VisualItems.FindPrevInputItem(cursor.RowCol, Feature);
      }
      else
      {
        vc = Cursor.ItemCursor.PrevInputItem( Feature );
      }

      if (vc == null)
        cursor = null;
      else if (Reuse == true)
      {
        cursor.ItemCursor = vc;
        cursor.Position = 1;
      }
      else
      {
        cursor = new CanvasPositionCursor(vc, 1);
      }

      return cursor;
    }
    public static CanvasPositionCursor NextInputItemNextLine(
      this CanvasPositionCursor PosCursor, ScreenVisualItems VisualItems)
    {
      var posCursor = PosCursor;

      // save the row/col of the current 
      var startRowCol = PosCursor.RowCol;

      while(true)
      {
        posCursor = posCursor.NextInputItemCircular(VisualItems);
        if (startRowCol.RowNum != posCursor.RowCol.RowNum)
          break;
        if (posCursor.RowCol.CompareTo(startRowCol) != 1)
          break;
      }

      return posCursor;
    }

    public static CanvasPositionCursor PrevInputItemCircular(
      this CanvasPositionCursor Cursor, ScreenVisualItems VisualItems, 
      VisualFeature? Feature = null, bool Reuse = true)
    {
      var posCursor = Cursor.PrevInputItem(VisualItems, Feature, Reuse);
      if (posCursor == null)
      {
        var ic = VisualItems.InputItemList().Last();
        if (ic != null)
          posCursor = new CanvasPositionCursor(ic);
      }

      return posCursor;
    }
  }
}
