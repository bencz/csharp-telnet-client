using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AutoCoder.Shell.Enums
{
  // message from browser 
  public enum MessageFromBrowser : uint
  {
    /// <summary>
    /// The dialog box has finished initializing. 
    /// </summary>
    BFFM_INITIALIZED = 1,

    /// <summary>
    /// The selection has changed in the dialog box. 
    /// </summary>
    BFFM_SELCHANGED = 2,

    /// <summary>
    /// (ANSI) The user typed an invalid name into the dialog's edit box. A 
    /// nonexistent folder is considered an invalid name.
    /// </summary>
    BFFM_VALIDATEFAILEDA = 3,

    /// <summary>
    /// (Unicode) The user typed an invalid name into the dialog's edit box. A 
    /// nonexistent folder is considered an invalid name.
    /// </summary>
    BFFM_VALIDATEFAILEDW = 4,

    /// <summary>
    /// An IUnknown interface is available to the dialog box.
    /// </summary>
    BFFM_IUNKNOWN = 5
  }
}
