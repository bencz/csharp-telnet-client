using System;
using System.Collections.Generic;
using System.Text;
using EnvDTE;
using EnvDTE80;
using Extensibility;
using Microsoft.VisualStudio.CommandBars;

namespace AutoCoder.DTE
{
  public class DTE_Menu
  {
    DTE_Main mMain = null;
    CommandBar mBar = null;

    // the control of the parent menu that contains this sub menu.
    CommandBarControl mParentMenuControl = null;

    public DTE_Menu(DTE_Main InMain, CommandBar InBar)
    {
      mMain = InMain;
      mBar = InBar;
    }

    /// <summary>
    /// construct a DTE_Menu from the sub menu contained by a CommandBarControl 
    /// </summary>
    /// <param name="InMain"></param>
    /// <param name="InControl"></param>
    public DTE_Menu(DTE_Main InMain, CommandBarControl InParentMenuControl)
    {
      mMain = InMain;
      mParentMenuControl = InParentMenuControl;
      mBar = DTE_Common.ControlToCommandBar(mParentMenuControl);
    }

    /// <summary>
    /// the control of the parent menu which contains this sub menu.
    /// </summary>
    public CommandBarControl ParentMenuControl
    {
      get { return mParentMenuControl; }
    }

    public CommandBarControls Controls
    {
      get { return mBar.Controls; }
    }

    public CommandBarControl AddButtonControl(
      DTE_Command InCmd)
    {
      EnvDTE.Command cmd = InCmd.command;
      CommandBarControl added =
        (CommandBarControl)cmd.AddControl(
        mBar, Controls.Count + 1);
      added.Enabled = true;
      return added;
    }

    public DTE_Menu AddMenuControl(
      string InMenuName,
      int InPositionInParent)
    {
      CommandBar subMenu = null;
      EnvDTE.Commands commands = mMain.Commandsx;

      subMenu = (CommandBar)commands.AddCommandBar(
        InMenuName, vsCommandBarType.vsCommandBarTypeMenu,
        mBar,
        InPositionInParent);

      return new DTE_Menu(mMain, subMenu);
    }

    public void EnumerateMenuControls( DTE_Output InOutput )
    {
      CommandBarControls controls = mBar.Controls;
      foreach (CommandBarControl control in controls)
      {
        Microsoft.VisualStudio.CommandBars.MsoControlType msoType = control.Type;
        InOutput.WriteLine(
          "Control Caption: " + control.Caption + " type: " + msoType.ToString() +
          " Enabled: " + control.Enabled.ToString());
        InOutput.WriteLine("   DescriptionText: " + control.DescriptionText);
        InOutput.WriteLine("   TooltipText: " + control.TooltipText);
        InOutput.WriteLine(
          "  Index: " + control.Index.ToString() +
          "  Id: " + control.Id.ToString());

        Command cmd = mMain.Commandsx.Item(control.Id, -1);
        string cmdName = cmd.Name;

        if ( control.Type == MsoControlType.msoControlButton )
        {
          CommandBarButton but = (CommandBarButton)control;
          DTE_Button dbut = new DTE_Button(mMain, control);
          InOutput.WriteLine("     " + dbut.Command.Name + " guid: " + dbut.CommandGuid +
            " LocalizedName:" + dbut.Command.command.LocalizedName );

          DTE_Addin addin = mMain.FindAddinByGuid(dbut.CommandGuid);
          if (addin != null)
          {
            InOutput.WriteLine("  addin progid:" + addin.ProgId);
          }
        }
      }
    }

    public CommandBarControl
      FindControl( string InControlName)
    {
      Microsoft.VisualStudio.CommandBars.CommandBarControl foundControl = null;

      try
      {
        foundControl = mBar.Controls[InControlName];
      }
      catch (Exception)
      {
        foundControl = null;
      }

      return foundControl;
    }

    /// <summary>
    /// Find the sub menu contained within this menu.
    /// </summary>
    /// <param name="InName"></param>
    /// <returns></returns>
    public DTE_Menu FindMenuControl(string InName)
    {
      DTE_Menu foundMenu = null;
      CommandBarControl control = FindControl(InName);
      if ((control != null) && ( control.Type == MsoControlType.msoControlPopup ))
      {
        foundMenu = new DTE_Menu(mMain, control);
      }
      return foundMenu;
    }

    public DTE_Button FindButtonControl( ButtonText InText)
    {
      DTE_Button found = null;
      CommandBarControl control = FindControl(InText.Text);
      if ((control != null) && (control.Type == MsoControlType.msoControlButton))
      {
        found = new DTE_Button(mMain, control);
      }
      return found;
    }

    public void RemoveMenuControl( DTE_Menu InSubMenu )
    {
      EnvDTE.Commands commands = mMain.Commandsx;
      commands.RemoveCommandBar( InSubMenu.mBar ) ;
    }

    public void RemoveButtonControl( DTE_Button InButton)
    {
      object temp = null;
      InButton.Control.Delete( temp );
    }

    public void RemoveButtonControl( ButtonText InText)
    {
      DTE_Button button = FindButtonControl(InText);
      if (button == null)
        throw new ArgumentException(
          "Remove failed. ButtonControl " + InText.Text + " is not found");

      RemoveButtonControl(button);
    }

  }
}
