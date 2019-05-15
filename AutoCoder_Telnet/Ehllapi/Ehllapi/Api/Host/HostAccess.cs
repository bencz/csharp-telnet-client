using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.Diagnostics;

namespace Ehllapi.Api.Host
{
  /// <summary>
  /// static methods which call functions in the host access class library.
  /// </summary>
  public static class HostAccess
  {

    private static readonly string DllPath = 
      "c:\\srcpp\\ClientAccessHookDll32\\Debug\\ClientAccessHookDll32.dll";

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    private delegate IntPtr delGetSessionHwnd(char SessId);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    [return: MarshalAs(UnmanagedType.BStr)]
    private delegate string delGetSessionTitle(char SessId);

    [DllImport("kernel32")]
    public extern static IntPtr LoadLibrary(string librayName);

    [DllImport("kernel32")]
    public extern static IntPtr GetProcAddress(IntPtr hLibrary, string procedureName);

    [DllImport("kernel32.dll")]
    public static extern bool FreeLibrary(IntPtr hModule);

    /// <summary>
    /// Call into a C++ DLL function, which in turn, calls into the host access class
    /// library to get the window title of the client access session. Then use that
    /// window title to FindWindow and return the hWnd of the window.
    /// </summary>
    /// <param name="SessId"></param>
    /// <returns></returns>
    public static IntPtr GetSessionHwnd( string SessId )
    {
      IntPtr hSessWnd = IntPtr.Zero;
      IntPtr hLib = IntPtr.Zero;
      IntPtr hProc = IntPtr.Zero;
      IntPtr hTitleProc = IntPtr.Zero;

      string procName = "GetSessionHwnd";

      hLib = LoadLibrary(DllPath);
      if (hLib == IntPtr.Zero)
      {
        Trace.WriteLine("dll not loaded.");
      }

      if (hLib != IntPtr.Zero)
      {
        hProc = GetProcAddress(hLib, procName);
        if (hProc == IntPtr.Zero)
          Trace.WriteLine("GetProcAddress failed. Proc name:" + procName);
      }

      // call the method.
      if (hProc != IntPtr.Zero)
      {
        var getSessionHwnd =
             (delGetSessionHwnd)Marshal.GetDelegateForFunctionPointer(
             hProc, typeof(delGetSessionHwnd));
        hSessWnd = getSessionHwnd(SessId[0]);
      }

      if (hLib != IntPtr.Zero)
      {
        FreeLibrary(hLib);
        hLib = IntPtr.Zero;
        Trace.WriteLine("DLL is unloaded.");
      }

      return hSessWnd;
    }

    /// Call into a C++ DLL function, which in turn, calls into the host access class
    /// library to get the window title of the client access session.
    public static string GetSessionTitle(string SessId)
    {
      string sessTitle = null;
      IntPtr hLib = IntPtr.Zero;
      IntPtr hProc = IntPtr.Zero;
      string titleProcName = "GetSessionTitle";

      hLib = LoadLibrary(DllPath);
      if (hLib == IntPtr.Zero)
      {
        Trace.WriteLine("dll not loaded.");
      }

      if (hLib != IntPtr.Zero)
      {
        hProc = GetProcAddress(hLib, titleProcName);
        if (hProc == IntPtr.Zero)
          Trace.WriteLine("GetProcAddress failed. Proc name:" + titleProcName);
      }

      // call the method.
      if (hProc != IntPtr.Zero)
      {
        var getSessionTitle = (delGetSessionTitle)Marshal.GetDelegateForFunctionPointer(
          hProc, typeof(delGetSessionTitle));
        sessTitle = getSessionTitle(SessId[0]);
      }

      if (hLib != IntPtr.Zero)
      {
        FreeLibrary(hLib);
        hLib = IntPtr.Zero;
        Trace.WriteLine("DLL is unloaded.");
      }

      return sessTitle;
    }

  }
}
