using AutoCoder.Telnet.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tnClient.Brand;
using tnClient.Runtime;

namespace tnClient.Threads
{
  public static class TelnetCore
  {
    // --------------------------------- Login ------------------------------
    public static string[] Login(
      TelnetConnection InConn, RunLogListBox InRunLog,
      string InUserName, string InPass, int InLoginTimeOutMs)
    {
      string s = null;

      int cx = 0;
      while (true)
      {
        InRunLog.Write("Read login prompt ...");
        s = InConn.Read(InLoginTimeOutMs);
        if (s.Length > 0)
          break;
        cx += 1;
        if (cx > 5)
          throw new Exception("Did not receive any login prompt text from telnet server");
      }

      if (!s.TrimEnd().EndsWith(":"))
        throw new Exception("Failed to connect : no login prompt");
      InRunLog.Write("Username " + InUserName);
      InConn.WriteLine(InUserName);

      s += InConn.Read(InLoginTimeOutMs);
      if (!s.TrimEnd().EndsWith(":"))
        throw new Exception("Failed to connect : no password prompt");
      InRunLog.Write("Password " + InPass);
      InConn.WriteLine(InPass);

      s += InConn.Read(InLoginTimeOutMs);

      string[] lines = SplitReadText(s);

      return lines;
    }

    // ----------------------------- RunCommand ------------------------------------
    public static void RunCommand(
      ThreadSupervisor InSupervisor,
      TelnetConnection InConn, TelnetCommandRoute InCommandRoute,
      RunLogListBox InRunLog,
      string InCmdText)
    {
      {
        RunLogListBox runLog = new RunLogListBox(InRunLog, "RunCommand");
        CommandThread ct = new CommandThread(
          InSupervisor, InConn, InCommandRoute, InRunLog);
        ct.CommandText = InCmdText;

        // run the logout thread.
        CommandThread.ThreadStart(ct);
      }
    }

    // ------------------------------ ReadUntilEmpty --------------------------------
    public static string[] ReadUntilEmpty(TelnetConnection InConn)
    {
      List<string> respLines = new List<string>();

      while (true)
      {
        string respLine = InConn.Read(500);
        if ((respLine == null) || (respLine == ""))
          break;
        respLines.Add(respLine);
      }

      return respLines.ToArray();
    }

    // ---------------------------- SplitReadText --------------------------------
    public static string[] SplitReadText(string InText)
    {
      string[] splitPatterns = new string[] { Environment.NewLine, "\r" };

      string[] lines =
        InText.Split(splitPatterns, StringSplitOptions.None);

      return lines;
    }

    // ------------------------------- Validate --------------------------------
    public static void Validate(string InText)
    {
      string lastChar = "";

      // get last non blank from the string.
      string s1 = InText.TrimEnd();
      if (s1.Length > 0)
        lastChar = s1.Substring(s1.Length - 1);

      // server output should end with "$" or ">", otherwise the connection failed
      if ((lastChar != "$") && (lastChar != ">"))
        throw new Exception("Connection failed");
    }

  }

}
