using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace AutoCoder.Ext.System.IO
{
  public static class FileInfoExt
  {
    public static string ViewableLength(this FileInfo FileInfo)
    {
      var lx = FileInfo.Length;
      if (lx < 1000)
        return lx.ToString() + " bytes";
      else if (lx < 1000000000)
      {
        var kbLx = Math.Round(lx / 1000.00,0) ;
        return kbLx.ToString() + " KB";
      }
      else
      {
        var mbLx = lx / 1000000;
        return mbLx.ToString() + " MB";
      }
    }

  }
}
