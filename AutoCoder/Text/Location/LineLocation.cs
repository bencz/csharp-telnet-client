using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AutoCoder.Text.Location
{
  // stores the start location of each line within a text stream.     
  public class LineLocation
  {
    public LineIdentifier LineId
    { get; set; }

    public int StartIndex
    { get; set; }

    /// <summary>
    /// the end position of the line in the stream. 
    /// This value should include the EOL pattern of the line.
    /// </summary>
    public int EndIndex
    { get; set; }

    public LineLocation(LineIdentifier LineId, int StartIndex, int EndIndex)
    {
      this.LineId = LineId;
      this.StartIndex = StartIndex;
      this.EndIndex = EndIndex;
    }

    public override string ToString()
    {
      return this.LineId.ToString() + " " +
        this.StartIndex.ToString() + "-" + this.EndIndex.ToString();
    }
  }

}
