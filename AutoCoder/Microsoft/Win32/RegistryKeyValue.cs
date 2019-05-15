using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Win32;
using AutoCoder.Ext.System;

namespace AutoCoder.Microsoft.Win32
{
  public class RegistryKeyValue
  {
    public string ShowValueName
    {
      get
      {
        if (ValueName.IsNullOrEmpty() == true)
          return "[Default]";
        else
          return ValueName;
      }
    }

    public string ValueName
    { get; set; }

    public object ValueValue
    { get; set; }

    public RegistryValueKind ValueKind
    { get; set; }
  }
}
