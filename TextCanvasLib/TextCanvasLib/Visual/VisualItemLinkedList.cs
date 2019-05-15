using AutoCoder.Ext.System;
using AutoCoder.Report;
using AutoCoder.Telnet.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TextCanvasLib.Common;

namespace TextCanvasLib.Visual
{
  public class VisualItemLinkedList : LinkedList<IVisualItem>, IDataStreamReport
  {
    public LinkedListNode<IVisualItem> FirstShowItem( )
    {
      var node = this.First;
      while( node != null )
      {
        var item = node.Value;
        if ((item is VisualSpanner) == false)
          break;

        node = node.Next;
      }
      return node;
    }

    /// <summary>
    /// return a sequence of IVisualItem from the linked list where the items are
    /// fields.
    /// </summary>
    public IEnumerable<IVisualItem> FieldItems
    {
      get
      {
        var node = this.First;
        while(node !=null)
        {
          var item = node.Value as IVisualItem;
          if (item.IsField == true)
            yield return item;
          node = node.Next;
        }
        yield break;
      }
    }
    public IEnumerable<LinkedListNode<IVisualItem>> Nodes
    {
      get
      {
        var node = this.First;
        while(node != null )
        {
          yield return node;
          node = node.Next;
        }
        yield break;
      }
    }

    public string ReportTitle
    {
      get
      {
        var titleText = "List of Visual Items.";
        return titleText;
      }
    }

    public IEnumerable<string> ToColumnReport(string Title = null)
    {
      var report = new ColumnReport();
      report.AddColDefn("RowNum");
      report.AddColDefn("ColNum");
      report.AddColDefn("TypeCode");
      report.AddColDefn("Length");
      report.AddColDefn("Class", 30);

      var title = Title;
      if (title == null)
        title = ReportTitle;
      if (title != null)
        report.WriteTextLine(title);
      report.WriteColumnHeading(true);

      foreach( var item in this)
      {
        var itemRowCol = item.ItemRowCol;
        var className = item.GetType().ToString();
        var iMore = item as IVisualItemMore;
        string[] columnValues = new string[]
        {
          itemRowCol.RowNum.ToString( ),
          itemRowCol.ColNum.ToString( ),
          iMore.TypeCode( ),
          item.ShowLength.ToString( ),
          className.LastSplit(".")
        };
        report.WriteDetail(columnValues);
      }

      return report;
    }

  }
}
