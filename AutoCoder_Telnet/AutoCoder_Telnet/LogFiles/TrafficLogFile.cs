using AutoCoder.Ext.System;
using AutoCoder.Telnet.Enums;
using AutoCoder.Telnet.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace AutoCoder.Telnet.LogFiles
{
  /// <summary>
  /// log file containing the byte traffic sent and received between the
  /// telnet client and server.
  /// </summary>
  public class TrafficLogFile
  {
    public DateTime LogTime
    { get; set; }

    public byte[] ByteStream
    { get; set; }

    public Direction Direction
    { get; set; }

    public static string FilePath
    {
      get { return "c:\\downloads\\telnetTraffic.txt"; }
    }

    public static void ClearFile()
    {
      lock (FilePath)
      {
        System.IO.File.WriteAllText(FilePath, "");
      }
    }

    public static void Write(Direction Direction, byte[] ByteStream)
    {
      var logItem = new TrafficLogItem(Direction, ByteStream);
      var xelem = logItem.ToXElement("TrafficItem");
      AppendAllText(xelem.ToString() + Environment.NewLine);
    }

    private static void AppendAllText(string Text)
    {
      lock (FilePath)
      {
        System.IO.File.AppendAllText(FilePath, Text);
      }
    }

    public static IEnumerable<string> ToTextLines( )
    {
      IEnumerable<string> lines = null;
      lock(FilePath)
      { 
        lines = System.IO.File.ReadAllLines(FilePath);
      }
      return lines;
    }

    public static IEnumerable<TrafficLogItemTreeModel> ToLogItemList( )
    {
      var sb = new StringBuilder();
      foreach( var textLine in ToTextLines( ))
      {
        sb.Append(textLine);
        if ( textLine == "</TrafficItem>")
        {
          var item = sb.ToString().ToTrafficLogItem();
          yield return new TrafficLogItemTreeModel(item);
          sb.Clear();
        }
      }
      yield break;
    }
  }
}
