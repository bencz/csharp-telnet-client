using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using AutoCoder.Ext;

namespace TelnetTester.Common
{
  public class NamedDataStreamList : List<NamedDataStream>
  {
    public void Apply( NamedDataStream DataStream)
    {
      var item = this.FirstOrDefault(c => c.Name == DataStream.Name);
      if (item == null)
        this.Add(DataStream);
      else
      {
        this.Remove(item);
        this.Add(DataStream);
      }
    }
  }

  public static class NamedDataStreamListExt
  {
    public static XElement ToXElement(this NamedDataStreamList Model, XName Name)
    {

      if (Model == null)
        return new XElement(Name);
      else
      {

        return new XElement(Name,
          from c in Model
          select c.ToXElement("NamedDataStream")
          );
      }
    }
    public static NamedDataStreamList ToNamedDataStreamList(
      this XElement Elem, XNamespace Namespace)
    {
      if (Elem == null)
        return new NamedDataStreamList();
      else
      {
        var sl = from c in Elem.Elements(Namespace + "NamedDataStream")
                 select c.ToNamedDataStream(Namespace);

        var recentList = new NamedDataStreamList();
        foreach (var sf in sl)
        {
          recentList.Add(sf);
        }

        return recentList;
      }
    }

  }
}
