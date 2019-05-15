using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;

namespace AutoCoder.Ext.Windows.Input
{
  public static class KeyboardExt
  {
    /// <summary>
    /// the control key is down. Either left or right shift.
    /// </summary>
    /// <returns></returns>
    public static bool IsControlDown()
    {
      if (Keyboard.IsKeyDown(Key.LeftCtrl) == true)
        return true;
      else if (Keyboard.IsKeyDown(Key.RightCtrl) == true)
        return true;
      else
        return false;
    }

    /// <summary>
    /// the shift key is down. Either left or right shift.
    /// </summary>
    /// <returns></returns>
    public static bool IsShiftDown( )
    {
      if (Keyboard.IsKeyDown(Key.RightShift) == true)
        return true;
      else if (Keyboard.IsKeyDown(Key.LeftShift) == true)
        return true;
      else
        return false;
    }

    /// <summary>
    /// convert the key code and shift state to the string rep of the keyboard 
    /// character.
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    public static string ToTextInput(this Key key)
    {
      string keyText = null;
      int ix = key.ToIndex();
      if (ix == -1)
        keyText = null;
      else if ((Keyboard.IsKeyDown(Key.LeftShift)) 
        || (Keyboard.IsKeyDown(Key.RightShift)))
      {
        string charSet = "ABCDEFGHIJKLMNOPQRSTUVWXYZ)!@#$%^&*( <>:?+_\"";
        keyText = charSet.Substring(ix, 1);
      }
      else
      {
        string charSet = "abcdefghijklmnopqrstuvwxyz0123456789 ,.;/=-'";
        keyText = charSet.Substring(ix, 1);
      }
      return keyText;
    }

    public static int DigitToIndex(this Key key)
    {
      if (key == Key.D0)
        return 0;
      else if (key == Key.D1)
        return 1;
      else if (key == Key.D2)
        return 2;
      else if (key == Key.D3)
        return 3;
      else if (key == Key.D4)
        return 4;
      else if (key == Key.D5)
        return 5;
      else if (key == Key.D6)
        return 6;
      else if (key == Key.D7)
        return 7;
      else if (key == Key.D8)
        return 8;
      else if (key == Key.D9)
        return 9;
      else
        return -1;
    }

    public static int LetterToIndex(this Key key)
    {
      if (key == Key.A)
        return 0;
      else if (key == Key.B)
        return 1;
      else if (key == Key.C)
        return 2;
      else if (key == Key.D)
        return 3;
      else if (key == Key.E)
        return 4;
      else if (key == Key.F)
        return 5;
      else if (key == Key.G)
        return 6;
      else if (key == Key.H)
        return 7;
      else if (key == Key.I)
        return 8;
      else if (key == Key.J)
        return 9;
      else if (key == Key.K)
        return 10;
      else if (key == Key.L)
        return 11;
      else if (key == Key.M)
        return 12;
      else if (key == Key.N)
        return 13;
      else if (key == Key.O)
        return 14;
      else if (key == Key.P)
        return 15;
      else if (key == Key.Q)
        return 16;
      else if (key == Key.R)
        return 17;
      else if (key == Key.S)
        return 18;
      else if (key == Key.T)
        return 19;
      else if (key == Key.U)
        return 20;
      else if (key == Key.V)
        return 21;
      else if (key == Key.W)
        return 22;
      else if (key == Key.X)
        return 23;
      else if (key == Key.Y)
        return 24;
      else if (key == Key.Z)
        return 25;
      else
        return -1;
    }

    /// <summary>
    /// convert the key code of the character key to an index to the array of 
    /// characters, numbers and other keyboard characters.
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    public static int ToIndex(this Key key)
    {
      int ix = key.LetterToIndex();
      if (ix >= 0)
        return ix;

      ix = key.DigitToIndex();
      if (ix >= 0)
        return ix + 26;

      int startIx = 36;

      if (key == Key.Space)
        return startIx + 0;
      else if (key == Key.OemComma)
        return startIx + 1;
      else if (key == Key.OemPeriod)
        return startIx + 2;
      else if (key == Key.Oem1)   // colon and semicolon
        return startIx + 3;
      else if (key == Key.Oem2)
        return startIx + 4;
      else if (key == Key.OemPlus)  // = and +
        return startIx + 5;
      else if (key == Key.OemMinus)  // - and _
        return startIx + 6;
      else if (key == Key.Oem7)  // ' and "
        return startIx + 7;
      else
        return -1;
    }

  }
}
