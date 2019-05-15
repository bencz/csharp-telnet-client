using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace AutoCoder.Ext.System.Reflection
{
  public static class AssemblyNameExt
  {
    public static bool IsEqual(this AssemblyName name1, AssemblyName name2)
    {
      if ((name1.FullName == name2.FullName)
            && (name1.Version.Equals(name2.Version) == true))
        return true;
      else
        return false;
    }
  }
}
