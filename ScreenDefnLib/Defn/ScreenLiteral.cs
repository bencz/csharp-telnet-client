using AutoCoder.Ext.System;
using AutoCoder.Telnet.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScreenDefnLib.Defn
{
  public class ScreenLiteral : ScreenAtomic, IScreenLiteral
  {
    public ScreenLiteral( )
      : base(ShowItemType.Literal)
    {
    }

    public ScreenLiteral(IScreenItem source)
      : base(source.ItemType)
    {
      this.Apply(source);
    }

    public string Value
    {
      get
      {
        if (this.ListValues.Count == 0)
          return "";
        else
          return this.ListValues[0];
      }
#if skip
      set
      {
        return;
        _Value = value;
        if (_ListValues == null)
          _ListValues = new List<string>();
        _ListValues.Clear();
        _ListValues.Add(_Value);
        return;
        LoadValue(value);
        SetNumValues();
      }
#endif
    }
    //    private string _Value;

#if skip
    void LoadValue(string value)
    {
      if (value == null)
        _Value = "";
      else
        _Value = value.TrimEndWhitespace();
    }
#endif

    /// <summary>
    /// list of allowed values. If not null, then match screen value to any of
    /// this list of values. Otherwise, match to the single Value.
    /// </summary>
    public List<string> ListValues
    {
      get
      {
        if (_ListValues == null)
          _ListValues = new List<string>();
        return _ListValues;
      }
      set
      {
        if (value == null)
          _ListValues = new List<string>();
        else
        {
          _ListValues = new List<string>();
          foreach (var v in value)
          {
            _ListValues.Add(v.TrimEndWhitespace());
          }
        }

        SetNumValues();
      }
    }
    List<string> _ListValues;

    public int NumValues { get; private set; }

    IList<string> IScreenLiteral.ListValues
    {
      get
      {
        return this.ListValues;
      }
    }

    void SetNumValues()
    {
      this.NumValues = this.ListValues.Count;
    }

    public override void Apply(IScreenItem sourceItem)
    {
      var source = sourceItem as IScreenLiteral;
      base.Apply(source);
//      this.Value = source.Value;
      this.ListValues = source.ListValues.ToList();
    }
    public override void ApplyMatch(IScreenItem sourceItem)
    {
      base.ApplyMatch(sourceItem);
      if (sourceItem is IScreenLiteral)
      {
        var source = sourceItem as IScreenLiteral;
//        this.Value = source.Value;
        this.ListValues = source.ListValues.ToList();
      }
    }

    public override string ToText()
    {
      var s1 = base.ToText();
      s1 = s1 + " Value:" + this.Value;
      return s1;
    }

    public override string ClassName()
    {
      return "ScreenLiteral";
    }
  }
}
