using AutoCoder.Ext.System;
using AutoCoder.Report;
using AutoCoder.Systm;
using AutoCoder.Telnet.Common;
using AutoCoder.Telnet.Common.RowCol;
using AutoCoder.Telnet.Common.ScreenDm;
using AutoCoder.Telnet.IBM5250.WorkstationCommands;
using AutoCoder.Telnet.IBM5250.WtdOrders.StructuredFields;
using System.Collections.Generic;

namespace AutoCoder.Telnet.IBM5250.WtdOrders.wtdCommon
{
  public static class wtdReport
  {
    public static IEnumerable<string> Print_WTD_Orders(this WriteToDisplayCommand wtdCmd)
    {
      var lines = new List<string>();
      var lb = new BlankFillLineBuilder();
      var item = new PrintItem();

      {
        var titleText = "5250 Write To Display Workstation Command Orders.";
        lines.Add(titleText.PadCenter(80));
        var chd = PrintColumnHeading();
        lines.AddRange(chd);
      }

      foreach (var order in wtdCmd.OrderList)
      {
        if (order is SetBufferAddressOrder)
        {
          if (item.ItemType != null )
            item = PrintAndAdvance(lines, item);

          var sba = order as SetBufferAddressOrder;
          item = new PrintItem(sba);
          {
            var item2 = new PrintItem(sba);
            item2.ItemType = ReportItemType.sba;
            PrintDetailLine(lines, item2);
          }
        }

        else if (order is StartFieldOrder)
        {
          item = CheckPrintItem(lines, item);
          var sof = order as StartFieldOrder;
          item.ItemType = ReportItemType.Field;
          item.ItemLgth = sof.LL_Length;
          item.AttrByte = sof.AttrByte;
          item.ffw = sof.FFW_Bytes.Concat(sof.FCW_Bytes);
        }

        else if (order is RepeatToAddressOrder)
        {
          var ra = order as RepeatToAddressOrder;
          item = CheckPrintItem(lines, item);
          item.ItemType = ReportItemType.RepeatToAddress;

          // the length covered by the repeat order. From the current screen location
          // to the row/col of the RA order.
          item.ItemLgth = item.RowCol.DistanceInclusive(ra.RowCol);

          item.ItemText = ra.RepeatPrintableChar;

          item = PrintAndAdvance(lines, item);
        }

        else if (order is EraseToAddressOrder)
        {
          var ea = order as EraseToAddressOrder;
          item = CheckPrintItem(lines, item);
          item.ItemType = ReportItemType.EraseToAddress;

          // the length covered by the repeat order. From the current screen location
          // to the row/col of the EA order.
          item.ItemLgth = item.RowCol.DistanceInclusive(ea.RowCol);

          item.ItemText = ((string)"00 ").Repeat(item.ItemLgth.Value);

          item = PrintAndAdvance(lines, item);
        }

        else if (order is TextDataOrder)
        {
          var td = order as TextDataOrder;
          {
            var s1 = td.PrintableText;
            if (s1 != null)
              item.ItemText = s1;
            if (item.AttrByte == null)
              item.AttrByte = td.AttrByte;

            if (item.ItemType == null)
            {
              if (td.DisplayText != null)
                item.ItemLgth = td.TextLength;
              item.ItemType = ReportItemType.Literal;
            }
          }
          item = PrintAndAdvance(lines, item);
        }
        else if (order is CreateWindowStructuredField)
        {
          var sfo = order as CreateWindowStructuredField;
          item = CheckPrintItem(lines, item);
          var sof = order as StartFieldOrder;
          item.ItemType = ReportItemType.CrtWdw;
          item.WindowDim = new ScreenDim(sfo.NumRow, sfo.NumCol);
          item = PrintAndAdvance(lines, item);
        }
      }

      if (item.HasPrintableContent())
      {
        PrintAndAdvance(lines, item);
      }
      return lines;
    }

    /// <summary>
    /// print the item if something to print.  Then advance the screen address and
    /// return a new item.
    /// </summary>
    /// <param name="lines"></param>
    /// <param name="item"></param>
    /// <returns></returns>
    private static PrintItem CheckPrintItem(List<string> lines, PrintItem item)
    {
      PrintItem nextItem = item;
      if (item.ItemType != null)
      {
        nextItem = PrintAndAdvance(lines, item);
      }
      return nextItem;
    }
    public static PrintItem PrintAndAdvance(List<string> lines, PrintItem Item)
    {
      PrintItem nextItem = Item;
      var nextRowCol = Item.RowCol;
      if (Item.EndRowCol != null)
        nextRowCol = Item.EndRowCol.Advance(1);

      var s1 = Item.ToDetailLine();
      lines.Add(s1);

      nextItem = new PrintItem((OneRowCol)nextRowCol);

      return nextItem;
    }
    private static void PrintDetailLine(List<string> lines, PrintItem Item)
    {
      var s1 = Item.ToDetailLine();
      lines.Add(s1);
    }
    public static string[] PrintColumnHeading()
    {
      string line1 = null;
      string line2 = null;

      {
        var lb = new BlankFillLineBuilder();
        lb.Put("-- from --", 0);
        lb.Put("-- End ---", 33);
        line1 = lb.ToString();
      }

      {
        var lb = new BlankFillLineBuilder();
        lb.Put("Row", 0);
        lb.Put("Col", 5);
        lb.Put("ItemType", 10);
        lb.Put("Lgth", 20);
        lb.Put("dspatr", 26);
        lb.Put("Row", 33);
        lb.Put("Col", 38);
        lb.Put("Text", 43);
        lb.Put("FFW and FCW", 70);
        line2 = lb.ToString();
      }

      return new string[] { line1, line2 };
    }

  }
}
