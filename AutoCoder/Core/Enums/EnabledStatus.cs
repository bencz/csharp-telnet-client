using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AutoCoder.Core.Enums
{
  public enum EnabledStatus
  {
    Enabled,
    Disabled
  }

  public static class EnabledStatusExt
  {
    public static IEnumerable<EnabledStatus> StatusValues()
    {
      yield return EnabledStatus.Enabled;
      yield return EnabledStatus.Disabled;
      yield break;
    }

    public static bool Equals(this Nullable<EnabledStatus> Value1, EnabledStatus Value2)
    {
      if (Value1 == null)
        return false;
      else if (Value1.Value == Value2)
        return true;
      else
        return false;
    }
  }
}
