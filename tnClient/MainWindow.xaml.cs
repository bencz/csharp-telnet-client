using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using AutoCoder.Ext.Windows.Controls;
using AutoCoder.Telnet.Common;
using System.Net.Sockets;
using tnClient.Models;
using TextCanvasLib.Canvas;
using TextCanvasLib.xml;
using AutoCoder.Telnet.Connect;
using AutoCoder.Telnet.Enums;
using System.Diagnostics;
using AutoCoder.Print;
using AutoCoder.Ext;
using AutoCoder.Telnet.IBM5250.WorkstationCommands;
using ScreenDefnLib.Defn;
using TextCanvasLib.Telnet;
using AutoCoder.Telnet.Common.RowCol;
using AutoCoder.Threading;
using System.Threading;
using AutoCoder.Telnet.Threads;
using AutoCoder.Core.Interfaces;
using AutoCoder.Telnet;
using TextCanvasLib.Threads;
using AutoCoder.Telnet.LogFiles;
using TextCanvasLib.ThreadMessages;
using AutoCoder.Telnet.ThreadMessages;
using System.Windows.Threading;
using AutoCoder.Telnet.Common.ScreenDm;
using AutoCoder.Telnet.IBM5250.Content;
using AutoCoder.Serialize;
using AutoCoder.Telnet.Enums.IBM5250;
using AutoCoder.Ext.System;
using tnClient.Windows;
using AutoCoder.Telnet.Settings;

namespace tnClient
{
  /// <summary>
  /// Interaction logic for MainWindow.xaml
  /// </summary>
  public partial class MainWindow : Window
  {
    ItemCanvas TelnetCanvas
    {
      get { return this.Model.TelnetCanvas; }
      set { this.Model.TelnetCanvas = value; }
    }

    public ClientModel Model
    { get; set; }

    public ServerConnectPack ConnectPack
    { get; set; }

    /// <summary>
    /// event that is set when the window is closed. Reference to this event is 
    /// passed to background threads, FromThread, PaintThread.
    /// </summary>
    private ExtendedManualResetEvent ShutdownFlag
    { get; set; }

    FromThread FromThread
    { get; set; }
    PaintThread PaintThread
    { get; set; }
    MasterThread MasterThread
    { get; set; }

    PrinterThread PrinterThread
    { get; set; }

    IThreadBase MatchThread
    { get; set; }
    IThreadBase CaptureThread
    { get; set; }

    IThreadBase ConnectThread
    { get; set; }

    ToThread ToThread
    { get; set; }

    /// <summary>
    /// queue created by this window. Reference is passed to FromThread and
    /// the ConnectThread. FromThread posts telnet commands to the TelnetQueue.
    /// Then the ConnectThread receives from TelnetQueue during connect 
    /// processing.
    /// </summary>
    ConcurrentMessageQueue TelnetQueue
    { get; set; }

    public TelnetLogList LogList
    { get; set; }

    /// <summary>
    /// bind target of TabControl selectedIndex property. When a tab is selected 
    /// this property is set.
    /// </summary>
    public int TabSelectedIndex
    {
      get { return _TabSelectedIndex; }
      set
      {
        _TabSelectedIndex = value;
        if (this.TabSelectedIndex == 0)
          RunLogTabSelected();
        else if (this.TabSelectedIndex == 1)
          SetCanvasFocus();
        else if (this.TabSelectedIndex == 5)
        {
          FillTrafficListBox();
        }
      }
    }
    /// <summary>
    /// the most recent screen capture report. Save the report so that report
    /// data can be accumulated with the postAidKey if pagedown.
    /// </summary>
    private DataItemReport CaptureReport
    { get; set; }

    private void FillTrafficListBox()
    {
      this.Model.TrafficItems = TrafficLogFile.ToLogItemList().ToObservableCollection();
    }

    int _TabSelectedIndex = 0;

    public MainWindow()
    {
      InitializeComponent();
      this.ShutdownFlag = new ExtendedManualResetEvent();
      this.FromThread = null;
      SpecialLogFile.ClearFile();

      this.Closed += new EventHandler(MainWindow_Closed);
      this.Loaded += new RoutedEventHandler(MainWindow_Loaded);
      this.Closing += MainWindow_Closing;

      this.cbFontSize.ItemsSource = new List<double>()
      { 8, 9, 10, 11, 12, 13, 14, 16, 18, 20, 22, 24, 26, 28, 36, 48, 72 };
    }

    private void TelnetWindowInputSignalHandler(ThreadMessageBase message)
    {
      if (message is MatchScreenDefnMessage)
      {
        var defnMessage = message as MatchScreenDefnMessage;
        this.Model.MatchScreenDefn = defnMessage.ScreenDefn;

        if (this.Dispatcher.CheckAccess() == false)
        {
          this.Dispatcher.BeginInvoke(
            DispatcherPriority.Input, new ThreadStart(
              () =>
              {
                if (defnMessage.ScreenDefn != null)
                {
                  tbStatusBarMessage.Text = defnMessage.ScreenDefn.ScreenName;

                  // auto capture of content of every screen with a matching 
                  // screen defn. Signal the capture thread to perform the capture.
                  if (this.Model.CaptureAuto == true)
                  {
                    var captureMessage = new CaptureContentMessage(
                      this.Model.CaptureFolderPath, this.Model.CaptureAuto,
                      defnMessage.ScreenDefn, defnMessage.ScreenContent);
                    this.CaptureThread.PostInputMessage(captureMessage);
                  }
                }
              }));
        }
      }
    }

    bool? canAutoClose;
    private void MainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
    {
      if ((canAutoClose != null) && (canAutoClose.Value == true))
      {

      }
      else
      {
        var rc = MessageBox.Show(
          "allow window to be closed:", "window closing", MessageBoxButton.YesNo);
        if (rc == MessageBoxResult.No)
          e.Cancel = true;
      }
    }

    void MainWindow_Loaded(object sender, RoutedEventArgs e)
    {
      this.LogList = new TelnetLogList("Main");

      {
        var settings = ClientSettings.RecallSettings();

        if (settings.WindowClientSize.Height > 0)
        {
          this.Height = settings.WindowClientSize.Height;
          this.Width = settings.WindowClientSize.Width;
        }
        this.Model = ClientModel.ToModel(settings);

        // recall list of screen defn from screen defn xml file.
        var defnList = ScreenDefnList.RecallFromXmlFile(this.Model.ScreenDefnPath);
        this.Model.ScreenDefnObservableList = defnList.ToObservableCollection();

        this.Model.SystemList_AddBlankItem();
        grdMain.DataContext = this.Model;

        this.Canvas1.Focusable = true;
        this.Canvas1.IsEnabled = true;
        this.Canvas1.Focus();

        this.Model.TelnetCanvas = new ItemCanvas(
          this.Canvas1, 9.83, 18.5, new ScreenDim(24, 80), settings.FontPointSize,
          null);

        this.udFontSize.NumericValue = (int)this.Model.TelnetCanvas.FontPointSize;

        this.Model.TelnetCanvas.CanvasChanged += ScreenCanvas_CanvasChanged;

        TrafficLogFile.ClearFile();
      }
    }

    private void ScreenCanvas_CanvasChanged(ItemCanvas Canvas)
    {
      // get the canvas cursor.  show the row/column location in the status bar.
      var cursor = this.Model.TelnetCanvas.CaretCursor;
      var rowCol = cursor.RowCol as ZeroRowCol;
      tbStatusBarMessage.Text = rowCol.ToOneRowCol().ToString() +
        " Mouse:" + this.Model.TelnetCanvas.MousePosOnCanvas.ToString();
    }

    void MainWindow_Closed(object sender, EventArgs e)
    {
      // signal that the tnClient app is shutting down. Background threads all wait
      // on this event.
      this.ShutdownFlag.Set();

      // wait for background threads to end.
      if (this.FromThread != null)
      {
        this.FromThread.ThreadEndedEvent.WaitOne();
      }
      if (this.PaintThread != null)
      {
        this.PaintThread.ThreadEndedEvent.WaitOne();
      }
      if (this.MasterThread != null)
      {
        this.MasterThread.ThreadEndedEvent.WaitOne();
      }
      if (this.PrinterThread != null)
      {
        this.PrinterThread.ThreadEndedEvent.WaitOne();
      }
      if (this.ToThread != null)
      {
        this.ToThread.ThreadEndedEvent.WaitOne();
      }
      if (this.MatchThread != null)
      {
        this.MatchThread.ThreadEndedEvent.WaitOne();
      }
      if (this.CaptureThread != null)
      {
        this.CaptureThread.ThreadEndedEvent.WaitOne();
      }

      // make sure the telnet connection is closed.
      this.ConnectPack?.Dispose();

      this.TelnetCanvas.Dispose();

      this.ClientSettingsControl_ApplySettings(this.Model);

      // serialize the ScreenDefnList
      if ( this.Model.ScreenDefnPath.IsNullOrEmpty( ) == false )
      {
        var defnList = new ScreenDefnList(this.Model.ScreenDefnObservableList);
        defnList.StoreToXmlFile(this.Model.ScreenDefnPath);
      }
    }

    private void MenuItem_Click(object sender, RoutedEventArgs e)
    {
      string itemText = null;
      string tagText = null;
      var senderItem = sender as MenuItem;
      if (sender is MenuItem)
        itemText = (sender as MenuItem).Header as string;
      tagText = senderItem.Tag as string;

      if (itemText == "Telnet")
      {
        var devName = "STEVE26";
        var termType = "IBM-3477-FC";  // IBM-3477-FC, IBM-3179-2
        TelnetInitialConnect(
          this.Model.SystemName, this.Model.AutoConnect, this.Model.DeviceName, this.Model.TerminalType);
      }

      else if (itemText == "Printer")
      {
        TelnetPrinterConnect(this.Model.SystemName);
      }

      else if (itemText == "Exit")
      {
        this.canAutoClose = true;
        this.Close();
      }

      else if (itemText == "Test")
      {
        var s2 = this.Model.ScreenDefnPath;
        Debug.Print("ScreenDefnPath:" + s2);
      }

      else if (itemText == "Read xml")
      {
        string xmlPath = "c:\\skydrive\\c#\\TextCanvasLib\\xmlfile1.xml";
        var items = ScreenDocReader.ReadDoc(xmlPath);
        OneRowCol caret = null;
        this.Model.TelnetCanvas.PaintScreen(items, caret);
      }

      else if (itemText == "Print log")
      {
        LinePrinter.PrintLines(this.Model.RunLog);
      }
      else if (itemText == "Clear log")
      {
        MainWindow.LogFile.ClearFile();
        this.Model.RunLog.Clear();
        this.PaintThread.PostInputMessage(ThreadMessageCode.ClearLog);
        this.MasterThread.PostInputMessage(ThreadMessageCode.ClearLog);
        this.ToThread.PostInputMessage(ThreadMessageCode.ClearLog);
        this.FromThread.PostInputMessage(ThreadMessageCode.ClearLog);
      }
      else if (itemText == "view special")
      {
        var specialPath = "c:\\downloads\\specialLog.txt";
        string exePath =
          Environment.ExpandEnvironmentVariables(@"%windir%\system32\notepad.exe");
        Process.Start(exePath, specialPath);

      }

      else if (itemText == "Report canvas items")
      {
        MasterThread.PostInputMessage(ThreadMessageCode.ReportVisualItems);
        var visualItems = this.Model.TelnetCanvas.VisualItems;
        var report = wtdReportExt.PrintVisualItems(visualItems);
        this.Model.RunLog.AddRange(report);
      }

      else if (itemText == "Send data")
      {
        var dataBytes = new byte[] {0x00, 0x0a, 0x12, 0xa0, 0x01, 0x02,
        0x04, 0x00, 0x00, 0x01, 0xff, 0xef };

        dataBytes = new byte[] { 0x00, 0x0A, 0x12, 0xA0, 0x01, 0x02,
        0x04, 0x00, 0x00, 0x01, 0xFF, 0xEF };

        var dataMessage = new SendDataMessage(dataBytes);
        this.ToThread.PostInputMessage(dataMessage);
      }

      // capture the currently matched screen. 
      else if (tagText == "Capture")
      {
        if (this.Model.MatchScreenDefn == null)
          MessageBox.Show("no matched screen to capture");
        else
        {

          // send message to master thread to get the current screen content.
        var msg = new ExchangeMessage(ThreadMessageCode.GetScreenContent);
        this.MasterThread.PostInputMessage(msg);
        msg.WaitReplyEvent();
        var content = msg.ReplyMessage as ScreenContent;

          // send message to capture thread telling it to capture the current
          // screen.
        var captureMessage = new CaptureContentMessage(
          this.Model.CaptureFolderPath, this.Model.CaptureAuto,
          this.Model.MatchScreenDefn, content);
        this.CaptureThread.PostInputMessage(captureMessage);
        }
      }

      else if (tagText == "CaptureViewer")
      {
        var window = new CaptureViewerWindow();
        window.CaptureFolderPath = this.Model.CaptureFolderPath;
        window.Show();
      }
    }
    void RunLogTabSelected()
    {
      var mergeList = this.LogList.Merge(
        this.LogList,
        this.FromThread?.LogList, this.PaintThread?.LogList,
        this.MasterThread?.LogList, this.ToThread?.LogList);
      this.Model.RunLog.Clear();
      if (mergeList != null)
      {
        var reportLines = mergeList.ToColumnReport();
        this.Model.RunLog.AddRange(reportLines);
      }
    }

    private void TelnetInitialConnect(
      string ServerName, bool AutoConnect, string DeviceName, 
      string TerminalType)
    {
      var host = ServerName;
      int port = 23;
      SessionSettings sessionSettings = null;

      // already connected.
      if (this.ConnectPack?.IsConnected() == true)
      {
        MessageBox.Show("already connected to telnet server.");
        return;
      }

      MainWindow.LogFile.ClearFile();

      // TelnetNegotiateSettings
      //    - WILL EOR
      //    - WILL TERMINAL TYPE
      //    - TERMINAL TYPE
      //    - user name, terminal name
      // TelnetCommand.Negotiate( negotiateSettings )
      // returns TelnetSessionAttributes

      // after negotiate, call method to send and receive 5250 DataStreamHeader
      // followed by escape 04 orders
      // code is receive 5250 command, render to screen, wait for screen AID key,
      // send 5250 command response.
      var negSettings = NegotiateSettings.Build5250Settings(
        "SRICHTER", DeviceName, TerminalType);

      // connect to the server.
      var rv = ConnectToServer_StartServerThreads(ServerName, port);
      this.ConnectPack = rv.Item1;
      sessionSettings = rv.Item2;

      if (this.ConnectPack.IsConnected() == true)
      {
        // send message to connect thread. This will run telnet device startup
        // processing on a backround thread. This way problems do not lock up 
        // the UI.
        {
          var startupMessage = new TelnetStartupMessage(
            this.TelnetQueue,
            this.ConnectPack, negSettings, this, MainWindow_TelnetStartupComplete,
            TypeTelnetDevice.Terminal);

          this.ConnectThread.PostInputMessage(startupMessage);
        }
      }
    }

    /// <summary>
    /// method called from the ConnectThread. Called when telnet startup has been
    /// completed. Telnet client is now ready to process workstation messages and
    /// render them to the canvas.
    /// </summary>
    /// <param name="obj"></param>
    private void MainWindow_TelnetStartupComplete(bool obj, TypeTelnetDevice? TypeDevice)
    {
      // send message to FromThread telling it the connection is complete and the
      // device type.
      {
        var message = new TelnetDeviceAttrMessage(TypeDevice.Value);
        this.FromThread.PostInputMessage(message);
      }

      // start the master screen content thread.
      {
        var job = new ThreadStart(this.MasterThread.EntryPoint);
        Thread t = new Thread(job);
        t.IsBackground = true;
        t.Name = "MasterThread";
        t.Start();
      }

      // start the printer thread.
      if (TypeDevice.Value == TypeTelnetDevice.Printer)
      {
        var job = new ThreadStart(this.PrinterThread.EntryPoint);
        Thread t = new Thread(job);
        t.IsBackground = true;
        t.Name = "PrinterThread";
        t.Start();
      }

      // start the paint thread. This thread reads WorkstationCommand blocks that are
      // created by the FromThread and paints them to the ItemCanvas.
      {
        var job = new ThreadStart(this.PaintThread.EntryPoint);
        Thread t = new Thread(job);
        t.IsBackground = true;
        t.Name = "PaintThread";
        t.Start();
      }

      // start the match thread. This thread matches the screenContent against the
      // ScreenDefn screen definitions.  The screen defn contains hover scripts,
      // screen enhancements, etc.
      {
        var job = new ThreadStart(this.MatchThread.EntryPoint);
        Thread t = new Thread(job);
        t.IsBackground = true;
        t.Name = "MatchThread";
        t.Start();

        // the model can send alert messages to the threads. set ref to the threads.
        this.Model.MatchThread = this.MatchThread;

        // send the screenDefnList to the match thread.
        var screenDefnList = new ScreenDefnList(this.Model.ScreenDefnObservableList);
        var assignMessage = new AssignScreenDefnListMessage(screenDefnList);
        this.MatchThread.PostInputMessage(assignMessage);
      }

      // start the capture thread. This thread capture screen contents according
      // to a screen defn. Captured data is stored to database, stream file or
      // clipboard.
      {
        var job = new ThreadStart(this.CaptureThread.EntryPoint);
        Thread t = new Thread(job);
        t.IsBackground = true;
        t.Name = "CaptureThread";
        t.Start();

        // the model can send alert messages to the threads. set ref to the threads.
        this.Model.CaptureThread = this.CaptureThread;
      }

      // todo: make this two method calls.
      //       FindSelectCanvas( ). then SetCanvasFocus( ).
      //       also, move SetCanvasFocus into Canvas class. probably make a
      //       Canvas class extension method. SetCompleteFocus. But try to make
      //       ItemCanvas to have Canvas as a base class.
      FindCanvasSetFocus();
    }

    private void StartServerThreads()
    {
      // start the ConnectThread.
      {
        var job = new ThreadStart(this.ConnectThread.EntryPoint);
        Thread t = new Thread(job);
        t.IsBackground = true;
        t.Name = "ConnectThread";
        t.Start();
      }

      // start the FromThread.
      {
        var job = new ThreadStart(this.FromThread.EntryPoint);
        Thread t = new Thread(job);
        t.IsBackground = true;
        t.Name = "FromThread";
        t.Start();
      }

      // start the ToThread.
      {
        var job = new ThreadStart(this.ToThread.EntryPoint);
        Thread t = new Thread(job);
        t.IsBackground = true;
        t.Name = "ToThread";
        t.Start();
      }
    }

    private void CreateThreads(TcpClient tcpClient, SessionSettings sessionSettings)
    {
      // telnetQueue. FromThread sends telnet commands to the ConnectThread thru
      // the TelnetQueue.
      this.TelnetQueue = new ConcurrentMessageQueue();

      // receive telnet data stream from server thread.
      this.FromThread =
        new FromThread(this.ShutdownFlag, tcpClient, this.TelnetQueue,
        sessionSettings,
        this.Model.TelnetCanvas.ScreenDim);

      // send telnet data stream up to the server.
      this.ToThread = new ToThread(
        this.ShutdownFlag, tcpClient, this.Model.TelnetCanvas.ScreenDim);

      // connection startup thread.
      this.ConnectThread = new ConnectThread(
        this.ShutdownFlag, this.FromThread, this.ToThread,
        sessionSettings);

      // MasterThread. apply telnet and keyboard input to master 
      // ScreenContent block.
      this.MasterThread = new MasterThread(
        this.ShutdownFlag, this.FromThread.ConnectionFailedEvent,
        this.Model.TelnetCanvas.ScreenDim, this.Model.TelnetCanvas);

      // PrinterThread. process printer data stream messages.
      this.PrinterThread = new PrinterThread(
        this.ShutdownFlag, this.FromThread.ConnectionFailedEvent);

      // PaintThread. paints the contents of ScreenContent.
      this.PaintThread = new PaintThread(
        this.ShutdownFlag, this, this.TelnetCanvas);

      // match screenContent against screen defn.
      this.MatchThread = new MatchThread(
        this.ShutdownFlag, this.MasterThread, this.PaintThread,
        TelnetWindowInputSignalHandler);

      // capture data from screen content according to screen defn.
      this.CaptureThread = new CaptureThread(
        this.ShutdownFlag, this.MasterThread);

      this.PaintThread.MasterThread = this.MasterThread;
      this.MasterThread.PaintThread = this.PaintThread;
      this.MasterThread.ToThread = this.ToThread;
      this.MasterThread.MatchThread = this.MatchThread;
      this.FromThread.MasterThread = this.MasterThread;
      this.FromThread.PrinterThread = this.PrinterThread;
      this.PrinterThread.ToThread = this.ToThread;

      // set the ItemCanvas to send keyboard input to the master thread.
      this.TelnetCanvas.MasterThread = this.MasterThread;
      this.TelnetCanvas.PaintThread = this.PaintThread;

      // apply references in the model.
      this.Model.MasterThread = this.MasterThread;
    }

    private void TelnetPrinterConnect(string ServerName)
    {
      var host = ServerName;
      int port = 23;
      SessionSettings sessionSettings = null;

      // already connected.
      if (this.ConnectPack?.IsConnected() == true)
      {
        MessageBox.Show("already connected to telnet server.");
        return;
      }

      MainWindow.LogFile.ClearFile();

      var negSettings = NegotiateSettings.Build5250Settings(
      "SRICHTER", "STEVE41P", "IBM-3812-1");
      //      var negSettings = NegotiateSettings.Build5250Settings(
      //        "SRICHTER", "STEVE42P", "IBM-5256-1");

      // connect to the server.
      var rv = ConnectToServer_StartServerThreads(ServerName, port);
      this.ConnectPack = rv.Item1;
      sessionSettings = rv.Item2;

      if (this.ConnectPack.IsConnected() == true)
      {
        // send message to connect thread. This will run telnet device startup
        // processing on a backround thread. This way problems do not lock up 
        // the UI.
        {
          var startupMessage = new TelnetStartupMessage(
            this.TelnetQueue,
            this.ConnectPack, negSettings, this, MainWindow_TelnetStartupComplete,
            TypeTelnetDevice.Printer);
          this.ConnectThread.PostInputMessage(startupMessage);
        }
      }
    }

    private Tuple<ServerConnectPack, SessionSettings> ConnectToServer_StartServerThreads(
      string serverName, int port)
    {
      SessionSettings sessionSettings = null;
      try
      {
        var tcpClient = new TcpClient();
        tcpClient.Connect(serverName, port);

        sessionSettings = new SessionSettings();
        CreateThreads(tcpClient, sessionSettings);

        // start the ConnectThread, FromThread, ToThread.
        StartServerThreads();

        this.ConnectPack = new ServerConnectPack(this.FromThread, tcpClient, serverName);
      }
      catch (SocketException ex)
      {
        MessageBox.Show("Socket Error: " + ex.Message);
      }

      return new Tuple<ServerConnectPack, SessionSettings>(this.ConnectPack, sessionSettings);
    }

    void SetCanvasFocus()
    {
      this.Canvas1.Focusable = true;
      Keyboard.Focus(this.Canvas1);
      this.Canvas1.Focus();
    }

    void FindCanvasSetFocus()
    {
      foreach (var item in TabControl1.Items)
      {
        var tabItem = item as TabItem;
        if (tabItem != null)
        {
          var header = tabItem.Header as string;
          if ((header != null) && (header == "Canvas"))
          {
            this.TabControl1.SelectedItem = tabItem;
            this.UpdateLayout();  // update layout needed to make sure tab item gets focus.
            SetCanvasFocus();
            break;
          }
        }
      }
    }

    private static TelnetLogFile _LogFile = null;
    public static TelnetLogFile LogFile
    {
      get
      {
        if (_LogFile == null)
        {
          _LogFile = new TelnetLogFile("c:\\downloads\\telnetlog.txt");
          _LogFile.ByteChunkOnly = true;
        }
        return _LogFile;
      }
    }

    private void MenuItem2_Click(object sender, RoutedEventArgs e)
    {
      string itemText = null;
      if (sender is MenuItem)
        itemText = (sender as MenuItem).Header as string;

      if (itemText == "Parse")
      {
        ParseDataStream();
        this.Model.TabSelectedIndex = 0;
      }
      if (itemText == "wtd order detail")
      {
        wtdCommandOrderDetail();
        this.Model.TabSelectedIndex = 0;
      }
    }

    private void wtdCommandOrderDetail()
    {
      Report report = new Report();
      var wrkstnCmdList = DataStreamControl1.WrkstnCmdList;
      foreach (var wsc in wrkstnCmdList)
      {
        var wtdCmd = wsc as WriteToDisplayCommand;
        if (wtdCmd != null)
        {
          var lines = wtdCmd.ToOrderDetailReport();
          this.Model.RunLog.AddRange(lines);
        }
      }
    }

    private void ParseDataStream()
    {
      Report report = new Report();
      var dsh = DataStreamControl1.dsh;
      var wrkstnCmdList = DataStreamControl1.WrkstnCmdList;
      var responseList = DataStreamControl1.ResponseList;
      var logList = DataStreamControl1.LogList;

      // data stream header.
      if (dsh != null)
      {
        var rep = dsh.ToColumnReport();
        report.WriteTextLines(rep);
      }

      // draw the fields and literals on the canvas.
      if (wrkstnCmdList?.Count > 0)
      {
        {
          var rep = wrkstnCmdList.ToColumnReport();
          logList.AddItems(Direction.Read, rep, true);
          report.WriteGapLine();
          report.WriteTextLines(rep);
        }

        var rv = wrkstnCmdList.PaintCanvas(this.TelnetCanvas);
      }

      if (responseList?.Count > 0)
      {
        var rep = responseList.ReportResponseItems();
        report.WriteGapLine();
        report.WriteTextLines(rep);
      }

      this.LogList.Clear();
      foreach (var repLine in report)
      {
        this.LogList.AddItem(Direction.Read, repLine);
      }
    }

    private void cbFontSize_TextChanged(object sender, RoutedEventArgs e)
    {
      var fontSize = cbFontSize.SelectedValue as double?;
      if (fontSize != null)
      {
        this.TelnetCanvas.ChangeFontSize(fontSize);
      }
    }

    private void udFontSize_ValueChanged(int Value)
    {
      this.TelnetCanvas.ChangeFontSize(Value);
    }

    private void MainWindow_InputSignalHandler(ThreadMessageBase Message)
    {
    }

    /// <summary>
    /// method called when Apply button is clicked in ClientSettingsControl. 
    /// Also this method called when tnClient window is closed.
    /// </summary>
    /// <param name="model"></param>
    private void ClientSettingsControl_ApplySettings(ClientModel model)
    {
      {
        var settings = ClientSettings.RecallSettings();
        model.WindowClientSize = new Size(this.ActualWidth, this.ActualHeight);
        model.FontPointSize = model.TelnetCanvas.CanvasDefn.FontDefn.PointSize;

        settings.Apply(model);

        // save the named data stream.
        {
          var namedDataStream =
            new NamedDataStream(model.DataStreamName, model.ParseTextLines);
          settings.NamedDataStreamList.Apply(namedDataStream);
        }

        settings.StoreSettings();
      }
    }
  }
}
