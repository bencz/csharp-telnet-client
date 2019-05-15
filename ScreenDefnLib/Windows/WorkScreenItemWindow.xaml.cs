using AutoCoder.Enums;
using AutoCoder.Ext;
using AutoCoder.Telnet.Enums;
using ScreenDefnLib.Defn;
using ScreenDefnLib.Models;
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

namespace ScreenDefnLib.Windows
{
  /// <summary>
  /// Interaction logic for AddScreenItemWindow.xaml
  /// </summary>
  public partial class WorkScreenItemWindow : Window
  {
    /// <summary>
    /// list of items already defined in the screen.
    /// </summary>
    private IEnumerable<ScreenAtomic> Items
    { get; set; }
    public ActionCode ActionCode
    {
      get { return (ActionCode)GetValue(ActionCodeProperty); }
      set { SetValue(ActionCodeProperty, value); }
    }

    public string WindowTitle
    {
      get
      {
        string s1 = "Screen Item";
        return ActionCode.ToString() + " " + s1;
      }
    }

    // Using a DependencyProperty as the backing store for ActionCode.  This enables animation, styling, binding, etc...
    public static readonly DependencyProperty ActionCodeProperty =
        DependencyProperty.Register("ActionCode", typeof(ActionCode), typeof(WorkScreenItemWindow), 
          new PropertyMetadata(ActionCode.None));

    public ScreenItemModel ScreenItemModel
    {
      get { return (ScreenItemModel)GetValue(ScreenItemModelProperty); }
      set { SetValue(ScreenItemModelProperty, value); }
    }

    // Using a DependencyProperty as the backing store for ScreenItemModel.  This enables animation, styling, binding, etc...
    public static readonly DependencyProperty ScreenItemModelProperty =
        DependencyProperty.Register("ScreenItemModel", typeof(ScreenItemModel),
          typeof(WorkScreenItemWindow),
          new FrameworkPropertyMetadata(null,
            FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

    public ScreenItemModel Model
    {
      get; set;
    }

    public bool ModelTypeChanged
    { get; set; }

    public ObservableCollection<object> ModelList
    {
      get
      {
        if (_ModelList == null)
        {
          _ModelList = new ObservableCollection<object>();
          this.ModelList.Add(Model);
        }
        return _ModelList;
      }
    }
    ObservableCollection<object> _ModelList;

    public WorkScreenItemWindow( ActionCode ActionCode)
    {
      InitializeComponent();
      this.ActionCode = ActionCode;

      this.Closing += WorkScreenItemWindow_Closing;
      this.Loaded += MainWindow_Loaded;
      this.PreviewKeyDown += WorkScreenItemWindow_PreviewKeyDown;
    }

    private void WorkScreenItemWindow_PreviewKeyDown(object sender, KeyEventArgs e)
    {
      // handle the enter key. Normally, want ENTER to be the same as clicking
      // the OK button. But if the caret is located in a multi line textbox, want
      // the enter key to function as is. Insert a newline into the textbox text.
      if ( e.Key == Key.Enter)
      {
        bool doAccept = true;
        if ( e.OriginalSource is TextBox )
        {
          var textBox = e.OriginalSource as TextBox;
          if ((textBox.Tag as string) == "hover")
            doAccept = false;
        }
        if (doAccept == true )
        {
          this.butOK.Focus();    // take focus off of the textbox
          AcceptEntry();
        }
      }
      else if ( e.Key == Key.Escape)
      {
        DialogResult = false;
      }
    }

    private void MainWindow_Loaded(object sender, RoutedEventArgs e)
    {
      this.Model = ScreenItemModel.Factory(this.ScreenItemModel);
      this.DataContext = this;

      // screenSection. Make the window larger.
      if (this.Model is IScreenSection)
      {
        this.Width += 150;
        this.Height += 250;
      }
    }

    /// <summary>
    /// use closing event to handle situation where window is closed using the 
    /// "X" in the upper right corner. Run the AcceptEntry method when the 
    /// window is closing but have not yet accepted entry.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void WorkScreenItemWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
    {
      if (EntryWasAccepted == false)
        AcceptEntry();
    }

    private void butOK_Click(object sender, RoutedEventArgs e)
    {
      AcceptEntry();
    }
    void AcceptEntry( )
    {
      if (this.ModelTypeChanged == true)
        this.ScreenItemModel = this.Model;
      else
        this.ScreenItemModel.Apply(this.Model);
      this.EntryWasAccepted = true;
      DialogResult = true;
    }
    private bool EntryWasAccepted
    { get; set; }

    private void NumericUpDown_ValueChanged(int obj)
    {
      int xx = obj;
      var ee = this.Model.MatchNum;
    }

    private void EnumRadioButton_EnumValueChanged(
      Enum wasValue, Enum currentValue, AutoCoder.Controls.EnumRadioButton arg2)
    {
      var itemType = (ShowItemType) currentValue;
      ShowItemType? wasType = (ShowItemType?)wasValue;

      // type of the ScreenItem has been changed. Create a new ItemModel that
      // matches the new item type. 
      if ((wasType != null) && (wasType.Value != itemType))
      {
        this.Model = ScreenItemModel.Factory(itemType, this.Model);

        this.ModelTypeChanged = true;
        AcceptEntry();
      }
    }

    public static void WorkScreenItem(
      ActionCode action, ISectionHeader sectionHeader, ScreenItemModel itemModel)
    {
      bool cancelAction = false;

      while (true)
      {
        var window = new WorkScreenItemWindow(action);
        window.ScreenItemModel = itemModel;
        var rv = window.ShowDialog();
        if ((rv == null) || (rv.Value == false))
        {
          cancelAction = true;
          break;
        }

        if (window.ModelTypeChanged == true)
        {
          if (action != ActionCode.Add)
          {
            sectionHeader.ReplaceModel(itemModel, window.ScreenItemModel);
            sectionHeader.OnSectionHeaderChanged();
          }
          itemModel = window.ScreenItemModel;
          continue;
        }
        else
        {
          // the selectedItem was passed by reference. On return that same reference
          // has been updated by the WorkScreenItemWindow. No need to apply to the
          // itemsSource. It is already updated.
          cancelAction = false;
          itemModel = window.ScreenItemModel;
          break;
        }
      }

      // entry accepted. Add to list of items.
      if ((cancelAction == false) && ( action == ActionCode.Add))
      {
        sectionHeader.AddItem(itemModel);
      }
    }

    private void butConvertToOutputField_Click(object sender, RoutedEventArgs e)
    {
      this.Model = ScreenItemModel.Factory(ShowItemType.Field, this.Model);
      (this.Model as ScreenFieldModel).Usage = ShowUsage.Output;
      this.ModelTypeChanged = true;
      AcceptEntry();
    }
  }
}
