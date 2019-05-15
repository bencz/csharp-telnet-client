using System;
using System.Collections.Generic;
using System.IO ;
using System.Text;
using AutoCoder.Text;
using AutoCoder.Xml;
using System.Collections;

namespace AutoCoder.Ftp
{
  // how does the FTP command use the data channel.
  //   None=does not use it  example: HELP 
  //   Text= returns text lines on the data channel.  example: LIST
  //   Bytes=returns byte, asis data on the data channel. example: GET 
  public enum DataChannel { None, Text, Bytes } ;
  public enum ReplyCodeAction { None, OK } ;

  /// <summary>
  /// Collection of FTP commands 
  /// </summary>
  public class FtpCommandTable :
    Dictionary<string, FtpCommandItem>, IEnumerable<FtpCommandItem>
  {
    public FtpCommandTable()
    {
    }

    public FtpCommandItem AddCommand(string InName, string InText)
    {
      FtpCommandItem item = new FtpCommandItem();
      item.Name = InName;
      item.Text = InText;
      base.Add(item.Key, item);
      return item;
    }

    public static FtpCommandTable LoadFromXml( Stream InXmlStream, Encoding InEncoding )
    {
      byte[] xmlBytes = null;
      int Ox = 0;
      int Lx = 0;
      string xmlString = null;

      // get all the bytes of the stream.
      Lx = (int)InXmlStream.Length;
      xmlBytes = new byte[Lx];
      InXmlStream.Read(xmlBytes, 0, xmlBytes.Length);

      // advance past the UTF8 BOM.
      if (Lx >= 3)
      {
        if ((xmlBytes[0] == 0xef) &&
          (xmlBytes[1] == 0xbb) &&
          (xmlBytes[2] == 0xbf))
        {
          Lx -= 3;
          Ox += 3;
        }
      }

      // encoded bytes to string.
      xmlString = InEncoding.GetString(xmlBytes, Ox, Lx);

      return LoadFromXml(xmlString);
    }

    public static FtpCommandTable LoadFromXml(string InXmlString )
    {
      FtpCommandTable cmdTable = new FtpCommandTable();

      XmlStream xmlStream = new XmlStream(InXmlString);

      XmlElem cmdsElem = xmlStream.GetDocumentElem( "FtpCommands" );

      // for each FtpCommand of the FtpCommands elem.
      foreach (XmlElem cmdElem in cmdsElem)
      {
        if (cmdElem.ElemName != "FtpCommand")
          continue;

        // new up the item to add to the table.
        FtpCommandItem cmdItem = new FtpCommandItem();

        // command name.
        XmlElem nameElem = cmdElem.FindChildElem("Name");
        if (nameElem != null)
          cmdItem.Name = nameElem.ElemValue;

        // text description
        XmlElem textElem = cmdElem.FindChildElem("Text");
        if (textElem != null)
          cmdItem.Text = textElem.ElemValue;

        // how does the ftp command use the data channel.
        cmdItem.SetDataChannel( cmdElem["DataChannel"]);

        // load the ReplyCodes element. ( the replies expected from the server to
        // this Ftp command )
        LoadFromXml_ReplyCodes(cmdItem, cmdElem); 

        // add to the FtpCommandTable. 
        cmdTable.Add(cmdItem.Key, cmdItem);
      }

      return cmdTable;
    }

    /// <summary>
    /// Load the contents of the ReplyCodes element of the FtpCommand element.
    /// </summary>
    /// <param name="InCmdElem"></param>
    /// <param name="InCmdItem"></param>
    private static void LoadFromXml_ReplyCodes(
      FtpCommandItem InCmdItem, XmlElem InCmdElem)
    {
      XmlElem codesItem = InCmdElem["ReplyCodes"];
      if (codesItem != null)
      {
        foreach (XmlElem codeElem in codesItem)
        {
          if (codeElem.ElemName != "ReplyCode")
            continue;

          ReplyCodeItem codeItem = new ReplyCodeItem();
          codeItem.SetReplyCode( codeElem["Value"]) ;
          codeItem.SetAction( codeElem["Action"]) ;

          // add the ReplyCodeItem to the collection of expected server 
          // replies to this FTP command.
          InCmdItem.ReplyCodes.Add(codeItem.Key, codeItem);
        }
      }
    }

    IEnumerator<FtpCommandItem> IEnumerable<FtpCommandItem>.GetEnumerator()
    {
      foreach ( KeyValuePair<string,FtpCommandItem> pair in this )
      {
        yield return pair.Value;
      }
    }

    #region IEnumerable Members
    IEnumerator IEnumerable.GetEnumerator()
    {
      throw new Exception("The method or operation is not implemented.");
    }
    #endregion

  }
}
