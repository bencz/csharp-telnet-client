using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AutoCoder.Text.Location
{

#if skip

  /// <summary>
  /// list used to convert stream location to TextLocation
  /// </summary>
  public class LineLookup : List<LineLocation>
  {

    public LineLocation FindLineLocation(LineIdentifier LineId)
    {
      LineLocation found = null;
      foreach (var loc in this)
      {
        if (loc.LineId.Equals(LineId))
        {
          found = loc;
          break;
        }
      }

      if (found == null)
        throw new ApplicationException("Line not found");

      return found;
    }
  }

#endif

}
