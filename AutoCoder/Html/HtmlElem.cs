using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AutoCoder.Html
{
  public class HtmlElem : HtmlBase
  {
    public HtmlElem(string tag)
      : base(tag)
    {
    }

    public List<HtmlBase> Children
    {
      get
      {
        if (_Children == null)
          _Children = new List<HtmlBase>();
        return _Children;
      }
    }
    List<HtmlBase> _Children;

    public HtmlElem AddElem( string tag)
    {
      var child = new HtmlElem(tag);
      this.Children.Add(child);
      return child;
    }

    public HtmlText AddText( string tag, string text)
    {
      var child = new HtmlText(tag, text);
      this.Children.Add(child);
      return child;
    }

    public override string ToString()
    {
      var sb = new StringBuilder();
      sb.Append("<" + this.Tag + ">");

      // insert the children elements.
      foreach( var child in this.Children)
      {
        var text = child.ToString();
        sb.Append(text);
      }

      // close out the element.
      sb.Append("</" + this.Tag + ">" + Environment.NewLine);

      return sb.ToString();
    }
  }
}
