using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows;
using System.Windows.Media;

namespace AutoCoder.Ext.Windows.Input
{
  public static class MouseButtonEventArgsExt
  {

    /// <summary>
    /// Return the TreeViewItem which is the original source of the mouse button event.
    /// ( Most commonly, this original source TreeViewItem is the item which contains 
    /// the control that was clicked on. ( a TextBlock. )
    /// </summary>
    /// <param name="Args"></param>
    /// <returns></returns>
    public static TreeViewItem OriginalSourceTreeViewItem(this MouseButtonEventArgs Args)
    {
      TreeViewItem origItem = null;

      var child = Args.OriginalSource as DependencyObject;
      while ((child != null) && ((child is TreeViewItem) == false))
      {
        child = VisualTreeHelper.GetParent(child);
      }

      if (child != null)
      {
        origItem = child as TreeViewItem;
      }
      return origItem;
    }
  }
}
