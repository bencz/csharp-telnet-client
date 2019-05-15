using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AutoCoder.Ext.Shapes.LineClasses
{
  /// <summary>
  /// the y-axis length of a line. Value is negative or positive depending on which
  /// direction the line slopes from the left side origin of the line.
  /// </summary>
  public struct LineHeight
  {
    public double Value;

    public LineHeight(double Value)
    {
      this.Value = Value;
    }

    public static LineHeight CalcHeight(double Origin, double End)
    {
      if (Origin < End)
        return new LineHeight(End - Origin + 1.00);
      else
        return new LineHeight(Origin - End - 1.00);
    }
  }
}
