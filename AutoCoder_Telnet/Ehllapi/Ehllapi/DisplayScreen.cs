using System;
using System.Collections.Generic;
using System.Text;
using AutoCoder.Ehllapi.Presentation;

namespace AutoCoder.Ehllapi
{
  public abstract class DisplayScreen
  {
    PresentationSpace mPs = null;

    public DisplayScreen(PresentationSpace InPs)
    {
      mPs = InPs;
    }

    public PresentationSpace PresentationSpace
    {
      get { return mPs; }
    }

    public abstract bool IsScreen( ) ;

    public void ThrowIsNotScreen()
    {
      if (IsScreen() == false)
      {
        throw new EhllapiExcp("presentation space does not contain a " +
          this.GetType().Name);
      }
    }
  }
}
