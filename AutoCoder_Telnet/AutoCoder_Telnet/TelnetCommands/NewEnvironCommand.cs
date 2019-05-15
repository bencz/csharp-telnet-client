using AutoCoder.Ext.System;
using AutoCoder.Systm;
using AutoCoder.Telnet.Common;
using AutoCoder.Telnet.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoCoder.Telnet.TelnetCommands
{
  public class NewEnvironCommand : TelnetCommand
  {
    /// <summary>
    /// sub option byte code. Server sends SEND to tell client what NEW_ENVIRON info
    /// to send. Client sends IS to tell server that what follows are the values of
    /// VAR and USERVAR that were requested.
    /// </summary>
    public TelnetOptionParm? SubOption
    { get; set; }

    public List<OptionVariable> OptionList
    { get; set; }

    /// <summary>
    /// stmt end IAC SE is found.
    /// </summary>
    private bool EndFound
    { get; set; }

    public NewEnvironCommand(
      CommandCode CmdCode, TelnetOptionParm SubOption)
      : base(CmdCode, TelnetSubject.NewEnviron)
    {
      this.SubOption = SubOption;
      this.OptionList = new List<OptionVariable>();
    }

    public NewEnvironCommand(InputByteArray InputArray, CommandCode CmdCode )
      : base(InputArray, CmdCode, TelnetSubject.NEW_ENVIRON )
    {
      this.SubOption = null;
      this.OptionList = new List<OptionVariable>();
      this.EndFound = false;

      // statement contains additional parameters.
      if (this.CmdCode == CommandCode.SB)
      {
        var b1 = InputArray.GetNextByte();
        this.RawBytes.Append(b1);
        this.SubOption = b1.ToTelnetOptionParm();  // IS, SEND, INFO

        // list of VARs and USERVARS follow until IAC SE.
        if ((this.SubOption.Value == TelnetOptionParm.SEND) ||
          (this.SubOption.Value == TelnetOptionParm.IS ))
        {
          while (true)
          {
            var ov = OptionVariable.Construct(InputArray);
            if (ov == null)
              break;
            this.OptionList.Add(ov);
            this.RawBytes.Append(ov.ToBytes());
          }

          if (InputArray.PeekIacSe())
          {
            this.EndFound = true;
            this.RawBytes.Append(InputArray.GetBytes(2));
          }
        }

        // parse the closing IAC SE
        ParseClosingSE(InputArray);
      }
    }

    public void AddOptionVar(EnvironVarCode VarCode, string VarName, string VarValue)
    {
      var ov = new OptionVariable(VarCode, VarName, VarValue);
      this.OptionList.Add(ov);
    }

    public void AddOptionVar( EnvironVarCode VarCode, string VarName, byte[] VarValue )
    {
      var ov = new OptionVariable(VarCode, VarName, VarValue);
      this.OptionList.Add(ov);
    }

    public void AddOptionVar(OptionVariable Var)
    {
      this.OptionList.Add(Var);
    }

    private List<byte[]> AccumVarName()
    {
      return new List<byte[]>();
    }

    /// <summary>
    /// one of the OptionVariables is an IBMRSEED variable.
    /// </summary>
    /// <returns></returns>
    public bool ContainsUserVar_IBMRSEED( )
    {
      bool doesContain = false;

      foreach( var optn in this.OptionList )
      {
        if ( optn.IsSeedVar == true )
        {
          doesContain = true;
          break;
        }
      }
      return doesContain;
    }

    public byte[] IBMRSEED_SeedValue( )
    {
      byte[] seedValue = null;
      foreach (var optn in this.OptionList)
      {
        if (optn.IsSeedVar == true)
        {
          seedValue = optn.SeedValue;
          break;
        }
      }
      return seedValue;
    }

    public byte[] ToxBytes()
    {
      ByteArrayBuilder ab = new ByteArrayBuilder();
      var buf = base.ToBytes();
      ab.Append(buf);
      if (this.SubOption != null)
        ab.Append(this.SubOption.Value.ToByte());

      foreach (var optn in this.OptionList)
      {
        ab.Append(optn.ToBytes());
      }

      return ab.ToByteArray();
    }
    protected override byte[] BodyToBytes()
    {
      ByteArrayBuilder ab = new ByteArrayBuilder();
      if (this.SubOption != null)
        ab.Append(this.SubOption.Value.ToByte());

      foreach (var optn in this.OptionList)
      {
        ab.Append(optn.ToBytes());
      }

      return ab.ToByteArray();
    }
    protected override string BodyToString()
    {
      var sb = new StringBuilder();

      if (this.SubOption != null)
      {
        sb.Append(this.SubOption.Value.ToString());
      }

      foreach (var optn in this.OptionList)
      {
        if (sb.Length > 0)
          sb.Append(" ");
        sb.Append(optn.ToString());
      }
      return sb.ToString();
    }

    public string xxToString()
    {
      var sb = new StringBuilder();
      sb.Append(base.ToString());
      if (this.SubOption != null)
      {
        sb.Append(" ");
        sb.Append(this.SubOption.Value.ToString());
      }

      foreach (var optn in this.OptionList)
      {
        sb.Append(" ");
        sb.Append(optn.ToString());
      }

      // stmt ended with IAC SE
      if (this.EndFound == true)
        sb.Append(" IAC SE");

      return sb.ToString();
    }
  }
}
