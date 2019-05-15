using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AutoCoder.Attribute
{
  [AttributeUsage(AttributeTargets.All)]
  public class PromptTextAttribute : global::System.Attribute
  {
    public readonly string Text;

    public PromptTextAttribute(string text)
    {
      Text = text;
    }

    public override string ToString()
    {
      return this.Text;
    }

  }
}
