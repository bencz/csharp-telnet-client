using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AutoCoder.Attribute
{
  [AttributeUsage(AttributeTargets.All)]
  public class EvaluatePriorityAttribute : global::System.Attribute
  {
    public int Value { get; set; }

    public EvaluatePriorityAttribute( int Value )
    {
      this.Value = Value;
    }

    public override string ToString()
    {
      return this.Value.ToString();
    }
  }
}
