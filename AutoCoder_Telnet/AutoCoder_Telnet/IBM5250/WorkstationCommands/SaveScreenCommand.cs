using AutoCoder.Systm;
using AutoCoder.Telnet.TelnetCommands;
using AutoCoder.Telnet.Enums.IBM5250;
using AutoCoder.Telnet.IBM5250.Common;
using AutoCoder.Telnet.IBM5250.Content;
using AutoCoder.Telnet.IBM5250.Header;
using AutoCoder.Telnet.Common.ScreenDm;

namespace AutoCoder.Telnet.IBM5250.WorkstationCommands
{
  public class SaveScreenCommand : WorkstationCommandBase
  {
    public SaveScreenCommand(InputByteArray InputArray)
      : base(InputArray, WorkstationCode.SaveScreen)
    {
      InputArray.AdvanceIndex(2);
    }
    public static byte[] BuildSaveScreenResponse(ScreenContent ScreenContent)
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
      if ( ScreenContent.ScreenDim.GetIsWideScreen( ) == true)
      {
        var cmd = new ClearUnitAlternateCommand(0x00);
        ra.Append(cmd.ToBytes());
      }
      else
      {
        var cmd = new ClearUnitCommand();
        ra.Append(cmd.ToBytes());
      }

      // WTD command.
      {
        var ordersByteStream = ScreenContent.BuildOrderStream( );
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
  }
}
