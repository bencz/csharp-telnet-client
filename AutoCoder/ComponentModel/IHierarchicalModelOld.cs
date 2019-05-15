using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Collections.ObjectModel;

namespace AutoCoder.ComponentModel
{
  public interface IHierarchicalModelOld : INotifyPropertyChanged
  {
    ObservableCollection<IHierarchicalModelOld> Children { get; }
    bool IsExpanded { get; set; }
    bool IsSelected { get; set; }
    IHierarchicalModelOld Parent { get; }
  }
}
