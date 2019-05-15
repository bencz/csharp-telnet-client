using AutoCoder.Ext;
using AutoCoder.Ext.System;
using AutoCoder.File;
using AutoCoder.Windows.Ext;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;
using TelnetTester.Common;

namespace TelnetTester.Settings
{
  public class TesterSettings : IXmlSerializable, INotifyPropertyChanged
  {
    public static string ClassName = "TesterSettings";

    // Declare the event
    public event PropertyChangedEventHandler PropertyChanged;

    public bool AutoConnect { get; set; }

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

    public string DataStreamName
    { get; set; }

    public NamedDataStreamList NamedDataStreamList { get; set; }

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

    public string SystemName { get; set; }
    public List<string> SystemList { get; set; }
    public string TextFilePath { get; set; }

    public TesterSettings()
    {
    }

    public System.Xml.Schema.XmlSchema GetSchema()
    {
      throw new NotImplementedException();
    }

    private static string BuildApplDataDir()
    {
      string vendorName = "AutoCoder";
      string solutionName = "AutoCoder_Telnet";
      string projectName = "TelnetTester";

      string applDataDir =
        Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
      applDataDir = Path.Combine(applDataDir, vendorName);
      applDataDir = Path.Combine(applDataDir, solutionName);
      applDataDir = Path.Combine(applDataDir, projectName);

      return applDataDir;
    }

    public static TesterSettings RecallSettings()
    {
      TesterSettings settings = new TesterSettings();
      string applDataDir = TesterSettings.BuildApplDataDir();

      Pather.AssureDirectoryExists(applDataDir);
      string applStateFileName = "TesterSettings.xml";
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
          settings = new TesterSettings();
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
      TesterSettings settings = new TesterSettings();
      string applDataDir = TesterSettings.BuildApplDataDir();

      Pather.AssureDirectoryExists(applDataDir);
      string applStateFileName = "TesterSettings.xml";
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
      string applDataDir = TesterSettings.BuildApplDataDir();
      string applStateFileName = "TesterSettings.xml";
      string applStatePath = Path.Combine(applDataDir, applStateFileName);

      using (StreamWriter stmw = new StreamWriter(applStatePath))
      {
        XmlSerializer xmlser = new XmlSerializer(typeof(TesterSettings));
        xmlser.Serialize(stmw, this);
      }
    }

    public void ReadXml(XmlReader Reader)
    {
      XDocument doc = XDocument.Load(Reader);
      XNamespace ns = doc.Root.Name.Namespace;
      var docRoot = doc.Root.Elements().First();
      XElement xe = doc.Element(ns + TesterSettings.ClassName);
      xe = xe.Element(ns + TesterSettings.ClassName);

      this.ParseText = xe.Element(ns + "ParseText").StringOrDefault();
      this.DataStreamName = xe.Element(ns + "DataStreamName").StringOrDefault();
      this.NamedDataStreamList = xe.Element(ns + "NamedDataStreamList").ToNamedDataStreamList(ns);

      this.UserName = xe.Element(ns + "UserName").StringOrDefault();
      this.Password =
        xe.Element(ns + "Password").StringOrDefault();

      this.WindowClientSize =
        docRoot.Element(ns + "WindowClientSize").SizeOrDefault(new Size(0, 0)).Value;

      this.IpAddr =
        xe.Element(ns + "IpAddr").StringOrDefault();

      this.SystemList = xe.Element(ns + "SystemList").ToIEnumerableString("System").ToList();
      this.SystemName = xe.Element(ns + "SystemName").StringOrDefault(null);

      this.AutoConnect = xe.Element(ns + "AutoConnect").BooleanOrDefault(false).Value;


      this.TextFilePath = xe.Element(ns + "TextFilePath").StringOrDefault(null);
    }

    public void WriteXml(XmlWriter Writer)
    {
      XElement xml = new XElement(TesterSettings.ClassName,
        this.WindowClientSize.ToXElement("WindowClientSize"),
        new XElement("ParseText", this.ParseText),
        new XElement("DataStreamName", this.DataStreamName),
        this.NamedDataStreamList.ToXElement("NamedDataStreamList"),
        new XElement("UserName", this.UserName),
        new XElement("Password", this.Password),
        this.SystemList.RemoveEmptyOrBlankItems().ToXElement("SystemList", "System"),
        new XElement("SystemName", this.SystemName),
        new XElement("TextFilePath", this.TextFilePath),
        new XElement("AutoConnect", this.AutoConnect),
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
