using AutoCoder.Telnet.Enums;
using ScreenDefnLib.Defn;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScreenDefnLib.Models
{
  public class ScreenFieldModel : ScreenAtomicModel, IScreenField
  {
    public ScreenFieldModel( )
      :base(ShowItemType.Field)
    {
    }
    public ScreenFieldModel(IScreenItem Source)
      : base(Source)
    {
      this.Apply(Source);
    }


    // BgnTemp
    public string Value
    {
      get { return "jess"; }
      set { var xx = value; }
    }
    // EndTemp

    public ShowUsage Usage
    {
      get { return _Usage; }
      set
      {
        if (_Usage != value)
        {
          _Usage = value;
          RaisePropertyChanged("Usage");
          RaisePropertyChanged("UsageInput");
          RaisePropertyChanged("UsageOutput");
          RaisePropertyChanged("UsageBoth");
        }
      }
    }
    ShowUsage _Usage;

    public bool UsageInput
    {
      get
      {
        return (this.Usage == ShowUsage.Input);
      }
      set
      {
        this.Usage = ShowUsage.Input;
      }
    }
    public bool UsageOutput
    {
      get
      {
        return (this.Usage == ShowUsage.Output);
      }
      set
      {
        this.Usage = ShowUsage.Output;
      }
    }
    public bool UsageBoth
    {
      get
      {
        return (this.Usage == ShowUsage.Both);
      }
      set
      {
        this.Usage = ShowUsage.Both;
      }
    }

    public override void Apply(IScreenItem Item)
    {
      base.Apply(Item);

      if (Item is IScreenField)
      {
        var item = Item as IScreenField;
        this.Usage = item.Usage;
      }
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
  }
}
