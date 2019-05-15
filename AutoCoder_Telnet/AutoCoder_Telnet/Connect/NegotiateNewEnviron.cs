using AutoCoder.Telnet.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoCoder.Telnet.Connect
{
  /// <summary>
  /// rules for building a NEW_ENVIRON command to send to the server.
  /// </summary>
  public class NegotiateNewEnviron
  {
    public EnvironVarCode NameCode
    { get; set; }

    public string NameText
    { get; set; }

    public bool DoSendValue
    { get; set; }

    public string ValueText
    { get; set; }

    public NegotiateNewEnviron( EnvironVarCode NameCode, string NameText )
    {
      this.NameCode = NameCode;
      this.NameText = NameText;
      this.DoSendValue = false;
      this.ValueText = null;
    }
  }
}
