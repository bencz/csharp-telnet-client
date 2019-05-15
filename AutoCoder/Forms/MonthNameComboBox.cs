using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using AutoCoder.Text;

namespace AutoCoder.Forms
{
  public class MonthNameComboBox : TextComboBox
  {
    string[] mMonthNames =
      new string[] {"Jan", "Feb", "Mar", "April", "May", "June", "July", "Aug", "Sept", 
          "Oct", "Nov", "Dec" };

    public MonthNameComboBox()
    {
      this.DropDownStyle = ComboBoxStyle.DropDownList;
      this.Items.AddRange(mMonthNames);
      this.Value = "Jan";
    }

    public string MonthName
    {
      get
      {
        return this.Value;
      }
      set { this.Value = value; }
    }

    public string[] MonthNames
    {
      get { return mMonthNames; }
    }

    public int MonthNumber
    {
      get
      {
        int ix = Array.IndexOf<string>(mMonthNames, this.Value);
        if (ix >= 0)
          return ix + 1;
        else
          return 0;
      }
      set
      {
        int xx = value - 1;
        if ((xx >= 0) && (xx <= 11))
          this.Value = mMonthNames[xx];
      }
    }
  }
}
