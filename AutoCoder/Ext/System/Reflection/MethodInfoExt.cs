using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace AutoCoder.Ext.System.Reflection
{
  public static class MethodInfoExt
  {
    public static string[] GetParameterNames(this MethodInfo methodInfo)
    {
      var parms = methodInfo.GetParameters();
      string[] names = new string[parms.Length];
      for (int ix = 0; ix < parms.Length; ++ix)
      {
        names[ix] = parms[ix].Name;
      }
      return names;
    }
  }
}
