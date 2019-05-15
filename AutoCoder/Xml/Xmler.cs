using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Drawing;
using AutoCoder.Text;
using AutoCoder.File;
using System.Xml.Linq;
using System.IO;

namespace AutoCoder.Xml
{
  public static class Xmler
  {

    public static void CopyXmlDocumentNodeByNode(
      FullPath DocPath, FullPath ToPath,
      Dictionary<string, string> ElementReplaceValues)
    {
      string insertAfterElementName = null;
      XElement insertElement = null;
      string skipElementName = null;

      CopyXmlDocumentNodeByNode(
        DocPath, ToPath, ElementReplaceValues,
        insertAfterElementName,
        insertElement,
        skipElementName );
    }

    /// <summary>
    /// Copy an xml document by copying the nodes returned by XmlReader.
    /// Use the replacement element dictionary to change the values of 
    /// elements contained in the dictionary.
    /// </summary>
    /// <param name="InDocPath"></param>
    /// <param name="InToPath"></param>
    /// <param name="InElementReplaceValues"></param>
    public static void CopyXmlDocumentNodeByNode(
      FullPath InDocPath, FullPath InToPath,
      Dictionary<string, string> ElementReplaceValues,
      string InsertAfterElementName,
      XElement InsertElement,
      string SkipElementName = null )
    {
      XmlWriterSettings settings = new XmlWriterSettings();
      settings.Indent = true;
      string[] nameStack = new string[99];
      int ix;

      using (XmlReader reader = XmlReader.Create(InDocPath.ToString()))
      {
        settings.Encoding = Encoding.Unicode;
        using (XmlWriter writer = XmlWriter.Create(InToPath.ToString(), settings))
        {
          bool skipFlag = false;
          while (reader.Read())
          {
            string rplValue = null;
            if (reader.NodeType == XmlNodeType.Element)
            {
              ix = reader.Depth;
              nameStack[ix] = reader.Name;

              // set the skip flag.
              if ((SkipElementName != null) && (SkipElementName == reader.Name))
                skipFlag = true;
            }

            if (reader.NodeType == XmlNodeType.Text)
            {
              string nodeName = nameStack[reader.Depth - 1];
              if (ElementReplaceValues.ContainsKey(nodeName) == true)
                rplValue = ElementReplaceValues[nodeName];
            }

            if (skipFlag == false)
            {
              CopyXmlDocumentNode(reader, writer, rplValue);
            }

            // the end of the skip element.  Set off the skip flag.
            if ((skipFlag == true) 
              && (reader.NodeType == XmlNodeType.EndElement)
              && (reader.Name == SkipElementName))
            {
              skipFlag = false;
            }

            // ----------- Insert the InsertElement after the specified element. --------
            if ((InsertElement != null) && (reader.NodeType == XmlNodeType.EndElement))
            {
              string s1 = nameStack[0];
              string s2 = reader.Name;
              if (InsertAfterElementName == s2)
              {
                InsertElement.WriteTo(writer);

#if skip
                string s4 = InsertElement.ToString();
                StringReader r2 = new StringReader(s4);
                using (XmlReader r1 = XmlReader.Create(r2, reader.Settings))
                {
                  settings.Encoding = Encoding.Unicode;
                  while (r1.Read())
                  {
                    CopyXmlDocumentNode(r1, writer, null);
                  }
                }
#endif

              }
            }
  
          }
        }
      }
    }

    private static void CopyXmlDocumentNode(
      XmlReader reader, XmlWriter writer, string InReplacementValue)
    {
      if (reader == null)
      {
        throw new ArgumentNullException("reader");
      }

      if (writer == null)
      {
        throw new ArgumentNullException("writer");
      }

      switch (reader.NodeType)
      {
        case XmlNodeType.Element:
          writer.WriteStartElement(reader.Prefix, reader.LocalName, reader.NamespaceURI);
          writer.WriteAttributes(reader, true);
//          writer.WriteStartElement(reader.LocalName);
          if (reader.IsEmptyElement)
          {
            writer.WriteEndElement();
          }
          break;

        case XmlNodeType.Text:
          if (InReplacementValue != null)
            writer.WriteString(InReplacementValue);
          else
            writer.WriteString(reader.Value);
          break;

        case XmlNodeType.Whitespace:

        case XmlNodeType.SignificantWhitespace:
          writer.WriteWhitespace(reader.Value);
          break;

        case XmlNodeType.CDATA:
          writer.WriteCData(reader.Value);
          break;

        case XmlNodeType.EntityReference:
          writer.WriteEntityRef(reader.Name);
          break;

        case XmlNodeType.XmlDeclaration:

        case XmlNodeType.ProcessingInstruction:
          writer.WriteProcessingInstruction(reader.Name, reader.Value);
          break;

        case XmlNodeType.DocumentType:
          writer.WriteDocType(
            reader.Name, reader.GetAttribute("PUBLIC"), reader.GetAttribute("SYSTEM"), 
            reader.Value);
          break;

        case XmlNodeType.Comment:
          writer.WriteComment(reader.Value);
          break;

        case XmlNodeType.EndElement:
          writer.WriteFullEndElement();
          break;
      }
    }


    /// <summary>
    /// split the stream of xml text on open and close xml elements
    /// </summary>
    /// <param name="XmlStream"></param>
    /// <returns></returns>
    public static string[] ToLines(string XmlStream)
    {
      var lines = new List<string>();
      int fx = 0;
      int ex = -1;
      int ix = 0;

      while (true)
      {
        // start of the next line.
        int bx = ex + 1;
        if (bx >= XmlStream.Length)
          break;

        // calc pos to start scan for next lines "<" char.
        ix = bx + 1;

        // scan start pos is out of bounds.
        if (ix >= XmlStream.Length)
          ex = bx;

          // scan for the end of the line.
        else
        {
          fx = XmlStream.IndexOf('<', ix);
          if (fx == -1)
            ex = XmlStream.Length - 1;
          else
            ex = fx - 1;
        }

        int lx = ex - bx + 1;
        var line = XmlStream.Substring(bx, lx);
        lines.Add(line.TrimEnd(new char[] { '\n', '\r', '\t' }));
      }

      return lines.ToArray( );
    }

    /// <summary>
    /// write the Size property to the XmlWriter stream.
    /// </summary>
    /// <param name="InWriter"></param>
    /// <param name="InSize"></param>
    /// <param name="InPropertyName"></param>
    public static void WriteXml(XmlWriter InWriter, Size InSize, string InPropertyName)
    {
      InWriter.WriteStartElement(InPropertyName);
      InWriter.WriteElementString("Height", InSize.Height.ToString());
      InWriter.WriteElementString("Width", InSize.Width.ToString());
      InWriter.WriteEndElement();
    }

    /// <summary>
    /// write the Point property to the XmlWriter stream.
    /// </summary>
    /// <param name="InWriter"></param>
    /// <param name="InSize"></param>
    /// <param name="InPropertyName"></param>
    public static void WriteXml(XmlWriter InWriter, Point InPoint, string InPropertyName)
    {
      InWriter.WriteStartElement(InPropertyName);
      InWriter.WriteElementString("X", InPoint.X.ToString());
      InWriter.WriteElementString("Y", InPoint.Y.ToString());
      InWriter.WriteEndElement();
    }

    /// <summary>
    /// write the list of strings to the XmlWriter stream.
    /// </summary>
    /// <param name="InWriter"></param>
    /// <param name="InList"></param>
    /// <param name="InListName"></param>
    /// <param name="InEntryName"></param>
    public static void WriteXml(
      XmlWriter InWriter, List<string> InList, string InListName, string InEntryName)
    {
      InWriter.WriteStartElement(InListName);
      foreach (string s1 in InList)
      {
        InWriter.WriteElementString(InEntryName, s1);
      }
      InWriter.WriteEndElement();
    }

    /// <summary>
    /// Read the Size property element from the XmlReader stream.
    /// </summary>
    /// <param name="OutSize"></param>
    /// <param name="reader"></param>
    /// <param name="InPropertyName"></param>
    public static void ReadXml(out Size OutSize, XmlReader reader, string InPropertyName)
    {
      int ix;
      string[] nameStack = new string[10];
      int sizeWidth = 0;
      int sizeHeight = 0;

      // the current node must be the FormatField.ClassName node.
      if ((reader.NodeType == XmlNodeType.Element) && (reader.Name == InPropertyName))
      { }
      else
        throw new ApplicationException(
          "ReadXml method expects current node to be " +
          "named " + Stringer.Enquote(InPropertyName, '\''));

      int startDepth = reader.Depth;
      bool fExit = false;
      while (true)
      {
        switch (reader.NodeType)
        {
          case XmlNodeType.Element:

            // save the element name in stack of element names.
            ix = reader.Depth - startDepth;
            nameStack[ix] = reader.Name;
            break;

          case XmlNodeType.Text:

            ix = reader.Depth - startDepth;
            if ((ix > 1) && (nameStack[ix - 2] == InPropertyName))
            {
              string elemName = nameStack[ix - 1];
              if (elemName == "Width")
                sizeWidth = Int32.Parse(reader.Value);
              else if (elemName == "Height")
                sizeHeight = Int32.Parse(reader.Value);
            }
            break;

          case XmlNodeType.EndElement:
            if (reader.Name == InPropertyName)
            {
              fExit = true;
            }
            break;
        }
        if (fExit == true)
          break;

        // read the next node in the xml document.
        bool rc = reader.Read();

        // error if eof. should find the Field end element first.
        if (rc == false)
        {
          throw new ApplicationException(
            "ReadXml method did not find " + Stringer.Enquote(InPropertyName, '\'') + " end element");
        }
      }
      OutSize = new Size(sizeWidth, sizeHeight);
    }


    /// <summary>
    /// Read the Point property element from the XmlReader stream.
    /// </summary>
    /// <param name="OutSize"></param>
    /// <param name="reader"></param>
    /// <param name="InPropertyName"></param>
    public static void ReadXml(out Point OutPoint, XmlReader reader, string InPropertyName)
    {
      int ix;
      string[] nameStack = new string[10];
      int pointX = 0;
      int pointY = 0;

      // the current node must be the FormatField.ClassName node.
      if ((reader.NodeType == XmlNodeType.Element) && (reader.Name == InPropertyName))
      { }
      else
        throw new ApplicationException(
          "ReadXml method expects current node to be " +
          "named " + Stringer.Enquote(InPropertyName, '\''));

      int startDepth = reader.Depth;
      bool fExit = false;
      while (true)
      {
        switch (reader.NodeType)
        {
          case XmlNodeType.Element:

            // save the element name in stack of element names.
            ix = reader.Depth - startDepth;
            nameStack[ix] = reader.Name;
            break;

          case XmlNodeType.Text:

            ix = reader.Depth - startDepth;
            if ((ix > 1) && (nameStack[ix - 2] == InPropertyName))
            {
              string elemName = nameStack[ix - 1];
              if (elemName == "X")
                pointX = Int32.Parse(reader.Value);
              else if (elemName == "Y")
                pointY = Int32.Parse(reader.Value);
            }
            break;

          case XmlNodeType.EndElement:
            if (reader.Name == InPropertyName)
            {
              fExit = true;
            }
            break;
        }
        if (fExit == true)
          break;

        // read the next node in the xml document.
        bool rc = reader.Read();

        // error if eof. should find the Field end element first.
        if (rc == false)
        {
          throw new ApplicationException(
            "ReadXml method did not find " + Stringer.Enquote(InPropertyName, '\'') + " end element");
        }
      }
      OutPoint = new Point(pointX, pointY);
    }

    public static void ReadXml(
      out List<string> OutList, XmlReader InReader, string InListName, string InEntryName)
    {
      List<string> l1 = new List<string>();
      int ix;
      string[] nameStack = new string[10];

      // the current node must be the ListName node.
      if ((InReader.NodeType == XmlNodeType.Element) && (InReader.Name == InListName))
      { }
      else
        throw new ApplicationException(
          "ReadXml method expects current node to be " +
          "named " + Stringer.Enquote(InListName, '\''));

      int startDepth = InReader.Depth;
      bool fExit = false;
      while (true)
      {
        switch (InReader.NodeType)
        {
          case XmlNodeType.Element:

            // save the element name in stack of element names.
            ix = InReader.Depth - startDepth;
            nameStack[ix] = InReader.Name;
            break;

          case XmlNodeType.Text:

            ix = InReader.Depth - startDepth;
            if ((ix > 1) && (nameStack[ix - 2] == InListName))
            {
              string elemName = nameStack[ix - 1];
              if (elemName == InEntryName)
              {
                string s1 = InReader.Value;
                l1.Add(s1);
              }
            }
            break;

          case XmlNodeType.EndElement:
            if (InReader.Name == InListName)
            {
              fExit = true;
            }
            break;
        }
        if (fExit == true)
          break;

        // read the next node in the xml document.
        bool rc = InReader.Read();

        // error if eof. should find the Field end element first.
        if (rc == false)
        {
          throw new ApplicationException(
            "ReadXml method did not find " + Stringer.Enquote(InListName, '\'') + " end element");
        }
      }
      OutList = l1;
    }
  }
}
