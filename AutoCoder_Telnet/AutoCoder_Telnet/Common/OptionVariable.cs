using AutoCoder.Ext.System;
using AutoCoder.Systm;
using AutoCoder.Telnet.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoCoder.Telnet.Common
{
  /// <summary>
  /// variable name and value. Where the data stream is in the form USERVAR or VAR
  /// followed by the variable name, then VALUE followed by the value.
  /// </summary>
  public class OptionVariable
  {
    /// <summary>
    /// single byte code that indicates if the name value that follows is VAR or
    /// USERVAR.
    /// </summary>
    public EnvironVarCode VarCode
    { get; set; }

    /// <summary>
    /// the bytes from the data stream that follow the USERVAR or VAR NameCode. Name
    /// bytes run until the VALUE byte code.
    /// </summary>
    public byte[] NameBytes
    { get; set; }

    public string NameAsText
    {
      get
      {
        if (this.NameBytes == null)
          return null;
        else
          return this.NameBytes.ToAscii();
      }
    }

    public byte[] ValueBytes
    { get; set; }

    public string ValueText
    {
      get
      {
        if (this.ValueBytes == null)
          return null;
        else
          return this.ValueBytes.ToAscii();
      }
      set
      {
        this.ValueBytes = Encoding.ASCII.GetBytes(value);
      }
    }

    /// <summary>
    /// when true place VALUE keyword in option variable byte stream without a 
    /// following byte stream value.
    /// </summary>
    public bool ValueEmpty
    {
      get; set;
    }

    public bool IsSeedVar
    { get; set; }

    public byte[] SeedValue
    { get; set; }

    private OptionVariable( )
    {
      this.IsSeedVar = false;
      this.SeedValue = null;
    }
    public OptionVariable(InputByteArray ByteArray)
      :this( )
    {
    }

    public OptionVariable(EnvironVarCode VarCode, byte[] NameBytes, byte[] ValueBytes)
      : this( )
    {
      this.VarCode = VarCode;
      this.NameBytes = NameBytes;
      this.ValueBytes = ValueBytes;

      ParseNameBytes(this.NameBytes);
    }

    public OptionVariable(EnvironVarCode VarCode, string NameText, string ValueText)
      : this( ) 
    {
      this.VarCode = VarCode;
      this.NameBytes = Encoding.ASCII.GetBytes(NameText);
      this.ValueBytes = Encoding.ASCII.GetBytes(ValueText);
    }
    public OptionVariable(EnvironVarCode VarCode, string NameText, byte[] Value)
      : this()
    {
      this.VarCode = VarCode;
      this.NameBytes = Encoding.ASCII.GetBytes(NameText);
      this.ValueBytes = Value;
    }
    public OptionVariable(EnvironVarCode VarCode, string NameText, bool ValueEmpty)
      : this()
    {
      this.VarCode = VarCode;
      this.NameBytes = Encoding.ASCII.GetBytes(NameText);
      this.ValueEmpty = ValueEmpty;
    }
    public OptionVariable(EnvironVarCode VarCode, string NameText)
      : this()
    {
      this.VarCode = VarCode;
      this.NameBytes = Encoding.ASCII.GetBytes(NameText);
    }

    /// <summary>
    /// check that the name bytes contain a special value name, such as IBMRSEED, 
    /// which contains a value immed after the name.
    /// </summary>
    /// <param name="NameBytes"></param>
    private void ParseNameBytes( byte[] NameBytes )
    {
      // IBMRSEED in ebcdic encoded bytes
      byte[] kwd = new byte[8] { 0x49, 0x42, 0x4d, 0x52, 0x53, 0x45, 0x45, 0x44 };
      if ( NameBytes.CompareEqual(kwd,8) == true )
      {
        // the remaining name bytes are the server seed.
        if ( NameBytes.Length > 8 )
        {
          this.IsSeedVar = true;
          this.SeedValue = NameBytes.SubArray(8);
        }
      }
    }
    public byte[] ToBytes()
    {
      ByteArrayBuilder ab = new ByteArrayBuilder();
      ab.Append(this.VarCode.ToByte());
      ab.Append(this.NameBytes);
      if ((this.ValueBytes != null) && (this.ValueBytes.Length > 0))
      {
        EnvironVarCode optnCode = EnvironVarCode.VALUE;
        ab.Append(optnCode.ToByte());
        ab.Append(this.ValueBytes);
      }
      else if ( this.ValueEmpty == true )
      {
        EnvironVarCode optnCode = EnvironVarCode.VALUE;
        ab.Append(optnCode.ToByte());
      }
      return ab.ToByteArray();
    }

    public override string ToString()
    {
      StringBuilder sb = new StringBuilder();
      // the option variable starts with VAR or USERVAR
      sb.Append(VarCode.ToString());

      // the variable name
      if ( this.NameBytes.Length > 0)
      {
      sb.Append(" ");

        if ( this.IsSeedVar == true )
        {
          sb.Append("IBMRSEED " + this.SeedValue.ToHex(' ')) ;
        }
        else
        {
          sb.Append(this.NameBytes.ToAscii());
        }
      }

      if ((this.ValueBytes != null) && (this.ValueBytes.Length > 0))
      {
        sb.Append(" VALUE ");
        var nameText = this.NameAsText;
        var valueText = this.ValueBytes.ToAscii();
        if ( nameText != null )
        {
          if (( nameText == "IBMRSEED") || ( nameText == "IBMSUBSPW"))
          {
            valueText = this.ValueBytes.ToHex(' ');
          }
        }
        sb.Append(valueText);
      }
      else if (this.ValueEmpty == true )
      {
        sb.Append(" VALUE ");
      }

      return sb.ToString();
    }

    /// <summary>
    /// construct an OptionVariable starting at the next byte in the input array.
    /// </summary>
    /// <param name="ByteArray"></param>
    /// <returns></returns>
    public static OptionVariable Construct(InputByteArray InputArray)
    {
      OptionVariable optnVar = null;

      if (InputArray.IsEof() == false)
      {
        var b1 = InputArray.PeekNextByte();
        var ev = b1.ToEnvironVarCode();
        if (ev != null)
        {
          var varCode = ev.Value;
          if ((varCode == EnvironVarCode.VAR) || (varCode == EnvironVarCode.USERVAR))
          {

            byte[] valueBytes = null;
            bool gotValue = false;

            // advance past the VAR or USERVAR code.
            InputArray.GetNextByte();

            // isolate the name text which follows the VAR or USERVAR code.
            // ( return the stop code. But do not consume it. )
            var rv = InputArray.GetBytesUntilCode(
              new byte[] { 0xff, 0x00, 0x01, 0x02, 0x03 });
            var nameBytes = rv.Item1;
            var endAccumCode = rv.Item2.ToEnvironVarCode();

            // the name text ends with VALUE marker. The text that follows is the text value
            // of the variable.
            if ((endAccumCode != null) && (endAccumCode.Value == EnvironVarCode.VALUE))
            {
              gotValue = true;

              // advance past the VALUE var code.
              InputArray.GetNextByte();

              // get the value bytes until end of value code.
              var rv2 = InputArray.GetBytesUntilCode(
                new byte[] { 0xff, 0x00, 0x01, 0x02, 0x03 });
              valueBytes = rv2.Item1;
            }

            optnVar = new OptionVariable(varCode, nameBytes, valueBytes);
            if (gotValue == true)
              optnVar.ValueEmpty = true;
          }
        }
      }

      return optnVar;
    }

  }
}

