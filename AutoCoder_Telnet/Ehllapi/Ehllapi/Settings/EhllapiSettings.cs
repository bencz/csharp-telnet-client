using System;
using System.Collections.Generic;
// using System.Drawing;
using System.Windows ;
// using System.Windows.Forms;
using System.IO;
using System.Text;
using System.Xml.Serialization;
using System.Xml.Linq;
using AutoCoder.Text;
using AutoCoder.File;
using AutoCoder.Ext;
using System.Xml;
using System.Windows.Media;
using System.ComponentModel;
using AutoCoder.Ext.System;

namespace AutoCoder.Ehllapi.Settings
{
  public class EhllapiSettings : INotifyPropertyChanged, IXmlSerializable
  {
    public static string ClassName = "EhllapiSettings";

    Size mFormSize = new Size(0, 0);
    string mFontFamilyName = "Lucida Console" ;
    float mFontSize = 12;
    Color mForeColor = Colors.Black ;
    string mSystemName;
    string mUserName;
    string mPassword;
    string mSessId;
    string mPath_cwblogon;
    string mPath_pcsws;
    string mPath_WrkstnProfile;
    string _DirPath_Emulator;

    public EhllapiSettings()
    {
    }

    public static string DefaultPath_pcsws
    {
      get
      {
        string defaultPath =
          "C:\\Program Files\\IBM\\Client Access\\Emulator\\pcsws.exe";
        if (System.IO.File.Exists(defaultPath) == false)
          defaultPath = "";
        return defaultPath;
      }
    }

    public static string DefaultPath_cwblogon
    {
      get
      {
        string defaultPath =
          "C:\\Program Files\\IBM\\Client Access\\cwblogon.exe";
        if (System.IO.File.Exists(defaultPath) == false)
          defaultPath = "";
        return defaultPath;
      }
    }

    public static string DefaultPath_WrkstnProfile
    {
      get
      {
        // Steve/AppData/Roaming/IBM/Client Access/emulator/private/steve1.ws
        string defaultPath =
                 Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        if (System.IO.File.Exists(defaultPath) == false)
          defaultPath = "";
        return defaultPath;
      }
    }

    /// <summary>
    /// the path of the client access\emulator directory.
    /// This path varies depending on if 64 or 32 bit windows.
    /// This path is important. The DllImports for the ehllapi api functions are not
    /// fully qualified. This directory is added to the environment search path.
    /// </summary>
    public string DirPath_Emulator
    {
      get { return _DirPath_Emulator; }
      set
      {
        var s1 = value.TrimWhitespace();
        if (_DirPath_Emulator != s1)
        {
          _DirPath_Emulator = s1;
          RaisePropertyChanged("DirPath_Emulator");
        }
      }
    }

    public Size FormSize
    {
      get { return mFormSize; }
      set { mFormSize = value; }
    }

    public string FontFamilyName
    {
      get { return mFontFamilyName; }
      set
      {
        if (mFontFamilyName == value)
          return;
        else
        {
          mFontFamilyName = value;
          RaisePropertyChanged("FontFamilyName");
        }
      }
    }

    public Color ForeColor
    {
      get { return mForeColor; }
      set { mForeColor = value; }
    }

    public string ForeColor_string
    {
      get
      {
        ColorConverter d = new ColorConverter( ) ;
        string sColor = d.ConvertToString(mForeColor);
        return sColor;
      }
      set
      { 
        ColorConverter d = new ColorConverter( ) ;
        mForeColor = (Color)ColorConverter.ConvertFromString( value ) ;
      }
    }

    public float FontSize
    {
      get { return mFontSize; }
      set
      {
        if (mFontSize == value)
          return;
        else
        {
          mFontSize = value;
          RaisePropertyChanged("FontSize");
        }
      }
    }

    public string Password
    {
      get { return mPassword; }
      set 
      {
        string s1 = DefaultEmpty(value, "").ToUpper();
        if (mPassword == s1)
          return;
        else
        {
          mPassword = s1;
          RaisePropertyChanged("Password");
        }
      }
    }

    public string Path_cwblogon
    {
      get
      {
        return mPath_cwblogon;
      }
      set 
      {
        string defaultValue = EhllapiSettings.DefaultPath_cwblogon;

        string s1 = DefaultEmpty(value, defaultValue);
        if (mPath_cwblogon == s1)
          return;
        else
        {
          mPath_cwblogon = s1;
          RaisePropertyChanged("Path_cwblogon");
        }
      }
    }

    public string Path_pcsws
    {
      get
      {
        return mPath_pcsws;
      }
      set 
      {
        string defaultValue = EhllapiSettings.DefaultPath_pcsws;

        string s1 = DefaultEmpty(value, defaultValue);
        if (mPath_pcsws == s1)
          return;
        else
        {
          mPath_pcsws = s1;
          RaisePropertyChanged("Path_pcsws");
        }
      }
    }

    public string Path_WrkstnProfile
    {
      get { return mPath_WrkstnProfile; }
      set 
      {
        string defaultValue = EhllapiSettings.DefaultPath_WrkstnProfile;
        string s1 = DefaultEmpty(value, defaultValue);
        if (mPath_WrkstnProfile == s1)
          return;
        else
        {
          mPath_WrkstnProfile = s1;
          RaisePropertyChanged("Path_WrkstnProfile");
        }
      }
    }

    private static string DefaultEmpty(string Text, string DefaultValue = "")
    {
      string text = null;
      if (Text == null)
        text = "";
      else
        text = Text.TrimEnd(new char[] { ' ', '\t', '\r', '\n' });
      if (text.Length == 0)
        text = DefaultValue;
      return text;
    }

    public string SessId
    {
      get { return mSessId; }
      set 
      {
        string defaultValue = "";
        string s1 = DefaultEmpty(value, defaultValue).ToUpper();
        if (mSessId == s1)
          return;
        else
        {
          mSessId = s1;
          RaisePropertyChanged("SessId");
        }
      }
    }

    public string SystemName
    {
      get { return mSystemName; }
      set 
      {
        string s1 = DefaultEmpty(value, "").ToUpper();
        if (mSystemName == s1)
          return;
        else
        {
          mSystemName = s1;
          RaisePropertyChanged("SystemName");
        }
      }
    }

    public string UserName
    {
      get { return mUserName; }
      set
      {
        string s1 = DefaultEmpty(value, "").ToUpper();
        if (mUserName == s1)
          return;
        else
        {
          mUserName = s1;
          RaisePropertyChanged("UserName");
        }
      }
    }

    public Size WindowClientSize
    { get; set; }

    private static string BuildApplDataDir()
    {
      string vendorName = "AutoCoder";
      string solutionName = "Ehllapi";
      string projectName = "EhllapiLib";

      string applDataDir = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
      applDataDir = Path.Combine(applDataDir, vendorName);
      applDataDir = Path.Combine(applDataDir, solutionName);
      applDataDir = Path.Combine(applDataDir, projectName);

      return applDataDir;
    }

    public event PropertyChangedEventHandler PropertyChanged;
    private void RaisePropertyChanged(string propName)
    {
      PropertyChangedEventHandler eh = PropertyChanged;

      if (eh != null)
      {
        eh(this, new PropertyChangedEventArgs(propName));
      }
    }

    public static EhllapiSettings RecallSettings()
    {
      EhllapiSettings settings = new EhllapiSettings();
      string applDataDir = EhllapiSettings.BuildApplDataDir();

      Pather.AssureDirectoryExists(applDataDir);
      string applStateFileName = "EhllapiSettings.xml";
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
        catch (Exception)
        {
          settings = new EhllapiSettings();
        }
      }

      return settings;
    }

    public void StoreSettings()
    {
      string applDataDir = EhllapiSettings.BuildApplDataDir();
      string applStateFileName = "EhllapiSettings.xml";
      string applStatePath = Path.Combine(applDataDir, applStateFileName);

      using (StreamWriter stmw = new StreamWriter(applStatePath))
      {
        XmlSerializer xmlser = new XmlSerializer(typeof(EhllapiSettings));
        xmlser.Serialize(stmw, this);
      }
    }

    public void ReadXml(XmlReader Reader)
    {
      XDocument doc = XDocument.Load(Reader);
      XNamespace ns = doc.Root.Name.Namespace;
      XElement xe = doc.Element(ns + EhllapiSettings.ClassName);
      xe = xe.Element(ns + EhllapiSettings.ClassName);

      this.SystemName = xe.Element(ns + "SystemName").StringOrDefault();
      this.UserName = xe.Element(ns + "UserName").StringOrDefault();
      this.Password =
        xe.Element(ns + "Password").StringOrDefault();
      this.SessId =
        xe.Element(ns + "SessId").StringOrDefault();

      this.DirPath_Emulator =
        xe.Element(ns + "DirPath_Emulator").StringOrDefault();
      this.Path_WrkstnProfile =
        xe.Element(ns + "Path_WrkstnProfile").StringOrDefault();
      this.Path_cwblogon =
        xe.Element(ns + "Path_cwblogon").StringOrDefault();
      this.Path_pcsws =
        xe.Element(ns + "Path_pcsws").StringOrDefault();

      XElement elemSize = xe.Element("WindowClientSize");
      if (elemSize != null)
      {
        this.WindowClientSize = new Size(
          elemSize.Element(ns + "Width").DoubleOrDefault().Value,
          elemSize.Element(ns + "Height").DoubleOrDefault().Value
          );
      }
    }

    public void WriteXml(XmlWriter Writer)
    {
      XElement xml = new XElement(EhllapiSettings.ClassName,
        new XElement("SessId", this.SessId),
        new XElement("Path_cwblogon", this.Path_cwblogon),
        new XElement("Path_pcsws", this.Path_pcsws),
        new XElement("Path_WrkstnProfile", this.Path_WrkstnProfile),
        new XElement("DirPath_Emulator", this.DirPath_Emulator),
        new XElement("WindowClientSize",
          new XElement("Width", this.WindowClientSize.Width),
          new XElement("Height", this.WindowClientSize.Height)),
          
          new XElement("SystemName", this.SystemName),
          new XElement("UserName", this.UserName),
          new XElement("Password", this.Password));

      xml.WriteTo(Writer);
    }

    public System.Xml.Schema.XmlSchema GetSchema()
    {
      throw new NotImplementedException();
    }
  }
}

