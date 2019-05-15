using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AutoCoder.Ext.Windows
{
  public static class ThicknessExt
  {
    public static IEnumerable<double> EachSideThickness(this global::System.Windows.Thickness thickness)
    {
      yield return thickness.Left;
      yield return thickness.Top;
      yield return thickness.Right;
      yield return thickness.Bottom;
      yield break;
    }

    public static double MaxThickness(this global::System.Windows.Thickness thickness)
    {
      double maxThick = Math.Max(thickness.Left, thickness.Top);
      maxThick = Math.Max(maxThick, thickness.Right);
      maxThick = Math.Max(maxThick, thickness.Bottom);
      return maxThick;
    }
  }
}
