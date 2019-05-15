using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using AutoCoder.Xml;
using System.Xml;
using System.IO;
using System.Xml.Linq;
using AutoCoder.Ext;
using AutoCoder.Ext.System;
using AutoCoder.Core.Enums;
using System.Xml.Schema;
using AutoCoder.Core.Interfaces;

namespace AutoCoder.Core
{
  public class RunLogMessage : IXmlSerializable, ILogItem
  {
    static string ClassName = "RunLogMessage";

    /// <summary>
    /// Edge is used to indicate which edge of the method is being logged.
    /// "Enter" and "Exit" being commonly used values.
    /// </summary>
    public string Edge { get; set; }

    public DateTime LogTime { get; set; }

    public string LogText
    {
      get
      {
        return this.Message;
      }
    }

    /// <summary>
    /// tag that provide identification info about this message.
    /// </summary>
    public object Tag
    { get; set; }
    public string Message { get; set; }
    public MessageType MessageType { get; set; }
    public string MergeName { get; set; }

    public string Method { get; set; }
    public bool NewGroup { get; set; }
    public string ActionMessage { get; set; }
    public string CompletionMessage { get; set; }
    public string ErrorMessage { get; set; }

    /// <summary>
    /// this message is written to the database.
    /// </summary>
    public bool IsWritten { get; set; }

    public RunLogMessage( RunLogMessage from = null)
    {
      this.IsWritten = false;
      if ( from != null )
      {
        this.LogTime = from.LogTime;
        this.Message = from.Message;
        this.MessageType = from.MessageType;
        this.Method = from.Method;
        this.ActionMessage = from.ActionMessage;
        this.IsWritten = from.IsWritten;
        this.Edge = from.Edge;
        this.Tag = from.Tag;
        this.MergeName = from.MergeName;
        this.ErrorMessage = from.ErrorMessage;
        this.CompletionMessage = from.CompletionMessage;
      }
    }

    public RunLogMessage(string Message)
    {
      this.LogTime = DateTime.Now;
      this.Message = Message;
      this.MessageType = Enums.MessageType.Info;

      this.Method = null;
      this.ActionMessage = Message;
      this.IsWritten = false;
    }

    public RunLogMessage(string Method, string ActionMessage)
    {
      this.LogTime = DateTime.Now;
      this.MessageType = Enums.MessageType.Info;

      this.Method = Method;
      this.ActionMessage = ActionMessage;
      this.IsWritten = false;
    }

    public XmlSchema GetSchema()
    {
      throw new NotImplementedException();
    }

    public ILogItem NewCopy()
    {
      var item = new RunLogMessage(this);
      return item;
    }
    public ILogItem NewItem( )
    {
      var item = new RunLogMessage( );
      return item;
    }

    public static RunLogMessage Parse(string InText)
    {
      RunLogMessage rlm = null;

      XmlSerializer xmlser = new XmlSerializer(typeof(RunLogMessage));
      StringReader sr = new StringReader(InText);
      rlm = xmlser.Deserialize(sr) as RunLogMessage;

      return rlm;
    }

    public void ReadXml(XmlReader Reader)
    {
      var doc = XDocument.Load(Reader);
      var ns = doc.Root.Name.Namespace;
      var xe = doc.Element(ns + RunLogMessage.ClassName);
      xe = xe.Element(ns + RunLogMessage.ClassName);

      this.LogTime = 
        xe.Element(ns + "LogDateTime").DateTimeOrDefault(DateTime.Now).Value ;
      this.Message = xe.Element(ns + "Message").StringOrDefault();
      this.MessageType = 
        xe.Element(ns + "MessageType").MessageTypeOrDefault(MessageType.Info);
    }

    public string TimedMessageLine( )
    {
      var timeText = this.LogTime.ToString("HH:mm:ss.ffff");
      return timeText + " " + this.Message;
    }

    public void WriteXml(XmlWriter Writer)
    {
      XElement xml = new XElement(RunLogMessage.ClassName,
        this.LogTime.ToXElement("LogDateTime"),
        new XElement("MessageType", this.MessageType.ToString( )),
        new XElement("Message", this.Message)
          );

      xml.WriteTo(Writer);
    }

    public void ReadXml(XmlReader InReader, string InClassName )
    {
      bool fsPass = true;
      XmlNameStack nameStack = new XmlNameStack();

      while (true)
      {
        // Next in reader. ( When this method is called, the reader may already be
        // positioned at a node.
        if (fsPass == true)
        {
          if (InReader.NodeType == XmlNodeType.None)
            InReader.Read();
        }
        else
        {
          InReader.Read();
        }

        fsPass = false;

        if (InReader.EOF == true)
          break;
        nameStack.UpdateCurrent(InReader);

        switch (InReader.NodeType)
        {

          case (XmlNodeType.Element):
            break;

          case (XmlNodeType.Text):
            if (nameStack[2] == InClassName)
            {
              string elemName = nameStack[1];
              if (elemName == "LogDateTime")
                this.LogTime = DateTime.Parse(InReader.Value);
              else if (elemName == "Method")
                this.Method = InReader.Value;
              else if (elemName == "Edge")
                this.Edge = InReader.Value;
              else if (elemName == "ActionMessage")
                this.ActionMessage = InReader.Value;
              else if (elemName == "CompletionMessage")
                this.CompletionMessage = InReader.Value;
              else if (elemName == "ErrorMessage")
                this.ErrorMessage = InReader.Value;
              else if (elemName == "IsWritten")
                this.IsWritten = Boolean.Parse(InReader.Value);
            }
            break;

          case (XmlNodeType.EndElement):
            break;
        }
      }
    }

    public override string ToString()
    {
      XmlSerializer xmlser = new XmlSerializer(typeof(RunLogMessage));
      StringWriter sw = new StringWriter();
      xmlser.Serialize(sw, this);
      return sw.ToString();
    }

    public void WriteXml(XmlWriter InWriter, string InClassName)
    {
      InWriter.WriteStartElement(InClassName);

      if ( this.LogTime != null )
        InWriter.WriteElementString("LogDateTime", this.LogTime.ToString( ));
      if ( this.Method != null )
        InWriter.WriteElementString("Method", this.Method);
      if (this.Edge != null)
        InWriter.WriteElementString("Edge", this.Edge);
      if (this.ActionMessage != null)
        InWriter.WriteElementString("ActionMessage", this.ActionMessage);
      if (this.CompletionMessage != null)
        InWriter.WriteElementString("CompletionMessage", this.CompletionMessage);
      if (this.ErrorMessage != null)
        InWriter.WriteElementString("ErrorMessage", this.ErrorMessage);
      InWriter.WriteElementString("IsWritten", this.IsWritten.ToString( ));
      InWriter.WriteEndElement();
    }

  }
}
