using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AutoCoder.Ext.System.Text
{
  public static class StringBuilderExt
  {

    public static StringBuilder AppendRepeat(
      this StringBuilder Builder, char Text, int RepeatCount)
    {
      for (int ix = 0; ix < RepeatCount; ++ix)
      {
        Builder.Append(Text);
      }
      return Builder;
    }

    public static StringBuilder AppendRepeat(
      this StringBuilder Builder, string Text, int RepeatCount)
    {
      for (int ix = 0; ix < RepeatCount; ++ix)
      {
        Builder.Append(Text);
      }
      return Builder;
    }

    /// <summary>
    /// Append to a StringBuilder. If the StringBuilder already contains some text,
    /// first append a space before appending the value. 
    /// </summary>
    /// <param name="InBuilder"></param>
    /// <param name="InValue"></param>
    public static void SentenceAppend(this StringBuilder InBuilder, string InValue)
    {
      if (InBuilder.Length > 0)
        InBuilder.Append(" ");
      InBuilder.Append(InValue);
    }

  }
}
