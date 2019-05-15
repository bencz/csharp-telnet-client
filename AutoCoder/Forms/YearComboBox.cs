using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using AutoCoder.Text;

namespace AutoCoder.Forms
{
  public class YearComboBox : IntComboBox
  {
    string[] mYearValues = null;

    public YearComboBox( int InFromYear, int InToYear)
    {
      this.DropDownStyle = ComboBoxStyle.DropDown;
      SetYearRange(InFromYear, InToYear);
      Value = InFromYear;
    }

    public string[] YearValues
    {
      get { return mYearValues; }
      set 
      {
        mYearValues = value;
        this.Items.AddRange(mYearValues);
      }
    }

    public void SetYearRange(int InFromYear, int InToYear)
    {
      int sx = InToYear - InFromYear + 1;
      string[] values = new string[sx];
      for (int ix = 0; ix < sx; ++ix)
      {
        int yr = InFromYear + ix;
        values[ix] = yr.ToString();
      }
      YearValues = values;
    }


  }
}
