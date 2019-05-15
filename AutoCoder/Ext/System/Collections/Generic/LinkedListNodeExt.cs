using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AutoCoder.Ext.System.Collections.Generic
{
  public static class LinkedListNodeExt
  {
    public static LinkedListNode<T> NextCircular<T>(this LinkedListNode<T> Node)
    {
      var node = Node;
      if (node.Next == null)
        node = node.List.First;
      else
        node = node.Next;
      return node;
    }
    public static LinkedListNode<T> PrevCircular<T>( this LinkedListNode<T> Node )
    {
      var node = Node;
      if (node.Previous == null)
        node = node.List.Last;
      else
        node = node.Previous;
      return node;
    }
  }
}
