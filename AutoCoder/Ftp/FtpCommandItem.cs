using System;
using System.Collections.Generic;
using System.Text;
using AutoCoder.Xml;

namespace AutoCoder.Ftp
{
  public class FtpCommandItem
  {
    string mName;
    string mText;
    DataChannel mDataChannel = DataChannel.None;
    Dictionary<int,ReplyCodeItem> mReplyCodes = null;

    public FtpCommandItem()
    {
    }

    public DataChannel DataChannel
    {
      get { return mDataChannel; }
      set { mDataChannel = value; }
    }

    public string Name
    {
      get { return mName; }
      set { mName = value; }
    }

    public Dictionary<int,ReplyCodeItem> ReplyCodes
    {
      get
      {
        if ( mReplyCodes == null )
          mReplyCodes = new Dictionary<int,ReplyCodeItem>( ) ;
        return mReplyCodes ;
      }
    }

    public string Text
    {
      get { return mText; }
      set { mText = value; }
    }

    public string Key
    {
      get { return Name; }
    }

    public FtpCommandItem SetDataChannel(XmlElem InChannelElem)
    {
      if (InChannelElem != null)
      {
        DataChannel =
          (DataChannel)Enum.Parse(typeof(DataChannel), InChannelElem.ElemValue);
      }
      else
        DataChannel = DataChannel.None;   
      return this;
    }

  }
}
