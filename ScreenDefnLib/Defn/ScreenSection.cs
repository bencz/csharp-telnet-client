using AutoCoder;
using AutoCoder.Ext.System;
using AutoCoder.Telnet.Common.ScreenDm;
using AutoCoder.Telnet.Enums;
using ScreenDefnLib.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScreenDefnLib.Defn
{
  /// <summary>
  /// a section of the screen which itself contains screen items.
  /// </summary>
  public class ScreenSection : ScreenItem, IScreenSection, ISectionHeader
  {
    public ScreenSection( )
      : base(ShowItemType.Section)
    {
      this.Items = new List<IScreenItem>();
    }

    public ScreenSection(IScreenItem source)
      : base(ShowItemType.Section)
    {
      this.Apply(source);
    }

    /// <summary>
    /// event signaled when a change made to the section.  Most common being that
    /// a screen item within the section is changed, deleted, added.
    /// </summary>
    public event Action<ISectionHeader> SectionHeaderChanged;

    public void OnSectionHeaderChanged()
    {
      if ( this.SectionHeaderChanged != null)
      {
        this.SectionHeaderChanged(this);
      }
    }

    public IList<IScreenItem> Items
    {
      get;
      set;
    }

    /// <summary>
    /// purpose code. body, header, footer, reportColHead, reportDetail.
    /// Indicates the function of the section within the entire screen.
    /// </summary>
    public ScreenPurposeCode? PurposeCode
    {
      get; set;
    }

    /// <summary>
    /// the item name of the section that this section is associate with.
    /// The reportColHead and reportDetail sections will be associated with each
    /// other.
    /// </summary>
    public string AssocSectionName
    {
      get { return _AssocSectionName; }
      set
      {
        _AssocSectionName = value.TrimEndWhitespace();
      }
    }
    private string _AssocSectionName;

    /// <summary>
    /// Max number of repeats of the section starting from the rownum of the
    /// section. Used for reportDetail sections. Equates to page size of a 
    /// subfile.
    /// </summary>
    public int RepeatCount
    { get; set; }
    public bool IsExpanded
    { get; set; }

    public override void Apply(IScreenItem Source)
    {
      var source = Source as IScreenSection;
      if (source == null)
        throw new ApplicationException("source object is not IScreenSection. Use ApplyMatch method instead.");

      base.Apply(source);
      Apply_Actual(source);
    }

    private void Apply_Actual(IScreenSection source)
    {
      this.PurposeCode = source.PurposeCode;
      this.AssocSectionName = source.AssocSectionName;
      this.RepeatCount = source.RepeatCount;
      this.IsExpanded = source.IsExpanded;
      ApplyItems(source);
    }

    public void ApplyItems(IScreenSection source)
    {
      this.Items = new List<IScreenItem>();
      this.LoadItems(source.Items);
    }

    public override void ApplyMatch(IScreenItem sourceItem)
    {
      base.ApplyMatch(sourceItem);
      if (sourceItem is IScreenSection)
      {
        var source = sourceItem as IScreenSection;
        Apply_Actual(source);
      }
    }

    public override string ToString()
    {
      return this.ClassName() + ". " + ToText();
    }
    public override string ToText()
    {
      var sb = new StringBuilder();
      sb.Append("ScreenLoc:" + this.ScreenLoc.ValueText());
      if (this.RepeatCount > 0)
        sb.Append(" RepeatCount:" + this.RepeatCount);
      return sb.ToString();
    }

    public override string ClassName()
    {
      return "ScreenSection";
    }
  }
}
