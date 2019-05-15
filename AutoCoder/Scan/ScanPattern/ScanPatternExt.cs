using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AutoCoder.Scan
{
  public static class ScanPatternExt
  {
    public static bool IsPatternStartChar(this ScanPattern Pattern, char Char)
    {
      bool rv = false;
      if (Pattern == null)
        rv = false;
      else if (Char == Pattern.LeadChar)
        rv = true;
      else
        rv = false;
      return rv;
    }
  }
}
