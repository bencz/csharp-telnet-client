using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Collections.ObjectModel;
using AutoCoder.Collections;
using AutoCoder.Ext.System;
using AutoCoder.Ext;
using ScreenDefnLib.Defn;
using TextCanvasLib.Canvas;
using AutoCoder.Telnet.LogFiles;
using AutoCoder.Telnet.Models;
using AutoCoder.Telnet.Threads;
using TextCanvasLib.ThreadMessages;
using System.Windows;
using AutoCoder.Telnet.Settings;

namespace tnClient.Models
{
  public class ClientModel : INotifyPropertyChanged, IClientSettings
  {

    // declare the NotifyPropertyChanged event.
    public event PropertyChangedEventHandler PropertyChanged;

    public ClientModel()
    {
    }

    // Raise the property changed event of a specified property.
    protected void RaisePropertyChanged(string name)
    {
      PropertyChangedEventHandler handler = PropertyChanged;
      if (handler != null)
      {
        handler(this, new PropertyChangedEventArgs(name));
      }
    }

    public IThreadBase MatchThread
    { get; set; }
    public IThreadBase CaptureThread
    { get; set; }

    private bool _AutoConnect;

    public bool AutoConnect
    {
      get { return _AutoConnect; }
      set
      {
        if (_AutoConnect != value)
        {
          _AutoConnect = value;
          RaisePropertyChanged("AutoConnect");
        }
      }
    }

    public string DeviceName
    {
      get { return _DeviceName; }
      set
      {
        if (_DeviceName != value)
        {
          _DeviceName = value;
          RaisePropertyChanged("DeviceName");
        }
      }
    }
    private string _DeviceName;

    public ObservableCollection<string> DeviceNameList
    {
      get
      {
        if (_DeviceNameList == null)
        {
          _DeviceNameList = new ObservableCollection<string>();
          DeviceNameList.Add(this.DeviceName);
        }
        return _DeviceNameList;
      }
      set
      {
        if (_DeviceNameList != value)
        {
          _DeviceNameList = value;
          RaisePropertyChanged("DeviceNameList");
        }
      }
    }
    private ObservableCollection<string> _DeviceNameList;
    IList<string> IClientSettings.DeviceNameList
    {
      get
      {
        return this.DeviceNameList;
      }

      set
      {
        this.DeviceNameList = value.ToObservableCollection();
      }
    }

    public NamedDataStreamList NamedDataStreamList { get; set; }

    public void SystemList_AddBlankItem( )
    {
      this.SystemList.Add("<add>");
    }

    private ItemCanvas _TelnetCanvas;

    public ItemCanvas TelnetCanvas
    {
      get { return _TelnetCanvas; }
      set
      {
        if (_TelnetCanvas != value)
        {
          _TelnetCanvas = value;
          RaisePropertyChanged("TelnetCanvas");
        }
      }
    }

    public string TerminalType
    {
      get
      {
        if (_TerminalType.IsNullOrEmpty())
          _TerminalType = TerminalTypeList.FirstOrDefault();
        return _TerminalType;
      }
      set
      {
        if (_TerminalType != value)
        {
          _TerminalType = value;
          RaisePropertyChanged("TerminalType");
        }
      }
    }
    private string _TerminalType;
    public ObservableCollection<string> TerminalTypeList
    {
      get
      {
        if (_TerminalTypeList == null)
        {
          _TerminalTypeList = new ObservableCollection<string>();
          TerminalTypeList.Add("IBM-3179-2");
          TerminalTypeList.Add("IBM-3477-FC");
        }
        return _TerminalTypeList;
      }
      set
      {
        if (_TerminalTypeList != value)
        {
          _TerminalTypeList = value;
          RaisePropertyChanged("TerminalTypeList");
        }
      }
    }
    private ObservableCollection<string> _TerminalTypeList;

    /// <summary>
    /// the path to the xml file that contains the definition of telnet screens.
    /// </summary>
    public string ScreenDefnPath
    {
      get { return _ScreenDefnPath; }
      set
      {
        var s1 = value.TrimEndWhitespace();
        if (_ScreenDefnPath != s1)
        {
          _ScreenDefnPath = s1;
          RaisePropertyChanged("ScreenDefnPath");
        }
      }
    }
    private string _ScreenDefnPath;
    public Size WindowClientSize
    { get; set; }
    public double FontPointSize
    { get; set; }

    private string _CaptureFolderPath;

    public string CaptureFolderPath
    {
      get { return _CaptureFolderPath; }
      set
      {
        if (_CaptureFolderPath != value)
        {
          _CaptureFolderPath = value.TrimEndWhitespace();
          RaisePropertyChanged("CaptureFolderPath");
          PostCaptureAttributesMessage();
        }
      }
    }

    void PostCaptureAttributesMessage( )
    {
      if ( this.MatchThread != null)
      {
        var msg = new CaptureAttributesMessage(this.CaptureFolderPath, this.CaptureAuto);
        this.MatchThread.PostInputMessage(msg);
      }
    }

    // auto capture each match screen to capture folder.
    public bool CaptureAuto
    {
      get { return _CaptureAuto; }
      set
      {
        if (_CaptureAuto != value)
        {
          _CaptureAuto = value;
          RaisePropertyChanged("CaptureAuto");
          PostCaptureAttributesMessage();
        }
      }
    }
    private bool _CaptureAuto;

    public string RunTimeCodePath
    {
      get { return _RunTimeCodePath; }
      set
      {
        if (_RunTimeCodePath != value)
        {
          _RunTimeCodePath = value;
          RaisePropertyChanged("RunTimeCodePath");
        }
      }
    }
    private string _RunTimeCodePath;

    public string SupportCodeFilePath
    {
      get { return _SupportCodeFilePath; }
      set
      {
        if (_SupportCodeFilePath != value)
        {
          _SupportCodeFilePath = value;
          RaisePropertyChanged("SupportCodeFilePath");
        }
      }
    }
    private string _SupportCodeFilePath;

    /// <summary>
    /// SystemName is the textbox contents of the SystemNameList combo box.
    /// The idea is that changes to the textBox component of the ComboBox are 
    /// reflected into list that is bound to the items source of the combobox.
    /// </summary>
    public string SystemName
    {
      get { return _SystemName; }
      set
      {
        var s1 = value.TrimWhitespace();
        if ((s1 != "<add>") && (_SystemName != s1))
        {
          var nameInList = SystemList.FirstOrDefault(c => c == _SystemName);
          if (nameInList != null)
          {
            SystemList.Remove(nameInList);
          }
        }
        _SystemName = s1;
        RaisePropertyChanged("SystemName");

        if (SystemName.IsNullOrEmpty() == false)
        {
          var nameInList = SystemList.FirstOrDefault(c => c == _SystemName);
          if (nameInList == null)
          {
            SystemList.Add(this.SystemName);
          }
        }
      }
    }
    private string _SystemName;

    private string _SystemListSelectedItem;

    public string SystemListSelectedItem
    {
      get { return _SystemListSelectedItem; }
      set
      {
        if (_SystemListSelectedItem != value)
        {
          _SystemListSelectedItem = value;
          RaisePropertyChanged("SystemListSelectedItem");
          this.SystemName = value;
        }
      }
    }

    /// <summary>
    /// list of systems or ip addresses to connect to.
    /// </summary>
    public ObservableCollection<string> SystemList
    {
      get
      {
        if (_SystemList == null)
          _SystemList = new ObservableCollection<string>();
        return _SystemList;
      }
      set
      {
        if (_SystemList != value)
        {
          _SystemList = value;
          RaisePropertyChanged("SystemList");
        }
      }
    }
    private ObservableCollection<string> _SystemList;

    IList<string> IClientSettings.SystemList
    {
      get
      {
        return this.SystemList;
      }

      set
      {
        this.SystemList.Clear();
        foreach( var line in value)
        {
          this.SystemList.Add(line);
        }
      }
    }

    private ObservableCollection<TrafficLogItemTreeModel> _TrafficItems;
    public ObservableCollection<TrafficLogItemTreeModel> TrafficItems
    {
      get
      {
        if (_TrafficItems == null)
          _TrafficItems = new ObservableCollection<TrafficLogItemTreeModel>();
        return _TrafficItems;
      }
      set
      {
        _TrafficItems = value;
        RaisePropertyChanged("TrafficItems");
      }
    }


    string _IpAddr;
    public string IpAddr
    {
      get { return _IpAddr; }
      set
      {
        string s1 = value.TrimWhitespace();
        if (s1 != _IpAddr)
        {
          _IpAddr = s1;
          RaisePropertyChanged("IpAddr");

          if (this.IpAddrChanged != null)
          {
            this.IpAddrChanged(this);
          }
        }
      }
    }
    public event Action<ClientModel> IpAddrChanged;

    ObservableCollection<string> _RunLog;
    public ObservableCollection<string> RunLog
    {
      get
      {
        if (_RunLog == null)
          _RunLog = new ObservableCollection<string>();
        return _RunLog;
      }
      set
      {
        if (_RunLog != value)
        {
          _RunLog = value;
          RaisePropertyChanged("RunLog");
        }
      }
    }

    string _UserName;
    public string UserName
    {
      get { return _UserName; }
      set
      {
        string s1 = value.TrimWhitespace();
        if (s1 != _UserName)
        {
          _UserName = s1;
          RaisePropertyChanged("UserName");

          if (this.UserNameChanged != null)
          {
            this.UserNameChanged(this);
          }
        }
      }
    }
    public event Action<ClientModel> UserNameChanged;

    string _Password;
    public string Password
    {
      get { return _Password; }
      set
      {
        string s1 = value.TrimWhitespace();
        if (s1 != _Password)
        {
          _Password = s1;
          RaisePropertyChanged("Password");
        }
      }
    }

    public string OdbcDsn
    {
      get { return _OdbcDsn; }
      set
      {
        string s1;
        if (value == null)
          s1 = null;
        else
          s1 = value.Trim(new char[] { ' ', '\t', '\r', '\n' });
        if (_OdbcDsn != s1)
        {
          _OdbcDsn = s1;
          RaisePropertyChanged("OdbcDsn");
        }
      }
    }
    private string _OdbcDsn;


    public string DataStreamName
    {
      get { return _DataStreamName; }
      set
      {
        var trimmedValue = value.TrimEndWhitespace();
        if (_DataStreamName != trimmedValue)
        {
          _DataStreamName = trimmedValue;
          RaisePropertyChanged("DataStreamName");
        }
      }
    }
    private string _DataStreamName;

    public IScreenDefn MatchScreenDefn
    {
      get { return _MatchScreenDefn; }
      set
      {
        if (_MatchScreenDefn != value)
        {
          _MatchScreenDefn = value;
          RaisePropertyChanged("MatchScreenDefn");
        }
      }
    }
    private IScreenDefn _MatchScreenDefn;

    public string ParseText
    {
      get { return _ParseText; }
      set
      {
        if (_ParseText != value)
        {
          _ParseText = value;
          RaisePropertyChanged("ParseText");
        }
      }
    }
    private string _ParseText;
    public IEnumerable<string> ParseTextLines
    {
      get
      {
        var parseText = this.ParseText;
        if (parseText == null)
          parseText = String.Empty;

        var lines =
          parseText.Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
        return lines;
      }
    }

    public ObservableCollection<IScreenDefn> ScreenDefnObservableList
    {
      get; set;
    }

    private int _TabSelectedIndex;
    public int TabSelectedIndex
    {
      get { return _TabSelectedIndex; }
      set
      {
        if (_TabSelectedIndex != value)
        {
          _TabSelectedIndex = value;
          RaisePropertyChanged("TabSelectedIndex");
        }
      }
    }

    public IThreadBase MasterThread
    {
      get { return _MasterThread; }
      set
      {
        if (_MasterThread != value)
        {
          _MasterThread = value;
          RaisePropertyChanged("MasterThread");
        }
      }
    }

    private IThreadBase _MasterThread;

    public static ClientModel ToModel(ClientSettings Settings)
    {
      var model = new ClientModel();
      model.Apply(Settings);

      return model;
    }
  }
}
