using AutoCoder.Core.Enums;
using AutoCoder.Telnet.Common;
using AutoCoder.Telnet.Common.RowCol;
using System;
using System.Collections.Generic;
using System.Text;
using TextCanvasLib.Common;

namespace TextCanvasLib.Visual
{
  /// <summary>
  /// collection of VisualItems on the same row of the screen.
  /// </summary>
  public class VisualRow
  {
    public VisualItemLinkedList ItemList
    { get; set; }

    public int RowNum
    { get; set; }

    public VisualRow(int RowNum)
    {
      this.RowNum = RowNum;
      this.ItemList = new VisualItemLinkedList();
    }

    public void AddItem(IVisualItem AddItem)
    {
      var rv = FindInsertLoc(AddItem);
      InsertItem(AddItem, rv.Item1, rv.Item2);
    }

    public void DebugPrintItemList( )
    {

    }

    public Tuple<LinkedListNode<IVisualItem>, LocatedString> FindLoc(int ColNum)
    {
      LinkedListNode<IVisualItem> findNode = null;
      LocatedString findChar = null;

      // find the item to insert before.
      var node = ItemList.First;
      while (node != null)
      {
        if (ColNum < node.Value.ShowRowCol( ).ColNum)
        {
          findNode = null;
          break;
        }

        else if ((node.Value as IVisualItem).WithinColumnBounds(ColNum))
        {
          findNode = node;
          var s1 = node.Value.LocatedText( );
          findChar = s1.GetChar(ColNum);
          break;
        }

        node = node.Next;
      }

      return new Tuple<LinkedListNode<IVisualItem>, LocatedString>(findNode, findChar);
    }

    public Tuple<LinkedListNode<IVisualItem>, RelativePosition> FindShowInsertLoc(
      LocatedString Text)
    {
      LinkedListNode<IVisualItem> baseNode = null;
      RelativePosition rltv = RelativePosition.End;
      LinkedListNode<IVisualItem> atNode = null;

      // find the item to insert before.
      var node = ItemList.FirstShowItem();
      while (node != null)
      {
        // insert item starts before current node. Insert before since the next item
        // in the list will be even further away.
        if ( Text.BeginPos < node.Value.ShowRowCol( ).ColNum)
        {
          baseNode = node;
          rltv = RelativePosition.Before;
          break;
        }

        // insert item is entirely within this current item. Save the insert at node.
        else if ((Text.BeginPos >= node.Value.ShowRowCol( ).ColNum)
          && (Text.EndPos <= node.Value.ShowEndRowCol( ).ColNum))
        {
          atNode = node;
        }

#if skip
        else if (Text.BeginPos == node.Value.StrColNum)
        {
          baseNode = node;
          rltv = RelativePosition.After;
          break;
        }
        else if (Text.EndPos < node.Value.StrColNum)
        {
          baseNode = node;
          rltv = RelativePosition.Before;
          break;
        }
        else if ((Text.BeginPos >= node.Value.StrColNum)
          && (Text.BeginPos <= node.Value.EndColNum))
        {
          baseNode = node;
          rltv = RelativePosition.At;
          break;
        }

        // text to insert is immediately after the current node.
        else if (Text.BeginPos == node.Value.ImmedAfterColNum)
        {
          baseNode = node;
          rltv = RelativePosition.After;
          break;
        }
#endif

        node = node.NextShowItem();
      }

      // replace the base node with place at node.
      if (atNode != null)
      {
        baseNode = atNode;
        rltv = RelativePosition.At;
      }

      // found the base node and insert relative.
      else if (baseNode != null )
      { }

      // after the last item in the list.
      else
        rltv = RelativePosition.End;

      return new Tuple<LinkedListNode<IVisualItem>, RelativePosition>(baseNode, rltv);
    }

    public Tuple<LinkedListNode<IVisualItem>, RelativePosition> FindInsertLoc(
      int ColNum )
    {
      LinkedListNode<IVisualItem> baseNode = null;
      RelativePosition rltv = RelativePosition.End;

      foreach (var node in ItemList.Nodes)
      {
        {
          // insert item starts before current node. Insert before since the next item
          // in the list will be even further away.
          if (ColNum < node.Value.ShowRowCol( ).ColNum)
          {
            baseNode = node;
            rltv = RelativePosition.Before;
            break;
          }
        }
      }

      return new Tuple<LinkedListNode<IVisualItem>, RelativePosition>(baseNode, rltv);
    }

    public Tuple<LinkedListNode<IVisualItem>, RelativePosition> FindInsertLoc(
      IVisualItem AddItem)
    {
      LinkedListNode<IVisualItem> baseNode = null;
      RelativePosition rltv = RelativePosition.End;

      // find the item to insert before.
      var node = ItemList.First;
      while (node != null)
      {
        if (AddItem.ShowEndRowCol( ).ColNum < node.Value.ShowRowCol( ).ColNum)
        {
          baseNode = node;
          rltv = RelativePosition.Before;
          break;
        }
        else if (AddItem.ShowRowCol( ).ColNum <= node.Value.ShowEndRowCol( ).ColNum)
        {
          baseNode = node;
          rltv = RelativePosition.At;
          break;
        }
        node = node.Next;
      }

      return new Tuple<LinkedListNode<IVisualItem>, RelativePosition>(baseNode, rltv);
    }

    public IVisualItem FindItem(int ColNum)
    {
      IVisualItem foundItem = null;
      foreach (var item in ItemList)
      {
        if ((item.ShowRowCol( ).ColNum <= ColNum) && (item.ShowEndRowCol( ).ColNum >= ColNum))
        {
          foundItem = item;
          break;
        }
      }
      return foundItem;
    }
    public IVisualItem FindFieldItem(IRowCol ShowRowCol, IRowCol ShowEndRowCol)
    {
      IVisualItem foundItem = null;
      foreach( var item in ItemList.FieldItems)
      {
        if ( item.ShowRowCol( ).CompareTo( ShowRowCol) >= 0)
        {
          if ( item.ShowEndRowCol( ).CompareTo( ShowEndRowCol ) <= 0)
          {
            foundItem = item;
          }
        }
      }
      return foundItem;
    }

    public LinkedListNode<IVisualItem> FindFieldItem(IVisualItem Find)
    {
      var node = ItemList.FirstShowItem();
      while (node != null)
      {
        var item = node.Value;
        var visualItem = item as VisualItem;
        if (visualItem.ShowItem != null)
        {
          var showItem = visualItem.ShowItem;
          if (Find.ShowRowCol( ).ColNum >= item.ShowRowCol( ).ColNum)
          {
            if (Find.ShowEndRowCol().CompareTo(item.ShowEndRowCol()) <= 0)
            {
              break;
            }
          }
        }
        node = node.NextShowItem();
      }
      return node;
    }

#if skip
      var node = ItemList.FirstShowItem();
      while (node != null)
      {
        var item = node.Value;
        var visualItem = item as VisualItem;
        if (visualItem.ShowItem != null)
        {
          var showItem = visualItem.ShowItem;
          if (ShowRowCol.ColNum >= item.ShowRowCol().ColNum)
          {
            if (ShowEndRowCol.ColNum <= item.ShowEndRowCol().ColNum)
            {
              break;
            }
          }
        }
        node = node.NextShowItem();
      }
      return node;
    }

    public LinkedListNode<IVisualItem> FindFieldItem(IVisualItem Find)
    {
      var node = ItemList.FirstShowItem();
      while (node != null)
      {
        var item = node.Value;
        var visualItem = item as VisualItem;
        if (visualItem.ShowItem != null)
        {
          var showItem = visualItem.ShowItem;
          if (Find.ShowRowCol().ColNum >= item.ShowRowCol().ColNum)
          {
            if (Find.ShowEndRowCol().CompareTo(item.ShowEndRowCol()) <= 0)
            {
              break;
            }
          }
        }
        node = node.NextShowItem();
      }
      return node;
    }
#endif

    public LinkedListNode<IVisualItem> InsertItem(
      IVisualItem Item, LinkedListNode<IVisualItem> BaseNode, RelativePosition Rltv)
    {
      LinkedListNode<IVisualItem> node = null;
      if (Rltv == RelativePosition.Before)
      {
        node = this.ItemList.AddBefore(BaseNode, Item);
      }
      else if ((Rltv == RelativePosition.After) || (Rltv == RelativePosition.At))
      {
        node = this.ItemList.AddAfter(BaseNode, Item);
      }
      else
      {
        node = this.ItemList.AddLast(Item);
      }
      return node;
    }

    public void RemoveItem(LinkedListNode<IVisualItem> Node)
    {
      ItemList.Remove(Node);
      if (Node.Value.IsOnCanvas == true)
      {

      }
    }

    public override string ToString()
    {
      var sb = new StringBuilder();
      sb.Append("RowNum:" + this.RowNum);
      sb.Append(" Number items:" + this.ItemList.Count);
      return sb.ToString();
    }
  }
}
