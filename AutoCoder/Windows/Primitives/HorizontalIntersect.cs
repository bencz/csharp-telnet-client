using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using AutoCoder.Windows.Lines;

namespace AutoCoder.Windows.Primitives
{
  public class HorizontalIntersect
  {
    public LineCoordinates Line1
    { get; set; }

    public LineCoordinates Line2
    { get; set; }

    /// <summary>
    /// offset from the x-axis position of Line1 to the intersect range.
    /// </summary>
    public double Line1Ofs
    { get; set; }

    /// <summary>
    /// offset from the x-axis position of Line2 to the intersect range.
    /// </summary>
    public double Line2Ofs
    { get; set; }

    /// <summary>
    /// the width of the intersection
    /// </summary>
    public double Length
    { get; set; }

    public override string ToString()
    {
      return "Line1ofs:" + this.Line1Ofs + " Line2Ofs:" + this.Line2Ofs +
        " Length:" + this.Length;
    }
  }
}
