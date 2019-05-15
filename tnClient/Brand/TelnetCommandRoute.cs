using AutoCoder.Core;
using AutoCoder.Collections;
using AutoCoder.Ext.System;
using AutoCoder.Telnet.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tnClient.Threads;

namespace tnClient.Brand
{
  /// <summary>
  /// class used to send command to telnet server and receive
  /// responses back. 
  /// Some methods like Login and Logout are virtual to enable derived
  /// classes for each brand of server to specifically implement those
  /// functions. ( login to AMM may be different from login to Linux. )
  /// </summary>
  public class TelnetCommandRoute
  {
    LoginProperties mLoginProperties;

    // command prompt the telnet server sends to the client after it is finished running
    // the last command and is ready for another command.
    string[] mCommandPromptPatterns = null;

    public TelnetCommandRoute(
      LoginProperties InLoginProperties)
    {
      mLoginProperties = InLoginProperties;
      CommandPromptPatterns = new string[] { ">" };
    }

    public virtual string[] CommandPromptPatterns
    {
      get { return mCommandPromptPatterns; }
      set { mCommandPromptPatterns = value; }
    }

    // --------------------------------- Login ------------------------------
    public virtual string[] Login(
      CommandThread InCmdThread,
      ReadBuffer InReadBuffer,
      delWriteEventLog InWriteEventLog,
      string InNullText)
    {
      StringBuilder accumRead = new StringBuilder();
      string[] lines = null;
      string[] loginPrompts = new string[] { ":" };

      // telnet server sends a login prompt. read until it arrives. 
      if (InCmdThread.ShutdownFlag.State == false)
      {
        if (InWriteEventLog != null)
          InWriteEventLog(null, "Read login prompt ...", "Login");
        lines = InReadBuffer.ReadUntilEndsWith(loginPrompts);
        ThreadCommon.WriteToEventLog(InWriteEventLog, lines, "Login");
      }

      // send username to the telnet server.
      if (InCmdThread.ShutdownFlag.State == false)
      {
        if (InWriteEventLog != null)
          InWriteEventLog(null, "Username " + mLoginProperties.LoginUser, "Login");
        InCmdThread.Conn.WriteLine(mLoginProperties.LoginUser);
      }

      // read back the prompt for password.
      if (InCmdThread.ShutdownFlag.State == false)
      {
        if (InWriteEventLog != null)
          InWriteEventLog(null, "Read password prompt ...", "Login");
        lines = InReadBuffer.ReadUntilEndsWith(loginPrompts);
        ThreadCommon.WriteToEventLog(InWriteEventLog, lines, "Login");
      }

      // send password to telnet server.
      if (InCmdThread.ShutdownFlag.State == false)
      {
        if (InWriteEventLog != null)
          InWriteEventLog(null, "Password ********", "Login");
        InCmdThread.Conn.WriteLine(mLoginProperties.LoginPass);
      }

      // read back until the logged in command prompt.
      if (InCmdThread.ShutdownFlag.State == false)
      {
        if (InWriteEventLog != null)
          InWriteEventLog(null, "Read command prompt ...", "Login");


        while (true)
        {
          string[] xx = null;
          xx = InReadBuffer.Read();
          ThreadCommon.WriteToEventLog(InWriteEventLog, xx, "Login");
          string lastLine = Arrayer.LastItem(xx);
          if (lastLine != null)
          {
            string trimLastLine = lastLine.TrimEnd(new char[] { ' ', '\t', '\r', '\n' });
            string lsChar = StringExt.Tail(trimLastLine, 1);
            if (lsChar == ">")
              break;
          }
        }
      }

      // return all the response line received from the server.
      return lines;
    }

    // --------------------------------- Logout ------------------------------
    public virtual string[] Logout(
      CommandThread InCmdThread,
      ReadBuffer InReadBuffer,
      delWriteEventLog InWriteEventLog,
      string InNullText)
    {
      StringBuilder accumRead = new StringBuilder();
      string[] lines = null;

      if (InCmdThread.Conn.IsConnected == false)
      {
        InCmdThread.ShutdownFlag.Set();
      }

      // send "exit" command to the telnet server.
      if (InCmdThread.ShutdownFlag.State == false)
      {
        if (InWriteEventLog != null)
          InWriteEventLog(null, "exit", "Logout");
        try
        {
          InCmdThread.Conn.WriteLine("exit");
        }
        catch (NotConnectedException)
        {
          InCmdThread.ShutdownFlag.Set();
        }
      }

      // read back whatever is in the read buffer.
      if (InCmdThread.ShutdownFlag.State == false)
      {
        lines = InReadBuffer.Read();
        ThreadCommon.WriteToEventLog(InWriteEventLog, lines, "Logout");
      }

      // wait for the "socket is disconnected" signal. 
      int waitDur = 5000;
      while (true)
      {
        bool rc = InCmdThread.Conn.IsDisconnectedSignal.WaitOne(waitDur);
        if (rc == true)
          break;
        if (waitDur < 3600000)
          waitDur *= 2;
        InWriteEventLog(null,
          "waiting for telnet server to disconnect after exit", "Logout");
      }

      // return all the response line received from the server.
      return lines;
    }

    // --------------------------------- RunCommand ------------------------------
    public virtual string[] RunCommand(
      CommandThread InCmdThread,
      ReadBuffer InReadBuffer,
      delWriteEventLog InWriteEventLog,
      string InCmdText)
    {
      StringBuilder accumRead = new StringBuilder();
      string[] lines = null;

      // send command to the telnet server.
      if (InCmdThread.ShutdownFlag.State == false)
      {
        if (InWriteEventLog != null)
          InWriteEventLog(null, "exit", "Logout");
        InCmdThread.Conn.WriteLine(InCmdText);
      }

      // read back whatever is in the read buffer.
      if (InCmdThread.ShutdownFlag.State == false)
      {
        lines = InReadBuffer.ReadUntilEndsWith(CommandPromptPatterns);
        ThreadCommon.WriteToEventLog(InWriteEventLog, lines, "Run");
      }

      // return all the response line received from the server.
      return lines;
    }

  }

}
