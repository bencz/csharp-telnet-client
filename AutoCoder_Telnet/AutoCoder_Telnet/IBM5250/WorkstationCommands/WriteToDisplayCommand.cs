using AutoCoder.Core.Enums;
using AutoCoder.Ext.System;
using AutoCoder.Report;
using AutoCoder.Systm;
using AutoCoder.Telnet.Common;
using AutoCoder.Telnet.Common.RowCol;
using AutoCoder.Telnet.Enums.IBM5250;
using AutoCoder.Telnet.IBM5250.Common;
using AutoCoder.Telnet.IBM5250.WtdOrders;
using AutoCoder.Telnet.IBM5250.WtdOrders.wtdCommon;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AutoCoder.Telnet.IBM5250.WorkstationCommands
{
  public class WriteToDisplayCommand : WorkstationCommandBase, IDataStreamReport
  {
    public byte[] ControlChars
    { get; set; }

    public List<WtdOrderBase> OrderList
    { get; set; }

    public WriteToDisplayCommand(byte ControlChar1 = 0, byte ControlChar2 = 0)
      : base(WorkstationCode.WTD)
    {
      this.ControlChars = new byte[] { ControlChar1, ControlChar2 };
      this.OrderList = new List<WtdOrderBase>();
    }

    public WriteToDisplayCommand(InputByteArray InputArray)
      : base(InputArray, WorkstationCode.WTD)
    {
      this.OrderList = new List<WtdOrderBase>();

      if (InputArray.RemainingLength < 4)
        this.Errmsg = "Byte stream too short. Missing control chars.";

      if (this.Errmsg == null)
      {
        var buf = InputArray.PeekBytes(4);
        this.ControlChars = buf.SubArray(2, 2);

        InputArray.AdvanceIndex(4);
        this.BytesLength = 4;

        // gather WTD orders and display characters.
        while (true)
        {
          WtdOrderBase orderBase = null;
          if (InputArray.RemainingLength == 0)
            break;

          orderBase = WtdOrderBase.Factory(InputArray);

          // not an explicit WTD order.  Check that is a text data order.
          if (orderBase == null)
          {
            var b1 = InputArray.PeekByte(0);
            if (Common5250.IsTextDataChar(b1) == true)
            {
              orderBase = new TextDataOrder(InputArray);
            }
          }

          // current input stream bytes are not WTD order. End of WTD command.
          if (orderBase == null)
            break;

          // got an order but some sort of form error.
          if (orderBase.Errmsg != null)
          {
            throw new Exception("invalid WTD order");
          }

          // Append to list of orders of the WTD command.
          this.OrderList.Add(orderBase);
          this.BytesLength += orderBase.GetDataStreamLength();
        }
      }
    }

    public override int GetDataStreamLength()
    {
      // sum up the length of each of the orders.
      int orderLx = 0;
      foreach (var order in this.OrderList)
      {
        orderLx += order.GetDataStreamLength();
      }

      // length of wtd command is the first 4 bytes + length of all the orders.
      return 4 + orderLx;
    }
    public override byte[] ToBytes()
    {
      var ba = new ByteArrayBuilder();
      {
        var buf = base.ToBytes();
        ba.Append(buf);
      }
      ba.Append(this.ControlChars);
      foreach (var order in this.OrderList)
      {
        var buf = order.ToBytes();
        ba.Append(buf);
      }
      return ba.ToByteArray();
    }

    public static byte[] Build(byte ControlChar1, byte ControlChar2, byte[] OrderBytes )
    {
      var ba = new ByteArrayBuilder();
      {
        var buf = WorkstationCommandBase.ToBytes(WorkstationCode.WTD);
        ba.Append(buf);
      }
      ba.Append(ControlChar1);
      ba.Append(ControlChar2);
      ba.Append(OrderBytes);

      return ba.ToByteArray();
    }

    public string ReportTitle
    {
      get
      {
        var titleText = "5250 Write To Display Workstation Command Orders.";
        return titleText;
      }
    }

    public IEnumerable<string> DefnToColumnReport( )
    {
      var report = new ColumnReport();
      report.AddColDefn("Control Char1", 0, WhichSide.Left);
      report.AddColDefn("Control Char2");
      report.AddColDefn("Number orders");

      report.WriteColumnHeading();
      var valueList = new string[]
      {
        this.ControlChars[0].ToHex( ),
        this.ControlChars[1].ToHex( ),
        this.OrderList.Count.ToString( )
      };
      report.WriteDetail(valueList);
      return report;
    }

    public IEnumerable<string> ToColumnReport( string Title = null )
    {
      var report = new ReportList();

      // WTD command defn to column report form.
      report.WriteTextLines(this.DefnToColumnReport());
      report.WriteGapLine();

      var lines = wtdReport.Print_WTD_Orders(this);
      report.WriteTextLines(lines);
      return report;
    }

    public IEnumerable<string> ToOrderDetailReport( )
    {
      var lines = new List<string>();

      var headerLine = "Write to display orders. " + this.OrderList.Count() + " orders.";
      lines.Add(headerLine);

      IRowCol rowCol = new ZeroRowCol(0, 0);
      var nextRowCol = rowCol;
      foreach ( var order in this.OrderList)
      {
        rowCol = nextRowCol;
        lines.Add( order.ToString( rowCol.ToOneRowCol( )));
        nextRowCol = order.Advance(rowCol);
      }
      return lines;
    }

    public override string ToString()
    {
      var s1 = base.ToString();
      return s1 + " ControlChar byte 0:" + this.ControlChars[0].ToHex() +
        " Control char byte 1:" + this.ControlChars[1].ToHex() +
        " Number of orders:" + this.OrderList.Count.ToString();
    }
  }

  public static class WriteToDisplayCommandExt
  {
  }
}

