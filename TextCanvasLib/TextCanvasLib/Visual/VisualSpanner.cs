using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using TextCanvasLib.Common;
using TextCanvasLib.Canvas;
using TextCanvasLib.xml;
using AutoCoder.Telnet.Common;
using AutoCoder.Telnet.Common.RowCol;
using AutoCoder.Telnet.IBM5250.Content;

namespace TextCanvasLib.Visual
{
  public class VisualSpanner : VisualItem, IVisualItemMore
  {
    private ZeroRowCol _AdjustShowRowCol;
    public override ZeroRowCol AdjustShowRowCol
    {
      get { return _AdjustShowRowCol; }
      set
      {
        _AdjustShowRowCol = value;
        foreach (var item in SegmentList)
        {
          item.AdjustShowRowCol = value;
        }
      }
    }

    public string ClassCode
    {
      get { return "Spanner"; }
    }

    public bool IsField
    { get; set; }

    public override bool IsTabToItem
    {
      get { return false; }
    }

    public bool ModifiedFlag
    {
      get
      {
        bool modifiedFlag = false;
        foreach (var seg in this.SegmentList)
        {
          if (seg.ModifiedFlag == true)
          {
            modifiedFlag = true;
            break;
          }
        }
        return modifiedFlag;
      }

      set
      {
        throw new Exception("cannot set modified flag of spanner");
      }
    }

    public string ShowText
    {
      get
      {
        StringBuilder sb = new StringBuilder();
        foreach( var seg in this.SegmentList)
        {
          sb.Append(seg.ShowText);
        }
        return sb.ToString();
      }
      set
      {
        var s1 = value;
      }
    }
    public int ShowLength
    {
      get { return this.ShowText.Length; }
    }

    public List<VisualTextBlockSegment> SegmentList
    { get; set; }

    public VisualSpanner( string Text, ZeroRowCol ItemRowCol, byte? AttrByte,
      CanvasDefn CanvasDefn)
      : base(ItemRowCol)
    {
      // create VisualTextBlock items for each segment that fits on the row.
      this.SegmentList = new List<VisualTextBlockSegment>();
      CreateTextBlockSegments(this, Text, ItemRowCol, AttrByte, CanvasDefn);

      this.AttrByte = AttrByte;
    }

    public VisualSpanner(
      ScreenContent ScreenContent, ContinuedContentField ContContentField,
      CanvasDefn CanvasDefn)
      : base(ContContentField.RowCol)
    {
      this.SegmentList = new List<VisualTextBlockSegment>();
      var fieldNum = ContContentField.FieldNum;

      // create VisualTextBloc item for each segment of the continued field.
      int segBx = 0;
      foreach (var contField in ScreenContent.ContinuedFieldSegments(fieldNum))
      {
        var segNum = contField.FieldKey.SegNum;
        bool attrByteOccupySpace = true;
        var vtb = new VisualTextBlockSegment(
          this, segNum, segBx,
          contField.GetShowText(ScreenContent),
          contField.RowCol as ZeroRowCol, contField.GetAttrByte(ScreenContent),
          CanvasDefn, attrByteOccupySpace);
        vtb.SetupUnderline();
        this.SegmentList.Add(vtb);
        vtb.ContinuedFieldSegmentCode = contField.ContinuedFieldSegmentCode;
        segBx += contField.GetShowText(ScreenContent).Length;
      }

      this.AttrByte = ContContentField.GetAttrByte(ScreenContent);
    }

    /// <summary>
    /// add the individual items that make up the span to the canvas.
    /// </summary>
    /// <param name="itemCanvas"></param>
    public void AddToCanvas(ItemCanvas itemCanvas)
    {
      foreach( var item in SegmentList)
      {
        item.AddToCanvas(itemCanvas);
      }
    }

    /// <summary>
    /// Apply the Text to the spanner starting at the specified Pos.
    /// Meaning, calc the start and end pos in the spanner to apply the text to, 
    /// then find those segments which coincide with those locations and apply the
    /// text to the segments.
    /// </summary>
    /// <param name="Text"></param>
    /// <param name="Pos"></param>
    public void ApplyText( string Text, int Pos )
    {
      int px = Pos;
      var text = Text;
      foreach (var seg in SegmentList)
      {
        if (( px >= seg.SegBx ) && ( px <= seg.SegEx ))
        {
          int bx = px - seg.SegBx;
          seg.ApplyText(text, bx);
          int applyLx = seg.ShowText.Length - bx;
          if (text.Length > applyLx)
            text = text.Substring(applyLx);
          else
            text = null;
          px += seg.ShowText.Length;
        }

        // no more text to apply.
        if (text == null )
          break;
      }
    }
    public VisualItem ApplyText(LocatedString Text)
    {
      var s1 = this.LocatedText();
      var text = Text;
      var s3 = LocatedString.Union(text, s1);
      this.ShowText = s3.Text;
      return this;
    }

    private static void CreateTextBlockSegments( 
      VisualSpanner Parent,
      string Text, IRowCol StartRowCol, byte? AttrByte,
      CanvasDefn CanvasDefn)
    {
      IRowCol rowCol = StartRowCol;
      string text = Text;
      int segNum = 0;
      int segBx = 0;

      // advance past attrByte.
#if skip
      if (AttrByte != null)
      {
        rowCol = rowCol.Advance(1);
      }
#endif
      bool attrByteOccupySpace = true;

      // loop create textblock segments until no more text.
      while (text.Length > 0)
      {
        // space on this row.
        var remLx = rowCol.GetRowRemaining();
        if (attrByteOccupySpace == true)
          remLx -= 1;

        // text to place on current row.
        int usedLx = 0;
        if (remLx > text.Length)
          usedLx = text.Length;
        else
          usedLx = remLx;

        // create the textblock segment.
        if (usedLx > 0)
        {
          segNum += 1;
          var vtb = new VisualTextBlockSegment(
            Parent, segNum, segBx,
            Text.Substring(segBx, usedLx), rowCol as ZeroRowCol, AttrByte,
            CanvasDefn, attrByteOccupySpace);
          vtb.SetupUnderline();
          Parent.SegmentList.Add(vtb);

          // advance to next segment in show text.
          segBx += usedLx;
          if (text.Length > usedLx)
            text = text.Substring(usedLx);
          else
            text = "";
        }

        // advance rowCol 
        if (attrByteOccupySpace == true)
          usedLx += 1;
        rowCol = rowCol.Advance(usedLx);

        attrByteOccupySpace = false;
      }
    }

    public LinkedListNode<IVisualItem> InsertIntoVisualItemsList( ScreenVisualItems VisualItems)
    {
      // insert this spanner object.
      var node = VisualItems.InsertIntoVisualItemsList(this);

      // insert each of the TextBlock segments into VisualItems collection.
      foreach( var item in SegmentList)
      {
        VisualItems.InsertIntoVisualItemsList(item);
      }

      return node;
    }

    public void RemoveFromCanvas(ItemCanvas itemCanvas)
    {
      foreach (var item in SegmentList)
      {
        item.RemoveFromCanvas(itemCanvas);
      }
    }

    public void SetAttrByteOccupySpace( bool Value)
    {
      foreach( var item in SegmentList)
      {
        item.AttrByteOccupySpace = Value;
      }
    }

    public void SetupFieldItem(ShowFieldItem ShowItem, Size CharBoxDim, Size KernDim)
    {
      this.ShowItem = ShowItem;
      this.CreateFromItem = ShowItem;

      foreach (var item in SegmentList)
      {
        item.SetupFieldItem(ShowItem, CharBoxDim, KernDim);
      }
    }

    public void SetupFieldItem(
      ScreenContent ScreenContent, ContentField sfo, Size CharBoxDim, Size KernDim)
    {
      this.FromContentItem = sfo;

      foreach (var item in SegmentList)
      {
        item.SetupFieldItem(ScreenContent, sfo, CharBoxDim, KernDim);
      }
    }

    public void SetupUnderline( )
    {
    }
  }
}
