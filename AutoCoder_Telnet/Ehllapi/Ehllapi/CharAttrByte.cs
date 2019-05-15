using System;
using System.Collections.Generic;
using System.Text;

namespace AutoCoder.Ehllapi
{
  public class CharAttrByte
  {
    byte mAttrByte;

    public CharAttrByte(byte AttrByte)
    {
      mAttrByte = AttrByte;
    }

    public byte AttrByte
    {
      get { return mAttrByte; }
      set { mAttrByte = value; }
    }

    public bool Blink
    {
      get { return ((mAttrByte & 0x20) != 0); }
      set
      {
        if (value == true)
          mAttrByte |= 0x20; // turn on bit 2. 
        else
          mAttrByte &= 0xdf; // turn off bit 2.
      }
    }

    public bool ColumnSeparator
    {
      get { return ((mAttrByte & 0x10) != 0); }
      set
      {
        if (value == true)
          mAttrByte |= 0x10; // turn on bit 3. 
        else
          mAttrByte &= 0xef; // turn off bit 3.
      }
    }

    public bool ReverseImage
    {
      get { return ((mAttrByte & 0x80) != 0); }
      set
      {
        if (value == true)
          mAttrByte |= 0x80; // turn on bit 0. 
        else
          mAttrByte &= 0x7f; // turn off bit 0.
      }
    }

    public bool Underline
    {
      get { return ((mAttrByte & 0x40) != 0); }
      set
      {
        if (value == true)
          mAttrByte |= 0x40; // turn on bit 1. 
        else
          mAttrByte &= 0xbf; // turn off bit 1.
      }
    }

    public override string ToString()
    {
      StringBuilder sb = new StringBuilder();
      if (ReverseImage == true)
        sb.Append("ReverseImage");
      else
        sb.Append("Normal");

      if (Underline == true)
        sb.Append(", Underline");
      else
        sb.Append(", NoUnderline");

      if (Blink == true)
        sb.Append(", Blink");
      else
        sb.Append(", NoBlink");

      if (ColumnSeparator == true)
        sb.Append(", ColumnSeparators");

      return sb.ToString();
    }

    public string ToHexString()
    {
      return mAttrByte.ToString("X").PadLeft(2, '0');
    }
  }
}
