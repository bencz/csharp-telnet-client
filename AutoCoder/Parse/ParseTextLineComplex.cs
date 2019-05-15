using System;
using System.Collections.Generic;
using System.Text;
using AutoCoder.Core;

namespace AutoCoder.Parse
{

  // delegates used to create events which enable two LinkedLists of text lines to
  // be kept in sync. Where one list fires the appropriate event whenever a line is
  // added, removed or changed. The subscribing list then uses the LineGuid to 
  // identify the corr line and applies the change.
  public delegate void delRemoveTextLine( TextLineGuid InLineGuid ) ;
  
  public delegate void delAddTextLine(
    string InLineText, TextLineGuid InLineGuid, 
    AcRelativePosition InRltv, TextLineGuid InBase ) ;
  
  public delegate void delAddAfterTextLine( 
  string InLineText, TextLineGuid InLineGuid, TextLineGuid InAfter ) ;
  public delegate void delAddBeforeTextLine(
  string InLineText, TextLineGuid InLineGuid, TextLineGuid InBefore);
  public delegate void delAddFirstTextLine(
  string InLineText, TextLineGuid InLineGuid);
  public delegate void delAddLastTextLine(
  string InLineText, TextLineGuid InLineGuid);
  public delegate void delChangeTextLine(
  string InLineText, TextLineGuid InLineGuid);

  public class ParseTextLineComplex : LinkedList<ParseTextLine>
  {

    LinkedListNode<ParseTextLine> mLastFoundNode;

    public ParseTextLineComplex()
    {
    }

    public LinkedListNode<ParseTextLine> AddAfter(
      ParseTextLine InTextLine, TextLineGuid InAfter)
    {
      LinkedListNode<ParseTextLine> node = null;
      LinkedListNode<ParseTextLine> afterNode = null;
      afterNode = FindNode_Expected(InAfter);
      node = base.AddAfter(afterNode, InTextLine);
      return node;
    }

    public LinkedListNode<ParseTextLine> AddBefore(
      ParseTextLine InTextLine, TextLineGuid InBefore )
    {
      LinkedListNode<ParseTextLine> node = null;
      LinkedListNode<ParseTextLine> beforeNode = null;
      beforeNode = FindNode_Expected(InBefore);
      node = base.AddBefore(beforeNode, InTextLine);
      return node;
    }

    public new LinkedListNode<ParseTextLine> AddFirst(ParseTextLine InTextLine)
    {
      LinkedListNode<ParseTextLine> node = null;
      node = base.AddFirst(InTextLine);
      return node;
    }

    public new LinkedListNode<ParseTextLine> AddLast(ParseTextLine InTextLine)
    {
      LinkedListNode<ParseTextLine> node = null;
      node = base.AddLast(InTextLine);
      return node;
    }

    public void ChangeTextLine(string InLineText, TextLineGuid InLineGuid)
    {
      LinkedListNode<ParseTextLine> node = null;
      node = FindNode_Expected(InLineGuid);
      node.Value.LineText = InLineText;
    }

    public LinkedListNode<ParseTextLine> FindNode(TextLineGuid InLineGuid)
    {
      LinkedListNode<ParseTextLine> node = null;
      LinkedListNode<ParseTextLine> foundNode = null;

      foundNode = null;

      // search from LastFoundNode to end.
      if (mLastFoundNode != null)
      {
        node = mLastFoundNode;
        while (node != null)
        {
          if (node.Value.LineGuid.ToString() == InLineGuid.ToString())
          {
            foundNode = node;
            break;
          }

          node = node.Next;
        }
      }

      // search from the start to end.
      if (foundNode == null)
      {
        node = this.First;
        while (node != null)
        {
          if (node.Value.LineGuid.ToString() == InLineGuid.ToString())
          {
            foundNode = node;
            break;
          }

          node = node.Next;
        }
      }

      // update the globally used LastFoundNode value.
      if (foundNode != null)
        mLastFoundNode = foundNode;

      return foundNode;
    }

    public LinkedListNode<ParseTextLine> FindNode_Expected(TextLineGuid InLineGuid)
    {
      LinkedListNode<ParseTextLine> foundNode = null;
      foundNode = FindNode(InLineGuid);
      if (foundNode == null)
        throw new ApplicationException("text line not found in list of ParseTextLine");
      return foundNode;
    }

    public void Remove(TextLineGuid InLineGuid)
    {
      LinkedListNode<ParseTextLine> node = null;
      node = FindNode_Expected(InLineGuid);
      base.Remove(node);
    }

  }
}
