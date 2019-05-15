using AutoCoder.Ext.System.Collections.Generic;
using AutoCoder.Ext.System;
using AutoCoder.Telnet.Common;
using AutoCoder.Telnet.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoCoder.Telnet.Connect
{
  /// <summary>
  /// settings used to control the telnet negotiation process
  /// </summary>
  public class NegotiateSettings
  {

    public List<NegotiateOption> OptionList
    {
      get;
      private set;
    }

    // DoReplyDict - replies to server DO commands - telling client what the server does.
    // WillReplyDict - replies to server WILL command - asking what the client will DO

    public Dictionary<TelnetSubject,CommandCode> OptionDict
    {
      get;
      set;
    }
    public Dictionary<string, string> SbsValuesDict
    { get; set; }

    private string _UserName;
    public string UserName
    {
      get { return _UserName; }
      set
      {
        if ( value.IsNullOrEmpty( ))
        {
          SbsValuesDict.EnsureRemoved("*USERNAME");
        }
        else
        {
          SbsValuesDict.EnsureAdded("*USERNAME", value);
        }
        _UserName = value;
      }
    }
    private string _DevName;
    public string DevName
    {
      get { return _DevName; }
      set
      {
        _DevName = value;
        SbsValuesDict.EnsureAdded("*DEVNAME", value);
      }
    }

    private string _TerminalType;

    public string TerminalType
    {
      get
      {
        return _TerminalType;
      }
      set { _TerminalType = value; }
    }


    public List<BuildOptionVariable> SendNewEnvironList
    { get; set; }

    public NegotiateSettings( )
    {
      this.OptionList = new List<NegotiateOption>();
      this.OptionDict = new Dictionary<TelnetSubject, CommandCode>();
      this.SendNewEnvironList = new List<BuildOptionVariable>();
      this.SbsValuesDict = new Dictionary<string, string>();
      this.TerminalType = "IBM-3179-2";
      this.TerminalType = "IBM-3477-FC";
    }

    public void AddWillOption(TelnetSubject Option)
    {
      var negOptn = new NegotiateOption(Option, CommandCode.WILL);
      this.OptionList.Add(negOptn);

      this.OptionDict.Add(Option, CommandCode.WILL);
    }

    public void AddWontOption( TelnetSubject Option )
    {
      var negOptn = new NegotiateOption(Option, CommandCode.WONT);
      this.OptionList.Add(negOptn);

      this.OptionDict.Add(Option, CommandCode.WONT);
    }

   public CommandCode GetOptionResponse(TelnetSubject Option )
    {
      CommandCode resp = CommandCode.WILL;
      if (this.OptionDict.ContainsKey(Option) == false)
        resp = CommandCode.WONT;
      else
      {
        resp = this.OptionDict[Option];
      }
      return resp;
    }

    public static NegotiateSettings Build5250Settings(string UserName, string DevName,
      string TerminalType = "IBM-3179-2")
    {
      var settings = new NegotiateSettings();
      settings.AddWillOption(TelnetSubject.NEW_ENVIRON);
      settings.AddWillOption(TelnetSubject.TERMINAL_TYPE);
      settings.AddWillOption(TelnetSubject.TRANSMIT_BINARY);
      settings.AddWillOption(TelnetSubject.END_OF_RECORD);

      settings.UserName = UserName;
      settings.DevName = DevName;
      settings.TerminalType = TerminalType;

      {
        var item = new BuildOptionVariable(EnvironVarCode.USERVAR, "DEVNAME");
        item.ValueSubstitute = "*DEVNAME";
        settings.SendNewEnvironList.Add(item);
      }
      {
        var item = new BuildOptionVariable(EnvironVarCode.VAR, "USER");
        item.ValueEmpty = true;
        settings.SendNewEnvironList.Add(item);
      }
      {
        var item = new BuildOptionVariable(EnvironVarCode.USERVAR, "IBMRSEED");
        item.ValueEmpty = true;
        settings.SendNewEnvironList.Add(item);
      }

      return settings;
    }
  }
}
