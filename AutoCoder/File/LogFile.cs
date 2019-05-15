using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace AutoCoder.File
{
  public static class LogFile
  {
    public static void Write(string FilePath, string Text)
    {
      bool append = true;
      using (StreamWriter sw = new StreamWriter(FilePath, append))
      {
        string fullText = DateTime.Now.ToString() + " " + Text + Environment.NewLine;
        sw.Write(fullText);
      }
    }

    public static void Write(string FilePath, string[] Lines)
    {
      foreach( var line in Lines )
      {
        Write(FilePath, line);
      }
    }
  }
}
