using AutoCoder.Systm;
using AutoCoder.Telnet.Common;
using AutoCoder.Telnet.Enums.IBM5250;
using System.Collections.Generic;

namespace AutoCoder.Telnet.IBM5250.WorkstationCommands
{
  public class RestoreScreenCommand : WorkstationCommandBase, IDataStreamReport
  {
    public RestoreScreenCommand(InputByteArray InputArray)
      : base(InputArray, WorkstationCode.RestoreScreen)
    {
      InputArray.AdvanceIndex(2);
    }
    public RestoreScreenCommand( )
      : base(WorkstationCode.RestoreScreen)
    {
    }

    public string ReportTitle
    {
      get
      {
        var titleText = "RestoreScreen command.";
        return titleText;
      }
    }
    public IEnumerable<string> ToColumnReport(string Title = null)
    {
      var report = new List<string>();
      if (Title != null)
        report.Add(Title);
      else
        report.Add(this.ReportTitle);
      return report;
    }

  }
}
