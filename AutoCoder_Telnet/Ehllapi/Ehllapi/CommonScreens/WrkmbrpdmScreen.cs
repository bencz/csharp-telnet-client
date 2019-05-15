using System;
using System.Collections.Generic;
using System.Text;
using AutoCoder.Ehllapi;
using AutoCoder.Ehllapi.Presentation;
using AutoCoder.Ehllapi.Common;

namespace AutoCoder.Ehllapi.CommonScreens
{
  public partial class WrkmbrpdmScreen : DisplayScreen
  {

    public WrkmbrpdmScreen( PresentationSpace PresentationSpace )
      : base( PresentationSpace )
    {
    }

    public string FileName
    {
      get
      {
        PresentationSpaceField f1 =
          base.PresentationSpace.FindField(new DisplayLocation(3, 22));
        if (f1 == null)
          throw new EhllapiExcp("PresentationSpace does not contain FileName field");
        return f1.Text.TrimEnd(new char[] { ' ' });
      }
    }

    public string LibraryName
    {
      get
      {
        PresentationSpaceField f1 =
          base.PresentationSpace.FindField(new DisplayLocation(4, 24));
        if (f1 == null)
          throw new EhllapiExcp("PresentationSpace does not contain LibraryName field");
        return f1.Text.TrimEnd(new char[] { ' ' });
      }
    }

    public string PositionToMember
    {
      get
      {
        PresentationSpaceField f1 =
          base.PresentationSpace.FindField(new DisplayLocation(4, 70));
        if (f1 == null)
          throw new EhllapiExcp(
            "PresentationSpace does not contain PostionToMember field");
        return f1.Text.TrimEnd(new char[] { ' ' });
      }
    }

    public void Enter(
      DisplaySession Sess,
      string FileName, string LibraryName, string PositionToMember)
    {
      // tab to first entry field -  file name.
      DisplayLocation loc = new DisplayLocation(3, 22);
      Sess.TabRightToDisplayLocation(loc);

      if (FileName != null)
        Sess.SendKeys(FileName + Ehllapier.Key.FieldExit);

      if (LibraryName != null)
        Sess.SendKeys(LibraryName + Ehllapier.Key.FieldExit);

      if (PositionToMember != null)
        Sess.SendKeys(PositionToMember + Ehllapier.Key.FieldExit);

      Sess.SendKeys(Ehllapier.Key.Enter);
    }

    public void EnterSubfileOption(
      DisplaySession Sess,
      int RowNbr, string OptionValue )
    {
      // tab to subfile option entry field.
      DisplayLocation loc = new DisplayLocation(11 + RowNbr, 2);
      Sess.TabRightToDisplayLocation(loc);

      Sess.SendKeys(OptionValue + Ehllapier.Key.FieldExit);

      Sess.SendKeys(Ehllapier.Key.Enter);
    }

    public void F6_Add(DisplaySession InSess)
    {
      InSess.SendKeys(Ehllapier.Key.F6);
    }

    public SubfileRow GetSubfileRow(int RowNbr)
    {
      int sflStartRowNbr = 11;
      int rx = sflStartRowNbr + RowNbr;

      SubfileRow row = new SubfileRow();

      PresentationSpace ps = base.PresentationSpace;

      // option. column 2.
      {
        PresentationSpaceField f1 =
        ps.FindField(new DisplayLocation(rx, 2));
        row.Opt = f1.Text;
      }

      // member name. column 7.
      {
        PresentationSpaceField f1 =
        ps.FindField(new DisplayLocation(rx, 7));
        row.Member = f1.Text;
      }

      // member type. column 19.
      {
        PresentationSpaceField f1 =
        ps.FindField(new DisplayLocation(rx, 19));
        row.Type = f1.Text;
      }

      // member text. column 31.
      {
        PresentationSpaceField f1 =
        ps.FindField(new DisplayLocation(rx, 31));
        row.Text = f1.Text;
      }

      return row;
    }

    public override bool IsScreen( )
    {
      PresentationSpace ps = base.PresentationSpace;

      PresentationSpaceField f1 =
        ps.FindField( new DisplayLocation(1, 28));
      PresentationSpaceField f2 =
        ps.FindField( new DisplayLocation(3, 2));
      PresentationSpaceField f3 =
        ps.FindField( new DisplayLocation(4, 4));

      if ((f1.TextContains("Work with Members Using PDM") == true) 
        && (f2.TextContains("File  . . . . . .") == true)
        && (f3.TextContains("Library . . . .") == true))
      {
        return true;
      }
      else
        return false;
    }

    public static bool IsScreen(DisplaySession DisplaySession)
    {
      WrkmbrpdmScreen dms =
        new WrkmbrpdmScreen(DisplaySession.GetPresentationSpace());
      if (dms.IsScreen() == true)
        return true;
      else
        return false;
    }
  }
}
