using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using ScreenDefnLib.Defn;
using ScreenDefnLib.Models;
using AutoCoder.Telnet.IBM5250.WorkstationCommands;
using System.Diagnostics;
using AutoCoder.Telnet.Threads;
using AutoCoder.Enums;
using System.ComponentModel;
using AutoCoder.Ext.Windows.Controls;

namespace ScreenDefnLib.Controls
{
  /// <summary>
  /// Interaction logic for ScreenDefnCollectionControl.xaml
  /// 
  /// this control is the content of the ScreenDefn tab of tnClient MainWindow.
  /// The ScreenDefnObservableList property originates in the ClientModel.ToModel
  /// method. That is called when the tnClient MainWindow is created. The screen defn
  /// list is created from the ScreenDefnList stored in the settings xml file.
  /// </summary>
  public partial class ScreenDefnCollectionControl : UserControl, INotifyPropertyChanged
  {
    public ObservableCollection<IScreenDefn> ScreenDefnObservableList
    {
      get { return (ObservableCollection<IScreenDefn>)GetValue(ScreenDefnObservableListProperty); }
      set { SetValue(ScreenDefnObservableListProperty, value); }
    }

    // Using a DependencyProperty as the backing store for ScreenDefnObservableList.  This enables animation, styling, binding, etc...
    public static readonly DependencyProperty ScreenDefnObservableListProperty =
        DependencyProperty.Register("ScreenDefnObservableList", typeof(ObservableCollection<IScreenDefn>), 
          typeof(ScreenDefnCollectionControl), new PropertyMetadata(null));

    /// <summary>
    /// see tnClient. This control is placed in one of the tabs of tab control. the
    /// MasterThread of the tnClient is bound to this property. The control passes the
    /// reference onto the ScreenDefnControl. That control then uses that property
    /// when importing the defn of the current screen.
    /// </summary>
    public IThreadBase MasterThread
    {
      get { return (IThreadBase)GetValue(MasterThreadProperty); }
      set { SetValue(MasterThreadProperty, value); }
    }

    public static readonly DependencyProperty MasterThreadProperty =
    DependencyProperty.Register("MasterThread", typeof(IThreadBase),
    typeof(ScreenDefnCollectionControl), new PropertyMetadata(null));


    /// <summary>
    /// the currently selected ScreenDefn from the left side ListView that lists
    /// all of the screens. When a screen is selected that screen defn is loaded
    /// into a ScreenDefnModel. The right side listview then binds to this
    /// "current screen defn". When 
    /// </summary>
    public IScreenDefn CurrentScreenDefn
    {
      get { return _CurrentScreenDefn; }
      set
      {
        if (_CurrentScreenDefn != value)
        {
          // apply changes to the current screenDefn back to the left hand
          // side list of screens.
          ScreenDefnControl_ModelChanged(this.CurrentScreenDefn);

          _CurrentScreenDefn = value;
          RaisePropertyChanged("CurrentScreenDefn");
        }
      }
    }
    private IScreenDefn _CurrentScreenDefn;


    public ScreenDefnCollectionControl()
    {
      InitializeComponent();
      LayoutRoot.DataContext = this;

      lvScreenDefn.Items.SortDescriptions.Clear();
      lvScreenDefn.Items.SortDescriptions.Add(
        new SortDescription("NamespaceName", ListSortDirection.Ascending));
      lvScreenDefn.Items.SortDescriptions.Add(
        new SortDescription("ScreenName", ListSortDirection.Ascending));

      this.Loaded += ScreenDefnCollectionControl_Loaded;
    }

    private void ScreenDefnCollectionControl_Loaded(object sender, RoutedEventArgs e)
    {
      // control startup. select the first screen. This will display its contents
      // in the right side listbox.
      if (( lvScreenDefn.SelectedItem == null) 
        && ( this.ScreenDefnObservableList != null)
        && ( this.ScreenDefnObservableList.Count > 0))
      {
        lvScreenDefn.SelectedIndex = 0;
      }

      // hook system event that is signalled when the window that contains this
      // control is closing. 
      var currentWindow = Window.GetWindow(this);
      currentWindow.Closing += CurrentWindow_Closing;
    }

    /// <summary>
    /// the window the contains this control is closing. process changes to the
    /// current screenDefn.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void CurrentWindow_Closing(object sender, CancelEventArgs e)
    {
      ScreenDefnControl_ModelChanged(this.CurrentScreenDefn);
    }

    private void butAdd_Click(object sender, RoutedEventArgs e)
    {
      // prompt for screen name and namespace to add.
      var model = new WorkScreenDefnModel(ActionCode.Add);
      var wdw = new AddScreenDefnWindow();
      wdw.WorkScreenDefnModel = model;
      var rv = wdw.ShowDialog();

      // entry completed. add new screenDefn.
      if (rv == true)
      {
        var defn = model.ToScreenDefn();
        this.ScreenDefnObservableList.Add(defn);
      }
    }

    private void lvScreenDefn_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
      var lv = sender as ListView;
      var screenDefn = lv.SelectedItem as IScreenDefn;
      if ( screenDefn != null)
      {
        this.CurrentScreenDefn = new ScreenDefnModel(screenDefn);
      }
    }

    /// <summary>
    /// see the ModelChanged event of the ScreenDefnControl control. 
    /// When the Model property of ScreenDefnControl changes, the code behind of that control
    /// signals its ModelChanged event. This collection control monitors that event. When the
    /// model changes ( item is clicked in the listview of screen defn models. ) want to 
    /// apply changes back to the list of screen definitions.
    /// </summary>
    /// <param name="obj"></param>
    private void ScreenDefnControl_ModelChanged(IScreenDefn obj)
    {
      if (obj != null)
      {
        var item = this.ScreenDefnObservableList.FirstOrDefault(c => c.ScreenName == obj.ScreenName);
        if (item != null)
        {
          var ix = this.ScreenDefnObservableList.IndexOf(item);
          var defn = new ScreenDefn(obj);
          this.ScreenDefnObservableList.Insert(ix, defn);
          this.ScreenDefnObservableList.Remove(item);
        }
      }
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
          var model = lvScreenDefn.SelectedItem as IScreenDefn;
          if ( model != null )
          {
          var ix = this.ScreenDefnObservableList.IndexOf(model);
          this.ScreenDefnObservableList.Remove(model);
          }
        }

        else if (itemText == "Change")
        {
          // the item source of the list view is ScreenDefnObservableList
          var defn = lvScreenDefn.SelectedItem as IScreenDefn;
          if (defn != null)
          {
            var ix = this.ScreenDefnObservableList.IndexOf(defn);

            // prompt for screen name and namespace to add.
            var wdw = new AddScreenDefnWindow();
            wdw.WorkScreenDefnModel = 
              new WorkScreenDefnModel(ActionCode.Change, defn);
            var rv = wdw.ShowDialog();

            // entry completed. add new screenDefn.
            if (rv == true)
            {
              this.ScreenDefnObservableList.Remove(defn);
              var addDefn = wdw.WorkScreenDefnModel.ToScreenDefn();
              this.ScreenDefnObservableList.Add(addDefn);

              lvScreenDefn.FindFocusItem(addDefn);
            }
          }
        }
      }
    }

    private void lvScreenDefn_MouseDoubleClick(object sender, MouseButtonEventArgs e)
    {
      var defn = lvScreenDefn.SelectedItem as ScreenDefn;
      if (defn != null)
      {
        var ix = this.ScreenDefnObservableList.IndexOf(defn);

        // prompt for screen name and namespace to add.
        var wdw = new AddScreenDefnWindow();
        wdw.WorkScreenDefnModel = new WorkScreenDefnModel(ActionCode.Change, defn);
        var rv = wdw.ShowDialog();

        // entry completed. add new screenDefn.
        if (rv == true)
        {
          this.ScreenDefnObservableList.Remove(defn);
          var addDefn = wdw.WorkScreenDefnModel.ToScreenDefn();
          this.ScreenDefnObservableList.Add(addDefn);
        }
      }

    }

    public event PropertyChangedEventHandler PropertyChanged;

    // Raise the property changed event of a specified property.
    protected void RaisePropertyChanged(string name)
    {
      PropertyChangedEventHandler handler = PropertyChanged;
      if (handler != null)
      {
        handler(this, new PropertyChangedEventArgs(name));
      }
    }
  }
}
