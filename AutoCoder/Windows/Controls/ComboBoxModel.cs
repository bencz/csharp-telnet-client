using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using AutoCoder.Text;
using AutoCoder.Ext;
using AutoCoder.Ext.System;
using System.Collections.ObjectModel;
using System.Xml.Linq;
using System.Diagnostics;
using AutoCoder.Core;

namespace AutoCoder.Windows.Controls
{
  public class ComboBoxModel : INotifyPropertyChanged 
  {

    public ComboBoxModel()
    {
      this.AcceptBlanks = false;
    }

    public ComboBoxModel( bool ApplyRecentToTop)
      : this( )
    {
      this.ApplyRecentToTop = ApplyRecentToTop;
    }

    public ComboBoxModel(
      bool ApplyRecentToTop, bool AcceptBlanks, 
      string Value, IEnumerable<string> ValuesList)
    {
      this.ApplyRecentToTop = ApplyRecentToTop;
      this.AcceptBlanks = AcceptBlanks;
      this.Value = Value;

      this.ValuesList.Clear();
      foreach (var item in ValuesList)
      {
        this.ValuesList.Add(item);
      }

      // make sure the Value is in the ValuesList.
      if (this.ApplyRecentToTop == true)
      {
        var fs = this.ValuesList.FirstOrDefault(c => c == this.Value);
        if (fs == null)
        {
          this.ValuesList.ApplyRecent(this.Value);
        }
      }
    }

    public void ApplyToValuesList(string Value)
    {
      this.ValuesList.ApplyRecent(Value);
    }

    /// <summary>
    /// apply recently changed values to the top of the dropdown values list.
    /// </summary>
    public bool ApplyRecentToTop
    {
      get;
      set;
    }

    /// <summary>
    /// Ignore when Value is set to null or blanks. By default the value is false. 
    /// </summary>
    public bool AcceptBlanks
    { get; set; }

    public event PropertyChangedEventHandler PropertyChanged;

    /// <summary>
    /// Fires the PropertyChanged event.
    /// </summary>
    /// <param name="propertyName">The name of the changed property.</param>
    protected void RaisePropertyChanged(string propertyName)
    {
      if (PropertyChanged != null)
      {
        var e = new PropertyChangedEventArgs(propertyName);
        PropertyChanged(this, e);
      }
    }

    private ActionCommand _DeletePressedCommand = null;
    public ActionCommand DeletePressedCommand
    {
      get
      {
        if (_DeletePressedCommand == null)
        {
          _DeletePressedCommand = new ActionCommand(
            c =>
            {
              var tp = c.GetType( ) ;
              var s1 = this.Value ;
              var fx = this.ValuesList.FirstOrDefault( x => x == s1) ;
              if ( fx != null )
              {
                this.ValuesList.Remove(fx) ;
                if ( this.ValuesList.Count == 0 )
                  this.Value = "" ;
                else
                  this.Value = this.ValuesList[0] ;
              }
            });
        }
        return _DeletePressedCommand;
      }
    }

    string _Selected;
    public string Selected
    {
      get
      {
        if ((_Selected == null) && (this.ValuesList.Count > 0))
          _Selected = this.ValuesList[0];
        return _Selected;
      }
      set
      {
        if ((value != null) && (value != _Selected))
        {
          _Selected = value;
          this.Value = value;
          RaisePropertyChanged("Selected");
        }
      }
    }

    string _Value;
    public string Value
    { 
      get
      { 
        return _Value; 
      }
      set
      {
        string s1 = value.TrimWhitespace();

        // skip out if the value is blank.
        if ((this.AcceptBlanks == false) && (s1.IsNullOrEmpty()))
        {
        }
        else
        {
          if (_Value != s1)
          {
            _Value = s1;
            RaisePropertyChanged("Value");

            if (this.ApplyRecentToTop == true)
            {
              this.ValuesList.ApplyRecent(this.Value);
            }

            // signal the change to this specific property.
            if (this.ValueChanged != null)
            {
              this.ValueChanged(this.Value);
            }
          }
        }
      }
    }

    public event Action<string> ValueChanged;

    private ObservableCollection<string> _ValuesList;
    public ObservableCollection<string> ValuesList
    {
      get
      {
        if (_ValuesList == null)
        {
          _ValuesList = new ObservableCollection<string>();
        }
        return _ValuesList;
      }
      set
      {
        _ValuesList = value;
      }
    }
  }

  public static class ComboBoxModelExt
  {
    public static bool IsNullOrEmpty(this ComboBoxModel Model)
    {
      if (Model == null)
        return true;
      else if (Model.ValuesList.Count == 0)
        return true;
      else
        return false;
    }

    public static ComboBoxModel ToComboBoxModel(this XElement Elem, XNamespace Namespace)
    {
      ComboBoxModel combo = null;
      if (Elem != null)
      {
        var Value = Elem.Element(Namespace + "Value").StringOrDefault();
        var toTop =
          Elem.Element(Namespace + "ApplyRecentToTop").BooleanOrDefault(false).Value;
        var acceptBlanks =
          Elem.Element(Namespace + "AcceptBlanks").BooleanOrDefault(false).Value;
        var vl =
          Elem.Element(Namespace + "ValuesList").ObservableCollectionStringOrDefault(Namespace);
        combo = new ComboBoxModel(toTop, acceptBlanks, Value, vl) ;
      }
      return combo;
    }

    public static ComboBoxModel ToComboBoxModel(
      this XElement Parent, XNamespace Namespace, string ElemName)
    {
      ComboBoxModel combo = null;
      var elem = Parent.Element(Namespace + ElemName);
      if (elem == null)
        combo = null;
      else
        combo = elem.ToComboBoxModel(Namespace);
      return combo;
    }

    public static XElement ToXElement(this ComboBoxModel Combo, XName Name)
    {
      if (Combo == null)
        return new XElement(Name, null);
      else
      {
        XElement xe = new XElement(Name,
            new XElement("Value", Combo.Value),
            new XElement("ApplyRecentToTop", Combo.ApplyRecentToTop),
            new XElement("AcceptBlanks", Combo.ApplyRecentToTop),
            Combo.ValuesList.ToXElement("ValuesList")
            ) ;
        return xe;
      }
    }
  }
}

