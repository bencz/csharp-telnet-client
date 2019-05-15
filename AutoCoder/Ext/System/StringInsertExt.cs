using AutoCoder.Core.Enums;
using AutoCoder.Text;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AutoCoder.Ext.System
{
  public static class StringInsertExt
  {

    public static Tuple<string, string> SplitOnInsertPos(
      this string Text, InsertPosition InsertPos)
    {
      string beforeText = null;
      string afterText = null;
      int beforeLgth = 0;
      int afterLgth = 0;

      if (InsertPos.Relative == RelativePosition.Begin)
      {
        beforeLgth = 0;
      }

      else if (InsertPos.Relative == RelativePosition.End)
      {
        beforeLgth = Text.Length;
      }

      else if (InsertPos.Relative == RelativePosition.Before)
      {
        beforeLgth = InsertPos.Value;
      }

      else
      {
        beforeLgth = InsertPos.Value + 1;
      }

      // adjust beforeLgth if it exceeds length of text to split.
      if (beforeLgth > Text.Length)
        beforeLgth = Text.Length;

      // afterLgth is always what remains after beforeLgth.
      afterLgth = Text.Length - beforeLgth;

      // isolate the before and after text.
      if (beforeLgth == 0)
        beforeText = "";
      else
        beforeText = Text.Substring(0, beforeLgth);

      if (afterLgth == 0)
        afterText = "";
      else
        afterText = Text.Substring(beforeLgth);

      return new Tuple<string, string>(beforeText, afterText);
    }


  }
}
