using System;
using System.Collections.Generic;
using System.Text;

namespace AutoCoder.Collections
{
  public static class Lister
  {

    public static void CopyListToList(List<string> InToList, List<string> InFromList)
    {
      foreach (string line in InFromList)
      {
        InToList.Add(line);
      }
    }

    public static void CopyListToList(
      LinkedList<string> InToList, LinkedList<string> InFromList)
    {
      foreach (string line in InFromList)
      {
        InToList.AddLast(line);
      }
    }

    /// <summary>
    /// compare the contents of the two lists for equality.
    /// </summary>
    /// <param name="InList1"></param>
    /// <param name="InList2"></param>
    /// <returns></returns>
    public static bool CompareEqual(List<string> InList1, List<string> InList2)
    {
      bool rc = true;

      if ((InList1 == null) && (InList2 == null))
        rc = true;
      else if ((InList1 == null) || (InList2 == null))
        rc = false;
      else if (InList1.Count != InList2.Count)
        rc = false;
      else
      {
        List<string>.Enumerator it1 = InList1.GetEnumerator();
        List<string>.Enumerator it2 = InList2.GetEnumerator();

        while ( it1.MoveNext( ) == true)
        {
          it2.MoveNext();
          if (it1.Current != it2.Current)
          {
            rc = false;
            break;
          }
        }
      }

      return rc;
    }

    /// <summary>
    /// compare the contents of the two lists for equality.
    /// </summary>
    /// <param name="InList1"></param>
    /// <param name="InList2"></param>
    /// <returns></returns>
    public static bool CompareEqual(List<string> InList1, string[] InList2)
    {
      bool rc = true;

      if ((InList1 == null) && (InList2 == null))
        rc = true;
      else if ((InList1 == null) || (InList2 == null))
        rc = false;
      else if (InList1.Count != InList2.Length)
        rc = false;
      else
      {
        List<string>.Enumerator it1 = InList1.GetEnumerator();
        int ix = 0;
        while (it1.MoveNext() == true)
        {
          if (it1.Current != InList2[ix])
          {
            rc = false;
            break;
          }
          ix += 1;
        }
      }

      return rc;
    }

    /// <summary>
    /// Find the first node in LinkedList that matches the expression.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="InList"></param>
    /// <param name="InEvalMthd"></param>
    /// <returns></returns>
    public static LinkedListNode<T> FindFirst<T>(
      this LinkedList<T> InList, Func<T, bool> InEvalMthd)
    {
      LinkedListNode<T> foundNode = null;
      LinkedListNode<T> node = null;
      node = InList.First;
      while (true)
      {
        if (node == null)
          break;
        bool rv = InEvalMthd(node.Value);
        if (rv == true)
        {
          foundNode = node;
          break;
        }
        node = node.Next;
      }
      return foundNode;
    }

    /// <summary>
    /// Find the first node in LinkedList that matches the expression.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="InList"></param>
    /// <param name="InEvalMthd"></param>
    /// <returns></returns>
    public static IEnumerable<T> FindAll<T>(
      this LinkedList<T> InList, Func<T, bool> InEvalMthd)
    {
      List<T> foundItems = new List<T>();
      LinkedListNode<T> node = null;
      node = InList.First;
      while (true)
      {
        if (node == null)
          break;
        bool rv = InEvalMthd(node.Value);
        if (rv == true)
        {
          foundItems.Add(node.Value);
        }
        node = node.Next;
      }
      return foundItems;
    }

    /// <summary>
    /// Find the first node in LinkedList that matches the expression.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="InList"></param>
    /// <param name="InEvalMthd"></param>
    /// <returns></returns>
    public static LinkedListNode<T> FindLast<T>(
      this LinkedList<T> InList, Func<T, bool> InEvalMthd)
    {
      LinkedListNode<T> foundNode = null;
      LinkedListNode<T> node = null;
      node = InList.Last;
      while (true)
      {
        if (node == null)
          break;
        bool rv = InEvalMthd(node.Value);
        if (rv == true)
        {
          foundNode = node;
          break;
        }
        node = node.Previous;
      }
      return foundNode;
    }

  }
}
