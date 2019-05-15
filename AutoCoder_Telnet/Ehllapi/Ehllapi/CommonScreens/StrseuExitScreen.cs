using System;
using System.Collections.Generic;
using System.Text;
using AutoCoder;
using AutoCoder.Ehllapi;
using AutoCoder.Ehllapi.Presentation;
using AutoCoder.Ehllapi.Common;

namespace AutoCoder.Ehllapi.CommonScreens
{
  public class StrseuExitScreen : DisplayScreen
  {

    public StrseuExitScreen( PresentationSpace InPs )
      : base( InPs )
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
        var f1 =
          base.PresentationSpace.FindField(new DisplayLocation(2, 79));
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
        PresentationSpaceField f1 =
          base.PresentationSpace.FindField(new DisplayLocation(1, 79));
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

    public void Enter( DisplaySession InSess )
    {
      InSess.SendKeys(Ehllapier.Key.Enter);
    }

    public void Enter(
      DisplaySession InSess,  string InChangeCreateFlag )
    {
      // tab to first entry field -  file name.
      DisplayLocation loc = new DisplayLocation(5, 42);
      InSess.TabRightToDisplayLocation(loc);

      InSess.SendKeys(InChangeCreateFlag ) ;

      InSess.SendKeys(Ehllapier.Key.Enter);
    }

    public override bool IsScreen( )
    {
      PresentationSpace ps = base.PresentationSpace;

      PresentationSpaceField f1 =
        ps.FindField( new DisplayLocation(1, 38));
      PresentationSpaceField f2 =
        ps.FindField( new DisplayLocation(3, 2));
      PresentationSpaceField f3 =
        ps.FindField( new DisplayLocation(5, 4));

      // qualified file name
      PresentationSpaceField f4 =
        ps.FindField(new DisplayLocation(5, 42));
      
      if (((f1 != null) && (f1.Text.Contains("Exit") == true)) &&
        ((f2 != null) && (f2.Text.Contains("Type choices, press Enter") == true)) &&
        ((f3 != null) && (f3.Text.Contains("Change/create member") == true)) &&
        ((f4 != null) && (f4.Length == 1 )))
      {
        return true;
      }
      else
        return false;
    }
  }
}
