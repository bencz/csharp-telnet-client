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
using AutoCoder.Telnet.TelnetCommands;
using System;
using System.Net.Sockets;
using AutoCoder.Telnet.Connect;
using AutoCoder.Telnet.IBM5250.Header;
using System.Diagnostics;

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
    public static Tuple<DataStreamHeader, WorkstationCommandList, bool, bool>
      ParseWorkstationCommandList(
      InputByteArray InputArray,
      SessionSettings SessionSettings)
    {
      var wrkstnCmdList = new WorkstationCommandList();
      DataStreamHeader dsh = null;
      bool gotEOR = false;
      bool needMoreBytes = false;
      string errmsg = null;

      if (InputArray.IsDataStreamHeader())
      {
        var rv = DataStreamHeader.Factory(InputArray);
        dsh = rv.Item1;
        errmsg = rv.Item2;
      }

      bool lastCmdWasTelnet_EOR = false;
      bool gotWorkstationCommand = true;
      gotEOR = false;
      while ((InputArray.IsEof( ) == false) && (gotEOR == false) 
        && (gotWorkstationCommand == true))
      {
        // no input data to process.
        lastCmdWasTelnet_EOR = false;
        gotWorkstationCommand = false;

        // check for IAC EOR
        var telCode = InputArray.PeekTelnetCommandCode(CommandCode.EOR);
        if (telCode != null)
        {
          var telCmd = InputArray.NextTelnetCommand();
          lastCmdWasTelnet_EOR = true;
          gotEOR = true;
        }

        // process the input as workstation data stream commands.
        else
        {
          var rv = ParseAndProcessWorkstationCommand(InputArray);
          var workstationCommand = rv.Item1;
          var howRead = rv.Item2;

          if (workstationCommand != null)
          {
            wrkstnCmdList.Add(workstationCommand);
            gotWorkstationCommand = true;
          }
        }
      }

      // read available bytes from input stream.
      if (InputArray.IsEof() && (lastCmdWasTelnet_EOR == false))
      {
        needMoreBytes = true;
      }

      return new Tuple<DataStreamHeader, WorkstationCommandList, bool, bool>(
        dsh, wrkstnCmdList, gotEOR, needMoreBytes);
    }

    /// <summary>
    /// parse and return the next workstation data stream command from the server 
    /// byte data stream.
    /// </summary>
    /// <param name="inputArray"></param>
    /// <param name="StreamHeader"></param>
    /// <param name="showItemList"></param>
    /// <returns></returns>
    private static Tuple<WorkstationCommandBase,HowReadScreen?>
      ParseAndProcessWorkstationCommand(InputByteArray inputArray)
    {
      WorkstationCommandBase wrkstnCommand = null;
      HowReadScreen? howRead = null;

      wrkstnCommand = WorkstationCommandBase.ParseFactory(inputArray);
      if (wrkstnCommand != null)
      {
        howRead = wrkstnCommand.GetHowReadScreenCode();

        // process WriteToDisplay command. This command contains a list TextData and
        // start field orders. Create ShowItemList items from those orders.
        if (wrkstnCommand is WriteToDisplayCommand)
        {
          var wtdCommand = wrkstnCommand as WriteToDisplayCommand;
        }
      }
      return new Tuple<WorkstationCommandBase, HowReadScreen?>(
        wrkstnCommand, howRead);
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
          LogList.AddItem(Direction.Write, "Hex: " + Buffer.Length.ToString() +
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
