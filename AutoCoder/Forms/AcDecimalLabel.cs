using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using AutoCoder.Text;

namespace AutoCoder.Forms
{

  public class AcDecimalLabel : Label
  {
    public AcDecimalLabel()
      : base()
    {
    }

    public decimal Value
    {
      get
      {
        string textValue = this.Text;
        decimal decValue = Decimal.Parse(Stringer.ZeroIfBlank(textValue));
        return decValue;
      }
      set
      {
        string textValue = value.ToString();
        this.Text = textValue;
      }
    }
  }

}
