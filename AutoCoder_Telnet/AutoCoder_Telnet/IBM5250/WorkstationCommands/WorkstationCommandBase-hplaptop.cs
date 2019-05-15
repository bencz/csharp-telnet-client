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
      WorkstationCommandBase dsCmd = null;

      if (InputArray.RemainingLength >= 2)
      {
        // command codes as documented on 15.2 - 1 of 5494 function ref manual
        var buf = InputArray.PeekBytes(2);
        if (buf[0] == 0x04)
        {
          var wrkstnCode = buf[1].ToWorkstationCode();
          if (wrkstnCode != null)
          {
            var code = wrkstnCode.Value;
            if (code == WorkstationCode.ClearUnit)
              dsCmd = new ClearUnitCommand(InputArray);

            else if (code == WorkstationCode.WTD)
              dsCmd = new WriteToDisplayCommand(InputArray);

            else if (code == WorkstationCode.ReadMdtFields)
              dsCmd = new ReadMdtFieldsCommand(InputArray);

            // 04 F3. write structure field. Used as vehicle for the D9 70 5250 query
            // command.
            else if (code == WorkstationCode.WriteStructuredField)
            {
              dsCmd = new WriteStructuredFieldCommand(InputArray);
            }

            else if (code == WorkstationCode.SaveScreen)
            {
              dsCmd = new SaveScreenCommand(InputArray);
            }

            else if (code == WorkstationCode.RestoreScreen)
            {
              dsCmd = new RestoreScreenCommand(InputArray);
            }

            else
            throw new Exception(
              "workstation data stream command code not supported. " +
              code.ToString());
          }
        }
      }

      return dsCmd;
    }

    public virtual int GetDataStreamLength()
    {
      return this.BytesLength;
    }
    public virtual byte[] ToBytes( )
    {
      var ba = new ByteArrayBuilder();
      ba.Append(0x04);
      ba.Append((byte)this.CmdCode);
      return ba.ToByteArray();
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
