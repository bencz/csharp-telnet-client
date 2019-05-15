using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AutoCoder.Ext.System
{
  public static class EnumExt
  {
    /// <summary>
    /// return the int value of the Enum object. ( cannot cast direct from Enum to
    /// int. Have to cast first to object. )
    /// </summary>
    /// <param name="EnumValue"></param>
    /// <returns></returns>
    public static int ToInt( this Enum EnumValue )
    {
      object objValue = EnumValue as object;
      int enumAsInt = (int)objValue;
      return enumAsInt;
    }
  }
}
