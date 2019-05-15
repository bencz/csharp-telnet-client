using AutoCoder.Enums;
using AutoCoder.Ext;
using AutoCoder.Ext.System;
using AutoCoder.Systm;
using AutoCoder.Telnet.Common;
using AutoCoder.Telnet.Common.ScreenDm;
using AutoCoder.Telnet.Common.ScreenLoc;
using AutoCoder.Telnet.Enums;
using ScreenDefnLib.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace ScreenDefnLib.Defn
{
  public abstract class ScreenItem : IScreenItem, IToText
  {
    public ScreenItem( ShowItemType ItemType)
    {
      this.ItemType = ItemType;
    }

    public ScreenItem( IScreenItem source)
    {
      this.ItemType = source.ItemType;
      this.Apply(source);
    }

    public CopyPasteCode? CopyPasteCode
    { get; set; }

    /// <summary>
    /// the section that contains this item. Same as a parent item. Use this
    /// property to walk up the item/parent hierarchy to the IScreenDefn of the
    /// item.
    /// </summary>
    public ISectionHeader SectionHeader { get; set; }

    public string ItemName
    {
      get
      {
        return _ItemName;
      }
      set
      {
        if (value == null)
          _ItemName = "";
        else
          _ItemName = value.TrimEndWhitespace();
      }
    }
    private string _ItemName;

    public ShowItemType ItemType { get; set; }

    /// <summary>
    /// match this item to inbound data stream when determine the screen name of
    /// the data stream.
    /// </summary>
    public int MatchNum { get; set; }

    private string _ItemGuid;
    public string ItemGuid
    {
      get
      {
        if (_ItemGuid == null)
          this.AssignItemGuid();
        return _ItemGuid;
      }
      set { _ItemGuid = value; }
    }

    /// <summary>
    /// the c# code to run when the mouse hovers over the screen item.
    /// </summary>
    public string[] HoverCode
    {
      get { return _HoverCode; }
      set { _HoverCode = value; }
    }
    private string[] _HoverCode;

    /// <summary>
    /// the xaml code that draws the content of the hover window. This xaml binds
    /// to the object returned by the HoverCode.
    /// </summary>
    public string[] HoverXaml
    {
      get { return _HoverXaml; }
      set { _HoverXaml = value; }
    }
    private string[] _HoverXaml;

    public OneScreenLoc ScreenLoc
    {
      get
      {
        return _ScreenLoc;
      }
      set
      {
        _ScreenLoc = value;
      }
    }
    private OneScreenLoc _ScreenLoc;
    public bool IsOptional
    { get; set; }

    public virtual void Apply(IScreenItem source)
    {
      Apply_Actual(source);
    }
    void Apply_Actual(IScreenItem source)
    {
      this.ItemName = source.ItemName;
      this.ItemGuid = source.ItemGuid;
      this.MatchNum = source.MatchNum;
      this.ScreenLoc = source.ScreenLoc;
      this.HoverCode = source.HoverCode;
      this.IsOptional = source.IsOptional;
    }

    public virtual void ApplyMatch(IScreenItem source)
    {
      this.Apply_Actual(source);
    }

    public static IScreenItem Factory(ShowItemType itemType)
    {
      IScreenItem item = null;

      if ((itemType == ShowItemType.Field)
        || (itemType == ShowItemType.StaticLit)
        || (itemType == ShowItemType.VarLit))
        item = new ScreenField();
      else if ((itemType == ShowItemType.Literal) || (itemType == ShowItemType.Static))
        item = new ScreenLiteral();
      else if (itemType == ShowItemType.Section)
        item = new ScreenSection();
      else
        throw new Exception("unsupported itemType");

      return item;
    }

    /// <summary>
    /// construct a new ScreenItem with the specific itemType. Then apply properties
    /// from the source item.
    /// </summary>
    /// <param name="itemType"></param>
    /// <param name="source"></param>
    /// <returns></returns>
    public static IScreenItem Factory(ShowItemType itemType, IScreenItem source)
    {
      var item = Factory(itemType);
      item.ApplyMatch(source);
      return item;

      if ( item is ScreenAtomic)
      {
        var atomic = item as ScreenAtomic;
        if (source is IScreenAtomic)
        {
          atomic.Apply(source as IScreenAtomic);
        }
        else
        {
          (item as ScreenItem).ApplyMatch(source as IScreenItem);
        }
      }

      else if (item is ScreenSection)
      {
        var section = item as ScreenSection;
        if ( source is IScreenSection)
        {
          section.Apply(source);
        }
        else
        {
          (section as ScreenItem).ApplyMatch(source);
        }
      }

      return item;
    }

    public static IScreenItem Factory(IScreenItem source)
    {
      IScreenItem item = Factory(source.ItemType, source);
      return item;
    }

    public override string ToString()
    {
      return this.ClassName( ) + ". " + ToText();
    }
    public virtual string ToText( )
    {
      return "ItemType:" + this.ItemType.ToString() +
        " ScreenLoc:" + this.ScreenLoc.ValueText();
    }

    public virtual string ClassName( )
    {
      return "ScreenItem";
    }
  }

  public static class ScreenItemExt
  {

    public static ScreenAtomic FindFieldItem(
      this IEnumerable<ScreenAtomic> List, IScreenLoc ScreenLoc, int Length)
    {
      ScreenAtomic found = null;
      var dim = new ScreenDim(24, 80);
      var range = new ScreenLocRange(ScreenLoc, Length, dim);
      foreach (var item in List)
      {
        if (item.ItemType == ShowItemType.Field)
        {
          if (item.ScreenLocRange.CompletelyContains(range) == true)
          {
            found = item;
            break;
          }
        }
      }
      return found;
    }
  }
}
