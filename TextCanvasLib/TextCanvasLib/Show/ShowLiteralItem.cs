using AutoCoder.Telnet.Common;
using AutoCoder.Telnet.Common.RowCol;
using AutoCoder.Telnet.Enums;
using TextCanvasLib.Visual;

namespace TextCanvasLib.xml
{
  public class ShowLiteralItem : ShowItemBase, IVisualItem
  {
    public int tdo_Length
    { get; set; }

    public OneRowCol rao_ToRowCol
    { get; set; }
    public byte? rao_RepeatTextByte
    { get; set;  }

    public bool IsField
    {
      get { return false; }
    }
    public bool IsInputItem
    {
      get
      {
        return false;
      }
    }
    public bool IsTabToItem
    {
      get { return false; }
    }


    public int ShowLength
    {
      get { return Value.Length; }
    }

    public string ShowText
    {
      get { return Value; }
      set { var s1 = value; }
    }
    public ShowLiteralItem()
      : base()
    {
      this.ItemType = ShowItemType.Literal;
    }

    public ShowLiteralItem(ZeroRowCol RowCol, string Value)
      : base(RowCol, ShowItemType.Literal, Value )
    {
    }
    public ShowLiteralItem(
      ZeroRowCol RowCol, byte? AttrByte, string Value, byte? TailAttrByte = null)
      : base(RowCol, ShowItemType.Literal, AttrByte, Value)
    {
      this.TailAttrByte = TailAttrByte;
    }
  }
}
