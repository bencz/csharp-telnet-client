using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoCoder.Telnet.Common
{
  public interface IDataStreamReport
 {
    string ReportTitle { get; }
    IEnumerable<string> ToColumnReport(string Title = null);
  }
}
