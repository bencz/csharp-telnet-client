using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AutoCoder.Ext
{
  public static class IntArrayExt
  {
    /// <summary>
    /// parse the string encoded array of ints.
    /// </summary>
    /// <param name="InText"></param>
    /// <returns></returns>
    public static int[] Parse(string InText)
    {
      string[] nbrs = InText.Split(new char[] { ',' });
      int[] vals = new int[nbrs.Length];
      int ix = 0 ;
      foreach (string s1 in nbrs)
      {
        if (s1.Length == 0)
          vals[ix] = 0;
        else
          vals[ix] = Int32.Parse(s1);
        ix += 1;
      }

      return vals;
    }

    /// <summary>
    /// Transform the array of int to a string of comma delim ints as string.
    /// </summary>
    /// <param name="InValues"></param>
    /// <returns></returns>
    public static string ToString(int[] InValues)
    {
      StringBuilder sb = new StringBuilder();
      foreach (int val in InValues)
      {
        if (sb.Length > 0)
          sb.Append(", ");
        sb.Append(val.ToString());
      }
      return sb.ToString();
    }

  }
}
