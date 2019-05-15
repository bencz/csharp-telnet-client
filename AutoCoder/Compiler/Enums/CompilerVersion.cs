using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

#if skip

namespace AutoCoder.Compiler.Enums
{
  public enum CompilerVersion
  {
    v35,
    v40
  }

  public static class CompilerVersionExt
  {
    public static string ToCodeDomString(this CompilerVersion CompilerVersion)
    {
      switch (CompilerVersion)
      {
        case Enums.CompilerVersion.v35:
          return "v3.5";
        case Enums.CompilerVersion.v40:
          return "v4.0";
        default:
          throw new ApplicationException("unsupported CompilerVersion " + CompilerVersion.ToString());
      }
    }

  }
}

#endif
