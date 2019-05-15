using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AutoCoder.Systm
{
  public class ByteArrayBuilder
  {
    private byte[] Buffer
    { get; set; }

    public int Length
    {
      get { return UsedLgth; }
    }

    private int UsedLgth
    { get; set; }

    private int RemLgth
    {
      get
      {
        int remLx = this.Buffer.Length - this.UsedLgth;
        return remLx;
      }
    }

    public ByteArrayBuilder()
    {
      this.Buffer = new byte[256];
      this.UsedLgth = 0;
    }

    public ByteArrayBuilder(int Alloc)
    {
      this.Buffer = new byte[Alloc];
      this.UsedLgth = 0;
    }

    public void Append(byte Byte)
    {
      // accum array is full.  Alloc a new array and copy contents into it.
      if (this.UsedLgth == this.Buffer.Length)
      {
        DoubleBuffer();
      }

      // add the input byte to the end of the accum array.
      this.Buffer[this.UsedLgth] = Byte;
      this.UsedLgth += 1;
    }

    public void Append(int Value)
    {
      if ((Value < 0) || (Value > 255))
        throw new Exception("not a byte value");
      else
        this.Append((byte)Value);
    }

    public void Append(byte[] Bytes)
    {
      if (Bytes.Length <= this.RemLgth)
      {
        Array.Copy(Bytes, 0, this.Buffer, this.UsedLgth, Bytes.Length);
        this.UsedLgth += Bytes.Length;
      }
      else
      {
        DoubleBuffer();
        Append(Bytes);
      }
    }

    public void Append(byte[] Bytes, int Start, int Length )
    {
      if ( Length <= this.RemLgth)
      {
        Array.Copy(Bytes, Start, this.Buffer, this.UsedLgth, Length);
        this.UsedLgth += Length;
      }
      else
      {
        DoubleBuffer();
        Append(Bytes, Start, Length);
      }
    }

    public void AppendBigEndianShort(short Value)
    {
      var ip = IntParts.ToBigEndianShort(Value);
      Append(ip);
    }

    public void CopyToBuffer(byte[] Bytes, int ToIndex )
    {
      Array.Copy(Bytes, 0, this.Buffer, ToIndex, Bytes.Length);
    }

    private void DoubleBuffer()
    {
      int lx = this.Buffer.Length * 2;
      byte[] buf = new byte[lx];
      Array.Copy(this.Buffer, buf, this.UsedLgth);
      this.Buffer = buf;
    }

    public byte[] ToByteArray()
    {
      byte[] ba = new byte[this.UsedLgth];
      Array.Copy(this.Buffer, ba, this.UsedLgth);
      return ba;
    }
  }
}
