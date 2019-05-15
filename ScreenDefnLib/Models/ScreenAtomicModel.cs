using AutoCoder.ComponentModel;
using AutoCoder.Telnet.Common.ScreenLoc;
using AutoCoder.Telnet.Enums;
using ScreenDefnLib.Defn;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScreenDefnLib.Models
{
  public abstract class ScreenAtomicModel : ScreenItemModel, IScreenAtomic
  {
    public ScreenAtomicModel(ShowItemType ItemType)
      : base(ItemType)
    {
    }
    public ScreenAtomicModel(IScreenItem Item)
      : base(Item)
    {
      this.Apply(Item);
    }

    public DsplyAttr[] DsplyAttr
    {
      get { return _DsplyAttr; }
      set
      {
        if (_DsplyAttr != value)
        {
          _DsplyAttr = value;
          RaisePropertyChanged("DsplyAttr");
          RaisePropertyChanged("DsplyAttrText");
        }
      }
    }
    DsplyAttr[] _DsplyAttr;

    public string DsplyAttrText
    {
      get
      {
        return DsplyAttr.ToDsplyAttrText();
      }
      set
      {
        this.DsplyAttr = DsplyAttrExt.ParseArray(value);
      }
    }


    public int Length
    {
      get { return _Length; }
      set
      {
        if (_Length != value)
        {
          _Length = value;
          RaisePropertyChanged("Length");
        }
      }
    }
    int _Length;

    public override void Apply(IScreenItem Source)
    {
      base.Apply(Source);

      if ( Source is IScreenAtomic)
      {
        var source = Source as IScreenAtomic;
        this.Length = source.Length;
        this.DsplyAttr = source.DsplyAttr;
      }
    }

    public override void ApplyMatch(IScreenItem Source)
    {
      base.ApplyMatch(Source);
      if (Source is IScreenAtomic)
      {
        var source = Source as IScreenAtomic;
        this.Length = source.Length;
        this.DsplyAttr = source.DsplyAttr;
      }
    }
  }

  public static class ScreenItemModelExt
  {
    public static IEnumerable<IScreenItem> ToScreenItemList(this IEnumerable<ScreenAtomicModel> ModelList)
    {
      var list = new List<IScreenItem>();
      foreach (var model in ModelList)
      {
        var item = ScreenItem.Factory(model);
        list.Add(item);
      }
      return list;
    }
  }
}

