using System;
using System.Collections.Generic;
using System.Text;

namespace AutoCoder.DTE
{
  public class DTE_Commands : IEnumerable<DTE_Command>
  {
    DTE_Main mMain = null;
    EnvDTE.Commands mCmds = null ;

    public DTE_Commands(DTE_Main InMain, EnvDTE.Commands InCmds)
    {
      mMain = InMain;
      mCmds = InCmds;
    }

    public DTE_Command AddCommand( 
      EnvDTE.AddIn InInstance, DTE_Addin InAddin, DTE_Command.ConstructProperties InProps )
    {
      object[] contextGuids = InProps.ContextGuids;

      //Add a command to the Commands collection:
      EnvDTE.Command command =
        mCmds.AddNamedCommand(
        InAddin.addin, InProps.Name, InProps.ButtonText, InProps.TooltipText,
        InProps.IsMsoButton, InProps.BitmapIx,
        ref contextGuids,
        InProps.Useable ) ;

      InProps.ContextGuids = contextGuids;

      DTE_Command cmd = new DTE_Command(mMain, command);
      return cmd;
    }

    public DTE_CommandBar AddCommandBar()
    {
      Microsoft.VisualStudio.CommandBars.CommandBar cmdbar = null;
      DTE_CommandBar bar = new DTE_CommandBar(mMain, cmdbar);
      return bar;

      //      myPermanentCommandBar1 =
      //        (Microsoft.VisualStudio.CommandBars.CommandBar)
      //        InMain.dte2.Commands.AddCommandBar(
      //         MY_PERMANENT_COMMANDBAR_POPUP1_NAME,
      //         EnvDTE.vsCommandBarType.vsCommandBarTypeMenu,
      //         toolsCmdBar.cmdbar, toolsCmdBar.cmdbar.Controls.Count + 1);
      //      myPermanentCommandBar1.Enabled = true;
    }

    public void CommandInfo(
      Microsoft.VisualStudio.CommandBars.CommandBarControl InControl, 
      out string OutCmdGuid, out int OutCmdId)
    {
      string cmdGuid = null;
      int cmdId = 0;
      mCmds.CommandInfo(InControl, out cmdGuid, out cmdId);
      OutCmdGuid = cmdGuid;
      OutCmdId = cmdId;
    }

    public DTE_Command Item( string InCmdGuid, int InCmdId )
    {
      EnvDTE.Command cmd = mCmds.Item(InCmdGuid, InCmdId);
      DTE_Command xx = new DTE_Command(mMain, cmd);
      return xx;
    }

    #region IEnumerable<DTE_Command> Members

    public IEnumerator<DTE_Command> GetEnumerator()
    {
      if (mCmds == null)
        yield break;
      else
      {
        foreach (EnvDTE.Command cmd in mCmds)
        {
          DTE_Command xx = new DTE_Command(mMain, cmd);
          yield return xx;
        }
      }
    }

    #endregion

    #region IEnumerable Members

    System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
    {
      throw new NotImplementedException();
    }

    #endregion
  }
}
