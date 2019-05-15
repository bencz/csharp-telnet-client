using AutoCoder.Ext;
using AutoCoder.Ext.System;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AutoCoder.Systm
{
  /// <summary>
  /// InputByteArray is a byte array along with an index to the next byte in the array to
  /// process. 
  /// DataLgth is important in that the byte array is likely allocated with a larger size.
  /// </summary>
  public class InputByteArray
  {
    public InputByteArray()
    {
      this.Bytes = new byte[] { };
      this.Index = 0;
      this.DataLgth = 0;
    }

    public InputByteArray(byte[] Bytes, int DataLgth)
    {
      LoadArray(Bytes, DataLgth);
    }
    public InputByteArray(byte[] Bytes)
    {
      LoadArray(Bytes, Bytes.Length);
    }

    public void AdvanceIndex(int Value)
    {
      this.Index += Value;
    }

    public byte[] Bytes
    { get; set; }

    public int DataLgth
    { get; set; }

    public int Index
    { get; set; }

    public bool IsEof()
    {
      if (this.Index >= this.DataLgth)
        return true;
      else
        return false;
    }

    public int RemainingLength
    {
      get
      {
        int remLx = this.DataLgth - Index;
        return remLx;
      }
    }

    /// <summary>
    /// set data length of the input array to zer.
    /// </summary>
    public void EmptyArray()
    {
      this.Index = 0;
      this.DataLgth = 0;
    }

    public byte[] GetBytesToEnd()
    {
      byte[] buf = new Byte[this.RemainingLength];
      Array.Copy(this.Bytes, this.Index, buf, 0, this.RemainingLength);
      this.Index = this.DataLgth;
      return buf;
    }

    /// <summary>
    /// get bytes ( and advance index ) from buffer until one of the stop bytes.
    /// Returns the bytes and the actual stop byte.
    /// </summary>
    /// <param name="Codes"></param>
    /// <returns></returns>
    public Tuple<byte[], byte?> GetBytesUntilCode(byte[] Codes)
    {
      var ab = new ByteArrayBuilder();
      int bx = this.Index;
      int ix = this.Index;
      while (ix < this.DataLgth)
      {
        byte b1 = this.Bytes[ix];

        // one of the code bytes. Stop getting bytes when one of the code.
        // but if the code byte is doubled up, accum the byte as any other.
        if (Codes.Contains(b1) == true)
        {
          int nx = ix + 1;
          if (nx >= this.DataLgth)
            break;
          byte b2 = this.Bytes[nx];
          if (b1 != b2)
            break;
          else
            ix += 1;
        }

        // accum this byte.
        ab.Append(b1);
        ix += 1;
      }

      // advance the index past all of the accumulated bytes.
      this.Index = ix;

      // the code byte that ended the accum
      byte? endAccumByte = null;
      if (this.IsEof() == false)
        endAccumByte = this.PeekNextByte();

      // return the accumulated data bytes
      return new Tuple<byte[], byte?>(ab.ToByteArray(), endAccumByte);
    }

    /// <summary>
    /// convert the bytes of the array to ebcdic, then return that text.
    /// </summary>
    /// <param name="Length"></param>
    /// <returns></returns>
    public string GetEbcdicBytes(int Length)
    {
      if (Length > this.RemainingLength)
        throw new Exception("bounds of array exceeded");

      System.Text.Encoding encoding =
        System.Text.Encoding.GetEncoding(37); // 37 = ebcdic
      var displayText = encoding.GetString(this.Bytes, this.Index, Length);
      this.AdvanceIndex(Length);
      return displayText;
    }

    public int GetBigEndianInt( )
    {
      if (Index + 3 >= this.DataLgth)
        throw new Exception("no more bytes to read from array");
      var ip = IntParts.LoadBigEndianInt(this.Bytes, this.Index);
      this.Index += 4;
      return ip.IntValue;
    }

    public short GetBigEndianShort()
    {
      if (Index + 1 >= this.DataLgth)
        throw new Exception("no more bytes to read from array");
      var ip = IntParts.LoadBigEndianShort(this.Bytes, this.Index);
      this.Index += 2;
      return ip.ShortValue;
    }

    public byte GetByte()
    {
      if (Index >= this.DataLgth)
        throw new Exception("no more bytes to read from array");
      byte b1 = this.Bytes[this.Index];
      this.Index += 1;
      return b1;
    }

    public byte[] GetBytes(int Length)
    {
      if (this.RemainingLength < Length)
        throw new Exception("length exceeds end of byte array");
      byte[] buf = new byte[Length];
      Array.Copy(this.Bytes, this.Index, buf, 0, Length);
      this.Index += Length;
      return buf;
    }

    /// <summary>
    /// replace with GetByte method.
    /// </summary>
    /// <returns></returns>
    public byte GetNextByte()
    {
      if (Index >= this.DataLgth)
        throw new Exception("no more bytes to read from array");
      byte b1 = this.Bytes[this.Index];
      this.Index += 1;
      return b1;
    }
    public void LoadArray(byte[] Bytes, int DataLgth)
    {
      this.Bytes = Bytes;
      this.Index = 0;
      this.DataLgth = DataLgth;
    }
    public short PeekBigEndianShort(int Offset)
    {
      var ix = this.Index + Offset;
      if (ix + 1 >= this.DataLgth)
        throw new Exception("no more bytes to read from array");
      var ip = IntParts.LoadBigEndianShort(this.Bytes, ix);
      return ip.ShortValue;
    }

    public int PeekBigEndianInt(int Offset)
    {
      var ix = this.Index + Offset;
      if (ix + 3 >= this.DataLgth)
        throw new Exception("no more bytes to read from array");
      var ip = IntParts.LoadBigEndianInt(this.Bytes, ix);
      return ip.IntValue;
    }

    public byte PeekByte(int Offset)
    {
      var ix = this.Index + Offset;
      if ((ix < 0) | (ix >= this.DataLgth))
        throw new Exception("index out of bounds of array");
      byte b1 = this.Bytes[ix];
      return b1;
    }

    /// <summary>
    /// peek the next 2 bytes. return true if IAC SE.
    /// </summary>
    /// <returns></returns>
    public bool PeekIacSe()
    {
      if (this.RemainingLength >= 2)
      {
        byte b1 = this.Bytes[this.Index];
        byte b2 = this.Bytes[this.Index + 1];
        if ((b1 == 255) && (b2 == 240))
          return true;
      }
      return false;
    }

    public byte PeekNextByte()
    {
      if (Index >= this.DataLgth)
        throw new Exception("no more bytes to read from array");
      byte b1 = this.Bytes[this.Index];
      return b1;
    }

    public byte[] PeekBytes(int Length)
    {
      var buf = PeekBytes(0, Length);
      return buf;
    }

    public byte[] PeekBytes(int Offset, int Length)
    {
      var ix = this.Index + Offset;
      if (this.RemainingLength < ( Offset + Length))
        throw new Exception("length exceeds end of byte array");
      byte[] buf = new byte[Length];
      Array.Copy(this.Bytes, ix, buf, 0, Length);
      return buf;
    }

    /// <summary>
    /// returns byte array containing the remaining bytes of the input array.
    /// </summary>
    /// <returns></returns>
    public byte[] PeekBytes()
    {
      var buf = PeekBytes(this.RemainingLength);
      return buf;
    }

    /// <summary>
    /// peek and return specified length of bytes from current position in Bytes 
    /// array. If length exceeds remaining length then shorten the peek length.
    /// </summary>
    /// <param name="Length"></param>
    /// <returns></returns>
    public byte[] PeekBytesLenient(int Length)
    {
      int lx = IntExt.Min(this.RemainingLength, Length);
      var buf = this.Bytes.SubArray(this.Index, lx);
      return buf;
    }

    /// <summary>
    /// peek and return bytes from offset from current position in byte array. If 
    /// length or offset is out of bounds, adjust to return as much of the sub array
    /// data as is valid.
    /// </summary>
    /// <param name="Offset"></param>
    /// <param name="Length"></param>
    /// <returns></returns>
    public byte[] PeekBytesLenient(int Offset, int Length)
    {
      byte[] buf = null;
      var lx = Length;

      int ix = this.Index + Offset;
      if (ix >= this.DataLgth)
        buf = new byte[] { };

      // adjust in case index position before zero.
      if ((buf == null ) && (ix < 0))
      {
        var adj = 0 - ix;
        ix += adj;
        lx -= adj;
      }

      if (buf == null)
      {
        var ex = ix + lx - 1;
        if (ex >= this.DataLgth)
          lx = this.DataLgth - ix;
        buf = this.Bytes.SubArray(ix, lx);
      }

      return buf;
    }

    /// <summary>
    /// peek next Length bytes from InputArray. Return in an array of length RtnLgth
    /// which is padded with Pad bytes if RtnLgth exceeds Length.
    /// </summary>
    /// <param name="Length"></param>
    /// <param name="BufLength"></param>
    /// <param name="Pad"></param>
    /// <returns></returns>
    public byte[] PeekBytesPad(int Length, int RtnLgth, byte Pad)
    {
      if (this.RemainingLength < Length)
        throw new Exception("length exceeds end of byte array");
      if (Length == RtnLgth)
      {
        var buf = this.Bytes.SubArray(this.Index, Length);
        return buf;
      }
      else
      {
        var buf = this.Bytes.SubArrayPad(this.Index, Length, RtnLgth, Pad);
        return buf;
      }
    }

    public byte[] PeekToEnd()
    {
      int remLx = this.RemainingLength;
      if (remLx == 0)
        throw new Exception("end of input array");
      byte[] buf = new byte[remLx];
      Array.Copy(this.Bytes, this.Index, buf, 0, remLx);
      return buf;
    }
  }
}
