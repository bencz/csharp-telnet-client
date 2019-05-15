using System;
using System.Collections.Generic;
using System.Drawing; 
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms; 

namespace AutoCoder.Forms
{
  /// <summary>
  /// methods specific to a caret in a window.
  /// </summary>
  public class Careter
  {
    [DllImport("user32.dll")]
    public static extern bool ShowCaret(IntPtr hWnd);

    [DllImport("user32.dll")]
    public static extern bool CreateCaret(
      IntPtr hWnd, IntPtr hBitmap,
      int nWidth, int nHeight);

    [DllImport("user32.dll")]
    public static extern bool SetCaretPos(int x, int y);

    public static void CreateUnderscoreCaret( Form InForm, FixedFontMeasurer InFfm )
    {
      CreateCaret(InForm.Handle, IntPtr.Zero, InFfm.CharWidth, 2);
    }

    public static void CreateVerticalBarCaret( Form InForm)
    {
      CreateCaret( InForm.Handle, IntPtr.Zero, 1, InForm.Font.Height );
    }

    public static void CreateVerticalBarCaret(
      IntPtr InHandle, Font InFont)
    {
      CreateCaret(InHandle, IntPtr.Zero, 1, InFont.Height);
    }


  }
}
