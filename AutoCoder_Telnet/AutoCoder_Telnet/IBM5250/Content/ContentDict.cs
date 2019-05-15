using AutoCoder.Report;
using AutoCoder.Telnet.Common.RowCol;
using AutoCoder.Telnet.IBM5250.WtdOrders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoCoder.Telnet.IBM5250.Content
{
  public class ContentDict : Dictionary<ZeroRowCol,ContentItemBase>
  {
    public ContentField AddFieldOrder( ZeroRowCol RowCol, StartFieldOrder Field)
    {
      ContentField contentField = null;
      if ( Field.ContinuedFieldSegmentCode == null)
      {
        contentField = new ContentField(RowCol, Field);
      }
      else
      {
        contentField = new ContinuedContentField(RowCol, Field);
      }

      this.Add(RowCol, contentField);
      return contentField;
    }

    public ContentDict Copy( )
    {
      var dict = new ContentDict();
      foreach (var item in this)
      {
        dict.Add(item.Key, item.Value);
      }
      return dict;
    }

    public void AddAllContentText( ScreenContent ScreenContent )
    {
      // loop for each item within the content array.
      foreach (var contentItem in ScreenContent.ContentItems())
      {
        if (contentItem is ContentText)
        {
          ContentItemBase item = null;
          var rc = this.TryGetValue(contentItem.RowCol, out item);
          if (rc == false)
            this.Add(contentItem.RowCol, contentItem);
        }
      }
    }

    public IEnumerable<string> ToColumnReport(ScreenContent Content, string Title = null)
    {
      var report = new ColumnReport("Content Dictionary", 60);
      DefineReportColumns(report);

      // report.WriteColumnHeading();

      foreach (var item in this)
      {
        var rowkey = item.Key;
        var content = item.Value;

        var valueList = new List<string>();
        valueList.Add(rowkey.RowNum.ToString());
        valueList.Add(rowkey.ColNum.ToString());
        valueList.Add(content.GetItemLength(Content).ToString());
        valueList.Add(content.GetShowText(Content));

        report.WriteDetail(valueList.ToArray());
      }

      return report.ToLines();
    }

    private static void DefineReportColumns(ColumnReport Report)
    {
      Report.AddColDefn("Row");
      Report.AddColDefn("Col");
      Report.AddColDefn("Length");
      Report.AddColDefn("Value text", 40);
    }
  }
}
