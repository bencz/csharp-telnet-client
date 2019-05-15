using ScreenDefnLib.Defn;
using ScreenDefnLib.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace ScreenDefnLib.Converters
{
  public class ScreenDefnToScreenDefnModelConverter : IValueConverter
  {
    /// <summary>
    /// convert from ScreenDefn to ScreenDefnModel
    /// </summary>
    /// <param name="value"></param>
    /// <param name="targetType"></param>
    /// <param name="parameter"></param>
    /// <param name="culture"></param>
    /// <returns></returns>
    public object Convert(object value, Type targetType,
        object parameter, CultureInfo culture)
    {
      ScreenDefnModel model = null;
      var screenDefn = value as ScreenDefn;
      if (screenDefn == null)
        model = null;
//        model = new ScreenDefnModel();
      else
        model = new ScreenDefnModel(screenDefn);
      return model;
    }

    /// <summary>
    /// convert from ScreenDefnModel to ScreenDefn.
    /// </summary>
    /// <param name="value"></param>
    /// <param name="targetType"></param>
    /// <param name="parameter"></param>
    /// <param name="culture"></param>
    /// <returns></returns>
    public object ConvertBack(object value, Type targetType,
        object parameter, CultureInfo culture)
    {
      var model = value as ScreenDefnModel;
      var defn = new ScreenDefn(model);
      return defn;
    }
  }
}
