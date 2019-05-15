using AutoCoder.Systm;
using AutoCoder.Telnet.Common;
using AutoCoder.Telnet.Enums.IBM5250;
using System.Collections.Generic;

namespace AutoCoder.Telnet.IBM5250.WorkstationCommands
{
  public class ClearUnitCommand : WorkstationCommandBase, IDataStreamReport
  {
    public ClearUnitCommand( InputByteArray  InputArray )
      : base(InputArray, WorkstationCode.ClearUnit)
    {
      InputArray.AdvanceIndex(2);
    }
    public ClearUnitCommand( )
      : base(WorkstationCode.ClearUnit)
    {
    }
    public string ReportTitle
    {
      get
      {
        var titleText = "ClearUnit command.";
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
