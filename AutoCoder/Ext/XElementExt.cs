using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.Collections.ObjectModel;

namespace AutoCoder.Ext
{
  public static class XElementExt
  {
    public static XElement FindElement(
      this IEnumerable<XElement> Elements, string LocalName)
    {
      XElement foundElem = null;
      foreach (XElement elem in Elements)
      {
        if (elem.Name.LocalName == LocalName)
        {
          foundElem = elem;
          break;
        }
        else if (elem.HasElements)
        {
          foundElem = elem.Elements().FindElement(LocalName);
          if (foundElem != null)
            break;
        }
      }
      return foundElem;
    }

    public static bool? BooleanOrDefault(
      this XElement Element, bool? Default = null)
    {
      if (Element == null)
        return Default;
      else if (Element.Value.Trim().Length == 0)
        return Default;
      else
      {
        bool bv;
        bool rc = Boolean.TryParse(Element.Value, out bv);
        if (rc == true)
          return bv;
        else
          return Default;
      }
    }

    public static DateTime? DateTimeOrDefault(
      this XElement Element, DateTime? Default = null)
    {
      if (Element == null)
        return Default;
      else if (Element.Value.Trim().Length == 0)
        return Default;
      else
      {
        DateTime dt;
        bool rc = DateTime.TryParse(Element.Value, out dt);
        if (rc == true)
          return dt;
        else
          return Default;
      }
    }

    public static decimal? DecimalOrDefault(
      this XElement Element, decimal? Default = null)
    {
      if (Element == null)
        return Default;
      else if (Element.Value.Trim().Length == 0)
        return Default;
      else
      {
        decimal dv;
        bool rc = Decimal.TryParse(Element.Value, out dv);
        if (rc == true)
          return dv;
        else
          return Default;
      }
    }

    public static double? DoubleOrDefault(
      this XElement Element, double? Default = null)
    {
      if (Element == null)
        return Default;
      else if (Element.Value.Trim().Length == 0)
        return Default;
      else
      {
        double dv;
        bool rc = Double.TryParse(Element.Value, out dv);
        if (rc == true)
          return dv;
        else
          return Default;
      }
    }

    /// <summary>
    /// perform the reverse of IEnumerable<string> to XElement. 
    /// </summary>
    /// <param name="Elem"></param>
    /// <param name="Namespace"></param>
    /// <param name="ElemName"></param>
    /// <returns></returns>
    public static IEnumerable<string> EnumerableStringOrDefault(
      this XElement Elem, XNamespace Namespace,
      IEnumerable<string> Default = null)
    {
      IEnumerable<string> rv = null;
      if (Elem == null)
        rv = Default;
      else
      {
        rv =
          from sam in Elem.Element(Namespace + "Items")
          .Elements(Namespace + "Item")
          select sam.Value;
      }
      return rv;
    }

    public static T? EnumOrDefault<T>(this XElement Element, T? Default = null) where T: struct
    {
      if (Element == null)
        return Default;
      else
      {
        T enumValue ;
        var s1 = Element.StringOrDefault("");
        var rc = Enum.TryParse<T>(s1, out enumValue);
        if (rc == false)
          return Default;
        else
          return enumValue;
      }
    }

    public static int? IntOrDefault(
      this XElement Element, int? Default = null)
    {
      if (Element == null)
        return Default;
      else if (Element.Value.Trim().Length == 0)
        return Default;
      else
      {
        int iv;
        bool rc = Int32.TryParse(Element.Value, out iv);
        if (rc == true)
          return iv;
        else
          return Default;
      }
    }

    public static Guid? GuidOrDefault(
      this XElement Element, Guid? Default = null)
    {
      if (Element == null)
        return Default;
      else if (Element.Value.Trim().Length == 0)
        return Default;
      else
      {
        Guid guid;
        bool rc = Guid.TryParse(Element.Value, out guid);
        if (rc == true)
          return guid;
        else
          return Default;
      }
    }

    public static List<string> ListStringOrDefault(
      this XElement Element, XNamespace Namespace, string ItemName,
      List<string> Default = null)
    {
      if (Element == null)
        return Default;
      else
      {
        List<string> listString = new List<string>();

        var items =
          from x in Element.Elements(Namespace + ItemName)
          select x.StringOrDefault();

        foreach (var item in items)
        {
          listString.Add(item);
        }

        return listString;
      }
    }

    public static ObservableCollection<string> ObservableCollectionStringOrDefault(
      this XElement Element, XNamespace Namespace,
      ObservableCollection<string> Default = null)
    {
      if (Element == null)
        return Default;
      else
      {
        var es = EnumerableStringOrDefault(Element, Namespace, null);
        return es.ToObservableCollectionString();
      }
    }

    /// <summary>
    /// Find the child element of this parent XElement. If found, replace the
    /// value of the element. If not found, store the value as a new child element
    /// of the parent.
    /// </summary>
    /// <param name="Parent"></param>
    /// <param name="NS"></param>
    /// <param name="ChildName"></param>
    /// <param name="ChildValue"></param>
    /// <returns></returns>
    public static Tuple<XElement, bool> ReplaceOrAddChildElement(
      this XElement Parent, 
      XNamespace NS, string ChildName, string ChildValue)
    {
      bool wasAdded = false;
      XElement ce = Parent.Element(NS + ChildName);
      if (ce == null)
      {
        ce = new XElement(NS + ChildName, ChildValue);
        Parent.Add(ce);
        wasAdded = true;
      }
      else
      {
        ce.Value = ChildValue;
        wasAdded = false;
      }

      return new Tuple<XElement, bool>(ce, wasAdded);
    }


    public static string StringOrDefault(this XElement Element, string Default = "")
    {
      if (Element == null)
        return Default;
      else
        return Element.Value;
    }

    /// <summary>
    /// return the this XElement. Or return a new, empty XElement. 
    /// Used in LINQ to XML expressions when it is not known that an Elements method
    /// will find and return an XElement.
    /// </summary>
    /// <param name="Element"></param>
    /// <param name="EmptyName"></param>
    /// <returns></returns>
    public static XElement XElementOrEmpty(this XElement Element, string EmptyName)
    {
      if (Element == null)
      {
        object[] emptyList = new object[0];
        XElement elem = new XElement(EmptyName, emptyList);
        return elem;
      }
      else
      {
        return Element;
      }
    }

    /// <summary>
    /// perform the reverse of IEnumerable<string> to XElement. 
    /// </summary>
    /// <param name="Elem"></param>
    /// <param name="Namespace"></param>
    /// <param name="ElemName"></param>
    /// <returns></returns>
    public static IEnumerable<string> ToEnumerableOfString(
      this XElement Elem, XNamespace Namespace, string ElemName)
    {
      IEnumerable<string> rv = null;

      XElement elemCode = Elem.Element(Namespace + ElemName);
      if (elemCode != null)
      {
        rv =
          from sam in elemCode.Element(Namespace + "Items")
          .Elements(Namespace + "Item")
          select sam.Value;
      }
      return rv;
    }

    public static IEnumerable<string> ToEnumerableOfString(
      this XElement Elem, XNamespace Namespace, string ListName, string ItemName)
    {
      IEnumerable<string> rv = null;

      XElement listElem = Elem.Element(Namespace + ListName);
      if (listElem != null)
      {
        rv =
          from c in listElem.Elements(Namespace + ItemName)
          select c.Value ;
      }
      return rv;
    }
  }
}
