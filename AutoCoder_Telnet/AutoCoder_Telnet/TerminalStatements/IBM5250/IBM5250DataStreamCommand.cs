using AutoCoder.Telnet.Common;
using AutoCoder.Telnet.Enums.IBM5250;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoCoder.Telnet.TerminalStatements.IBM5250
{
  public class IBM5250DataStreamCommand
  {
    public CommandCode? CmdCode
    { get; set; }

    public IBM5250DataStreamCommand(CommandCode? CmdCode)
    {
      this.CmdCode = CmdCode;
    }

    public static IBM5250DataStreamCommand Factory(InputByteArray InputArray)
    {
      IBM5250DataStreamCommand dsCmd = null;

      if (InputArray.RemainingLength >= 2)
      {
        var buf = InputArray.PeekBytes(2);
        if (buf[0] == 0x04)
        {
          var cmdCode = buf[1].ToCommandCode();
          if (cmdCode != null)
          {
            if (cmdCode.Value == CommandCode.ClearUnit)
              dsCmd = new ClearUnitCommand();

            else if (cmdCode.Value == CommandCode.WTD)
              dsCmd = WriteToDisplayCommand.Factory(InputArray);

            if (dsCmd != null)
              InputArray.AdvanceIndex(dsCmd.GetDataStreamLength());
          }
        }
      }

      return dsCmd;
    }

    public virtual int GetDataStreamLength()
    {
      return 2;
    }

    public override string ToString()
    {
      return CmdCode.ToString();
    }
  }
}
