using AutoCoder.Telnet.Enums.IBM5250;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoCoder.Telnet.TerminalStatements.IBM5250
{
  public class ClearUnitCommand : IBM5250DataStreamCommand
  {
    public ClearUnitCommand()
      : base(CommandCode.ClearUnit)
    {
    }

    public override int GetDataStreamLength()
    {
      return 2;
    }
  }
}
