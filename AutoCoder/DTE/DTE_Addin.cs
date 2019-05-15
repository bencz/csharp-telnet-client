using System;
using System.Collections.Generic;
using System.Text;
using EnvDTE;
using EnvDTE80;
using Extensibility;
using Microsoft.VisualStudio.CommandBars;

namespace AutoCoder.DTE
{
  public class DTE_Addin
  {
    DTE_Main mMain = null;
    EnvDTE.AddIn mAddin = null;

    public DTE_Addin(DTE_Main InMain, EnvDTE.AddIn InAddin)
    {
      mMain = InMain;
      mAddin = InAddin;
    }

    public EnvDTE.AddIn addin
    {
      get { return mAddin; }
    }

    public string ProgId
    {
      get { return mAddin.ProgID; }
    }

    // add a command to the addin.
    // the added command will be assigned a full name from the progid of the addin.
    // VisualStudio will call the Exec function of the DLL of the addin when the command
    // is executed.
    // InAddinName    : local name of the command within the addin. The full name is a 
    //                  concat of the progid of the addin and this name.
    // InBitmapIx     : index into collection of icon bitmaps of VisualStudio. 
    //                  some values:  
    public DTE_Command AddCommand(
      string InName,
      ButtonText InButtonText,
      string InToolTipText,
      int InBitmapIx )
    {
      int defaultCommandDisabled = 16;
      object[] contextUIGuid = new object[] { };

      EnvDTE.Commands commands = mMain.dte2.Commands;
      EnvDTE.Command addedCmd =
        commands.AddNamedCommand(
        addin, InName, InButtonText.Text, InToolTipText, true,
        InBitmapIx, ref contextUIGuid, defaultCommandDisabled);

      return new DTE_Command(mMain, addedCmd);
    }

    public DTE_Command AddCommand(
      string InName,
      ButtonText InButtonText,
      string InToolTipText,
      BuiltInBitmapIx InBitmapIx )
    {
      DTE_Command addedCmd =
        AddCommand(
        InName, InButtonText, InToolTipText,
        (int)InBitmapIx);
      return addedCmd;
    }

    public DTE_Command FindCommand(string InName, DTE_Output InOutput)
    {
      EnvDTE.Command found = null;
      string fullName = mAddin.ProgID + "." + InName;

      EnvDTE.Commands commands = mMain.dte2.Commands;

      try
      {
        found = commands.Item(fullName, -1);
      }
      catch (Exception excp)
      {
        found = null;
        string xx = excp.ToString();
      }

      if (found == null)
        return null;
      else
        return new DTE_Command(mMain, found);
    }

  }
}

