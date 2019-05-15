using AutoCoder.Telnet.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoCoder.Telnet.TelnetCommands
{
  public class TelnetCommandList : List<TelnetCommand>, IDataStreamReport
  {

    public string ReportTitle
    {
      get { return "Telnet Commands"; }
    }

    public IEnumerable<string> ToColumnReport(string Title = null)
    {
      List<string> lines = new List<string>();

      if (Title != null)
        lines.Add(Title);
      else
        lines.Add(this.ReportTitle);

      foreach( var item in this)
      {
        lines.AddRange(item.ToReportLines());
      }
      return lines;
    }
  }
}
