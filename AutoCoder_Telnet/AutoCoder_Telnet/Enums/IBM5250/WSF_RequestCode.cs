using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoCoder.Telnet.Enums.IBM5250
{
  /// <summary>
  /// the t field that supplies the second level command identification of the write
  /// structured field workstation command.
  /// </summary>
  public enum WSF_RequestCode : byte
  {
    AuditWindow = 0x30,
    CommandKeyFunction = 0x31,
    ReadTextScreen = 0x32,
    PendingOperation = 0x33,
    TextScreenFormat = 0x34,
    ScaleLine = 0x35,
    WriteTextScreen = 0x36,
    SpecialCharacters = 0x37,
    PendingData = 0x38,
    OperatorErrorMessages = 0x39,
    PitchTable = 0x3a,
    FakeCommandKeyFunction = 0x3b,
    PassThrough = 0x3f,
    Query5250 = 0x70,
    Query5250State = 0x72
  }

  public static class WSF_RequestCodeExt
  {
    public static WSF_RequestCode? ToRequestCode(this byte Value)
    {
      if (Value == (byte)WSF_RequestCode.AuditWindow)
        return WSF_RequestCode.AuditWindow;
      else if (Value == (byte)WSF_RequestCode.CommandKeyFunction)
        return WSF_RequestCode.CommandKeyFunction;
      else if (Value == (byte)WSF_RequestCode.ReadTextScreen)
        return WSF_RequestCode.ReadTextScreen;
      else if (Value == (byte)WSF_RequestCode.PendingOperation)
        return WSF_RequestCode.PendingOperation;
      else if (Value == (byte)WSF_RequestCode.TextScreenFormat)
        return WSF_RequestCode.TextScreenFormat;
      else if (Value == (byte)WSF_RequestCode.ScaleLine)
        return WSF_RequestCode.ScaleLine;
      else if (Value == (byte)WSF_RequestCode.WriteTextScreen)
        return WSF_RequestCode.WriteTextScreen;
      else if (Value == (byte)WSF_RequestCode.SpecialCharacters)
        return WSF_RequestCode.SpecialCharacters;
      else if (Value == (byte)WSF_RequestCode.PendingData)
        return WSF_RequestCode.PendingData;
      else if (Value == (byte)WSF_RequestCode.OperatorErrorMessages)
        return WSF_RequestCode.OperatorErrorMessages;
      else if (Value == (byte)WSF_RequestCode.PitchTable)
        return WSF_RequestCode.PitchTable;
      else if (Value == (byte)WSF_RequestCode.FakeCommandKeyFunction)
        return WSF_RequestCode.FakeCommandKeyFunction;
      else if (Value == (byte)WSF_RequestCode.PassThrough)
        return WSF_RequestCode.PassThrough;
      else if (Value == (byte)WSF_RequestCode.Query5250)
        return WSF_RequestCode.Query5250;
      else if (Value == (byte)WSF_RequestCode.Query5250State)
        return WSF_RequestCode.Query5250State;
      else
        return null;
    }
  }
}
