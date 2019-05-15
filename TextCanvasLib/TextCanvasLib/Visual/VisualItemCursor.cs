using AutoCoder.Ext.System.Collections.Generic;
using AutoCoder.Telnet.Enums;
using System.Collections.Generic;
using TextCanvasLib.Enums;

namespace TextCanvasLib.Visual
{
  /// <summary>
  /// cursor that locates the visualItem within the linked list of rows and then
  /// linked list of visual items in the row.
  /// </summary>
  public class VisualItemCursor
  {
    public LinkedListNode<IVisualItem> ColumnNode { get; set; }

    public LinkedListNode<VisualRow> RowNode { get; set; }

    public VisualItemCursor( )
    {
    }

    public VisualItemCursor( VisualItemCursor Cursor )
    {
      this.ColumnNode = Cursor.ColumnNode;
      this.RowNode = Cursor.RowNode;
    }

    public IVisualItem GetVisualItem( )
    {
      if (this.ColumnNode == null)
        return null;
      else
        return this.ColumnNode.Value as IVisualItem;
    }

    /// <summary>
    /// item is first in the row and in the first row of the screen.
    /// </summary>
    /// <returns></returns>
    public bool IsFirstItem( )
    {
      if ((this.RowNode.Previous == null) 
        && (this.ColumnNode.Previous == null))
        return true;
      else
        return false;
    }

    /// <summary>
    /// the cursor points to the VisualItem of an input field.
    /// </summary>
    /// <returns></returns>
    public bool IsInputItem( )
    {
      if ((ColumnNode == null) || (ColumnNode.Value == null))
        return false;
      var item = ColumnNode.Value as VisualItem;
      if (item is VisualSpanner)
        return false;
      else if (item.Usage.IsInput())
        return true;
      else
        return false;
    }

    public bool IsSameItem(VisualItemCursor Cursor)
    {
      if ((this.RowNode == Cursor.RowNode)
        && (this.ColumnNode == Cursor.ColumnNode))
        return true;
      else
        return false;
    }

    public void RemoveColumnNode( )
    {
      var list = this.ColumnNode.List;
      list.Remove(this.ColumnNode);
    }
  }

  public static class VisualItemCursorExt
  {
    /// <summary>
    /// advance to the next item in collection of items on the canvas. 
    /// Use the Reuse flag to reuse the cursor. Update the cursor to the 
    /// next item and return it.
    /// </summary>
    /// <param name="Cursor"></param>
    /// <param name="Reuse"></param>
    /// <returns></returns>
    public static VisualItemCursor NextItem(
      this VisualItemCursor Cursor, bool Reuse = false)
    {
      VisualItemCursor cursor = null;
      LinkedListNode<IVisualItem> colNode = Cursor.ColumnNode;
      LinkedListNode<VisualRow> rowNode = Cursor.RowNode;

      // next item in the column.
      if (colNode != null)
      {
        var next = colNode.Next;
        colNode = next;
      }

      // no more column items. get next row.
      while (colNode == null)
      {
        rowNode = rowNode.Next;
        if (rowNode != null)
          colNode = rowNode.Value.ItemList.First;
        else
        {
          colNode = null;
          break;
        }
      }

      if (rowNode == null)
        cursor = null;
      else if (Reuse == true)
      {
        cursor = Cursor;
        cursor.ColumnNode = colNode;
        cursor.RowNode = rowNode;
      }
      else
      {
        cursor = new VisualItemCursor()
        {
          ColumnNode = colNode,
          RowNode = rowNode
        };
      }

      return cursor;
    }

    public static VisualItemCursor NextItemCircular(
      this VisualItemCursor Cursor, bool Reuse = false )
    {
      VisualItemCursor cursor = null;
      LinkedListNode<IVisualItem> colNode = Cursor.ColumnNode;
      LinkedListNode<VisualRow> rowNode = Cursor.RowNode;

      // next item in the column.
      if (colNode != null)
      {
        var next = colNode.Next;
        colNode = next;
      }

      // no more column items. get next row.
      if (colNode == null)
      {
        rowNode = rowNode.NextCircular();
        colNode = rowNode.Value.ItemList.First;
      }

      if ( Reuse == true )
      {
        cursor = Cursor;
        cursor.ColumnNode = colNode;
        cursor.RowNode = rowNode;
      }
      else
      {
        cursor = new VisualItemCursor()
        {
          ColumnNode = colNode,
          RowNode = rowNode
        };
      }

      return cursor;
    }

    public static VisualItemCursor NextInputItem(
      this VisualItemCursor Cursor, VisualFeature? Feature = null)
    {
      var cursor = Cursor;
      bool gotItem = false;
      while (gotItem == false)
      {
        cursor = cursor.NextItem(true);
        if (cursor == null)
          break;
        if (cursor.IsInputItem() == true)
        {
          gotItem = true;
        }

        if ((gotItem == true) 
          && (Feature != null) && (Feature.Value == VisualFeature.tabTo)
          && (cursor.GetVisualItem().IsTabToItem == false))
        {
          gotItem = false;
        }
      }
      return cursor;
    }

    public static VisualItemCursor PrevItem(this VisualItemCursor Cursor, bool Reuse = false)
    {
      VisualItemCursor cursor = null;
      LinkedListNode<IVisualItem> colNode = Cursor.ColumnNode;
      LinkedListNode<VisualRow> rowNode = Cursor.RowNode;

      // next item in the column.
      if (colNode != null)
      {
        var prev = colNode.Previous;
        colNode = prev;
      }

      // no more column items. get next row.
      if (colNode == null)
      {
        rowNode = rowNode.PrevCircular();
        colNode = rowNode.Value.ItemList.Last;
      }

      if (Reuse == true)
      {
        cursor = Cursor;
        cursor.ColumnNode = colNode;
        cursor.RowNode = rowNode;
      }
      else
      {
        cursor = new VisualItemCursor()
        {
          ColumnNode = colNode,
          RowNode = rowNode
        };
      }

      return cursor;
    }

    public static VisualItemCursor PrevInputItem(
      this VisualItemCursor Cursor, VisualFeature? Feature = null)
    {
      VisualItemCursor fs = null;
      bool gotItem = false;
      var cursor = Cursor;
      while (gotItem == false)
      {
        cursor = cursor.PrevItem(true);

        if (cursor.IsInputItem() == true)
        {
          gotItem = true;
        }

        if ((gotItem == true)
          && (Feature != null) && (Feature.Value == VisualFeature.tabTo)
          && (cursor.GetVisualItem().IsTabToItem == false))
        {
          gotItem = false;
        }

        if (gotItem == false)
        {
          // check that code has looped and no input item found.
          if (fs == null)
            fs = new VisualItemCursor(cursor);

          else if (cursor.IsSameItem(fs) == true)
          {
            cursor = null;
            break;
          }
        }
      }
      return cursor;
    }

  }
}
