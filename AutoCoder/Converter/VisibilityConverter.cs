using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.Windows;
using AutoCoder.Ext;
using AutoCoder.Ext.System;
using AutoCoder.Text;
using System.Globalization;

namespace AutoCoder.Converter
{

  /// <summary>
  /// a flexible class which converts a value to the Visibility enum.
  /// Inputs can be bool, string or int.
  /// </summary>
  [ValueConversion(typeof(string), typeof(Visibility))]
  public sealed class VisibilityConverter : IValueConverter
  {
    public object Convert(
      object value, Type targetType,
      object parameter, CultureInfo culture)
    {

      if (value == null)
      {
        return Visibility.Collapsed;
      }

      // a boolean. vibility is true or false.
      else if (value is bool)
      {
        bool bv = (bool)value;
        if (bv == true)
          return Visibility.Visible;
        else
          return Visibility.Collapsed;
      }

      // a string. visibilty based on if string is empty.
      else if (value is string)
      {
        string sv = (string)value;
        if (sv.IsNullOrEmpty())
          return Visibility.Collapsed;
        else
          return Visibility.Visible;
      }

      //   int.  zero or not zero.
      else if (value is int)
      {
        int iv = (int)value;
        if (iv == 0)
          return Visibility.Visible;
        else
          return Visibility.Collapsed;
      }

      // unsupported.
      else
      {
        throw new ApplicationException("unsupported VisibilityConverter type.");
      }
    }

    public object ConvertBack(
      object value, Type targetType, object parameter, CultureInfo culture)
    {
      throw new NotSupportedException(
        "VisibilityConverter.ConvertBack is not supported.");
    }
  }
}
