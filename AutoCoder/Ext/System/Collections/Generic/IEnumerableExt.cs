using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.Collections.ObjectModel;
using AutoCoder.Ext.System;
using System.Diagnostics;
using AutoCoder.Systm;

namespace AutoCoder.Ext
{
  public static class IEnumerableExt
  {
    /// <summary>
    /// call Debug.Print for each string in the IEnumerable sequence.
    /// </summary>
    /// <param name="List"></param>
    public static void DebugPrint( this IEnumerable<string> List)
    {
      foreach( var line in List)
      {
        Debug.Print(line);
      }
    }

    /// <summary>
    /// convert the sequence of pairs of hex characters on one or more input lines
    /// to an array of bytes. On each text line, convert the hex character pairs until
    /// text which is not a hex char pair is found. 
    /// </summary>
    /// <param name="Lines"></param>
    /// <returns></returns>
    public static byte[] HexTextLinesToBytes(this IEnumerable<string> Lines)
    {
      var ba = new ByteArrayBuilder();
      foreach (var line in Lines)
      {
        // make sure the line is padded on right with at least 1 blank.
        var curLine = line.Trim() + "    ";

        // process 3 char chunks on  the current line.
        int ix = 0;
        while (true)
        {
          var chunk = curLine.Substring(ix, 3);
          if (chunk == "   ")
            break;

          var rv = chunk.HexTextToByte();
          var errmsg = rv.Item2;
          if (errmsg != null)
            break;
          ba.Append(rv.Item1);

          ix += 3;
        }
      }

      return ba.ToByteArray();
    }

    /// <summary>
    /// return true if the list contains no items that contain actual text.
    /// </summary>
    /// <param name="List"></param>
    /// <returns></returns>
    public static bool IsNullOrEmpty(this IEnumerable<string> List)
    {
      bool isEmpty = true;
      if ( List != null)
      {
        foreach( var item in List)
        {
          if ( item.IsNullOrEmpty( ) == false )
          {
            isEmpty = false;
            break;
          }
        }
      }
      return isEmpty;
    }

    /// <summary>
    /// return all the items of the input IEnumerable of strings which contain non 
    /// blank text as an IEnumerable of strings.
    /// </summary>
    /// <param name="List"></param>
    /// <returns></returns>
    public static IEnumerable<string> RemoveEmptyOrBlankItems( this IEnumerable<string> List)
    {
      if (List == null)
        yield break;
      else
      {
        foreach( var item in List )
        {
          if (item.Length == 0)
            continue;
          else if (item.TrimWhitespace().Length == 0)
            continue;
          else
            yield return item;
        }
      }
    }

    /// <summary>
    /// return the list of lines as a concatenated string containing each line 
    /// with a newline at the end of each line.
    /// </summary>
    /// <param name="list"></param>
    /// <returns></returns>
    public static string ToAllText(this IEnumerable<string> list)
    {
      var sb = new StringBuilder();
      foreach (var textLine in list)
      {
        sb.Append(textLine + Environment.NewLine);
      }

      return sb.ToString();
    }

    public static IEnumerable<string> ToIEnumerableString(
      this XElement Element, XName ItemName)
    {
      if (Element == null)
        yield break;
      else
      {
        foreach (var xi in Element.Elements(ItemName))
        {
          yield return xi.Value;
        }
      }
    }

    /// <summary>
    /// copy the report to a temporary file. Then use Notepad to open the file.
    /// </summary>
    /// <param name="List"></param>
    public static void ToNotepad(this IEnumerable<string> List)
    {
      var tempPath = global::System.IO.Path.GetTempFileName();
      global::System.IO.File.WriteAllLines(tempPath, List.ToArray());
      var renamePath = System.IO.PathExt.SetExtension(tempPath, "txt");
      global::System.IO.File.Move(tempPath, renamePath);
      string exePath =
        Environment.ExpandEnvironmentVariables(@"%windir%\system32\notepad.exe");
      Process.Start(exePath, renamePath );
    }

    public static ObservableCollection<string> ToObservableCollectionString(
      this IEnumerable<string> List)
    {
      var ocs = new ObservableCollection<string>();
      if (List != null)
      {
        foreach (var item in List)
        {
          ocs.Add(item);
        }
      }
      return ocs;
    }
    public static List<T> sToList<T>(this IEnumerable<T> List) where T : class
    {
      var oc = new List<T>();
      if (List != null)
      {
        foreach (var item in List)
        {
          oc.Add(item);
        }
      }
      return oc;
    }

    public static ObservableCollection<T> ToObservableCollection<T>(this IEnumerable<T> List) where T : class
    {
      var oc = new ObservableCollection<T>();
      if ( List != null)
      {
        foreach( var item in List)
        {
          oc.Add(item);
        }
      }
      return oc;
    }

    /// <summary>
    /// transform a sequence of strings into an XElement.
    /// </summary>
    /// <param name="List"></param>
    /// <param name="Name"></param>
    /// <param name="NbrToTake"></param>
    /// <returns></returns>
    public static XElement ToXElement(
      this IEnumerable<string> List, XName Name, int? NbrToTake = null)
    {
      XElement xe = null;

      if (List == null)
        xe = new XElement(Name);

      else if (NbrToTake == null)
      {
        xe = new XElement(Name,
             new XElement("Items",
               from c in List
               select new XElement("Item", c))
               );
      }

      else
      {
        xe = new XElement(Name,
             new XElement("Items",
               from c in List.Take(NbrToTake.Value)
               select new XElement("Item", c))
               );
      }

      return xe;
    }

    public static XElement ToXElementNew(
      this IEnumerable<string> List, XName Name, int? NbrToTake = null)
    {
      XElement xe = null;

      if (List == null)
        xe = new XElement(Name);

      else if (NbrToTake == null)
      {
        xe = new XElement(Name,
               from c in List
               select new XElement("Item", c)
               );
      }

      else
      {
        xe = new XElement(Name,
               from c in List.Take(NbrToTake.Value)
               select new XElement("Item", c)
               );
      }

      return xe;
    }

    /// <summary>
    /// IEnumerable of strings to XElement. The XElement consists of outer ListName 
    /// element and inner itemName elements for each string item.
    /// </summary>
    /// <param name="List"></param>
    /// <param name="NodeName"></param>
    /// <param name="ItemName"></param>
    /// <returns></returns>
    public static XElement ToXElement(
      this IEnumerable<string> List, XName ListName, XName ItemName)
    {
      XElement xe = null;

      if (List == null)
        xe = new XElement(ListName);

      else
      {
        xe = new XElement(ListName,
               from c in List
               select new XElement(ItemName, c)) ;
      }

      return xe;
    }

  }
}
