using AutoCoder.Systm;
using AutoCoder.Telnet.Enums.IBM5250;
using AutoCoder.Telnet.IBM5250.Common;
using AutoCoder.Telnet.IBM5250.Content;
using AutoCoder.Telnet.IBM5250.Header;
using AutoCoder.Telnet.TelnetCommands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoCoder.Telnet.IBM5250.WorkstationCommands
{
  public class ReadScreenCommand : WorkstationCommandBase
  {
    public ReadScreenCommand(InputByteArray InputArray)
      : base(InputArray, WorkstationCode.ReadScreen)
    {
      InputArray.AdvanceIndex(2);
    }
    public static byte[] BuildReadScreenResponse(ScreenContent ScreenContent)
    {
      var ra = new ByteArrayBuilder();

      // data stream header.
      {
        var buf = DataStreamHeader.Build(50, TerminalOpcode.ReadScreen, 0, 0);
        ra.Append(buf);
      }

      // screen regeneration buffer. each byte on the screen.
      {
        var buf = ScreenContent.GetRegenerationBuffer();
        ra.Append(buf);
      }

      // update length of response data stream.
      {
        var wk = new ByteArrayBuilder();
        wk.AppendBigEndianShort((short)ra.Length);
        ra.CopyToBuffer(wk.ToByteArray(), 0);
      }

      // IAC EOR
      {
        ra.Append(EOR_Command.Build());
      }

      return ra.ToByteArray();
    }
  }
}
