using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using AutoCoder.Core;
using AutoCoder.Drawing;

namespace AutoCoder.Forms
{
  public class DrawStringColumn : Panel
  {
    DrawStringList mDrawStrings = null;
    ContentAlignment mTextAlign = ContentAlignment.BottomLeft;
    
    // FontHeight is use to calc the vertical line advance when drawing the
    // DrawString lines. Use the LineAdvanceHeight property to override this
    // vertical line advance value.
    Nullable<float> mLineAdvanceHeight = null;

    public DrawStringColumn()
    {
      BuildForm();
    }

    void BuildForm()
    {
      this.Paint += new PaintEventHandler(DrawStringColumn_Paint);
    }

    void DrawStringColumn_Paint(object sender, PaintEventArgs e)
    {
      using (Graphics g = this.CreateGraphics())
      {
        float y = 0;
        
        float ht = this.Font.GetHeight(g);
        if (LineAdvanceHeight != null)
          ht = LineAdvanceHeight.Value;

        float wx = this.ClientRectangle.Width;
        StringFormat sf = new StringFormat(StringFormatFlags.NoWrap);
        if (TextAlign == ContentAlignment.TopRight)
        {
          sf.Alignment = StringAlignment.Far;
          sf.LineAlignment = StringAlignment.Near;
        }

        foreach (DrawString lineNbr in DrawStrings)
        {
          RectangleF dsRect = new RectangleF(0, y, wx, ht);

          if (lineNbr.BackColorIsAssigned == true)
          {
            using (Brush bc = new SolidBrush(lineNbr.BackColor.Value))
            {
              g.FillRectangle(bc, dsRect);
            }
          }

          // draw the text.
          if (lineNbr.ForeColorIsAssigned == true)
          {
            using (Brush fb = new SolidBrush(lineNbr.ForeColor.Value))
            {
              g.DrawString(lineNbr.Text, this.Font, fb, dsRect, sf);
            }
          }
          else
          {
            g.DrawString(lineNbr.Text, this.Font, Brushes.Black, dsRect, sf);
          }

          // advance to the next line.
          y += ht;
        }
      }
    }

    public DrawStringList DrawStrings
    {
      get
      {
        if (mDrawStrings == null)
        {
          mDrawStrings = new DrawStringList();
          mDrawStrings.ListChanged += 
            new delListChanged(DrawStrings_ListChanged);
        }

        return mDrawStrings;
      }
      set
      {
        mDrawStrings = value;
        Invalidate();
      }
    }

    void DrawStrings_ListChanged(object InList)
    {
      Invalidate();
    }

    /// <summary>
    /// Number of points to vertically advance before drawing each successive
    /// DrawString.
    /// </summary>
    public Nullable<float> LineAdvanceHeight
    {
      get { return mLineAdvanceHeight; }
      set { mLineAdvanceHeight = value; }
    }

    public ContentAlignment TextAlign
    {
      get { return mTextAlign; }
      set { mTextAlign = value; }
    }
  }
}
