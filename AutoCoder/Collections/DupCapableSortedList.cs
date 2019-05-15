using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AutoCoder.Collections
{
  /// <summary>
  /// a sorted list that can contain multiple values for the same key.
  /// </summary>
  /// <typeparam name="K"></typeparam>
  /// <typeparam name="V"></typeparam>
  public class DupCapableSortedList<K, V>
  {

    private SortedList<K, FirstValue<V>> StorageList
    { get; set; }

    public DupCapableSortedList()
    {
      this.StorageList = new SortedList<K, FirstValue<V>>();
    }

    /// <summary>
    /// For a key, return the first value and the remaining duplicate key values.
    /// </summary>
    /// <param name="Key"></param>
    /// <returns></returns>
    public FirstValue<V> this[K Key]
    {
      get
      {
        return this.StorageList[Key];
      }
    }

    public void Add(K Key, V Value)
    {
      var keyExists = this.StorageList.ContainsKey(Key);
      if (keyExists == false)
      {
        var item = new FirstValue<V>(Value);
        this.StorageList.Add(Key, item);
      }
      else
      {
        var item = this.StorageList[Key];
        item.AddValue(Value);
      }
    }

    public bool ContainsKey(K Key)
    {
      return this.StorageList.ContainsKey(Key);
    }

    public IEnumerable<V> GetValues(K Key)
    {
      var keyExists = this.StorageList.ContainsKey(Key);
      if (keyExists == false)
        yield break;
      else
      {
        var item = this.StorageList[Key];
        foreach (var vlu in item)
        {
          yield return vlu;
        }
        yield break;
      }
    }

#if skip
    private class ListItem : IEnumerable<V>
    {
      V SingleValue { get; set; }
      List<V> ManyValues { get; set; }

      public ListItem(V Value)
      {
        this.AddValue(Value);
      }

      public void AddValue(V Value)
      {
        if (this.ManyValues != null)
          this.ManyValues.Add(Value);
        else if (this.SingleValue == null)
          this.SingleValue = Value;
        else
        {
          this.ManyValues = new List<V>();
          this.ManyValues.Add(this.SingleValue);
          this.ManyValues.Add(Value);
        }
      }

      IEnumerator<V> IEnumerable<V>.GetEnumerator()
      {
        if (this.SingleValue == null)
          yield break;
        else if (this.ManyValues == null)
        {
          yield return this.SingleValue;
          yield break;
        }
        else
        {
          foreach (var vlu in this.ManyValues)
          {
            yield return vlu;
          }
          yield break;
        }
      }

      #region IEnumerable Members
      System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
      {
        throw new Exception("The method or operation is not implemented.");
      }
      #endregion
    }
#endif

  }
}
