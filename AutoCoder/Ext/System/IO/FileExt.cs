using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AutoCoder.Ext.System.IO
{
  public static class FileExt
  {
    /// <summary>
    /// ReadAllText. If IOException ( file in use ), delay and then retry the read.
    /// </summary>
    /// <param name="FilePath"></param>
    /// <param name="RetryLimit"></param>
    /// <param name="RetryDelay"></param>
    /// <returns></returns>
    public static string ReadAllText(string FilePath, int RetryLimit, int RetryDelay = 1)
    {
      string allText = null;
      int retryCx = 0;
      while (true)
      {
        try
        {
          allText = global::System.IO.File.ReadAllText(FilePath);
          return allText;
        }
        catch (global::System.IO.IOException excp)
        {
          retryCx += 1;
          if (retryCx >= RetryLimit)
            throw excp;

          global::System.Threading.Thread.Sleep(RetryDelay);
        }
      }
    }

    /// <summary>
    /// check if simple file name exists in directory, checking for each of a list
    /// of possible extensions.
    /// </summary>
    /// <param name="DirPath"></param>
    /// <param name="SimpleName"></param>
    /// <param name="extensions"></param>
    /// <returns></returns>
    public static string ResolveSimpleName(string DirPath, string SimpleName, string[] extensions)
    {
      string filePath = null;
      foreach( var extension in extensions)
      {
        var fileName = SimpleName + "." + extension;
        var checkPath = global::System.IO.Path.Combine(DirPath, fileName);
        if ( global::System.IO.File.Exists(checkPath) == true )
        {
          filePath = checkPath;
          break;
        }
      }
      return filePath;
    }
  }
}
