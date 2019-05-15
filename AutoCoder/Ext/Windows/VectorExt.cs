using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace AutoCoder.Ext.Windows
{
  public static class VectorExt
  {

    /// <summary>
    /// determine if the vector exceeds system defined minimum drag distance in
    /// either direction.
    /// </summary>
    /// <param name="vec"></param>
    /// <returns></returns>
    public static bool ExceedsMinimumDragDistance(this Vector vec)
    {
      if ((Math.Abs(vec.X) > SystemParameters.MinimumHorizontalDragDistance)
        || (Math.Abs(vec.Y) > SystemParameters.MinimumVerticalDragDistance))
        return true;
      else
        return false;
        // abc
    }
  }
}
