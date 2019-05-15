using System;
using System.Collections.Generic;
using System.Text;

namespace AutoCoder.DTE
{
  public class DTE_CommandBars
  {
    DTE_Main mMain = null;
    Microsoft.VisualStudio.CommandBars.CommandBars mCmdBars = null;

    public DTE_CommandBars(DTE_Main InMain)
    {
      mMain = InMain;
      mCmdBars = (Microsoft.VisualStudio.CommandBars.CommandBars) mMain.dte2.CommandBars;
    }

    /// <summary>
    /// make sure no command bars exist with the specified name.
    /// </summary>
    /// <param name="InCmdBarName"></param>
    public void AssureNotExists(string InCmdBarName)
    {
      Microsoft.VisualStudio.CommandBars.CommandBar cmdbar = null;
      
      try
      {
        int cx = 0;
        bool found = false;
        foreach (Microsoft.VisualStudio.CommandBars.CommandBar cb in mCmdBars)
        {
          cx += 1;
          if (cb.Name == InCmdBarName)
          {
            found = true;
            break;
          }
        }
        if (found == true)
        {
          cmdbar = mCmdBars[InCmdBarName];
        }
      }
      catch (Exception excp)
      {
        cmdbar = null;
      }

      if (cmdbar != null)
      {
        cmdbar.Delete();
      }
    }
  }
}
