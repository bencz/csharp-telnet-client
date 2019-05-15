using ScreenDefnLib.CopyPaste;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScreenDefnLib.Common
{
  /// <summary>
  /// a static class. Intended to be accessed anywhere from the tnClient code.
  /// Contains the CopyPasteList used when copying and pasting ScreenItems from
  /// one list of screen items to another.
  /// </summary>
  public static class ScreenDefnGlobal
  {
    public static CopyPasteList CopyPasteList
    { get; private set; }

    static ScreenDefnGlobal( )
    {
      ScreenDefnGlobal.CopyPasteList = new CopyPasteList(true);
    }
  }
}
