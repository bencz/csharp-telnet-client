using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using AutoCoder.Text;
using AutoCoder.Ehllapi.CommonScreens;
using AutoCoder.Ehllapi.Settings;
using Ehllapi.Enums;
using AutoCoder.Ext.System;
using AutoCoder.Ehllapi.Api;
using AutoCoder.Ehllapi.Presentation;

namespace AutoCoder.Ehllapi
{
  public class SessionScript
  {

    /// <summary>
    /// make sure the client access display session is running. If it is not running,
    /// start it.
    /// </summary>
    /// <param name="InSettings"></param>
    public static bool Assure_ClientAccessSession(EhllapiSettings Settings)
    {
      bool rc = Assure_ClientAccessSession(
        Settings, Settings.Path_WrkstnProfile, Settings.SessId);
      return rc;
    }

    public static bool Assure_ClientAccessSession(
      EhllapiSettings Settings, string ProfilePath, string SessId)
    {
      bool rc = false;
      bool isRunning = false;

      isRunning = pcsapi.CheckSession(SessId[0]);
      if (isRunning == false)
      {
        Launch_LoadLogonCache(Settings);
        Launch_ClientAccessSession(
          Settings, ProfilePath, SessId, LaunchOptions.WaitForReady);
        rc = true; // session was started.
      }
      return rc;
    }

    public void AssureSignoff(EhllapiSettings Settings)
    {
      SessionScript script = new SessionScript();
      try
      {
        using (DisplaySession sess = new DisplaySession())
        {
          sess.Connect(Settings.SessId);
          var sos = new CommonScreens.SignonScreen( sess.GetPresentationSpace( ));
          if ( sos.IsScreen( ) == true )
          {
            Play_Signoff(sess);
          }
        }
      }
      catch (ApplicationException excp)
      {
        throw new EhllapiExcp("AssureSignoff failed. " + excp.Message, excp);
      }
    }

    /// <summary>
    /// End the client access display session window indentified by the SessId
    /// contained in the EhllapiSettings class object.
    /// </summary>
    /// <param name="InSettings"></param>
    public static void End_ClientAccessSession(EhllapiSettings InSettings)
    {
      pcsapi.StopSession(InSettings.SessId);
    }

#if skip
    /// <summary>
    /// Scrape the screen to determine if the Samuel signon screen is displayed.
    /// </summary>
    /// <param name="InSess"></param>
    /// <returns></returns>
    public bool IsAtxSignon( DisplaySession InSess)
    {
      PresentationSpace ps = null;
      ps = InSess.GetPresentationSpace();
      if ((ps.IndexOf("Sign On") != null)
        && (ps.IndexOf("User  . . .") != null)
        && (ps.IndexOf("Password  .") != null))
      {
        return true;
      }
      else
        return false ;
    }
#endif

    public static void Launch_ClientAccessSession(EhllapiSettings InSettings)
    {
      Launch_ClientAccessSession(InSettings, LaunchOptions.None);
    }

    public static void Launch_ClientAccessSession(
      EhllapiSettings InSettings, LaunchOptions InLaunchOptions)
    {
      Launch_ClientAccessSession(
        InSettings, InSettings.Path_WrkstnProfile, InSettings.SessId,
        InLaunchOptions);
    }

    public static void Launch_ClientAccessSession(
      EhllapiSettings InSettings, string ProfilePath, string SessId,
      LaunchOptions InLaunchOptions)
    {
      int sessCx = 0;
      if (InLaunchOptions == LaunchOptions.WaitForReady)
        sessCx = pcsapi.QueryActiveSessionCount();

      System.Diagnostics.Process proc = new System.Diagnostics.Process();
      proc.StartInfo.FileName = InSettings.Path_pcsws;

      // enclose the wrkstn profile path in quotes ( in case spaces in the path )
      string profilePath = ProfilePath;
      if (profilePath.IndexOf('"') == -1)
      {
        profilePath = '"' + profilePath + '"';
      }
      proc.StartInfo.Arguments = profilePath;

      //      proc.StartInfo.Arguments =
      //        '"' +@"c:\Program Files\IBM\Client Access\Emulator\private\pc04a.ws" + '"';
      //      proc.StartInfo.Arguments = "'" + InWrkstnProfilePath + "'";
      //      proc.StartInfo.Arguments = @"..\private\pc04a.ws";
      if (SessId.IsNullOrEmpty() == false)
      {
        proc.StartInfo.Arguments = @"/S=" + SessId + " " + proc.StartInfo.Arguments;
      }

      proc.Start();

      proc.WaitForInputIdle(5000);

      // wait until session count increments. that tells us the new session
      // is completely started.
      if (InLaunchOptions == LaunchOptions.WaitForReady)
      {
        int newSessCx = 0;
        while (newSessCx <= sessCx)
        {
          Thread.Sleep(100);
          newSessCx = pcsapi.QueryActiveSessionCount();
        }

        // wait until the SessId identified session can be connected to.
        bool isConnected = false;
        int cummWait = 0;
        while (isConnected == false)
        {
          using (DisplaySession sess = new DisplaySession())
          {
            try
            {
              isConnected = true;
              sess.Connect(SessId);
              sess.Wait();
            }
            catch (EhllapiExcp)
            {
              cummWait += 300;
              if (cummWait > 10000)
                throw new ApplicationException("client access session connection timeout");
              isConnected = false;
              Thread.Sleep(300);
            }
          }
        }

        // wait for SystemAvailable.
        int tx = 0;
        while (true)
        {
          ++tx;
          if (tx > 1000)
            throw new EhllapiExcp("Wait for system available timeout");
          using (DisplaySession sess = new DisplaySession())
          {
            sess.Connect(SessId);
            if (sess.SystemAvailable == true)
              break;
            Thread.Sleep(100);
          }
        }

      }
    }

    /// <summary>
    /// launch the cwblogon.exe program to load the client access logon cache
    /// with a user id and password.
    /// Be aware, cwblogon does not have to be run. If it is not run, the first
    /// time a client access session is started ( since the PC was booted. ) the user
    /// of the PC will be prompted for a client access login. Same as when they 
    /// click on the desktop to start a client access session. 
    /// </summary>
    /// <param name="Settings"></param>
    public static void Launch_LoadLogonCache(EhllapiSettings Settings)
    {
      if (Settings.UserName.IsNullOrEmpty() == false)
      {
        System.Diagnostics.Process proc = null;

        proc = new System.Diagnostics.Process();
        proc.StartInfo.FileName = Settings.Path_cwblogon;
        proc.StartInfo.Arguments =
          Settings.SystemName + " /u " + Settings.UserName +
          " /p " + Settings.Password;
        proc.Start();
        proc.WaitForExit();
      }
    }

    public static void Play_ItemLocatorInquiry(EhllapiSettings InSettings)
    {
      try
      {
        PresentationSpace ps = null;
        SessionScript script = new SessionScript();

        script.AssureSignoff(InSettings);

        // signon the display
        script.Play_Signon(InSettings);

        using (DisplaySession sess = new DisplaySession())
        {
          sess.Connect(InSettings.SessId);

          // tab to the menu option input field
          sess.SendKeys(Ehllapier.Key.TabRight);

          // option i
          sess.SendKeys("i");
          sess.Wait();

          // 6174054
          sess.SendKeys("6174054" + Ehllapier.Key.Enter);
          sess.Wait();

          // option 10
          sess.SendKeys(Ehllapier.Key.TabRight + Ehllapier.Key.NewLine + "10" + Ehllapier.Key.Enter);
          sess.Wait();

        }
      }
      catch (ApplicationException excp)
      {
        throw new EhllapiExcp("Play_ItemLocatorInquiry script failed", excp);
      }
    }

    /// <summary>
    /// send keystrokes intended to signoff the display session to the Settings.SessId
    /// identified display session window.
    /// </summary>
    /// <param name="InSettings"></param>
    public void Play_Signoff( EhllapiSettings InSettings )
    {
      try
      {
        using (DisplaySession sess = new DisplaySession())
        {
          sess.Connect(InSettings.SessId);
          Play_Signoff(sess);
        }
      }
      catch (ApplicationException excp)
      {
        throw new EhllapiExcp( "Play_Signoff script failed", excp);
      }
    }

    /// <summary>
    /// send keystrokes intended to signoff the display session to the Settings.SessId
    /// identified display session window.
    /// </summary>
    /// <param name="InSess"></param>
    public void Play_Signoff(DisplaySession InSess)
    {
      while (true)
      {
        PresentationSpace ps = InSess.GetPresentationSpace();
        CommonScreens.SignonScreen sos = new CommonScreens.SignonScreen( ps );
        if ( sos.IsScreen( ))
          throw new EhllapiExcp( "At signon screen. cant signoff from this screen." ) ;

        InSess.SendKeys(Ehllapier.Key.SystemRequest + Ehllapier.Key.Enter);
        InSess.Wait();

        ps = InSess.GetPresentationSpace();
        var sysrqs = new CommonScreens.SystemRequestMenuScreen( ps );
        if (sysrqs.IsScreen( ))
          break;
      }

      InSess.SendKeys("90" + Ehllapier.Key.Enter);
      InSess.Wait();

      // signoff prompt
      InSess.SendKeys(Ehllapier.Key.Enter);
      InSess.Wait();

      return;

      InSess.SendKeys(Ehllapier.Key.SystemRequest + "90" + Ehllapier.Key.Enter);
      Play_Signoff_WaitUntilSignedOff(InSess);
    }

    private void Play_Signoff_WaitUntilSignedOff( DisplaySession InSess )
    {
      while( true )
      {
        InSess.Wait( ) ;
        PresentationSpace ps = InSess.GetPresentationSpace();
        CommonScreens.SignonScreen sos = new CommonScreens.SignonScreen( ps );
        if ( sos.IsScreen( ))
          break ;
        Thread.Sleep( 1 ) ;
      }
    }

    public void Play_Signon(EhllapiSettings Settings)
    {
      try
      {
        using (DisplaySession sess = new DisplaySession())
        {
          sess.Connect(Settings.SessId);

          sess.SendKeys(Settings.UserName);
          if (Settings.UserName.Length < 10)
            sess.SendKeys(Ehllapier.Key.FieldExit);

          sess.SendKeys(Settings.Password);
          if (Settings.Password.Length < 10)
            sess.SendKeys(Ehllapier.Key.FieldExit);

          sess.SendKeys(Ehllapier.Key.Enter);
          sess.Wait();

          // display messages screen breaks on screen. Press enter.
          if (DisplayMessagesScreen.IsScreen(sess))
          {
            sess.SendKeys(Ehllapier.Key.Enter);
            sess.Wait();
          }

          // display program messages screen is displayed. Screen displays on
          // signon to say "message queue allocated to another job".
          // Press enter.
          if (DisplayProgramMessagesScreen.IsScreen(sess))
          {
            sess.SendKeys(Ehllapier.Key.Enter);
            sess.Wait();
          }

          // handle the "attempt to recover interactive job" display by running the signoff
          // option and signing on again.
          if (RecoverInteractiveJobScreen.IsScreen(sess))
          {
            sess.SendKeys("90" + Ehllapier.Key.Enter);
            sess.Wait();
            Play_Signon(Settings);
          }
        }
      }
      catch (ApplicationException excp)
      {
        throw new EhllapiExcp("Play_Signon script failed", excp);
      }
    }

    public static PresentationSpace ReadPresentationSpace(
      EhllapiSettings InSettings)
    {
      PresentationSpace ps = null;
      try
      {
        using (DisplaySession sess = new DisplaySession())
        {
          sess.Connect(InSettings.SessId);

          ps = sess.GetPresentationSpace();
        }
      }
      catch (ApplicationException excp)
      {
        throw new EhllapiExcp("ReadPresentationSpace failed", excp);
      }
      return ps;
    }

  }
}
