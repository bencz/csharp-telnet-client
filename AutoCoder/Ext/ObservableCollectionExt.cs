using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using AutoCoder.Ext.System;

namespace AutoCoder.Ext
{
  public static class ObservableCollectionExt
  {

    /// <summary>
    /// add Value to the collection if the collection does not yet contain that value.
    /// </summary>
    /// <param name="Collection"></param>
    /// <param name="Value"></param>
    public static void AddDistinct(this ObservableCollection<string> Collection, string Value)
    {
      var found = Collection.FirstOrDefault(c => c == Value);
      if (found == null)
        Collection.Add(Value);
    }

    public static void AddRange<T>(this ObservableCollection<T> Collection, IEnumerable<T> Range)
    {
      foreach (var item in Range)
      {
        Collection.Add(item);
      }
    }

    /// <summary>
    /// Add the value to the observable collection as the most recent item.
    /// If the item already exists in the collection, remove it and insert in the front.
    /// If the item does not exist, insert in the front.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="Collection"></param>
    /// <param name="Value"></param>
    public static void ApplyRecent<T>(this ObservableCollection<T> Collection, T Value)
    {
      var fv = Collection.FirstOrDefault(c => c.Equals(Value));
      if (fv == null)
        Collection.Insert(0, Value);
      else
      {
        Collection.Remove(fv);
        Collection.Insert(0, Value);
      }
    }

    public static void ApplyRecent(this ObservableCollection<string> Collection, string Value)
    {
      if (Value.IsNullOrEmpty() == false)
      {
        var fv = Collection.FirstOrDefault(c => c.Equals(Value));
        if (fv == null)
          Collection.Insert(0, Value);
        else
        {
          Collection.Remove(fv);
          Collection.Insert(0, Value);
        }
      }
    }
  }
}
