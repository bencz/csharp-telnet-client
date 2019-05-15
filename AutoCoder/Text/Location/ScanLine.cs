using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AutoCoder.Text.Location
{
  public class ScanLine
  {
    public string Text
    { get; set; }

    /// <summary>
    /// shortcut to the LineId stored in the Location object of this ScanLine.
    /// </summary>
    public LineIdentifier LineId
    {
      get
      {
        return Location.LineId;
      }
    }

    /// <summary>
    /// LineLocation stores the LineId and the index of the line within the text stream.
    /// </summary>
    public LineLocation Location
    { get; set; }

    public ScanLine(string Text, LineLocation Location)
    {
      this.Text = Text;
      this.Location = Location;
    }

    public override string ToString()
    {
      return this.Location.ToString() + " " + this.Text;
    }
  }
}
