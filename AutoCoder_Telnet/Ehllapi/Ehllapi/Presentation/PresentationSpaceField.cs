using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using AutoCoder.Forms;
using System.Xml.Linq;
using System.Windows.Media;
using System.Globalization;
using System.Windows;
using AutoCoder.Ehllapi.Common;

namespace AutoCoder.Ehllapi.Presentation
{
  public class PresentationSpaceField
  {
    PresentationSpace mPs;
    DisplayLocation mLocation;
    CharAttrByte mCharAttrByte = null;
    ColorAttrByte mColorAttrByte = null;
    int mLength = 0;
    string mText = null;
    char[] mEnteredText = null;

    public PresentationSpaceField(
      out LinkedListNode<PresentationSpacePixel> OutEndPixelNode,
      PresentationSpace InPs,
      LinkedListNode<PresentationSpacePixel> InPixelNode )
    {
      PresentationSpacePixel pixel = InPixelNode.Value;
      mPs = InPs;
      mLocation = pixel.DisplayLocation;
      this.FieldAttribute = new FieldAttribute(pixel.Byte1);

      // store the character attributes found in the first character pixel.
      if (pixel.CharAttrByte != null)
        mCharAttrByte = pixel.CharAttrByte;

      // length runs from pixel after the attribute pixel to the next
      // attribute pixel.
      mLength = 0;
      StringBuilder sb = new StringBuilder();
      LinkedListNode<PresentationSpacePixel> node = InPixelNode;
      LinkedListNode<PresentationSpacePixel> endNode = InPixelNode;
      while (true)
      {
        node = node.Next;
        if (node == null)
          break;
        pixel = node.Value;
        if (pixel.IsFieldAttribute == true)
          break;
        endNode = node;
        ++mLength;
        sb.Append( pixel.CharValue ) ;

        if (sb.Length == 1)
        {
          if (pixel.ColorAttrByte != null)
            mColorAttrByte = pixel.ColorAttrByte;
        }
      }
      mText = sb.ToString( ) ;
      OutEndPixelNode = endNode;
    }

    public PresentationSpaceField(
      FieldAttribute FldAttr, DisplayLocation Loc, string Text)
    {
      mLocation = Loc ;
      this.FieldAttribute = FldAttr;
      mText = Text;
      mLength = Text.Length;
    }

    public CharAttrByte CharAttrByte
    {
      get { return mCharAttrByte; }
      set { mCharAttrByte = value; }
    }

    public ColorAttrByte ColorAttrByte
    {
      get { return mColorAttrByte; }
      set { mColorAttrByte = value; }
    }

    /// <summary>
    /// The text of the field entered after PresentationSpace is 
    /// retrieved from the ehllapi session.
    /// </summary>
    public char[] EnteredText
    {
      get
      {
        if (mEnteredText == null)
        {
          mEnteredText = Text.ToCharArray();
        }
        return mEnteredText;
      }
    }

    FieldAttribute mFieldAttribute;
    public FieldAttribute FieldAttribute
    {
      get { return mFieldAttribute; }
      set { mFieldAttribute = value; }
    }

    public int Length
    {
      get { return mLength; }
      set { mLength = value; }
    }

    public DisplayLocation Location
    {
      get { return mLocation; }
      set { mLocation = value; }
    }

    public PresentationSpace PresentationSpace
    {
      get { return mPs; }
    }

    public string Text
    {
      get { return mText; }
    }

    public bool ContainsLocation(DisplayLocation Loc)
    {
      // calc the end location. Length is the char length of the field. Add 1 to
      // include the attribute pixel in the end point calculation.
      var endLoc = mPs.Dim.CalcEndLocation(Location, Length + 1);

      int bx = Location.ToLinear( mPs.Dim );
      int ex = endLoc.ToLinear( mPs.Dim );
      int ix = Loc.ToLinear( mPs.Dim );

      if ((ix >= bx) && (ix <= ex))
        return true;
      else
        return false;
    }

    public static System.Windows.Size CalcCharSize(double PointSize)
    {
      var singleChar = new FormattedText(
          "A",
          CultureInfo.GetCultureInfo("en-us"),
          System.Windows.FlowDirection.LeftToRight,
          new Typeface("Lucida Console"),
          PointSize,
          System.Windows.Media.Brushes.Black);

      var sz = new System.Windows.Size(singleChar.Width, singleChar.Height);
      return sz;
    }

    /// <summary>
    /// get the DrawContext of a control from its OnRender method.
    /// </summary>
    /// <param name="DrawContext"></param>
    public void DrawText(DrawingContext DrawContext)
    {
      double emSize = 12;
      var formattedText = new FormattedText(
          this.Text,
          CultureInfo.GetCultureInfo("en-us"),
          System.Windows.FlowDirection.LeftToRight,
          new Typeface("Lucida Console"),
          emSize,
          System.Windows.Media.Brushes.Black);

      var charSx = PresentationSpaceField.CalcCharSize(emSize); 

      // advance the fld location past the attribute byte.
      var psDim = new PresentationSpaceDim(24, 80);
      var loc = psDim.IncDisplayLocation(this.Location);
      double y = (loc.Row - 1) * charSx.Height;
      double x = (loc.Column - 1) * charSx.Width;

      // Draw the formatted text string to the DrawingContext of the control.
      DrawContext.DrawText(formattedText, new System.Windows.Point(x, y));
    }

    public void DrawText(
      FixedFontMeasurer InFm, System.Drawing.Graphics InGraphics, 
      System.Drawing.Font InFont)
    {
      var psDim = PresentationSpace.Dim;

      // advance the fld location past the attribute byte.
      var loc = psDim.IncDisplayLocation(this.Location);

      // the field could overflow to the next line ( or more ).
      // draw the field in segments. 
      int ix = 0;
      while ( ix < this.EnteredText.Length )
      {
        // calc length to draw on this line.
        int drawLx = EnteredText.Length - ix;
        int endColNx = loc.Column + drawLx - 1 ;
        if ( endColNx > psDim.Width )
        {
          drawLx = psDim.Width - loc.Column + 1 ;
        }

        var from = loc.ToCharPoint();
        var r1 = InFm.CalcRectangle( from, new System.Drawing.Size( drawLx, 1));
        var s1 = new String( this.EnteredText, ix, drawLx ) ;
        TextRenderer.DrawText(InGraphics, s1, InFont, r1, System.Drawing.Color.Red);

        // an input field. draw the underline.
        if (this.FieldAttribute.IsProtected == false)
        {
          var fr = Graphicser.SetRelativePoint(
            r1, AlignPoint.Left, 0,
            r1, AlignPoint.Bottom, 3);
          var redPen = new System.Drawing.Pen( System.Drawing.Color.Red ) ;
          Graphicser.DrawHorizontalLine(redPen, InGraphics, fr, r1.Width);
        }

        ix += drawLx;
        loc = psDim.CrLf(loc);
      }
    }

    public override string ToString()
    {
      if (mText == null)
        return "";
      else
        return mText;
    }
  }
}
