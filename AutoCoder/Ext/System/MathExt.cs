using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AutoCoder.Ext.System
{
  public static class MathExt
  {
    public static bool IsEven(long Value)
    {
      long rem;
      Math.DivRem(Value, 2, out rem);
      if (rem == 0)
        return true;
      else
        return false;
    }

    /// <summary>
    /// calc percentage achieved of total amount.
    /// </summary>
    /// <param name="total"></param>
    /// <param name="completeToDate"></param>
    /// <returns></returns>
    public static double PercentOf(double total, double completeToDate)
    {
      if (total == 0)
        return 0;
      else
      {
        var fraction = completeToDate / total;
        var pct = fraction * 100;
        return pct;
      }
    }
  }
}
