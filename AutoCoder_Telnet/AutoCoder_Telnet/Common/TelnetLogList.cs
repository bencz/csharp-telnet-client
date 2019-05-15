using AutoCoder.Ext.System;
using AutoCoder.Systm;
using AutoCoder.Telnet.TelnetCommands;
using AutoCoder.Telnet.Enums;
using AutoCoder.Telnet.IBM5250.Common;
using AutoCoder.Telnet.IBM5250.WorkstationCommands;
using AutoCoder.Telnet.TerminalStatements.Vt100;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoCoder.Ext.System.Collections.Generic;
using AutoCoder.Core.Interfaces;
using AutoCoder.Telnet.IBM5250.Header;

namespace AutoCoder.Telnet.Common
{
  public class TelnetLogList : List<LogListItem>, ILogList
  {
    public string ListName
    { get; set; }
    public string TextDesc
    { get; set; }

    public TelnetLogList( )
    {
    }

    public TelnetLogList(string ListName = "")
      : base( )
    {
      this.ListName = ListName;
    }
    public void AddChunk(Direction Direction, byte[] Chunk, string ChunkText = null )
    {
      var item = new LogListItem(Direction, Chunk, ChunkText);
      this.Add(item);
    }
    public LogListItem AddItem( Direction Direction, string Text, bool NewGroup = false )
    {
      var item = new LogListItem(Direction, Text, NewGroup);
      this.Add(item);
      return item;
    }
    public LogListItem AddImportantItem(Direction Direction, string Text, bool NewGroup = false)
    {
      var logItem = this.AddItem(Direction, Text, NewGroup);
      logItem.IsImportant = true;
      return logItem;
    }

    public void AddItems(TelnetLogList LogList)
    {
      if (LogList != null)
      {
        foreach (var item in LogList)
        {
          this.Add(item);
        }
      }
    }

    public void AddItems(Direction Direction, IEnumerable<string> List, bool NewGroup = false )
    {
      bool newGroup = NewGroup;
      foreach ( var line in List)
      {
        AddItem(Direction, line, newGroup );
        newGroup = false;
      }
    }

    public void AddReport( Direction Direction, IEnumerable<string> Report)
    {
      foreach( var line in Report)
      {
        var item = new LogListItem(Direction, line);
        AddItem(item);
      }
    }

    public void AddSpecialItem( LogItemSpecial LogSpecial)
    {
      var item = new LogListItem(LogSpecial);
      this.Add(item);
    }

    public void Write(Direction Direction, byte[] Buffer)
    {
      Write(Direction, Buffer, Buffer.Length);
    }

    /// <summary>
    /// write buffer contents to log file.
    /// </summary>
    /// <param name="Direction"></param>
    /// <param name="Buffer"></param>
    /// <param name="Length"></param>
    public void Write(Direction Direction, byte[] Buffer, int Length)
    {
      var inputArray = new InputByteArray(Buffer, Length);
      DataStreamHeader currentDataStreamHeader = null;

      while (inputArray.IsEof() == false)
      {

        // peek to see if the current byte is a telnet command.
        var escapeCmd = inputArray.PeekNextByte().ToTelnetCommandCode();

        // peek to see if the current bytes are an IBM5250 datastream header.
        DataStreamHeader dsh = null;
        string errmsg = null;
        {
          var rv = DataStreamHeader.Factory(inputArray);
          errmsg = rv.Item2;
          dsh = rv.Item1;
        }

        // current bytes are an ibm5250 datastream header. ( page 2-4 of rfc 1205 )
        // store this header as the "current datastream header". When there is a
        // current data stream header the code will match the current bytes against
        // ibm5250 data stream commands.
        if (errmsg == null)
        {
          this.AddItem(Direction, dsh.ToString());
          currentDataStreamHeader = dsh;
          continue;
        }

        // currently in a 5250 data stream header.
        if (currentDataStreamHeader != null)
        {
          var dataStreamCommand = WorkstationCommandBase.ParseFactory(inputArray);
          if (dataStreamCommand != null)
          {
            this.AddItem(Direction, dataStreamCommand.ToString());

            // all the data stream command ToString methods provide enough info.
            // But for the WriteToDisplayCommand, list out the ToString contents of
            // the WtdOrders of that command.
            if (dataStreamCommand is WriteToDisplayCommand)
            {
              var wtdCommand = dataStreamCommand as WriteToDisplayCommand;
              foreach (var order in wtdCommand.OrderList)
              {
                this.AddItem(Direction, order.ToString());
              }
            }

            continue;
          }
        }

        {
          var stmt = TelnetCommand.Factory(inputArray);
          if (stmt != null)
          {
            this.AddItem(Direction, stmt.ToString());
            continue;
          }
        }

        if ((escapeCmd != null) && (escapeCmd.Value == CommandCode.ESCAPE))
        {
          var rv = TerminalVt100Statement.Factory(inputArray);
          var vtStmt = rv.Item1;
          var otStmt = rv.Item2;

          this.AddItem(Direction, vtStmt.ToString());

          if (otStmt != null)
          {
            this.AddItem(Direction, otStmt.ToString());
          }

          continue;
        }

        {
          var remBytes = inputArray.GetBytesToEnd();
          this.AddItem(Direction, "Raw bytes follow:");
          this.AddItems(Direction, remBytes.ToHexReport());
        }
      }
    }

    public void AddItem(ILogItem item)
    {
      this.Add(item as LogListItem);
    }

    public ILogItem GetItem(int index)
    {
      return this[index];
    }

    public IEnumerable<ILogItem> LogItems( )
    {
      return this;
    }

    public ILogList NewList(string ListName = "")
    {
      return new TelnetLogList(ListName);
    }

    IEnumerator<ILogItem> ILogList.GetEnumerator()
    {
      return base.GetEnumerator();
    }
  }
}
