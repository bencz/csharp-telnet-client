using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;

namespace AutoCoder.Forms
{
  public static class ComboBoxer
  {

    /// <summary>
    /// Return the Items of the combobox as an array.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="InItems"></param>
    /// <returns></returns>
    public static T[] ItemsToArray<T>(IList InItems)
    {
      T[] sa = new T[InItems.Count];
      int ix = 0;
      foreach (T s1 in InItems)
      {
        sa[ix] = s1;
        ix += 1;
      }
      return sa;
    }

  }
}
