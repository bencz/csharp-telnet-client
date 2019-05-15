using AutoCoder.Composite;
using AutoCoder.Ext.System;
using AutoCoder.Ext.System.Data;
using AutoCoder.Ext.System.Xml;
using AutoCoder.Ext.Windows;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Xml;

namespace tnClient.Windows
{
  /// <summary>
  /// Interaction logic for CaptureViewerWindow.xaml
  /// </summary>
  public partial class CaptureViewerWindow : Window
  {
    public CaptureViewerWindow()
    {
      InitializeComponent();
      this.RunLog = new ObservableCollection<object>();

      this.Closed += MainWindow_Closed;
      this.Loaded += MainWindow_Loaded;
    }

    public ObservableCollection<object> RunLog
    {
      get { return (ObservableCollection<object>)GetValue(RunLogProperty); }
      set { SetValue(RunLogProperty, value); }
    }

    public static readonly DependencyProperty RunLogProperty =
    DependencyProperty.Register("RunLog", typeof(ObservableCollection<object>),
    typeof(CaptureViewerWindow), new PropertyMetadata(null));


    public string CaptureFolderPath
    {
      get { return (string)GetValue(CaptureFolderPathProperty); }
      set { SetValue(CaptureFolderPathProperty, value); }
    }

    public static readonly DependencyProperty CaptureFolderPathProperty =
    DependencyProperty.Register("CaptureFolderPath", typeof(string),
    typeof(CaptureViewerWindow), new PropertyMetadata("", OnCaptureFolderPathChanged));

    private static void OnCaptureFolderPathChanged(
      DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      var window = d as CaptureViewerWindow;
      window.FillRunlog(e.NewValue as string);
    }

    public string SelectedFileContent
    {
      get { return (string)GetValue(SelectedFileContentProperty); }
      set { SetValue(SelectedFileContentProperty, value); }
    }

    public static readonly DependencyProperty SelectedFileContentProperty =
    DependencyProperty.Register("SelectedFileContent", typeof(string),
    typeof(CaptureViewerWindow), new PropertyMetadata(""));


    public DataTable CaptureDataAsDataTable
    {
      get { return (DataTable)GetValue(CaptureDataAsDataTableProperty); }
      set { SetValue(CaptureDataAsDataTableProperty, value); }
    }

    public static readonly DependencyProperty CaptureDataAsDataTableProperty =
    DependencyProperty.Register("CaptureDataAsDataTable", typeof(DataTable),
    typeof(CaptureViewerWindow), new PropertyMetadata(null));


    private void MainWindow_Loaded(object sender, RoutedEventArgs e)
    {
      LayoutRoot.DataContext = this;
      lbLog.PreviewMouseLeftButtonDown += LbLog_PreviewMouseLeftButtonDown;
      lbLog.PreviewMouseMove += LbLog_PreviewMouseMove;
    }

    private void FillRunlog(string captureFolderPath)
    {
      this.RunLog.Clear();

      var dirFiles = Directory.GetFiles(captureFolderPath);
      foreach (string filePath in dirFiles)
      {
        var fa = System.IO.File.GetAttributes(filePath);
        var createDateTime = System.IO.File.GetCreationTime(filePath);

        var fileInfo = new
        {
          fileName = System.IO.Path.GetFileNameWithoutExtension(filePath),
          crtDate = createDateTime.ToString("yyyy-MM-dd") + " " +
                    createDateTime.ToString("hh:mm"),
          filePath,
          createDateTime
        };
        this.RunLog.Add(fileInfo);
      }
    }

    private void MainWindow_Closed(object sender, EventArgs e)
    {
    }

    private void MenuItem_Click(object sender, RoutedEventArgs e)
    {
      string itemText = null;
      if (sender is MenuItem)
        itemText = (sender as MenuItem).Header as string;

      if (itemText == "Test")
      {
        var rand = new Random(DateTime.Now.Millisecond);
        this.RunLog.Add("random number " + rand.Next(99));
      }

      else if (itemText == "Exit")
      {
        this.Close();
      }
    }

    private void lbLog_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
      var filePath = lbLog.SelectedItem.GetPropertyValue_String("filePath");
      if (filePath != null)
      {
        var htmlText = System.IO.File.ReadAllText(filePath);

        this.CaptureDataAsDataTable = htmlText.HtmlTextToDataTable( );
        var listObject = this.CaptureDataAsDataTable.ToListPropertiedObject();

        var lv = listObject.ToUIElement();
        this.TabItem2.Content = lv;

        var xamlText = AutoCoder.Html.HtmlParser.HtmlToXamlConverter.ConvertHtmlToXaml(htmlText, true);
        var flowDoc = XamlReader.Parse(xamlText) as FlowDocument;
        if (flowDoc != null)
          rtb1.Document = flowDoc;
      }
    }

    private void LbLog_PreviewMouseMove(object sender, MouseEventArgs e)
    {
      // Get the current mouse position
      Point mousePos = e.GetPosition(null);
      Vector diff = this.StartPoint - mousePos;

      if ((e.LeftButton == MouseButtonState.Pressed) &&
        (this.IsDragging == false) &&
        (diff.ExceedsMinimumDragDistance() == true))
      {
        StartDrag(e);
      }
    }

    void StartDrag(MouseEventArgs e)
    {
      this.IsDragging = true;

      var filePath = lbLog.SelectedItem.GetPropertyValue_String("filePath");
      if (filePath != null)
      {
        var htmlText = System.IO.File.ReadAllText(filePath);

        DataObject data = new DataObject("Text", htmlText);
        DragDropEffects de = DragDrop.DoDragDrop(lbLog, data, DragDropEffects.Move);
      }
    }

    /// <summary>
    /// start of a new drag process. But wait until mouse has moved a little bit
    /// before start the drag of a data object.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void LbLog_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
      this.StartPoint = e.GetPosition(null);
      this.IsDragging = false;
    }

    bool IsDragging { get; set; }
    Point StartPoint { get; set; }
  }
}

