using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace AutoCoder.Forms
{

  // ----------------------------- PercentWidthShare -------------------------------
  public class PercentWidthShare
  {
    Control mControl;
    int mUsedIntWx = 0;
    double mUsedDoubleWx = 0.0;

    public PercentWidthShare(Control InControl)
    {
      mControl = InControl;
    }

    public int RemainingShare
    {
      get { return mControl.ClientSize.Width - mUsedIntWx; }
    }

    public int CalcShare(double InPercentage)
    {
      double pct = InPercentage / 100.00;
      double controlWx = (double)mControl.ClientSize.Width;
      double remWx = controlWx - mUsedDoubleWx;
      double shareWx = controlWx * pct;
      if ((shareWx + mUsedDoubleWx) > controlWx)
        shareWx = controlWx - mUsedDoubleWx;

      mUsedDoubleWx += shareWx;
      int usedIntWx = (int)Math.Round(mUsedDoubleWx);
      int shareIntWx = usedIntWx - mUsedIntWx;
      if ((shareIntWx + mUsedIntWx) > mControl.ClientSize.Width)
        shareIntWx = mControl.ClientSize.Width - mUsedIntWx;

      mUsedIntWx += shareIntWx;
      return shareIntWx;
    }

  } // end class PercentWidthShare

}
