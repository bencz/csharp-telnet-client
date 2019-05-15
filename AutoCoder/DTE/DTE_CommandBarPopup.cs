using System;
using System.Collections.Generic;
using System.Text;

namespace AutoCoder.DTE
{
  public class DTE_CommandBarPopup
  {
    DTE_Main mMain = null;
    Microsoft.VisualStudio.CommandBars.CommandBarPopup mCmdBarPopup = null;

    public DTE_CommandBarPopup(
      DTE_Main InMain,
      Microsoft.VisualStudio.CommandBars.CommandBarPopup InCmdBarPopup)
    {
      mMain = InMain;
      mCmdBarPopup = InCmdBarPopup;
    }


    public void EnumerateControls(DTE_Output InOutput)
    {
      int ix = 0;
      foreach (Microsoft.VisualStudio.CommandBars.CommandBarControl control in mCmdBarPopup.Controls)
      {
        ++ix;
        InOutput.WriteLine(
          "Caption: " + control.Caption + " Text: " + control.DescriptionText +
          " index: " + control.Index.ToString() +
        " Enabled: " + control.Enabled.ToString( ) +
        " Visible: " + control.Visible.ToString( ) +
        " Type: " + control.Type.ToString( ) +
        " TypeCode: " + control.Type.GetTypeCode().ToString( ) );
      }
    }
  }
}
