using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using TextCanvasLib.Common;

namespace TextCanvasLib.Ext
{
  public static class TextBlockExt_TextCanvasLib
  {
    public static void SetFontDefn(this TextBlock TextBlock, FontDefn FontDefn)
    {
      TextBlock.FontFamily = FontDefn.Family;
      TextBlock.FontSize = FontDefn.PointSize;
      TextBlock.FontWeight = FontDefn.Weight;
      TextBlock.Foreground = FontDefn.Foreground;
    }
  }
}
