using AutoCoder.Ext;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace TelnetTester.Common
{
  public class NamedDataStream
  {
    public string Name
    { get; set; }

    public IEnumerable<string> DataStreamLines
    { get; set; }

    public NamedDataStream()
    {

    }

    public NamedDataStream(string Name, IEnumerable<string> DataStreamLines)
    {
      this.Name = Name;
      this.DataStreamLines = DataStreamLines;
    }
    public override string ToString()
    {
      return this.Name + " number lines:" + this.DataStreamLines.Count();
    }
  }

  public static class NamedDataStreamExt
  {
    public static XElement ToXElement(this NamedDataStream Model, XName Name)
    {
      if (Model == null)
        return new XElement(Name, null);
      else
      {
        XElement xe = new XElement(Name,
            new XElement("Name", Model.Name),
            Model.DataStreamLines.ToXElement("DataStreamLines", "Item")
            );
        return xe;
      }
    }

    public static NamedDataStream ToNamedDataStream(
      this XElement Elem, XNamespace Namespace)
    {
      NamedDataStream model = null;
      if (Elem != null)
      {
        model = new NamedDataStream();
        model.Name = Elem.Element(Namespace + "Name").StringOrDefault("");
        model.DataStreamLines = 
          Elem.ToEnumerableOfString(Namespace,"DataStreamLines", "Item").ToList();
      }
      return model;
    }
  }
}

