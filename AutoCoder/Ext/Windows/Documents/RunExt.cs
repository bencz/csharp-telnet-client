using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Documents;

namespace AutoCoder.Ext.Windows.Documents
{
  public static class RunExt
  {
    public static TextPointer GetPositionAtOffset(this Run Run, int Offset)
    {
      var tp1 = Run.ContentStart;
      var tp2 = tp1.GetPositionAtOffset(Offset);
      return tp2;
    }
  }
}
