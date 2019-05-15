using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AutoCoder.Shell.Enums
{

  // messages to browser
  public enum MessageToBrowser : uint
  {
    /// <summary>
    /// Win32 API macro - start of user defined window message range.
    /// </summary>
    WM_USER = 0x0400,

    /// <summary>
    /// (ANSI) Sets the status text. Set lpData to point to a null-terminated 
    /// string with the desired text.
    /// </summary>
    BFFM_SETSTATUSTEXTA = WM_USER + 100,

    /// <summary>
    /// Enables or disables the dialog box's OK button.  lParam - To enable, 
    /// set to a nonzero value. To disable, set to zero.
    /// </summary>
    BFFM_ENABLEOK = WM_USER + 101,

    /// <summary>
    /// (ANSI) Specifies the path of a folder to select. 
    /// </summary>
    BFFM_SETSELECTIONA = WM_USER + 102,

    /// <summary>
    /// (Unicode) Specifies the path of a folder to select.
    /// </summary>
    BFFM_SETSELECTIONW = WM_USER + 103,

    /// <summary>
    /// (Unicode) Sets the status text. Set lpData to point to a null-terminated 
    /// string with the desired text.
    /// </summary>
    BFFM_SETSTATUSTEXTW = WM_USER + 104,

    /// <summary>
    /// Sets the text that is displayed on the dialog box's OK button. 
    /// </summary>
    BFFM_SETOKTEXT = WM_USER + 105, // Unicode only  

    /// <summary>
    /// Specifies the path of a folder to expand in the Browse dialog box.
    /// </summary>
    BFFM_SETEXPANDED = WM_USER + 106      // Unicode only 
  }

}
