using AutoCoder.Telnet.Common;
using AutoCoder.Telnet.Common.RowCol;
using AutoCoder.Telnet.Common.ScreenDm;
using AutoCoder.Telnet.Enums;
using AutoCoder.Telnet.Enums.IBM5250;
using AutoCoder.Telnet.IBM5250.Response;
using AutoCoder.Telnet.IBM5250.WorkstationCommands;
using System;
using System.Net.Sockets;
using System.Threading;
using System.Windows;
using System.Windows.Threading;
using TextCanvasLib.Canvas;
using TextCanvasLib.Visual;
using TextCanvasLib.xml;

namespace TextCanvasLib.Telnet
{
  public static class WorkstationCommandListExt
  {
    /// <summary>
    /// run the BuildShowItemList method against the first WriteToDisplay command in
    /// the list of workstation commands.
    /// </summary>
    /// <param name="LogList"></param>
    /// <returns></returns>
    public static ShowItemList BuildShowItemList(
      this WorkstationCommandList CommandList, ScreenDim ScreenDim, TelnetLogList LogList)
    {
      ShowItemList showItemList = null;
      OneRowCol caret = null;
      foreach (var workstationCommand in CommandList)
      {
        if (workstationCommand is WriteToDisplayCommand)
        {
          var wtdCmd = workstationCommand as WriteToDisplayCommand;
          TelnetLogList logList = new TelnetLogList();
          var rv = wtdCmd.BuildShowItemList(ScreenDim, logList);
          showItemList = rv.Item1;
          if (rv.Item2 != null)
            caret = rv.Item2;
          break;
        }
      }
      return showItemList;
    }

    public static Tuple<bool, TelnetLogList> PaintCanvas(
      this WorkstationCommandList CmdList, ItemCanvas ItemCanvas, TelnetLogList LogList = null)
    {
      TelnetLogList logList = new TelnetLogList();
      bool drawDone = false;
      OneRowCol caret = null;

      foreach (var cmdBase in CmdList)
      {
        if ((cmdBase is ClearUnitCommand) || ( cmdBase is ClearUnitAlternateCommand))
        {
          ItemCanvas.EraseScreen();
        }

        else if (cmdBase is WriteToDisplayCommand)
        {
          // store the painted from wtdCommand in the ItemCanvas itself. Used when
          // ScreenDefn imports a screen layout into the defn of a screen.
          ItemCanvas.WriteToDisplayCommand = cmdBase as WriteToDisplayCommand;

          bool eraseScreen = false;
          var rv = ItemCanvas.WriteToDisplayCommand.PaintCanvas(
            ItemCanvas, eraseScreen, LogList);
          var localDrawDone = rv.Item1;
          if (rv.Item2 != null)
            caret = rv.Item2;

          if (localDrawDone == true)
            drawDone = true;
        }
      }

      ItemCanvas.PositionCaret(caret);

      return new Tuple<bool, TelnetLogList>(drawDone, logList);
    }

#if skip
    public static void ProcessAndPaint(
          this WorkstationCommandList CmdList, TcpClient TcpClient,
          ItemCanvas ItemCanvas, Window Window,
          TelnetLogList LogList = null)
    {
      OneRowCol caret = null;

      foreach (var cmdBase in CmdList)
      {
        if ((cmdBase is ClearUnitCommand) || (cmdBase is ClearUnitAlternateCommand))
        {
          ItemCanvas.EraseScreen();
          caret = null;
        }

        else if (cmdBase is WriteToDisplayCommand)
        {
          bool doEraseScreen = false;
          var WTD_Command = cmdBase as WriteToDisplayCommand;
          ItemCanvas.WriteToDisplayCommand = WTD_Command;
          var localCaret = ItemCanvas.PaintScreen(WTD_Command, doEraseScreen, LogList);
          if (localCaret != null)
            caret = localCaret;
        }

        else if (cmdBase is ReadMdtFieldsCommand)
        {
          ItemCanvas.HowRead = HowReadScreen.ReadMdt;
        }

        // save screen command. Build response, send back to server.
        else if (cmdBase is SaveScreenCommand)
        {

          var ra = SaveScreenCommandExt.BuildSaveScreenResponse(
              ItemCanvas.VisualItems, ItemCanvas.CaretCursor);

          // send response stream back to server.
          if ( TcpClient != null)
          {
            TelnetConnection.WriteToHost(
              null, ra, TcpClient.GetStream());
          }
        }

        else if (cmdBase is WriteStructuredFieldCommand)
        {
          var wsfCmd = cmdBase as WriteStructuredFieldCommand;
          if (wsfCmd.RequestCode == WSF_RequestCode.Query5250)
          {
            var ra = Query5250Response.BuildQuery5250Response();

            // send response stream back to server.
            if ( TcpClient != null)
            {
              TelnetConnection.WriteToHost(LogList, ra, TcpClient.GetStream());
            }
          }
        }
        else if (cmdBase is WriteSingleStructuredFieldCommand)
        { }
      }

      ItemCanvas.PositionCaret(caret);
    }
#endif

  }
}

