using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Controls;
using tnClient.Runtime;
using AutoCoder.Core;
using AutoCoder.Telnet.Common;
using AutoCoder.Threading;
using AutoCoder.Ext.System;

namespace tnClient.Threads
{
  public enum SystemBrand { AMM, VIOS, AIX, IBMi, Linux, None };

  // run a command on the telnet server.  
  // See EntryPoint method of CommandThread class.
  public delegate string[] delRunTelnetServerCommand(
      CommandThread InCmdThread,
      ReadBuffer InReadBuffer,
      delWriteEventLog InWriteEventLog,
      string InCmdText);

  public static class ThreadCommon
  {
    public delegate void delWriteToRunLog(string[] InLines, ListBox InRunLog);

    // --------------------------- CreateConnection -----------------------------
    public static TelnetConnection CreateConnection(LoginProperties InProps)
    {
      TelnetConnection conn = null;

      string ipAddr = InProps.LoginSystem;
      int portNx = 23;
      conn = new TelnetConnection(ipAddr, portNx);

      return conn;
    }

    // -------------------------------- Login -------------------------------------
    public static string[] Login(
      TelnetConnection InConn, LoginProperties InProps, RunLogListBox InRunLog)
    {
      RunLogListBox runLog = new RunLogListBox(InRunLog, "Login");
      string[] respLines =
        TelnetCore.Login(InConn, runLog, InProps.LoginUser, InProps.LoginPass, 500);

      return respLines;
    }

    // ---------------------- StartReceiveFromTelnetServerThread ----------------------
    public static void StartReceiveFromTelnetServerThread(
      //      out DataQueue OutReceiveDataQueue, 
      out AutoResetEvent OutStartReceivingEvent,
      TelnetConnection InConn, ThreadSupervisor InSupervisor,
      ExtendedManualResetEvent InShutdownFlag, RunLogListBox InRunLog)
    {

      // create the event that the receive thread listens to to be signaled that it 
      // should wake up and receive from the telnet server.
      OutStartReceivingEvent = new AutoResetEvent(false);

      // setup the ReceiveThread object with all the object references it needs.
      ReceiveThread rt = new ReceiveThread(
        InConn,
        OutStartReceivingEvent,
        InSupervisor,
        InShutdownFlag, InRunLog);

      // start the ReceiveThread.
      ReceiveThread.ThreadStart(rt);
    }

    // ------------------------------ WriteToEventLog ----------------------------
    public static void WriteToEventLog(
      delWriteEventLog InWriteEventLog, string[] InLines, string InActivity)
    {
      foreach (string s1 in InLines)
      {
        InWriteEventLog(null, s1, InActivity);
      }
    }

    // -------------------------- WriteToRunLog ----------------------------------
    public static void WriteToRunLog(string[] InLines, ListBox InRunLog)
    {
      if (InRunLog.Dispatcher.CheckAccess() == false)
      {
        InRunLog.Dispatcher.BeginInvoke(
          new delWriteToRunLog(WriteToRunLog),
          new object[] { InLines, InRunLog });
      }

      else
      {

        foreach (string respLine in InLines)
        {
          if (respLine.IsNotBlank())
          {
            InRunLog.Items.Add(respLine);
          }
        }
      }
    }

  }
}
