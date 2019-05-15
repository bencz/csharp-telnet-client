using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace AutoCoder.Systm.Diagnostics
{
  public static class DebugMore
  {
    public static void WriteExceptions(Exception[] ExcpArray)
    {
      foreach( var ex in ExcpArray)
      {
        Debug.WriteLine(ex.ToString());
      }
    }
  }
}
