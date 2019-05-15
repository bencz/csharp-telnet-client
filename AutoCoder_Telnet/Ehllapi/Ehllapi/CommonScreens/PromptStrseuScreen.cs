using System;
using System.Collections.Generic;
using System.Text;
using AutoCoder.Ehllapi;
using AutoCoder.Ehllapi.Presentation;
using AutoCoder.Ehllapi.Common;

namespace AutoCoder.Ehllapi.CommonScreens
{
  public class PromptStrseuScreen : DisplayScreen
  {
    public PromptStrseuScreen( PresentationSpace InPs )
      : base( InPs )
    {
    }

    public string FileName
    {
      get
      {
        PresentationSpaceField f1 =
          base.PresentationSpace.FindField(new DisplayLocation(5, 37));
        if (f1 == null)
          throw new EhllapiExcp("PresentationSpace does not contain FileName field");
        return f1.Text.TrimEnd(new char[] { ' ' });
      }
    }

    public void Enter(
      DisplaySession InSess, string InMemberName )
    {
      // tab to first entry field -  source member name.
      DisplayLocation loc = new DisplayLocation(7, 37);
      InSess.TabRightToDisplayLocation(loc);

      InSess.SendKeys(InMemberName + Ehllapier.Key.FieldExit);

      InSess.SendKeys(Ehllapier.Key.Enter);
    }

    public override bool IsScreen( )
    {
      PresentationSpace ps = base.PresentationSpace;

      PresentationSpaceField f1 =
        ps.FindField(new DisplayLocation(1, 34));
      PresentationSpaceField f2 =
        ps.FindField(new DisplayLocation(5, 2));
      PresentationSpaceField f3 =
        ps.FindField(new DisplayLocation(7, 2));

      if (((f1 != null) && (f1.Text.Contains("(STRSEU)") == true)) &&
        ((f2 != null) && (f2.Text.Contains("Source file  . . . . . .") == true)) &&
        ((f3 != null) && (f3.Text.Contains("Source member  . . . .") == true)))
      {
        return true;
      }
      else
        return false;


    }
  }
}
