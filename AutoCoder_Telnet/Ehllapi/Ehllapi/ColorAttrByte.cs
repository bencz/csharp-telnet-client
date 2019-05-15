using System;
using System.Collections.Generic;
using System.Text;
using AutoCoder.Ehllapi.Enums;

namespace AutoCoder.Ehllapi
{
  /// <summary>
  /// the color attribute byte is returned by the ehllapi CopyPresentationSpace
  /// functions when session parameters are set to EAB and XLATE.
  /// </summary>
  public class ColorAttrByte
  {
    byte mAttrByte;

    public ColorAttrByte(byte InAttrByte)
    {
      mAttrByte = InAttrByte;
    }

    public byte AttrByte
    {
      get { return mAttrByte; }
      set { mAttrByte = value; }
    }

    /// <summary>
    /// background color represented by bits 0-3 of the color attribute byte.
    /// </summary>
    public CharacterColor BackgroundColor
    {
      get
      {
        byte bc = (byte) (( mAttrByte & 0xf0 ) >> 4);
        return BitsToColor(bc);
      }
    }

    /// <summary>
    /// foreground color ( the color of the text ) is represented by bits 4-7
    /// of the color attribute byte.
    /// </summary>
    public CharacterColor ForegroundColor
    {
      get
      {
        byte bc = (byte)(mAttrByte & 0x0f);
        return BitsToColor(bc);
      }
    }

    private CharacterColor BitsToColor(byte InBits)
    {
      switch( InBits )
      {
        case 0x00:
          return CharacterColor.Black ;
        case 0x01:
          return CharacterColor.Blue;
        case 0x02:
          return CharacterColor.Green;
        case 0x03:
          return CharacterColor.Cyan;
        case 0x04:
          return CharacterColor.Red;
        case 0x05:
          return CharacterColor.Magenta;
        case 0x06:
          return CharacterColor.Yellow;
        case 0x07:
          return CharacterColor.White;
        case 0x08:
          return CharacterColor.Gray;
        case 0x09:
          return CharacterColor.LightBlue;
        case 0x0a:
          return CharacterColor.LightGreen;
        case 0x0b:
          return CharacterColor.LightCyan;
        case 0x0c:
          return CharacterColor.LightRed;
        case 0x0d:
          return CharacterColor.LightMagenta;
        case 0x0e:
          return CharacterColor.Yellow;
        default:
          return CharacterColor.BrightWhite ;
      }
    }

    public override string ToString()
    {
      return (
        "Forecolor=" + ForegroundColor.ToString() +
        " Backcolor=" + BackgroundColor.ToString());
    }

  }
}
