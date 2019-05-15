using AutoCoder.Telnet.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoCoder.Telnet.Common
{
  // name was ReplyOptionVariable

  public class BuildOptionVariable
  {
    /// <summary>
    /// single byte code that indicates if the name value that follows is VAR or
    /// USERVAR.
    /// </summary>
    public EnvironVarCode NameCode
    { get; set; }

    public string NameText
    { get; set; }

    /// <summary>
    /// text value of the variable. Will be converted to ascii bytes when composing 
    /// the option variable. 
    /// </summary>
    public string ValueText
    { get; set; }

    public byte[] ValueBytes
    { get; set; }

    public bool ValueEmpty
    { get; set; }

    /// <summary>
    /// value substitution name. telnet connection code will store a list of values,
    /// such as "server seed", "user name", "device name". When composing the 
    /// option variable byte stream match those values with 
    /// </summary>
    public string ValueSubstitute
    { get; set; }

    public BuildOptionVariable( EnvironVarCode NameCode, string NameText )
    {
      this.NameCode = NameCode;
      this.NameText = NameText;
    }

    public OptionVariable ToOptionVariable( Dictionary<string,string> SbsVarDict )
    {
      var optnVar = new OptionVariable(this.NameCode, this.NameText);
      if (this.ValueEmpty == true)
        optnVar.ValueEmpty = true;
      if (this.ValueText != null)
        optnVar.ValueText = this.ValueText;
      if (this.ValueBytes != null)
        optnVar.ValueBytes = this.ValueBytes;

      // a subsitution variable name is spcfd. Retrieve the value from the 
      // substitution value dictionary. Apply the value as the value of this variable.
      if ( this.ValueSubstitute != null )
      {
        if ( SbsVarDict.ContainsKey(this.ValueSubstitute))
        {
          optnVar.ValueText = SbsVarDict[this.ValueSubstitute];
        }
      }

      return optnVar;
    }

    public override string ToString()
    {
      var s1 = NameCode.ToString() + " " + this.NameText;
      if (this.ValueText != null)
        s1 += " " + this.ValueText;
      return s1;
    }
  }
}
