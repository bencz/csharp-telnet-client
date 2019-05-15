using AutoCoder.Systm;
using AutoCoder.Telnet.Common;
using AutoCoder.Telnet.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoCoder.Telnet.TelnetCommands
{
  public class EndSubNegCommand : TelnetCommand
  {
    public EndSubNegCommand( )
      : base(CommandCode.SE)
    {
    }
    public EndSubNegCommand(InputByteArray InputArray)
      : base(InputArray, CommandCode.SE)
    {
    }
  }
}
