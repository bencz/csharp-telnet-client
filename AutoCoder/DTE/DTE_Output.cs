using System;
using System.Collections.Generic;
using System.Text;
using EnvDTE;
using EnvDTE80;
using Extensibility;
using Microsoft.VisualStudio.CommandBars;

namespace AutoCoder.DTE
{
  /// <summary>
  /// An activated output pane within the DTE complex 
  /// </summary>
  public class DTE_Output
  {
    DTE_Main mMain;
    string mPaneName;
    private OutputWindowPane mPane;

    public DTE_Output(DTE_Main InMain, string InPaneName)
    {
      mMain = InMain;
      Open(InPaneName);
    }

    public OutputWindowPane OutputPane
    {
      get { return mPane; }
    }

    private void Open(string InPaneName)
    {
      Window win = mMain.dte2.Windows.Item(Constants.vsWindowKindOutput);
      OutputWindow window = (OutputWindow)win.Object;
      
      mPaneName = InPaneName;

      try
      {
        mPane = window.OutputWindowPanes.Item( mPaneName );
      }
      catch (Exception)
      {
        mPane = window.OutputWindowPanes.Add( mPaneName );
      }
    }

    public void WriteLine(string InText)
    {
      mPane.OutputString( InText + Environment.NewLine);
    }

  }
}
