using AutoCoder.Enums;
using AutoCoder.Ext;
using AutoCoder.Ext.System.Collections.Generic;
using AutoCoder.Ext.Windows.Controls;
using AutoCoder.Ext.Windows.Input;
using ScreenDefnLib.Defn;
using ScreenDefnLib.CopyPaste;
using ScreenDefnLib.Models;
using ScreenDefnLib.Windows;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using ScreenDefnLib.Common;
using AutoCoder.Ext.Windows;
using System.Diagnostics;

namespace ScreenDefnLib.Controls
{
  /// <summary>
  /// Interaction logic for SectionItemsControl.xaml
  /// </summary>
  public partial class SectionItemsControl : UserControl
  {
    /// <summary>
    /// list the IScreenItems in a ListBox. Display the items in the ListBox list
    /// of items using DataTemplates that vary based on the Type of the item.
    /// The user of the control sets the DataContext property to the list of
    /// IScreenItems.
    /// </summary>
    public SectionItemsControl()
    {
      InitializeComponent();
      this.Loaded += SectionItemsControl_Loaded;
    }

    private void SectionItemsControl_Loaded(object sender, RoutedEventArgs e)
    {
      lbScreenItems.PreviewMouseLeftButtonDown += LbScreenItems_PreviewMouseLeftButtonDown;
      lbScreenItems.PreviewMouseMove += LbScreenItems_PreviewMouseMove;
      lbScreenItems.PreviewMouseLeftButtonUp += LbScreenItems_PreviewMouseLeftButtonUp;
    }

    /// <summary>
    /// bind the ISectionHeader of the screenDefn or screenSection to this
    /// property. This control then uses the Items property of ISectionHeader
    /// as the items source of the ListBox that shows the screen items.
    /// </summary>
    public ISectionHeader SectionHeader
    {
      get { return (ISectionHeader)GetValue(SectionHeaderProperty); }
      set { SetValue(SectionHeaderProperty, value); }
    }

    public static readonly DependencyProperty SectionHeaderProperty =
    DependencyProperty.Register("SectionHeader", typeof(ISectionHeader),
    typeof(SectionItemsControl), new PropertyMetadata(OnSectionHeaderPropertyChanged));

    private static void OnSectionHeaderPropertyChanged(
       DependencyObject sender, DependencyPropertyChangedEventArgs e)
    {
      var defnControl = sender as SectionItemsControl;
      var newValue = e.NewValue as ISectionHeader;
      var oldValue = e.OldValue as ISectionHeader;

      if (newValue == null)
        defnControl.LayoutRoot.DataContext = null;
      else
        defnControl.LayoutRoot.DataContext = newValue.Items;
    }

    private void lbScreenItems_PreviewKeyDown(object sender, KeyEventArgs e)
    {
      if (e.Key == Key.Enter)
      {
        var lv = sender as ListBox;
        ListBox_ChangeSelectedItem(lv);
      }
      else if (e.Key == Key.Delete)
      {
        lbScreenItems_DeleteSelected();
        e.Handled = true;
      }
      else if (e.Key == Key.Escape)
      {
        ScreenDefnGlobal.CopyPasteList.Clear();

        this.IsDragging = false;
        this.DragItemIndex = null;
        this.DragSectionHeader = null;
        this.PopupWindow.IsOpen = false;
        this.StartPoint = null;
      }
      else if (KeyboardExt.IsControlDown())
      {
        if (e.Key == Key.X)
        {
          e.Handled = CutSelected();
        }
        else if (e.Key == Key.C)
        {
          CopySelected();
          e.Handled = true;
        }
        else if (e.Key == Key.V)
        {
          DoPaste();
          e.Handled = true;
        }

        // shift arrow down. Move the item down one slot. After the next item.
        else if ((e.Key == Key.Down) && (KeyboardExt.IsControlDown() == true))
        {
          var moveModel = lbScreenItems.SelectedItem as IScreenItem;
          if (moveModel != null)
          {
            MoveDown(moveModel);
            lbScreenItems.FindFocusItem(moveModel);
            e.Handled = true;
          }
        }

        // shift arrow up. Move the item up one slot. Before the prev item.
        else if ((e.Key == Key.Up) && (KeyboardExt.IsControlDown() == true))
        {
          var moveModel = lbScreenItems.SelectedItem as IScreenItem;
          if (moveModel != null)
          {
            MoveUp(moveModel);
            lbScreenItems.FindFocusItem(moveModel);
            e.Handled = true;
          }
        }
      }
    }
    private void lbScreenItems_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
      // the left click is a double click.
      if (e.ClickCount == 2)
      {
        var lb = sender as ListBox;
        HitTestResult ht =
          VisualTreeHelper.HitTest(lb, e.GetPosition(lb));
        if (ht != null)
        {
          // search up the visual tree to the ListBoxItem
          var lbItem = FindParent<ListBoxItem>(ht.VisualHit);
          if (lbItem != null)
          {
            e.Handled = ListBox_ChangeItem(lbItem.Content as ScreenItemModel);
          }
        }
      }
    }
    void ListBox_ChangeSelectedItem(ListBox ListBox)
    {
      var itemModel = ListBox.SelectedItem as ScreenItemModel;
      if (itemModel != null)
      {
        ListBox_ChangeItem(itemModel);
      }
    }

    bool ListBox_ChangeItem(ScreenItemModel item)
    {
      bool handled = false;
      if ( item != null)
      {
        var sectionHeader = item.SectionHeader;
        WorkScreenItemWindow.WorkScreenItem(ActionCode.Change, sectionHeader, item);
        handled = true;
      }
      return handled;
    }

    private void lbScreenItems_DeleteSelected()
    {
      int selectedItemIx = -1;

      // save list of selected items.
      var selectedItems = new List<ScreenItemModel>();
      foreach (var item in lbScreenItems.SelectedItems)
      {
        var itemModel = item as ScreenItemModel;
        selectedItems.Add(itemModel);
      }

      selectedItemIx = lbScreenItems.SelectedIndex;

      // now delete from list of items.
      foreach (var itemModel in selectedItems)
      {
        this.SectionHeader.RemoveItem(itemModel);
      }

      if (selectedItemIx != -1)
      {
        lbScreenItems.SelectedIndex = selectedItemIx;

        var selectedItem = lbScreenItems.SelectedItem;
        var uiElement = ((UIElement)this.lbScreenItems.ItemContainerGenerator.ContainerFromItem(selectedItem));
        if (uiElement != null)
          uiElement.Focus();
      }

      this.SectionHeader.OnSectionHeaderChanged();
    }

    private void MenuItem_Click(object sender, RoutedEventArgs e)
    {
      MenuItem mi = null;
      if (sender is MenuItem)
        mi = sender as MenuItem;

      if (mi != null)
      {
        var itemText = mi.Header as string;
        if (itemText == "Delete")
        {
          var model = lbScreenItems.SelectedItem as ScreenItemModel;
          if (model != null)
          {
            var ix = this.SectionHeader.ItemIndexOf(model);
            this.SectionHeader.RemoveItem(model);
            this.SectionHeader.OnSectionHeaderChanged();
          }
        }

        else if (itemText == "Change")
        {
          ListBox_ChangeSelectedItem(lbScreenItems);
          this.SectionHeader.OnSectionHeaderChanged();
        }

        else if ( itemText == "Insert")
        {
          // create the new screen item.
          var model = new ScreenLiteralModel();

          // insert after the selected item.
          var insertBase = lbScreenItems.SelectedItem as ScreenItemModel;
          if ( insertBase != null)
          {

            // insert after the selected item
            var ix = this.SectionHeader.ItemIndexOf(insertBase);
            this.SectionHeader.InsertItemAfter(ix, model);
            this.SectionHeader.OnSectionHeaderChanged();
          }

          // no selected item. ( list could be empty ). Add to end of list.
          else
          {
            this.SectionHeader.AddItem(model);
          }

          // run the work with window to edit the just inserted item.
          lbScreenItems.SelectedItem = model;
            ListBox_ChangeSelectedItem(lbScreenItems);
        }

        else if ( itemText == "Move up")
        {
          var moveModel = lbScreenItems.SelectedItem as IScreenItem;
          if (moveModel != null)
          {
            var ix = lbScreenItems.SelectedIndex;
            ScreenDefnGlobal.CopyPasteList.AddCut(moveModel);
            ScreenDefnGlobal.CopyPasteList.PasteAfter(this.SectionHeader, ix - 2);
            this.SectionHeader.OnSectionHeaderChanged();
            lbScreenItems.FindFocusItem(moveModel);
          }
        }

        else if (itemText == "Move down")
        {
          var moveModel = lbScreenItems.SelectedItem as IScreenItem;
          if (moveModel != null)
          {
            MoveDown(moveModel);
            lbScreenItems.FindFocusItem(moveModel);
          }
        }

        else if ( itemText == "Cut")
        {
          CutSelected();
        }

        else if (itemText == "Copy")
        {
          CopySelected();
        }

        else if (itemText == "Paste")
        {
          DoPaste();
        }
      }
    }

    private void MoveDown(IScreenItem moveModel)
    {
      moveModel.MoveDown();
      moveModel.SectionHeader.OnSectionHeaderChanged();
    }
    private void MoveUp(IScreenItem moveModel)
    {
      moveModel.MoveUp();
      moveModel.SectionHeader.OnSectionHeaderChanged();
    }

    private void DoPaste()
    {
      var ix = lbScreenItems.SelectedIndex;

      // no selected item.  insert at end of list.
      if ( ix == -1 )
      {
        var lastPasteItem =
          ScreenDefnGlobal.CopyPasteList.PasteBottom(this.SectionHeader);
        lbScreenItems.FindFocusItem(lastPasteItem);
      }

      else
      {

        // first check the screen item. If a section and the section is
        // expanded, paste at the top of the list of items of the section. 
        // Otherwise, paste after the selected item as usual.
        var sectModel = lbScreenItems.Items[ix] as ScreenSectionModel;
        if (( sectModel != null) && ( sectModel.IsExpanded == true ))
        {
          var lastPasteItem =
            ScreenDefnGlobal.CopyPasteList.PasteTop(sectModel);
          lbScreenItems.FindFocusItem(lastPasteItem);
        }
        else
        {
          var lastPasteItem =
            ScreenDefnGlobal.CopyPasteList.PasteAfter(this.SectionHeader, ix);
          lbScreenItems.FindFocusItem(lastPasteItem);
        }

        this.SectionHeader.OnSectionHeaderChanged();
      }
    }

    private void CopySelected()
    {
      var items = lbScreenItems.SelectedItems;
      foreach (var item in items.OfType<IScreenItem>())
      {
        ScreenDefnGlobal.CopyPasteList.AddCopy(this.SectionHeader, item);
      }
      lbScreenItems.UnselectAll();
    }

    private bool CutSelected()
    {
      bool handled = false;
      var items = lbScreenItems.SelectedItems;
      foreach( var item in items.OfType<IScreenItem>( ))
      {
        ScreenDefnGlobal.CopyPasteList.AddCut(item);
        handled = true;
      }
      lbScreenItems.UnselectAll();
      return handled;
    }
  }
}
