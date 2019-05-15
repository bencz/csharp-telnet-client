using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AutoCoder.Enums
{
  /// <summary>
  /// the form of a range. 
  /// Linear - range runs in a line. When it wraps to the next line it starts on
  ///          the next line at the left most column.
  /// Rectangular - the from and to positions form a rectangle in the coordinate
  ///               space. 
  /// </summary>
  public enum RangeForm
  {
    Linear,
    Rectangular
  };

}
