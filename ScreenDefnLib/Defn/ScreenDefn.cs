using AutoCoder.Ext;
using AutoCoder.Telnet.Common;
using AutoCoder.Telnet.Common.RowCol;
using AutoCoder.Telnet.Enums;
using AutoCoder.Telnet.IBM5250.Content;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using System;
using AutoCoder.Report;
using AutoCoder.Telnet.LogFiles;
using AutoCoder.Ext.System;
using AutoCoder.Telnet.Common.ScreenLoc;
using AutoCoder.Telnet.Common.ScreenDm;

namespace ScreenDefnLib.Defn
{
  /// <summary>
  /// definition of a telnet rendered screen. Includes a list of items ( fields and
  /// literals ) which are drawn on the screen. Screen defn items are intended to
  /// match the write to display command orders received from the telnet server for
  /// a specific, identifiable screen.
  /// </summary>
  public class ScreenDefn : IDataStreamReport, IScreenDefn, ISectionHeader
  {
    public ScreenDefn( 
      string ScreenName, string NamespaceName, string ScreenGuid,
      IScreenDim ScreenDim, IEnumerable<IScreenItem> Items)
    {
      this.ScreenName = ScreenName;
      this.NamespaceName = NamespaceName;
      this.ScreenGuid = ScreenGuid;
      this.ScreenDim = ScreenDim;

      this.Items = new List<IScreenItem>();
      this.LoadItems(Items);
    }

    public ScreenDefn(IScreenDefn source)
      : this(source.ScreenName, source.NamespaceName, source.ScreenGuid, 
          source.ScreenDim, source.Items) 
    {
    }

    /// <summary>
    /// event signaled when a change made to the section.  Most common being that
    /// a screen item within the section is changed, deleted, added.
    /// </summary>
    public event Action<ISectionHeader> SectionHeaderChanged;
    public void OnSectionHeaderChanged()
    {
      if (this.SectionHeaderChanged != null)
      {
        this.SectionHeaderChanged(this);
      }
    }

    public string ScreenName
    { get; set; }

    public string NamespaceName
    { get; set; }

    public string ScreenGuid
    {
      get
      {
        if (_ScreenGuid.IsNullOrEmpty( ) == true)
           this.AssignScreenGuid();
        return _ScreenGuid;
      }
      set { _ScreenGuid = value; }
    }
    private string _ScreenGuid;

    public IScreenDim ScreenDim
    { get; set; }

    public IList<IScreenItem> Items
    {
      get { return _Items; }
      set
      {
        _Items = value;
        this.ItemDict = null;
      }
    }
    private IList<IScreenItem> _Items;

    /// <summary>
    /// screen item dictionary. Provides item lookup by row/col address.
    /// </summary>
    public ScreenItemDict ItemDict
    {
      get
      {
        if (_ItemDict == null)
        {
          _ItemDict = new ScreenItemDict();
          this.ItemDict.Load(this.Items);
        }
        return _ItemDict;
      }
      private set { _ItemDict = value; }
    }
    private ScreenItemDict _ItemDict;

    public string ReportTitle
    {
      get
      {
        return "Screen Definition " + this.ScreenName;
      }
    }

    public override string ToString()
    {
      return "ScreenName:" + ScreenName + " Namespace:" + this.NamespaceName +
        " Screen dim. " + this.ScreenDim.ToText( ) +
        " Num items:" + this.Items.Count;
    }

    public IEnumerable<string> ToColumnReport(string Title = null)
    {
      var report = new ColumnReport(this.ReportTitle, 60);
      DefineReportColumns(report);

      // report.WriteColumnHeading();

      foreach (var itemBase in this.Items)
      {
        if (itemBase is ScreenSection)
        {

        }
        else
        {
          var item = itemBase as ScreenAtomic;

          var valueList = new List<string>();
          valueList.Add(item.ScreenLoc.RowNum.ToString());
          valueList.Add(item.ScreenLoc.ColNum.ToString());
          valueList.Add(item.ItemName);
          valueList.Add(item.ItemType.ToString());
          valueList.Add(item.GetValue());

          report.WriteDetail(valueList.ToArray());
        }
      }

      return report.ToLines();
    }

    private static void DefineReportColumns( ColumnReport Report)
    {
      Report.AddColDefn("Row");
      Report.AddColDefn("Col");
      Report.AddColDefn("Section");
      Report.AddColDefn("Item name");
      Report.AddColDefn("Item type");
      Report.AddColDefn("Value text", 40);
    }
  }

  public static class ScreenDefnExt
  {
  }

}
