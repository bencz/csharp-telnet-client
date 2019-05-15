using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoCoder.Telnet.Exceptions
{
  public class ConnectException : Exception
  {
    public ConnectException(string InMessage)
      : base(InMessage)
    {
    }
  }
}
