using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoCoder.Telnet.Enums
{
  public enum ShowDtyp
  {
    Char,
    Numeric,
    Date,
    Time,
    none
  } ;

  public static class ShowDtypExt
  {
    public static ShowDtyp? TryParse(string Text)
    {
      var lcText = Text.ToLower();
      if (lcText == "char")
        return ShowDtyp.Char;
      else if (lcText == "numeric")
        return ShowDtyp.Numeric;
      else if (lcText == "date")
        return ShowDtyp.Date;
      else if (lcText == "time")
        return ShowDtyp.Time;
      else
        return null;
    }
  }
}
