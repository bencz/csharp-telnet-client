using AutoCoder.Systm;
using AutoCoder.Telnet.Common;
using AutoCoder.Telnet.Enums;
using AutoCoder.Telnet.TelnetCommands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoCoder.Telnet.TelnetCommands
{
  public class NawsStatement : TelnetCommand
  {

    public NawsStatement(
      InputByteArray InputArray, CommandCode CmdCode)
      : base(InputArray, CmdCode, TelnetSubject.NAWS)
    {
    }
  }
}

