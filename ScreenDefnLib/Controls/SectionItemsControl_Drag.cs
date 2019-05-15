using AutoCoder.Ext.System.Collections.Generic;
using AutoCoder.Ext.Windows;
using ScreenDefnLib.CopyPaste;
using ScreenDefnLib.Defn;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace ScreenDefnLib.Controls
{
  public partial class SectionItemsControl
  {
    Point? StartPoint { get; set; }
    bool IsDragging { get; set; }
    int? DragItemIndex { get; set; }
    ISectionHeader DragSectionHeader { get; set; }

    private void LbScreenItems_PreviewMouseMove(object sender, MouseEventArgs e)
    {
      if ((e.LeftButton == MouseButtonState.Pressed) 
        && (IsDragging == false)
        && (this.StartPoint != null))
      {
        // Get the current mouse position
        Point mousePos = e.GetPosition(null);
        Vector diff = this.StartPoint.Value - mousePos;

        if (diff.ExceedsMinimumDragDistance() == true)
        {
          e.Handled = StartDrag(e);
        }
      }

      if (this.IsDragging == true)
      {
        var point = Mouse.GetPosition(LayoutRoot);
        PopupWindow.HorizontalOffset = point.X;
        PopupWindow.VerticalOffset = point.Y;
        e.Handled = true;
      }
    }

    private bool StartDrag(MouseEventArgs e)
    {
      this.DragItemIndex = null;
      this.DragSectionHeader = null;
      bool handled = false;

      // locate the visual object within the scope of the ListBox where the
      // cursor is located.
      HitTestResult ht = VisualTreeHelper.HitTest(lbScreenItems, e.GetPosition(lbScreenItems));
      if (ht != null)
      {

        // search up the visual tree to the ListBoxItem
        var lbItem = FindParent<ListBoxItem>(ht.VisualHit);
        if (lbItem != null)
        {
          // item to drag.
          var dragScreenItem = lbItem.Content as IScreenItem;
          if (dragScreenItem != null)
          {
            // the sectionHeader of the item to drag.
            var dragSectionHeader = dragScreenItem.SectionHeader;

            // find the SectionItemsControl that contains the ListBox which
            // contains this ListBoxItem. 
            //       var sic = FindParent<SectionItemsControl>(lbItem);

            // index of item to drag.
            //         var ix = sic.SectionHeader.ItemIndexOf(dragScreenItem);
            var ix = dragSectionHeader.ItemIndexOf(dragScreenItem);
            this.DragItemIndex = ix;
            this.DragSectionHeader = dragSectionHeader;

            this.IsDragging = true;

            this.PopupWindow.IsOpen = true;
            this.PopupText.Text = "Dragging. " + dragScreenItem.ToString();
            handled = true;

            Debug.WriteLine("start dragging");
          }
        }
      }
      return handled;
    }

    private void LbScreenItems_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
      this.StartPoint = e.GetPosition(null);
      this.IsDragging = false;
      this.PopupWindow.IsOpen = false;
      this.DragItemIndex = null;
    }

    private void LbScreenItems_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
    {
      // something being dragged.
      if (this.DragItemIndex != null)
      {
        ListBoxItem lbItem = null;

        // locate the visual within the scope of the ListBox. 
        // If null then the cursor is outside of the ListBox. Handle this as a 
        // delete of the item being dragged.
        HitTestResult ht = VisualTreeHelper.HitTest(lbScreenItems, e.GetPosition(lbScreenItems));
        if (ht != null)
        {
          // find the ListBoxItem located at the current mouse position.
          lbItem = FindParent<ListBoxItem>(ht.VisualHit);
        }

        // get the drag to target item.
        IScreenItem targetItem = null;
        if (lbItem != null)
        {
          targetItem = lbItem.Content as IScreenItem;
        }

        // got the drag item. And the drag to target item.
        if (targetItem != null)
        {

          // remove the item being dragged.
          var dragItem = this.DragSectionHeader.GetItemAt(this.DragItemIndex.Value);
          this.DragSectionHeader.RemoveItemAt(this.DragItemIndex.Value);

          // section header of target item.
          var targetSectionHeader = targetItem.SectionHeader;

          // target item is a section header. If the section is expanded, set to
          // insert to the end of the list of items of that section.
          var targetSection = targetItem as IScreenSection;
          if ((targetSection != null) && (targetSection.IsExpanded == true))
          {
            targetItem = null;
            targetSectionHeader = targetSection as ISectionHeader;
          }

          // insert at the end of the section.
          if (targetItem == null)
          {
            targetSectionHeader.AddItem(dragItem);
          }

          else
          {
            // index of item to insert after.
            int insertAfterIndex = targetSectionHeader.ItemIndexOf(targetItem);

            // insert after target item ( or add to end of list )
            if (insertAfterIndex >= 0)
              targetSectionHeader.InsertItemAfter(insertAfterIndex, dragItem);
          }
        }

        this.DragItemIndex = null;
        this.IsDragging = false;
        this.PopupWindow.IsOpen = false;

        e.Handled = true;
      }
    }

    /// <summary>
    /// find the parent visual of the "from" object which is of type T.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="from"></param>
    /// <returns></returns>
    static T FindParent<T>(DependencyObject from)
        where T : class
    {
      T result = null;
      DependencyObject parent = VisualTreeHelper.GetParent(from);

      if (parent is T)
        result = parent as T;
      else if (parent != null)
        result = FindParent<T>(parent);

      return result;
    }
  }
}

