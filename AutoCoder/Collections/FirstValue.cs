using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AutoCoder.Ext;
using AutoCoder.Text;
using AutoCoder.Ext.System.Text;
using System.Collections;

namespace AutoCoder.Collections
{

  // change name to "OneToManyValue", "SingleToManyValue".


  /// <summary>
  /// Class used in combination with the DupCapableSortedList.
  /// Contains a property Value that holds the first value of a duplicate key.
  /// The Remaining property holds the list of values that follow the first value of the
  /// duplicate key. ( the Remaining list is in arrival sequence. )
  /// </summary>
  /// <typeparam name="V"></typeparam>
  public class FirstValue<V> : IEnumerable<V>
  {
    public V Value
    { get; set; }

    public List<V> Remaining
    { get; set; }

    public FirstValue(V Value)
    {
      this.AddValue(Value);
    }

    public void AddValue(V Value)
    {
      if (this.Value == null)
        this.Value = Value;
      else if (this.Remaining == null)
      {
        this.Remaining = new List<V>();
        this.Remaining.Add(Value);
      }
      else
      {
        this.Remaining.Add(Value);
      }
    }

    IEnumerator<V> IEnumerable<V>.GetEnumerator()
    {
      if (this.Value == null)
        yield break;
      else if (this.Remaining == null)
      {
        yield return this.Value;
        yield break;
      }
      else
      {
        yield return this.Value;
        foreach (var vlu in this.Remaining)
        {
          yield return vlu;
        }
        yield break;
      }
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
      throw new Exception("The method or operation is not implemented.");
    }

    public override string ToString()
    {
      StringBuilder sb = new StringBuilder();
      foreach (var s1 in this)
      {
        sb.SentenceAppend(s1.ToString());
      }
      return sb.ToString();
    }
  }
}
