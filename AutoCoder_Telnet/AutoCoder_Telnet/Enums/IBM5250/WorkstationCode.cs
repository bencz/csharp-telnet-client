using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoCoder.Telnet.Enums.IBM5250
{
  public enum WorkstationCode : byte
  {
    ClearUnit = 0x40,
    ClearUnitAlternate = 0x20,
    ClearFormatTable = 0x50,
    WTD = 0x11,
    WriteErrorCode = 0x21,
    WriteErrorCodeToWindow = 0x22,
    ReadInputFields = 0x42,
    ReadMdtFields = 0x52,
    ReadMdtAlternate = 0x82,
    ReadScreen = 0x62,
    ReadScreenWithAttributes = 0x64,
    ReadScreenToPrint  = 0x66,
    ReadScreenToPrintWithAttributes = 0x68,
    ReadScreenToPrintWithGridlines = 0x6a,
    ReadScreenToPrintWithAttributesAndGridlines = 0x6c,
    ReadImmediate = 0x72,
    ReadModifiedImmediateAlternate = 0x83,
    SaveScreen = 0x02,
    SavePartialScreen = 0x03,
    RestoreScreen = 0x12,
    RestorePartialScreen = 0x13,
    Roll = 0x23, 
    WriteStructuredField = 0xf3,
    WriteSingleStructuredField = 0xf4,
    CopyToPrinter = 0x16
  }

  public static class WorkstationCodeExt
  {
    public static WorkstationCode? ToWorkstationCode(this byte Value)
    {
      WorkstationCode? cmdCode = null;

      byte[] byteValueArray = new byte[] { 0x40, 0x20,
        0x50, 0x11, 0x21,
        0x22, 0x42, 0x52,
      0x82, 0x62, 0x64, 0x66, 0x68, 0x6a, 0x6c, 0x72,
      0x83, 0x02, 0x03, 0x12, 0x13, 0x23, 0xf3,
      0xf4, 0x16 };

      WorkstationCode[] cmdCodeArray = new WorkstationCode[]
      { WorkstationCode.ClearUnit, WorkstationCode.ClearUnitAlternate,
        WorkstationCode.ClearFormatTable, WorkstationCode.WTD, WorkstationCode.WriteErrorCode,
        WorkstationCode.WriteErrorCodeToWindow, WorkstationCode.ReadInputFields,
        WorkstationCode.ReadMdtFields, WorkstationCode.ReadMdtAlternate,
      WorkstationCode.ReadScreen, WorkstationCode.ReadScreenWithAttributes, WorkstationCode.ReadScreenToPrint,
      WorkstationCode.ReadScreenToPrintWithAttributes, WorkstationCode.ReadScreenToPrintWithGridlines,
      WorkstationCode.ReadScreenToPrintWithAttributesAndGridlines,
      WorkstationCode.ReadImmediate, WorkstationCode.ReadModifiedImmediateAlternate,
      WorkstationCode.SaveScreen, WorkstationCode.SavePartialScreen, WorkstationCode.RestoreScreen,
      WorkstationCode.RestorePartialScreen, WorkstationCode.Roll, WorkstationCode.WriteStructuredField,
      WorkstationCode.WriteSingleStructuredField, WorkstationCode.CopyToPrinter
      };

      var ix = Array.IndexOf<byte>(byteValueArray, Value);
      if ( ix >= 0 )
        cmdCode = cmdCodeArray[ix];

      return cmdCode;
    }
  }
}
