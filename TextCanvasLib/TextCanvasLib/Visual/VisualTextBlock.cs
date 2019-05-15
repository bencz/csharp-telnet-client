using AutoCoder.Ext.System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using TextCanvasLib.Common;
using TextCanvasLib.Controls;
using TextCanvasLib.Canvas;
using TextCanvasLib.xml;
using AutoCoder.Telnet.IBM5250.WtdOrders;
using AutoCoder.Telnet.Common;
using AutoCoder.Telnet.Enums;
using AutoCoder.Telnet.Common.RowCol;
using AutoCoder.Telnet;
using AutoCoder.Ext.Windows;
using System.Windows.Media;
using AutoCoder.Telnet.IBM5250.Content;
using AutoCoder.Telnet.Enums.IBM5250;

namespace TextCanvasLib.Visual
{
  public class VisualTextBlock : VisualItem, IVisualItemMore, IVisualItemEntry
  {
    public string ClassCode
    {
      get { return "TextBlock"; }
    }

    public ContinuedFieldSegmentCode? ContinuedFieldSegmentCode
    { get; set;  }

    public Brush ForegroundBrush
    { get; set; }
    public bool IsBypass
    { get; set;  }

    /// <summary>
    /// textblock is created from StartFieldOrder
    /// </summary>
    public bool IsField
    { get; set; }
    public bool IsMonocase
    { get; set; }
    public bool IsNonDisplay
    { get; set; }
    public ClientTextBlock TextBlock
    { get; set; }
    public string ShowText
    {
      get
      {
        if (TextBlock == null)
          return null;
        else
          return TextBlock.Text;
      }
      set { TextBlock.Text = value; }
    }
    public bool ModifiedFlag
    { get; set; }
    public int ShowLength
    {
      get
      {
        return this.ShowText.Length;
      }
    }
    public byte[] FFW_Bytes
    { get; set; }

    public VisualTextBlock(string Text, ZeroRowCol RowCol, byte? AttrByte,
      Size CharBoxDim, Size KernDim, FontDefn FontDefn)
      : base(RowCol, AttrByte)
    {
      this.TextBlock = new ClientTextBlock(CharBoxDim, KernDim);
      this.ForegroundBrush = AttrByte.GetForegroundBrush();
      SetPaintDefn(CharBoxDim, KernDim, FontDefn);
      this.ShowText = Text;
    }

    /// <summary>
    /// set the font, color, etc of the TextBlock control that is on the Canvas.
    /// </summary>
    /// <param name="CharBoxDim"></param>
    /// <param name="KernDim"></param>
    /// <param name="FontDefn"></param>
    public void SetPaintDefn( Size CharBoxDim, Size KernDim, FontDefn FontDefn)
    {
      this.TextBlock.SetFontDefn(FontDefn);
      if (this.ForegroundBrush != null)
        this.TextBlock.Foreground = this.ForegroundBrush;
      this.TextBlock.ChangeSizeBasis(CharBoxDim, KernDim);
    }

    public void AddToCanvas(ItemCanvas itemCanvas)
    {
      // dspatr(nd)
      if ((this.AttrByte != null) && (this.AttrByte.Value == 0x27))
      {
        this.TextBlock.Visibility = System.Windows.Visibility.Hidden;
      }

      this.CanvasPos = itemCanvas.AddItemToCanvas(this, this.TextBlock);
    }

    public VisualItem ApplyText(LocatedString Text)
    {
      var s1 = this.LocatedText( );
      var text = Text;
      if (this.IsMonocase == true)
        text = text.ToUpper();
      var s3 = LocatedString.Union(text, s1);
      this.ShowText = s3.Text;
      return this;
    }

    /// <summary>
    /// apply the input text to the text value of this textblock.
    /// ( important: the length of the TextBlock is fixed. Any applied text that 
    ///   exceeds the length of the textblock is clipped. )
    /// </summary>
    /// <param name="Text"></param>
    /// <param name="Pos"></param>
    public void ApplyText( string Text, int Pos )
    {
      var sb = new StringBuilder();
      int showLx = this.ShowText.Length;

      var text = Text;
      if (this.IsMonocase == true)
        text = text.ToUpper();
      
      // setup length of text to apply. Clip the apply text length to prevent the
      // length of ShowText from being increased.
      int applyLx = text.Length;
      {
        int remLx = showLx - Pos;
        if (remLx < applyLx)
          applyLx = remLx;
      }

      var s1 = this.ShowText.ApplyText(text, Pos, applyLx);
      this.ShowText = s1;
    }

    public LinkedListNode<IVisualItem> InsertIntoVisualItemsList(
      ScreenVisualItems VisualItems)
    {
      // insert this spanner object.
      var node = VisualItems.InsertIntoVisualItemsList(this);

      return node;
    }

    public void RemoveFromCanvas(ItemCanvas itemCanvas)
    {
      itemCanvas.Canvas.Children.Remove(this.TextBlock);
      this.IsOnCanvas = false;
    }
    public VisualItem RemoveText(LocatedString RemoveText)
    {
      var s1 = this.LocatedText( ).Remove(RemoveText);
      this.ShowText = s1.Text;
      return this;
    }

    public void SetupFieldItem( 
      StartFieldOrder sfo, Size CharBoxDim, Size KernDim )
    {
      this.IsMonocase = sfo.IsMonocase;
      this.IsNonDisplay = sfo.IsNonDisplay;
      this.IsBypass = sfo.IsBypass;
      this.Usage = ShowUsage.Both;
      this.FFW_Bytes = sfo.FFW_Bytes;
      SetupFieldItem_Common(CharBoxDim, KernDim);

      this.FromOrder = sfo;
    }

    public void SetupFieldItem(
      ScreenContent ScreenContent,
      ContentField sfo, Size CharBoxDim, Size KernDim)
    {
      this.IsMonocase = sfo.IsMonocase;
      this.IsNonDisplay = sfo.GetIsNonDisplay(ScreenContent);
      this.IsBypass = sfo.IsBypass;
      this.Usage = ShowUsage.Both;
      this.FFW_Bytes = sfo.FFW_Bytes;
      SetupFieldItem_Common(CharBoxDim, KernDim);

      this.FromContentItem = sfo;
    }

    public void SetupFieldItem( 
      ShowFieldItem ShowItem, Size CharBoxDim, Size KernDim )
    {
      // store the fieldItem this visualItem is painted from.
      this.ShowItem = ShowItem;
      this.CreateFromItem = ShowItem;
      this.IsMonocase = ShowItem.IsMonocase;
      this.IsNonDisplay = ShowItem.IsNonDisplay;
      this.IsBypass = ShowItem.IsBypass;
      this.Usage = ShowItem.Usage;
      this.FFW_Bytes = ShowItem.sfo_FFW;

      SetupFieldItem_Common(CharBoxDim, KernDim);
    }

    private void SetupFieldItem_Common( Size CharBoxDim, Size KernDim )
    {
      // is a bypass field.  no entry. output only.
      if (this.IsBypass == true)
      {
        this.Usage = ShowUsage.Output;
      }
      this.IsField = true;
    }

    public void SetupUnderline( )
    {
      if (this.IsUnderline == true)
      {
        this.TextBlock.DrawUnderline();
      }
    }

    public bool IsUnderline
    {
      get
      {
        if (this.AttrByte == null)
          return false;
        else
        {
          var ab = this.AttrByte.Value;
          if ((ab == 0x24) || (ab == 0x25) || (ab == 0x26))
            return true;
          else
            return false;
        }
      }
    }

  }
}
