using TextCanvasLib.xml;
using AutoCoder.Telnet.IBM5250.WtdOrders;
using AutoCoder.Telnet.Common;
using AutoCoder.Telnet.Enums;
using AutoCoder.Telnet.Common.RowCol;
using System.Windows;
using AutoCoder.Telnet.IBM5250.Content;

namespace TextCanvasLib.Visual
{
  /// <summary>
  /// the base class of controls, like VisualTextBlock, which are placed on the
  /// Canvas.
  /// VisualTextBlock, VisualSpanner, VisualTextBlockSegment
  /// </summary>
  public abstract class VisualItem
  {
    /// <summary>
    /// when calc ShowRowCol, add this adjustment value to the end result.
    /// Used when adjusting rowcol pos of canvas elements in a window canvas.
    /// The SBA order sets the position as absolute value on the telnet screen
    /// canvas. Need to adjust to location within the window canvas.
    /// </summary>
    public virtual ZeroRowCol AdjustShowRowCol
    { get; set; }

    /// <summary>
    /// pos of this item when placed on the canvas. See AddToCanvas method in the
    /// VisualTextBlock class.
    /// </summary>
    public Point CanvasPos
    { get; set; }

    /// <summary>
    /// the show item this visual item was created from.
    /// </summary>
    public virtual ShowItemBase CreateFromItem { get; set; }

    public WtdOrderBase FromOrder { get; set; }
    public ContentItemBase FromContentItem { get; set; }

    /// <summary>
    /// the field item the visual item was created from. ( if is an input field. )
    /// </summary>
    public virtual ShowFieldItem ShowItem
    {
      get;
      set;
    }
    public ShowUsage Usage
    { get; set; }

    public byte? AttrByte
    { get; set; }

    /// <summary>
    /// see text in IVisualItem interface.
    /// </summary>
    public bool? AttrByteOccupySpace
    { get; set; }

    public byte? TailAttrByte
    { get; set; }

    public int EndColNum
    {
      get
      {
        return (this as IVisualItem).ShowEndRowCol().ColNum;
      }
    }

    public int ImmedAfterColNum
    {
      get
      {
        return EndColNum + 1;
      }
    }

    public int ImmedBeforeColNum
    {
      get
      {
        var rowCol = (this as IVisualItem).ShowRowCol();
        return rowCol.Advance(-1).ColNum; 
      }
    }

    public virtual bool IsInputItem
    {
      get
      {
        return this.Usage.IsInput();
      }
    }

    public virtual bool IsTabToItem
    {
      get { return true; }
    }

    /// <summary>
    /// the visual item is a field ( input or output ) and the specified position is
    /// at the last column of the field.
    /// </summary>
    /// <param name="Position"></param>
    /// <returns></returns>
    public bool IsLastColumn(int Position )
    {
      // if ((this.ShowItem == null) && ( this.FromOrder == null ))
      //  return false;
      var vtb = this as VisualTextBlock;
      if (Position == vtb.ShowLength)
        return true;
      else
        return false;
    }

    /// <summary>
    /// the visual of this VisualItem ( the TextBlock ) has been placed on a canvas.
    /// When the visualItem IsEmpty the item should be removed from the VisualRow and
    /// removed from the Canvas.
    /// </summary>
    public bool IsOnCanvas
    {
      get;
      set;
    }

    public int StrColNum
    {
      get
      {
        return (this as IVisualItem).ShowRowCol( ).ColNum;
      }
    }

    public ZeroRowCol ItemRowCol
    {
      get;
      set;
    }


    public string IndirectShowText
    {
      get { return (this as IVisualItem).ShowText; }
      set { (this as IVisualItem).ShowText = value; }
    }


    public VisualItem(ZeroRowCol ItemRowCol, byte? AttrByte = null)
    {
      this.ItemRowCol = ItemRowCol;
      this.AttrByte = AttrByte;
      this.Usage = ShowUsage.Output;
      this.IsOnCanvas = false;
    }

    public VisualItem(ZeroRowCol ItemRowCol)
    {
      this.ItemRowCol = ItemRowCol;
      this.Usage = ShowUsage.Output;
      this.IsOnCanvas = false;
    }

    public override string ToString()
    {
      string s1 = this.GetType( ).ToString( ) + " RowCol:" + this.ItemRowCol.ToString();
      return s1;
    }
  }
}
