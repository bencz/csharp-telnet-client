using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Collections.ObjectModel;

namespace AutoCoder.ComponentModel
{
  public interface IHierarchicalModel : INotifyPropertyChanged
  {
    ObservableCollection<IHierarchicalModel> Children { get; }
    bool IsExpanded { get; set; }
    bool IsSelected { get; set; }
    IHierarchicalModel Parent { get; }
    bool HasMarkerChild { get; }
    bool IsMarkerChild { get; }
  }
}
