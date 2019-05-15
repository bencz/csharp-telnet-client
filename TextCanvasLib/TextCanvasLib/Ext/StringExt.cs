using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TextCanvasLib.Ext
{
  public static class StringExt
  {
    public static int? TryParseInt32(this string Text)
    {
      int iv;
      var rc = Int32.TryParse(Text, out iv);
      if (rc == false)
        return null;
      else
        return iv;
    }
  }
}
