using System;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.InteropServices;

namespace AutoCoder.COMInterop
{
  [StructLayout(LayoutKind.Sequential)]
  public struct RECT
  {
    public int left;
    public int top;
    public int right;
    public int bottom;
    public Rectangle ToRectangle() { return Rectangle.FromLTRB(left, top, right, bottom); }
  }

}
