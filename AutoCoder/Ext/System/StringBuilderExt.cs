using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AutoCoder.Ext.System
{
  public static class StringBuilderExt
  {

    /// <summary>
    /// append the input text to the stringBuilder. If the last char of the string
    /// builder is not a space, append the space before appending the text.
    /// </summary>
    /// <param name="Text"></param>
    /// <param name="AppendText"></param>
    public static void SpaceSeparatorAppend(this StringBuilder Text, string AppendText)
    {
      if ((Text.Length > 0) && (Text.Tail(1) != " "))
        Text.Append(' ');
      Text.Append(AppendText);
    }

    /// <summary>
    /// Return the ending chars from the StringBuilder.
    /// </summary>
    /// <param name="Text"></param>
    /// <param name="Length"></param>
    /// <returns></returns>
    public static string Tail(this StringBuilder Text, int Length)
    {
      int Lx = Length;
      if (Text.Length < Length)
        Lx = Text.Length;
      if (Lx == 0)
        return "";
      else
      {
        int Bx = Text.Length - Lx;
        return (Text.ToString(Bx, Lx));
      }
    }

  }
}
