using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AutoCoder.Ext.System
{
  public static class BoolExt
  {
    public static bool Toggle(this Boolean Value)
    {
      if (Value == true)
        return false;
      else
        return true;
    }
  }
}
