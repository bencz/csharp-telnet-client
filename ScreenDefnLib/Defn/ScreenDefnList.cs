using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace ScreenDefnLib.Defn
{
  public class ScreenDefnList : List<IScreenDefn>
  {
    public ScreenDefnList()
    {
    }

  public ScreenDefnList( IEnumerable<IScreenDefn> List)
    {
      if ( List != null )
        this.AddRange(List);
    }
    public void Apply(IScreenDefn screenDefn)
    {
      var item = this.FirstOrDefault(c => c.ScreenName == screenDefn.ScreenName);
      if (item == null)
        this.Add(screenDefn);
      else
      {
        this.Remove(item);
        this.Add(screenDefn);
      }
    }

    public static ScreenDefnList ReadXml(string XmlText)
    {
      var doc = XDocument.Parse(XmlText);
      XNamespace ns = doc.Root.Name.Namespace;
      XElement xe = doc.Element(ns + "ScreenDefnList");
      var screenDefnList = xe.ToScreenDefnList(ns);
      return screenDefnList;
    }

    public static ScreenDefnList RecallFromXmlFile(string ScreenDefnPath)
    {
      ScreenDefnList defnList = null;

      if (File.Exists(ScreenDefnPath) == true)
      {
        try
        {
          var text = System.IO.File.ReadAllText(ScreenDefnPath);
          defnList = ScreenDefnList.ReadXml(text);
        }
        catch (Exception excp)
        {
          Debug.Print(excp.ToString());
          throw excp;
        }
      }

      return defnList;
    }
    public void StoreToXmlFile(string ScreenDefnPath)
    {
      var xe = this.ToXElement("ScreenDefnList");

      XDocument xdoc = new XDocument(
      new XDeclaration("1.0", "utf-8", "yes"),
      new XComment("tnClient screen definitions"),
      xe);
      xdoc.Save(ScreenDefnPath);
    }
  }

  public static class ScreenDefnListExt
  {
    public static XElement ToXElement(this ScreenDefnList Model, XName Name)
    {
      if (Model == null)
        return new XElement(Name);
      else
      {

        return new XElement(Name,
          from c in Model
          select c.ToXElement("ScreenDefn")
          );
      }
    }
    public static ScreenDefnList ToScreenDefnList(
      this XElement Elem, XNamespace Namespace)
    {
      if (Elem == null)
        return new ScreenDefnList();
      else
      {
        var sl = from c in Elem.Elements(Namespace + "ScreenDefn")
                 select c.ToScreenDefn(Namespace);

        var defnList = new ScreenDefnList();
        foreach (var sf in sl)
        {
          defnList.Add(sf);
        }

        return defnList;
      }
    }
  }
}

