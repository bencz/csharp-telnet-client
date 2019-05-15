using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ehllapi.Messages
{
  /// <summary>
  /// a text message that relates to a SessId.
  /// </summary>
  public class SessIdMessage
  {
    public string SessId
    { get; set; }

    public string Message
    { get; set; }

    public SessIdMessage()
    {
    }

    public SessIdMessage(string SessId, string Message)
    {
      this.SessId = SessId;
      this.Message = Message;
    }

    public override string ToString()
    {
      return "SessId:" + this.SessId + " " + this.Message;
    }
  }
}
