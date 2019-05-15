using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace AutoCoder.Forms
{
  public enum WidthSpecial { None, Remaining } ;

  public class AcColumnHeader : ColumnHeader
  {
    int mWidthShare;
    WidthSpecial mWidthSpecial;

    public AcColumnHeader()
      : base()
    {
      mWidthShare = 0;
      mWidthSpecial = WidthSpecial.None;
    }

    public int WidthShare
    {
      get { return mWidthShare; }
      set { mWidthShare = value; }
    }

    public WidthSpecial WidthSpecial
    {
      get { return mWidthSpecial; }
      set { mWidthSpecial = value; }
    }

    /// <summary>
    /// Calc the width of each column of the ListView 
    /// </summary>
    /// <param name="InListView"></param>
    public static void CalcListViewColumnsWidth(ListView InListView)
    {
      PercentWidthShare share = new PercentWidthShare(InListView);
      foreach (AcColumnHeader hdr in InListView.Columns)
      {
        if (hdr.WidthSpecial == WidthSpecial.Remaining)
          hdr.Width = share.RemainingShare;
        else
          hdr.Width = share.CalcShare(hdr.WidthShare);
      }
    }



  }


}
