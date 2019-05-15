using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AutoCoder.Ext
{
  public static class ProcessExt
  {

    /// <summary>
    /// Start a process that runs the Notepad application.
    /// </summary>
    /// <param name="InProcess"></param>
    /// <param name="InFilePath"></param>
    /// <returns></returns>
    public static bool StartNotepad(
      this global::System.Diagnostics.Process InProcess, string InFilePath)
    {
      string exePath =
        Environment.ExpandEnvironmentVariables(@"%SystemRoot%\system32\notepad.exe");
      InProcess.StartInfo.FileName = exePath;

      InProcess.StartInfo.Arguments = InFilePath;

      bool rc = InProcess.Start();
      return rc;
    }
  }
}
