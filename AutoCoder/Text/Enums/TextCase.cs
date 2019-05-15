using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AutoCoder.Text.Enums
{
  public enum TextCase
  {
    // case is significant. Compare char to char, if case is different, compare
    // returns false.
    SameCase,

    // case is not significant. Upper case char compares equal to lower case
    // version of that same letter.
    MonoCase
  }
}
