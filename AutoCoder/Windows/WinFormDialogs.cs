using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace AutoCoder.Windows
{
  public static class WinFormDialogs
  {

    public static string FolderBrowserDialog( string InInitialPath )
    {
      string selectedPath = null;

      FolderBrowserDialog dlg = new FolderBrowserDialog();
      dlg.ShowNewFolderButton = true;
      dlg.Description = "Select the folder to store the scanned documents in";

      if (InInitialPath != null)
      {
        dlg.SelectedPath = InInitialPath ;
      }
      
      DialogResult rv = dlg.ShowDialog();
      if (rv == System.Windows.Forms.DialogResult.OK)
        selectedPath = dlg.SelectedPath.Trim();
      return selectedPath;
    }
  }
}
