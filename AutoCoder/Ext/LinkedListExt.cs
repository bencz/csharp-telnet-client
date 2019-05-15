using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AutoCoder.Ext
{
  public static class LinkedListExt
  {

    /// <summary>
    /// Return the first node in the linked list that returns true from the compare predicate.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="List"></param>
    /// <param name="Predicate"></param>
    /// <returns></returns>
    public static LinkedListNode<T> FindNode<T>(this LinkedList<T> List, Func<T, bool> Predicate)
    {
      var node = List.First;
      while (true)
      {
        if (node == null)
          break;
        bool rc = Predicate(node.Value);
        if (rc == true)
          break;
        node = node.Next;
      }

      return node;
    }

    /// <summary>
    /// use the IComparer interface of an item to insert that item into the
    /// linked list in an ordered sequence.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="List"></param>
    /// <param name="InsertItem"></param>
    /// <param name="CurrentNode"></param>
    /// <returns></returns>
    public static LinkedListNode<T> OrderedListInsert<T>(
      this LinkedList<T> List, T InsertItem, LinkedListNode<T> CurrentNode)
      where T : IComparer<T>
    {
      LinkedListNode<T> insertedNode = null;
      LinkedListNode<T> baseNode = null;

      // start the search for the ordered insert position at the CurrentNode.
      var startNode = CurrentNode;
      if (startNode == null)
        startNode = List.First;

      // search forward for the insert after node.
      var node = startNode;
      LinkedListNode<T> pvNode = null;
      while (true)
      {
        if (node == null)
        {
          baseNode = pvNode;
          break;
        }

        var rc = InsertItem.Compare(InsertItem, node.Value);

        // insert item is before the current item. If there is a pvNode,
        // then setup to insert after the pvNode.
        if (rc == -1)
        {
          baseNode = pvNode;
          break;
        }

        pvNode = node;
        node = node.Next;
      }

      // search backward for the insert after node.
      if (baseNode == null)
      {
        node = startNode;
        while (true)
        {
          if (node == null)
          {
            baseNode = null;
            break;
          }

          var rc = InsertItem.Compare(InsertItem, node.Value);

          // insert item is equal or after the current item.
          // Setup to insert after this current item.
          if ((rc == 0) || (rc == 1))
          {
            baseNode = node;
            break;
          }

          node = node.Previous;
        }
      }

      // insert after the base node.
      if (baseNode != null)
        insertedNode = List.AddAfter(baseNode, InsertItem);
      else
        insertedNode = List.AddFirst(InsertItem);

      return insertedNode;
    }
  }
}
