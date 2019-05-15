using AutoCoder.Ext.System;
using AutoCoder.Telnet.Common;
using AutoCoder.Telnet.IBM5250.WtdOrders;
using System.Collections.Generic;

namespace AutoCoder.Telnet.IBM5250.Response
{
  public class ResponseItemList : List<object>
  {
    /// <summary>
    /// add the items of the source list to the end of this list.
    /// </summary>
    /// <param name="Source"></param>
    public void Append(ResponseItemList Source)
    {
      foreach (var item in Source)
      {
        this.Add(item);
      }
    }

    public IEnumerable<string> ReportResponseItems()
    {
      bool gotDataOrder = false;
      var lines = new List<string>();

      {
        var titleText = "5250 Response Data Stream.";
        lines.Add(titleText.PadCenter(80));
      }

      foreach( var responseItem in this)
      {
        if ( responseItem is IDataStreamReport )
        {
          var report = responseItem as IDataStreamReport;
          lines.AddRange(report.ToColumnReport( report.ReportTitle ));
        }

        else if (responseItem is LocatedTextDataOrderPair)
        {
          if (gotDataOrder == false )
          {
            var s1 = LocatedTextDataOrderPair.ToColumnReportHeaderLine();
            lines.Add(s1);
            gotDataOrder = true;
          }

          {
            var item = responseItem as LocatedTextDataOrderPair;
            var s1 = item.ToColumnReportLine();
            lines.Add(s1);
          }
        }
      }

      return lines;
    }

  }
}
