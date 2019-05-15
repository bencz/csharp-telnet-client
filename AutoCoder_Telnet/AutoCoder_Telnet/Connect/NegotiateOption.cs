using AutoCoder.Telnet.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoCoder.Telnet.Connect
{
  /// <summary>
  /// telnet negotiation option and the response to the option.
  /// Used to store if code should reply WILL or WONT to telnet options such as 
  /// NEW_ENVIRON and TERMINAL_TYPE
  /// </summary>
  public class NegotiateOption
  {
    /// <summary>
    /// Option: NEW_ENVIRON, TERMINAL_TYPE, ...
    /// </summary>
    public TelnetSubject Option
    { get; set; }

    /// <summary>
    /// Response: WILL WONT
    /// </summary>
    public CommandCode Response
    { get; set; }

    public NegotiateOption( TelnetSubject Option, CommandCode Response )
    {
      this.Option = Option;
      this.Response = Response;
    }
  }
}
