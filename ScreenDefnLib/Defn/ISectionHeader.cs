using AutoCoder.Core.Enums;
using AutoCoder.Ext.System;
using AutoCoder.Ext.System.Collections.Generic;
using AutoCoder.Report;
using AutoCoder.Serialize;
using AutoCoder.Telnet.Common.ScreenLoc;
using AutoCoder.Telnet.Enums;
using AutoCoder.Telnet.IBM5250.Content;
using ScreenDefnLib.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace ScreenDefnLib.Defn
{
  public interface ISectionHeader
  {
    IList<IScreenItem> Items { get; }

    event Action<ISectionHeader> SectionHeaderChanged;
    void OnSectionHeaderChanged();
  }

  public static class ISectionHeaderExt
  {
    public static void AddItem(
      this ISectionHeader sectionHeader, IScreenItem item)
    {
      sectionHeader.Items.Add(item);
      item.SectionHeader = sectionHeader;
    }

    public static DataItemReport CaptureToReport(
      this ISectionHeader section, IScreenLoc Start, ScreenContent Content)
    {
      var report = new DataItemReport();
      var row = new DataItemList();

      // capture item data from each item of the section that is marked for capture.
      // ( by default, all ScreenField items are captured. )
      foreach (var item in section.Items)
      {
        var rv = item.CaptureReport(Start, Content);
        var dataItem = rv.Item1;
        var itemReport = rv.Item2;

        if ( dataItem != null)
        {
          report.Columns.Add(dataItem.ToColumnDefn());
          row.Add(dataItem);
        }

        // this item is a section. The capture function returned a DataItemReport
        // contains the data of the report. Join that report with what has been
        // captured up to this point. 
        if ( itemReport != null)
        {
          var comboReport = DataItemReport.CombineHorizontally(report, itemReport);
          report = comboReport;
        }
      }

      if ( row.Count > 0)
        report.Rows.Add(row);

      return report;
    }

    public static void ClearItems(this ISectionHeader sectionHeader)
    {
      // first clear the link to section header in all the items.
      foreach( var item in sectionHeader.Items)
      {
        item.SectionHeader = null;
      }

      // clear the list of items of the sectionHeader.
      sectionHeader.Items.Clear();
    }

    public static ScreenItemInstance FindItem(
      this ISectionHeader section, IScreenLoc Start, IScreenLoc FindLoc, ScreenContent Content)
    {
      ScreenItemInstance findItem = null;

      // find the screenitem located at the find location.
      foreach (var item in section.Items)
      {
        findItem = item.FindItem(Start, FindLoc, Content);
        if (findItem != null)
          break;
      }

      return findItem;
    }

    public static IScreenItem GetItemAt( this ISectionHeader sectionHeader, int Index )
    {
      var item = sectionHeader.Items[Index];
      return item;
    }

    /// <summary>
    /// match the section of the screen definition against content of an actual 
    /// screen.
    /// </summary>
    /// <param name="section"></param>
    /// <param name="Content"></param>
    /// <returns></returns>
    public static bool Match(
      this ISectionHeader section, IScreenLoc Start, ScreenContent Content, string DebugInfo)
    {
      bool isMatch = true;

      // match from screenDefn to the screenContent.
      foreach (var item in section.Items)
      {
        if (isMatch == false)
          break;

        isMatch = item.Match(Start, Content, DebugInfo);

        // BgnTemp
        if (( isMatch == false ) && (DebugInfo == "DisplayMessages"))
        {
          isMatch = item.Match(Start, Content, DebugInfo);
        }
        // EndTemp
      }

      return isMatch;
    }

    public static void ReplaceModel(
      this ISectionHeader sectionHeader, ScreenItemModel itemModel, 
      ScreenItemModel screenItemModel)
    {
      var ix = sectionHeader.ItemIndexOf(itemModel);
      if (ix >= 0)
      {
        sectionHeader.InsertItemBefore(ix, screenItemModel);
        sectionHeader.RemoveItem(itemModel);
      }
    }
    public static void InsertItemAfter(
      this ISectionHeader sectionHeader, int AfterIndex, IScreenItem item)
    {
      sectionHeader.Items.InsertAfter(AfterIndex, item);
      item.SectionHeader = sectionHeader;
    }

    public static void InsertItemBefore(
      this ISectionHeader sectionHeader, int BeforeIndex, IScreenItem item)
    {
      sectionHeader.Items.Insert(BeforeIndex, item);
      item.SectionHeader = sectionHeader;
    }

    /// <summary>
    /// insert item at the start of the list. ( before item 0. )
    /// </summary>
    /// <param name="sectionHeader"></param>
    /// <param name="item"></param>
    public static void InsertItemBegin(
      this ISectionHeader sectionHeader, IScreenItem item)
    {
      if (sectionHeader.Items.Count == 0)
        AddItem(sectionHeader, item);
      else
        InsertItemBefore(sectionHeader, 0, item);
    }

    public static bool IsLastItem(this ISectionHeader sectionHeader, int Index)
    {
      if (Index + 1 == sectionHeader.Items.Count)
        return true;
      else
        return false;
    }

    public static int ItemIndexOf(this ISectionHeader sectionHeader, IScreenItem item)
    {
      var ix = sectionHeader.Items.IndexOf(item);
      return ix;
    }

    public static void LoadItems(this ISectionHeader sectionHeader, IEnumerable<IScreenItem> items)
    {
      foreach( var item in items)
      {
        sectionHeader.AddItem(item);
      }
    }

    public static void RemoveItem(
      this ISectionHeader sectionHeader, IScreenItem item)
    {
      sectionHeader.Items.Remove(item);
      item.SectionHeader = null;
    }
    public static void RemoveItemAt(
      this ISectionHeader sectionHeader, int Index)
    {
      var item = sectionHeader.Items[Index];
      sectionHeader.Items.RemoveAt(Index);
      item.SectionHeader = null;
    }

    public static ColumnReport Report(this ISectionHeader Defn, string Title = null)
    {
      var report = new ColumnReport();

      // write the title line.
      if ( Title.IsNullOrEmpty( ) == false)
      {
        report.WriteTextLine(Title);
        report.WriteGapLine();
      }

      report.AddColDefn("ItemName", 15, WhichSide.Left);
      report.AddColDefn("Type", 7);
      report.AddColDefn("Row/Col", 7);
      report.AddColDefn("Lgth", 5);
      report.AddColDefn("Usage", 6);
      report.AddColDefn("Dspatr", 6);
      report.AddColDefn("Text", 50);

      report.WriteColumnHeading(true);

      foreach (var item in Defn.Items)
      {
        var valueList = new string[]
        {
          item.ItemName,
          item.ItemType.ToString( ),
          item.ScreenLoc.ToText( ),
          item.GetLength( ),
          item.GetUsage( ).ToString( ),
          item.GetDsplyAttr( ).ToDsplyAttrText( ),
          item.GetValue( )
        };

        report.WriteDetail(valueList);

        if (item is IScreenSection)
        {
          var sectReport = 
            (item as ISectionHeader)
            .Report( "Items of section " + item.ItemName);
          report.WriteGapLine();
          report.WriteTextLines(sectReport, 5);
          report.WriteGapLine();
        }
      }

      return report;
    }

    public static XElement ToXElement(this ISectionHeader Header, XName Name)
    {
      if (Header == null)
        return new XElement(Name, null);
      else
      {
        XElement xe = new XElement(Name,
            Header.Items.ToXElement("Items")
            );
        return xe;
      }
    }
  }
}

