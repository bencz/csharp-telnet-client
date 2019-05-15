using AutoCoder.Ext;
using AutoCoder.Ext.System;
using AutoCoder.Telnet.Common;
using AutoCoder.Telnet.IBM5250.Common;
using AutoCoder.Telnet.IBM5250.Header;
using AutoCoder.Telnet.IBM5250.Response;
using AutoCoder.Telnet.IBM5250.WorkstationCommands;
using AutoCoder.Telnet.TelnetCommands;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
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

namespace AutoCoder.Telnet.Controls
{
  /// <summary>
  /// Interaction logic for DataStreamControl.xaml
  /// </summary>
  public partial class DataStreamControl : UserControl
  {

    public string DataStreamName
    {
      get { return (string)GetValue(DataStreamNameProperty); }
      set { SetValue(DataStreamNameProperty, value); }
    }

    public static readonly DependencyProperty DataStreamNameProperty =
    DependencyProperty.Register("DataStreamName", typeof(string),
    typeof(DataStreamControl),
    new FrameworkPropertyMetadata(String.Empty, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

    public string ParseText
    {
      get { return (string)GetValue(ParseTextProperty); }
      set
      {
        SetValue(ParseTextProperty, value);
      }
    }

    public static readonly DependencyProperty ParseTextProperty =
    DependencyProperty.Register("ParseText", typeof(string),
    typeof(DataStreamControl), 
    new FrameworkPropertyMetadata(String.Empty, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,  
      OnParseTextPropertyChanged));

    private static void OnParseTextPropertyChanged(
       DependencyObject sender, DependencyPropertyChangedEventArgs e)
    {
      var control = sender as DataStreamControl;
      var newValue = e.NewValue as string;
      var oldValue = e.OldValue as string;
      control.ParseTextChanged = true;
      control.tbErrmsg.Text = String.Empty;
    }

    public ObservableCollection<string> ResultsList
    {
      get
      {
        if (_ResultsList == null)
          _ResultsList = new ObservableCollection<string>();
        return _ResultsList;
      }
      set { _ResultsList = value; }
    }
    private ObservableCollection<string> _ResultsList;

    public int TabSelectedIndex
    {
      get { return _TabSelectedIndex; }
      set
      {
        if (_TabSelectedIndex != value)
        {
          _TabSelectedIndex = value;
          if ( TabSelectedIndex == 1)
          {
            LoadResultsList();
          }
        }
      }
    }
    int _TabSelectedIndex;

    public DataStreamControl()
    {
      InitializeComponent();
      LayoutRoot.DataContext = this;
    }

    private void butClear_Click(object sender, RoutedEventArgs e)
    {
      this.ParseText = String.Empty;
    }

    private bool _ParseTextChanged;
    public bool ParseTextChanged
    {
      get { return _ParseTextChanged; }
      set
      {
        _ParseTextChanged = value;
        if (this.ParseTextChanged == true)
          tbErrmsg.Text = String.Empty;
      }
    }

    /// <summary>
    /// method called when the parse results are needed. That is, when any of the
    /// parse result properties are read. The idea is to not parse everytime the
    /// parseText textbox is changed. Just save the "text has been changed" flag
    /// and parse when the parse results are read.
    /// </summary>
    private void ParseTheParseText()
    {
      if ((this.ParseTextChanged == true) || ( tbErrmsg.Text.Length != 0))
      {
        tbErrmsg.Text = String.Empty;
        if (this.ParseText.Length == 0)
          tbErrmsg.Text = "Enter hex text to parse";
        else
        {
          var lines =
            this.ParseText.Split(new string[]
            { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);

          var ba = lines.HexTextLinesToBytes();

          var rv = ServerDataStream.ParseByteArray(ba, -1, this.DataStreamName);
          this.WrkstnCmdList = rv.Item1;
          this.ResponseList = rv.Item2;
          this.dsh = rv.Item3;
          this.TelList = rv.Item4;
          this.ParseTextChanged = false;
        }
      }
    }

    private DataStreamHeader _dsh;
    public DataStreamHeader dsh
    {
      get
      {
        ParseTheParseText();
        return _dsh;
      }
      set { _dsh = value; }
    }

    private TelnetCommandList _TelList;
    public TelnetCommandList TelList
    {
      get
      {
        ParseTheParseText();
        return _TelList;
      }
      private set { _TelList = value; }
    }

    private TelnetLogList _LogList;
    public TelnetLogList LogList
    {
      get
      {
        ParseTheParseText();
        return _LogList;
      }
      private set { _LogList = value; }
    }
    private WorkstationCommandList _WrkstnCmdList;
    public WorkstationCommandList WrkstnCmdList
    {
      get
      {
        ParseTheParseText();
        return _WrkstnCmdList;
      }
      set { _WrkstnCmdList = value; }
    }
    private ResponseItemList _ResponseList;
    public ResponseItemList ResponseList
    {
      get
      {
        ParseTheParseText();
        return _ResponseList;
      }
      set { _ResponseList = value; }
    }

    private void LoadResultsList()
    {
      ParseTheParseText();
      this.ResultsList.Clear();
      this.ResultsList.AddRange(from c in this.LogList select c.ToString());

      if (this.dsh != null)
      {
        var rep = dsh.ToColumnReport();
        this.ResultsList.AddRange(rep);
      }

      if ((this.WrkstnCmdList != null ) && (WrkstnCmdList.Count > 0))
      {
        var rep = WrkstnCmdList.ToColumnReport("workstation command list");
        this.ResultsList.AddRange(rep);
      }

      if ( this.ResponseList != null)
      {
        var rep = this.ResponseList.ReportResponseItems();
        this.ResponseList.AddRange(rep);
      }
    }

    private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
    {
      this.ParseTextChanged = true;
      Debug.Print("textbox changed");
    }

    private void butPrepare_Click(object sender, RoutedEventArgs e)
    {
      var list2 = new List<string>();

      // split the parse text into lines.
      var lines = this.ParseText.Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
      foreach( var line in lines)
      {
        // remove all blanks.
        var s2 = line.ReplaceAll(" ", "");

        {
          s2 = s2 + "    ";
          var sb = new StringBuilder();
          for( int ix = 0; ix < s2.Length; ix += 2)
          {
            var ch2 = s2.Substring(ix, 2).TrimEnd( );
            if (ch2.Length == 0)
              break;
            sb.Append(ch2 + " ");
          }
          s2 = sb.ToString();
        }

        list2.Add(s2);
      }

      {
        var sb = new StringBuilder();
        foreach( var s2 in list2)
        {
          sb.Append(s2 + Environment.NewLine);
        }
        this.ParseText = sb.ToString();
      }
    }
  }
}
