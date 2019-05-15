using global::System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AutoCoder.Core
{
  public static class LogCommon
  {
    public static void OpenNew(string LogFilePath, bool Archive = false)
    {
      // archive the existing file.
      if ((Archive == true ) && (global::System.IO.File.Exists(LogFilePath)))
      {
        string s1 = DateTime.Now.ToString("yyyy-MM-dd_HH.mm");
        var arcFileName = global::System.IO.Path.GetFileNameWithoutExtension(LogFilePath)
          + "_" + s1
          + global::System.IO.Path.GetExtension(LogFilePath);
        var arcPath = global::System.IO.Path.Combine(
          global::System.IO.Path.GetDirectoryName(LogFilePath), arcFileName);

        if (global::System.IO.File.Exists(arcPath))
        {
        global::System.IO.File.Delete(arcPath);
        }

      global::System.IO.File.Move(LogFilePath, arcPath);
      }

      // write an empty string to the file. effectively clears the file.
      System.IO.File.WriteAllText(LogFilePath, "");
    }

    public static void WriteToLog(string LogFilePath, string Message)
    {
      return;
#if skip
      System.IO.File.AppendAllText(
        LogFilePath, DateTime.Now.ToString("yyyy-MM-dd HH.mm.ss.ffff") + " "
        + Message + Environment.NewLine);
#endif
    }
  }
}
