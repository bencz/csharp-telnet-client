using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Specialized;

namespace AutoCoder.Collections.Generic
{
  public class ObservableLinkedList<T> : LinkedList<T>, INotifyCollectionChanged where T:class
  {
    public event NotifyCollectionChangedEventHandler CollectionChanged;

    public new void AddAfter(LinkedListNode<T> BaseNode, LinkedListNode<T> Node)
    {
      base.AddAfter(BaseNode, Node);

      if (CollectionChanged != null)
      {
        CollectionChanged(
          this,
          new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, Node));
      }
    }

    public new LinkedListNode<T> AddAfter(LinkedListNode<T> BaseNode, T Value)
    {
      var node = base.AddAfter(BaseNode, Value);

      if (CollectionChanged != null)
      {
        CollectionChanged(
          this,
          new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, node));
      }
      return node;
    }

    public new void AddBefore(LinkedListNode<T> BaseNode, LinkedListNode<T> Node)
    {
      base.AddBefore(BaseNode, Node);

      if (CollectionChanged != null)
      {
        CollectionChanged(
          this,
          new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, Node));
      }
    }

    public new LinkedListNode<T> AddBefore(LinkedListNode<T> BaseNode, T Value)
    {
      var node = base.AddBefore(BaseNode, Value);

      if (CollectionChanged != null)
      {
        CollectionChanged(
          this,
          new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, node));
      }
      return node;
    }

    public new LinkedListNode<T> AddFirst(T Value)
    {
      var node = base.AddFirst(Value);

      if (CollectionChanged != null)
      {
        CollectionChanged(
          this,
          new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, node));
      }

      return node;
    }

    public new void AddFirst(LinkedListNode<T> Node)
    {
      base.AddFirst(Node);

      if (CollectionChanged != null)
      {
        CollectionChanged(
          this,
         new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, Node));
      }
    }

    public new LinkedListNode<T> AddLast(T Value)
    {
      var node = base.AddLast(Value);

      if (CollectionChanged != null)
      {
        CollectionChanged(
          this,
          new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, node));
      }

      return node;
    }

    public new void AddLast(LinkedListNode<T> Node)
    {
      base.AddLast(Node);

      if (CollectionChanged != null)
      {
        CollectionChanged(
          this,
          new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, Node));
      }
    }

    public new void Clear()
    {
      base.Clear();
      if (CollectionChanged != null)
      {
        CollectionChanged(
          this,
          new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
      }
    }

    public new bool Remove(T Value)
    {
      var rc = base.Remove(Value);

      if ((rc == true ) && (CollectionChanged != null))
      {
        CollectionChanged(
          this,
          new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, Value));
      }

      return rc;
    }

    public new void Remove(LinkedListNode<T> Node)
    {
      base.Remove(Node);

      if (CollectionChanged != null)
      {
        CollectionChanged(
          this,
          new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, Node.Value));
      }
    }

    public new void RemoveFirst()
    {
      var fsNode = this.First;

      base.RemoveFirst( );

      if (CollectionChanged != null)
      {
        CollectionChanged(
          this,
          new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, fsNode.Value));
      }
    }

    public new void RemoveLast()
    {
      var lsNode = this.Last;

      base.RemoveLast();

      if (CollectionChanged != null)
      {
        CollectionChanged(
          this,
          new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, lsNode.Value));
      }
    }
  }
}
