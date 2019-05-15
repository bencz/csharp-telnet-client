using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AutoCoder.Core.Enums;

namespace AutoCoder.Windows.Lines
{
  /// <summary>
  /// two lines that meet at their end points and form a vertex.
  /// </summary>
  public class LineVertexJunction
  {
    public LineCoordinates Line1
    { get; set; }

    public WhichEndPoint EndPoint1
    { get; set; }

    public LineCoordinates Line2
    { get; set; }

    public WhichEndPoint EndPoint2
    { get; set; }


    public static LineVertexJunction TryMatchLines(
      LineCoordinates Line1, LineCoordinates Line2)
    {
      return null;
    }
  }
}
