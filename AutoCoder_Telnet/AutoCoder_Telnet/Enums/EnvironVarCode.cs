using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoCoder.Telnet.Enums
{

  public enum EnvironVarCode
  {
    VAR = 0,
    VALUE = 1,
    ESC = 2,
    USERVAR = 3
  }


  public static class EnvironVarCodeExt
  {
    public static EnvironVarCode? ToEnvironVarCode( this byte? Value )
    {
      if (Value == null)
        return null;
      else if ( Value == 0 )
        return EnvironVarCode.VAR ;
      else if ( Value == 1 )
        return EnvironVarCode.VALUE ;
      else if ( Value == 2 )
        return EnvironVarCode.ESC ;
      else if ( Value == 3 )
        return EnvironVarCode.USERVAR ;
      else
        return null ;
    }
    public static EnvironVarCode? ToEnvironVarCode(this byte Value)
    {
      if (Value == 0)
        return EnvironVarCode.VAR;
      else if (Value == 1)
        return EnvironVarCode.VALUE;
      else if (Value == 2)
        return EnvironVarCode.ESC;
      else if (Value == 3)
        return EnvironVarCode.USERVAR;
      else
        return null;
    }

    public static byte ToByte(this EnvironVarCode OptionCode)
    {
      byte b1 = (byte)OptionCode;
      return b1;
    }
  }
}

