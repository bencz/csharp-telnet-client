using AutoCoder.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TelnetTester.Windows
{
  public class Model2 : ModelBase
  {
    private string _FirstName;

    public string FirstName
    {
      get { return _FirstName; }
      set
      {
        if (_FirstName != value)
        {
          _FirstName = value;
          RaisePropertyChanged("FirstName");
        }
      }
    }

  }
}
