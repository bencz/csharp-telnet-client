using AutoCoder.Ext.System;
using AutoCoder.Systm;
using AutoCoder.Telnet.Cipher;
using AutoCoder.Telnet.Common;
using AutoCoder.Telnet.Enums;
using AutoCoder.Telnet.Enums.IBM5250;
using AutoCoder.Telnet.IBM5250.Common;
using AutoCoder.Telnet.IBM5250.WorkstationCommands;
using AutoCoder.Telnet.IBM5250.WtdOrders;
using AutoCoder.Telnet.IBM5250.Response;
using AutoCoder.Telnet.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using TextCanvasLib.Enums;
using TextCanvasLib.xml;
using AutoCoder.Telnet.Connect;

namespace AutoCoder.Telnet.IBM5250
{
  /// <summary>
  /// read and process an IBM 5250 telnet data stream.
  /// Class provides a Process method which reads the 5250 data stream, parses and
  /// processes it.  The Process method returns a field map which is used to draw
  /// and accept input from the 5250 display screen.
  /// See 15.2 in 5494 reference. table 42 - workstation data stream commands.
  /// </summary>
  public static class Process5250
  {
    public static Tuple<List<ShowItemBase>, TelnetLogList> NextServerRequest(
      NetworkStreamBackedInputByteArray InputArray,
      NegotiateSettings NegotiateSettings,
      bool ForceRead = false)
    {
      TelnetLogList logList = new TelnetLogList();
      var showItemList = new ShowItemList();

      while (true)
      {
        // read available bytes from input stream.
        if (InputArray.IsEof())
        {
          var log = InputArray.ReadFromNetworkStream(5, 5, ForceRead);
          logList.AddItems(log);
        }

        // no input data to process.
        if (InputArray.IsEof())
          break;

        var buf = InputArray.PeekDataStreamHeader();
        if (buf != null)
        {
          var dsh = new DataStreamHeader(InputArray);
          logList.AddItems(Direction.Read, dsh.ToReportLines( ), true);

          var rv = ParseAndProcessWorkstationCommand(InputArray, dsh);
          var workstationCommand = rv.Item1;
          showItemList = rv.Item2;
          logList.AddItems(rv.Item4);
        }

        // check for IAC EOR
        {
          var telCode = InputArray.PeekTelnetCommandCode(CommandCode.EOR);
          if (telCode != null)
          {
            var telCmd = InputArray.NextTelnetCommand();
            logList.AddItems(Direction.Read, telCmd.ToReportLines(), true);
          }
        }

        // peek and process input bytes as a telnet stmt. ( starts with IAC )
        {
          var telCmd = InputArray.NextTelnetCommand();
          if (telCmd != null)
          {

            var rv = TelnetConnection.ProcessTelnetCommand(telCmd, NegotiateSettings);
            var cx = rv.Item1;
            var writeBytes = rv.Item2;
            logList.AddItems(rv.Item3);
            WriteToHost(logList, writeBytes, InputArray.NetStream);
          }
        } 
      }
      return new Tuple<List<ShowItemBase>, TelnetLogList>(showItemList, logList);
    }

    public static Tuple<WorkstationCommandList,TelnetLogList> GetAndParseWorkstationCommandList(
      NetworkStreamBackedInputByteArray InputArray,
      SessionSettings SessionSettings)
    {
      TelnetLogList logList = new TelnetLogList();
      var wrkstnCmdList = new WorkstationCommandList();

      var dsh = new DataStreamHeader(InputArray);
      if (dsh != null)
      {
        logList.AddItems(Direction.Read, dsh.ToReportLines( ), true);
      }

      if ((dsh != null) && (dsh.Errmsg == null))
      {
        bool lastCmdWasTelnet_EOR = false;
        while (dsh != null)
        {
          // read available bytes from input stream.
          if (InputArray.IsEof() && (lastCmdWasTelnet_EOR == false ))
          {
            var log = InputArray.ReadFromNetworkStream(5, 5);
            logList.AddItems(log);
          }

          // no input data to process.
          if (InputArray.IsEof())
            break;
          lastCmdWasTelnet_EOR = false;

          // check for IAC EOR
          var telCode = InputArray.PeekTelnetCommandCode(CommandCode.EOR);
          if (telCode != null)
          {
            var telCmd = InputArray.NextTelnetCommand();
            logList.AddItems(Direction.Read, telCmd.ToReportLines(), true);
            lastCmdWasTelnet_EOR = true;
          }

          // process the input as workstation data stream commands.
          else
          {
            var rv = ParseAndProcessWorkstationCommand(InputArray, dsh);
            var workstationCommand = rv.Item1;
            dsh = rv.Item3;
            logList.AddItems(rv.Item4);

            if ( workstationCommand != null )
              wrkstnCmdList.Add(workstationCommand);
          }
        }
      }
      return new Tuple<WorkstationCommandList, TelnetLogList>(wrkstnCmdList, logList);
    }

    public static Tuple<int, int, List<ShowItemBase>, TelnetLogList> GetAndProcessStream(
      TcpClient Client, NetworkStream NetStream, 
      NetworkStreamBackedInputByteArray InputArray,
      NegotiateSettings NegotiateSettings,
      bool ForceRead = false)
    {
      int sendStmtCx = 0;
      int getStmtCx = 0;
      var logList = new TelnetLogList();
      var showItemList = new ShowItemList();
      bool exitLoop = false;

      while (exitLoop == false )
      {
        // read available bytes from input stream.
        if (InputArray.IsEof())
        {
          var log = InputArray.ReadFromNetworkStream(5, 5, ForceRead);
          logList.AddItems(log);
        }

        // no input data to process.
        if (InputArray.IsEof())
          break;

        DataStreamHeader currentDataStreamHeader = null;
        while (true)
        {
          if (InputArray.IsEof() == true)
            break;

          // process the input as 5250 data stream commands.
          if (currentDataStreamHeader != null)
          {
            var rv = ParseAndProcessWorkstationCommand(InputArray, currentDataStreamHeader);
            var workstationCommand = rv.Item1;
            showItemList = rv.Item2;
            currentDataStreamHeader = rv.Item3;
            logList.AddItems(rv.Item4);
            continue;
          }

          // peek and process input bytes as a telnet stmt. ( starts with IAC )
          var telCmd = InputArray.NextTelnetCommand();
          if (telCmd != null)
          {
            getStmtCx += 1;

            var rv = TelnetConnection.ProcessTelnetCommand(telCmd, NegotiateSettings);
            var cx = rv.Item1;
            var writeBytes = rv.Item2;
            logList.AddItems(rv.Item3);
            sendStmtCx += cx;
            WriteToHost(logList, writeBytes, NetStream);

            continue;
          }

          var dsh = new DataStreamHeader(InputArray);
          if ((dsh != null) && (dsh.Errmsg == null))
          {
            logList.AddItem(Direction.Read, dsh.ToString());
            currentDataStreamHeader = dsh;
            continue;
          }
          exitLoop = true;
          break;
        }
      }
      return new Tuple<int, int, List<ShowItemBase>, TelnetLogList>(
        getStmtCx, sendStmtCx, showItemList, logList);
    }

    /// <summary>
    /// parse the 5250 data stream that is sent from the client to the server.
    /// </summary>
    /// <param name="LogFile"></param>
    /// <param name="ToServerStream"></param>
    /// <returns></returns>
    public static Tuple<object[], string> ParseResponseStream(
      TelnetLogList LogList, byte[] ResponseStream)
    {
      List<object> listItems = new List<object>();
      string errmsg = null;

      var inputArray = 
        new NetworkStreamBackedInputByteArray(ResponseStream, ResponseStream.Length);
      var writeStream = new ByteArrayBuilder();
      DataStreamHeader dsHeader = null;
      ResponseHeader responseHeader = null;

      // stream starts with data stream header.
        dsHeader = new DataStreamHeader(inputArray);
      listItems.Add(dsHeader);
      errmsg = dsHeader.Errmsg;
      LogList.AddItems(Direction.Write, dsHeader.ToReportLines(), true);

      // next is the response header.
      if (errmsg == null )
      {
        responseHeader = new ResponseHeader(inputArray);
        listItems.Add(responseHeader);
        errmsg = responseHeader.Errmsg;
        LogList.AddItems(Direction.Write, responseHeader.ToReportLines(), true);
      }

      // repeating instances of sbaOrder, textDataOrder pairs.
      while (true)
      {
        // check that an SBA order is starting. Leave loop when it is not.
        if (SetBufferAddressOrder.CheckOrder(inputArray) != null)
          break;

        var orderPair = new LocatedTextDataOrderPair(inputArray);
        if ( orderPair.Errmsg != null )
        {
          break;
        }
        listItems.Add(orderPair);
        LogList.AddItems(Direction.Write, orderPair.ToReportLines(), true );
      }

      return new Tuple<object[], string>(listItems.ToArray(), errmsg);
    }

    /// <summary>
    /// parse and return the next workstation data stream command from the server 
    /// byte data stream.
    /// </summary>
    /// <param name="inputArray"></param>
    /// <param name="StreamHeader"></param>
    /// <param name="showItemList"></param>
    /// <returns></returns>
    private static Tuple<WorkstationCommandBase,ShowItemList,DataStreamHeader,TelnetLogList> 
      ParseAndProcessWorkstationCommand( 
      InputByteArray inputArray, DataStreamHeader StreamHeader )
    {
      var logList = new TelnetLogList();
      WorkstationCommandBase wrkstnCommand = null;
      DataStreamHeader currentDataStreamHeader = StreamHeader;
      ShowItemList showItemList = null;

      wrkstnCommand = WorkstationCommandBase.ParseFactory(inputArray);
      if (wrkstnCommand == null)
        currentDataStreamHeader = null;
      else
      {
        logList.AddItems(Direction.Read, wrkstnCommand.ToReportLines(), true);

        // process WriteToDisplay command. This command contains a list TextData and
        // start field orders. Create ShowItemList items from those orders.
        if (wrkstnCommand is WriteToDisplayCommand)
        {
          var wtdCommand = wrkstnCommand as WriteToDisplayCommand;
          showItemList = wtdCommand.BuildShowItemList(logList);
        }
      }
      return new Tuple<WorkstationCommandBase,ShowItemList,DataStreamHeader,TelnetLogList>(
        wrkstnCommand, showItemList, currentDataStreamHeader, logList);
    }

    private static void WriteToHost(
      TelnetLogList LogList, TelnetCommand Stmt, NetworkStream NetStream)
    {
      WriteToHost(LogList, Stmt.ToBytes(), NetStream);
    }

    private static void WriteToHost(
      TelnetLogList LogList, byte[] Buffer, NetworkStream NetStream)
    {
      if (NetStream.CanWrite == true)
      {
        LogList.Write(Direction.Write, Buffer);

        {
          string hexText = Buffer.ToHex(' ');
          LogList.AddItem( Direction.Write, "Hex: " + Buffer.Length.ToString() +
            "  " + hexText);
        }

        NetStream.Write(Buffer, 0, Buffer.Length);
      }
      else
      {
        LogList.AddItem(Direction.Write, "Error. Could not write to network stream.");
      }
    }

  }
}
