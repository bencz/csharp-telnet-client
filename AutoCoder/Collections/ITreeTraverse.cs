using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AutoCoder.Collections
{
  public interface ITreeTraverse<T>
  {
    LinkedList<T> Children
    { get; }

    T NextSibling();

    T FirstChild
    { get; }

    T Parent
    {
      get;
    }
  }

  public static class ITreeTraverseExt
  {
    public static bool HasChildren<T>(this ITreeTraverse<T> Node)
    {
      if (Node.Children == null)
        return false;
      else if (Node.Children.Count > 0)
        return true;
      else
        return false;
    }
  }
}
