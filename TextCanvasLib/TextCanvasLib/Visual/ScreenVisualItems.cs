using System.Collections.Generic;
using System.Linq;
using TextCanvasLib.Canvas;
using AutoCoder.Systm;
using AutoCoder.Telnet.IBM5250.WtdOrders;
using TextCanvasLib.xml;
using AutoCoder.Telnet.Common;
using AutoCoder.Telnet.Common.RowCol;
using System;
using TextCanvasLib.Enums;

namespace TextCanvasLib.Visual
{
  /// <summary>
  /// collection of all the visual items on the screen. Where visual item is a literal
  /// or entry field. The visual items are organized in rows on the screen. Where a
  /// VisualRow contains all the VisualItem items in that row.  So to find a
  /// VisualItem on the screen by its row and column position, first find the
  /// VisualRow, then seach that row for the item at the column position.
  /// </summary>
  public class ScreenVisualItems
  {
    public LinkedList<VisualRow> Rows
    { get; set; }

    public ScreenVisualItems()
    {
      this.Rows = new LinkedList<VisualRow>();
    }

    /// <summary>
    /// build a byte stream containing the WTD workstation command orders from all
    /// of the visual items on the canvas. The visual item list is the equivalent of
    /// the 5250 format table.
    /// </summary>
    /// <param name="VisualItems"></param>
    /// <returns></returns>
    public byte[] BuildOrderStream(CanvasPositionCursor Caret)
    {
      ByteArrayBuilder ba = new ByteArrayBuilder();

      // start header order.
      {
        byte[] cmdKeySwitches = new byte[] { 0x00, 0x00, 0x00 };
        var buf = StartHeaderOrder.Build(0x00, 0x00, 24, cmdKeySwitches);
        ba.Append(buf);
      }

      // insert cursor order.
      {
        var zeroRowCol = Caret.RowCol as ZeroRowCol;
        var rowCol = zeroRowCol.ToOneRowCol() as OneRowCol;
        var buf = InsertCursorOrder.Build(rowCol);
        ba.Append(buf);
      }

      // build sets of SBA, StartField and TextData orders for each visual item. The
      // visual item represents something on the screen. Whether output literal or
      // and input field.
      // VisualItem visualItem = null;
      IVisualItem iVisual = null;
      var cursor = this.FirstVisualItem();
      while (cursor != null)
      {
        var pvVisualItem = iVisual;
        iVisual = cursor.GetVisualItem();

        if (iVisual is VisualSpanner)
        {
        }
        else
        {
          {
            var buf = SetBufferAddressOrder.Build(
              iVisual.ItemRowCol.ToOneRowCol() as OneRowCol);
            ba.Append(buf);
          }

          var visualItem = iVisual as VisualItem;
          if (iVisual.IsField == true)
          {
            var vtb = iVisual as VisualTextBlock;
            var ffw = vtb.FFW_Bytes;

            byte attrByte = iVisual.AttrByte.Value;
            int lgth = iVisual.ShowText.Length;
            var buf = StartFieldOrder.Build(ffw[0], ffw[1], attrByte, lgth);
            ba.Append(buf);
          }

          // create a text data order from either the text of the literal item. Or the
          // text value of the entry field.
          {
            byte[] buf = null;
            var s1 = iVisual.ShowText;
            if (iVisual.IsField == true)
            {
              buf = TextDataOrder.Build(s1, null, iVisual.TailAttrByte);
            }
            else if (visualItem.CreateFromItem != null)
            {
              var litItem = visualItem.CreateFromItem as ShowLiteralItem;

              if (litItem.rao_RepeatTextByte != null)
              {
                var toRowCol = (iVisual.ItemEndRowCol() as ZeroRowCol).ToOneRowCol();
                buf = RepeatToAddressOrder.Build(
                  litItem.rao_RepeatTextByte.Value, toRowCol as OneRowCol);
              }

              else
              {
                var attrByte = litItem.AttrByte;
                if (s1.Length < litItem.tdo_Length)
                  s1 = s1.PadRight(litItem.tdo_Length);
                buf = TextDataOrder.Build(s1, attrByte, iVisual.TailAttrByte);
              }
            }
            else
            {
              buf = TextDataOrder.Build(s1, iVisual.AttrByte, iVisual.TailAttrByte);
            }
            ba.Append(buf);
          }
        }
        cursor = cursor.NextItem(true);
      }

      return ba.ToByteArray(); ;
    }

    /// <summary>
    /// iterate each of the items of the visual items list which are placed on the 
    /// canvas.
    /// </summary>
    /// <returns></returns>
    public IEnumerable<VisualItemCursor> CanvasItemList()
    {
      var cursor = FirstVisualItem();
      while (cursor != null)
      {
        var visualItem = cursor.GetVisualItem();

        if ( visualItem.IsOnCanvas == true )
        {
          yield return cursor;
        }
        cursor = cursor.NextItem(true);
      }
      yield break;
    }

    /// <summary>
    /// return an IEnumerable containing cursor to each of the visual items on the
    /// screen. 
    /// </summary>
    /// <returns></returns>
    public IEnumerable<VisualItemCursor> VisualItemList( )
    {
      var cursor = FirstVisualItem();
      while(cursor != null )
      {
        yield return cursor;
        cursor = cursor.NextItem(true);
      }
      yield break;
    }

    public IEnumerable<VisualItemCursor> InputItemList( VisualFeature? Feature = null)
    {
      var cursor = FirstInputItem(Feature);
      while (cursor != null)
      {
        var visualItem = cursor.GetVisualItem();

        if (visualItem.IsOnCanvas == true)
        {
          yield return cursor;
        }
        cursor = cursor.NextInputItem(Feature);
      }
      yield break;
    }

    /// <summary>
    /// clear the collection visual item rows.
    /// </summary>
    public void Clear( )
    {
      this.Rows.Clear();
    }

#if skip
    public LinkedListNode<IVisualItem> FindFieldItem(
      IRowCol ItemRowCol, IRowCol ShowRowCol, IRowCol ShowEndRowCol)
    {
      var row = this.GetVisualRow(ItemRowCol.RowNum);
      var node = row.FindFieldItem(ShowRowCol, ShowEndRowCol);
      return node;
    }

    public LinkedListNode<IVisualItem> FindFieldItem(IVisualItem Find)
    {
      var row = this.GetVisualRow(Find.ItemRowCol.RowNum);
      var node = row.FindFieldItem(Find);
      return node;
    }
#endif

    public IVisualItem FindFieldItem(
      IRowCol ItemRowCol, IRowCol ShowRowCol, IRowCol ShowEndRowCol)
    {
      var row = this.GetVisualRow(ItemRowCol.RowNum);
      var node = row.FindFieldItem(ShowRowCol, ShowEndRowCol);
      return node;
    }

    public LinkedListNode<IVisualItem> FindFieldItem(IVisualItem Find)
    {
      var row = this.GetVisualRow(Find.ItemRowCol.RowNum);
      var node = row.FindFieldItem(Find);
      return node;
    }

    /// <summary>
    /// find the input item on the canvas that follows the row/col position.
    /// </summary>
    /// <param name="rowCol"></param>
    /// <returns></returns>
    public VisualItemCursor FindNextInputItem(IRowCol rowCol, VisualFeature? Feature = null)
    {
      VisualItemCursor findCursor = null;
      foreach( var cursor in this.InputItemList(Feature))
      {
        var iItem = cursor.GetVisualItem();
        var showRowCol = iItem.ShowRowCol();
        if (showRowCol.CompareTo(rowCol) == 1)
        {
          findCursor = new VisualItemCursor(cursor);
          break;
        }
      }

      if (findCursor == null)
        findCursor = this.InputItemList(Feature).First();

      return findCursor;
    }

    /// <summary>
    /// find the input item on the canvas that preceeds the row/col position.
    /// </summary>
    /// <param name="rowCol"></param>
    /// <returns></returns>
    public VisualItemCursor FindPrevInputItem(
      IRowCol rowCol, VisualFeature? Feature = null)
    {
      VisualItemCursor findCursor = null;
      foreach (var cursor in this.InputItemList(Feature))
      {
        var iItem = cursor.GetVisualItem();
        var showRowCol = iItem.ShowRowCol();
        if (showRowCol.CompareTo(rowCol) == -1)
        {
          findCursor = new VisualItemCursor(cursor);
        }
        else
        {
          break;
        }
      }

      if (findCursor == null)
        findCursor = this.InputItemList(Feature).Last();

      return findCursor;
    }

    public VisualItemCursor FindVisualItem(IRowCol FindRowCol)
    {
      LinkedListNode<IVisualItem> colNode = null;
      LinkedListNode<IVisualItem> foundNode = null;

      // find the row within list of rows that contain items.
      var rowNode = Rows.First;
      while( rowNode != null )
      {
        if (rowNode.Value.RowNum == FindRowCol.RowNum)
          break;
        rowNode = rowNode.Next;
      }

      // found the row.  now search for the item in the items of the row.
      if ( rowNode != null )
      {
        colNode = rowNode.Value.ItemList.First;
        while( colNode != null )
        {
          if ((colNode.Value as IVisualItem).WithinColumnBounds(FindRowCol.ColNum))
          {
            foundNode = colNode;
            break;
          }
          colNode = colNode.Next;
        }
      }

      VisualItemCursor cursor = null;
      if (foundNode != null)
      {
        cursor = new VisualItemCursor()
        {
          ColumnNode = foundNode,
          RowNode = rowNode
        };
      }
      return cursor;
    }

    public CanvasPositionCursor FindVisualItemCanvas(IRowCol RowCol)
    {
      CanvasPositionCursor posCursor = null;
      VisualItemCursor foundCursor = null;
      int foundPos = 0;
      foreach ( var ic in this.InputItemList())
      {
        var ivi = ic.GetVisualItem();
        if (ivi.ContainsLocation(RowCol))
        {
          foundCursor = ic;
          foundPos = RowCol.ColNum - ivi.ShowRowCol().ColNum + 1;
          break;
        }
      }

      if (foundCursor == null)
        posCursor = new CanvasPositionCursor(RowCol as ZeroRowCol);
      else
        posCursor = new CanvasPositionCursor(foundCursor, foundPos);

      return posCursor;
    }

    public VisualItemCursor FirstVisualItem()
    {
      VisualItemCursor cursor = null;
      LinkedListNode<IVisualItem> colNode = null;

      var rowNode = this.Rows.First;
      if (rowNode != null)
        colNode = rowNode.Value.ItemList.First;

      if (rowNode != null)
      {
        cursor = new VisualItemCursor()
        {
          ColumnNode = colNode,
          RowNode = rowNode
        };
      }

      return cursor;
    }

    /// <summary>
    /// find the first entry field in the list of visual items.
    /// </summary>
    /// <returns></returns>
    public VisualItemCursor FirstInputItem( VisualFeature? Feature = null )
    {
      VisualItemCursor cursor = null;
      foreach( var ic in  VisualItemList( ))
      {
        bool gotItem = false;
        if ( ic.IsInputItem( ) == true )
        {
          gotItem = true;
        }

        if (( gotItem == true ) 
          && ( Feature != null) && (Feature.Value == VisualFeature.tabTo)
          && ( ic.GetVisualItem( ).IsTabToItem == false ))
        {
          gotItem = false;
        }

        if ( gotItem == true )
        {
          cursor = ic;
          break;
        }
      }
      return cursor;
    }

    /// <summary>
    /// return list of visual items which lie fully or partially within the input
    /// range.
    /// </summary>
    /// <param name="Range"></param>
    /// <returns></returns>
    public IEnumerable<VisualItemCursor> GetOverlapItems(RowColRange Range)
    {
      List<VisualItemCursor> overlapList = null;

      // build the list of all the items that are overlapped by the repeat range.
      foreach (var cursor in ItemList())
      {
        var item = cursor.GetVisualItem();
        var itemRange = item.ItemRange();
        if (Range.ContainsAny(itemRange) == true)
        {
          if ( overlapList == null)
            overlapList = new List<VisualItemCursor>();
          overlapList.Add(new VisualItemCursor(cursor));
        }
      }
      return overlapList;
    }

    public VisualRow GetVisualRow(int RowNum)
    {
      VisualRow getRow = null;
      foreach (var row in Rows)
      {
        if (row.RowNum == RowNum)
        {
          getRow = row;
          break;
        }
      }

      if (getRow == null)
      {
        getRow = new VisualRow(RowNum);
        this.Rows.AddLast(getRow);
      }

      return getRow;
    }

    public LinkedListNode<IVisualItem> InsertIntoVisualItemsList( IVisualItem visualItem )
    {
      var showRowCol = visualItem.ShowRowCol();
      var row = this.GetVisualRow(showRowCol.RowNum);

      // find the visual item on the row which the colnum of this new item is 
      // closest to.
      var rv = row.FindInsertLoc(showRowCol.ColNum);
      var baseNode = rv.Item1;
      var rltv = rv.Item2;

      var node = row.InsertItem(visualItem, baseNode, rltv);

      return node;
    }

    /// <summary>
    /// iterate each of the items of the visual items list.
    /// </summary>
    /// <returns></returns>
    public IEnumerable<VisualItemCursor> ItemList()
    {
      var cursor = FirstVisualItem();
      while (cursor != null)
      {
        if ( cursor.ColumnNode != null )
          yield return cursor;
        cursor = cursor.NextItem(true);
      }
      yield break;
    }


    /// <summary>
    /// remove the visualItem from the list of visual items and also from the
    /// ItemCanvas.
    /// </summary>
    /// <param name="itemCursor"></param>
    /// <param name="ItemCanvas"></param>
    public void RemoveItem( VisualItemCursor itemCursor, ItemCanvas ItemCanvas )
    {
      var ivm = itemCursor.GetVisualItem( ) as IVisualItemMore;
      if (ivm.IsOnCanvas == true)
        ivm.RemoveFromCanvas(ItemCanvas);
      itemCursor.RemoveColumnNode();
    }

    public void ApplyRepeatToAddressOrder(
      IRowCol curRowCol, RepeatToAddressOrder rao, ItemCanvas Canvas)
    {
      var raoRange = new RowColRange(curRowCol, rao.RowCol);
      var deleteList = new List<VisualItemCursor>();

      // build the list of all the items that are overlapped by the repeat range.
      foreach ( var cursor in ItemList( ))
      {
        var item = cursor.GetVisualItem();
        var itemRange = item.ItemRange();
        if ( raoRange.CompletelyContains(itemRange) == true )
        {
          deleteList.Add(new VisualItemCursor(cursor));
        }
      }

      // delete all the visual items which are overlapped by the RA order.
      foreach( var cursor in deleteList )
      {
        var item = cursor.GetVisualItem();
        var ivm = item as IVisualItemMore;
        if ((ivm != null ) && (item.IsOnCanvas == true ))
        {
          ivm.RemoveFromCanvas(Canvas);
          cursor.RemoveColumnNode();
        }
      }
    }

    public void ApplyEraseToAddressOrder(
      IRowCol curRowCol, EraseToAddressOrder eao, ItemCanvas Canvas)
    {
      var eaoRange = new RowColRange(curRowCol, eao.RowCol);
      var deleteList = new List<VisualItemCursor>();

      // build the list of all the items that are overlapped by the repeat range.
      foreach (var cursor in ItemList())
      {
        var item = cursor.GetVisualItem();
        var itemRange = item.ItemRange();
        if (eaoRange.CompletelyContains(itemRange) == true)
        {
          deleteList.Add(new VisualItemCursor(cursor));
        }
      }

      // delete all the visual items which are overlapped by the RA order.
      foreach (var cursor in deleteList)
      {
        var item = cursor.GetVisualItem();
        var ivm = item as IVisualItemMore;
        if ((ivm != null) && (item.IsOnCanvas == true))
        {
          ivm.RemoveFromCanvas(Canvas);
          cursor.RemoveColumnNode();
        }
      }
    }

  }
}
