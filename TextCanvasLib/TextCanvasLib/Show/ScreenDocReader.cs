using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using AutoCoder.Ext;
using AutoCoder.Ext.System;
using AutoCoder.Telnet.Common;
using AutoCoder.Telnet.Common.RowCol;

namespace TextCanvasLib.xml
{
  public class ScreenDocReader
  {

    public static List<ShowItemBase> ReadDoc(string FilePath)
    {
      XDocument doc = null;

      var xmlText = System.IO.File.ReadAllText(FilePath);

      // parse the xml text.
      doc = XDocument.Parse(xmlText);
      
      var showItems = ReadDoc(doc) ;
      
      return showItems ;
    }

    public static List<ShowItemBase> ReadDoc(XDocument Doc)
    {
      List<ShowItemBase> showItems = new List<ShowItemBase>();

      // isolate the namespace. Need to concat namespace to element names when accessing
      // an element of the document.
      var ns = Doc.Root.Name.Namespace;

      // drill down to the <ScreenCapture><PresentationSpace><Fields> element.
      var contentElem = Doc.Element(ns + "ScreenDoc").Element(ns + "Content");
      if (contentElem != null)
      {
        var litItems =
        from sam in Doc.Element(ns + "ScreenDoc")
          .Element(ns + "Content").Elements("Literal")
        select new ShowLiteralItem
        {
          ItemRowCol = new ZeroRowCol(
           sam.Attribute("Row").StringOrDefault().TryParseInt32(0).Value,
           sam.Attribute("Col").StringOrDefault().TryParseInt32(0).Value),
          Value = sam.Attribute("Value").StringOrDefault()
        };

        foreach (var item in litItems)
        {
          showItems.Add(item);
        }

        // input field items in the xml data stream.
        var fldItems =
        from sam in Doc.Element(ns + "ScreenDoc")
          .Element(ns + "Content").Elements("Field")
        select new ShowFieldItem
        {
          ItemRowCol = new ZeroRowCol(
           sam.Attribute("Row").StringOrDefault().TryParseInt32(0).Value,
           sam.Attribute("Col").StringOrDefault().TryParseInt32(0).Value),
          Value = sam.Attribute("Value").StringOrDefault(),
          Name = sam.Attribute("Name").StringOrDefault(),
          UsageText = sam.Attribute("Usage").StringOrDefault( ),
          DataDefn = sam.Attribute("DataDefn").StringOrDefault( )
        };

        foreach (var item in fldItems)
        {
          showItems.Add(item);
        }
      }

      return showItems;
    }
  }
}
