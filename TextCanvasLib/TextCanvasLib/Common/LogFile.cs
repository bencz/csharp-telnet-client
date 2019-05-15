using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TextCanvasLib.Common
{

  public enum LogFileAction
  {
    Clear,
    None
  }

  public class LogFile
  {
    public int Counter
    { get; set; }

    public string FilePath
    { get; set; }

    public LogFile(string FilePath, LogFileAction? Action = null )
    {
      this.FilePath = FilePath;

      if ((Action != null ) && (Action.Value == LogFileAction.Clear))
      {
        ClearFile();
      }
    }

    private object _Lock;
    private object LockFlag
    {
      get
      {
        if (_Lock == null)
          _Lock = "abc";
        return _Lock;
      }
    }
    public LogFile()
    {
    }

    /// <summary>
    /// clear the log file.
    /// </summary>
    public void ClearFile()
    {
      System.IO.File.WriteAllText(this.FilePath, "");
    }

    public string[] ReadAllLines()
    {
      var lines = System.IO.File.ReadAllLines(this.FilePath);
      return lines;
    }

    public void Write(string Text)
    {
      lock (this.LockFlag)
      {
        System.IO.File.AppendAllText(
          FilePath, Text + Environment.NewLine);
      }
    }
  }
}

