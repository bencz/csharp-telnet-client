using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace AutoCoder.Ext.Windows.Controls
{
  public static class ItemsControlExt
  {
    /// <summary>
    /// Find the named control within the DataTemplate of the container of an item
    /// within the ItemsControl.
    /// </summary>
    /// <param name="ItemsControl"></param>
    /// <param name="Item"></param>
    /// <param name="ChildControlName"></param>
    /// <returns></returns>
    public static FrameworkElement FindItemTemplateChild(
      this ItemsControl ItemsControl, object Item, string ChildControlName)
    {
      FrameworkElement found = null;
      var container = ItemsControl.ItemContainerGenerator.ContainerFromItem(
        Item) as FrameworkElement;
      if (container != null)
      {
        found = ItemsControl.ItemTemplate.FindName(ChildControlName, container) as FrameworkElement;
      }
      return found;
    }
  }
}
