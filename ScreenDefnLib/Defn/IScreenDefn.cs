using AutoCoder.Core.Enums;
using AutoCoder.Ext;
using AutoCoder.Ext.System;
using AutoCoder.Report;
using AutoCoder.Serialize;
using AutoCoder.Systm.Data;
using AutoCoder.Telnet.Common.ScreenDm;
using AutoCoder.Telnet.Common.ScreenLoc;
using AutoCoder.Telnet.Enums;
using AutoCoder.Telnet.IBM5250.Content;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Xml.Linq;

namespace ScreenDefnLib.Defn
{
  public interface IScreenDefn
  {
    string ScreenName { get; set; }
    string NamespaceName { get; set; }
    string ScreenGuid { get; set; }
    IScreenDim ScreenDim { get; set; }
    IList<IScreenItem> Items { get; }
  }

  public static class IScreenDefnExt
  {
    public static void AssignScreenGuid(this IScreenDefn defn)
    {
      defn.ScreenGuid = Guid.NewGuid().ToString();
    }

    /// <summary>
    /// capture the data of the screen to a DataTable class. 
    /// </summary>
    /// <param name="Defn"></param>
    /// <param name="Content"></param>
    /// <returns></returns>
    public static EnhancedDataTable Capture(
      this IScreenDefn Defn, ScreenContent Content, 
      ScreenItemInstance ItemInstance = null)
    {
      ISectionHeader sectionHeader = Defn as ISectionHeader;
      var start = new OneScreenLoc(1, 1);
      var report = sectionHeader.CaptureToReport(start, Content);
      var table = report.ToDataTable();

      // mark the selected datarow in the dataTable.
      int rownum = 0;
      if ((ItemInstance != null) && (ItemInstance.RepeatNum > 0))
      {
        rownum = ItemInstance.RepeatNum - 1;
      }
      table.MarkSelectedRow(rownum);

      return table;
    }

    /// <summary>
    /// capture the data of the screen to a DataItemReport class. 
    /// </summary>
    /// <param name="Defn"></param>
    /// <param name="Content"></param>
    /// <returns></returns>
    public static DataItemReport CaptureToReport(this IScreenDefn Defn, ScreenContent Content)
    {
      ISectionHeader sectionHeader = Defn as ISectionHeader;
      var start = new OneScreenLoc(1, 1);
      var itemReport = sectionHeader.CaptureToReport(start, Content);
      return itemReport;
    }

    public static ScreenItemInstance FindItem(
      this IScreenDefn Defn, IScreenLoc FindLoc, ScreenContent Content)
    {
      ScreenItemInstance found = null;

      ISectionHeader sectionHeader = Defn as ISectionHeader;
      var start = new OneScreenLoc(1, 1);
      found = sectionHeader.FindItem(start, FindLoc, Content);

      return found;
    }

    /// <summary>
    /// determine if the ScreenDefn matches the screen content.
    /// </summary>
    /// <param name="Content"></param>
    /// <returns></returns>
    public static bool Match(this IScreenDefn Defn, ScreenContent Content)
    {
      bool isMatch = true;

      // BgnTemp
      // SpecialLogFile.AppendTextLines(this.ToColumnReport());
      // EndTemp

      // extract all ContentText on the screen. Add to FieldDict.
      Content.AddAllContentText();

      ISectionHeader sectionHeader = Defn as ISectionHeader;
      var start = new OneScreenLoc(1, 1);
      isMatch = sectionHeader.Match(start, Content, Defn.ScreenName);

      return isMatch;
    }

    public static ColumnReport Report(this IScreenDefn Defn )
    {
      var defnHeader = Defn as ISectionHeader;
      var report = defnHeader.Report("Screen Definition. " + Defn.ScreenName);

      return report;
    }

    public static XElement ToXElement(this IScreenDefn Defn, XName Name)
    {
      if (Defn == null)
        return new XElement(Name, null);
      else
      {
        XElement xe = new XElement(Name,
            new XElement("ScreenName", Defn.ScreenName),
            new XElement("NamespaceName", Defn.NamespaceName),
            new XElement("ScreenGuid", Defn.ScreenGuid),
            Defn.ScreenDim.ToXElement("ScreenDim"),
            Defn.Items.ToXElement("ScreenItemList")
            );
        return xe;
      }
    }

    public static IScreenDefn ToScreenDefn(
      this XElement Elem, XNamespace Namespace)
    {
      IScreenDefn screenDefn = null;
      if (Elem != null)
      {
        var screenName = Elem.Element(Namespace + "ScreenName").StringOrDefault("");
        var namespaceName = Elem.Element(Namespace + "NamespaceName").StringOrDefault("");
        var screenGuid = Elem.Element(Namespace + "ScreenGuid").StringOrDefault(null);
        var screenDim = Elem.Element(Namespace + "ScreenDim").ToScreenDim(Namespace);
        var itemList = Elem.Element(Namespace + "ScreenItemList").ToScreenItemList(Namespace);
        screenDefn = new ScreenDefn(screenName, namespaceName, screenGuid, screenDim, itemList);
      }
      return screenDefn;
    }
  }
}
