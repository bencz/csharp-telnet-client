using AutoCoder.Ext.System;
using AutoCoder.Telnet.Common;
using AutoCoder.Telnet.Common.RowCol;
using AutoCoder.Telnet.Enums;
using System.Text;

namespace TextCanvasLib.xml
{
  /// <summary>
  /// ShowItemBase objects represent the wtd orders from the telnet data stream. Or 
  /// the individual draw instructions from xml instruction stream. ( the TextBlocks
  /// drawn on the canvas are represented by the VisualItem class. )
  /// </summary>
  public abstract class ShowItemBase
  {
    public ZeroRowCol AdjustShowRowCol
    { get; set; }

    private bool _IsOnCanvas = false;
    public bool IsOnCanvas
    {
      get { return _IsOnCanvas; }
      set { _IsOnCanvas = value; }
    }
    public ShowItemType ItemType { get; set; }
    public ZeroRowCol ItemRowCol { get; set; }
    public string Value { get; set; }

    public byte? AttrByte { get; set; }
    public bool? AttrByteOccupySpace { get; set; }
    public byte? TailAttrByte { get; set; }

    /// <summary>
    /// error message when parsing the show item.
    /// </summary>
    public string Errmsg { get; set; }
    public bool IsNonDisplay
    { get; set; }

    protected ShowItemBase( )
    {
      this.IsNonDisplay = false;
    }
    protected ShowItemBase( ZeroRowCol RowCol, ShowItemType ItemType, string Value = null )
      : this( )
    {
      this.ItemRowCol = RowCol;
      this.ItemType = ItemType;
      this.Value = Value;
    }
    protected ShowItemBase(ZeroRowCol RowCol, ShowItemType ItemType, 
      byte? AttrByte, string Value = null)
      : this()
    {
      this.ItemRowCol = RowCol;
      this.ItemType = ItemType;
      this.AttrByte = AttrByte;
      this.Value = Value;
    }
    public override string ToString()
    {
      var sb = new StringBuilder();

      sb.Append("ItemType:" + this.ItemType.ToString() + " " + this.ItemRowCol.ToString()) ;
      if (this.IsNonDisplay == true)
        sb.Append(" NonDisplay");
      if ( this.Value.IsNullOrEmpty( ) == false )
        sb.Append(" Value:" + this.Value);
      return sb.ToString();
    }
  }
}

