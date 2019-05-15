using AutoCoder.Ext.System;
using AutoCoder.Telnet.Common;
using AutoCoder.Telnet.Common.RowCol;
using AutoCoder.Telnet.Common.ScreenDm;
using AutoCoder.Telnet.Enums;
using AutoCoder.Telnet.Enums.IBM5250;
using AutoCoder.Telnet.IBM5250.WorkstationCommands;
using AutoCoder.Telnet.IBM5250.WtdOrders;
using System;
using System.Linq;
using TextCanvasLib.Canvas;
using TextCanvasLib.Visual;
using TextCanvasLib.xml;

namespace TextCanvasLib.Telnet
{
  public static class WriteToDisplayCommandExt
  {
    /// <summary>
    /// create list of ShowLiteralItem and ShowFieldItem from the list of orders of
    /// a WriteToDisplay command.
    /// </summary>
    /// <param name="WtdCommand"></param>
    /// <param name="LogList"></param>
    /// <returns></returns>
    public static Tuple<ShowItemList, OneRowCol> BuildShowItemList(
      this WriteToDisplayCommand WtdCommand, ScreenDim ScreenDim,
      TelnetLogList LogList)
    {
      var itemList = new ShowItemList();
      IRowCol curRowCol = new ZeroRowCol(0, 0);
      OneRowCol caret = null;

      foreach (var order in WtdCommand.OrderList)
      {
        bool newGroup = false;
        if ((order.OrderCode == WtdOrder.SetBufferAddress)
          || (order.OrderCode == WtdOrder.StartHeader))
        {
          newGroup = true;
        }
        LogList.AddItem(Direction.Read, order.ToString(), newGroup);
        LogList.AddItem(Direction.Read, order.ToHexString());

        if (order is TextDataOrder)
        {
          var tdo = order as TextDataOrder;
          var s1 = tdo.ShowText;
          if ((tdo.AttrByte != null) || (s1.Length > 0)
            || (tdo.TailAttrByte != null))
          {
            var litItem = new ShowLiteralItem(
              (ZeroRowCol)curRowCol, tdo.AttrByte, s1, tdo.TailAttrByte);
            litItem.tdo_Length = s1.Length;

            itemList.Add(litItem);
            curRowCol = curRowCol.Advance(litItem.ItemLength());
          }
        }

        else if (order.OrderCode == WtdOrder.StartField)
        {
          var sfo = order as StartFieldOrder;
          var lx = sfo.LL_Length;
          var attrByte = sfo.AttrByte;

          var fldItem = new ShowFieldItem((ZeroRowCol)curRowCol, sfo.AttrByte,
            ShowUsage.Both, ShowDtyp.Char, lx);
          fldItem.IsMonocase = sfo.IsMonocase;
          fldItem.sfo_FCW = sfo.FCW_Bytes;
          fldItem.sfo_FFW = sfo.FFW_Bytes;
          fldItem.sfo_Length = sfo.LL_Length;
          fldItem.IsNonDisplay = sfo.IsNonDisplay;

          // field is non display.
  //        if ((attrByte & 0x07) == 0x07)
  //        {
  //          fldItem.IsNonDisplay = true;
  //        }

          itemList.Add(fldItem);
          curRowCol = curRowCol.Advance(1);  // advance because of attrbyte.
        }

        else if (order.OrderCode == WtdOrder.SetBufferAddress)
        {
          var sba = order as SetBufferAddressOrder;
          curRowCol = sba.GetRowCol(ScreenDim).ToZeroRowCol();
        }

        else if (order.OrderCode == WtdOrder.InsertCursor)
        {
          var ico = order as InsertCursorOrder;
          caret = ico.RowCol;
        }

        else if (order.OrderCode == WtdOrder.RepeatToAddress)
        {
          var rao = order as RepeatToAddressOrder;
          var s1 = rao.RepeatShowText((ZeroRowCol)curRowCol);

          var litItem = new ShowLiteralItem((ZeroRowCol)curRowCol, s1);
          litItem.rao_RepeatTextByte = rao.RepeatTextByte;
          litItem.rao_ToRowCol = rao.RowCol;

          itemList.Add(litItem);
          curRowCol = curRowCol.Advance(s1.Length);
        }

        else if (order.OrderCode == WtdOrder.EraseToAddress)
        {
          var eao = order as EraseToAddressOrder;
          var lx = eao.EraseLength((ZeroRowCol)curRowCol);
          var s1 = (" ").Repeat(lx);
          var litItem = new ShowLiteralItem((ZeroRowCol)curRowCol, s1);
          litItem.rao_RepeatTextByte = 0x00;
          litItem.rao_ToRowCol = eao.RowCol;

          itemList.Add(litItem);
          curRowCol = curRowCol.Advance(s1.Length);
        }
      }
      return new Tuple<ShowItemList, OneRowCol>(itemList, caret);
    }

    public static Tuple<bool, OneRowCol> PaintCanvas(
      this WriteToDisplayCommand WTD_command, ItemCanvas ItemCanvas,
      bool EraseScreen = true, TelnetLogList LogList = null 
      )
    {
      OneRowCol caret = null;
      bool drawDone = false;

      caret = ItemCanvas.PaintScreen(WTD_command, EraseScreen, LogList);
      drawDone = true;

      return new Tuple<bool, OneRowCol>(drawDone, caret);
    }
  }
}
