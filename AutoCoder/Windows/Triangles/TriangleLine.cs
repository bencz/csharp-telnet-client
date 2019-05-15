using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AutoCoder.Windows.Lines;
using AutoCoder.Core.Enums;

namespace AutoCoder.Windows.Triangles
{
  /// <summary>
  /// a line in a triangle. Stores the line and the vertices of the line.
  /// </summary>
  public class TriangleLine
  {
    public string SideName
    { get; set; }

    /// <summary>
    /// The line of the triangle
    /// </summary>
    public LineCoordinates LineCoor
    { get; set; }

    /// <summary>
    /// The vertex that is formed at the start of the line.
    /// </summary>
    public TriangleVertex StartVertex
    { get; set; }

    /// <summary>
    /// the line of the triangle that intersects the start point of this line.
    /// </summary>
    public TriangleLine StartLine
    { get; set; }

    /// <summary>
    /// the line of the triangle that intersects the end point of this line.
    /// </summary>
    public TriangleLine EndLine
    { get; set; }

    public override string ToString()
    {
      string startInfo = "";
      if ( this.StartLine != null )
      {
        startInfo = "Start " + this.StartLine.SideName + " " + this.LineCoor.Start;
      }
      string endInfo = "" ;
      if ( this.EndLine != null )
      {
        endInfo = "End " + this.EndLine.SideName + " " + this.LineCoor.End;
      }
      return this.SideName + " " + startInfo + " " + endInfo;
    }

    /// <summary>
    /// The vertex that is formed at the end of the line.
    /// </summary>
    public TriangleVertex EndVertex
    { get; set; }

#if skip
    public bool TryMatch( WhichEndPoint WhichEnd, TriangleLine[] OtherLines )
    {
      bool gotMatch = false;

      // match the start of this line to the start or end of the other lines.
      if (WhichEnd == WhichEndPoint.Start)
      {
        foreach (var otherLine in OtherLines)
        {
          if (this.StartLine != null)
          {
            gotMatch = true;
            break;
          }

          if ((otherLine.StartLine == null)
            && (this.LineCoor.Start.Equals(otherLine.LineCoor.Start)))
          {
            otherLine.StartLine = this;
            this.StartLine = otherLine;
            gotMatch = true;
          }

          else if ((otherLine.EndLine == null)
            && (this.LineCoor.Start.Equals(otherLine.LineCoor.End)))
          {
            this.StartLine = otherLine;
            otherLine.EndLine = this;
            gotMatch = true;
          }
        }
      }

      if (WhichEnd == WhichEndPoint.End)
      {
        foreach (var otherLine in OtherLines)
        {
          if (this.EndLine != null)
          {
            gotMatch = true;
            break;
          }

          if ((otherLine.StartLine == null)
            && (this.LineCoor.End.Equals(otherLine.LineCoor.Start)))
          {
            otherLine.EndLine = this;
            this.StartLine = otherLine;
            gotMatch = true;
          }

          else if ((otherLine.EndLine == null)
            && (this.LineCoor.End.Equals(otherLine.LineCoor.End)))
          {
            this.EndLine = otherLine;
            otherLine.EndLine = this;
            gotMatch = true;
          }
        }
      }

      return gotMatch;
    }
#endif

  }
}
