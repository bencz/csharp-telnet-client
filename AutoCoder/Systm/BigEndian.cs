using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace AutoCoder.Systm
{
  public static class BigEndian
  {
    // ----------------------- BigEndianBytesToShort -------------------------
    public static short BigEndianBytesToShort(this byte[] Bytes, int Start)
    {
      IntParts ip = new IntParts();
      ip.byte1 = Bytes[Start];
      ip.byte0 = Bytes[Start + 1];
      return ip.ShortValue;
    }

    public static int BigEndianBytesToInt( this byte[] Bytes, int Start)
    {
      IntParts ip = new IntParts();
      ip.byte3 = Bytes[Start];
      ip.byte2 = Bytes[Start +1];
      ip.byte1 = Bytes[Start + 2];
      ip.byte0 = Bytes[Start + 3];
      return ip.IntValue;
    }

    // -------------------- IntToBigEndianBytes ------------------------------
    public static byte[] IntToBigEndianBytes(int InValue)
    {
      IntParts ip = new IntParts(InValue);
      byte[] beBytes = new byte[4];
      beBytes[0] = ip.byte3;
      beBytes[1] = ip.byte2;
      beBytes[2] = ip.byte1;
      beBytes[3] = ip.byte0;
      return beBytes;
    }

    // -------------------- ShortToBigEndianBytes ------------------------------
    public static byte[] ShortToBigEndianBytes(short InValue)
    {
      IntParts ip = new IntParts(InValue);
      byte[] beBytes = new byte[2];
      beBytes[1] = ip.byte0;
      beBytes[0] = ip.byte1;
      return beBytes;
    }
  }
}

