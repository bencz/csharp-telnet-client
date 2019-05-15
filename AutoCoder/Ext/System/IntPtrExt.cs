using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AutoCoder.Ext.System
{
  public static class IntPtrExt
  {

    /// <summary>
    /// the HIWORD portion of the IntPtr.
    /// </summary>
    /// <param name="Value"></param>
    /// <returns></returns>
    public static int HiWord(this IntPtr Value)
    {
      int hiWord = unchecked((short)((uint)Value >> 16));
      return hiWord;
    }

    /// <summary>
    /// the LOWORD portion of the IntPtr.
    /// </summary>
    /// <param name="Value"></param>
    /// <returns></returns>
    public static int LoWord(this IntPtr Value)
    {
      int loWord = unchecked((short)Value);
      return loWord;
    }
  }
}
