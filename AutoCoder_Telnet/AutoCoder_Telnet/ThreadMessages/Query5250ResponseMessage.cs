using AutoCoder.Telnet.IBM5250.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoCoder.Telnet.ThreadMessages
{
  public class Query5250ResponseMessage : ThreadMessageBase
  {

    public Query5250ResponseMessage( )
    {
    }

    public override string ToString()
    {
      return "Query5250ResponseMessage.";
    }
  }
}
