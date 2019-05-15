using System;
using System.Collections.Generic;
using System.Text;
using EnvDTE;
using EnvDTE80;
using Extensibility;
using Microsoft.VisualStudio.CommandBars;

namespace AutoCoder.DTE
{
  public class DTE_TextWindow
  {
    DTE_Main mMain;
    TextWindow mTextWindow;

    public DTE_TextWindow(DTE_Main InMain, TextWindow InTextWindow)
    {
      mMain = InMain;
      mTextWindow = InTextWindow;
    }

    public TextSelection Selection
    {
      get { return mTextWindow.Selection; }
    }

    public TextWindow TextWindow
    {
      get { return mTextWindow; }
    }
  }
}
