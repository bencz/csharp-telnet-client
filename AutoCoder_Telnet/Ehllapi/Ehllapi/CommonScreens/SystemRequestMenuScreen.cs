using System;
using System.Collections.Generic;
using System.Text;
using AutoCoder.Ehllapi;
using AutoCoder.Ehllapi.Presentation;
using AutoCoder.Ehllapi.Common;

namespace AutoCoder.Ehllapi.CommonScreens
{
  public class SystemRequestMenuScreen : DisplayScreen
  {
    string mSelection = null;

    public SystemRequestMenuScreen( PresentationSpace InPs )
      : base( InPs )
    {
    }

    public string Selection
    {
      set { mSelection = value; }
    }

    public void Enter(
      DisplaySession InSess, string InSelection)
    {
      // tab to entry field as row xxx, col yyy
      var loc = new DisplayLocation( 20, 7 ) ;
      InSess.TabRightToDisplayLocation(loc);

      InSess.SendKeys(InSelection + Ehllapier.Key.FieldExit);
      InSess.SendKeys(Ehllapier.Key.Enter);
      InSess.Wait();
    }

    public override bool IsScreen( )
    {
      var ps = base.PresentationSpace;

      var f1 = ps.FindField(new DisplayLocation(1, 34));
      var f2 = ps.FindField(new DisplayLocation(19, 2));
      var f3 = ps.FindField(new DisplayLocation(3, 2));

      if ((f1.TextContains("System Request") == true)
        && (f2.TextContains("Selection") == true)
        && (f3.TextContains("Select one of the following:") == true))
      {
        return true;
      }
      else
        return false;
    }
  }
}
