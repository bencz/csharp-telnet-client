using System;
using System.Collections.Generic;
using System.Text;
using AutoCoder.Core;
using AutoCoder.Core.Enums;

namespace AutoCoder.Ftp
{
  public class CommLog : List<CommLogItem>
  {

    /// <summary>
    /// copy the items of the CommLog argument to the end of this CommLog. 
    /// </summary>
    /// <param name="InCommLog"></param>
    /// <returns></returns>
    public CommLog Add(CommLog InCommLog)
    {
      foreach (CommLogItem item in InCommLog)
      {
        base.Add(item);
      }
      return this;
    }

    public CommLogItem AddDiagnostic(string InMsgText, Exception InExcp)
    {
      CommLogItem item = new CommLogItem();
      item.MessageType = MessageType.Diagnostic;
      item.Direction = CommDirection.None;
      item.MessageText = 
        InMsgText + Environment.NewLine +
        InExcp.Message;
      Add(item);
      return item;
    }

    public CommLogItem AddException(string InMsgText)
    {
      CommLogItem item = new CommLogItem();
      item.MessageType = MessageType.Exception;
      item.Direction = CommDirection.None;
      item.MessageText = InMsgText;
      Add(item);
      return item;
    }

    public CommLogItem AddException(Exception InExcp)
    {
      CommLogItem item = null;
      if (InExcp.InnerException != null)
        AddDiagnostic(InExcp.InnerException.Message, InExcp.InnerException.InnerException);
      item = AddException(InExcp.Message);
      return item;
    }

    public CommLogItem AddItem(CommDirection InDir, string InMsgText)
    {
      CommLogItem item = new CommLogItem();
      item.Direction = InDir;
      item.MessageText = InMsgText;
      base.Add(item);
      return item;
    }
  }
}
