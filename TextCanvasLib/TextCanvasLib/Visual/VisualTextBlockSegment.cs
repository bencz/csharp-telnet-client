using AutoCoder.Telnet.Common;
using AutoCoder.Telnet.Common.RowCol;
using TextCanvasLib.Canvas;
using TextCanvasLib.xml;

namespace TextCanvasLib.Visual
{
  /// <summary>
  /// textBlock which is a segment of a textBlock that spans multiple rows.
  /// </summary>
  public class VisualTextBlockSegment : VisualTextBlock
  {
    ShowItemBase _CreateFromItem;
    public override ShowItemBase CreateFromItem
    {
      get
      {
        if (_CreateFromItem != null)
          return _CreateFromItem;
        else
          return this.Parent.CreateFromItem;
      }
      set { _CreateFromItem = value; }
    }

    public override bool IsTabToItem
    {
      get
      {
        if (this.SegNum == 1)
          return true;
        else
          return false;
      }
    }

    ShowFieldItem _ShowItem;
    public override ShowFieldItem ShowItem
    {
      get
      {
        if (_ShowItem != null)
          return _ShowItem;
        else
          return this.Parent.ShowItem;
      }
      set { _ShowItem = value; }
    }

    /// <summary>
    /// one based segment number of the segment within the spanner.
    /// </summary>
    public int SegNum
    {
      get; set;
    }

    /// <summary>
    /// begin pos of this segment in the spanner field.
    /// </summary>
    public int SegBx
    { get; set; }

    public int SegEx
    {
      get
      {
        int ex = this.SegBx + this.ShowText.Length - 1;
        return ex;
      }
    }

    /// <summary>
    /// the VisualSpanner from which this segment was created. ( this linkage is
    /// important in that the textblock segment is placed on the canvas. When
    /// reading controls on the canvas need this property to find the spanner
    /// the segment is a part of. Then the spanner points to the 5250 data stream
    /// entry field. )
    /// </summary>
    public VisualSpanner Parent
    { get; set; }
    public VisualTextBlockSegment(
      VisualSpanner Parent, int SegNum, int SegBx,
      string Text, ZeroRowCol RowCol, byte? AttrByte, CanvasDefn CanvasDefn,
      bool AttrByteOccupySpace )
      : base(Text, RowCol, AttrByte, 
          CanvasDefn.CharBoxDim, CanvasDefn.KernDim, CanvasDefn.FontDefn)
    {
      this.Parent = Parent;
      this.SegNum = SegNum;
      this.SegBx = SegBx;
      this.AttrByteOccupySpace = AttrByteOccupySpace;
    }
  }
}
