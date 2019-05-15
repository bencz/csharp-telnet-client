using AutoCoder.Ext;
using AutoCoder.Ext.System;
using AutoCoder.Telnet.Common;
using AutoCoder.Telnet.Enums;
using AutoCoder.Telnet.Statements;
using AutoCoder.Telnet.TerminalStatements.IBM5250;
using AutoCoder.Telnet.TerminalStatements.Vt100;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoCoder.Telnet.Common
{
  public class TelnetLogFile
  {
    public int Counter
    { get; set; }

    public string FilePath
    { get; set; }

    public TelnetLogFile(string FilePath)
    {
      this.FilePath = FilePath;
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

    /// <summary>
    /// clear the log file.
    /// </summary>
    public void ClearFile()
    {
      System.IO.File.WriteAllText(this.FilePath, "");
    }

    private static void LogFile_WriteRawBytes(byte[] Buffer, string FilePath)
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
          System.IO.File.AppendAllText(
            FilePath, sb.ToString() + Environment.NewLine);
        }

        {
          var sb = new StringBuilder();
          sb.Append(Buffer.ToAscii(ix, lx, "  "));
          System.IO.File.AppendAllText(
            FilePath, sb.ToString() + Environment.NewLine);
        }

        ix += lx;
      }
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
      lock (this.LockFlag)
      {

        this.Counter += 1;
      if (this.Counter > 7)
      {
        int cc = 3;
      }

        InputByteArray inputArray = new InputByteArray(Buffer, Length);

        while (inputArray.IsEof() == false)
        {
          TelnetStatement stmt = null;
          var escapeCmd = inputArray.PeekNextByte().ToTelnetCommand();
          var dsHeader = DataStreamHeader.Factory(inputArray);
          IBM5250DataStreamCommand dataStreamCommand;

          if (dsHeader != null)
          {
            var s1 = dsHeader.ToString();
            System.IO.File.AppendAllText(
              FilePath, Direction + " " + this.Counter.ToString() + " "
              + s1 + Environment.NewLine);

            this.CurrentDataStreamHeader = dsHeader;

            continue;
          }

            // an ibm 5250 data stream command.
          else if ((this.CurrentDataStreamHeader != null ) 
            && ((dataStreamCommand = IBM5250DataStreamCommand.Factory(inputArray))
            != null ))
          {
            var s1 = dataStreamCommand.ToString();
            System.IO.File.AppendAllText(
              FilePath, Direction + " " + this.Counter.ToString() + " "
              + s1 + Environment.NewLine);
            continue;
          }

          else if ((stmt = TelnetStatement.Factory(inputArray)) != null)
          {
            var s1 = stmt.ToString();
            System.IO.File.AppendAllText(
              FilePath, Direction + " " + this.Counter.ToString() + " "
              + s1 + Environment.NewLine);
            continue;
          }

          else if ((escapeCmd != null) && (escapeCmd.Value == TelnetCommand.ESCAPE))
          {
            var rv = TerminalVt100Statement.Factory(inputArray);
            var vtStmt = rv.Item1;
            var otStmt = rv.Item2;

            System.IO.File.AppendAllText(
              this.FilePath, Direction + " " + this.Counter.ToString() + " "
              + vtStmt.ToString() + Environment.NewLine);

            if (otStmt != null)
            {
              System.IO.File.AppendAllText(
                this.FilePath, Direction + " " + this.Counter.ToString() + " "
                + otStmt.ToString() + Environment.NewLine);
            }

            continue;
          }

          else
          {
            var remBytes = inputArray.GetBytesToEnd();
            System.IO.File.AppendAllText(
              this.FilePath, Direction + " " + this.Counter.ToString()
              + " Raw bytes follow:" + Environment.NewLine);
            LogFile_WriteRawBytes(remBytes, this.FilePath);
          }
        }
      }

    }
  }
}


