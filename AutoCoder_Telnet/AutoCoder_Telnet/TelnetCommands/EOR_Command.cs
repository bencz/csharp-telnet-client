using AutoCoder.Systm;
using AutoCoder.Telnet.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoCoder.Telnet.TelnetCommands
{
  public class EOR_Command : TelnetCommand
  {
    public EOR_Command()
      : base(CommandCode.EOR)
    {
    }

    public EOR_Command(InputByteArray InputArray)
      : base(InputArray, CommandCode.EOR)
    {
    }

    /// <summary>
    /// build byte array containing a telnet EOR command.
    /// </summary>
    /// <returns></returns>
    public static byte[] Build( )
    {
      byte[] buf = new byte[] { (byte)CommandCode.IAC, (byte)CommandCode.EOR };
      return buf;
    }
  }
}
