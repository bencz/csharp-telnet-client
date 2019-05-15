using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using AutoCoder.Text;

namespace AutoCoder.Forms
{
  public class IntComboBox : AcComboBox
  {

    public IntComboBox()
    {
    }

    public new int Value
    {
      get
      {
        if (base.Value.Length == 0)
          return 0;
        return Int32.Parse(base.Value);
      }
      set
      {
        base.Value = value.ToString( ) ;
      }
    }

    public new int? WasValue
    {
      get
      {
        int? wasValue = null ;
        if ( base.WasValue != null )
          wasValue = Int32.Parse(base.WasValue) ;
        return wasValue ;
      }
      set 
      {
        base.WasValue = value.ToString();
      }
    }

  }
}
