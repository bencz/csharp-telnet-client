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
    public class EditScreen : DisplayScreen
    {

      public EditScreen(PresentationSpace PresentationSpace)
        : base(PresentationSpace)
      {
      }

      public string FileName
      {
        get
        {
          return QualifiedFileName.a;
        }
      }

      public string LibraryName
      {
        get
        {
          return QualifiedFileName.b;
        }
      }

      public string MemberName
      {
        get
        {
          var f1 = base.PresentationSpace.FindField(new DisplayLocation(2, 79));
          if (f1 == null)
            throw new EhllapiExcp(
              "PresentationSpace does not contain MemberName field");
          return f1.Text.TrimEnd(new char[] { ' ' });
        }
      }

      public StringPair QualifiedFileName
      {
        get
        {

          // get the psFld that contains qualified file name.
          var f1 = base.PresentationSpace.FindField(new DisplayLocation(1, 79));
          if (f1 == null)
            throw new EhllapiExcp(
              "PresentationSpace does not contain QualifiedFileName field");
          string qualName = f1.Text.Trim(new char[] { ' ' });

          // locate the qualifiying slash.
          int ix = qualName.IndexOf('/');
          if (ix == -1)
            throw new EhllapiExcp(
              "PresentationSpace does not contain QualifiedFileName field");

          // library before the slash, file name after.
          string libName = qualName.Substring(0, ix);
          string fileName = qualName.Substring(ix + 1);
          return new StringPair(fileName, libName);
        }
      }

      public void Enter(DisplaySession InSess)
      {
        InSess.SendKeys(Ehllapier.Key.Enter);
      }

      public void Enter(DisplaySession InSess, string InText)
      {
        InSess.SendKeys(InText + Ehllapier.Key.Enter);
      }

      public void Enter_Insert(DisplaySession InSess)
      {
        // tab to first entry field -  file name.
        DisplayLocation loc = new DisplayLocation(3, 1);
        InSess.TabRightToDisplayLocation(loc);

        InSess.SendKeys("I" + Ehllapier.Key.Enter);
      }

      public void Enter_Text(DisplaySession InSess, string InText)
      {
        // tab to first entry field -  file name.
        InSess.TabRightToColumn(9);

        InSess.SendKeys(InText + Ehllapier.Key.Enter);
      }

      public void Enter(
        DisplaySession InSess,
        string InFileName, string InLibraryName, string InPositionToMember)
      {
        // tab to first entry field -  file name.
        DisplayLocation loc = new DisplayLocation(3, 22);
        InSess.TabRightToDisplayLocation(loc);

        if (InFileName != null)
          InSess.SendKeys(InFileName + Ehllapier.Key.FieldExit);

        if (InLibraryName != null)
          InSess.SendKeys(InLibraryName + Ehllapier.Key.FieldExit);

        if (InPositionToMember != null)
          InSess.SendKeys(InPositionToMember + Ehllapier.Key.FieldExit);

        InSess.SendKeys(Ehllapier.Key.Enter);
      }

      public PresentationSpace F3_Exit(DisplaySession InSess)
      {
        PresentationSpace ps = null;
        InSess.SendKeys(Ehllapier.Key.F3);
        ps = InSess.GetPresentationSpace();
        return ps;
      }

      public static void F3_Exit(EhllapiSettings Settings)
      {
        using (DisplaySession sess = new DisplaySession())
        {
          sess.Connect(Settings.SessId);

          StrseuScreen.EditScreen dms =
            new StrseuScreen.EditScreen(sess.GetPresentationSpace());
          if (dms.IsScreen() == false)
            throw new ApplicationException("not at STRSEU Edit screen.");

          dms.F3_Exit(sess);
        }
      }

      public override bool IsScreen()
      {
        PresentationSpace ps = base.PresentationSpace;

        PresentationSpaceField f1 =
          ps.FindField(new DisplayLocation(1, 2));
        PresentationSpaceField f2 =
          ps.FindField(new DisplayLocation(2, 2));
        PresentationSpaceField f3 =
          ps.FindField(new DisplayLocation(1, 39));

        // qualified file name
        PresentationSpaceField f4 =
          ps.FindField(new DisplayLocation(1, 79));

        // member name
        PresentationSpaceField f5 =
          ps.FindField(new DisplayLocation(2, 79));

        if ((f1.TextContains("Columns") == true)
          && (f2.TextContains("SEU==>") == true)
          && (f3.TextContains("Edit") == true) 
          && ((f4 != null) && (f4.Length >= 21)) &&
          ((f5 != null) && (f5.Length == 10)))
        {
          return true;
        }
        else
          return false;
      }

      public static bool IsScreen(DisplaySession DisplaySession)
      {
        StrseuScreen.EditScreen dms =
          new StrseuScreen.EditScreen(DisplaySession.GetPresentationSpace());
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
          isScreen = StrseuScreen.EditScreen.IsScreen(sess);
        }
        return isScreen;
      }

    }
  }
}

