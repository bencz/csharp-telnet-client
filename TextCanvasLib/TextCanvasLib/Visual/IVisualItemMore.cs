using AutoCoder.Telnet.Common.RowCol;
using AutoCoder.Telnet.IBM5250.Content;
using AutoCoder.Telnet.IBM5250.WtdOrders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using TextCanvasLib.Canvas;
using TextCanvasLib.xml;

namespace TextCanvasLib.Visual
{
  // VisualSpanner, VisualTextBlock implement this interface.
  // intended to encapsulate functions where items are place on the canvas
  // ( when a spanner item is placed on the canvas, the AddToCanvas method of the
  //   spanner handles the placing of spanned items. )
  // IVisualItemEntry encompasses methods which direct user input to the item.


  public interface IVisualItemMore : IVisualItem
  {
    ShowItemBase CreateFromItem { get; set; }

    /// <summary>
    /// the field item the visual item was created from. ( if is an input field. )
    /// </summary>
    ShowFieldItem ShowItem { get; set; }

    WtdOrderBase FromOrder { get; set;  }

    /// <summary>
    /// modified data tag. Setoff when field is created from input data stream. Set
    /// on when field text is changed by screen entry.
    /// </summary>
    bool ModifiedFlag
    { get; set; }

    LinkedListNode<IVisualItem> InsertIntoVisualItemsList(
      ScreenVisualItems VisualItems);
    void AddToCanvas(ItemCanvas itemCanvas);

    void ApplyText(string Text, int Pos);

    void RemoveFromCanvas(ItemCanvas itemCanvas);

    void SetupFieldItem(ShowFieldItem FieldItem, Size CharBoxDim, Size KernDim);
    void SetupFieldItem(
      ScreenContent ScreenContent, ContentField FieldItem, Size CharBoxDim, 
      Size KernDim);

    void SetupUnderline();
    string ClassCode { get; }
  }

  public static class IVisualItemMoreExt
  {
    public static string TypeCode(this IVisualItemMore item)
    {
      if (item.ClassCode == "Spanner")
        return item.ClassCode;
      else if (item.ShowItem == null)
        return "Literal";
      else
        return "Field";
    }
  }
}
