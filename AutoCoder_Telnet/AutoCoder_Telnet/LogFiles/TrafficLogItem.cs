using AutoCoder.Ext;
using AutoCoder.Ext.System;
using AutoCoder.Telnet.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace AutoCoder.Telnet.LogFiles
{
  public class TrafficLogItem
  {
    public DateTime LogTime
    { get; set; }

    public byte[] ByteStream
    { get; set; }

    public Direction Direction
    { get; set; }

    public TrafficLogItem(Direction Direction, byte[] ByteStream)
      : this(Direction, ByteStream, DateTime.Now) 
    {
    }

    public TrafficLogItem(Direction Direction, byte[] ByteStream, DateTime LogTime)
    {
      this.LogTime = LogTime;
      this.Direction = Direction;
      this.ByteStream = ByteStream;
    }

    public XElement ToXElement( string Name )
    {
      XElement xml = new XElement(Name,
        new XElement("Direction", Direction.ToString()),
        new XElement("LogTime", LogTime.ToString("yyyy-MM-dd HH.mm.ss.ffff")),
        new XElement("ByteStream", ByteStream.ToHex(' ')));
      return xml;
    }

    public override string ToString()
    {
      var x1 = this.ToXElement("TrafficLogItem");
      return x1.ToString();
    }
  }

  public static class TrafficLogItemExt
  {
    public static TrafficLogItem ToTrafficLogItem(this string XmlText)
    {
      TrafficLogItem item = null;
      XElement xe = XElement.Parse(XmlText);
      var ns = xe.GetDefaultNamespace();
      var dir = xe.Element("Direction").StringOrDefault("none").TryParseDirection().Value;
      var x2 = xe.Element("LogTime").StringOrDefault("").TryParseDateTimeExact("yyyy-MM-dd HH.mm.ss.ffff");
      var x3 = xe.Element("ByteStream").StringOrDefault("").HexTextToBytes();

      item = new TrafficLogItem(dir, x3, x2.Value);
      return item;
    }
  }
}
