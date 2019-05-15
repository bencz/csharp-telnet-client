using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Interop;

namespace AutoCoder.Ext.Windows
{
  public static class WindowExt
  {
    /// <summary>
    /// use WindowInteropHelper to get the HWND of the Window.
    /// </summary>
    /// <param name="Window"></param>
    /// <returns></returns>
    public static IntPtr GetHandle(this Window Window)
    {
      var interHelper = new WindowInteropHelper(Window);
      return interHelper.Handle;
    }
  }
}
