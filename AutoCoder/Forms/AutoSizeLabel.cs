using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Drawing;

namespace AutoCoder.Forms
{
  /// <summary>
  /// A label with enough height will wrap its text contents. 
  /// AutoSizeLabel uses GetPreferredSize to calc the height needed
  /// to show all the text within the width of the label.
  /// </summary>
  public class AutoSizeLabel : Label
  {
    bool mIsResizing;
    int mLastResizeWidth = -1;

    public AutoSizeLabel()
    {
      this.AutoSize = false;
    }

    public override bool AutoSize
    {
      get
      {
        return base.AutoSize;
      }
      set
      {
        if (value == true)
          throw new ArgumentException(
            "AutoSize property must be false in an AutoSizeLabel");
        base.AutoSize = value;
      }
    }

    private void ResizeLabel()
    {
      if (mIsResizing) return;
      try
      {
        mIsResizing = true;
        Size sz2 = new Size(this.Width, Int32.MaxValue);
        sz2 = this.GetPreferredSize(sz2);
        if (this.Height != sz2.Height)
        {
          this.Height = sz2.Height;
        }
        mLastResizeWidth = this.Width;
      }
      finally
      {
        mIsResizing = false;
      }
    }

    protected override void OnPaddingChanged(EventArgs e)
    {
      base.OnPaddingChanged(e);
      ResizeLabel();
    }

    protected override void OnFontChanged(EventArgs e)
    {
      base.OnFontChanged(e);
      ResizeLabel();
    }

    protected override void OnMarginChanged(EventArgs e)
    {
      base.OnMarginChanged(e);
      ResizeLabel();
    }

    protected override void OnSizeChanged(EventArgs e)
    {
      base.OnSizeChanged(e);
      if ((mIsResizing == false) && ( this.Width != mLastResizeWidth ))
      {
        ResizeLabel();
      }
    }

    protected override void OnTextChanged(EventArgs e)
    {
      base.OnTextChanged(e);
      ResizeLabel();
    }
  }
}
