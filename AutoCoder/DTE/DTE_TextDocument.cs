using System;
using System.Collections.Generic;
using System.Text;

namespace AutoCoder.DTE
{
  public class DTE_TextDocument
  {
    DTE_Main mMain;
    EnvDTE.TextDocument mTextDocument;

    public DTE_TextDocument(DTE_Main InMain, EnvDTE.TextDocument InTextDocument)
    {
      mMain = InMain;
      mTextDocument = InTextDocument;
    }

    public EnvDTE.EditPoint StartPoint
    {
      get { return mTextDocument.StartPoint.CreateEditPoint( ) ; }
    }

    public EnvDTE.TextDocument TextDocument
    {
      get { return mTextDocument; }
    }
  }
}
