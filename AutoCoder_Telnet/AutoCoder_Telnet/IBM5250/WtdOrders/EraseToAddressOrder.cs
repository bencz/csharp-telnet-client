using AutoCoder.Ext.System;
using AutoCoder.Systm;
using AutoCoder.Telnet.Common.RowCol;
using AutoCoder.Telnet.Enums.IBM5250;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoCoder.Telnet.IBM5250.WtdOrders
{
  public class EraseToAddressOrder : WtdOrderBase
  {
    public byte RowAddress
    { get; set; }

    public byte ColumnAddress
    { get; set; }

    /// <summary>
    /// length byte that follows row and column address in the byte stream. Will
    /// contain the number of following attr type codes. Will be set to x'FF'
    /// when AttrType is set to X'FF'.
    /// </summary>
    public byte AttrTypesLength
    { get; set; }

    public byte[] AttrTypesArray
    { get; set; }

    public OneRowCol RowCol
    {
      get
      {
        return new OneRowCol(this.RowAddress, this.ColumnAddress);
      }
    }

    public EraseToAddressOrder(InputByteArray InputArray)
      : base(InputArray, WtdOrder.EraseToAddress)
    {
      if (InputArray.RemainingLength < 5)
      {
        this.Errmsg = "erase address order. end of stream.";
      }

      if (this.Errmsg == null)
      {
        var buf = InputArray.PeekBytes(4);
        this.RowAddress = buf[1];
        this.ColumnAddress = buf[2];
        this.AttrTypesLength = buf[3];

        this.BytesLength += 3;
        InputArray.AdvanceIndex(4);
      }

      // attr types length between 2 and 5.
      if ((this.Errmsg == null) 
        && (( this.AttrTypesLength < 2) || (this.AttrTypesLength > 5)))
      {
        this.Errmsg = "attribute types length not between 2 and 5.";
      }

      // isolate the list of attr type bytes.
      if ( this.Errmsg == null)
      {
        // the count of attrType bytes.
        int lx = 0;
        if (this.AttrTypesLength == 0xff)
          lx = 1;
        else
          lx = this.AttrTypesLength - 1;

        if (InputArray.RemainingLength < lx)
          this.Errmsg = "attribute types length not valid";
        else
        {
          this.AttrTypesArray = InputArray.GetBytes(lx);
        }
      }
    }

    /// <summary>
    /// build a byte array containing an EraseToAddress order.
    /// </summary>
    /// <param name="RowNum"></param>
    /// <param name="ColNum"></param>
    /// <returns></returns>
    public static byte[] Build(OneRowCol RowCol, byte[] AttrTypesArray)
    {
      var ba = new ByteArrayBuilder();
      ba.Append((byte)WtdOrder.EraseToAddress);
      ba.Append(RowCol.RowNum);
      ba.Append(RowCol.ColNum);

      if ((AttrTypesArray.Length == 1 ) && ( AttrTypesArray[0] == 0xff))
      {
        ba.Append(new byte[] { 0xff, 0xff });
      }
      else
      {
        ba.Append((byte)AttrTypesArray.Length);
        ba.Append(AttrTypesArray);
      }

      return ba.ToByteArray();
    }

    /// <summary>
    /// calc the length of the erase order. Calc as the difference between input 
    /// from pos and erase to location.
    /// </summary>
    /// <param name="FromRowCol"></param>
    /// <returns></returns>
    public int EraseLength(IRowCol FromRowCol)
    {
      var fromRowCol = FromRowCol.ToOneRowCol();
      var toRowCol = this.RowCol.ToOneRowCol();
      int eraseLength = fromRowCol.DistanceInclusive(toRowCol);
      return eraseLength;
    }


    public override string ToString()
    {
      var s1 = OrderCode.Value.ToString() + " order. Row:" + this.RowAddress.ToString() +
        " Column:" + this.ColumnAddress.ToString() + " AttrTypes:" + 
        this.AttrTypesArray.ToHex(' ');
      return s1;
    }

    /// <summary>
    /// return array of text lines which list the contents of the EraseToAddress
    /// order.
    /// </summary>
    /// <returns></returns>
    public override string[] ToReportLines()
    {
      List<string> lines = new List<string>();
      lines.Add("Erase to address Order");
      lines.Add(this.ToHexString());
      lines.Add(this.ToString());

      return lines.ToArray();
    }
  }
}
