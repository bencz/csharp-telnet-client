using System;
using System.Collections.Generic;
using System.Text;
using AutoCoder.Ehllapi;
using AutoCoder.Ehllapi.Settings;
using AutoCoder.Ext.System;
using Ehllapi.Messages;
using AutoCoder.Ehllapi.Presentation;
using AutoCoder.Ehllapi.Common;

namespace AutoCoder.Ehllapi.CommonScreens
{
  public class SignonScreen : DisplayScreen
  {
    string mUserName = null;
    string mPassword = null;

    public SignonScreen( PresentationSpace InPs )
      : base( InPs )
    {
    }

    public string UserName
    {
      set { mUserName = value; }
    }

    public string Password
    {
      set { mPassword = value; }
    }

    public static SessIdMessage AssureSignedOff(EhllapiSettings Settings)
    {
      SessIdMessage msg = null;
      if (SignonScreen.IsScreen(Settings) == false)
      {
        var script = new SessionScript();
        var sess = new DisplaySession();
        script.Play_Signoff(Settings);
        msg = new SessIdMessage()
        {
          SessId = Settings.SessId,
          Message = "Session signed off"
        };
      }

      return msg;
    }

    /// <summary>
    /// If the session display does not contain a signon screen, automatically sign
    /// the session off.
    /// </summary>
    /// <param name="SessId"></param>
    public static SessIdMessage AssureSignedOff( string SessId )
    {
      SessIdMessage msg = null;
      var settings = EhllapiSettings.RecallSettings();

      settings.SessId = SessId;

      // make sure the session is active.
      SessionScript.Assure_ClientAccessSession(settings);

      msg = SignonScreen.AssureSignedOff(settings);
      return msg;
    }

    public bool AssureSignedOn(
      DisplaySession InSess, string InUserName, string InPassword)
    {
      bool wasSignedOn = false;
      if (IsScreen( ) == true)
      {
        Enter(InSess, InUserName, InPassword);
        wasSignedOn = false;
      }
      else
        wasSignedOn = true;
      return wasSignedOn;
    }

    public void Enter(
      DisplaySession InSess, string InUserName, string InPassword)
    {
      // tab to entry field as row xxx, col yyy
      var loc = new DisplayLocation( 6, 53 ) ;
      InSess.TabRightToDisplayLocation(loc);

      InSess.SendKeys(InUserName + Ehllapier.Key.FieldExit);
      InSess.SendKeys(InPassword + Ehllapier.Key.FieldExit);
      InSess.SendKeys(Ehllapier.Key.Enter);
      InSess.Wait();

      // advance past "display messages" screen.
      DisplayMessagesScreen dpm = 
        new DisplayMessagesScreen( InSess.GetPresentationSpace( ));
      if (dpm.IsScreen( ))
      {
        dpm.Enter(InSess);
      }
    }

    public override bool IsScreen( )
    {
      PresentationSpace ps = base.PresentationSpace;

      PresentationSpaceField f1 =
        ps.FindField( new DisplayLocation(1, 37));
      PresentationSpaceField f2 = 
        ps.FindField( new DisplayLocation(6, 17));
      PresentationSpaceField f3 =
        ps.FindField( new DisplayLocation(7, 17));

      if ((f1.TextContains("Sign On") == true )
        && (f2.TextContains("User  . . .") == true ) 
        && (f3.TextContains("Password  .") == true ))
      {
        return true;
      }
      else
        return false;
    }

    public static bool IsScreen(DisplaySession DisplaySession)
    {
      SignonScreen dms =
        new SignonScreen(DisplaySession.GetPresentationSpace());
      if (dms.IsScreen() == true)
        return true;
      else
        return false;
    }

    public static bool IsScreen(EhllapiSettings Settings)
    {
      bool isScreen = false;
      using (DisplaySession sess = new DisplaySession())
      {
        sess.Connect(Settings.SessId);
        isScreen = SignonScreen.IsScreen(sess);
      }
      return isScreen;
    }
  }
}
