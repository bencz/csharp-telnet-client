using AutoCoder;
using AutoCoder.Ext;
using AutoCoder.Telnet.Common.ScreenDm;
using AutoCoder.Telnet.Enums;
using ScreenDefnLib.Defn;
using ScreenDefnLib.Enums;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScreenDefnLib.Models
{
  public class ScreenSectionModel : ScreenItemModel, IScreenSection, ISectionHeader
  {
    public ScreenSectionModel( )
      : base(ShowItemType.Section)
    {
      this.Items = new ObservableCollection<IScreenItem>();
    }

    public ScreenSectionModel(IScreenSection Item)
      : base(Item)
    {
      this.Apply(Item);
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


    /// <summary>
    /// height and width of this section. Used especially with a section that 
    /// repeats ( a subfile ). 
    /// </summary>
    public IntPair Dim
    {
      get
      {
        return this.CalcDim();
      }
    }
    public IntPair RepeatDim
    {
      get { return this.CalcRepeatDim(); }
    }
    public string DimAsText
    {
      get
      {
        return "Height:" + this.Dim.a + " Width:" + this.Dim.b;
      }
    }

    /// <summary>
    /// toggle whether to show the screen items of this section in the screen
    /// items control.
    /// </summary>
    public bool ExpandItems
    {
      get { return _ExpandItems; }
      set
      {
        if (_ExpandItems != value)
        {
          _ExpandItems = value;
          RaisePropertyChanged("ExpandItems");
        }
      }
    }
    private bool _ExpandItems;

    private bool _IsExpanded;

    public bool IsExpanded
    {
      get { return _IsExpanded; }
      set
      {
        if (_IsExpanded != value)
        {
          _IsExpanded = value;
          RaisePropertyChanged("IsExpanded");
        }
      }
    }

    public ObservableCollection<IScreenItem> Items
    {
      get { return _Items; }
      set
      {
        _Items = value;
        _Items.CollectionChanged += _Items_CollectionChanged;
      }
    }
    ObservableCollection<IScreenItem> _Items;

    private void _Items_CollectionChanged(
      object sender, NotifyCollectionChangedEventArgs e)
    {
      RaisePropertyChanged("InfoText");
    }

    IList<IScreenItem> IScreenSection.Items
    {
      get { return this.Items; }
      set { this.Items = value.ToObservableCollection(); }
    }
    IList<IScreenItem> ISectionHeader.Items
    {
      get { return this.Items; }
    }

    public ScreenPurposeCode? PurposeCode
    {
      get
      {
        if (_PurposeCode == null)
          PurposeCode = ScreenPurposeCode.none;
        return _PurposeCode;
      }
      set
      {
        if (_PurposeCode != value)
        {
          _PurposeCode = value;
          RaisePropertyChanged("PurposeCode");
        }
      }
    }
    private ScreenPurposeCode? _PurposeCode;


    public string AssocSectionName
    {
      get { return _AssocSectionName; }
      set
      {
        if (_AssocSectionName != value)
        {
          _AssocSectionName = value;
          RaisePropertyChanged("AssocSectionName");
        }
      }
    }
    private string _AssocSectionName;

    public int RepeatCount
    {
      get { return _RepeatCount; }
      set
      {
        if (_RepeatCount != value)
        {
          _RepeatCount = value;
          RaisePropertyChanged("RepeatCount");
          RaisePropertyChanged("InfoText");
        }
      }
    }
    private int _RepeatCount;

    public string InfoText
    {
      get
      {
        var sb = new StringBuilder();
        sb.Append("items:" + this.Items.Count);
        if (this.RepeatCount > 1)
          sb.Append(" Repeat:" + this.RepeatCount);

        var dim = this.RepeatDim;
        sb.Append(" Dim:" + dim.a + "x" + dim.b);

        return sb.ToString();
      }
    }

    public override void Apply(IScreenItem Source)
    {
      base.Apply(Source);
      var source = Source as IScreenSection;
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
      this.Items = new ObservableCollection<IScreenItem>();
      var jj = from a in source.Items
               select ScreenItemModel.Factory(a) as IScreenItem;
      this.LoadItems(jj);
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
      return "seed. " + this.ClassName() + ". " + ToText();
    }
    public override string ToText()
    {
      return " ScreenLoc:" + this.ScreenLoc.ValueText();
    }

    public override string ClassName()
    {
      return "ScreenSection";
    }
  }
}
