using AutoCoder.Ext.System;
using AutoCoder.Telnet.CodedBytes;
using AutoCoder.Telnet.Common;
using AutoCoder.Telnet.Common.RowCol;
using AutoCoder.Telnet.Enums;
using AutoCoder.Telnet.Enums.IBM5250;
using AutoCoder.Telnet.IBM5250.WtdOrders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoCoder.Telnet.IBM5250.Content
{
  public class ContentField : ContentItemBase
  {
    public int LL_Length
    { get; set; }

    public byte[] FCW_Bytes
    { get; set; }
    public byte[] FFW_Bytes
    { get; set; }

    /// <summary>
    /// unique field num of field within the ScreenContent block. Each field in the
    /// content block is assigned a unique number. But if field is a continued
    /// segment field all segments share the same FieldNum.
    /// </summary>
    public ContentFieldKey FieldKey
    { get; set;  }
    public override int FieldNum
    {
      get
      {
        return this.FieldKey.FieldNum;
      }
    }

    public StartFieldOrder FieldOrder
    { get; set; }
    public bool IsBypass
    {
      get
      {
        return FieldFormatWord.IsBypass(this.FFW_Bytes);
      }
    }

    /// <summary>
    /// translate to uppercase. bin 10 of FFW is set to '1'.
    /// </summary>
    public bool IsMonocase
    {
      get
      {
        return FieldFormatWord.IsMonocase(FFW_Bytes);
      }
    }

    public bool ModifiedFlag
    { get; set; }
    public override int GetItemLength(ScreenContent ScreenContent)
    {
      return this.LL_Length;
    }

    public virtual bool GetModifiedFlag( ScreenContent ScreenContent )
    {
      return this.ModifiedFlag;
    }

    public override byte[] GetTextBytes(ScreenContent ScreenContent)
    {
      var ix = ScreenContent.ToContentIndex(this.RowCol, 1);
      return ScreenContent.ContentArray.SubArray(ix, this.LL_Length);
    }

    public byte? GetTailAttrByte(ScreenContent ScreenContent)
    {
        var ix = ScreenContent.ToContentIndex(this.RowCol, this.LL_Length + 1);
        var b1 = ScreenContent.ContentArray[ix];
        if (b1.IsAttrByte() == true)
          return b1;
        else
          return null;
      }

    /// <summary>
    /// return the index into the content array of the field start row/col.
    /// </summary>
    /// <param name="ScreenContent"></param>
    /// <returns></returns>
    public int GetContentIndex( ScreenContent ScreenContent)
    {
      var ix = ScreenContent.ToContentIndex(this.RowCol);
      return ix;
    }

    public void SetTailAttrByte(ScreenContent ScreenContent, byte? Value)
    {
        var ix = ScreenContent.ToContentIndex(this.RowCol, this.LL_Length + 1);
        if (Value == null)
          ScreenContent.ContentArray[ix] = 0x00;
        else
          ScreenContent.ContentArray[ix] = Value.Value;
      }

    public ContentField( ZeroRowCol RowCol, StartFieldOrder FieldOrder)
    {
      this.RowCol = RowCol;
      this.FieldOrder = FieldOrder;
      this.LL_Length = FieldOrder.LL_Length;
      this.FFW_Bytes = FieldOrder.FFW_Bytes;
      this.FCW_Bytes = FieldOrder.FCW_Bytes;
    }
    public bool GetIsNonDisplay(ScreenContent ScreenContent)
    {
      var attrByte = this.GetAttrByte(ScreenContent);
      if ((attrByte != null) && ((attrByte.Value & 0x07) == 0x07))
      {
        return true;
      }
      else
        return false;
    }

    public void LoadContents( ScreenContent Content)
    {
      var ix = Content.ToContentIndex(this.RowCol);
      this.AttrByte = Content.ContentArray[ix];
    }

    public override string ToString()
    {
      string s1 = "ContentField. " + this.RowCol.ToString() +
        " LL:" + this.LL_Length;
      return s1;
    }

    /// <summary>
    /// does this field allow input or not. Look to the FFW to see if the 
    /// "bypass" bit is set on or not.
    /// </summary>
    public ShowUsage Usage
    {
      get
      {
        if (this.IsBypass == true)
          return ShowUsage.Output;
        else
          return ShowUsage.Both;
      }
    }
  }
}
