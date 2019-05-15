using AutoCoder.Systm;
using AutoCoder.Telnet.Common;
using AutoCoder.Telnet.Enums.IBM5250;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoCoder.Telnet.IBM5250.WorkstationCommands
{
  public class WriteErrorCodeCommand : WorkstationCommandBase
//    , IDataStreamReport
  {
    public WriteErrorCodeCommand(InputByteArray InputArray)
      : base(InputArray, WorkstationCode.WriteErrorCode)
    {
      if (InputArray.RemainingLength < 4)
        this.Errmsg = "Byte stream too short. Missing control chars.";

      if (this.Errmsg == null)
      {
        var buf = InputArray.PeekBytes(4);

        this.BytesLength += 2;
        InputArray.AdvanceIndex(this.BytesLength);
      }
    }

  }
}
