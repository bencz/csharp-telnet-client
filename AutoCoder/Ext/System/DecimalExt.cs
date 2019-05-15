using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AutoCoder.Ext.System
{
  public static class DecimalExt
  {
    /// <summary>
    /// compare the two nullable decimal values for equality.
    /// </summary>
    /// <param name="Value1"></param>
    /// <param name="Value2"></param>
    /// <returns></returns>
    public static bool DecimalEqual(this decimal? Value1, decimal? Value2)
    {
      if ((Value1 == null) && (Value2 == null))
        return true;
      else if ((Value1 == null) || (Value2 == null))
        return false;
      else if (Value1.Value == Value2.Value)
        return true;
      else
        return false;
    }

    /// <summary>
    /// compare the two nullable decimal values for not equal.
    /// </summary>
    /// <param name="Value1"></param>
    /// <param name="Value2"></param>
    /// <returns></returns>
    public static bool DecimalNotEqual(this decimal? Value1, decimal? Value2)
    {
      if ((Value1 == null) && (Value2 == null))
        return false;
      else if ((Value1 == null) || (Value2 == null))
        return true;
      else if (Value1.Value == Value2.Value)
        return false;
      else
        return true;
    }

    /// <summary>
    /// return the length of the decimal value and the number of precision digits.
    /// </summary>
    /// <param name="Value"></param>
    /// <returns></returns>
    public static Tuple<int,int> GetLengthAndPrecision(this decimal Value)
    {
      int whole = 0 ;
      int prec = 0 ;
      var s1 = Math.Abs(Value).ToString();
      var fx = s1.IndexOf(".");
      if (fx == -1)
        whole = s1.Length;
      else
      {
        whole = fx;
        prec = s1.Length - (fx + 1);
      }
      return new Tuple<int, int>(whole + prec, prec);
    }

  }
}
