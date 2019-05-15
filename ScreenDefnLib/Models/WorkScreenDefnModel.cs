using AutoCoder.ComponentModel;
using AutoCoder.Enums;
using AutoCoder.Ext;
using AutoCoder.Telnet.Common.ScreenDm;
using ScreenDefnLib.Defn;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScreenDefnLib.Models
{
  public class WorkScreenDefnModel : ModelBase
  {
    public WorkScreenDefnModel(ActionCode WorkMode)
      : this(WorkMode, "", "", new ScreenDimModel(new ScreenDim(24,80)), new List<IScreenItem>( ))
    {
    }
    public WorkScreenDefnModel(ActionCode WorkMode, IScreenDefn defn)
      : this(WorkMode, defn.ScreenName, defn.NamespaceName, defn.ScreenDim, defn.Items)
    {
    }
    public WorkScreenDefnModel(WorkScreenDefnModel defn)
      : this(defn.WorkMode, defn.ScreenName, defn.NamespaceName, defn.ScreenDim, defn.ModelItems)
    {
    }
    public WorkScreenDefnModel(
      ActionCode? WorkMode,
      string ScreenName, string NamespaceName, IScreenDim ScreenDim, IEnumerable<IScreenItem> Items)
    {
      this.WorkMode = WorkMode;
      this.ScreenName = ScreenName;
      this.NamespaceName = NamespaceName;
      this.ScreenDim = new ScreenDimModel(ScreenDim);
      if (Items != null)
        this.ModelItems = Items.ToObservableCollection();
      else
        this.ModelItems = new ObservableCollection<IScreenItem>();
    }

    private ActionCode? _WorkMode;
    public ActionCode? WorkMode
    {
      get { return _WorkMode; }
      set
      {
        _WorkMode = value;
        RaisePropertyChanged("ScreenTitle");
      }
    }

    public string ScreenTitle
    {
      get
      {
        string title = "Screen Definition";
        if (this.WorkMode != null)
          title = this.WorkMode.Value.ToString() + " " + title;
        return title;
      }
    }
    public string ScreenGuid { get; set; }

    private string _ScreenName;
    public string ScreenName
    {
      get { return _ScreenName; }
      set
      {
        if (_ScreenName != value)
        {
          _ScreenName = value;
          RaisePropertyChanged("ScreenName");
        }
      }
    }

    private string _NamespaceName;

    public string NamespaceName
    {
      get { return _NamespaceName; }
      set
      {
        if (_NamespaceName != value)
        {
          _NamespaceName = value;
          RaisePropertyChanged("NamespaceName");
        }
      }
    }

    public IScreenDim ScreenDim
    {
      get { return _ScreenDim; }
      set
      {
        if (_ScreenDim != value)
        {
          _ScreenDim = value;
          RaisePropertyChanged("ScreenDim");
        }
      }
    }
    private IScreenDim _ScreenDim;

    public ObservableCollection<IScreenItem> ModelItems
    { get; set; }

    public void Apply(WorkScreenDefnModel Model)
    {
      Apply(Model.ScreenName, Model.NamespaceName, Model.ScreenGuid,
        Model.ScreenDim, Model.ModelItems);
    }

    public void Apply(
      string ScreenName, string NamespaceName, string ScreenGuid,
      IScreenDim ScreenDim, IEnumerable<IScreenItem> ModelItems)
    {
      this.ScreenName = ScreenName;
      this.NamespaceName = NamespaceName;
      this.ScreenGuid = ScreenGuid;
      this.ScreenDim = ScreenDim;
      this.ModelItems = ModelItems.ToObservableCollection();
    }

    /// <summary>
    /// convert screenDefn model class to ScreenDefn class.
    /// </summary>
    /// <returns></returns>
    public ScreenDefn ToScreenDefn()
    {
      var defn = new ScreenDefn(
        this.ScreenName, this.NamespaceName, this.ScreenGuid, this.ScreenDim, this.ModelItems);

      var jj = from a in this.ModelItems
               select ScreenItem.Factory(a);
      defn.Items = jj.ToList();

      return defn;
    }
  }
}
