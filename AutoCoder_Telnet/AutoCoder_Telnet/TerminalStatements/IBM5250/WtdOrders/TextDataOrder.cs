using AutoCoder.Telnet.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoCoder.Telnet.TerminalStatements.IBM5250.WtdOrders
{
  public class TextDataOrder : WtdOrderBase
  {
    public byte? AttrByte
    { get; set; }

    public string DisplayText
    { get; set; }

    public TextDataOrder( byte? AttrByte, string DisplayText )
      : base(null)
    {
      this.AttrByte = AttrByte;
      this.DisplayText = DisplayText;
    }

    public static TextDataOrder Factory(InputByteArray InputArray)
    {
      byte? attrByte = null ;
      string displayText = null ;

      // scan forward in the input array for a non text character.
      int ix = ScanNonTextDataByte(InputArray);
      int remLx = ix - InputArray.Index;

      // first byte is the attribute byte.
      {
        var b1 = InputArray.PeekByte(0);
        if ((b1 >= 0x20) && (b1 <= 0x2f))
        {
          attrByte = b1;
          InputArray.AdvanceIndex(1);
          remLx -= 1;
        }
      }

      // remaining bytes are text characters in ebcdic.
      if (remLx > 0)
      {
        System.Text.Encoding encoding =
          System.Text.Encoding.GetEncoding(37); // 37 = ebcdic
        displayText = InputArray.GetEbcdicBytes(remLx);
      }

      TextDataOrder tdOrder = new TextDataOrder(attrByte, displayText);
      return tdOrder;
    }

    /// <summary>
    /// scan the InputArray buffer until a char which is not a valid text data byte.
    /// </summary>
    /// <param name="InputArray"></param>
    /// <returns></returns>
    public static int ScanNonTextDataByte(InputByteArray InputArray)
    {
      int lx = InputArray.RemainingLength;
      int ix = 0;
      while (ix < lx)
      {
        var b1 = InputArray.PeekByte(ix);
        if ( IsTextDataChar(b1) == true )
          ix += 1;
        else
          break;
      }

      return ix;
    }

    public static bool IsTextDataChar(byte Value)
    {
      if ((Value == 0x00) || (Value == 0x1c) || (Value == 0x1e)
        || (Value == 0x0e) || (Value == 0x0f)
        || ((Value >= 0x20) && (Value <= 0xfe)))
        return true;
      else
        return false;
    }
  }
}
