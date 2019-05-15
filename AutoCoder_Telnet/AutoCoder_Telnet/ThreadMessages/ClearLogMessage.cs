using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoCoder.Telnet.ThreadMessages
{
  public class ClearLogMessage : ThreadMessageBase
  {
    public ClearLogMessage( )
    {
    }

    public override string ToString()
    {
      return "ClearLogMessage.";
    }
  }
}
