using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows;

namespace AutoCoder.Ext
{
  public static class UIElementCollectionExt
  {

    /// <summary>
    /// Remove elements from the UIElementCollection which have a matching
    /// Tag string.
    /// </summary>
    /// <param name="ElemColl"></param>
    /// <param name="TagName"></param>
    public static void RemoveByTag(
      this UIElementCollection ElemColl, string TagName)
    {

      // build list of elements to remove.
      List<FrameworkElement> removeElements = new List<FrameworkElement>();
      foreach (UIElement elem in ElemColl)
      {
        if (elem is FrameworkElement)
        {
          FrameworkElement fwe = elem as FrameworkElement;
          if ((fwe.Tag != null) && (fwe.Tag is string))
          {
            string tabText = fwe.Tag as string;
            if (tabText == TagName)
            {
              removeElements.Add(fwe);
            }
          }
        }
      }

      // remove the elements.
      foreach(FrameworkElement elem in removeElements)
      {
        ElemColl.Remove(elem) ;
      }
    }
  }
}
