using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AutoCoder.Ext.System
{
  public static class CharExt
  {
    public static string ToString(char ch1)
    {
      return new string(new char[] { ch1 });
    }

    /// <summary>
    /// is the character printable on the screen or printed page.
    /// </summary>
    /// <param name="ch"></param>
    /// <returns></returns>
    public static bool IsPrintable(this char ch)
    {
      if (Char.IsLetterOrDigit(ch))
        return true;
      else if (Char.IsPunctuation(ch))
        return true;
      else if (ch == ' ')
        return true;
      else if (Char.IsSymbol(ch))
        return true;
      else if (Char.IsControl(ch))  // tab, cr, lf, null or not displayable.
        return false;
      else
        return false;
    }

    /// <summary>
    /// return the char value in hex form.
    /// </summary>
    /// <param name="ch"></param>
    /// <returns></returns>
    public static string ToHex(this char ch)
    {
      var s1 = String.Format("{0:X}", (short)ch);
      return s1.PadLeft(2, '0');
    }
  }
}
