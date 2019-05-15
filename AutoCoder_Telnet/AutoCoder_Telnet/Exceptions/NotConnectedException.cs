using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoCoder.Telnet.Exceptions
{
  public class NotConnectedException : Exception
  {
    public NotConnectedException()
      : base("no longer connected to telnet server.")
    {
    }
  }
}
