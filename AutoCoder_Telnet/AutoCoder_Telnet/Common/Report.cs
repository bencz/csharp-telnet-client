using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoCoder.Telnet.Common
{
  public class Report : IEnumerable<string>
  {
    public List<string> ReportLines { get; set; }
    private bool LastLineWasEmpty { get; set; }

    public Report( )
    {
      this.ReportLines = new List<string>();
      this.LastLineWasEmpty = false;
    }

    public void WriteGapLine()
    {
      if ((this.LastLineWasEmpty == false) && (this.ReportLines.Count > 0))
        WriteTextLine("");
    }
    public void WriteTextLine(string Text)
    {
      this.ReportLines.Add(Text);
      if (Text.Length == 0)
        this.LastLineWasEmpty = true;
      else
        this.LastLineWasEmpty = false;
    }

    public void WriteTextLines(IEnumerable<string> Lines)
    {
      foreach( var line in Lines)
      {
        this.WriteTextLine(line);
      }
    }

    public IEnumerable<string> ToLines()
    {
      return this.ReportLines;
    }

    public IEnumerator<string> GetEnumerator()
    {
      return ((IEnumerable<string>)ReportLines).GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
      return ((IEnumerable<string>)ReportLines).GetEnumerator();
    }
  }
}
