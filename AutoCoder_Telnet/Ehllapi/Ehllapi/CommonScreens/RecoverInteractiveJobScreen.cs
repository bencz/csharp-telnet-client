using System;
using System.Collections.Generic;
using System.Text;
using AutoCoder.Ehllapi;
using AutoCoder.Ehllapi.Presentation;
using AutoCoder.Ehllapi.Common;

namespace AutoCoder.Ehllapi.CommonScreens
{
  public class RecoverInteractiveJobScreen : DisplayScreen
  {

    public RecoverInteractiveJobScreen( PresentationSpace PresentationSpace )
      : base( PresentationSpace )
    {
    }

    public void Enter( DisplaySession Sess )
    {
      Sess.SendKeys(Ehllapier.Key.Enter);
    }

    public override bool IsScreen( )
    {
      var ps = base.PresentationSpace;

      PresentationSpaceField f1 =
        ps.FindField( new DisplayLocation(1, 24));

      if ((f1 != null) 
        && (f1.Text.Contains("Attempt to Recover Interactive Job") == true))
      {
        return true;
      }
      else
        return false;
    }

    public static bool IsScreen(DisplaySession DisplaySession)
    {
      RecoverInteractiveJobScreen dms =
        new RecoverInteractiveJobScreen(DisplaySession.GetPresentationSpace());
      if (dms.IsScreen() == true)
        return true;
      else
        return false;
    }

  }
}
