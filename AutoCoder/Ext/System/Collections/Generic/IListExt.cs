using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AutoCoder.Ext.System.Collections.Generic
{
  public static class IListExt
  {
    /// <summary>
    /// insert into list, after the specified list item index.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="list"></param>
    /// <param name="index"></param>
    /// <param name="item"></param>
    public static void InsertAfter<T>(this IList<T> list, int index, T item)
    {
      int ix = index + 1;
      if (ix >= list.Count)
        list.Add(item);
      else
        list.Insert(ix, item);
    }

    /// <summary>
    /// insert into list, before the specified list item index.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="list"></param>
    /// <param name="index"></param>
    /// <param name="item"></param>
    public static void InsertBefore<T>(this IList<T> list, int index, T item)
    {
      int ix = index;
      if (ix >= list.Count)
        list.Add(item);
      else
        list.Insert(ix, item);
    }
  }
}
