using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoCoder.Telnet.Enums
{
  public enum ShowUsage
  {
    Input,
    Output,
    Both,
    none
  } ;

  public static class ShowUsageExt
  {
    public static ShowUsage? TryParseShowUsage( this string Text )
    {
      var rv = TryParse(Text);
      return rv;
    }
    public static ShowUsage? TryParse(string Text)
    {
      var lcText = Text.ToLower();
      if (lcText == "input")
        return ShowUsage.Input ;
      else if (lcText == "output")
        return ShowUsage.Output;
      else if (lcText == "both")
        return ShowUsage.Both;
      else
        return null;
    }

    public static bool IsInput(this ShowUsage Usage )
    {
      if (Usage == ShowUsage.Both)
        return true;
      else if (Usage == ShowUsage.Input)
        return true;
      else
        return false;
    }
  }
}
