using System.Text;
using TextCanvasLib.Common;
using AutoCoder.Telnet.Common;
using AutoCoder.Telnet.Common.RowCol;

namespace TextCanvasLib.Visual
{
  public interface IVisualItem
  {
    ZeroRowCol AdjustShowRowCol { get; set; }

    /// <summary>
    /// length of the item as it appears on the screen. Does not contain the length
    /// of the optional attribute byte. ( Use ItemLength extension method to get
    /// that value. )
    /// </summary>
    int ShowLength { get; }

    /// <summary>
    /// the show text of the item. If a literal, this is the literal text as it 
    /// will be shown on the canvas. If an entry field, this is the text contents of
    /// the entry field padded with blanks to the length of the field.
    /// </summary>
    string ShowText { get; set; }
    
    /// <summary>
    /// optional attribute byte that preceeds the item text or entry field.
    /// </summary>
    byte? AttrByte { get; set; }

    /// <summary>
    /// the attribute byte of this item occupies space in the screen content block.
    /// Use this property when calc ShowRowCol from ItemRowCol. 
    /// VisualTextBlockSegment items have an attr byte. But the ItemRowCol and 
    /// ShowRowCol values of the item still match.
    /// </summary>
    bool? AttrByteOccupySpace { get; set;  }

    /// <summary>
    /// attribute byte that follows the text of the item.
    /// ( not used when showing the item. But is used when calc the length of the
    ///   item. Also used when create TextDataOrder from this item. )
    /// </summary>
    byte? TailAttrByte { get; set; }

    bool IsOnCanvas { get; set; }

    bool IsField { get; }

    /// <summary>
    /// item can accept keyed input. 
    /// </summary>
    bool IsInputItem { get; }

    /// <summary>
    /// item can be tabbed to. Also, when advancing after the last char of the
    /// field is keyed into, this item can be advanced to.
    /// </summary>
    bool IsTabToItem { get; }

    /// <summary>
    /// start row/col location of the item as it appears on the screen. 
    /// </summary>
    ZeroRowCol ItemRowCol
    { get; }
  }

  public static class IVisualItemExt
  {
    public static ZeroRowCol AttrByteRowCol(this IVisualItem Item)
    {
      if (Item.AttrByte == null)
        return null;
      else
        return Item.ItemRowCol;
    }

    /// <summary>
    /// the location is within the column/row bounds of the visual item.
    /// </summary>
    /// <param name="Loc"></param>
    /// <returns></returns>
    public static bool ContainsLocation(this IVisualItem Item, IRowCol Loc)
    {
      var showRowCol = Item.ShowRowCol();
      if ((Loc.RowNum == showRowCol.RowNum)
        && (Loc.ColNum >= showRowCol.ColNum)
        && (Loc.ColNum <= Item.ShowEndRowCol( ).ColNum))
        return true;
      else
        return false;
    }

    /// <summary>
    /// determine if the show portion of the item ( not the leading attr byte ) spans
    /// multiple rows of the canvas.
    /// </summary>
    /// <param name="Item"></param>
    /// <returns></returns>
    public static bool DoesShowSpanMultipleRows( this IVisualItem Item)
    {
      bool doesSpan = false;

      var showRowCol = Item.ShowRowCol();
      var endRowCol = Item.ShowEndRowCol();

      if (showRowCol.RowNum == endRowCol.RowNum)
        doesSpan = false;
      else
        doesSpan = true;

      return doesSpan;
    }

    public static bool IsEmpty(this IVisualItem Item)
    {
      if (Item.ShowText.Length == 0)
        return true;
      else
        return false;
    }
    public static IRowCol ItemEndRowCol(this IVisualItem Item)
    {
      var itemEndRowCol = Item.ShowEndRowCol();
      if (Item.TailAttrByte != null)
        itemEndRowCol = itemEndRowCol.Advance(1);
      return itemEndRowCol;
    }
    public static int ItemLength(this IVisualItem Item)
    {
      int lx = 0;
      if (Item.AttrByte != null)
        lx += 1;
      if (Item.TailAttrByte != null)
        lx += 1;
      lx += Item.ShowLength;
      return lx;
    }

    public static RowColRange ItemRange(this IVisualItem Item)
    {
      var from = Item.ItemRowCol;
      var to = Item.ItemEndRowCol();
      var range = new RowColRange(from, to);
      return range;
    }

    // ItemText method does not make sense. Returns the attr byte concat with ShowText.
    // But attrByte does not translate from its byte code to unicode.
    public static string ItemText(this IVisualItem Item)
    {
      var sb = new StringBuilder();
      if (Item.AttrByte != null)
        sb.Append((char)Item.AttrByte.Value);
      sb.Append(Item.ShowText);
      if (Item.TailAttrByte != null)
        sb.Append((char)Item.TailAttrByte.Value);
      return sb.ToString();
    }
    public static LocatedString LocatedText(this IVisualItem Item)
    {
      var showRowCol = Item.ShowRowCol();
      var s1 = new LocatedString(showRowCol.ColNum, Item.ShowText);
      return s1;
    }

    /// <summary>
    /// advance RowCol to location immed after the end of this item.
    /// </summary>
    /// <param name="Item"></param>
    /// <returns></returns>
    public static IRowCol NextRowCol(this IVisualItem Item)
    {
      var nextRowCol = Item.ItemRowCol.Advance(Item.ItemLength());
      return nextRowCol;
    }

    /// <summary>
    /// the row/col location of the last positon of the item.
    /// </summary>
    /// <param name="Item"></param>
    /// <returns></returns>
    public static IRowCol ShowEndRowCol(this IVisualItem Item)
    {
      var showRowCol = Item.ShowRowCol();
      var endRowCol = showRowCol.Advance(Item.ShowLength - 1);
      return endRowCol;
    }

    public static ZeroRowCol ShowRowCol(this IVisualItem Item)
    {
      ZeroRowCol showRowCol = null;
      if (Item.AttrByte == null)
        showRowCol = Item.ItemRowCol;
      else if ((Item.AttrByteOccupySpace != null) 
        && (Item.AttrByteOccupySpace.Value == false))
        showRowCol = Item.ItemRowCol;
      else
        showRowCol = Item.ItemRowCol.Advance(1) as ZeroRowCol;

      if ( Item.AdjustShowRowCol != null)
      {
        showRowCol = showRowCol.Add(Item.AdjustShowRowCol) as ZeroRowCol;
      }

      return showRowCol;
    }

    /// <summary>
    /// column location is within the column bounds of the item.
    /// </summary>
    /// <param name="ColNum"></param>
    /// <returns></returns>
    public static bool WithinColumnBounds(this IVisualItem Item, int ColNum)
    {
      if ((ColNum >= Item.ShowRowCol().ColNum) 
        && (ColNum <= Item.ShowEndRowCol().ColNum))
        return true;
      else
        return false;
    }


  }
}
