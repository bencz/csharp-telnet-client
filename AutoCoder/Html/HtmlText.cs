using AutoCoder.Ext.System;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;

namespace AutoCoder.Html
{
  public class HtmlText : HtmlBase
  {
    public HtmlText( string tag, string Text)
      : base(tag)
    {
      this.Text = Text;
    }
    public string Text { get; set; }

    public override string ToString()
    {
      var sb = new StringBuilder();
      sb.Append("<" + this.Tag + ">");

      var s1 =this.Text.TrimEndWhitespace();
      sb.Append( WebUtility.HtmlEncode(s1));

      sb.Append("</" + this.Tag + ">");

      return sb.ToString();
    }

  }
}
