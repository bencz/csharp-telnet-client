using AutoCoder.Ext.System;
using AutoCoder.Print;
using AutoCoder.Telnet.Commands;
using AutoCoder.Telnet.Common;
using AutoCoder.Telnet.Connect;
using AutoCoder.Telnet.Enums;
using AutoCoder.Telnet.IBM5250;
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
using TelnetTester.Common;
using TelnetTester.Models;
using TextCanvasLib.Main;

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
      this.Model = new MainWindowModel();
      this.Model.SystemList_AddBlankItem();
      grdMain.DataContext = this.Model;

      this.Canvas1.Focusable = true;
      this.Canvas1.Focus();
      this.ScreenCanvas = new ItemCanvas(this.Canvas1, 9.83, 18.5, 50, 15);
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
        var logList = new TelnetLogList();
        var negSettings = NegotiateSettings.Build5250Settings("SRICHTER", "Steve25");

        var lines = System.IO.File.ReadAllLines("c:\\downloads\\hextext.txt");
        var ba = ParseHexLines(lines);

        // print the bytes of the response stream as lines of hex text.
        {
          var rep = ba.ToHexReport(16);
          logList.AddItem(Direction.Read, "Length:" + ba.Length + " Byte stream bytes:");
          logList.AddItems(Direction.Read, rep);
        }

        var inputArray = new NetworkStreamBackedInputByteArray(ba, ba.Length);
        var sessionSettings = new SessionSettings();

        var rv = Process5250.GetAndParseWorkstationCommandList(
          inputArray, sessionSettings);

        logList.AddItems(rv.Item2);
        var wrkstnCmdList = rv.Item1;

        // draw the fields and literals on the canvas.
        if (wrkstnCmdList != null)
        {
          foreach (var workstationCmd in wrkstnCmdList)
          {

          }
        }

        foreach (var item in logList)
        {
          this.Model.RunLog.Add(item.Text);
        }

        return;
      }

      else if (itemText == "Parse server stream")
      {
        var text = TextBox1.Text;
        var lines = text.Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);

        var ba = ParseHexLines(lines);

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
        var ba = ParseHexLines(lines);

        // print the bytes of the response stream as lines of hex text.
        {
          var rep = ba.ToHexReport(16);
          logList.AddItem(Direction.Read, "Length:" + ba.Length + " Byte stream bytes:");
          logList.AddItems(Direction.Read, rep);
        }

        Process5250.ParseResponseStream(logList, ba);

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

    byte[] ParseHexLines(string[] Lines)
    {
      List<byte> bl = new List<byte>();

      foreach (var line in Lines)
      {
        // make sure the line is padded on right with at least 1 blank.
        var curLine = line.Trim() + "    ";

        // process 3 char chunks on  the current line.
        int ix = 0;
        while (true)
        {
          var chunk = curLine.Substring(ix, 3);
          if (chunk == "   ")
            break;

          var rv = chunk.HexTextToByte();
          var errmsg = rv.Item2;
          if (errmsg != null)
            break;
          bl.Add(rv.Item1);

          ix += 3;
        }

      }

      return bl.ToArray();
    }

    public TelnetLogList ParseByteArray(byte[] ByteArray)
    {
      var inputArray = new NetworkStreamBackedInputByteArray(ByteArray, ByteArray.Length);
      var sessionSettings = new SessionSettings();

      var logList = ParseDataStream(inputArray, sessionSettings);
      return logList;
    }

    public TelnetLogList ParseDataStream(
      NetworkStreamBackedInputByteArray inputArray, SessionSettings sessionSettings)
    {
      var logList = new TelnetLogList();

      bool didParse = true;
      while ((didParse == true) && (inputArray.RemainingLength > 0))
      {
        didParse = false;

        if (didParse == false)
        {
          var telCmd = inputArray.NextTelnetCommand();
          if (telCmd != null)
          {
            var s1 = telCmd.ToString();
            logList.AddItem(Direction.Read, s1);
            didParse = true;
          }
        }

        if (didParse == false)
        {
          var rv = Process5250.GetAndParseWorkstationCommandList(
          inputArray, sessionSettings);

          logList.AddItems(rv.Item2);
          var wrkstnCmdList = rv.Item1;

          // draw the fields and literals on the canvas.
          if (wrkstnCmdList?.Count > 0)
          {
            didParse = true;
            foreach (var workstationCmd in wrkstnCmdList)
            {

            }
          }
        }

      }
      return logList;
    }

    private void lbLog_PreviewKeyDown(object sender, KeyEventArgs e)
    {
      if ((e.Key == Key.C) && (Keyboard.IsKeyDown(Key.LeftCtrl) == true))
      {
        var items = lbLog.SelectedItems;
        var sb = new StringBuilder();
        foreach (var item in items)
        {
          sb.Append(item + Environment.NewLine);
        }
        Clipboard.SetText(sb.ToString());
      }
    }

    private void butClear_Click(object sender, RoutedEventArgs e)
    {
      TextBox1.Text = "";
    }

    private void butParse_Click(object sender, RoutedEventArgs e)
    {
      var text = TextBox1.Text;
      var lines = text.Split(
        new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);

      var ba = ParseHexLines(lines);
      var logList = ParseByteArray(ba);

      foreach (var item in logList)
      {
        this.Model.RunLog.Add(item.Text);
      }

      TabControl1.SelectedItem = 0;
    }

    private void butParseResponse_Click(object sender, RoutedEventArgs e)
    {
      var logList = new TelnetLogList();

      var text = TextBox1.Text;
      var lines = text.Split(
        new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
      var ba = ParseHexLines(lines);

      Process5250.ParseResponseStream(logList, ba);

      foreach (var item in logList)
      {
        if (item.NewGroup == true)
          this.Model.RunLog.Add("");
        this.Model.RunLog.Add(item.Text);
      }

      // print the bytes of the response stream as lines of hex text.
      {
        var rep = ba.ToHexReport(16);
        logList.AddItem(Direction.Read, "Length:" + ba.Length + " Byte stream bytes:");
        logList.AddItems(Direction.Read, rep);
      }
    }

    void NewParse()
    {
      var logList = new TelnetLogList();

      var text = TextBox1.Text;
      var lines = text.Split(
        new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
      var ba = ParseHexLines(lines);
      var inputArray = new NetworkStreamBackedInputByteArray(ba, ba.Length);
      var sessionSettings = new SessionSettings();

      Process5250.ParseResponseStream(logList, ba);

      foreach (var item in logList)
      {
        if (item.NewGroup == true)
          this.Model.RunLog.Add("");
        this.Model.RunLog.Add(item.Text);
      }


      // peek at the input stream from server. Classify the data that is next 
      // to receive.
      var typeData = ServerDataStream.PeekServerCommand(inputArray);

      // input data not recogizied. Not a 5250 data strem header.
      if (typeData == null)
      {
        logList.AddItem(Direction.Read, "Unknown data stream data");
        logList.AddItems(
          Direction.Read, inputArray.PeekBytes().ToHexReport(16));
      }

      else if (typeData.Value == TypeServerData.workstationHeader)
      {
        {
          var rv = Process5250.GetAndParseWorkstationCommandList(
          inputArray, sessionSettings);

          var workstationCmdList = rv.Item1;
          logList.AddItems(rv.Item2);
        }
      }
    }
  }
}
