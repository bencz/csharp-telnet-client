using AutoCoder.Enums;
using AutoCoder.Ext;
using AutoCoder.Ext.System;
using AutoCoder.Telnet.Common.RowCol;
using AutoCoder.Telnet.Common.ScreenLoc;
using AutoCoder.Telnet.Enums;
using AutoCoder.Telnet.IBM5250.Content;
using AutoCoder.Telnet.IBM5250.WorkstationCommands;
using AutoCoder.Telnet.IBM5250.WtdOrders;
using AutoCoder.Telnet.ThreadMessages;
using AutoCoder.Telnet.Threads;
using ScreenDefnLib.Defn;
using ScreenDefnLib.Enums;
using ScreenDefnLib.Models;
using ScreenDefnLib.Windows;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace ScreenDefnLib.Controls
{
  /// <summary>
  /// Interaction logic for ScreenDefnControl.xaml
  /// </summary>
  public partial class ScreenDefnControl : UserControl
  {

    public event Action<IScreenDefn> ModelChanged;
    public ScreenDefnModel Model
    {
      get { return (ScreenDefnModel)GetValue(ModelProperty); }
      set { SetValue(ModelProperty, value); }
    }

    // Using a DependencyProperty as the backing store for Model.  This enables animation, styling, binding, etc...
    public static readonly DependencyProperty ModelProperty =
        DependencyProperty.Register("Model", typeof(ScreenDefnModel), typeof(ScreenDefnControl), 
          new PropertyMetadata(OnModelPropertyChanged));

    public IThreadBase MasterThread
    {
      get { return (IThreadBase)GetValue(MasterThreadProperty); }
      set { SetValue(MasterThreadProperty, value); }
    }

    public static readonly DependencyProperty MasterThreadProperty =
    DependencyProperty.Register("MasterThread", typeof(IThreadBase),
    typeof(ScreenDefnControl), new PropertyMetadata(null));

    private static void OnModelPropertyChanged(
       DependencyObject sender, DependencyPropertyChangedEventArgs e)
    {
      var defnControl = sender as ScreenDefnControl;
      var newValue = e.NewValue as ScreenDefnModel;
      var oldValue = e.OldValue as ScreenDefnModel;

      defnControl.LayoutRoot.DataContext = newValue;
    }
    public ScreenDefnControl()
    {
      InitializeComponent();
      this.Loaded += ScreenDefnControl_Loaded;
    }

    private void ScreenDefnControl_Loaded(object sender, RoutedEventArgs e)
    {
    }

    /// <summary>
    /// the contents of the model have been changed. Send signal back to the 
    /// control or window that subscribes to the ModelChanged event. 
    /// ( the ScreenDefnCollectionControl subscribes to this event. )
    /// </summary>
    /// <param name="Model"></param>
    void SignalModelChanged( IScreenDefn Model )
    {
      if ((Model != null ) && (this.ModelChanged != null))
      {
        this.ModelChanged(Model);
      }
    }

    private void butAdd_Click(object sender, RoutedEventArgs e)
    {
      var item = new ScreenLiteral();
      var model = new ScreenLiteralModel(item);

      WorkScreenItemWindow.WorkScreenItem(ActionCode.Add, this.Model, model);
    }

    private void ListView_MouseDoubleClick(object sender, MouseButtonEventArgs e)
    {
      var lv = sender as ListView;
      ListView_ChangeItem(lv);
    }

    void ListView_ChangeItem(ListView ListView)
    {
      var itemModel = ListView.SelectedItem as ScreenAtomicModel;
      var window = new WorkScreenItemWindow(ActionCode.Change);
      window.ScreenItemModel = itemModel;
      var rv = window.ShowDialog();
      if ((rv != null) && (rv.Value == true))
      {
      }
    }

    private void ListBox_MouseDoubleClick(object sender, MouseButtonEventArgs e)
    {
      var lv = sender as ListView;
      ListView_ChangeItem(lv);
    }

    /// <summary>
    /// import the ContentItems of the current ScreenContent. Use the MasterThread
    /// property to send a message to the MasterThread asking for a copy of the
    /// current screenContent. That sent message waits for a response and receives
    /// the screenContent as the reply. With the screenContent, then list the 
    /// ScreenContent items and import into ScreenDefn.
    /// 
    /// Necessary for this to work, the telnetClient which is the parent of the
    /// ScreenDefnListControl must bind to the MasterThread property.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void butImport_Click(object sender, RoutedEventArgs e)
    {

      // send a message to the master thread asking for a copy of the current
      // screenContent.
      ScreenContent content = null;
      {
        var msg = new ExchangeMessage(ThreadMessageCode.GetScreenContent);
        this.MasterThread.PostInputMessage(msg);
        msg.WaitReplyEvent();
        content = msg.ReplyMessage as ScreenContent;
      }

      // create screen section. type body.
      var section = new ScreenSectionModel()
      {
        ScreenLoc = new OneScreenLoc(1,1),
        ItemName = "Body",
        ItemType = ShowItemType.Section,
        PurposeCode = ScreenPurposeCode.Body
      };
      this.Model.AddItem(section);

      int fieldNum = 0;
      int litNum = 0;
      foreach ( var citem in content.ContentItems( ))
      {
        if ( citem is ContentField)
        {
          var fld = citem as ContentField;

          {
            fieldNum += 1;
            var xx = fld.IsBypass;
            var item = new ScreenFieldModel()
            {
              ScreenLoc = new OneScreenLoc(fld.RowCol.ToOneBased()),
              ItemName = "Field" + fieldNum,
              Length = fld.LL_Length,
              ItemType = ShowItemType.Field,
              Usage = fld.Usage,
              DsplyAttr = fld.AttrByte.ToDsplyAttr()
            };
            section.AddItem(item);
          }

        }
        else if ( citem is ContentText)
        {
          var contentText = citem as ContentText;
          {
            litNum += 1;
            var item = new ScreenLiteralModel()
            {
              ScreenLoc = new OneScreenLoc(contentText.RowCol.ToOneBased()),
              ItemName = "Lit" + litNum,
              ListValues = contentText.GetShowText(content).ToListString().ToObservableCollection( ),
              Length = contentText.GetShowText(content).Length,
              ItemType = ShowItemType.Literal,
              DsplyAttr = contentText.GetAttrByte(content).ToDsplyAttr()
            };

#if skip
            // adjust screenLoc to account for neutral dsply attr.
            if (item.DsplyAttr.IsEqual(DsplyAttr.NU))
            {
              item.ScreenLoc = item.ScreenLoc.Advance(1, new AutoCoder.Telnet.Common.ScreenDm.ScreenDim(24, 80)) as OneScreenLoc;
            }
#endif

            section.AddItem(item);
          }
        }
      }
    }

    private void butClear_Click(object sender, RoutedEventArgs e)
    {
      this.Model.ClearItems();
    }

    private void butTest_Click(object sender, RoutedEventArgs e)
    {
      ISectionHeader sh = this.Model;

      // find the first ScreenSection. set to the items of that section.
      var sect = sh.Items.OfType<ScreenSectionModel>().First();
      if (sect != null)
        sh = sect;

      var found = sh.Items.FirstOrDefault(c => c.ItemName == "abc3");
      if (found != null)
        sh.RemoveItem(found);

      var item = new ScreenLiteralModel();
      item.ItemName = "abc3";
      item.ScreenLoc = new OneScreenLoc(1, 5);

      sh.AddItem(item);
    }

    private void butReport_Click(object sender, RoutedEventArgs e)
    {
      var report = (this.Model as IScreenDefn).Report();
      var allText = report.ToAllText();
      Clipboard.SetText(allText);
      MessageBox.Show("Screen definition report placed on clipboard");
    }
  }
}
