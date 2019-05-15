using AutoCoder.Ext.System;
using AutoCoder.Report;
using AutoCoder.Systm;
using AutoCoder.Telnet.Common;
using AutoCoder.Telnet.Common.RowCol;
using AutoCoder.Telnet.Common.ScreenDm;
using AutoCoder.Telnet.Enums;

namespace AutoCoder.Telnet.IBM5250.WtdOrders.wtdCommon
{
  public class PrintItem
  {
    bool StartedBySetBufferAddressOrder { get; set; }

    public IRowCol RowCol
    {
      get; set;
    }

    public ReportItemType? ItemType { get; set; }
    public int? ItemLgth { get; set; }
    public byte? AttrByte { get; set; }
    public string ItemText { get; set; }
    public byte[] ffw { get; set; }

    public IRowCol EndRowCol
    {
      get
      {
        IRowCol endRowCol = null;
        if ((ItemType != null) && (ItemType.Value == ReportItemType.CrtWdw))
        {
          var rowcol = new OneRowCol(this.WindowDim.Height, this.WindowDim.Width);
          endRowCol = rowcol;
        }
        else
        {
          int lgth = this.ItemLgth.GetValueOrDefault();
          if (this.AttrByte != null)
            lgth += 1;

          // calc the end position row/col.
          if (lgth > 0)
          {
            int advLgth = lgth - 1;
            endRowCol = this.RowCol.Advance(advLgth);
          }
        }
        return endRowCol;
      }
    }

    public ScreenDim WindowDim
    { get; set; }

    public PrintItem()
      : this(new OneRowCol(0,0))
    {
    }

    public PrintItem(OneRowCol RowCol)
    {
      this.RowCol = RowCol;
      this.ItemType = null;
      this.ItemLgth = 0;
      this.AttrByte = null;
      this.ItemText = null;
      this.ffw = null;
    }

    public PrintItem(SetBufferAddressOrder sba)
      : this(new OneRowCol(sba.RowNum, sba.ColNum))
    {
      this.StartedBySetBufferAddressOrder = true;
    }

    public string ToDetailLine()
    {
      var lb = new BlankFillLineBuilder();
      lb.Put(this.RowCol.RowNum.ToString().RightJustify(3), 0);
      lb.Put(this.RowCol.ColNum.ToString().RightJustify(3), 5);
      lb.Put(this.ItemType.ToString(), 10, 9);

      lb.Put(this.ItemLgth.NullEmptyString().RightJustify(4), 20);

      var daArray = this.AttrByte.ToColorDsplyAttr();
      lb.Put(daArray.ToText( ), 26);

      if (this.EndRowCol != null)
      {
        lb.Put(this.EndRowCol.RowNum.ToString().RightJustify(3), 33);
        lb.Put(this.EndRowCol.ColNum.ToString().RightJustify(3), 38);
      }

      lb.Put(this.ItemText.EmptyIfNull(), 43);
      if ( this.ffw != null)
      {
        lb.Put(this.ffw.ToHex(' '), 70);
      }
      return lb.ToString();
    }

    public bool HasPrintableContent()
    {
      if (this.StartedBySetBufferAddressOrder == true)
        return true;
      else if (this.ItemType != null)
        return true;
      else
        return false;
    }

  }
}
