using System;
using System.Collections.Generic;
using System.Text;
using EnvDTE;
using EnvDTE80;
using Extensibility;
using Microsoft.VisualStudio.CommandBars;

namespace AutoCoder.DTE
{
  public class DTE_Button
  {
    DTE_Main mMain = null;
    CommandBarControl mControl = null;

    public DTE_Button(DTE_Main InMain, CommandBarControl InControl)
    {
      mMain = InMain;
      mControl = InControl;
    }

    public CommandBarControl Control
    {
      get { return mControl; }
    }

    public DTE_Command Command
    {
      get
      {
        string cmdGuid = null;
        int cmdId = 0;
        mMain.Commands.CommandInfo(mControl, out cmdGuid, out cmdId);
        DTE_Command cmd = mMain.GetCommands().Item(cmdGuid, cmdId);
        return cmd;
//        Command cmd = mMain.Commands.Item(cmdGuid, cmdId);
//        return new DTE_Command(mMain, cmd);
      }
    }

    public string CommandGuid
    {
      get
      {
        string cmdGuid = null;
        int cmdId = 0;
        mMain.Commands.CommandInfo(mControl, out cmdGuid, out cmdId);
        return cmdGuid;
      }
    }

  }
}
