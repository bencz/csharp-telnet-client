using AutoCoder.Telnet.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoCoder.Telnet.Common
{
  public class SubjectByte
  {
    public TelnetSubject? Code
    { get; set; }

    public byte Value
    { get; set; }

    public SubjectByte(byte Value)
    {
      this.Value = Value;
      this.Code = Value.ToTelnetSubject();
    }

    public SubjectByte(TelnetSubject Code)
    {
      this.Code = Code;
      this.Value = (byte)Code;
    }

    public override string ToString()
    {
      if (this.Code != null)
        return this.Code.Value.ToString();
      else
        return this.Value.ToString();
    }

    public bool IsEqual(TelnetSubject Code)
    {
      if (this.Code == null)
        return false;
      else
        return (this.Code.Value == Code);
    }
  }

  public static class SubjectByteExt
  {
    public static SubjectByte ToSubjectByte(this byte Value)
    {
      return new SubjectByte(Value);
    }
  }
}
