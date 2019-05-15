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
using System.Diagnostics;

namespace AutoCoder.Telnet.Settings
{
  public class ClientSettings : IXmlSerializable, IClientSettings
  {
    public static string ClassName = "ClientSettings";
    public bool AutoConnect { get; set; }
    public string DeviceName { get; set; }
    public IList<string> DeviceNameList { get; set; }

    public double FontPointSize
    {
      get
      {
        if (_FontPointSize == 0)
          _FontPointSize = 12;
        return _FontPointSize;
      }
      set { _FontPointSize = value; }
    }
    double _FontPointSize;
    public string IpAddr { get; set; }
    public string UserName { get; set; }
    public string Password { get; set; }
    public string OdbcDsn { get; set; }
    public Size WindowClientSize { get; set; }

    double? _PreviewPaneWidth;
    public double? PreviewPaneWidth
    {
      get { return _PreviewPaneWidth; }
      set { _PreviewPaneWidth = value; }
    }

    public string ScreenDefnPath { get; set;  }
    public string TerminalType { get; set; }
    public string CaptureFolderPath { get; set; }

    /// <summary>
    /// run time executable assemblies placed in RunTimeCodePath.
    /// </summary>
    public string RunTimeCodePath { get; set; }
    
    /// <summary>
    /// path to file that contains script support library code. When runtime code
    /// is compiled the support code is included in the compile package.
    /// </summary>
    public string SupportCodeFilePath { get; set; }
    /// <summary>
    /// auto capture to capture database on every match.
    /// </summary>
    public bool CaptureAuto { get; set; }

    public string SystemName { get; set; }
    public List<string> SystemList { get; set; }
    IList<string> IClientSettings.SystemList
    {
      get
      {
        return this.SystemList;
      }

      set
      {
        this.SystemList = value.ToList();
      }
    }

    public string ParseText { get; set; }
    public string DataStreamName
    { get; set; }

    private NamedDataStreamList _NamedDataStreamList;
    public NamedDataStreamList NamedDataStreamList
    {
      get
      {
        if (_NamedDataStreamList == null)
          _NamedDataStreamList = new NamedDataStreamList();
        return _NamedDataStreamList;
      }
      set { _NamedDataStreamList = value; }
    }

    public ClientSettings()
    {
    }

    public System.Xml.Schema.XmlSchema GetSchema()
    {
      throw new NotImplementedException();
    }

    private static string BuildApplDataDir()
    {
      string vendorName = "AutoCoder";
      string solutionName = "tnClient";
      string projectName = "tnClient";

      string applDataDir =
        Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
      applDataDir = Path.Combine(applDataDir, vendorName);
      applDataDir = Path.Combine(applDataDir, solutionName);
      applDataDir = Path.Combine(applDataDir, projectName);

      return applDataDir;
    }

    public static string GetCachedOdbcDsn( )
    {
      if (_CachedOdbcDsn == null)
      {
        var settings = ClientSettings.RecallSettings();
        _CachedOdbcDsn = settings.OdbcDsn;
      }
      return _CachedOdbcDsn;
    }
    static string _CachedOdbcDsn;

    /// <summary>
    /// return the runtime script support library code. 
    /// </summary>
    /// <returns></returns>
    public static string GetCachedSupportCode( )
    {
      if (_CachedSupportCode == null)
      {
        var settings = ClientSettings.RecallSettings();
        if (settings.SupportCodeFilePath.IsNullOrEmpty() == true)
          _CachedSupportCode = "";
        else
          _CachedSupportCode = 
            System.IO.File.ReadAllText(settings.SupportCodeFilePath);
      }
      return _CachedSupportCode;
    }
    static string _CachedSupportCode;

    public static ClientSettings RecallSettings()
    {
      ClientSettings settings = new ClientSettings();
      string applDataDir = ClientSettings.BuildApplDataDir();

      Pather.AssureDirectoryExists(applDataDir);
      string applStateFileName = "ClientSettings.xml";
      string applStatePath = Path.Combine(applDataDir, applStateFileName);

      if (System.IO.File.Exists(applStatePath) == true)
      {
        try
        {
          using (XmlTextReader tr = new XmlTextReader(applStatePath))
          {
            settings.ReadXml(tr);
          }
        }
        catch (Exception excp)
        {
          Debug.Print(excp.ToString());
          throw excp;
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
      ClientSettings settings = new ClientSettings();
      string applDataDir = ClientSettings.BuildApplDataDir();

      Pather.AssureDirectoryExists(applDataDir);
      string applStateFileName = "ClientSettings.xml";
      string applStatePath = Path.Combine(applDataDir, applStateFileName);

      if (System.IO.File.Exists(applStatePath) == true)
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
      string applDataDir = ClientSettings.BuildApplDataDir();
      string applStateFileName = "ClientSettings.xml";
      string applStatePath = Path.Combine(applDataDir, applStateFileName);

      using (StreamWriter stmw = new StreamWriter(applStatePath))
      {
        XmlSerializer xmlser = new XmlSerializer(typeof(ClientSettings));
        xmlser.Serialize(stmw, this);  // calls WriteXml
      }

      ClientSettings._CachedOdbcDsn = this.OdbcDsn;
    }

    public void ReadXml(XmlReader Reader)
    {
      XDocument doc = XDocument.Load(Reader);
      XNamespace ns = doc.Root.Name.Namespace;
      var docRoot = doc.Root.Elements().First();
      XElement xe = doc.Element(ns + ClientSettings.ClassName);
      xe = xe.Element(ns + ClientSettings.ClassName);

      this.NamedDataStreamList = xe.Element(ns + "NamedDataStreamList").ToNamedDataStreamList(ns);

      this.ApplyXElement(ns, xe);
    }

    public void WriteXml(XmlWriter Writer)
    {
      var xml = this.ToXElement(ClientSettings.ClassName);
      xml.WriteTo(Writer);
    }
  }
}
