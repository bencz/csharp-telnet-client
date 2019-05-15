using AutoCoder.Ext.System;
using System.Collections.Generic;
using TextCanvasLib.Visual;
using AutoCoder.Telnet.IBM5250.WtdOrders.wtdCommon;
using AutoCoder.Telnet.Common;
using AutoCoder.Telnet.Common.RowCol;

namespace TextCanvasLib.Telnet
{
  public static class wtdReportExt
  {
    public static IEnumerable<string> PrintVisualItems(ScreenVisualItems VisualItems)
    {
      var lines = new List<string>();

      {
        var titleText = "----------- Canvas Visual Item Listing -----------";
        lines.Add(titleText.PadCenter(80));
        var chd = wtdReport.PrintColumnHeading();
        lines.AddRange(chd);
      }

      foreach( var cursor in VisualItems.CanvasItemList( ))
      {
        var iVisual = cursor.GetVisualItem();
        var visualItem = iVisual as VisualItem;

        var printItem = new PrintItem(iVisual.ItemRowCol.ToOneRowCol() as OneRowCol);
        if ( visualItem.IsInputItem == false )
        {
          printItem.ItemType = ReportItemType.Literal;
          printItem.ItemText = iVisual.ShowText;
          printItem.ItemLgth = iVisual.ShowText.Length;
        }
        else
        {
          printItem.ItemType = ReportItemType.Field;
          printItem.ItemText = iVisual.ShowText;
          printItem.ItemLgth = iVisual.ShowText.Length;
        }

        if (printItem.ItemLgth > 0)
        {
          printItem = wtdReport.PrintAndAdvance(lines, printItem);
        }
      }

      return lines;
    }

  }
}
