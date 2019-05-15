using AutoCoder.Telnet.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScreenDefnLib.Defn
{
  public class ScreenField : ScreenAtomic, IScreenField
  {
    public ScreenField( )
      : base(ShowItemType.Field)
    {
    }

    public ScreenField(IScreenItem source)
      : base(ShowItemType.Field)
    {
      this.Apply(source);
    }

    public ShowUsage Usage
    {
      get
      {
        if (_Usage == ShowUsage.none)
          _Usage = ShowUsage.Output;
        return _Usage;
      }
      set { _Usage = value; }
    }
    private ShowUsage _Usage;

    public override void Apply(IScreenItem sourceItem)
    {
      var source = sourceItem as IScreenField;
      base.Apply(source);
      this.Usage = source.Usage;
    }
    public override void ApplyMatch(IScreenItem sourceItem)
    {
      base.ApplyMatch(sourceItem);
      if (sourceItem is IScreenField)
      {
        var source = sourceItem as IScreenField;
        this.Usage = source.Usage;
      }
    }
    public override string ToText()
    {
      var s1 = base.ToText();
      s1 = s1 + " Usage:" + this.Usage.ToString();
      return s1;
    }

    public override string ClassName()
    {
      return "ScreenField";
    }


  }
}
