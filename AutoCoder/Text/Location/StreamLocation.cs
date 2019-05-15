using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AutoCoder.Text.Location
{
  /// <summary>
  /// the location within a text stream.
  /// Simply encapsulates an integer index. A class is used instead of the integer
  /// to establish the associations between LineLocation and StreamLocation.
  /// </summary>
  public class StreamLocation
  {
    public int Value
    { get; set; }

    public StreamLocation(int Value)
    {
      this.Value = Value;
    }

    public TextLocation ToTextLocation(ScanStream Lookup)
    {
      LineLocation found = null;

      // find the LineLocation on which the StreamIndex is located.
      foreach (var line in Lookup.Lines)
      {
        var loc = line.Location;
        if ((this.Value >= loc.StartIndex) && (this.Value <= loc.EndIndex))
        {
          found = loc;
          break;
        }
        else if (loc.StartIndex > this.Value)
          break;
      }

      // location not found.
      if (found == null)
        throw new ApplicationException("stream location " + this.Value.ToString() +
          " is not found.");

      TextLocation textLoc =
        new TextLocation(found.LineId, this.Value - found.StartIndex);
      return textLoc;
    }
  }
}
