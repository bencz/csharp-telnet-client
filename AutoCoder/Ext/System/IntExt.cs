using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AutoCoder.Ext.System
{
  public static class IntExt
  {
    /// <summary>
    /// return value with lowest value
    /// </summary>
    /// <param name="Value1"></param>
    /// <param name="Value2"></param>
    /// <returns></returns>
    public static int Min(int Value1, int Value2)
    {
      if (Value1 < Value2)
        return Value1;
      else
        return Value2;
    }

    /// <summary>
    /// return value with highest value
    /// </summary>
    /// <param name="Value1"></param>
    /// <param name="Value2"></param>
    /// <returns></returns>
    public static int Max(int Value1, int Value2)
    {
      if (Value1 > Value2)
        return Value1;
      else
        return Value2;
    }

    /// <summary>
    /// format the int as a string. If value is zero return blanks. Otherwise return
    /// the int as a string.
    /// </summary>
    /// <param name="Value"></param>
    /// <returns></returns>
    public static string ZeroEmptyString(this int Value)
    {
      if (Value == 0)
        return "";
      else
        return Value.ToString();
    }

    /// <summary>
    /// return the nullable int as a string. If the value is null then return empty
    /// string. Otherwise, return the ToString value of the int.
    /// </summary>
    /// <param name="Value"></param>
    /// <returns></returns>
    public static string NullEmptyString(this int? Value)
    {
      if (Value == null)
        return "";
      else
        return Value.Value.ToString();
    }
  }
}
