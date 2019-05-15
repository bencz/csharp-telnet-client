using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using AutoCoder.Ehllapi.Settings;
using AutoCoder.Ext.System;
using AutoCoder.Ehllapi.Api.Structs;
using System.Windows;
using Ehllapi.Messages;

namespace AutoCoder.Ehllapi.Api
{
  // there are two dlls: pcsapi.dll, pcsapi32.dll
  // c:\program files\ibm\client access\emulator\pcsapi32.dll

  public enum pcs_SessionStatus { Started, OnLine, ApiEnabled, InterimState, None } ;

  public class pcsapi
  {
  const uint PCS_SESSION_STARTED = 0x01 ;
  const uint PCS_SESSION_ONLINE = 0x02 ;
  const uint PCS_SESSION_API_ENABLED = 0x04 ;
  const uint PCS_SESSION_INTERIM_STATE = 0x08 ;

  //  [DllImport("c:\\program files\\ibm\\client access\\emulator\\pcsapi32.dll")]

    // note: since the DllImport DLL is not a qualified path the Environment.Path will
    //       be used to search for pcsapi32.dll. 
    //       See the static constructor of this pcsapi class. It reads the emulator
    //       directory path from the ehllapi settings file. Then check that the
    //       Environment.Path variable contains this path. By default, it should have
    //       it since when client access is installed the search path of the PC is 
    //       updated to include that directory. 


    // pcsQuerySessionList returns the total number of sessions.
    [DllImport("pcsapi32.dll")]
    public static extern uint 
      pcsQuerySessionList( uint InSessCx, [Out] SessInfo[] pSessInfo);

    [DllImport("pcsapi32.dll",
      CharSet = CharSet.Ansi)]
    public static extern uint pcsStopSession(char InSessId, ushort InSaveProfile ) ;

    [DllImport("pcsapi32.dll",
      CharSet = CharSet.Ansi)]
    public static extern uint pcsConnectSession(char InSessId);

    [DllImport("pcsapi32.dll",
      CharSet = CharSet.Ansi)]
    public static extern uint pcsDisconnectSession(char InSessId);

    [DllImport("pcsapi32.dll",
      CharSet = CharSet.Ansi)]
    public static extern uint pcsQueryEmulatorStatus(char InSessId);

    [DllImport("pcsapi32.dll",
      CharSet = CharSet.Ansi)]
    public static extern uint pcsStartSession(
      string ProfilePath, char SessId, ushort CmdShow );

    static pcsapi( )
    {
      var settings = EhllapiSettings.RecallSettings();
      if (settings.DirPath_Emulator.IsNullOrEmpty( ) == false )
      {
        var path = Environment.GetEnvironmentVariable("PATH") ?? string.Empty;
        if (path.Contains(settings.DirPath_Emulator) == false)
        {
          path += ";" + settings.DirPath_Emulator;
          Environment.SetEnvironmentVariable("PATH", path);
        }
      }
    }

    /// <summary>
    /// check that sessid identified session is active. returns true
    /// if active.
    /// </summary>
    /// <param name="InSessId"></param>
    /// <returns></returns>
    public static bool CheckSession(char SessId)
    {
      pcs_SessionStatus sts = QueryEmulatorStatus(SessId);
      if (sts == pcs_SessionStatus.None)
        return false;
      else
        return true;
    }

    public static int QueryActiveSessionCount()
    {
      SessInfo[] siArray = new SessInfo[2];
      uint cx = pcsQuerySessionList(0, siArray);

      cx = pcsQuerySessionList(2, siArray);
      if (cx >= 1)
      {
        char ssnId = siArray[0].ShortName;
      }

      return (int)cx;
    }

    public static pcs_SessionStatus QueryEmulatorStatus(char SessId)
    {
      uint rv = 0;
      rv = pcsQueryEmulatorStatus(SessId);

      if ((rv & PCS_SESSION_STARTED) != 0)
        return pcs_SessionStatus.Started;
      else if ((rv & PCS_SESSION_ONLINE) != 0)
        return pcs_SessionStatus.OnLine;
      else if ((rv & PCS_SESSION_API_ENABLED) != 0)
        return pcs_SessionStatus.ApiEnabled;
      else if ((rv & PCS_SESSION_INTERIM_STATE) != 0)
        return pcs_SessionStatus.InterimState;
      else
        return pcs_SessionStatus.None;
    }

    public static uint QuerySessionCount()
    {
      SessInfo[] siArray = new SessInfo[2];
      uint cx = pcsQuerySessionList(0, siArray);
      return cx;
    }

    /// <summary>
    /// return the output of pcsQuerySessionList as an IEnumerable of SessInfo.
    /// </summary>
    /// <returns></returns>
    public static IEnumerable<SessInfo> QuerySessionList()
    {
      var sessCx = QuerySessionCount();
      SessInfo[] siArray = new SessInfo[sessCx];
      uint rv = pcsQuerySessionList(sessCx, siArray);
      foreach (var sessInfo in siArray)
      {
        yield return sessInfo;
      }
      yield break;
    }

    /// <summary>
    /// Ends the SessId identified display session window.
    /// </summary>
    /// <param name="InSessId"></param>
    public static SessIdMessage StopSession(string SessId)
    {
      SessIdMessage msg = null;

      // pcsStopSession returns FALSE when the method fails. TRUE when it succeeds.
      uint rv = pcsStopSession(SessId[0], 2);
      if (rv == 0)
        msg = new SessIdMessage(SessId, "pcsStopSession failed.");
      else
        msg = new SessIdMessage(SessId, "Session stopped.");
      
      return msg;
    }

    public static void StartSession(string ProfilePath, string SessId)
    {

//#define PCS_HIDE                     0
//#define PCS_SHOW                     1
//#define PCS_MINIMIZE                 2
//#define PCS_MAXIMIZE                 3

      uint rv = 0;
      rv = pcsStartSession(ProfilePath, SessId[0], 1);
      if (rv != 0)
        throw new ApplicationException("pcsStartSession failed. Return code:" + rv.ToString());
    }
  }
}
