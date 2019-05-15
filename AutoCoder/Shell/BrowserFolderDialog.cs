using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;
using AutoCoder.Shell.Enums;

namespace AutoCoder.Shell
{

  /// <summary>
  /// Represents a common dialog box (Win32::SHBrowseForFolder()) that allows a user 
  /// to select a folder.
  /// </summary>
  public class BrowseForFolderDialog
  {
    /// <summary>
    /// Gets the current and or final selected folder path.
    /// </summary>
    public string SelectedFolder
    {
      get;
      protected set;
    }

    /// <summary>
    /// Gets or sets the string that is displayed above the tree view control in 
    /// the dialog box (must set BEFORE calling ShowDialog()).      
    /// /// </summary>
    public string Title
    {
      get { return BrowseInfo.lpszTitle; }
      set { BrowseInfo.lpszTitle = value; }
    }        /// <summary>

    /// Gets or sets the initially selected folder path.
    /// </summary>
    public string InitialFolder { get; set; }

    /// <summary>
    /// Gets or sets the initially selected and expanded folder path.  Overrides 
    /// SelectedFolder.
    /// </summary>
    public string InitialExpandedFolder { get; set; }

    /// <summary>
    /// Gets or sets the text for the dialog's OK button.
    /// </summary>
    public string OKButtonText { get; set; }

    BROWSEINFOW browseInfo;

    /// <summary>
    /// Provides direct access to the Win32::SHBrowseForFolder() BROWSEINFO 
    /// structure used to create the dialog in ShowDialog(). 
    /// </summary>
    public BROWSEINFOW BrowseInfo
    {
      get { return browseInfo; }
      protected set { browseInfo = value; }
    }

    /// <summary>
    /// Provides direct access to the ulFlags field of the Win32::SHBrowseForFolder() 
    /// structure used to create the dialog in ShowDialog(). 
    /// </summary>
    public BrowseInfoFlags BrowserDialogFlags
    {
      get { return BrowseInfo.ulFlags; }
      set { BrowseInfo.ulFlags = value; }
    }

    /// <summary>
    /// Constructs a BrowseForFolderDialog with default BrowseInfoFlags set 
    /// to BIF_NEWDIALOGSTYLE.
    /// </summary>
    public BrowseForFolderDialog()
    {
      BrowseInfo = new BROWSEINFOW();
      BrowseInfo.hwndOwner = IntPtr.Zero;
      BrowseInfo.pidlRoot = IntPtr.Zero;
      BrowseInfo.pszDisplayName = new String(' ', 260);
      BrowseInfo.lpszTitle = "Select a folder:";
      BrowseInfo.ulFlags = BrowseInfoFlags.BIF_NEWDIALOGSTYLE;
      BrowseInfo.lpfn = new BrowseCallbackProc(BrowseEventHandler);
      BrowseInfo.lParam = IntPtr.Zero;
      BrowseInfo.iImage = -1;
    }

    /// <summary>
    /// Shows the dialog (Win32::SHBrowseForFolder()).  
    /// </summary>
    public Nullable<bool> ShowDialog()
    {
      return PInvokeSHBrowseForFolder(null);
    }

    /// <summary>
    /// Shows the dialog (Win32::SHBrowseForFolder()) with its hwndOwner set to the handle of 'owner'.        /// </summary>
    public Nullable<bool> ShowDialog(Window owner)
    {
      return PInvokeSHBrowseForFolder(owner);
    }

    private Nullable<bool> PInvokeSHBrowseForFolder(Window owner)
    {
      WindowInteropHelper windowhelper;
      if (null != owner)
      {
        windowhelper = new WindowInteropHelper(owner);
        BrowseInfo.hwndOwner = windowhelper.Handle;
      }

      IntPtr pidl = SHBrowseForFolderW(browseInfo);
      if (IntPtr.Zero != pidl)
      {
        StringBuilder pathsb = new StringBuilder(260);
        if (false != SHGetPathFromIDList(pidl, pathsb))
        {
          SelectedFolder = pathsb.ToString();
          Marshal.FreeCoTaskMem(pidl);
          return true;
        }
      }
      return false;
    }

    private int BrowseEventHandler(
      IntPtr hwnd, MessageFromBrowser uMsg, IntPtr lParam, IntPtr lpData)
    {
      switch (uMsg)
      {
        case MessageFromBrowser.BFFM_INITIALIZED:
          {
            // The dialog box has finished initializing.
            // lParam   Not used, value is NULL.
            if (!string.IsNullOrEmpty(InitialExpandedFolder))
              SendMessageW(
                hwnd, MessageToBrowser.BFFM_SETEXPANDED, new IntPtr(1), 
                InitialExpandedFolder);
            else if (!string.IsNullOrEmpty(InitialFolder))
              SendMessageW(
                hwnd, MessageToBrowser.BFFM_SETSELECTIONW, new IntPtr(1), InitialFolder);

            if (!string.IsNullOrEmpty(OKButtonText))
              SendMessageW(
                hwnd, MessageToBrowser.BFFM_SETOKTEXT, new IntPtr(1), OKButtonText);
            break;
          }

        case MessageFromBrowser.BFFM_SELCHANGED:
          {
            // The selection has changed in the dialog box.
            // lParam A pointer to an item identifier list (PIDL) identifying the newly 
            // selected item.
            StringBuilder pathsb = new StringBuilder(260);
            if (false != SHGetPathFromIDList(lParam, pathsb))
            {
              SelectedFolder = pathsb.ToString();
            }
            break;
          }

        case MessageFromBrowser.BFFM_VALIDATEFAILEDA:
          // ANSI            
          {
            // The user typed an invalid name into the dialog's edit box. A nonexistent
            // folder is considered an invalid name.
            // lParam   A pointer to a string containing the invalid name. An 
            // application can use this data in an error dialog informing the user 
            // that the name was not valid.
            // Return zero to dismiss the dialog or nonzero to keep the dialog displayed
            break;
          }

        case MessageFromBrowser.BFFM_VALIDATEFAILEDW:
          // Unicode
          {
            // The user typed an invalid name into the dialog's edit box. A nonexistent
            // folder is considered an invalid name.
            // lParam   A pointer to a string containing the invalid name. An 
            // application can use this data in an error dialog informing the user
            // that the name was not valid.
            // Return zero to dismiss the dialog or nonzero to keep the dialog 
            // displayed    
            break;
          }

        case MessageFromBrowser.BFFM_IUNKNOWN:
          {
            // An IUnknown interface is available to the dialog box.
            // lParam   A pointer to an IUnknown interface.
            break;
          }
      }
      return 0;
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    public class BROWSEINFOW
    {
      /// <summary>
      /// A handle to the owner window for the dialog box.
      /// </summary>
      public IntPtr hwndOwner;

      /// <summary>
      /// A pointer to an item identifier list (PIDL) specifying the location 
      /// of the root folder from which to start browsing.
      /// </summary>
      public IntPtr pidlRoot;         // PCIDLIST_ABSOLUTE

      /// <summary>
      /// The address of a buffer to receive the display name of the folder selected 
      /// by the user. The size of this buffer is assumed to be MAX_PATH characters.
      /// </summary>
      public string pszDisplayName;  // Output parameter! (length must be >= MAX_PATH)

      /// <summary>
      /// The address of a null-terminated string that is displayed above the tree 
      /// view control in the dialog box.
      /// </summary>
      public string lpszTitle;

      /// <summary>
      /// Flags specifying the options for the dialog box.  
      /// </summary>
      public BrowseInfoFlags ulFlags;

      /// <summary>
      /// A BrowseCallbackProc delegate that the dialog box calls when an 
      /// event occurs. 
      /// </summary>
      public BrowseCallbackProc lpfn;

      /// <summary>
      /// An application-defined value that the dialog box passes to the 
      /// BrowseCallbackProc delegate, if one is specified.  
      /// </summary>
      public IntPtr lParam;

      /// <summary>
      /// A variable to receive the image associated with the selected folder. 
      /// The image is specified as an index to the system image list.
      /// </summary>
      public int iImage;       // Output parameter!
    }

    public delegate int BrowseCallbackProc(
      IntPtr hwnd, MessageFromBrowser uMsg, IntPtr lParam, IntPtr lpData);

    [DllImport("shell32.dll")]
    private static extern IntPtr SHBrowseForFolderW(
      [MarshalAs(UnmanagedType.LPStruct), In, Out] BROWSEINFOW bi);

    [DllImport("shell32.dll")]
    private static extern bool SHGetPathFromIDList(IntPtr pidl, StringBuilder path);

    [DllImport("user32.dll")]
    public static extern IntPtr SendMessageW(
      IntPtr hWnd, uint Msg, IntPtr wParam, IntPtr lParam);

    [DllImport("user32.dll")]
    public static extern IntPtr SendMessageW(
      IntPtr hWnd, MessageToBrowser msg, IntPtr wParam, 
      [MarshalAs(UnmanagedType.LPWStr)] string str);

  }
}
