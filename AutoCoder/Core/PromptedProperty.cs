using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;
using System.Xml;
using AutoCoder.Xml;
using System.Xml.Schema;

namespace AutoCoder.Core
{
  public class PromptedProperty : IXmlSerializable 
  {
    string mPropertyName;
    string mPromptText;
    object mValue;
    bool mAllowChange;
    List<string> mAllowedValues;
    string mDefaultValue;
    public const string ClassName = "PromptedProperty";

    public PromptedProperty()
    {
      mAllowChange = true;
      mValue = null;
      mAllowedValues = null;
      mDefaultValue = null;
    }

    public bool AllowChange
    {
      get { return mAllowChange; }
      set { mAllowChange = value; }
    }

    public List<string> AllowedValues
    {
      get { return mAllowedValues; }
      set { mAllowedValues = value; }
    }

    public string DefaultValue
    {
      get { return mDefaultValue; }
      set { mDefaultValue = value; }
    }

    public bool HasAllowedValues
    {
      get
      {
        if (mAllowedValues == null)
          return false;
        else if (mAllowedValues.Count == 1)
          return false;
        else
          return true;
      }
    }

    public string PropertyName
    {
      get { return mPropertyName; }
      set { mPropertyName = value; }
    }

    public string PromptText
    {
      get
      {
        if (mPromptText == null)
          return mPropertyName;
        else
          return mPropertyName;
      }
      set { mPromptText = value; }
    }

    public object Value
    {
      get { return mValue; }
      set { mValue = value; }
    }

    /// <summary>
    /// Build a PromptedProperty object from the 'PromptedProperty' element in the XmlReader.
    /// </summary>
    /// <param name="InReader"></param>
    /// <returns></returns>
    public static PromptedProperty Parse(XmlReader InReader)
    {
      PromptedProperty fld = new PromptedProperty();
      fld.ReadXml(InReader);
      return fld;
    }


    #region IXmlSerializable Members

    public XmlSchema GetSchema()
    {
      return null;
    }

    public void ReadXml(XmlReader reader)
    {
      int ix;
      string[] nameStack = new string[99];

      // the current node must be the PromptedProperty.ClassName node.
      if ((reader.NodeType == XmlNodeType.Element) && (reader.Name == PromptedProperty.ClassName))
      { }
      else
        throw new ApplicationException(
          "ReadXml method of PromptedProperty class expects current node to be " +
          "named 'Field'");

      bool fExit = false;
      while (true)
      {
        switch (reader.NodeType)
        {
          case XmlNodeType.Element:

            if (reader.Name == "AllowedValues")
            {
              List<string> vlus = new List<string>();
              Xmler.ReadXml(out vlus, reader, "AllowedValues", "AllowedValue");
            }

            else
            {

              // save the element name in stack of element names.
              ix = reader.Depth;
              nameStack[ix] = reader.Name;
            }
            break;

          case XmlNodeType.Text:

            ix = reader.Depth;
            if ((ix > 1) && (nameStack[ix - 2] == PromptedProperty.ClassName))
            {
              string elemName = nameStack[ix - 1];
              if (elemName == "PropertyName")
                this.PropertyName = reader.Value;
              else if (elemName == "PromptText")
                this.PromptText = this.PromptText;
              else if (elemName == "DefaultValue")
                this.PromptText = this.DefaultValue;
              else if (elemName == "AllowChange")
                this.AllowChange = Boolean.Parse(reader.Value);
            }
            break;

          case XmlNodeType.EndElement:
            if (reader.Name == PromptedProperty.ClassName)
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
            "ReadXml method of PromptedProperty class did not find 'Field' end element");
        }
      }
    }

    public void WriteXml(XmlWriter writer)
    {
      writer.WriteStartElement(PromptedProperty.ClassName);
      writer.WriteElementString("PropertyName", this.PropertyName);
      writer.WriteElementString("PromptText", this.PromptText);
      writer.WriteElementString("DefaultValue", this.DefaultValue);
      writer.WriteElementString("AllowChange", this.AllowChange.ToString());
      if (HasAllowedValues == true)
      {
        Xmler.WriteXml(writer, this.AllowedValues, "AllowedValues", "AllowedValue");
      }
      writer.WriteEndElement();
    }

    #endregion



  }
}
