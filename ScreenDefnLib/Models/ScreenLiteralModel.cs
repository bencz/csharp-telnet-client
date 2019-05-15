using AutoCoder.Ext;
using AutoCoder.Telnet.Enums;
using ScreenDefnLib.Defn;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScreenDefnLib.Models
{
  public class ScreenLiteralModel : ScreenAtomicModel, IScreenLiteral
  {
    public ScreenLiteralModel( )
      : base(ShowItemType.Literal)
    {

    }
    public ScreenLiteralModel(IScreenItem Source)
      : base(Source)
    {
      this.Apply(Source);
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
          _ListValues = new ObservableCollection<string>();
        _ListValues.Clear();
        _ListValues.Add(_Value);
      }
#endif

    }
//    string _Value;

    private ObservableCollection<string> _ListValues;

    public ObservableCollection<string> ListValues
    {
      get
      {
        if (_ListValues == null)
        {
          _ListValues = new ObservableCollection<string>();
          SetListValuesChangeHandler();
        }
        return _ListValues;
      }
      set
      {
        if (_ListValues != value)
        {
          UnhookListValuesChangeHandler();

          _ListValues = value;
          SetListValuesChangeHandler();

          RaisePropertyChanged("ListValues");
          RaisePropertyChanged("Value");
        }
      }
    }

    void SetListValuesChangeHandler( )
    {
      if (_ListValues != null)
      {
        _ListValues.CollectionChanged += ListValues_CollectionChanged;
      }
    }

    void UnhookListValuesChangeHandler( )
    {
      if ( _ListValues != null)
      {
        _ListValues.CollectionChanged -= ListValues_CollectionChanged;
      }
    }

    private void ListValues_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
    {
      RaisePropertyChanged("Value");

      // update length
      var curLength = this.Length;
      int maxLength = 0;
      foreach( var item in this.ListValues)
      {
        maxLength = Math.Max(maxLength, item.Length);
      }
      if (this.Length != maxLength)
        this.Length = maxLength;
    }

    IList<string> IScreenLiteral.ListValues
    {
      get
      {
        return this.ListValues;
      }
    }

    public override void Apply(IScreenItem Item)
    {
      base.Apply(Item);

      if (Item is IScreenLiteral)
      {
        var item = Item as IScreenLiteral;
        this.ListValues = item.ListValues.ToObservableCollection();
      }
    }

    /// <summary>
    /// apply source to target where the types match.
    /// </summary>
    /// <param name="sourceItem"></param>
    public override void ApplyMatch(IScreenItem sourceItem)
    {
      base.ApplyMatch(sourceItem);
      if (sourceItem is IScreenLiteral)
      {
        var source = sourceItem as IScreenLiteral;
        this.ListValues = source.ListValues.ToObservableCollection();
      }
    }

    public override string ToString()
    {
      return this.ClassName() + ". " + ToText();
    }
    public override string ToText()
    {
      return base.ToText() + " vlu:" + this.Value;
    }
    public override string ClassName()
    {
      return "ScreenLiteralModel";
    }
  }
}
