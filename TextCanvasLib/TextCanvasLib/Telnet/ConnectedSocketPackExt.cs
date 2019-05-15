using AutoCoder.Ext.System;
using AutoCoder.Telnet.Common;
using AutoCoder.Telnet.Enums;
using AutoCoder.Telnet.Enums.IBM5250;
using AutoCoder.Telnet.IBM5250;
using AutoCoder.Telnet.IBM5250.Common;
using AutoCoder.Telnet.IBM5250.Header;
using AutoCoder.Telnet.IBM5250.Response;
using AutoCoder.Telnet.IBM5250.WorkstationCommands;
using System;
using System.Collections.Generic;
using TextCanvasLib.Canvas;
using TextCanvasLib.Visual;

namespace TextCanvasLib.Telnet
{
  public static class ConnectedSocketPackExt
  {

#if skip
    public static Tuple<HowReadScreen?, List<WriteToDisplayCommand>,
      TelnetLogList, WorkstationCommandList, DataStreamHeader, bool>
      ProcessWorkstationDataStream(
      this ServerConnectPack ConnectPack, ScreenVisualItems VisualItems,
      CanvasPositionCursor Caret)
    {
      WorkstationCommandList workstationCmdList = null;
      List<WriteToDisplayCommand> wtdCmdList = null;
      DataStreamHeader dsh = null;
      var returnCmdList = new WorkstationCommandList();
      HowReadScreen? howRead = null;
      var logList = new TelnetLogList();
      bool gotEOR = false;

      while (gotEOR == false)
      {
        // input array is eof. Exit loop if have read a READ workstn command. 
        // Otherwise, read from server.
        if (ConnectPack.ReadBlocks.IsEmpty == true)
        {
          if (howRead != null)
            break;
        }

        var item = ConnectPack.ReadBlocks.WaitAndDequeue();

        if (item is DataStreamHeader)
        {
          dsh = item as DataStreamHeader;
          continue;
        }

        gotEOR = true;
        if (item is WorkstationCommandList)
        {
          workstationCmdList = item as WorkstationCommandList;

          foreach (var workstationCmd in workstationCmdList)
          {
            if (workstationCmd is ClearUnitCommand)
            {
              returnCmdList.Add(workstationCmd);
            }

            // WTD command. Add to list of WTD commands. This list is returned to
            // the caller of this method.
            else if (workstationCmd is WriteToDisplayCommand)
            {
              returnCmdList.Add(workstationCmd);
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
            else if (workstationCmd is SaveScreenCommand)
            {
              var ra = 
                SaveScreenCommandExt.BuildSaveScreenResponse(VisualItems, Caret);

              // send response stream back to server.
              {
                TelnetConnection.WriteToHost(
                  logList, ra, ConnectPack.TcpClient.GetStream());
                gotEOR = false;
              }
            }

            else if (workstationCmd is WriteStructuredFieldCommand)
            {
              var wsfCmd = workstationCmd as WriteStructuredFieldCommand;
              if (wsfCmd.RequestCode == WSF_RequestCode.Query5250)
              {
                var ra = Query5250Response.BuildQuery5250Response();

                // send response stream back to server.
                {
                  TelnetConnection.WriteToHost(
                    logList, ra, ConnectPack.TcpClient.GetStream());
                  gotEOR = false;
                }
              }
            }
          }
        }
      }

      return new Tuple<HowReadScreen?, List<WriteToDisplayCommand>,
        TelnetLogList, WorkstationCommandList, DataStreamHeader, bool>(
        howRead, wtdCmdList, logList, returnCmdList, dsh, gotEOR);
    }

#endif

#if skip
    public static Tuple<HowReadScreen?, List<WriteToDisplayCommand>,
      TelnetLogList, WorkstationCommandList, DataStreamHeader, bool>
      ProcessWorkstationDataStream(
      this ConnectedSocketPack SocketPack, ScreenVisualItems VisualItems,
      CanvasPositionCursor Caret)
    {
      WorkstationCommandList workstationCmdList = null;
      List<WriteToDisplayCommand> wtdCmdList = null;
      DataStreamHeader dsh = null;
      var returnCmdList = new WorkstationCommandList();
      HowReadScreen? howRead = null;
      var logList = new TelnetLogList();
      bool gotEOR = false;

      while (gotEOR == false)
      {
        // input array is eof. Exit loop if have read a READ workstn command. 
        // Otherwise, read from server.
        if (SocketPack.InputArray.IsEof() == true)
        {
          if (howRead != null)
            break;
          {
            var log = SocketPack.InputArray.ReadFromNetworkStream(10, 60);
            logList.AddItems(log);
            if (SocketPack.InputArray.IsEof() == true)
              break;
          }
        }

        // peek at the input stream from server. Classify the data that is next 
        // to receive.
        var typeData = ServerDataStream.PeekServerCommand(SocketPack.InputArray);

        // input data not recogizied. Not a 5250 data strem header.
        if (typeData == null)
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
            dsh = rv.Item1;
            workstationCmdList = rv.Item2;
            logList.AddItems(rv.Item3);
            gotEOR = rv.Item4;
          }

          foreach (var workstationCmd in workstationCmdList)
          {
            if (workstationCmd is ClearUnitCommand)
            {
              returnCmdList.Add(workstationCmd);
            }

            // WTD command. Add to list of WTD commands. This list is returned to the
            // caller of this method.
            else if (workstationCmd is WriteToDisplayCommand)
            {
              returnCmdList.Add(workstationCmd);
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
            else if (workstationCmd is SaveScreenCommand)
            {
              var ra = SaveScreenCommandExt.BuildSaveScreenResponse(VisualItems, Caret);

              // send response stream back to server.
              {
                TelnetConnection.WriteToHost(logList, ra, SocketPack.TcpStream);
                gotEOR = false;
              }
            }

            else if (workstationCmd is WriteStructuredFieldCommand)
            {
              var wsfCmd = workstationCmd as WriteStructuredFieldCommand;
              if (wsfCmd.RequestCode == WSF_RequestCode.Query5250)
              {
                var ra = Query5250Response.BuildQuery5250Response();

                // send response stream back to server.
                {
                  TelnetConnection.WriteToHost(logList, ra, SocketPack.TcpStream);
                  gotEOR = false;
                }
              }
            }
            else if (workstationCmd is WriteSingleStructuredFieldCommand)
            {
            }
          }
        }
      }
      return new Tuple<HowReadScreen?, List<WriteToDisplayCommand>,
        TelnetLogList, WorkstationCommandList, DataStreamHeader, bool>(
        howRead, wtdCmdList, logList, returnCmdList, dsh, gotEOR);
    }
#endif

  }
}
