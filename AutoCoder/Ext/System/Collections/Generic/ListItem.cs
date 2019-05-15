using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AutoCoder.Ext.System.Collections.Generic
{
  /// <summary>
  /// item of a List. Used by Next() extension method in ListExt.cs. />
  /// </summary>
  /// <typeparam name="T"></typeparam>
  public class ListItem<T> where T : class
  {
    public T Value { get; set; }
    public int? Index { get; set; }

    public ListItem()
    {
      this.Value = null;
      this.Index = null;
    }
  }
}

