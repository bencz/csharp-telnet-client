using AutoCoder.Ext;
using AutoCoder.Ext.System;
using AutoCoder.Print;
using AutoCoder.Systm;
using AutoCoder.Telnet.Common;
using AutoCoder.Telnet.Common.ScreenDm;
using AutoCoder.Telnet.Connect;
using AutoCoder.Telnet.Enums;
using AutoCoder.Telnet.IBM5250;
using AutoCoder.Telnet.IBM5250.Response;
using AutoCoder.Telnet.IBM5250.WorkstationCommands;
using AutoCoder.Telnet.IBM5250.WtdOrders.wtdCommon;
using Microsoft.Win32;
using System;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using TelnetTester.Common;
using TelnetTester.Models;
using TelnetTester.Settings;
using TextCanvasLib.Canvas;
using TextCanvasLib.Common;
using TextCanvasLib.Telnet;

namespace TelnetTester
{
  /// <summary>
  /// Interaction logic for MainWindow.xaml
  /// </summary>
  public partial class MainWindow : Window
  {
    ItemCanvas ScreenCanvas { get; set; }

    public MainWindowModel Model
    { get; set; }

    public MainWindow()
    {
      InitializeComponent();
      this.Closed += MainWindow_Closed;
      this.Loaded += MainWindow_Loaded;
    }

    private void MainWindow_Loaded(object sender, RoutedEventArgs e)
    {
      {
        var settings = TesterSettings.RecallSettings();

        if (settings.WindowClientSize.Height > 0)
        {
          this.Height = settings.WindowClientSize.Height;
          this.Width = settings.WindowClientSize.Width;
        }
        this.Model = MainWindowModel.ToModel(settings);
        this.Model.SystemList_AddBlankItem();
      }

      grdMain.DataContext = this.Model;

      this.Canvas1.Focusable = true;
      this.Canvas1.Focus();
      this.ScreenCanvas = new ItemCanvas(
        this.Canvas1, 9.83, 18.5, new ScreenDim(24,80), 16, null);
    }

    private void MainWindow_Closed(object sender, EventArgs e)
    {
      {
        var settings = TesterSettings.RecallSettings();

        settings.WindowClientSize =
          new Size(this.ActualWidth, this.ActualHeight);
        settings.IpAddr = this.Model.IpAddr;
        settings.UserName = this.Model.UserName;
        settings.ParseText = this.Model.ParseText;
        settings.DataStreamName = this.Model.DataStreamName;
        settings.Password = this.Model.Password;
        settings.SystemList = this.Model.SystemList.Where(c => c != "<add>").ToList();
        settings.SystemName = this.Model.SystemName;
        settings.TextFilePath = this.Model.TextFilePath;
        settings.AutoConnect = this.Model.AutoConnect;

        // save the named data stream.
        {
          var namedDataStream = 
            new NamedDataStream(this.Model.DataStreamName, this.Model.ParseTextLines);
          settings.NamedDataStreamList.Apply(namedDataStream);
        }

        settings.StoreSettings();
      }
    }

    private void MenuItem_Click(object sender, RoutedEventArgs e)
    {
      string itemText = null;
      if (sender is MenuItem)
        itemText = (sender as MenuItem).Header as string;

      if (itemText == "Test")
      {
        var logList = new TelnetLogList();
        var negSettings = NegotiateSettings.Build5250Settings("SRICHTER", "Steve25");

        var lines = System.IO.File.ReadAllLines("c:\\downloads\\hextext.txt");
        var ba = lines.HexTextLinesToBytes();

        // print the bytes of the response stream as lines of hex text.
        {
          var rep = ba.ToHexReport(16);
          logList.AddItem(Direction.Read, "Length:" + ba.Length + " Byte stream bytes:");
          logList.AddItems(Direction.Read, rep);
        }

        var inputArray = new InputByteArray(ba, ba.Length);
        var sessionSettings = new SessionSettings();

        var rv = Process5250.ParseWorkstationCommandList(
          inputArray, sessionSettings);

        var dsh = rv.Item1;
        var wrkstnCmdList = rv.Item2;

        // draw the fields and literals on the canvas.
        if (wrkstnCmdList != null)
        {
          foreach (var workstationCmd in wrkstnCmdList)
          {

          }
        }
        return;
      }

      else if (itemText == "Parse server stream")
      {
        var ba = this.Model.ParseTextLines.HexTextLinesToBytes( );

        // print the bytes of the response stream as lines of hex text.
        {
          TelnetLogList logList = new TelnetLogList();
          var rep = ba.ToHexReport(16);
          logList.AddItem(Direction.Read, "Length:" + ba.Length + " Byte stream bytes:");
          logList.AddItems(Direction.Read, rep);
        }
      }

      else if (itemText == "Parse response stream")
      {
        var logList = new TelnetLogList();
        var negSettings = NegotiateSettings.Build5250Settings("SRICHTER", "Steve25");

        var lines = System.IO.File.ReadAllLines("c:\\downloads\\hextext.txt");
        var ba = lines.HexTextLinesToBytes();

        // print the bytes of the response stream as lines of hex text.
        {
          var rep = ba.ToHexReport(16);
          logList.AddItem(Direction.Read, "Length:" + ba.Length + " Byte stream bytes:");
          logList.AddItems(Direction.Read, rep);
        }

        {
          var inputArray = new InputByteArray(ba);
          var rv = Response5250.ParseResponseStream(inputArray);
          foreach( var respItem in rv.Item1)
          {
            this.Model.RunLog.Add(respItem.ToString());
          }
        }

        foreach (var item in logList)
        {
          if (item.NewGroup == true)
            this.Model.RunLog.Add("");
          this.Model.RunLog.Add(item.Text);
        }

        return;
      }

      else if (itemText == "Exit")
      {
        this.Close();
      }

      else if (itemText == "Settings")
      {
      }

      else if (itemText == "Print")
      {
        LinePrinter.PrintLines(this.Model.RunLog);
      }
      else if (itemText == "Clear log")
      {
        Model.RunLog.Clear();
      }
    }
    private void Canvas1_MouseDown(object sender, MouseButtonEventArgs e)
    {
      if (this.Canvas1.IsKeyboardFocused == false)
        this.Canvas1.Focus();

      Point pos = e.GetPosition(Canvas1);
    }

    private void butEdit_Click(object sender, RoutedEventArgs e)
    {
      var fileName = "c:\\downloads\\tn5250.txt";
      fileName = this.Model.TextFilePath;
      string[] lines = System.IO.File.ReadAllLines(fileName);

      var flowDoc = new FlowDocument();

      var para = new Paragraph();
      para.FontSize = 16.0;
      para.FontFamily = new FontFamily("Lucida console");

      foreach (string line in lines)
      {
        Run run = new Run(line);
        para.Inlines.Add(run);
        para.Inlines.Add(new LineBreak());
      }
      flowDoc.Blocks.Add(para);
       RichTextBox1.Document = flowDoc;
    }

    private void butTextFilePath_Click(object sender, RoutedEventArgs e)
    {
      var openDialog = new OpenFileDialog();
      openDialog.Filter = "txt Files |*.txt|All Files |*.*";
      var rc = openDialog.ShowDialog();
      if ((rc != null) && (rc.Value == true))
      {
        this.Model.TextFilePath = openDialog.FileName;
      }
    }
    private void menu2_Click(object sender, RoutedEventArgs e)
    {
      string itemText = null;
      if (sender is MenuItem)
        itemText = (sender as MenuItem).Header as string;

      if (itemText == "Report items")
      {
        var visualItems = this.ScreenCanvas.VisualItems;
        var report = wtdReportExt.PrintVisualItems(visualItems) ;
        this.Model.RunLog.AddRange(report);

        return;
      }
    }
  }
}
