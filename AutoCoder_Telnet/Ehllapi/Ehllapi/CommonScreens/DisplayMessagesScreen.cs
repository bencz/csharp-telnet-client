using System;
using System.Collections.Generic;
using System.Text;
using AutoCoder.Ehllapi;
using AutoCoder.Ehllapi.Presentation;
using AutoCoder.Ehllapi.Common;

namespace AutoCoder.Ehllapi.CommonScreens
{
  public class DisplayMessagesScreen : DisplayScreen
  {

    public DisplayMessagesScreen( PresentationSpace InPs )
      : base( InPs )
    {
    }

    public void Enter( DisplaySession InSess )
    {
      InSess.SendKeys(Ehllapier.Key.Enter);
    }

    public override bool IsScreen( )
    {
      PresentationSpace ps = base.PresentationSpace;

      PresentationSpaceField f1 =
        ps.FindField( new DisplayLocation(1, 33));
      PresentationSpaceField f2 =
        ps.FindField( new DisplayLocation(2, 56));
      PresentationSpaceField f3 =
        ps.FindField( new DisplayLocation(7, 2));

      if (((f1 != null) && (f1.Text.Contains("Display Messages") == true)) &&
        ((f2 != null) && (f2.Text.Contains("System:") == true)) &&
        ((f3 != null)
        && (f3.Text.Contains("Type reply (if required), press Enter.") == true)))
      {
        return true;
      }
      else
        return false;
    }

    public static bool IsScreen(DisplaySession DisplaySession)
    {
      DisplayMessagesScreen dms =
        new DisplayMessagesScreen(DisplaySession.GetPresentationSpace());
      if (dms.IsScreen() == true)
        return true;
      else
        return false;
    }
  }
}
