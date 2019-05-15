using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace AutoCoder.Collections
{
  public class KeyedLinkedList<K,V> : IEnumerable<V>
  {
    LinkedList<V> mList = new LinkedList<V>();
    Dictionary<K,LinkedListNode<V>> mNodeLookupDict =
      new Dictionary<K,LinkedListNode<V>>( ) ;

    public KeyedLinkedList()
    {
    }

    public LinkedListNode<V> this[K InKey]
    {
      get
      {
        LinkedListNode<V> node = mNodeLookupDict[InKey];
        return node;
      }
    }

    public int Count
    {
      get { return mList.Count; }
    }

    public LinkedListNode<V> AddAfter(
      LinkedListNode<V> InBaseNode, K InKey, V InValue)
    {
      LinkedListNode<V> node = null;
      node = mList.AddAfter(InBaseNode, InValue ) ;
      mNodeLookupDict.Add(InKey, node);
      return node;
    }

    public LinkedListNode<V> AddBefore(
      LinkedListNode<V> InBaseNode, K InKey, V InValue)
    {
      LinkedListNode<V> node = null;
      node = mList.AddBefore(InBaseNode, InValue);
      mNodeLookupDict.Add(InKey, node);
      return node;
    }

    public LinkedListNode<V> AddFirst(K InKey, V InValue)
    {
      LinkedListNode<V> node = null;
      node = mList.AddFirst(InValue);
      mNodeLookupDict.Add(InKey, node);
      return node;
    }

    public LinkedListNode<V> AddLast(K InKey, V InValue)
    {
      LinkedListNode<V> node = null;
      node = mList.AddLast(InValue);
      mNodeLookupDict.Add(InKey, node);
      return node;
    }


    #region IEnumerable<V> Members

    public IEnumerator<V> GetEnumerator()
    {
      if (mList == null)
        yield break;
      else
      {
        foreach (V elem in mList)
        {
          yield return elem;
        }
      }
    }

    #endregion

    #region IEnumerable Members

    IEnumerator IEnumerable.GetEnumerator()
    {
      throw new Exception("The method or operation is not implemented.");
    }

    #endregion
  }
}
