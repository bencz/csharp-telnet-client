using AutoCoder.Systm;
using AutoCoder.Telnet.Enums.IBM5250;
using AutoCoder.Telnet.IBM5250.Common;
using AutoCoder.Telnet.IBM5250.Header;
using AutoCoder.Telnet.IBM5250.WorkstationCommands;
using AutoCoder.Telnet.TelnetCommands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TextCanvasLib.Canvas;
using TextCanvasLib.Visual;

namespace TextCanvasLib.Telnet
{
  public static class SaveScreenCommandExt
  {
#if skip
    /// <summary>
    /// no longer used. called WorkstationCommandListExt.ProcessAndPaint, which
    /// itself is not used.
    /// </summary>
    /// <param name="VisualItems"></param>
    /// <param name="Caret"></param>
    /// <returns></returns>
    public static byte[] BuildSaveScreenResponse(
      ScreenVisualItems VisualItems, CanvasPositionCursor Caret)
    {
      var ra = new ByteArrayBuilder();

      // data stream header.
      {
        var buf = DataStreamHeader.Build(50, TerminalOpcode.SaveScreen, 0, 0);
        ra.Append(buf);
      }

      // restore screen workstation command.
      {
        var cmd = new RestoreScreenCommand();
        ra.Append(cmd.ToBytes());
      }

      // clear unit command.
      {
        var cmd = new ClearUnitCommand();
        ra.Append(cmd.ToBytes());
      }

      // WTD command.
      {
        var ordersByteStream = VisualItems.BuildOrderStream(Caret);
        var buf = WriteToDisplayCommand.Build(0x00, 0x18, ordersByteStream);
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
#endif
  }
}
