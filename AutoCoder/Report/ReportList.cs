using AutoCoder.Ext;
using AutoCoder.Ext.System;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AutoCoder.Report
{
  public class ReportList : IEnumerable<string>
  {
    public List<string> ReportLines { get; set; }
    private bool LastLineWasEmpty { get; set; }

    public ReportList()
    {
      this.ReportLines = new List<string>();
      this.LastLineWasEmpty = false;
    }

    public void WriteGapLine()
    {
      if ((this.LastLineWasEmpty == false) && (this.ReportLines.Count > 0))
        WriteTextLine("");
    }
    public virtual void WriteTextLine(string Text)
    {
      this.ReportLines.Add(Text);
      if (Text.Length == 0)
        this.LastLineWasEmpty = true;
      else
        this.LastLineWasEmpty = false;
    }

    public void WriteTextLines(IEnumerable<string> Lines, int Indent = 0)
    {
      var indentText = StringExt.Repeat(" ", Indent);

      foreach (var line in Lines)
      {
        this.WriteTextLine(indentText + line);
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
