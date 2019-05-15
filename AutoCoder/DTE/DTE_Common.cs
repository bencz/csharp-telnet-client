using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.VisualStudio.CommandBars;
using EnvDTE80;

namespace AutoCoder.DTE
{

  public enum BuiltInBitmapIx
  {
    diskette = 3, printer = 4, search = 46, happyFace = 59,
    sadFace = 276, whiteLightBulb = 342, yellowLightBulb = 343,
    redHeart = 481, redDiamond = 482
  } ;

  public static class DTE_Common
  {

    public static Microsoft.VisualStudio.CommandBars.CommandBar
      FindCommandBar(
      DTE2 InDte2,
      string InBarName)
    {
      CommandBars bars = (CommandBars)InDte2.CommandBars;
      CommandBar bar = CommandBars_FindBar(bars, InBarName);
      return bar;
    }


    public static Microsoft.VisualStudio.CommandBars.CommandBar
      CommandBars_FindBar(
      Microsoft.VisualStudio.CommandBars.CommandBars InBars,
      string InBarName)
    {
      Microsoft.VisualStudio.CommandBars.CommandBar foundBar = null;

      try
      {
        foundBar = InBars[InBarName];
      }
      catch (System.Exception)
      {
        foundBar = null;
      }

      return foundBar;
    }


    /// <summary>
    /// return the CommandBar that the CommandBarControl represents.
    /// </summary>
    /// <param name="InControl"></param>
    /// <returns></returns>
    public static CommandBar ControlToCommandBar(CommandBarControl InControl)
    {
      CommandBar bar = null;
      if (InControl == null)
      {
        throw new ArgumentNullException(
          "InControl", "Control to convert to CommandBar is null");
      }

      else if (InControl.Type == MsoControlType.msoControlPopup)
      {
        CommandBarPopup pu = (CommandBarPopup)InControl;
        bar = pu.CommandBar;
      }

      else
      {
        throw new ArgumentException("Control type " + InControl.Type.ToString() +
          " not supported by ControlToCommandBar method.");
      }
      return bar;
    }

  }
}
