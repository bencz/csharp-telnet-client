using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;

namespace AutoCoder.ComponentModel
{
  public interface ITreeViewHierarchy<T> where T : ITreeViewHierarchy<T>
  {
    ObservableCollection<T> Children { get; }
    bool IsExpanded { get; set; }
    bool IsSelected { get; set; }
    IHierarchicalModel Parent { get; }
  }
}
