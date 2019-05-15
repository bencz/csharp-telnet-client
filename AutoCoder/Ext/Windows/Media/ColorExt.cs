using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;

namespace AutoCoder.Ext.Windows.Media
{
  public static class ColorExt
  {
    public static Color LightenColor(this Color color)
    {
      float correctionFactor = 0.5f;
      float red = (255 - color.R) * correctionFactor + color.R;
      float green = (255 - color.G) * correctionFactor + color.G;
      float blue = (255 - color.B) * correctionFactor + color.B;
      Color lighterColor = Color.FromArgb(color.A, (byte)red, (byte)green, (byte)blue);
      return lighterColor;
    }

    public static Color AdjustColor(this Color color, float correctionFactor)
    {
      float red = (255 - color.R) * correctionFactor + color.R;
      float green = (255 - color.G) * correctionFactor + color.G;
      float blue = (255 - color.B) * correctionFactor + color.B;
      Color lighterColor = Color.FromArgb(color.A, (byte)red, (byte)green, (byte)blue);
      return lighterColor;
    }
  }
}
