using System;
using System.Collections.Generic;
using System.Text;
using AutoCoder.Ehllapi;
using AutoCoder.Ehllapi.Presentation;
using AutoCoder.Ehllapi.Common;

namespace AutoCoder.Ehllapi.CommonScreens
{
  public class DisplayProgramMessagesScreen : DisplayScreen
  {
    string mUserName = null;
    string mPassword = null;

    public DisplayProgramMessagesScreen( PresentationSpace InPs )
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

    public void Enter( DisplaySession InSess )
    {
      InSess.SendKeys(Ehllapier.Key.Enter);
      InSess.Wait();
    }

    public override bool IsScreen( )
    {
      var ps = base.PresentationSpace;

      var f1 = ps.FindField(new DisplayLocation(1, 28));
      var f2 = ps.FindField(new DisplayLocation(19, 2));

      if (f1.TextContains("Display Program Messages") == true)
        return true;
      else
        return false;
    }

    public static bool IsScreen(DisplaySession DisplaySession)
    {
      var dms =
        new DisplayProgramMessagesScreen(DisplaySession.GetPresentationSpace());
      if (dms.IsScreen() == true)
        return true;
      else
        return false;
    }
  }
}
