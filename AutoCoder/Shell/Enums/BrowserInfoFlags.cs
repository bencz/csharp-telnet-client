using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AutoCoder.Shell.Enums
{
  [Flags]
  public enum BrowseInfoFlags : uint
  {
    /// <summary>
    /// No specified BIF_xxx flags. 
    /// /// </summary>
    /// 
    BIF_None = 0x0000,
    /// <summary>
    /// Only return file system directories. If the user selects folders that are
    /// not part of the file system, the OK button is grayed.
    /// /// </summary>
    BIF_RETURNONLYFSDIRS = 0x0001,
    // For finding a folder to start document searching         
    /// <summary>
    /// Do not include network folders below the domain level in the dialog 
    /// box's tree view control.  
    /// /// </summary>
    BIF_DONTGOBELOWDOMAIN = 0x0002,  // For starting the Find Computer 

    // Include a status area in the dialog box.   
    BIF_STATUSTEXT = 0x0004,     // Top of the dialog has 2 lines of text
    // for BROWSEINFO.lpszTitle and one line if
    // this flag is set.  Passing the message 
    // BFFM_SETSTATUSTEXTA to the hwnd can set the
    // rest of the text.  This is not used with BIF_USENEWUI and 
    // BROWSEINFO.lpszTitle gets  
    // all three lines of text.  

    /// <summary>
    /// Only return file system ancestors. An ancestor is a subfolder that 
    /// is beneath the root folder in the namespace hierarchy.  
    /// /// </summary>
    BIF_RETURNFSANCESTORS = 0x0008,

    /// <summary>
    /// Include an edit control in the browse dialog box that allows the user 
    /// to type the name of an item. 
    /// </summary>
    BIF_EDITBOX = 0x0010,           // Add an editbox to the dialog 

    /// <summary>
    /// If the user types an invalid name into the edit box, the browse 
    /// dialog box will call the application's BrowseCallbackProc with 
    /// the BFFM_VALIDATEFAILED message.   
    /// </summary>

    BIF_VALIDATE = 0x0020,          // insist on valid result (or CANCEL)

    /// <summary>
    /// Use the new user interface. Setting this flag provides the user 
    /// with a larger dialog box that can be resized.     
    /// /// </summary>
    BIF_NEWDIALOGSTYLE = 0x0040,    // Use the new dialog layout with the 
    // ability to resize
    // Caller needs to call OleInitialize() before using this API

    /// <summary>
    /// Use the new user interface, including an edit box. This flag is 
    /// equivalent to BIF_EDITBOX | BIF_NEWDIALOGSTYLE.
    /// </summary>
    BIF_USENEWUI = BIF_NEWDIALOGSTYLE | BIF_EDITBOX,

    /// <summary>
    /// The browse dialog box can display URLs. The BIF_USENEWUI and 
    /// BIF_BROWSEINCLUDEFILES flags must also be set.   
    /// </summary>
    BIF_BROWSEINCLUDEURLS = 0x0080, // Allow URLs to be displayed or 
    // entered. (Requires BIF_USENEWUI)

    /// <summary>
    /// When combined with BIF_NEWDIALOGSTYLE, adds a usage hint to the dialog 
    /// box in place of the edit box.
    /// </summary>
    BIF_UAHINT = 0x0100,    // Add a UA hint to the dialog, in place 
    // of the edit box. May not be combined with 
    // BIF_EDITBOX    

    /// <summary>
    /// Do not include the New Folder button in the browse dialog box.  
    /// </summary>
    BIF_NONEWFOLDERBUTTON = 0x0200, // Do not add the "New Folder" button 
    // to the dialog.  Only applicable with 
    // BIF_NEWDIALOGSTYLE.  

    /// <summary>
    /// When the selected item is a shortcut, return the PIDL of the shortcut 
    /// itself rather than its target.   
    /// </summary>
    BIF_NOTRANSLATETARGETS = 0x0400, // don't traverse target as shortcut

    /// <summary>
    /// Only return computers. If the user selects anything other than a 
    /// computer, the OK button is grayed.
    /// /// </summary>
    BIF_BROWSEFORCOMPUTER = 0x1000, // Browsing for Computers.

    /// <summary>
    /// Only allow the selection of printers. If the user selects anything 
    /// other than a printer, the OK button is grayed. 
    /// </summary>
    BIF_BROWSEFORPRINTER = 0x2000,  // Browsing for Printers

    /// <summary>
    /// The browse dialog box will display files as well as folders.
    /// /// </summary>
    BIF_BROWSEINCLUDEFILES = 0x4000,// Browsing for Everything

    /// <summary>
    /// The browse dialog box can display shareable resources on remote systems.
    /// </summary>
    BIF_SHAREABLE = 0x8000          // sharable resources displayed (remote shares, 
    // requires BIF_USENEWUI)
  }
}
