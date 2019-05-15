using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace AutoCoder.Core.Enums
{
  public enum MessageType
  {
    Data, Info, Exception, Diagnostic, Completion, Progress, Trace, None
  } ;


  public static class MessageTypeExt
  {
    public static MessageType MessageTypeOrDefault(
      this XElement Element, MessageType Default = MessageType.Info)
    {
      var s1 = Element.Value;
      if (s1 == null)
        return Default;
      else
      {
        MessageType mt;
        var rc = Enum.TryParse<MessageType>(s1, out mt ) ;
        if (rc == true)
          return mt;
        else
          return Default;
      }
    }
  }

}
