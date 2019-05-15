using AutoCoder.Ext.System;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoCoder.Telnet.LogFiles
{
  public static class SpecialLogFile
  {
    public static string FilePath
    {
      get {  return "c:\\downloads\\specialLog.txt" ; }
    }

    public static void ClearFile( )
    {
      lock (FilePath)
      {
        System.IO.File.WriteAllText(FilePath, "");
      }
    }

    public static void AppendTextLine( string TextLine)
    {
      AppendAllText(TextLine + Environment.NewLine);
    }

    public static void AppendTextLines( 
      IEnumerable<string> Lines, string OverrideFilePath = null)
    {
      var sb = new StringBuilder();
      foreach( var line in Lines)
      {
        sb.Append(line + Environment.NewLine);
      }
      AppendAllText(sb.ToString(), OverrideFilePath);
    }

    public static void AppendTitleLine( string TitleText )
    {
      var text = "------------" + DateTime.Now.ToMillisecondString( )
        + " " + TitleText + "--------------";
      AppendTextLine(text);
    }

    private static void AppendAllText( string Text, string OverrideFilePath = null )
    {
      var filePath = FilePath;
      if (OverrideFilePath != null)
        filePath = OverrideFilePath;

      lock(filePath)
      {
        System.IO.File.AppendAllText(filePath, Text);
      }
    }
  }
}
