using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace AutoCoder.Systm
{
  [StructLayout(LayoutKind.Explicit)]
  public class IntParts
  {
    [FieldOffset(0)]
    public byte byte0;
    [FieldOffset(1)]
    public byte byte1;
    [FieldOffset(2)]
    public byte byte2;
    [FieldOffset(3)]
    public byte byte3;
    [FieldOffset(0)]
    public int int03;
    [FieldOffset(0)]
    public short short01;

    [FieldOffset(0)]
    byte mByte0;
    [FieldOffset(1)]
    byte mByte1;
    [FieldOffset(2)]
    byte mByte2;
    [FieldOffset(3)]
    byte mByte3;
    [FieldOffset(0)]
    int mInt;
    [FieldOffset(0)]
    short mShort;
    [FieldOffset(0)]
    ushort mUShort;

    public IntParts()
    {
      IntValue = 0;
    }

    public IntParts(int InValue)
    {
      IntValue = InValue;
    }

    public short ShortValue
    {
      get
      {
        return mShort;
      }
      set
      {
        mShort = value;
      }
    }

    public ushort UnsignedShortValue
    {
      get
      {
        return mUShort;
      }
      set
      {
        mUShort = value;
      }
    }

    public int IntValue
    {
      get
      {
        return mInt;
      }
      set
      {
        mInt = value;
      }
    }

    public static IntParts LoadBigEndianInt(byte[] Buf, int Ix)
    {
      IntParts ip = new IntParts();
      ip.byte3 = Buf[Ix];
      ip.byte2 = Buf[Ix + 1];
      ip.byte1 = Buf[Ix + 2];
      ip.byte0 = Buf[Ix + 3];
      return ip;
    }

    public static IntParts LoadBigEndianInt3(byte[] Buf, int Ix)
    {
      IntParts ip = new IntParts();
      ip.byte3 = 0;
      ip.byte2 = Buf[Ix];
      ip.byte1 = Buf[Ix + 1];
      ip.byte0 = Buf[Ix + 2];

      return ip;
    }

    public static IntParts LoadBigEndianShort(byte[] Buf, int Ix)
    {
      IntParts ip = new IntParts();
      ip.byte1 = Buf[Ix];
      ip.byte0 = Buf[Ix + 1];
      return ip;
    }

    /// <summary>
    /// return the short value as a big endian short stored in byte[] array.
    /// </summary>
    /// <param name="Value"></param>
    /// <returns></returns>
    public static byte[] ToBigEndianShort( short Value )
    {
      IntParts ip = new IntParts();
      ip.ShortValue = Value;
      byte[] buf = new byte[2];
      buf[0] = ip.byte1;
      buf[1] = ip.byte0;
      return buf;
    }

    public byte Byte0
    {
      get { return mByte0; }
    }
    public byte Byte1
    {
      get { return mByte1; }
    }
  }
}
