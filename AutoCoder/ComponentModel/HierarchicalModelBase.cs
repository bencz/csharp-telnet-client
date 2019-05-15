using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using AutoCoder.Core.Enums;

namespace AutoCoder.ComponentModel
{
  /// <summary>
  /// base class of a model class that implements the IHierarchicalModel interface.
  /// An IHierarchicalModel is used when binding to a hierarchical control such as the
  /// TreeView.
  /// </summary>
  public abstract class HierarchicalModelBase : ModelBase, IHierarchicalModel
  {

    public ObservableCollection<IHierarchicalModel> Children
    {
      get 
      {
        if ((this.CanExpand == true ) && (this.IsMarkerChild == false ) 
          && (this.FillExpandableMarkerMethodHasBeenCalled == false) 
          && (_Children.Count() == 0))
        {
          this.FillExpandableMarkerMethodHasBeenCalled = true;
          FillExpandableMarker();
        }
        return _Children; 
      }
    }
    private readonly ObservableCollection<IHierarchicalModel> _Children =
      new ObservableCollection<IHierarchicalModel>();

    public void AddChild(HierarchicalModelBase Item)
    {
      this.Children.Add(Item);
      return;

      if (Item.CanExpand == true)
      {
        var dummy = new DummyChild();
        dummy.HasChildrenMarker = CollectionMarker.HasChildrenMarker;
        Item.Children.Add(dummy);
      }
    }

    public virtual bool CanExpand
    {
      get { return true; }
    }

    public void CheckAddDummyChild()
    {
      if (this.CanExpand == true)
      {
        var dummy = new DummyChild();
        dummy.HasChildrenMarker = CollectionMarker.HasChildrenMarker;
        this.Children.Add(dummy);
      }
    }

    /// <summary>
    /// ensure that the Children contents of this Hierarchical object contain the
    /// actual data, not the initial marker child item.
    /// </summary>
    protected void EnsureChildrenData( )
    {
      if (this.HasMarkerChild == true)
      {
        this.Children.Clear();
        FillChildren();
      }
    }

    protected abstract void FillChildren();

    /// <summary>
    /// method responsible for adding the single HasChildrenMarker item to the Children
    /// collection. The binding expandable control ( TreeView, TreeViewItem ) looks to 
    /// the Children collection to see if it has any contents when deciding to display an
    /// expand button that the user can click.
    /// </summary>
    protected abstract void FillExpandableMarker();
    // FillChildren_ExpandableMarkerPurposes

    private bool FillExpandableMarkerMethodHasBeenCalled
    { get; set; }

    protected CollectionMarker? HasChildrenMarker
    { get; set; }

    public bool HasMarkerChild
    {
      get
      {
        if ((this.Children.Count() == 1) && (this.Children[0].IsMarkerChild == true))
          return true;
        else
          return false;
      }
    }

    public virtual bool IsMarkerChild
    {
      get
      {
        return false;
        if (this.HasChildrenMarker != null)
          return true;
        else
          return false;
      }
    }

    bool _IsExpanded;
    public bool IsExpanded
    {
      get { return _IsExpanded; }
      set
      {
        if (_IsExpanded != value)
        {
          _IsExpanded = value;
          RaisePropertyChanged("IsExpanded");

          if (this.IsExpanded == true)
          {
            EnsureChildrenData();
          }
        }
      }
    }

    bool _IsSelected;
    public bool IsSelected
    {
      get { return _IsSelected; }
      set
      {
        if (_IsSelected != value)
        {
          _IsSelected = value;
          RaisePropertyChanged("IsSelected");
        }
      }
    }

    IHierarchicalModel _Parent;
    public IHierarchicalModel Parent
    {
      get { return _Parent; }
      set
      {
        if (_Parent != value)
        {
          _Parent = value;
          RaisePropertyChanged("Parent");
        }
      }
    }

    public class DummyChild : HierarchicalModelBase
    {
      protected override void FillChildren()
      {
      }

      protected override void FillExpandableMarker()
      {
      }

      public override bool CanExpand
      {
        get { return false; }
      }

      public override bool IsMarkerChild
      {
        get
        {
          return true;
        }
      }
    }
  }
}
