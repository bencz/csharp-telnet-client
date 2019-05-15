using AutoCoder.Ext.System;
using AutoCoder.Systm;
using AutoCoder.Telnet.Common;
using AutoCoder.Telnet.Enums.IBM5250;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoCoder.Telnet.IBM5250.WorkstationCommands
{
  // derived classes:  ClearUnitCommand, ReadMdtFieldsCommand, RestoreScreenCommand,
  //                   WriteStructuredFieldCommand, WriteToDisplayCommand,
  //                   WriteErrorCodeCommand
  public abstract class WorkstationCommandBase
  {
    public WorkstationCode CmdCode
    { get; set; }

    /// <summary>
    /// the bytes from the input stream from which the command was constructed.
    /// </summary>
    public byte[] CommandBytes
    {
      get
      {
        if (this.InputBytes == null)
          return null;
        else if ((this.BytesStart == 0) || (this.BytesLength == 0))
          return new byte[] { };
        else
        {
          // adjust length for now if it exceeds input bytes.
          int lx = this.BytesLength;
          if (this.InputBytes.Length < (this.BytesStart + lx))
            lx = this.InputBytes.Length - this.BytesStart;
          return this.InputBytes.SubArray(this.BytesStart, lx );
        }
      }
    }

    public string Errmsg
    { get; set; }

    protected byte[] InputBytes
    { get; private set; }

    /// <summary>
    /// start pos of this data stream command in the InputBytes byte stream.
    /// </summary>
    protected int BytesStart
    { get; private set; }

    /// <summary>
    /// byte length of this data stream command.
    /// </summary>
    protected int BytesLength
    { get; set; }

    public WorkstationCommandBase(InputByteArray InputArray, WorkstationCode CmdCode)
    {
      this.CmdCode = CmdCode;
      this.InputBytes = InputArray.Bytes;
      this.BytesStart = InputArray.Index;
      this.BytesLength = 2;
    }

    public WorkstationCommandBase( WorkstationCode CmdCode )
    {
      this.CmdCode = CmdCode;
    }

    public static WorkstationCommandBase ParseFactory(InputByteArray InputArray)
    {
      WorkstationCommandBase wrkstnCmd = null;

      if (InputArray.RemainingLength >= 2)
      {
        // command codes as documented on 15.2 - 1 of 5494 function ref manual
        var buf = InputArray.PeekBytes(2);
        if (buf[0] == 0x04)
        {
          var funcCode = buf[1].ToWorkstationCode();
          if (funcCode != null)
          {
            var code = funcCode.Value;
            if (code == WorkstationCode.ClearUnit)
              wrkstnCmd = new ClearUnitCommand(InputArray);

            else if (code == WorkstationCode.WTD)
              wrkstnCmd = new WriteToDisplayCommand(InputArray);

            else if (code == WorkstationCode.ReadMdtFields)
              wrkstnCmd = new ReadMdtFieldsCommand(InputArray);

            else if (code == WorkstationCode.ReadInputFields)
            {
              throw new Exception("read input fields workstation command not supported");
            }

            // 04 F3. write structure field. Used as vehicle for the D9 70 5250 query
            // command.
            else if (code == WorkstationCode.WriteStructuredField)
            {
              wrkstnCmd = new WriteStructuredFieldCommand(InputArray);
            }

            else if (code == WorkstationCode.SaveScreen)
            {
              wrkstnCmd = new SaveScreenCommand(InputArray);
            }

            else if (code == WorkstationCode.RestoreScreen)
            {
              wrkstnCmd = new RestoreScreenCommand(InputArray);
            }

            else if (code == WorkstationCode.WriteErrorCode)
            {
              wrkstnCmd = new WriteErrorCodeCommand(InputArray);
            }

            else if ( code == WorkstationCode.ReadScreen)
            {
              wrkstnCmd = new ReadScreenCommand(InputArray);
            }

            // 04 F4 - write single structured field. Use to set keyboard
            // buffering.
            else if ( code == WorkstationCode.WriteSingleStructuredField)
            {
              wrkstnCmd = new WriteSingleStructuredFieldCommand(InputArray);
            }

            else if ( code == WorkstationCode.ClearUnitAlternate)
            {
              wrkstnCmd = new ClearUnitAlternateCommand(InputArray);
            }

            else
            throw new Exception(
              "workstation data stream command code not supported. " +
              code.ToString());
          }
        }
      }

      return wrkstnCmd;
    }

    public virtual int GetDataStreamLength()
    {
      return this.BytesLength;
    }
    public virtual HowReadScreen? GetHowReadScreenCode( )
    {
      return null;
    }
    public virtual byte[] ToBytes( )
    {
      var ba = new ByteArrayBuilder();
      ba.Append(0x04);
      ba.Append((byte)this.CmdCode);
      return ba.ToByteArray();
    }

    public static byte[] ToBytes( WorkstationCode CmdCode )
    {
      var buf = new byte[2];
      buf[0] = 0x04;
      buf[1] = (byte)CmdCode;
      return buf;
    }

    public string ToHexString( )
    {
      return "Lgth: " + this.BytesLength + " Bytes: " +
        this.CommandBytes.ToHex(' ');
    }

    public virtual string[] ToReportLines( )
    {
      List<string> lines = new List<string>();
      lines.Add(this.ToString());
      lines.Add(this.ToHexString());
      return lines.ToArray();
    }

    public override string ToString()
    {
      return "Workstation command. " + this.CmdCode.ToString();
    }
  }
}
