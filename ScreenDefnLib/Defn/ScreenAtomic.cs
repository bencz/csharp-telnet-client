using AutoCoder.Ext;
using AutoCoder.Ext.System;
using AutoCoder.Telnet.Common;
using AutoCoder.Telnet.Common.RowCol;
using AutoCoder.Telnet.Common.ScreenDm;
using AutoCoder.Telnet.Common.ScreenLoc;
using AutoCoder.Telnet.Enums;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Xml.Linq;

namespace ScreenDefnLib.Defn
{
  public class ScreenAtomic : ScreenItem, IScreenAtomic
  {
    public ScreenAtomic( ShowItemType ItemType)
      : base(ItemType)
    {
    }

    public ScreenAtomic(IScreenItem Item)
      : base(Item.ItemType)
    {
      this.Apply(Item);
    }

    public ScreenAtomic(ShowItemType ItemType, IScreenItem Item)
      : base(ItemType)
    {
      if (Item is IScreenAtomic)
      {
        this.Apply(Item as IScreenAtomic);
        this.ItemType = ItemType;
      }
      else
      {
        base.Apply(Item);
        this.ItemType = ItemType;
      }
    }

    public int Length { get; set; }

    public ScreenLocRange ScreenLocRange
    {
      get
      {
        return new ScreenLocRange(
          this.ScreenLoc, this.Length, new ScreenDim(24,80));
      }
    }

    private DsplyAttr[] _DsplyAttr;
    public DsplyAttr[] DsplyAttr
    {
      get
      {
        if (_DsplyAttr == null)
          _DsplyAttr = new DsplyAttr[0];
        return _DsplyAttr;
      }
      set { _DsplyAttr = value; }
    }

    public override void Apply(IScreenItem Source)
    {
      var source = Source as IScreenAtomic;
      base.Apply(source);
      this.Length = source.Length;
      this.DsplyAttr = source.DsplyAttr;
    }
    public override void ApplyMatch(IScreenItem Source)
    {
      base.ApplyMatch(Source);
      if ( Source is IScreenAtomic)
      {
        var source = Source as IScreenAtomic;
        this.Length = source.Length;
        this.DsplyAttr = source.DsplyAttr;
      }
    }
    public override string ToText()
    {
      var s1 = base.ToText();
      if (this.Length > 0)
      {
        s1 = s1 + " Length:" + this.Length;
      }

      if ((this.DsplyAttr != null) && ( this.DsplyAttr.Length > 0))
      {
        s1 = s1 + "DsplyAttr:" + this.DsplyAttr.ToDsplyAttrText();
      }
      return s1;
    }

    public override string ClassName()
    {
      return "ScreenAtomic";
    }
  }
}
