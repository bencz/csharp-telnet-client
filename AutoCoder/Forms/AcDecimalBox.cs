using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using AutoCoder.Text;

namespace AutoCoder.Forms
{
  public class AcDecimalBox : TextBox 
  {
    Nullable<decimal> mGotFocusValue = null;
    string mValuesDelim = null;

    public AcDecimalBox()
      : base()
    {
      this.GotFocus += new EventHandler(AcDecimalBox_GotFocus);
    }

    public decimal GotFocusValue
    {
      get
      {
        if ( mGotFocusValue == null )
          return 0;
        else
          return mGotFocusValue.Value;
      }
    }

    public bool ValueChangedSinceGotFocus
    {
      get
      {
        bool bChanged = false;
        if ((mGotFocusValue == null) && (this.Text == null))
          bChanged = false;
        else if (mGotFocusValue == null)
          bChanged = true;
        else if (this.Text == null)
          bChanged = true;
        else if (mGotFocusValue != this.Value)
          bChanged = true;
        else
          bChanged = false;
        return bChanged;
      }
    }

    public decimal Value
    {
      get
      {
        string textValue = this.Text;
        decimal decValue = Decimal.Parse( Stringer.ZeroIfBlank(textValue));
        return decValue;
      }
      set
      {
        string textValue = value.ToString();
        this.Text = textValue;
      }
    }

    public decimal[] Values
    {
      get
      {
        decimal[] values = null;
        if (mValuesDelim == null)
          mValuesDelim = " ";
        int fx = 0;
        fx = this.Text.IndexOf(mValuesDelim, fx);
        string[] parts = Stringer.Split(this.Text, mValuesDelim);
        values = new decimal[parts.Length];
        int ix = -1 ;
        foreach (string part in parts)
        {
          ++ix;
          values[ix] = Decimal.Parse(Stringer.ZeroIfBlank(part));
        }
        return values;
      }
      set
      {
        if (mValuesDelim == null)
          mValuesDelim = " ";
        StringBuilder sb = new StringBuilder( ) ;
        foreach( decimal v1 in value )
        {
          if ( sb.Length > 0 )
            sb.Append( mValuesDelim ) ;
          sb.Append( v1.ToString( )) ;
        }
        this.Text = sb.ToString( ) ;
      }
    }

    public string ValuesDelim
    {
      get { return mValuesDelim; }
      set { mValuesDelim = value; }
    }

    void AcDecimalBox_GotFocus(object sender, EventArgs e)
    {
      mGotFocusValue = this.Value;
    }
  }
}
