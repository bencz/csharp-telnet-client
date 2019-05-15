using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AutoCoder.Attribute
{
  [AttributeUsage(AttributeTargets.All)]
  public class IsEnumAttribute : global::System.Attribute
  {
    public readonly bool Value;

    public IsEnumAttribute(bool value)
    {
      Value = value;
    }

    public IsEnumAttribute(string value)
    {
      Value = Boolean.Parse(value);
    }

    public override string ToString()
    {
      return this.Value.ToString( );
    }

  }
}


