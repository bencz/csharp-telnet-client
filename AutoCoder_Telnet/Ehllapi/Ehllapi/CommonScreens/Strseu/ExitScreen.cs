using System;
using System.Collections.Generic;
using System.Text;
using AutoCoder;
using AutoCoder.Ehllapi;
using AutoCoder.Ehllapi.Settings;
using AutoCoder.Ehllapi.Presentation;
using AutoCoder.Ehllapi.Common;

namespace AutoCoder.Ehllapi.CommonScreens
{
  public partial class StrseuScreen
  {
    public class ExitScreen : DisplayScreen
    {

      public ExitScreen(PresentationSpace PresentationSpace)
        : base(PresentationSpace)
      {
      }

      public string MemberName
      {
        get
        {
          PresentationSpaceField f1 =
            base.PresentationSpace.FindField(new DisplayLocation(6, 42));
          if (f1 == null)
            throw new EhllapiExcp(
              "PresentationSpace does not contain MemberName field");
          return f1.Text.TrimEnd(new char[] { ' ' });
        }
      }

      public void Enter(DisplaySession InSess)
      {
        InSess.SendKeys(Ehllapier.Key.Enter);
      }

      public static void Enter(EhllapiSettings Settings)
      {
        using (DisplaySession sess = new DisplaySession())
        {
          sess.Connect(Settings.SessId);

          StrseuScreen.ExitScreen dms =
            new StrseuScreen.ExitScreen(sess.GetPresentationSpace());
          if (dms.IsScreen() == false)
            throw new ApplicationException("not at STRSEU Exit screen.");

          dms.Enter(sess);
        }
      }

      public override bool IsScreen()
      {
        PresentationSpace ps = base.PresentationSpace;

        PresentationSpaceField f1 =
          ps.FindField(new DisplayLocation(1, 38));
        PresentationSpaceField f2 =
          ps.FindField(new DisplayLocation(5, 4));

        if ((f1.TextContains("Exit") == true)
          && (f2.TextContains("Change/create member  . .") == true))
        {
          return true;
        }
        else
          return false;
      }

      public static bool IsScreen(DisplaySession DisplaySession)
      {
        StrseuScreen.ExitScreen dms =
          new StrseuScreen.ExitScreen(DisplaySession.GetPresentationSpace());
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
          isScreen = StrseuScreen.ExitScreen.IsScreen(sess);
        }
        return isScreen;
      }
    }
  }
}

