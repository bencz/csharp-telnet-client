using AutoCoder.Ext.System;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TextCanvasLib.Common
{
#if skip
  public static class FieldFormatWord
  {

    /// <summary>
    /// bit 2 is on. is a bypass field. No entry accepted in this field.
    /// </summary>
    /// <param name="ffw"></param>
    /// <returns></returns>
    public static bool IsBypass(byte[] ffw)
    {
      if (IsFFW(ffw) == false)
        return false;
      else if ((ffw[0] & 0x20) == 0)
        return false;
      else
        return true;
    }

    public static bool IsFFW(byte[] ffw)
    {
      if ((ffw == null) || (ffw.Length != 2))
        return false;
      else if ((ffw[0] & 0xc0) == 0x40)   // bits 0 - 1 = 01
        return true;
      else
        return false;
    }


    /// <summary>
    /// bit 10 is on. Translate entry to uppercase.
    /// </summary>
    /// <param name="ffw"></param>
    /// <returns></returns>
    public static bool IsMonocase(byte[] ffw)
    {
      if (IsFFW(ffw) == false)
        return false;
      else if ((ffw[1] & 0x20) == 0)
        return false;
      else
        return true;
    }

    public static string ToString(byte[] ffw)
    {
      var sb = new StringBuilder();
      if (ffw == null)
        sb.Append("null");
      else
      {
        sb.Append(ffw.ToHex());
        if (IsMonocase(ffw) == true)
          sb.Append(" monocase");
        if (IsBypass(ffw) == true)
          sb.Append(" bypass");
      }

      return sb.ToString();
    }
  }
#endif
}
