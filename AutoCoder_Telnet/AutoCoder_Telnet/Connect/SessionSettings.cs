using AutoCoder.Telnet.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoCoder.Telnet.Connect
{
  public class SessionSettings
  {
    /// <summary>
    /// telnet options that the server WILL do
    /// </summary>
    public Dictionary<TelnetSubject, CommandCode> WillOptionDict
    {
      get;
      set;
    }

    public SessionSettings( )
    {
      this.WillOptionDict = new Dictionary<TelnetSubject, CommandCode>();
    }

  }
}
