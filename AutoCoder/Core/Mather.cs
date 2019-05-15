using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AutoCoder.Core
{
  public static class Mather
  {

    /// <summary>
    /// build a double value from a whole part and a fraction part.
    /// </summary>
    /// <param name="WholePart"></param>
    /// <param name="FractionPart"></param>
    /// <returns></returns>
    public static double BuildDouble(int WholePart, int FractionPart)
    {
      string s1 = WholePart.ToString() + "." + FractionPart.ToString();
      double rv = Double.Parse(s1);
      return rv;
    }
  }
}
