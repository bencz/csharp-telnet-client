using AutoCoder.ComponentModel;
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
  public class ScreenDefnModel : ModelBase, IScreenDefn, ISectionHeader
  {
    public ScreenDefnModel( IScreenDefn defn)
      : this(defn.ScreenName, defn.NamespaceName, defn.ScreenGuid, defn.ScreenDim, defn.Items )
    {
    }

    public ScreenDefnModel(
      string ScreenName, string NamespaceName, string ScreenGuid, 
      IScreenDim ScreenDim, IEnumerable<IScreenItem> Items)
    {
      this.ScreenName = ScreenName;
      this.NamespaceName = NamespaceName;
      this.ScreenGuid = ScreenGuid;
      this.ScreenDim = new ScreenDimModel(ScreenDim);

      // create the ObservableCollection of item models from the list of items of
      // the ScreenDefn.
      this.Items = new ObservableCollection<IScreenItem>();
      var jj = from a in Items
               select ScreenItemModel.Factory(a) as IScreenItem;
      this.LoadItems(jj);
    }

    /// <summary>
    /// event signaled when a change made to the section.  Most common being that
    /// a screen item within the section is changed, deleted, added.
    /// </summary>
    public event Action<ISectionHeader> SectionHeaderChanged;
    public void OnSectionHeaderChanged()
    {
      if (this.SectionHeaderChanged != null)
      {
        this.SectionHeaderChanged(this);
      }
    }

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
    private string _NamespaceName;

    public string ScreenGuid { get; set; }

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

    public ObservableCollection<IScreenItem> Items
    { get; set; }

    IList<IScreenItem> IScreenDefn.Items
    {
      get
      {
        return this.Items;
      }
    }
    IList<IScreenItem> ISectionHeader.Items
    {
      get { return this.Items; }
    }
  }
}
