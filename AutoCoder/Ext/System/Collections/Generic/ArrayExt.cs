using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AutoCoder.Ext.System.Collections.Generic
{
  public static class ArrayExt
  {
    /// <summary>
    /// do not use. Use FirstOrDefault linq expression instead.
    /// 
    /// return the first non null element in the array.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="array"></param>
    /// <returns></returns>
    public static T FirstNonNull<T>(this T[] array) where T : class
    {
      T item = null;
      for (int ix = 0; ix < array.Length; ++ix)
      {
        item = array[ix];
        if (item != null)
          break;
      }
      return item;
    }

    /// <summary>
    /// set all elements of the array to the initial value.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="array"></param>
    /// <param name="Item"></param>
    public static void ArrayInit<T>(this T[] array, T Item)
    {
      for( int ix = 0; ix < array.Length; ++ix)
      {
        array[ix] = Item;
      }
    }

    public static void ArrayPush<T>(this T[] array, T Item)
    {
      int ix = array.Length - 2;
      while (ix >= 0)
      {
        array[ix + 1] = array[ix];
        ix -= 1;
      }
      array[0] = Item;
    }
  }
}

