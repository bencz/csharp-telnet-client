using AutoCoder.ComponentModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ScreenDefnLib.Defn;

namespace ScreenDefnLib.Models
{
  public class ScreenDefnCollectionModel : ModelBase
  {
    private ObservableCollection<ScreenDefn> _ScreenDefnList;
    public ObservableCollection<ScreenDefn> ScreenDefnList
    {
      get
      {
        if (_ScreenDefnList == null)
          _ScreenDefnList = new ObservableCollection<ScreenDefn>();
        return _ScreenDefnList;
      }
      set { _ScreenDefnList = value; }
    }
  }
}
