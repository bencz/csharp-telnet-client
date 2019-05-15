using AutoCoder.Telnet.Enums.IBM5250;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoCoder.Telnet.CodedBytes
{
  public static class FieldControlWord
  {
    public static bool IsContinuedFieldSegment(this byte[] Bytes)
    {
      if ((Bytes != null) && (Bytes[0] == 0x86))
      {
        var b2 = Bytes[1];
        if ((b2 == 0x01) || (b2 == 0x02) || (b2 == 0x03))
          return true;
      }
      return false;
    }

    public static ContinuedFieldSegmentCode? ToContinuedFieldSegmentCode( this byte[] Bytes)
    {
      if ( Bytes.IsContinuedFieldSegment( )== true)
      {
        var segCode = (ContinuedFieldSegmentCode)Bytes[1];
        return segCode;
      }
      return null;
    }

  }
}
