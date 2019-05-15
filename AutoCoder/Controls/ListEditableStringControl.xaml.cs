using AutoCoder.Ext.System.Collections.Generic;
using AutoCoder.Ext.Windows;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace AutoCoder.Controls
{
  // rename to "EditableStringsControl".  kind of along the lines of "ItemsControl"
  /// <summary>
  /// Interaction logic for ListEditableStringControl.xaml
  /// </summary>
  public partial class ListEditableStringControl : UserControl
  {
    public ListEditableStringControl()
    {
      InitializeComponent();
      this.LineCollection = new ObservableCollection<ItemsControlTextItem>();
      this.Loaded += ListEditableStringControl_Loaded;
    }
    Point StartPoint { get; set; }
    bool IsDragging { get; set; }
    int? DragItemIndex { get; set; }

    public ObservableCollection<string> Items
    {
      get { return (ObservableCollection<string>)GetValue(ItemsProperty); }
      set { SetValue(ItemsProperty, value); }
    }

    public static readonly DependencyProperty ItemsProperty =
    DependencyProperty.Register("Items", typeof(ObservableCollection<string>),
    typeof(ListEditableStringControl), new PropertyMetadata(null, OnItemsPropertyChanged));

    /// <summary>
    /// handle the instance where something is bound to the Items property.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private static void OnItemsPropertyChanged(
       DependencyObject sender, DependencyPropertyChangedEventArgs e)
    {
      var thisControl = sender as ListEditableStringControl;

      // Items property contains some data. Load these strings into the internal
      // list which is actually bound to the items control.
      if (e.NewValue != null)
      {
        var items = e.NewValue as ObservableCollection<string>;
        thisControl.LoadLineCollection(items);
      }
    }

    public ObservableCollection<ItemsControlTextItem> LineCollection
    { get; set; }

    public int SelectedIndex
    {
      get { return (int)GetValue(SelectedIndexProperty); }
      set { SetValue(SelectedIndexProperty, value); }
    }

    public static readonly DependencyProperty SelectedIndexProperty =
    DependencyProperty.Register("SelectedIndex", typeof(int),
    typeof(ListEditableStringControl), new PropertyMetadata(0));


    private void ListEditableStringControl_Loaded(object sender, RoutedEventArgs e)
    {
      this.LayoutRoot.DataContext = this;
      ItemsControl1.PreviewMouseLeftButtonUp += ItemsControl1_PreviewMouseLeftButtonUp;
      ItemsControl1.PreviewMouseMove += ItemsControl1_PreviewMouseMove;
    }

    private void ItemsControl1_PreviewMouseMove(object sender, MouseEventArgs e)
    {
      if ((e.LeftButton == MouseButtonState.Pressed) && (IsDragging == false))
      {
        // Get the current mouse position
        Point mousePos = e.GetPosition(null);
        Vector diff = this.StartPoint - mousePos;

        if (diff.ExceedsMinimumDragDistance() == true)
        {
          StartDrag(e);
        }
      }

      // dragging is proceeding. Update the position of popup that contains image
      // of what is being dragged.
      if (this.IsDragging == true)
      {
        var point = Mouse.GetPosition(Application.Current.MainWindow);
        Popup1.HorizontalOffset = point.X;
        Popup1.VerticalOffset = point.Y;
      }
    }

    private void ItemsControl1_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
    {
      // something being dragged.
      if (this.DragItemIndex != null)
      {
        ContentPresenter lbItem = null;

        // remove the item being dragged.
        var dragItem = this.LineCollection[this.DragItemIndex.Value];
        this.LineCollection.RemoveAt(this.DragItemIndex.Value);

        // locate the visual within the scope of the ListBox. 
        HitTestResult ht = VisualTreeHelper.HitTest(ItemsControl1, e.GetPosition(ItemsControl1));

        // If null then the cursor is outside of the ListBox. Handle this as a 
        // delete of the item being dragged.
        if (ht == null)
        {
        }

        else
        {
          // find the ListBoxItem located at the current mouse position.
          if (ht != null)
            lbItem = FindParent<ContentPresenter>(ht.VisualHit);

          // index of item to insert before.
          int insertBeforeIndex = 0;
          if (lbItem != null)
            insertBeforeIndex = this.LineCollection.IndexOf(lbItem.DataContext as ItemsControlTextItem);
          else
            insertBeforeIndex = -1;

          // insert before target item ( or add to end of list )
          if (insertBeforeIndex >= 0)
            this.LineCollection.Insert(insertBeforeIndex, dragItem);
          else
            this.LineCollection.Add(dragItem);
        }

        this.DragItemIndex = null;
        this.IsDragging = false;
        this.Popup1.IsOpen = false;
      }
    }
    private void StartDrag(MouseEventArgs e)
    {
      this.DragItemIndex = null;

      // locate the visual object within the scope of the ItemsControl where the
      // cursor is located.
      HitTestResult ht = VisualTreeHelper.HitTest(ItemsControl1, e.GetPosition(ItemsControl1));
      if (ht != null)
      {

        // search up the visual tree to the ListBoxItem
        var lbItem = FindParent<ContentPresenter>(ht.VisualHit);
        if (lbItem != null)
        {
          var textItem = lbItem.DataContext as ItemsControlTextItem;

          // index of item to drag.
          var ix = this.LineCollection.IndexOf(textItem);
          this.DragItemIndex = ix;

          this.IsDragging = true;

          this.Popup1.IsOpen = true;
          this.PopupText.Text = "Dragging. " + textItem.Value;
        }
      }
    }

    private void LoadLineCollection(ObservableCollection<string> items)
    {
      if (this.collectionChangedHooked == false)
      {
        items.CollectionChanged += Items_CollectionChanged;
        this.collectionChangedHooked = true;
      }

      this.LineCollection.Clear();

      // input list of items is emtpy. add a blank line.
      if (items.Count == 0)
      {
        items.Add("");
      }

      else
      {
        foreach (var item in items)
        {
          var line = new ItemsControlTextItem(item);
          this.LineCollection.Add(line);
        }
      }
    }

    bool collectionChangedHooked { get; set; }
    private void Items_CollectionChanged(
      object sender, NotifyCollectionChangedEventArgs e)
    {
      if (e.NewItems != null)
      {
        var ix = e.NewStartingIndex;
        foreach (var item in e.NewItems)
        {
          var line = new ItemsControlTextItem(item as string);
          this.LineCollection.Insert(ix, line);
          ix += 1;
        }
      }

      if (e.OldItems != null)
      {
        var ix = e.OldStartingIndex;
        foreach (var item in e.OldItems)
        {
          this.LineCollection.RemoveAt(ix);
        }
      }
    }

    private void ContextMenu_Click(object sender, RoutedEventArgs e)
    {
      string itemText = null;
      if (sender is MenuItem)
        itemText = (sender as MenuItem).Header as string;

      if (itemText == "Insert")
      {
        this.Items.InsertAfter(this.SelectedIndex, "");
      }

      else if (itemText == "Delete")
      {
        this.Items.RemoveAt(this.SelectedIndex);
      }
    }

    private void ItemsControl1_PreviewMouseLeftButtonDown(
      object sender, MouseButtonEventArgs e)
    {
      // locate the visual object within the scope of the ListBox where the
      // cursor is located.
      HitTestResult ht =
        VisualTreeHelper.HitTest(ItemsControl1, e.GetPosition(ItemsControl1));
      if (ht != null)
      {
        // search up the visual tree to the ListBoxItem
        var cpItem = FindParent<ContentPresenter>(ht.VisualHit);
        if (cpItem != null)
        {
          var textItem = cpItem.DataContext as ItemsControlTextItem;
          if (textItem != null)
          {
            this.SelectedIndex = this.LineCollection.IndexOf(textItem);
          }
        }
      }

      // setup for possible dragging of items within the control.
      this.StartPoint = e.GetPosition(null);
      this.IsDragging = false;
      this.Popup1.IsOpen = false;
    }

    /// <summary>
    /// find the parent visual of the "from" object which is of type T.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="from"></param>
    /// <returns></returns>
    public static T FindParent<T>(DependencyObject from)
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

    private void ItemsControl1_KeyDown(object sender, KeyEventArgs e)
    {
    }

    /// <summary>
    /// textChanged event on the textbox within the datatemplate of the 
    /// items control. The Tag property is bound to the ItemsControlTextItem. Use
    /// this reference to get the index of the string in the list of strings. By
    /// the time this method is called the changed from the textbox has been 
    /// bound to the ItemsControlTextItem. Take the text value in from that 
    /// object and apply to the Items list of strings that the user of this 
    /// control has bound to the Items property.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
    {
      var tb = sender as TextBox;
      var icItem = tb.Tag as ItemsControlTextItem;
      this.SelectedIndex = this.LineCollection.IndexOf(icItem);

      // text being entered into an empty items contro.
      if ((this.SelectedIndex == 0) && (this.Items.Count == 0))
        this.Items.Add("");

      // apply the changed text back to the items collection which the user of
      // the control bound to the control.
      this.Items[this.SelectedIndex] = icItem.Value;
    }

    private void ItemsControl1_PreviewMouseRightButtonDown(object sender, MouseButtonEventArgs e)
    {
      // locate the visual object within the scope of the ListBox where the
      // cursor is located.
      HitTestResult ht =
        VisualTreeHelper.HitTest(ItemsControl1, e.GetPosition(ItemsControl1));
      if (ht != null)
      {
        // search up the visual tree to the ListBoxItem
        var cpItem = FindParent<ContentPresenter>(ht.VisualHit);
        if (cpItem != null)
        {
          var textItem = cpItem.DataContext as ItemsControlTextItem;
          if (textItem != null)
          {
            this.SelectedIndex = this.LineCollection.IndexOf(textItem);
          }
        }
      }
    }
  }

  public class ItemsControlTextItem
  {
    public string Value
    { get; set; }

    public ItemsControlTextItem(string value)
    {
      this.Value = value;
    }
  }
}

