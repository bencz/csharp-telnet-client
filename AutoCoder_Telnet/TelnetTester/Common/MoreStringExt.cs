using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoCoder.Ext.System;

namespace TelnetTester.Common
{
  public static class MoreStringExt
  {
    public static Tuple<byte,string> HsxTextToByte(this string HexText )
    {
      byte bv = 0x00;
      string errmsg = null;

      var text = HexText.ToUpper();

      // chunk is 2 chars and then a space.
      text = text.TrimEnd(new char[] { ' ' });
      if (text.Length != 2)
      {
        errmsg = "hex text must be length 2.";
      }

      // both chars of the chunk must be 0 thru 9, A thru F.
      if (errmsg == null)
      {
        char[] charSet = new char[] { '0', '1', '2', '3', '4', '5', '6',
          '7', '8', '9', 'A', 'B', 'C', 'D', 'E', 'F' };
        var fx = text.IndexOfNone(0, charSet);
        if (fx >= 0)
          errmsg = "hex text contains invalid character";
      }

      if ( errmsg == null )
      {
//        var xx = text.AsciiHexPairToChar(0);
        bv = global::System.Convert.ToByte(text, 16);
      }

      return new Tuple<byte, string>(bv, errmsg);
    }

    public static char xAsciiHexPairToChar(this string Text, int Start)
    {
      string hexPair = Text.Substring(Start, 2);
      byte b1 = global::System.Convert.ToByte(hexPair, 16);
      char[] ch1 = global::System.Text.Encoding.ASCII.GetChars(new byte[] { b1 });
      return ch1[0];
    }

  }
}
