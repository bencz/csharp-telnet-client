using AutoCoder.Telnet.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AutoCoder.Telnet.ThreadMessages
{
  /// <summary>
  /// message sent to the input queue of a thread. After the message is sent to
  /// the input queue the sending thread waits for the response event to be 
  /// signaled. That event is signaled when the receiving thread assigns to the
  /// replyMessage object and signals the response event.
  /// </summary>
  public class ExchangeMessage : ThreadMessageBase
  {
    public object SendMessage
    { get; set; }
    public object ReplyMessage
    { get; set; }
    public ThreadMessageCode? MessageCode
    { get; set; }

    public ManualResetEvent ReplyEvent
    { get; set; }

    public ExchangeMessage(object SendMessage)
    {
      this.SendMessage = SendMessage;
      this.ReplyEvent = new ManualResetEvent(false);
    }
    public ExchangeMessage(ThreadMessageCode MessageCode )
    {
      this.MessageCode = MessageCode;
      this.ReplyEvent = new ManualResetEvent(false);
    }

    public void WaitReplyEvent( )
    {
      this.ReplyEvent.WaitOne();
    }

    public void PostReplyMessage( object ReplyMessage)
    {
      this.ReplyMessage = ReplyMessage;
      this.ReplyEvent.Set();
    }
  }
}
