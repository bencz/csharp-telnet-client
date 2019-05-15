using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AutoCoder.Html
{
  public abstract class HtmlBase
  {
    public HtmlBase(string tag)
    {
      this.Tag = tag;
    }
    public string Tag { get; set; }

  }
}
