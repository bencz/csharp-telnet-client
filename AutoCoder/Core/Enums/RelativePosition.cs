using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AutoCoder.Core.Enums
{
  public enum RelativePosition
  {
    Begin,
    Before,
    After,
    End,
    At,
    None
  } ;

  public static class RelativePositionExt
  {

    /// <summary>
    /// test if the enum value is equal any of the compare to enum values.
    /// </summary>
    /// <param name="Position"></param>
    /// <param name="Values"></param>
    /// <returns></returns>
    public static bool EqualAny(this RelativePosition Position, params RelativePosition[] Values)
    {
      bool rc = false;
      foreach (var vlu in Values)
      {
        if (vlu == Position)
        {
          rc = true;
          break;
        }
      }
      return rc;
    }
  }

}
