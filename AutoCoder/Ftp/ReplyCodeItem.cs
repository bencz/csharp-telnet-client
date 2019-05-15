using System;
using System.Collections.Generic;
using System.Text;
using AutoCoder.Xml;

namespace AutoCoder.Ftp
{
  public class ReplyCodeItem
  {
    int mReplyCode = 0;
    ReplyCodeAction mAction = ReplyCodeAction.None;

    public ReplyCodeItem()
    {
    }

    public int ReplyCode
    {
      get { return mReplyCode; }
      set { mReplyCode = value; }
    }

    public ReplyCodeAction Action
    {
      get { return mAction; }
      set { mAction = value; }
    }

    /// <summary>
    /// The key to this item. 
    /// Used when the item is added to a Dictionary type collection. 
    /// </summary>
    public int Key
    {
      get { return ReplyCode; }
    }

    public ReplyCodeItem SetAction(XmlElem InActionElem)
    {
      if (InActionElem != null)
      {
        Action =
          (ReplyCodeAction)Enum.Parse(typeof(ReplyCodeAction), InActionElem.ElemValue);
      }
      else
        Action = ReplyCodeAction.None;
      return this;
    }

    /// <summary>
    /// Set ReplyCode from the Value sub elem of the ReplyCode xml elem. 
    /// </summary>
    /// <param name="InValueElem"></param>
    /// <returns></returns>
    public ReplyCodeItem SetReplyCode(XmlElem InValueElem)
    {
      if (InValueElem == null)
        throw new ApplicationException("ReplyCode/Value element missing from xml stream");
      else if (InValueElem.ElemName != "Value")
        throw new ApplicationException("not the Value element");
      else
      {
        ReplyCode = Int32.Parse(InValueElem.ElemValue);
      }
      return this;
    }

  }
}
