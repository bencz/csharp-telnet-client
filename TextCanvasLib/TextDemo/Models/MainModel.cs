using AutoCoder.Collections;
using AutoCoder.Ext.System;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TextDemo.Settings;

namespace TextDemo.Models
{
  public class MainModel : INotifyPropertyChanged
  {

    // declare the NotifyPropertyChanged event.
    public event PropertyChangedEventHandler PropertyChanged;

    public MainModel()
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

    string _FtpCommand;
    public string FtpCommand
    {
      get
      {
        return _FtpCommand;
        if (_FtpCommand == null)
        {
          var item = this.FtpCommandRecentList.MostRecent;
          if (item != null)
            this.FtpCommand = item.ItemText;
        }
      }
      set
      {
        string s1 = value.TrimWhitespace();
        if (s1 != _FtpCommand)
        {
          _FtpCommand = s1;

          // apply the FtpCommand to the recently used list.
          // ( it the string is in the list, update the last use DateTime.
          //   If not in the list, add it. )
          if (s1.IsNullOrEmpty() == false)
          {
            this.FtpCommandRecentList.ApplyJustUsed(this.FtpCommand);
            this.FtpCommandShowList_Refill();
          }

          // signal to subscribers that FtpCommand has changed.
          if (this.FtpCommandChanged != null)
          {
            this.FtpCommandChanged(this);
          }

          if (s1.IsNullOrEmpty() == false)
          {
            if (this.FtpCommandRecentList_SelectedItem == null)
            {
              this.FtpCommandRecentList_SelectedItem = this.FtpCommand;
            }
          }
          RaisePropertyChanged("FtpCommand");
        }
      }
    }
    public event Action<MainModel> FtpCommandChanged;

    RecentUsedList _FtpCommandRecentList;
    public RecentUsedList FtpCommandRecentList
    {
      set
      {
        _FtpCommandRecentList = value;
        FtpCommandShowList_Refill();
        string s1 = this.FtpCommand;
      }
      get
      {
        if (_FtpCommandRecentList == null)
          _FtpCommandRecentList = new RecentUsedList();
        return _FtpCommandRecentList;
      }
    }

    private void FtpCommandShowList_Refill()
    {
      var items =
        from c in this.FtpCommandRecentList
        orderby c.LastUsedDate descending
        select c.ItemText;

      this.FtpCommandShowList.Clear();

      foreach (var item in items)
      {
        this.FtpCommandShowList.Add(item);
      }

      RaisePropertyChanged("FtpCommandShowList");
    }

    private ObservableCollection<string> _FtpCommandShowList;
    public ObservableCollection<string> FtpCommandShowList
    {
      get
      {
        if (_FtpCommandShowList == null)
        {
          _FtpCommandShowList = new ObservableCollection<string>();
          this.FtpCommandShowList_Refill();
        }
        return _FtpCommandShowList;
      }
      set
      {
        _FtpCommandShowList = value;
      }
    }

    string _FtpCommandRecentList_SelectedItem;
    public string FtpCommandRecentList_SelectedItem
    {
      get
      {
        if (_FtpCommandRecentList_SelectedItem == null)
        {
          var item = this.FtpCommandRecentList.MostRecent;
          if (item != null)
            _FtpCommandRecentList_SelectedItem = item.ItemText;
        }
        return _FtpCommandRecentList_SelectedItem;
      }
      set
      {
        if ((value != null) && (value != _FtpCommandRecentList_SelectedItem))
        {
          _FtpCommandRecentList_SelectedItem = value;
          this.FtpCommand = value;
          RaisePropertyChanged("FtpCommandRecentList_SelectedItem");
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

          if (this.IpAddrChanged != null)
          {
            this.IpAddrChanged(this);
          }
        }
      }
    }
    public event Action<MainModel> IpAddrChanged;

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
    public event Action<MainModel> UserNameChanged;

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

    public static MainModel ToModel(TextDemoSettings Settings)
    {
      var model = new MainModel()
      {
        UserName = Settings.UserName,
        IpAddr = Settings.IpAddr,
        Password = Settings.Password,
        FtpCommandRecentList = Settings.FtpCommandRecentList
      };

      return model;
    }
  }


}
