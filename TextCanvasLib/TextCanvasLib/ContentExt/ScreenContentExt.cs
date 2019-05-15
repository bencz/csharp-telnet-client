using AutoCoder.Ext.System;
using AutoCoder.Telnet;
using AutoCoder.Telnet.Common;
using AutoCoder.Telnet.Common.RowCol;
using AutoCoder.Telnet.Common.ScreenDm;
using AutoCoder.Telnet.Common.ScreenLoc;
using AutoCoder.Telnet.Enums.IBM5250;
using AutoCoder.Telnet.IBM5250.Content;
using AutoCoder.Telnet.IBM5250.WorkstationCommands;
using AutoCoder.Telnet.IBM5250.WtdOrders;
using AutoCoder.Telnet.IBM5250.WtdOrders.StructuredFields;
using AutoCoder.Telnet.LogFiles;
using AutoCoder.Telnet.ThreadMessages;
using AutoCoder.Telnet.Threads;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls.Primitives;
using TextCanvasLib.Canvas;
using TextCanvasLib.Telnet;
using TextCanvasLib.ThreadMessages;
using TextCanvasLib.Threads;
using TextCanvasLib.Visual;
using TextCanvasLib.Windows;

namespace TextCanvasLib.ContentExt
{
  public static class ScreenContentExt
  {
    static Tuple<ScreenContent, ScreenContent> ProcessClearUnit(
      this ScreenContent InBaseMaster, ScreenDim ScreenDim, 
      PaintThread PaintThread)
    {
      var baseMaster = InBaseMaster;

      baseMaster = new ScreenContent(
        null, ScreenDim, baseMaster.ContentOdom);
      var master = baseMaster.GetWorkingContentBlock();

      // post ClearUnit message to paint thread.
      {
        var cu = new ClearUnitMessage(master.ContentNum);
        PaintThread.PostInputMessage(cu);
      }

      // mark the screenContent as having ClearUnit applied on it. When the SCB
      // is sent to the PaintThread code there will clear the TelnetCanvas and
      // dispose of any window item canvases.
      // todo: Send ClearUnitMessage to PaintThread. Get rid of DoClearUnit flag.
      baseMaster.DoClearUnit = true;

      return new Tuple<ScreenContent, ScreenContent>(baseMaster, master);
    }

    /// <summary>
    /// apply the commands of the workstation command list to the screen content
    /// block.
    /// </summary>
    /// <param name="applyMaster"></param>
    /// <param name="CmdList"></param>
    /// <param name="ToThread"></param>
    /// <param name="LogList"></param>
    /// <returns></returns>
    public static Tuple<bool, ScreenContent> Apply(
      this ScreenContent InBaseMaster,
      WorkstationCommandList CmdList, 
      ToThread ToThread, PaintThread PaintThread, TelnetLogList LogList)
    {
      bool wtdApplied = false;
      var baseMaster = InBaseMaster;
      var master = baseMaster.GetWorkingContentBlock();

      foreach (var cmdBase in CmdList)
      {
        if (cmdBase is ClearUnitCommand)
        {
          var rv = ProcessClearUnit(baseMaster, baseMaster.ScreenDim, PaintThread);
          baseMaster = rv.Item1;
          master = rv.Item2;
        }

        // same as ClearUnit. Only signals that a wide screen to be used.
        else if (cmdBase is ClearUnitAlternateCommand)
        {
          var cua = cmdBase as ClearUnitAlternateCommand;

          // screen size.
          ScreenDim screenDim;
          if (cua.RequestByte == 0x00)
            screenDim = new ScreenDim(27, 132);
          else
            screenDim = new ScreenDim(24, 80);

          var rv = ProcessClearUnit(baseMaster, screenDim, PaintThread);
          baseMaster = rv.Item1;
          master = rv.Item2;
        }

        // apply the orders of the WriteToDisplay command.
        else if (cmdBase is WriteToDisplayCommand)
        {
          var curMaster = master.Apply(cmdBase as WriteToDisplayCommand);
          master = curMaster;
          wtdApplied = true;
        }

        else if (cmdBase is ReadMdtFieldsCommand)
        {
          master.HowRead = HowReadScreen.ReadMdt;
        }

        // save screen command. Build response, send back to server.
        else if (cmdBase is SaveScreenCommand)
        {
          var msg = new SaveScreenMessage(master.Copy());
          ToThread.PostInputMessage(msg);
        }

        // read screen command. Build response, send back to server.
        else if (cmdBase is ReadScreenCommand)
        {
          var msg = new ReadScreenMessage(master.Copy());
          ToThread.PostInputMessage(msg);
        }

        else if (cmdBase is WriteStructuredFieldCommand)
        {
          var wsfCmd = cmdBase as WriteStructuredFieldCommand;
          if (wsfCmd.RequestCode == WSF_RequestCode.Query5250)
          {
            var msg = new Query5250ResponseMessage();
            ToThread.PostInputMessage(msg);
          }
        }
        else if (cmdBase is WriteSingleStructuredFieldCommand)
        {
        }
      }
      return new Tuple<bool, ScreenContent>(wtdApplied, baseMaster);
    }

    /// <summary>
    /// apply the orders of the WriteToDisplay command to the screen content
    /// block.
    /// </summary>
    /// <param name="ApplyMaster"></param>
    /// <param name="wtdCmd"></param>
    /// <returns></returns>
    public static ScreenContent Apply(
      this ScreenContent ApplyMaster, WriteToDisplayCommand wtdCmd)
    {
      IRowCol curRowCol = new ZeroRowCol(0, 0, ApplyMaster);
      var master = ApplyMaster;

      foreach (var order in wtdCmd.OrderList)
      {
        if (order is SetBufferAddressOrder)
        {
          var sba = order as SetBufferAddressOrder;
          curRowCol = sba.GetRowCol(master.ScreenDim).ToZeroRowCol().ToContentRelative(master);
        }

        // screen position is out of bounds. Do not place text or field onto the
        // screen at an invalid position.
        else if (curRowCol.ColNum >= master.ScreenDim.Width)
        {
        }

        else if (order is StartFieldOrder)
        {
          var sfo = order as StartFieldOrder;
          var contentField = master.ApplyStartFieldOrder(curRowCol, sfo);
          curRowCol = curRowCol.Advance(1);
        }

        else if (order is RepeatToAddressOrder)
        {
          var rao = order as RepeatToAddressOrder;
          var lx = rao.RepeatLength((ZeroRowCol)curRowCol.ToParentRelative(master));
          var textBytes = rao.GetRepeatTextBytes(lx);
          master.ApplyTextBytes(curRowCol, textBytes);
          curRowCol = curRowCol.Advance(lx);
        }

        else if (order is TextDataOrder)
        {
          var tdo = order as TextDataOrder;
          master.ApplyTextBytes(curRowCol, tdo.RawBytes);
          curRowCol = tdo.Advance(curRowCol);
        }

        else if (order is InsertCursorOrder)
        {
          var ico = order as InsertCursorOrder;
          master.CaretRowCol = ico.RowCol;
        }

        else if ( order is EraseToAddressOrder)
        {
          var eao = order as EraseToAddressOrder;
          var lx = eao.EraseLength((ZeroRowCol)curRowCol.ToParentRelative(master));
          var textBytes = ((byte)0x00).Repeat(lx);
          master.ApplyTextBytes(curRowCol, textBytes);
          curRowCol = curRowCol.Advance(lx);
        }

        // WriteStructuredField order. used to create a window ScreenContent
        // within the main screenContent.
        else if (order is CreateWindowStructuredField)
        {
          var sfo = order as CreateWindowStructuredField;

          // create the window as a ScreenContent block onto itself. All future
          // WTD orders are applied to the window screen content.
          var windowMaster = new WindowScreenContent(
            master,
            sfo.WindowDim, (ZeroRowCol)curRowCol);

          // store the index of this new child window as the index of the
          // "current window" SCB within the parent SCB.
          master.CurrentChildIndex = master.Children.Length - 1;

          // the window is now the current SCB.
          master = windowMaster;

          // reset the current rowCol to position 0,0 within the window.
          curRowCol = new ZeroRowCol(0, 0, master);
        }
      }

      return master;
    }

    public static ContentField ApplyStartFieldOrder(
      this ScreenContent master, IRowCol CurRowCol, StartFieldOrder StartField)
    {
      ContentField createField = null;
      var fieldRange = new RowColRange(CurRowCol, StartField.LL_Length + 1);
      var buf = master.GetContentBytes(CurRowCol, StartField.LL_Length + 1);

      // get the fields located within the bounds of the StartField. All of these
      // fields are removed from the ScreenTable.
      if (buf.AllMatch(0x00) == false)
      {
        foreach (var contentField in master.ContentFields(fieldRange))
        {
          master.FieldDict.Remove(contentField.RowCol);
          master.SetContentBytes(contentField.RowCol, 0x00);
        }
      }

      // add the field to the content table.
      {
        // store the attrByte at ItemRowCol location of the field.
        master.SetContentBytes(CurRowCol, StartField.AttrByte);

        // store field defn in FieldDict.
        createField =
          master.FieldDict.AddFieldOrder(CurRowCol as ZeroRowCol, StartField);
      }

      // assign unique fieldKey to the field.
      {
        createField.FieldKey = ContentFieldKey.AssignFieldKey(master, createField);
      }

      return createField;
    }

    /// <summary>
    /// find the ItemCanvas in which the ScreenContent is painted.
    /// </summary>
    /// <param name="ScreenContent"></param>
    /// <param name="TelnetCanvas"></param>
    /// <returns></returns>
    public static ItemCanvas FindItemCanvas(
      this ScreenContent ScreenContent, ItemCanvas TelnetCanvas)
    {
      ItemCanvas found = null;
      // screen content of the telnet ( main ) screen.
      if ( ScreenContent.Parent == null)
      {
        if (ScreenContent.ContentNum == TelnetCanvas.ContentNum)
          found = TelnetCanvas;
      }

      // a window screen content. Search the window ItemCanvases of the 
      // TelnetCanvas for the matching ContentNum.
      else
      {
        if ( TelnetCanvas.Children != null)
        {
          foreach( var child in TelnetCanvas.Children)
          {
            if (ScreenContent.ContentNum == child.ContentNum )
            {
              found = child;
              break;
            }
          }
        }
      }

      return found;
    }

    public static ItemCanvas CreateItemCanvas(this WindowScreenContent ScreenContent,
      ItemCanvas ParentItemCanvas, MasterThread MasterThread,
      PaintThread PaintThread)
    {
      // create the canvas to drawn upon.
        var rv = ScreenContent.CreateWindowedItemCanvas(
          ParentItemCanvas, ParentItemCanvas.CanvasDefn.FontDefn.PointSize,
          MasterThread, PaintThread, ScreenContent.ContentNum);
      var border = rv.Item1;
      var itemCanvas = rv.Item2;

        itemCanvas.ContentStart = ScreenContent.StartCharPoint;

        // add the canvas control ( actually the border control that contains the
        // canvas ) to the parent canvas.
        var canvasPos = ParentItemCanvas.AddItemToCanvas(
          ScreenContent.StartRowCol, itemCanvas.BorderControl);
        ScreenContent.WindowPos = canvasPos;

        ParentItemCanvas.RemoveCaret();
        itemCanvas.SetFocus();
        itemCanvas.HookBorderDrag();

      return itemCanvas;
    }

    /// <summary>
    /// paint the text and fields of the screen content block onto the item canvas.
    /// </summary>
    /// <param name="ScreenContent"></param>
    /// <param name="ItemCanvas"></param>
    /// <param name="Window"></param>
    public static void PaintScreenContent(this ScreenContent ScreenContent,
          ItemCanvas ItemCanvas)
    {
      // use ContentNum to match ScreenContent with the ItemCanvas.
      ItemCanvas.ContentNum = ScreenContent.ContentNum;

      // apply the clear unit command to the itemcanvas. 
      if (ScreenContent.DoClearUnit == true)
      {
        ItemCanvas.EraseScreen();
        ScreenContent.DoClearUnit = false;
      }

      else
      {
        // add ContentText items to the ContentDict of the SCB.
        ScreenContent.AddAllContentText();
      }

      // Remove all items on the ItemCanvas that are not in the SCB.
      // todo: need to clear the caret position if field removed from the screen
      //       contains the caret.
      var visualItems = ItemCanvas.VisualItems;
      foreach( var itemCursor in visualItems.ItemList())
      {
        var vi = itemCursor.GetVisualItem();
        var rowCol = vi.ItemRowCol;
        ContentItemBase contentItem = null;
        var rc = ScreenContent.FieldDict.TryGetValue(rowCol, out contentItem);
        if ( rc == false )
        {
          visualItems.RemoveItem(itemCursor, ItemCanvas);
        }
      }

      // loop for each item within the content array.
      foreach (var contentItem in ScreenContent.ContentItems())
      {
        if (contentItem is ContentText)
        {
          var contentText = contentItem as ContentText;
          var visualItem = ItemCanvas.VisualItemFactory(
            contentText.GetShowText(ScreenContent),
            contentText.RowCol, contentText.GetAttrByte(ScreenContent),
            contentText.GetTailAttrByte(ScreenContent));

          var iMore = visualItem as IVisualItemMore;
          var node = iMore.InsertIntoVisualItemsList(ItemCanvas.VisualItems);
          iMore.AddToCanvas(ItemCanvas);
          iMore.SetupUnderline();
        }

        else if (contentItem is ContentField)
        {
          var contentField = contentItem as ContentField;
          var visualItem = 
            ItemCanvas.VisualItemFactory(ScreenContent, contentField);

          if (visualItem != null)
          {
            var iMore = visualItem as IVisualItemMore;
            var node = iMore.InsertIntoVisualItemsList(ItemCanvas.VisualItems);
            iMore.AddToCanvas(ItemCanvas);
            iMore.SetupUnderline();
            iMore.SetupFieldItem(
              ScreenContent, contentField, 
              ItemCanvas.CanvasDefn.CharBoxDim, ItemCanvas.CanvasDefn.KernDim);
          }
        }
      }

      // position the caret.
      ItemCanvas.PositionCaret(ScreenContent.CaretRowCol);
      ItemCanvas.SetFocus();
    }

    public static IEnumerable<string> ReportVisualItems( 
      this ScreenContent ScreenContent, ItemCanvas TelnetCanvas)
    {
      var report = new List<string>();

      {
        var itemCanvas = ScreenContent.FindItemCanvas(TelnetCanvas);
        if (itemCanvas == null)
          throw new Exception("item canvas of screen content is not  found");

        var subReport = ReportVisualItems_Actual(
          ScreenContent, itemCanvas, "Telnet Canvas Visual Items");
        report.AddRange(subReport);
      }

      // report the visual items of each child screenContent. ( windows on the 
      // canvas. )
      if (ScreenContent.Children != null)
      {
        foreach (var childContent in ScreenContent.Children)
        {
          var itemCanvas = childContent.FindItemCanvas(TelnetCanvas);
          if (itemCanvas == null)
            throw new Exception("item canvas of screen content is not  found");

          var subReport = ReportVisualItems_Actual(
            childContent, itemCanvas, "Window Canvas Visual Items");
          report.AddRange(subReport);
        }
      }

      return report;
    }

    private static IEnumerable<string> ReportVisualItems_Actual( 
      ScreenContent ScreenContent, ItemCanvas ItemCanvas, string Title = null )
    {
      var report = new List<string>();

      if (Title != null)
        report.Add(Title);

      var visualItems = ItemCanvas.VisualItems;

      if (ScreenContent.Parent != null)
      {
        var contentStart = ScreenContent.StartCharPoint;
        report.Add("window start:" + contentStart.ToString());
      }

      {
        var subReport = wtdReportExt.PrintVisualItems(visualItems);
        report.AddRange(subReport);
      }

      return report;
    }
  }
}
