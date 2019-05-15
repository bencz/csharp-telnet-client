using System;
using System.Collections.Generic;
using System.Text;
using EnvDTE;
using EnvDTE80;
using Extensibility;
using Microsoft.VisualStudio.CommandBars;

namespace AutoCoder.DTE
{
  public class DTE_Main 
  {
    DTE2 mDte2 = null;

    public DTE_Main(DTE2 InDte)
    {
      mDte2 = InDte;
    }

    public EnvDTE.Commands Commandsx
    {
      get { return mDte2.Commands; }
    }

    public DTE_Commands Commands
    {
      get
      {
        EnvDTE.Commands xx = mDte2.Commands;
        DTE_Commands cmds = new DTE_Commands(this, xx);
        return cmds;
      }
    }
 
    public EnvDTE.Documents Documents
    {
      get { return mDte2.Documents; }
    }

    public DTE2 dte2
    {
      get { return mDte2; }
    }

    public DTE_TextWindow ActiveTextWindow
    {
      get
      {
        TextWindow tw = null;
        object awo = mDte2.ActiveWindow.Object;
        EnvDTE.Window theWindow = mDte2.ActiveWindow;
        if (awo is TextWindow)
          tw = (TextWindow)awo;

        if (tw == null)
          return null;
        else
          return new DTE_TextWindow(this, tw);
      }
    }

    public DTE_TextDocument ActiveTextDocument
    {
      get
      {
        EnvDTE.TextDocument doc = (EnvDTE.TextDocument) mDte2.ActiveDocument.Object("TextDocument");

        if (doc == null)
          return null;
        else if ((doc is EnvDTE.TextDocument) == false)
          return null;
        else
          return new DTE_TextDocument(this, (EnvDTE.TextDocument)doc);
      }
    }

    public void EnumerateCommands(DTE_Output InOutput)
    {
      DTE_Commands cmds = GetCommands();
      int jx = 0;
      foreach (DTE_Command cmd in cmds)
      {
        ++jx;
        InOutput.WriteLine(
          "Command " + cmd.Name + " ID:" + cmd.ID.ToString() +
          " guid:" + cmd.Guid +
          " jx: " + jx.ToString());
      }
    }

    public DTE_Addin FindAddin( string InProgId )
    {
      EnvDTE.AddIns addins = dte2.AddIns;
      EnvDTE.AddIn foundAddin = null;

      try
      {
        foundAddin = addins.Item(InProgId);
      }
      catch (Exception)
      {
        foundAddin = null;
      }

      if (foundAddin == null)
        return null;
      else
        return new DTE_Addin(this, foundAddin);
    }

    public DTE_Addin FindAddinByGuid(string InGuid)
    {
      EnvDTE.AddIns addins = dte2.AddIns;
      EnvDTE.AddIn foundAddin = null;

      foreach( AddIn addin in dte2.AddIns )
      {
        if ( addin.Guid == InGuid )
        {
          foundAddin = addin ;
          break ;
        }
      }

      if (foundAddin == null)
        return null;
      else
        return new DTE_Addin(this, foundAddin);
    }

    /// <summary>
    /// Find the DTE_Menu ( CommandBar ) within the DTE complex.
    /// </summary>
    /// <param name="InMenuName"></param>
    /// <returns></returns>
    public DTE_Menu FindMenu(string InMenuName)
    {
      CommandBars bars = (CommandBars)dte2.CommandBars;
      Microsoft.VisualStudio.CommandBars.CommandBar cmdBar = null;
      cmdBar = DTE_Common.FindCommandBar(dte2, InMenuName);

      return new DTE_Menu(this, cmdBar);
    }

    public DTE_CommandBar FindCommandBar(string InName)
    {
      Microsoft.VisualStudio.CommandBars.CommandBar cb =
        ((Microsoft.VisualStudio.CommandBars.CommandBars)mDte2.CommandBars)[InName];
      DTE_CommandBar cmdBar = new DTE_CommandBar(this, cb);
      return cmdBar;
    }

    public DTE_Commands GetCommands()
    {
      EnvDTE.Commands xx = mDte2.Commands;
      DTE_Commands cmds = new DTE_Commands(this, xx);
      return cmds;
    }

    // write the text line to the named OutputPane
    public void WriteToOutputPane(
      string InWindowName, string InMessageText)
    {
      OutputWindow outputWin = mDte2.ToolWindows.OutputWindow;
      OutputWindowPane pane;

      try
      {
        pane = outputWin.OutputWindowPanes.Item(InWindowName);
      }
      catch (Exception)
      {
        pane = outputWin.OutputWindowPanes.Add(InWindowName);
      }

      outputWin.Parent.Activate();
      pane.Activate();

      pane.OutputString(InMessageText + Environment.NewLine);
    }


  }
}
