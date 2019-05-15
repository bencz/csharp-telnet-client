using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace AutoCoder.File
{
  public class TextFile
  {

    /// <summary>
    /// write line to the end of the text file.
    /// </summary>
    /// <param name="InPath"></param>
    /// <param name="InLine"></param>
    public static void Append(FullPath InPath, string InLine)
    {
      using (StreamWriter sw = new StreamWriter(InPath.ToString(), true))
      {
        sw.WriteLine(InLine);
      }
    }

#if skip
    /// <summary>
    /// copy lines of text file into string[]
    /// </summary>
    /// <param name="InPath"></param>
    /// <returns></returns>
    public static string[] FileToArray( string InPath )
    {
      List<string> listLines = new List<string>();
      using (StreamReader sr = new StreamReader(InPath))
      {
        while (true)
        {
          string line = sr.ReadLine();
          if (line == null)
            break;
          listLines.Add(line);
        }
      }
      return listLines.ToArray();
    }
#endif

#if skip
    /// <summary>
    /// Copy lines of text file into string with NewLine added to end of
    /// each line.
    /// </summary>
    /// <param name="InPath"></param>
    /// <returns></returns>
    public static string FileToString(string InPath)
    {
      StringBuilder sb = new StringBuilder(20000);
      using (StreamReader sr = new StreamReader(InPath))
      {
        while (true)
        {
          string line = sr.ReadLine();
          if (line == null)
            break;
          sb.Append(line + Environment.NewLine);
        }
      }
      return sb.ToString();
    }
#endif

    public static TextLines FileToTextLines(string InPath)
    {
      TextLines lines = new TextLines();
      StringBuilder sb = new StringBuilder(20000);
      using (StreamReader sr = new StreamReader(InPath))
      {
        while (true)
        {
          string line = sr.ReadLine();
          if (line == null)
            break;
          lines.AddLast(line);
        }
      }
      return lines;
    }


  }
}
