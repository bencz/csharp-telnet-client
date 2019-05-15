using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AutoCoder.Text.Location
{
  /// <summary>
  /// location of text within a text stream
  /// basically, a line id and a column position.
  /// The lineId is an identifier that uniquely identifies the line in the stream.
  /// The column position is the start position of the text in the line.
  /// </summary>
  public class TextLocation
  {
    public LineIdentifier LineId
    { get; set; }

    public int ColumnIndex
    { get; set; }

    public TextLocation()
    {
      this.LineId = null;
      this.ColumnIndex = -1;
    }

    public TextLocation(LineIdentifier LineId, int ColumnIndex)
    {
      this.LineId = LineId;
      this.ColumnIndex = ColumnIndex;
    }

    public TextLocation EndColumnIndex(int Length)
    {
      int ex = this.ColumnIndex + Length - 1;
      return new TextLocation(this.LineId, ex);
    }

    public StreamLocation ToStreamLocation(ScanStream Lookup)
    {
      var lineLoc = Lookup.FindLineLocation(this.LineId);
      int ix = lineLoc.StartIndex + this.ColumnIndex;
      return new StreamLocation(ix);
    }

    public override string ToString()
    {
      return LineId.ToString() + " " + ColumnIndex.ToString();
    }
  }
}
