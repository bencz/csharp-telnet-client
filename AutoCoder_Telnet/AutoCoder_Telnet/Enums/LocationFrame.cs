using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoCoder.Telnet.Enums
{
  /// <summary>
  /// location frame of reference of a point on a canvas.
  /// </summary>
  public enum LocationFrame
  {
    OneBased,
    ZeroBased
  }

  public static class LocationFrameExt
  {
    public static LocationFrame? TryParseLocationFrame(this string Text, LocationFrame? Default = null)
    {
      var rv = TryParse(Text, Default);
      return rv;
    }
    public static LocationFrame? TryParse(string Text, LocationFrame? Default = null)
    {
      var lcText = Text.ToLower();
      if (lcText == "onebased")
        return LocationFrame.OneBased;
      else if (lcText == "zerobased")
        return LocationFrame.ZeroBased;
      else
        return Default;
    }
  }

}
