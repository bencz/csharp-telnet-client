using System;
using System.Collections.Generic;
using System.Text;

namespace AutoCoder.DTE
{
  public class DTE_CommandBar
  {
    DTE_Main mMain = null;
 	  Microsoft.VisualStudio.CommandBars.CommandBar mCmdBar = null ;

    // --------------------- DTE_CommandBar -----------------------------
    public DTE_CommandBar(
      DTE_Main InMain, 
      Microsoft.VisualStudio.CommandBars.CommandBar InCmdBar )
    {
      mMain = InMain;
      mCmdBar = InCmdBar;
    }

    public Microsoft.VisualStudio.CommandBars.CommandBar cmdbar
    {
      get { return mCmdBar; }
    }

    // -------------------------- AddButton -------------------------------
    public Microsoft.VisualStudio.CommandBars.CommandBarButton AddButton(
      DTE_Command InCmd, string InCaptionText)
    {
      Microsoft.VisualStudio.CommandBars.CommandBarButton but = null;
      but = (Microsoft.VisualStudio.CommandBars.CommandBarButton)
        InCmd.command.AddControl(mCmdBar, mCmdBar.Controls.Count + 1);

      but.Caption = InCaptionText;

      return but;
    }

    // ---------------------- AddCommandBar -----------------------------
    public DTE_CommandBar AddCommandBar(
      string InBarName)
    {
      Microsoft.VisualStudio.CommandBars.CommandBar envBar = null;
      DTE_CommandBar addBar = null;

      envBar =
        (Microsoft.VisualStudio.CommandBars.CommandBar)
        mMain.dte2.Commands.AddCommandBar(
        InBarName,
         EnvDTE.vsCommandBarType.vsCommandBarTypeMenu,
         this.cmdbar, this.cmdbar.Controls.Count + 1);
      envBar.Enabled = true;

      addBar = new DTE_CommandBar(mMain, envBar);
      return addBar;
    }

    /// <summary>
    /// if the item exists on the command bar, delete it. 
    /// </summary>
    /// <param name="InCmdBarName"></param>
    public bool AssureNotExists(string InItemName)
    {
      bool wasDeleted = false;
      while (true)
      {
        try
        {
          mCmdBar.Controls[InItemName].Delete(false);
          wasDeleted = true;
        }
        catch (Exception excp)
        {
          break;
        }
      }
      return wasDeleted;
    }

    // ------------------------ FindSubCommandBar -----------------------
    public DTE_CommandBar FindSubCommandBar(string InName)
    {
      // the popup is a child menu on the CommandBar menu.
      Microsoft.VisualStudio.CommandBars.CommandBarControl toolsControl = null;
      toolsControl = mCmdBar.Controls[InName];

      // cast to a CommandBar
      Microsoft.VisualStudio.CommandBars.CommandBar subCmdBar = null;

      try
      {
        subCmdBar = (Microsoft.VisualStudio.CommandBars.CommandBar)toolsControl;
      }
      catch (Exception)
      {
        subCmdBar = null;
      }

      if (subCmdBar == null)
        return null;
      else
        return new DTE_CommandBar(mMain, subCmdBar);
    }

    public DTE_CommandBarPopup FindCommandBarPopup(string InName)
    {
      // the popup is a child menu on the CommandBar menu.
      Microsoft.VisualStudio.CommandBars.CommandBarControl toolsControl = null;
      toolsControl = mCmdBar.Controls[InName];
      
      // cast to a CommandBarPopup
      Microsoft.VisualStudio.CommandBars.CommandBarPopup popup = null;
      popup = (Microsoft.VisualStudio.CommandBars.CommandBarPopup)toolsControl;

      if (popup == null)
        return null;
      else
        return new DTE_CommandBarPopup(mMain, popup);
    }

  }
}
