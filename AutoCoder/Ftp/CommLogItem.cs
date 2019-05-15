using System;
using System.Collections.Generic;
using System.Text;
using AutoCoder.Core;
using AutoCoder.Core.Enums;

namespace AutoCoder.Ftp
{
  public class CommLogItem
  {
    DateTime mTimestamp = DateTime.Now ;
    CommDirection mDirection = CommDirection.None ;
    string mMessageText = null ;
    MessageType mMessageType = MessageType.Data;

    public CommLogItem()
    {
    }

    public DateTime Timestamp
    {
      get { return mTimestamp; }
      set { mTimestamp = value; }
    }

    public CommDirection Direction
    {
      get { return mDirection; }
      set { mDirection = value; }
    }

    public string MessageText
    {
      get { return mMessageText; }
      set { mMessageText = value; }
    }

    public MessageType MessageType
    {
      get { return mMessageType; }
      set { mMessageType = value; }
    }
  }
}
