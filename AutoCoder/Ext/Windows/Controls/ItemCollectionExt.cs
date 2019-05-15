using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;

namespace AutoCoder.Ext.Windows.Controls
{
  public static class ItemCollectionExt
  {
    public static void AddRange(this ItemCollection Items, IEnumerable<string> Range)
    {
      foreach (var item in Range)
      {
        Items.Add(item);
      }
    }
  }
}
