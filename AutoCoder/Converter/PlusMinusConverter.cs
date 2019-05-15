using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.Windows;
using System.Globalization;
using AutoCoder.Text ;
using AutoCoder.Ext.System;

namespace AutoCoder.Converter
{
  [ValueConversion(typeof(bool), typeof(string))]
  public sealed class PlusMinusConverter : IValueConverter
  {
    public object Convert(
      object value, Type targetType, 
      object parameter, CultureInfo culture)
    {
      bool bv = false;

      if (value is bool)
      {
        bv = (bool)value;
      }

      else if (value is string)
      {
        string sv = (string)value;
        if (sv.IsNullOrEmpty())
          bv = false;
        else
          bv = true;
      }

      else if (value is int)
      {
        int iv = (int)value;
        if (iv == 0)
          bv = false;
        else
          bv = true;
      }

      // first, invert the value.
      if ((parameter != null) && ((string)parameter == "invert"))
        bv = !bv;

      if (bv == true)
        return "+";
      else
        return "-";
    }

    public object ConvertBack(
      object value, Type targetType, object parameter, CultureInfo culture)
    {
      throw new NotSupportedException(
        "VisibilityConverter.ConvertBack is not supported.");
    }
  }
}
