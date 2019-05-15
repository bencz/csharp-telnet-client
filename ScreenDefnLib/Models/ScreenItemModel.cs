using AutoCoder.ComponentModel;
using AutoCoder.Enums;
using AutoCoder.Systm;
using AutoCoder.Telnet.Common.ScreenLoc;
using AutoCoder.Telnet.Enums;
using ScreenDefnLib.Defn;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ScreenDefnLib.Models
{
  /// <summary>
  /// base class of ScreenAtomicModel and ScreenSectionModel. The idea is to
  /// mirror the class hierarchy of ScreenItem, ScreenAtomic, ScreenSection,
  /// etc.
  /// </summary>
  public abstract class ScreenItemModel : ModelBase, IScreenItem, IToText
  {
    public ScreenItemModel(ShowItemType ItemType)
    {
      this.ItemType = ItemType;
    }
    public ScreenItemModel(IScreenItem Item)
      : this(Item.ItemType)
    {
      this.Apply(Item);
    }

    public int MatchNumWidth
    { get { return 70; } }

    public string[] HoverCode
    {
      get { return _HoverCode; }
      set
      {
        if (_HoverCode != value)
        {
          _HoverCode = value;
          RaisePropertyChanged("HoverCode");
          RaisePropertyChanged("HoverCodeText");
        }
      }
    }
    private string[] _HoverCode;

    public string HoverCodeText
    {
      get
      {
        var sb = new StringBuilder();
        foreach (var s1 in this.HoverCode)
        {
          sb.Append(s1 + Environment.NewLine);
        }
        return sb.ToString();
      }
      set
      {
        this.HoverCode = value.Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
        RaisePropertyChanged("HoverCodeText");
        RaisePropertyChanged("HoverCode");
      }
    }
    public string[] HoverXaml
    {
      get { return _HoverXaml; }
      set
      {
        if (_HoverXaml != value)
        {
          _HoverXaml = value;
          RaisePropertyChanged("HoverXaml");
          RaisePropertyChanged("HoverXamlText");
        }
      }
    }
    private string[] _HoverXaml;

    public string HoverXamlText
    {
      get
      {
        var sb = new StringBuilder();
        foreach (var s1 in this.HoverXaml)
        {
          sb.Append(s1 + Environment.NewLine);
        }
        return sb.ToString();
      }
      set
      {
        this.HoverXaml = value.Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
        RaisePropertyChanged("HoverXamlText");
        RaisePropertyChanged("HoverXaml");
      }
    }

    public int ItemTypeWidth
    { get { return 70; } }
    public int ItemNameWidth
    { get { return 150; } }

    public bool IsOptional
    {
      get { return _IsOptional; }
      set
      {
        if (_IsOptional != value)
        {
          _IsOptional = value;
          RaisePropertyChanged("IsOptional");
        }
      }
    }
    private bool _IsOptional;

    public string ItemName
    {
      get { return _ItemName; }
      set
      {
        if (_ItemName != value)
        {
          _ItemName = value;
          RaisePropertyChanged("ItemName");
        }
      }
    }
    string _ItemName;

    public ShowItemType ItemType
    {
      get { return _ItemType; }
      set
      {
        if (_ItemType != value)
        {
          _ItemType = value;
          RaisePropertyChanged("ItemType");
        }
      }
    }
    ShowItemType _ItemType;

    public string ItemGuid { get; set; }

    public OneScreenLoc ScreenLoc
    {
      get
      {
        if (_ScreenLoc == null)
          _ScreenLoc = new OneScreenLoc(1, 1);
        return _ScreenLoc;
      }
      set
      {
        if (_ScreenLoc != value)
        {
          _ScreenLoc = value;
          RaisePropertyChanged("ScreenLoc");
          RaisePropertyChanged("RowColText");
          RaisePropertyChanged("RowNum");
          RaisePropertyChanged("ColNum");
        }
      }
    }
    OneScreenLoc _ScreenLoc;

    public int RowNum
    {
      get { return this.ScreenLoc.RowNum; }
      set
      {
        var screenLoc = this.ScreenLoc.NewInstance(value, this.ScreenLoc.ColNum);
        this.ScreenLoc = screenLoc as OneScreenLoc;
      }
    }
    public int ColNum
    {
      get
      {
        return this.ScreenLoc.ColNum;
      }
      set
      {
        var screenLoc = this.ScreenLoc.NewInstance(this.ScreenLoc.RowNum, value);
        this.ScreenLoc = screenLoc as OneScreenLoc;
      }
    }


    /// <summary>
    /// pending cut, copy, paste option that applies to this item.
    /// </summary>
    public CopyPasteCode? CopyPasteCode
    {
      // considered using linq to join to ScreenDefnGlobal.CopyPasteList to
      // determine if Copy/Paste is pending for a screen item. But problem is
      // that there is nothing to RaisePropertyChanged when the CopyPasteList is
      // cleared. Or when item added to CopyPasteList how to signal that this
      // property has changed?
      get { return _CopyPasteCode; }
      set
      {
        if (_CopyPasteCode != value)
        {
          _CopyPasteCode = value;
          RaisePropertyChanged("CopyPasteCode");
          RaisePropertyChanged("CopyPasteVisible");
          RaisePropertyChanged("MarkedCutOrCopy");
        }
      }
    }
    private CopyPasteCode? _CopyPasteCode;

    public ISectionHeader SectionHeader { get; set; }

    public Visibility CopyPasteVisible
    {
      get
      {
        if (this.CopyPasteCode == null)
          return Visibility.Hidden;
        else
          return Visibility.Visible;
      }
    }

    public bool MarkedCutOrCopy
    {
      get
      {
        if (this.CopyPasteCode == null)
          return false;
        else if (this.CopyPasteCode.Value == AutoCoder.Enums.CopyPasteCode.Copy)
          return true;
        else if (this.CopyPasteCode.Value == AutoCoder.Enums.CopyPasteCode.Cut)
          return true;
        else
          return false;
      }
    }

    public string RowColText
    {
      get
      {
        if (this.ScreenLoc == null)
          return "";
        else
          return this.ScreenLoc.ValueText();
      }
    }

    public int MatchNum
    {
      get { return _MatchNum; }
      set
      {
        if (_MatchNum != value)
        {
          _MatchNum = value;
          RaisePropertyChanged("MatchNum");
        }
      }
    }
    int _MatchNum;

    public virtual void Apply(IScreenItem Item)
    {
      this.ItemName = Item.ItemName;
      this.ItemGuid = Item.ItemGuid;
      this.ItemType = Item.ItemType;
      this.MatchNum = Item.MatchNum;
      this.ScreenLoc = Item.ScreenLoc;
      this.HoverCode = Item.HoverCode;
      this.HoverXaml = Item.HoverXaml;
      this.IsOptional = Item.IsOptional;
    }

    public virtual void ApplyMatch(IScreenItem source)
    {
      this.Apply(source);
    }

    public static ScreenItemModel Factory(IScreenItem source)
    {
      ScreenItemModel model = null;
      if (source is IScreenField)
      {
        model = new ScreenFieldModel(source);
      }
      else if (source is IScreenLiteral)
      {
        model = new ScreenLiteralModel(source);
      }
      else if (source is IScreenSection)
      {
        model = new ScreenSectionModel(source as IScreenSection);
      }
      return model;
    }
    public static ScreenItemModel Factory(ShowItemType ItemType,  IScreenItem source)
    {
      var item = ScreenItem.Factory(ItemType, source);
      var model = ScreenItemModel.Factory(item);
      return model;
    }

    public override string ToString()
    {
      return this.ClassName() + ". " + ToText();
    }
    public virtual string ToText()
    {
      return "ItemType:" + this.ItemType.ToString() +
        " ScreenLoc:" + this.ScreenLoc.ValueText();
    }

    public virtual string ClassName()
    {
      return "ScreenItem";
    }
  }
}
