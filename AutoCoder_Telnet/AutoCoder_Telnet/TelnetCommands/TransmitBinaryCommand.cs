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
  public class TransmitBinaryCommand : TelnetCommand
  {

    public TransmitBinaryCommand(
      InputByteArray InputArray, CommandCode CmdCode )
      : base(InputArray, CmdCode, TelnetSubject.TRANSMIT_BINARY)
    {
    }
  }
}
