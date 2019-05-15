using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AutoCoder.Windows.Lines;

namespace AutoCoder.Windows.Primitives
{
  /// <summary>
  /// info on how two shapes vertically intersect each other.
  /// See the LineCoordinates.VerticalIntersect method. 
  /// </summary>
  public class VerticalIntersect
  {
    public LineCoordinates Line1
    { get; set; }

    public LineCoordinates Line2
    { get; set; }

    /// <summary>
    /// offset from the y-axis position of Line1 to the intersect range.
    /// </summary>
    public double Line1Ofs
    { get; set; }

    /// <summary>
    /// offset from the y-axis position of Line2 to the intersect range.
    /// </summary>
    public double Line2Ofs
    { get; set; }

    /// <summary>
    /// the height of the intersection
    /// </summary>
    public double Length
    { get; set; }
  }
}
