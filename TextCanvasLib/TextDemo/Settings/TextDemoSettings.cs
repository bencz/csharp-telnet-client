using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoCoder.Collections;
using AutoCoder.Ext.System;
using AutoCoder.Windows.Ext;
using System.Xml.Serialization;
using System.ComponentModel;
using System.Windows;
using AutoCoder.File;
using System.IO;
using System.Xml;
using System.Xml.Linq;
using AutoCoder.Ext;

namespace TextDemo.Settings
{
  public class TextDemoSettings : IXmlSerializable, INotifyPropertyChanged
  {
    public static string ClassName = "TextDemoSettings";

    // Declare the event
    public event PropertyChangedEventHandler PropertyChanged;

    RecentUsedList _FtpCommandRecentList;
    public RecentUsedList FtpCommandRecentList
    {
      get
      {
        if (_FtpCommandRecentList == null)
          _FtpCommandRecentList = new RecentUsedList()
          {
            RetentionLimit = 10
          };
        return _FtpCommandRecentList;
      }
      set
      {
        _FtpCommandRecentList = value;
        if ((this.FtpCommandRecentList != null)
          && (this.FtpCommandRecentList.RetentionLimit == null))
          this.FtpCommandRecentList.RetentionLimit = 10;
      }
    }

    private string _IpAddr;
    public string IpAddr
    {
      get { return _IpAddr; }
      set
      {
        string s1;
        if (value == null)
          s1 = null;
        else
          s1 = value.Trim(new char[] { ' ', '\t', '\r', '\n' });
        if (_IpAddr != s1)
        {
          _IpAddr = s1;
          RaisePropertyChanged("IpAddr");
        }
      }
    }

    string _UserName;
    public string UserName
    {
      get { return _UserName; }
      set
      {
        string av = value.TrimWhitespace();
        if (av != _UserName)
        {
          _UserName = av;
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
        string av = value.TrimWhitespace();
        if (av != _Password)
        {
          _Password = av;
          RaisePropertyChanged("Password");
        }
      }
    }

    Size _WindowClientSize;
    public Size WindowClientSize
    {
      get { return _WindowClientSize; }
      set { _WindowClientSize = value; }
    }

    double? _PreviewPaneWidth;
    public double? PreviewPaneWidth
    {
      get { return _PreviewPaneWidth; }
      set { _PreviewPaneWidth = value; }
    }

    public TextDemoSettings()
    {
    }

    public System.Xml.Schema.XmlSchema GetSchema()
    {
      throw new NotImplementedException();
    }

    private static string BuildApplDataDir()
    {
      string vendorName = "AutoCoder";
      string solutionName = "TextCanvasLib";
      string projectName = "TextDemo";

      string applDataDir =
        Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
      applDataDir = Path.Combine(applDataDir, vendorName);
      applDataDir = Path.Combine(applDataDir, solutionName);
      applDataDir = Path.Combine(applDataDir, projectName);

      return applDataDir;
    }

    public static TextDemoSettings RecallSettings()
    {
      TextDemoSettings settings = new TextDemoSettings();
      string applDataDir = TextDemoSettings.BuildApplDataDir();

      Pather.AssureDirectoryExists(applDataDir);
      string applStateFileName = "TextDemoSettings.xml";
      string applStatePath = Path.Combine(applDataDir, applStateFileName);

      if (File.Exists(applStatePath) == true)
      {
        try
        {
          using (XmlTextReader tr = new XmlTextReader(applStatePath))
          {
            settings.ReadXml(tr);
          }
        }
        catch (Exception)
        {
          settings = new TextDemoSettings();
        }
      }

      return settings;
    }

    /// <summary>
    /// check that settings exist for the appl.
    /// </summary>
    /// <returns></returns>
    public static bool SettingsExist()
    {
      TextDemoSettings settings = new TextDemoSettings();
      string applDataDir = TextDemoSettings.BuildApplDataDir();

      Pather.AssureDirectoryExists(applDataDir);
      string applStateFileName = "TextDemoSettings.xml";
      string applStatePath = Path.Combine(applDataDir, applStateFileName);

      if (File.Exists(applStatePath) == true)
      {
        return true;
      }
      else
      {
        return false;
      }
    }

    public void StoreSettings()
    {
      string applDataDir = TextDemoSettings.BuildApplDataDir();
      string applStateFileName = "TextDemoSettings.xml";
      string applStatePath = Path.Combine(applDataDir, applStateFileName);

      using (StreamWriter stmw = new StreamWriter(applStatePath))
      {
        XmlSerializer xmlser = new XmlSerializer(typeof(TextDemoSettings));
        xmlser.Serialize(stmw, this);
      }
    }

    public void ReadXml(XmlReader Reader)
    {
      XDocument doc = XDocument.Load(Reader);
      XNamespace ns = doc.Root.Name.Namespace;
      var docRoot = doc.Root.Elements().First();
      XElement xe = doc.Element(ns + TextDemoSettings.ClassName);
      xe = xe.Element(ns + TextDemoSettings.ClassName);

      this.UserName = xe.Element(ns + "UserName").StringOrDefault();
      this.Password =
        xe.Element(ns + "Password").StringOrDefault();

      this.WindowClientSize =
        docRoot.Element(ns + "WindowClientSize").SizeOrDefault(new Size(0, 0)).Value;

      this.IpAddr =
        xe.Element(ns + "IpAddr").StringOrDefault();

      this.FtpCommandRecentList =
        xe.Element(ns + "FtpCommandRecentList").ToRecentUsedList(ns);
    }

    public void WriteXml(XmlWriter Writer)
    {
      XElement xml = new XElement(TextDemoSettings.ClassName,
        this.WindowClientSize.ToXElement("WindowClientSize"),
        this.FtpCommandRecentList.ToXElement("FtpCommandRecentList"),
        new XElement("UserName", this.UserName),
        new XElement("Password", this.Password),
        new XElement("IpAddr", this.IpAddr));

      xml.WriteTo(Writer);
    }

    // Create the RaisePropertyChanged method to raise the event
    protected void RaisePropertyChanged(string name)
    {
      PropertyChangedEventHandler handler = PropertyChanged;
      if (handler != null)
      {
        handler(this, new PropertyChangedEventArgs(name));
      }
    }

  }
}

