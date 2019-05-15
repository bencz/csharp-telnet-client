using AutoCoder.Report;
using AutoCoder.Telnet.Common;
using AutoCoder.Telnet.IBM5250.WtdOrders.wtdCommon;
using System.Collections.Generic;

namespace AutoCoder.Telnet.IBM5250.WorkstationCommands
{
  /// <summary>
  /// list of workstation commands received from the server. Some workstation commands
  /// are WriteToDisplay, ClearUnit, ReadMdt and WriteStructuredField.
  /// </summary>
  public class WorkstationCommandList : List<WorkstationCommandBase>, IDataStreamReport
  {

    public string ReportTitle
    {
      get
      {
        var titleText = "Workstation command list.";
        return titleText;
      }
    }

    /// <summary>
    /// add the items of the source list to the end of this list.
    /// </summary>
    /// <param name="Source"></param>
    public void Append( WorkstationCommandList Source )
    {
      foreach( var item in Source )
      {
        this.Add(item);
      }
    }

    public IEnumerable<string> DumpContents( )
    {
      var list = new List<string>();

      foreach (var cmdBase in this)
      {
        var line = cmdBase.ToString();
        list.Add(line);
      }
      return list;
    }

    public override string ToString()
    {
      return "WorkstationCommandList. " + this.Count + " items.";
    }

    public IEnumerable<string> ToColumnReport(string Title = null)
    {
      var report = new ReportList();

      if (Title != null)
        report.WriteTextLine(Title);

      foreach (var workstationCmd in this)
      {
        if (workstationCmd is WriteToDisplayCommand)
        {
          report.WriteGapLine();
          report.WriteTextLine("Write to display command");
          var wtdCmd = workstationCmd as WriteToDisplayCommand;
          var printedLines = wtdCmd.ToColumnReport();
          report.WriteTextLines(printedLines);
        }
        else if ( workstationCmd is IDataStreamReport)
        {
          report.WriteGapLine();
          var iReport = workstationCmd as IDataStreamReport;
          var report2 = iReport.ToColumnReport();
          report.WriteTextLines(report2);
        }
        else
        {
          var textLine = workstationCmd.ToString();
          report.WriteTextLine(textLine);
        }
      }

      return report;
    }

    /// <summary>
    /// look thru the WorkstationCommandList. For each WriteToDisplay command, print
    /// the orders of the command.
    /// </summary>
    /// <returns></returns>
    public IEnumerable<string> Report_WTD_Orders( 
      string Title = null, string Footer = null )
    {
      List<string> report = new List<string>();
      if (Title != null)
        report.Add(Title);

      foreach (var workstationCmd in this)
      {
        if (workstationCmd is WriteToDisplayCommand)
        {
          var wtdCmd = workstationCmd as WriteToDisplayCommand;

          var printedLines = wtdCmd.Print_WTD_Orders();
          report.AddRange(printedLines);
        }
      }

      if (Footer != null)
        report.Add(Footer);

      return report;
    }
  }
}
