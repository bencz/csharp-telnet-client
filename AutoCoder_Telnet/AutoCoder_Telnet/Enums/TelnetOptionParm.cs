using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoCoder.Telnet.Enums
{
  public enum TelnetOptionParm
  {
    IS = 0,
    SEND = 1,
    INFO = 2
  }
    public static class TelnetOptionParmExt
    {
      public static TelnetOptionParm? ToTelnetOptionParm(this byte Value)
      {
        TelnetOptionParm? optn;

        if (Value == 0)
          optn = TelnetOptionParm.IS;
        else if (Value == 1)
          optn = TelnetOptionParm.SEND;
        else if (Value == 2)
          optn = TelnetOptionParm.INFO;
        else
          optn = null;

        return optn;
      }

      public static byte ToByte(this TelnetOptionParm SubOption)
      {
        byte b1 = (byte)SubOption;
        return b1;
      }
    }
  }


