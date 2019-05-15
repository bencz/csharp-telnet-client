using AutoCoder.Systm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AutoCoder.Ext.System
{
  public static class ShortExt
  {
    public static byte[] ToBigEndian(this short Value)
    {
      var buf = IntParts.ToBigEndianShort(Value);
      return buf;
    }
  }
}
