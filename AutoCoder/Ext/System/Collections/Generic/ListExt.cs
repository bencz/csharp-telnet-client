using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AutoCoder.Collections;
using AutoCoder.Core.Enums;

namespace AutoCoder.Ext.System.Collections.Generic
{
  public static class ListExt
  {
    /// <summary>
    /// concat a list and an item into a resulting list.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="List1"></param>
    /// <param name="Item"></param>
    /// <returns></returns>
    public static List<T> ListConcat<T>(this List<T> List1, T Item2) where T: class
    {
      var rv = new List<T>();
      if (List1 != null)
      {
        foreach (var item in List1)
        {
          rv.Add(item);
        }
      }
      if (Item2 != null)
        rv.Add(Item2);
      return rv;
    }

    public static List<T> ListConcat<T>(this T Item1, List<T> List2) where T : class
    {
      var rv = new List<T>();
      if ( Item1 != null )
        rv.Add(Item1);
      if (List2 != null)
      {
        foreach (var item in List2)
          rv.Add(item);
      }
      return rv;
    }

    public static List<T> ListConcat<T>(this T Item1, T Item2) where T : class
    {
      var rv = new List<T>();
      if ( Item1 != null )
        rv.Add(Item1);
      if ( Item2 != null )
        rv.Add(Item2);
      return rv;
    }

    /// <summary>
    /// advance to the next item in the list. Return the list item and the index of
    /// item.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="List"></param>
    /// <param name="CurrentIndex"></param>
    /// <returns></returns>
    public static ListItem<T> NextItem<T>(this List<T> List, ListItem<T> Current) where T : class
    {
      ListItem<T> item =Current;

      int ix = 0;
      if (item == null)
      {
        ix = 0;
        item = new ListItem<T>();
      }
      else if (item.Index == null)
        ix = 0;
      else
        ix = item.Index.Value + 1;

      if ( ix < List.Count )
      {
        item.Index = ix;
        item.Value = List[ix];
      }
      else
      {
        item = null;
      }

      return item;
    }

    public static ListCursor<T> PositionBegin<T>(this List<T> List) where T : class
    {
      ListCursor<T> cursor = new ListCursor<T>(List, -1, RelativePosition.Begin);
      return cursor;
    }
  }
}
