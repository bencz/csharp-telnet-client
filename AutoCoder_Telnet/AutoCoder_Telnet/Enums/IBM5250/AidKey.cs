using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace AutoCoder.Telnet.Enums.IBM5250
{
  public enum AidKey : byte
  {
    Enter = 0xf1,
    Help = 0xf3,
    RollDown = 0xf4,
    RollUp = 0xf5,
    Print = 0xf6,
    F1 = 0x31,
    F2 = 0x32,
    F3 = 0x33,
    F4 = 0x34,
    F5 = 0x35,
    F6 = 0x36,
    F7 = 0x37,
    F8 = 0x38,
    F9 = 0x39,
    F10 =  0x3a,
    F11 = 0x3b,
    F12 = 0x3c,
    F13 = 0xb1,
    F14 = 0xb2,
    F15 = 0xb3,
    F16 = 0xb4,
    F17 = 0xb5,
    F18 = 0xb6,
    F19 = 0xb7,
    F20 = 0xb8,
    F21 = 0xb9,
    F22 = 0xba,
    F23 = 0xbb,
    F24 = 0xbc,
    applUse1 = 0x70,
    applUse2 = 0x71,
    applUse16 = 0x7f,

    // special code. Indicates a reply to 5250 query request.
    Query5250Reply = 0x88
  }

  public static class AidKeyExt
  {
    private static Key[] funcKeyArray = { Key.F1, Key.F2, Key.F3, Key.F4,
    Key.F5, Key.F6, Key.F7, Key.F8,
    Key.F9, Key.F10, Key.F11, Key.F12 };

    private static AidKey[] lowerAidKeyArray = {AidKey.F1, AidKey.F2, AidKey.F3,
    AidKey.F4, AidKey.F5, AidKey.F6, AidKey.F7, AidKey.F8,
    AidKey.F9, AidKey.F10, AidKey.F11, AidKey.F12 };

    private static AidKey[] upperAidKeyArray = {AidKey.F13, AidKey.F14, AidKey.F15,
    AidKey.F16, AidKey.F17, AidKey.F18, AidKey.F19, AidKey.F20,
    AidKey.F21, AidKey.F22, AidKey.F23, AidKey.F24 };

    public static AidKey? ToAidKey(this Key Key, bool IsShiftDown = false)
    {
      if (Key == Key.Enter)
        return AidKey.Enter;
      else if (Key == Key.Help)
        return AidKey.Help;
      else if (Key == Key.PageUp)
        return AidKey.RollDown;
      else if (Key == Key.PageDown)
        return AidKey.RollUp;
      else if (Key == Key.PrintScreen)
        return AidKey.Print;
#if skip
      else if (Key == Key.System)
      {
        if (IsShiftDown == true)
          return AidKey.F22;
        else
          return AidKey.F10;
      }
#endif

      else
      {
        int fx = Array.IndexOf(funcKeyArray, Key);
        if (fx == -1)
          return null;
        else if (IsShiftDown == true)
          return upperAidKeyArray[fx];
        else
          return lowerAidKeyArray[fx];
      }
    }
    public static AidKey? ToAidKey(this byte Key)
    {
      if (Key == 0xf1)
        return AidKey.Enter;
      else if (Key == 0xf3)
        return AidKey.Help;
      else if (Key == 0xf4)
        return AidKey.RollDown;
      else if (Key == 0xf5)
        return AidKey.RollUp;
      else if (Key == 0xf6)
        return AidKey.Print;
      else if (Key == 0x31)
        return AidKey.F1;
      else if (Key == 0x32)
        return AidKey.F2;
      else if (Key == 0x33)
        return AidKey.F3;
      else if (Key == 0x34)
        return AidKey.F4;
      else if (Key == 0x35)
        return AidKey.F5;
      else if (Key == 0x36)
        return AidKey.F6;
      else if (Key == 0x37)
        return AidKey.F7;
      else if (Key == 0x38)
        return AidKey.F8;
      else if (Key == 0x39)
        return AidKey.F9;
      else if (Key == 0x3a)
        return AidKey.F10;
      else if (Key == 0x3b)
        return AidKey.F11;
      else if (Key == 0x3c)
        return AidKey.F12;
      else if (Key == 0xb1)
        return AidKey.F13;
      else if (Key == 0xb2)
        return AidKey.F14;
      else if (Key == 0xb3)
        return AidKey.F15;
      else if (Key == 0xb4)
        return AidKey.F16;
      else if (Key == 0xb5)
        return AidKey.F17;
      else if (Key == 0xb6)
        return AidKey.F18;
      else if (Key == 0xb7)
        return AidKey.F19;
      else if (Key == 0xb8)
        return AidKey.F20;
      else if (Key == 0xb9)
        return AidKey.F21;
      else if (Key == 0xba)
        return AidKey.F22;
      else if (Key == 0xbb)
        return AidKey.F23;
      else if (Key == 0xbc)
        return AidKey.F24;
      else if (Key == 0x70)
        return AidKey.applUse1;
      else if (Key == 0x71)
        return AidKey.applUse2;
      else if (Key == 0x7f)
        return AidKey.applUse16;
      else if (Key == 0x88)
        return AidKey.Query5250Reply;
      else
        return null;
    }

    public static bool IsValidAidKeyByte( this byte AidKeyByte)
    {
      if ((AidKeyByte >= 0xf1) && (AidKeyByte <= 0xf6))
        return true;
      else if ((AidKeyByte >= 0x31) && (AidKeyByte <= 0x3c))
        return true;
      else if ((AidKeyByte >= 0xb1) && (AidKeyByte <= 0xbc))
        return true;
      else if (AidKeyByte == 0x88)
        return true;
      else if ((AidKeyByte >= 0x70) && (AidKeyByte <= 0x7f))
        return true;
      else
        return false;
    }

    public static string ToString(this AidKey? Key)
    {
      if (Key == null)
        return "none";
      else
        return Key.Value.ToString();
    }
  }
}


