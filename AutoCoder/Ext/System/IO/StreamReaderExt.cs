using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace AutoCoder.Ext.System.IO
{
  public static class StreamReaderExt
  {

    public static string ReadFirstLine(string FilePath)
    {
      string fsLine = null;
      using (var sr = new StreamReader(FilePath))
      {
        fsLine = sr.ReadLine();
      }
      return fsLine;
    }

  }
}
