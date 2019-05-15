using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;

namespace AutoCoder.Ext.Windows.Controls
{
  public static class MenuItemExt
  {
    /// <summary>
    /// return the header of the MenuItem as a string.
    /// </summary>
    /// <param name="MenuItem"></param>
    /// <returns></returns>
    public static string HeaderText(this MenuItem MenuItem)
    {
      if (MenuItem == null)
        return "";
      else
      {
        var s1 = MenuItem.Header as string;
        if (s1 == null)
          return "";
        else
          return s1;
      }
    }

    public static string ParentHeaderText(this MenuItem MenuItem)
    {
      if (MenuItem == null)
        return "";
      else
      {
        var parentItem = MenuItem.Parent as MenuItem;
        if (parentItem == null)
          return "";
        else
        {
          var s1 = parentItem.Header as string;
          if (s1 == null)
            return "";
          else
            return s1;
        }
      }
    }
  }
}
