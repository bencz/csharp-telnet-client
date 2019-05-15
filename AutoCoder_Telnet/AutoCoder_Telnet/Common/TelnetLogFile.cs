using AutoCoder.Ext;
using AutoCoder.Ext.System;
using AutoCoder.Telnet.Common;
using AutoCoder.Telnet.Enums;
using AutoCoder.Telnet.TelnetCommands;
using AutoCoder.Telnet.IBM5250;
using AutoCoder.Telnet.TerminalStatements.Vt100;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoCoder.Telnet.IBM5250.WorkstationCommands;
using AutoCoder.Telnet.IBM5250.Common;
using AutoCoder.Systm;
using System.IO;
using AutoCoder.Telnet.IBM5250.Header;

namespace AutoCoder.Telnet.Common
{
  public class TelnetLogFile
  {
    public int Counter
    { get; set; }

    public string FilePath
    { get; set; }

    /// <summary>
    /// generation number. Used when creating a new generation of the file. Inc the
    /// gen nbr and assign the new log file name as the original name with gen nbr
    /// appended to the end.
    /// </summary>
    public int GenerationNumber
    { get; set; }
    private string OrigFilePath
    { get; set; }

    public bool ByteChunkOnly
    { get; set; }

    public TelnetLogFile(string FilePath)
    {
      this.FilePath = FilePath;
      this.ByteChunkOnly = false;
    }

    public DataStreamHeader CurrentDataStreamHeader
    { get; set; }

    public bool TransmitBinaryReadMode
    {
      get;
      set;
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

    public TelnetLogFile()
    {
      this.CurrentDataStreamHeader = null;
    }
    private void AppendAllLines(IEnumerable<string> Lines)
    {
      lock (this.LockFlag)
      {
        System.IO.File.AppendAllLines(FilePath, Lines);
      }
    }

    /// <summary>
    /// clear the log file.
    /// </summary>
    public void ClearFile()
    {
      System.IO.File.WriteAllText(this.FilePath, "");
    }

    public void CreateNewGeneration( )
    {
      this.GenerationNumber += 1;
      if (this.OrigFilePath == null)
        this.OrigFilePath = this.FilePath;
      var dirName = Path.GetDirectoryName(this.OrigFilePath);
      var fileName = Path.GetFileName(this.OrigFilePath);
      var extName = Path.GetExtension(this.OrigFilePath);
      var genFileName = Path.GetFileNameWithoutExtension(this.OrigFilePath) + " " + this.GenerationNumber;
      this.FilePath = Path.Combine(dirName, genFileName + "." + extName);

      System.IO.File.WriteAllText(FilePath, "");

    }

    public string[] ReadAllLines()
    {
      var lines = System.IO.File.ReadAllLines(this.FilePath);
      return lines;
    }

    private static void LogFile_WriteRawBytes(byte[] Buffer, List<string> Lines)
    {
      int ix = 0;
      while (ix < Buffer.Length)
      {
        int remLx = Buffer.Length - ix;
        int lx = 1;

        if ((Buffer[ix] == 0x1b) && (remLx > 1))
        {
          var fx = Buffer.IndexOf(ix + 1, new byte[] { 0x1b });
          if (fx == -1)
            lx = remLx;
          else
            lx = fx - ix;
        }
        else
        {
          lx = IntExt.Min(remLx, 15);
        }

        {
          var sb = new StringBuilder();
          sb.Append(Buffer.ToHex(ix, lx, ' '));
          Lines.Add(sb.ToString());
        }

        {
          var sb = new StringBuilder();
          sb.Append(Buffer.ToAscii(ix, lx, "  "));
          Lines.Add(sb.ToString());
        }

        ix += lx;
      }
    }

    public void WriteGapLine( List<string> Lines)
    {
      Lines.Add("");
    }

    public void WriteImportantItem( Direction Direction, string Text )
    {
      var item = new LogListItem(Direction, Text);
      item.IsImportant = true;
      LogListItem[] items = new LogListItem[] { item };
      WriteItems(items);
    }

    public void WriteItems(IEnumerable<LogListItem> Items)
    {
      var lines = new List<string>();
      foreach (var item in Items)
      {
        if (item.Chunk != null)
        {
          WriteGapLine(lines);
          InternalAddLine(lines, item.Direction.ToString(), 
            item.LogTime.ToString("HH:mm:ss.fff") + " " + item.ChunkText.EmptyIfNull());
          lines.AddRange(item.Chunk.ToHexReport(32));
          WriteGapLine(lines);
        }

        else if (item.IsImportant == true )
        {
          InternalAddLine( lines, item.Direction.ToString(), 
            item.LogTime.ToString("HH:mm:ss.fff") + " " + item.Text + " // important" );
        }

        else if (this.ByteChunkOnly == true)
        {
        }

        else
        {

          if (item.LogSpecial != null)
          {
            if (item.LogSpecial.Value == LogItemSpecial.ClearLog)
              this.ClearFile();
            else if (item.LogSpecial.Value == LogItemSpecial.NewGeneration)
              this.CreateNewGeneration();
          }

          if (item.NewGroup == true)
            lines.Add("");
          InternalAddLine( lines, item.Direction.ToString(), item.Text);
        }
      }
      AppendAllLines(lines);
    }
    public void Write(string Direction, string Text)
    {
      lock (this.LockFlag)
      {
        this.Counter += 1;

        System.IO.File.AppendAllText(
          this.FilePath, Direction + " " + this.Counter.ToString()
          + " " + Text + Environment.NewLine);
      }
    }

    public void Write(string Direction, byte[] Buffer)
    {
      Write(Direction, Buffer, Buffer.Length);
    }

    /// <summary>
    /// write buffer contents to log file.
    /// </summary>
    /// <param name="Direction"></param>
    /// <param name="Buffer"></param>
    /// <param name="Length"></param>
    public void Write(string Direction, byte[] Buffer, int Length)
    {
      var lines = new List<string>();
      this.Counter += 1;

      var inputArray = new InputByteArray(Buffer, Length);

      while (inputArray.IsEof() == false)
      {

        // peek to see if the current byte is a telnet command.
        var escapeCmd = inputArray.PeekNextByte().ToTelnetCommandCode();

        // peek to see if the current bytes are an IBM5250 datastream header.
        DataStreamHeader dsh = null;
        string errmsg = null;
        {
          var rv = DataStreamHeader.Factory(inputArray);
          dsh = rv.Item1;
          errmsg = rv.Item2;
        }

        // current bytes are an ibm5250 datastream header. ( page 2-4 of rfc 1205 )
        // store this header as the "current datastream header". When there is a
        // current data stream header the code will match the current bytes against
        // ibm5250 data stream commands.
        if (errmsg == null)
        {
          lines.Add(
            Direction + " " + this.Counter.ToString() + " "
            + dsh.ToString());

          this.CurrentDataStreamHeader = dsh;

          continue;
        }

        // currently in a 5250 data stream header.
        if (this.CurrentDataStreamHeader != null)
        {
          var dataStreamCommand = WorkstationCommandBase.ParseFactory(inputArray);
          if (dataStreamCommand != null)
          {
            lines.Add(
              Direction + " " + this.Counter.ToString() + " "
              + dataStreamCommand.ToString());

            // all the data stream command ToString methods provide enough info.
            // But for the WriteToDisplayCommand, list out the ToString contents of
            // the WtdOrders of that command.
            if (dataStreamCommand is WriteToDisplayCommand)
            {
              var wtdCommand = dataStreamCommand as WriteToDisplayCommand;
              foreach (var order in wtdCommand.OrderList)
              {
                lines.Add(order.ToString());
              }
            }

            continue;
          }
        }

        {
          var stmt = TelnetCommand.Factory(inputArray);
          if (stmt != null)
          {
            lines.Add(
              Direction + " " + this.Counter.ToString() + " " + stmt.ToString());
            continue;
          }
        }

        if ((escapeCmd != null) && (escapeCmd.Value == CommandCode.ESCAPE))
        {
          var rv = TerminalVt100Statement.Factory(inputArray);
          var vtStmt = rv.Item1;
          var otStmt = rv.Item2;

          lines.Add(
            Direction + " " + this.Counter.ToString() + " " + vtStmt.ToString());

          if (otStmt != null)
          {
            lines.Add(
              Direction + " " + this.Counter.ToString() + " " + otStmt.ToString());
          }

          continue;
        }

        {
          var remBytes = inputArray.GetBytesToEnd();
          lines.Add( Direction + " " + this.Counter.ToString()
            + " Raw bytes follow:");
          LogFile_WriteRawBytes(remBytes, lines);
        }
      }
      WriteTextLines(lines);
    }

    private void InternalAddLine( List<string> Lines, string Direction, string Text )
    {
      Lines.Add(Direction + " " + Text);
    }
    public void WriteTextLine(string Direction, string Text)
    {
      WriteTextLine(Direction + " " + Text);
    }

    public void WriteTextLine(string Text)
    {
      lock (this.LockFlag)
      {
        System.IO.File.AppendAllText(
          FilePath, Text + Environment.NewLine);
      }
    }
    public void WriteTextLines(IEnumerable<string> Lines)
    {
      this.AppendAllLines(Lines);
    }
  }
}


