using AutoCoder.Ext.System;
using AutoCoder.Systm;
using AutoCoder.Telnet.TelnetCommands;
using AutoCoder.Telnet.Common;
using AutoCoder.Telnet.Connect;
using AutoCoder.Telnet.Enums;
using AutoCoder.Telnet.Enums.IBM5250;
using AutoCoder.Telnet.IBM5250.Response;
using AutoCoder.Telnet.IBM5250.WorkstationCommands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoCoder.Telnet.IBM5250.Header;

namespace AutoCoder.Telnet.IBM5250.Common
{
  public static class Common5250
  {

    /// <summary>
    /// test if byte is an attribute byte. Hex value 0x20 thru 0x3f.
    /// </summary>
    /// <param name="Value"></param>
    /// <returns></returns>
    public static bool IsAttributeByte(this byte Value)
    {
      bool isAttrByte = false;
      if ((Value >= 0x20) && (Value <= 0x3f))
        isAttrByte = true;
      return isAttrByte;
    }


    /// <summary>
    /// scan the InputArray buffer until a char which is not a valid text data byte.
    /// Returns absolute index location of the non text byte.
    /// If non text char is not found, returns index one past last byte of the array.
    /// Also returns the length of text data that was scanned past.
    /// </summary>
    /// <param name="InputArray"></param>
    /// <returns></returns>
    public static Tuple<int, int> ScanNonTextDataByte(InputByteArray InputArray)
    {
      int remLx = InputArray.RemainingLength;
      int ix = 0;
      while (ix < remLx)
      {
        var b1 = InputArray.PeekByte(ix);
        if (IsTextDataChar(b1) == true)
          ix += 1;
        else
          break;
      }

      int fx = InputArray.Index + ix;
      int lx = fx - InputArray.Index;
      return new Tuple<int, int>(fx, lx);
    }

    static byte[] NonTextBytes = new byte[] {0x01, 0x02, 0x03, 0x04, 0x10, 0x11,
    0x12, 0x13, 0x14, 0x15, 0x1d, 0xff }; 
    public static bool IsTextDataChar(byte Value)
    {
      if (Array.IndexOf(NonTextBytes, Value) == -1)
        return true;
      else
        return false;
      
      if ((Value == 0x00) || (Value == 0x1c) || (Value == 0x1e)
        || (Value == 0x0e) || (Value == 0x0f)
        || ((Value >= 0x20) && (Value <= 0xfe)))
        return true;
      else
        return false;
    }

    public static string ToShowChar(this byte DataStreamByte)
    {
      var buf = new byte[] { DataStreamByte };
      if (DataStreamByte == 0x00)
        buf[0] = 0x40;
      else if ((DataStreamByte >= 0x20) && (DataStreamByte <= 0x3f))
        buf[0] = 0x40;
      return buf.EbcdicBytesToString();
    }
  }
}
