using AutoCoder.Ext.System;
using AutoCoder.Telnet.Connect;
using AutoCoder.Telnet.Enums;
using AutoCoder.Telnet.Enums.IBM5250;
using AutoCoder.Telnet.IBM5250;
using AutoCoder.Telnet.IBM5250.Response;
using AutoCoder.Telnet.IBM5250.WorkstationCommands;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace AutoCoder.Telnet.Common
{
  /// <summary>
  /// store the telnet connection socket and stream.
  /// </summary>
  public class ConnectedSocketPack : IDisposable
  {
    public TcpClient TcpClient
    { get; set; }

    public NetworkStream TcpStream
    { get; set; }

    public SessionSettings Settings
    { get; set; }

    public NetworkStreamBackedInputByteArray InputArray
    { get; set; }

    public ConnectedSocketPack( 
      TcpClient Client, NetworkStream Stream, 
      NetworkStreamBackedInputByteArray InputArray )
    {
      this.TcpClient = Client;
      this.TcpStream = Stream;
      this.InputArray = InputArray;
    }
    public ConnectedSocketPack(NetworkStreamBackedInputByteArray InputArray)
    {
      this.InputArray = InputArray;
      this.TcpClient = InputArray.Client;
      this.TcpStream = InputArray.NetStream;
    }

    public void Dispose()
    {
      this.TcpStream?.Close();
      this.TcpClient?.Close();
    }

    public bool IsConnected( )
    {
      if (this.TcpClient?.Connected == true)
        return true;
      else
        return false;
    }
  }

  public static class ConnectedSocketPackExt
  {
    public static Tuple<HowReadScreen?, List<WriteToDisplayCommand>, TelnetLogList>
      ProcessWorkstationDataStream(this ConnectedSocketPack SocketPack)
    {
      WorkstationCommandList workstationCmdList = null;
      List<WriteToDisplayCommand> wtdCmdList = null;
      HowReadScreen? howRead = null;
      var logList = new TelnetLogList();

      while (true)
      {
        // input array is eof. Exit loop if have read a READ workstn command. 
        // Otherwise, read from server.
        if (SocketPack.InputArray.IsEof( ) == true )
        {
          if (howRead != null)
            break;
          {
            var log = SocketPack.InputArray.ReadFromNetworkStream(20, 30);
            logList.AddItems(log);
            if (SocketPack.InputArray.IsEof() == true)
              break;
          }
        }

        // peek at the input stream from server. Classify the data that is next 
        // to receive.
        var typeData = ServerDataStream.PeekServerCommand(SocketPack.InputArray);

        // input data not recogizied. Not a 5250 data strem header.
        if ( typeData == null )
        {
          logList.AddItem(Direction.Read, "Unknown data stream data");
          logList.AddItems(
            Direction.Read, SocketPack.InputArray.PeekBytes().ToHexReport(16));
          break;
        }

        if (typeData.Value == TypeServerData.workstationHeader)
        {
          {
            var rv = Process5250.GetAndParseWorkstationCommandList(
            SocketPack.InputArray, SocketPack.Settings);

            workstationCmdList = rv.Item1;
            logList.AddItems(rv.Item2);
          }

          foreach (var workstationCmd in workstationCmdList)
          {
            if (workstationCmd is ClearUnitCommand)
            {

            }

            else if (workstationCmd is WriteToDisplayCommand)
            {
              var wtdCommand = workstationCmd as WriteToDisplayCommand;
              if (wtdCmdList == null)
                wtdCmdList = new List<WriteToDisplayCommand>();
              wtdCmdList.Add(wtdCommand);
            }

            else if (workstationCmd is ReadMdtFieldsCommand)
            {
              howRead = HowReadScreen.ReadMdt;
            }

            // save screen command. Build response, send back to server.
            else if ( workstationCmd is SaveScreenCommand)
            {
              var ra = SaveScreenCommand.BuildSaveScreenResponse();

              Debug.WriteLine("Response: " + ra.ToHex(' '));

              // send response stream back to server.
              {
                TelnetConnection.WriteToHost(logList, ra, SocketPack.TcpStream);
              }
              // BgnTemp
              logList.AddSpecialItem(LogItemSpecial.NewGeneration);
              // EndTemp
            }

            else if (workstationCmd is WriteStructuredFieldCommand)
            {
              var wsfCmd = workstationCmd as WriteStructuredFieldCommand;
              if (wsfCmd.RequestCode == WSF_RequestCode.Query5250)
              {
                var ra = Query5250Response.BuildQuery5250Response();

                Debug.WriteLine("Response: " + ra.ToHex(' '));

                // send response stream back to server.
                {
                  TelnetConnection.WriteToHost(logList, ra, SocketPack.TcpStream);
                }

              }
            }
          }
        }
      }
      return new Tuple<HowReadScreen?, List<WriteToDisplayCommand>, TelnetLogList>(
        howRead, wtdCmdList, logList );
    }

  }
}
