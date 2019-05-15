using AutoCoder.Ext;
using AutoCoder.Ext.System;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TelnetTester.Settings;

namespace TelnetTester.Models
{
  public class MainWindowModel : INotifyPropertyChanged
  {

    // declare the NotifyPropertyChanged event.
    public event PropertyChangedEventHandler PropertyChanged;

    public MainWindowModel()
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
    private string _DataStreamName;

    public string DataStreamName
    {
      get { return _DataStreamName; }
      set
      {
        if (_DataStreamName != value)
        {
          _DataStreamName = value;
          RaisePropertyChanged("DataStreamName");
        }
      }
    }

    private string _ParseText;

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
    public IEnumerable<string> ParseTextLines
    {
      get
      {
        var lines = 
          this.ParseText.Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
        return lines;
      }
    }
    public void SystemList_AddBlankItem()
    {
      this.SystemList.Add("<add>");
    }


    private string _SystemName;

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

    private ObservableCollection<string> _SystemList;

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

    private string _TextFilePath;
    public string TextFilePath
    {
      get { return _TextFilePath; }
      set
      {
        if (_TextFilePath != value)
        {
          _TextFilePath = value;
          RaisePropertyChanged("TextFilePath");
        }
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
        }
      }
    }

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
        }
      }
    }

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


    public static MainWindowModel ToModel(TesterSettings Settings)
    {
      var model = new MainWindowModel()
      {
        UserName = Settings.UserName,
        ParseText = Settings.ParseText,
        DataStreamName = Settings.DataStreamName,
        IpAddr = Settings.IpAddr,
        Password = Settings.Password,
        AutoConnect = Settings.AutoConnect,
        TextFilePath = Settings.TextFilePath,
        SystemList = Settings.SystemList.ToObservableCollectionString(),
        SystemName = Settings.SystemName
      };

      return model;
    }
  }
}
