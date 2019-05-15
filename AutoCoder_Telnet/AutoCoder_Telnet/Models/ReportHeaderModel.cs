using AutoCoder.ComponentModel;
using AutoCoder.Telnet.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoCoder.Telnet.Models
{
  public class ReportHeaderModel : HierarchicalModelBase, IDataStreamReport
  {
    public string Title
    { get; set; }

    public IEnumerable<string> ReportLines
    { get; set; }

    public ReportHeaderModel(string Title, IEnumerable<string> ReportLines)
    {
      this.Title = Title;
      this.ReportLines = ReportLines;
    }

    public override bool CanExpand
    {
      get
      {
        return true;
      }
    }

    public string ReportTitle
    {
      get
      {
        return this.Title;
      }
    }

    protected override void FillChildren()
    {
      foreach (var line in this.ReportLines)
      {
        var lineModel = new ReportLineModel(line);
        this.Children.Add(lineModel);
      }
    }

    protected override void FillExpandableMarker()
    {
      var dummy = new HierarchicalModelBase.DummyChild();
      this.Children.Add(dummy);
    }

    public override string ToString()
    {
      return "ReportHeaderModel. " + this.Title + " " + this.ReportLines.Count().ToString() + " lines.";
    }

    public IEnumerable<string> ToColumnReport(string Title = null)
    {
      List<string> report = new List<string>();
      report.Add(this.ReportTitle);
      report.AddRange(this.ReportLines);
      return report;
    }
  }
}

