using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace AutoCoder.Ext
{
  public static class RectExt
  {
    public static Point LowerRight(this Rect Rect)
    {
      var x = Rect.X;
      var y = Rect.Y + Rect.Height - 1.00;
      return new Point(x, y);
    }
  }
}
