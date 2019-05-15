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
using TextCanvasLib.Common;
using TextCanvasLib.Main;
using TextCanvasLib.xml;
using TextDemo.Models;
using TextDemo.Settings;

namespace TextDemo
{
  /// <summary>
  /// Interaction logic for MainWindow.xaml
  /// </summary>
  public partial class MainWindow : Window
  {
    TestCanvas ScreenCanvas { get; set; }
    LogFile LogFile { get; set; }
    public MainModel Model
    { get; set; }

    public MainWindow()
    {
      InitializeComponent();

      this.Loaded += MainWindow_Loaded;
      this.Closed += MainWindow_Closed;
    }

    private void MainWindow_Closed(object sender, EventArgs e)
    {
      {
        var settings = TextDemoSettings.RecallSettings();

        settings.WindowClientSize =
          new Size(this.ActualWidth, this.ActualHeight);
        settings.IpAddr = this.Model.IpAddr;
        settings.UserName = this.Model.UserName;
        settings.Password = this.Model.Password;
        settings.FtpCommandRecentList = this.Model.FtpCommandRecentList;

        settings.StoreSettings();
      }
    }

    void MainWindow_Loaded(object sender, RoutedEventArgs e)
    {
      var settings = TextDemoSettings.RecallSettings();

      if (settings.WindowClientSize.Height > 0)
      {
        this.Height = settings.WindowClientSize.Height;
        this.Width = settings.WindowClientSize.Width;
      }
      this.Model = MainModel.ToModel(settings);
      grdMain.DataContext = this.Model;


      this.Canvas1.Focusable = true;
      this.Canvas1.Focus();
      this.ScreenCanvas = new TestCanvas(this.Canvas1, 9.83, 18.5, 50, 15);
      this.ScreenCanvas.ScreenExitEvent += ScreenCanvas_ScreenExitEvent;

      this.LogFile = new LogFile("c:\\downloads\\textCanvasLog.txt", LogFileAction.Clear);
      this.ScreenCanvas.LogFile = this.LogFile;
    }

    void ScreenCanvas_ScreenExitEvent(object o, EventArgs args)
    {
      this.Close();
    }

    private void MenuItem_Click(object sender, RoutedEventArgs e)
    {
      string itemText = null;
      if (sender is MenuItem)
        itemText = (sender as MenuItem).Header as string;

      if (itemText == "Test")
      {
        return;
      }

      else if (itemText == "Exit")
      {
        this.Close();
      }

      else if (itemText == "Read xml")
      {
        string xmlPath = "c:\\skydrive\\c#\\TextCanvasLib\\xmlfile1.xml";
        var items = ScreenDocReader.ReadDoc(xmlPath);
        this.ScreenCanvas.PaintScreen(items);
      }
    }

    private void Canvas1_MouseDown(object sender, MouseButtonEventArgs e)
    {
      if (this.Canvas1.IsKeyboardFocused == false)
        this.Canvas1.Focus();

      Point pos = e.GetPosition(Canvas1);
    }

    private void butOk_Click(object sender, RoutedEventArgs e)
    {
      var rc = Canvas1.Focus();
      if (rc == true)
      {
      }
    }
  }
}
