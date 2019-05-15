using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AutoCoder.Ehllapi.Settings;
using AutoCoder.Ehllapi.Presentation;
using AutoCoder.Ehllapi.Common;

namespace AutoCoder.Ehllapi.CommonScreens
{
  public partial class StrseuScreen
  {
    public class BrowseScreen : EditScreen
    {
      public BrowseScreen(PresentationSpace PresentationSpace)
        : base(PresentationSpace)
      {
      }

      /// <summary>
      /// press enter key to exit the browse screen.
      /// </summary>
      /// <param name="Settings"></param>
      public static void Enter_Exit(EhllapiSettings Settings)
      {
        using (DisplaySession sess = new DisplaySession())
        {
          sess.Connect(Settings.SessId);

          var dms =
            new StrseuScreen.BrowseScreen(sess.GetPresentationSpace());
          if (dms.IsScreen() == false)
            throw new ApplicationException("not at STRSEU Browse screen.");

          dms.Enter(sess) ;
        }
      }

      public override bool IsScreen()
      {
        PresentationSpace ps = base.PresentationSpace;

        var f1 = ps.FindField(new DisplayLocation(1, 2));
        var f2 = ps.FindField(new DisplayLocation(2, 2));
        var f3 = ps.FindField(new DisplayLocation(1, 37));

        // qualified file name
        var f4 = ps.FindField(new DisplayLocation(1, 79));

        // member name
        var f5 = ps.FindField(new DisplayLocation(2, 79));

        if ((f1.TextContains("Columns") == true)
          && (f2.TextContains("SEU==>") == true)
          && (f3.TextContains("Browse") == true)
          && ((f4 != null) && (f4.Length >= 21)) &&
          ((f5 != null) && (f5.Length == 10)))
        {
          return true;
        }
        else
          return false;
      }

      public new static bool IsScreen(DisplaySession DisplaySession)
      {
        StrseuScreen.BrowseScreen dms =
          new StrseuScreen.BrowseScreen(DisplaySession.GetPresentationSpace());
        if (dms.IsScreen() == true)
          return true;
        else
          return false;
      }

      public new static bool IsScreen(EhllapiSettings Settings)
      {
        bool isScreen = false;
        using (DisplaySession sess = new DisplaySession())
        {
          sess.Connect(Settings.SessId);
          isScreen = StrseuScreen.BrowseScreen.IsScreen(sess);
        }
        return isScreen;
      }
    }
  }
}
