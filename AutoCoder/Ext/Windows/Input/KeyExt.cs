using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;
using System.Xml.Linq;

namespace AutoCoder.Ext.Windows.Input
{
  public static class KeyExt
  {
    public static bool EqualAny(this Key Key, params Key[] Codes)
    {
      foreach (var code in Codes)
      {
        if (Key == code)
          return true;
      }
      return false;
    }

    public static bool IsLetterKey(this Key key)
    {
      bool isLetter = false;
      if ((key >= Key.A) && (key <= Key.Z))
      {
        isLetter = true;
      }
      return isLetter;
    }

    public static bool IsTopRowNumberKey(this Key key )
    {
      bool isNumber = false;
      if ((key >= Key.D0) && (key <= Key.D9))
        isNumber = true;
      return isNumber;
    }

    public static Key? ToKeyOrDefault(this XElement elem, Key? dft = null)
    {
      if (elem == null)
        return dft;
      else
      {
        var text = elem.StringOrDefault("");
        var key = text.TryParse();
        return key;
      }
    }

    /// <summary>
    /// another possible name - KeyNameToKeyEnum
    /// </summary>
    /// <param name="KeyName"></param>
    /// <returns></returns>
    public static Key? TryParse(this string KeyName)
    {
      Key res;
      var rc = Enum.TryParse<Key>(KeyName, out res);
      if (rc == true)
        return res;
      else if (KeyName == "Caps")
        return Key.CapsLock;
      else
        return null;
    }

    public static char ToChar(this Key key, bool shiftDown = false)
    {
      char c = '\0';

      if (key.IsLetterKey() == true)
      {
        char baseChar = 'a';
        if (shiftDown == true)
          baseChar = 'A';

        c = (char)((int)baseChar + (int)(key - Key.A));
      }

      else if ( key.IsTopRowNumberKey( ) == true)
      {
        var ix = (int)(key - Key.D0);
        if (shiftDown == false)
          c = LowerCaseTopRowNumberKeys[ix];
        else
          c = UpperCaseTopRowNumberKeys[ix];
      }

      return c;
    }

    /// <summary>
    /// upper case characters typed when keys D0 - D9 are pressed.
    /// </summary>
    static char[] UpperCaseTopRowNumberKeys = { ')', '!', '@', '#', '$', '%', '^', '&', '*', '(' };
    static char[] LowerCaseTopRowNumberKeys = { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9' };
  }
}
