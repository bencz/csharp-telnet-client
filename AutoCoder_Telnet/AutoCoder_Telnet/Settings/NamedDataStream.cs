using AutoCoder.Ext;
using AutoCoder.Ext.System;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace AutoCoder.Telnet.Settings
{
  public class NamedDataStream
  {

    private string _Name;
    public string Name
    {
      get { return _Name; }
      set
      {
        _Name = value.TrimEndWhitespace( );
      }
    }


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

    /// <summary>
    /// return true if the name of the data stream is empty. And the list of lines
    /// is also empty.
    /// </summary>
    /// <returns></returns>
    public bool IsEmpty( )
    {
      if ((this.Name.IsNullOrEmpty() == true)
        && (this.DataStreamLines.IsNullOrEmpty() == true))
        return true;
      else
        return false;
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
            Model.DataStreamLines.RemoveEmptyOrBlankItems( ).ToXElement("DataStreamLines", "Item")
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
          Elem.ToEnumerableOfString(Namespace, "DataStreamLines", "Item").ToList();
      }
      return model;
    }
  }
}

