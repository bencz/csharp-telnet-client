using AutoCoder.ComponentModel;
using AutoCoder.Ext;
using AutoCoder.Ext.System;
using AutoCoder.Telnet.Common;
using AutoCoder.Telnet.Enums;
using AutoCoder.Telnet.IBM5250.Common;
using AutoCoder.Telnet.LogFiles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoCoder.Telnet.Models
{
  public class TrafficLogItemTreeModel : HierarchicalModelBase, IDataStreamReport
  {
    public DateTime LogTime
    { get; set; }

    public string LogTimeText
    {
      get { return this.LogTime.ToString("HH:mm:ss.fff"); }
    }

    public byte[] ByteStream
    { get; set; }

    public Direction Direction
    { get; set; }

    public TrafficLogItemTreeModel(TrafficLogItem Item)
    {
      this.LogTime = Item.LogTime;
      this.ByteStream = Item.ByteStream;
      this.Direction = Item.Direction;
    }

    public override bool CanExpand
    {
      get
      {
        return true;
      }
    }

    /// <summary>
    /// parse the bytestream.  Then write objects to the Child collection which
    /// are displayed when this tree node is expanded.
    /// In this case, write the byte stream as chunks of hex bytes. Then, 
    /// depending on the complexity of the byte stream, parse the bytes and write
    /// a report of the parse results.
    /// </summary>
    protected override void FillChildren()
    {
      // the byte stream as a report of hex bytes.
      {
        var report = this.ByteStream.ToHexReport(32);
        var headerModel = new ReportHeaderModel("Byte data stream", report);
        this.Children.Add(headerModel);
      }

      // parse the bytes.
      var rv = ServerDataStream.ParseByteArray(this.ByteStream);
      var wrkstnCmdList = rv.Item1;
      var responseList = rv.Item2;
      var dsh = rv.Item3;
      var telList = rv.Item4;
      var funcList = rv.Item5;

      if ( telList != null)
      {
        var report = telList.ToColumnReport("");
        var headerModel = new ReportHeaderModel("telnet data stream", report);
        this.Children.Add(headerModel);
      }

      if ( dsh != null)
      {
        var report = dsh.ToColumnReport("");
        var headerModel = new ReportHeaderModel("Datastream header", report);
        this.Children.Add(headerModel);
      }

      if ((wrkstnCmdList != null) && (wrkstnCmdList.Count > 0))
      {
        var report = wrkstnCmdList.ToColumnReport("workstation command list");
        var headerModel = new ReportHeaderModel("workstation command list", report);
        this.Children.Add(headerModel);
      }

      if (funcList?.Count > 0)
      {
        var report = funcList.ToColumnReport();
        var headerModel = new ReportHeaderModel("SCS control function list", report);
        this.Children.Add(headerModel);
      }

      if (responseList.Count > 0)
      {
        var report = responseList.ReportResponseItems();
        var headerModel = new ReportHeaderModel("Response data stream", report);
        this.Children.Add(headerModel);
      }
    }

    protected override void FillExpandableMarker()
    {
      var dummy = new HierarchicalModelBase.DummyChild();
      this.Children.Add(dummy);
    }

    public PrintTrafficItemCommand PrintCommand
    {
      get
      {
        return new PrintTrafficItemCommand(c => PrintCommand_Actual());
      }
    }

    public string ReportTitle
    {
      get
      {
        return "Traffic log item";
      }
    }

    private void PrintCommand_Actual( )
    {
      var report = this.ToColumnReport();
      report.ToNotepad();
    }

    public IEnumerable<string> ToColumnReport(string Title = null)
    {
      List<string> report = new List<string>();
      this.EnsureChildrenData();
      report.Add(ReportTitle);
      foreach (var item in this.Children)
      {
        if ( item is IDataStreamReport)
        {
          var itemReport = item as IDataStreamReport;
          report.AddRange(itemReport.ToColumnReport());
        }
        else
        {
          var s1 = item.ToString();
          report.Add(s1);
        }
      }
      return report;
    }
  }
}
