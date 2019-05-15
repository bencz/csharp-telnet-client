using AutoCoder.Ext;
using AutoCoder.Windows.Ext;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Xml.Linq;

namespace AutoCoder.Telnet.Settings
{
  public interface IClientSettings
  {
    bool AutoConnect { get; set; }
    string DataStreamName { get; set; }
    double FontPointSize { get; set; }
    string UserName { get; set; }
    string Password { get; set; }
    string OdbcDsn { get; set;  }
    string ScreenDefnPath { get; set; }
    string TerminalType { get; set; }
    string CaptureFolderPath { get; set; }
    string RunTimeCodePath { get; set; }
    string SupportCodeFilePath { get; set; }
    bool CaptureAuto { get; set; }
    string IpAddr { get; set; }
    NamedDataStreamList NamedDataStreamList { get; set; }
    string ParseText { get; set; }
    IList<string> SystemList { get; set; }
    string SystemName { get; set; }
    string DeviceName { get; set; }
    IList<string>DeviceNameList { get; set; }
    Size WindowClientSize { get; set; }

  }

  public static class IClientSettingsExt
  {
    public static void Apply(this IClientSettings to, IClientSettings from)
    {
      to.AutoConnect = from.AutoConnect;
      to.DataStreamName = from.DataStreamName;
      to.FontPointSize = from.FontPointSize;
      to.ScreenDefnPath = from.ScreenDefnPath;
      to.TerminalType = from.TerminalType;
      to.CaptureAuto = from.CaptureAuto;
      to.CaptureFolderPath = from.CaptureFolderPath;
      to.ParseText = from.ParseText;
      to.RunTimeCodePath = from.RunTimeCodePath;
      to.SupportCodeFilePath = from.SupportCodeFilePath;
      to.UserName = from.UserName;
      to.Password = from.Password;
      to.OdbcDsn = from.OdbcDsn;
      to.IpAddr = from.IpAddr;
      to.SystemName = from.SystemName;
      to.DeviceName = from.DeviceName;
      to.DeviceNameList = from.DeviceNameList.ToList();
      to.NamedDataStreamList = from.NamedDataStreamList;
      to.SystemList = from.SystemList.Where(c => c != "<add>").ToList();
      to.WindowClientSize = from.WindowClientSize;
    }

    public static void ApplyXElement(this IClientSettings to, XNamespace ns, XElement xe)
    {
      to.AutoConnect = xe.Element(ns + "AutoConnect").BooleanOrDefault(false).Value;
      to.ScreenDefnPath = xe.Element(ns + "ScreenDefnPath").StringOrDefault(null);
      to.CaptureFolderPath = xe.Element(ns + "CaptureFolderPath").StringOrDefault(null);
      to.CaptureAuto = xe.Element(ns + "CaptureAuto").BooleanOrDefault(false).Value;
      to.DataStreamName = xe.Element(ns + "DataStreamName").StringOrDefault();
      to.FontPointSize = xe.Element(ns + "FontPointSize").DoubleOrDefault(16).Value;
      to.TerminalType = xe.Element(ns + "TerminalType").StringOrDefault(null);
      to.RunTimeCodePath = xe.Element(ns + "RunTimeCodePath").StringOrDefault(null);
      to.SupportCodeFilePath = xe.Element(ns + "SupportCodeFilePath").StringOrDefault(null);
      to.UserName = xe.Element(ns + "UserName").StringOrDefault();
      to.Password = xe.Element(ns + "Password").StringOrDefault();
      to.IpAddr = xe.Element(ns + "IpAddr").StringOrDefault();
      to.OdbcDsn = xe.Element(ns + "OdbcDsn").StringOrDefault();
      to.ParseText = xe.Element(ns + "ParseText").StringOrDefault();
      to.SystemName = xe.Element(ns + "SystemName").StringOrDefault(null);
      to.SystemList = xe.Element(ns + "SystemList").ToIEnumerableString("System").ToList();
      to.DeviceName = xe.Element(ns + "DeviceName").StringOrDefault(null);
      to.DeviceNameList = xe.Element(ns + "DeviceNameList").ToIEnumerableString("Item").ToList();
      to.WindowClientSize = xe.Element(ns + "WindowClientSize")
                              .SizeOrDefault(new Size(0, 0)).Value;
    }

    public static XElement ToXElement(this IClientSettings settings, XName name)
    {
      XElement xml = new XElement(name,
        settings.WindowClientSize.ToXElement("WindowClientSize"),
        new XElement("UserName", settings.UserName),
        new XElement("Password", settings.Password),
        settings.SystemList.RemoveEmptyOrBlankItems().ToXElement("SystemList", "System"),
        new XElement("SystemName", settings.SystemName),
        new XElement("ScreenDefnPath", settings.ScreenDefnPath),
        new XElement("CaptureFolderPath", settings.CaptureFolderPath),
        new XElement("RunTimeCodePath", settings.RunTimeCodePath),
        new XElement("SupportCodeFilePath", settings.SupportCodeFilePath),
        new XElement("CaptureAuto", settings.CaptureAuto),
        new XElement("ParseText", settings.ParseText),
        new XElement("DataStreamName", settings.DataStreamName),
        settings.NamedDataStreamList.ToXElement("NamedDataStreamList"),
        new XElement("AutoConnect", settings.AutoConnect),
        new XElement("DeviceName", settings.DeviceName),
        settings.DeviceNameList.ToXElementNew("DeviceNameList"),
        new XElement("TerminalType", settings.TerminalType),
        new XElement("FontPointSize", settings.FontPointSize),
        new XElement("OdbcDsn", settings.OdbcDsn),
        new XElement("IpAddr", settings.IpAddr));
     
      return xml;
    }
  }
}
