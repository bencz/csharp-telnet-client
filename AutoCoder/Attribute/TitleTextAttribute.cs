using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AutoCoder.Attribute
{

  /// <summary>
  /// TitleText stores the title= values of an html element.
  /// The title of an element is the tooltip popup text that displays as a popup
  /// when the mouse hovers over the element.
  /// </summary>
  [AttributeUsage(AttributeTargets.All)]
  public class TitleTextAttribute : global::System.Attribute
  {
    string mText;

    public TitleTextAttribute(string Text)
    {
      this.Text = Text;
    }

    public string Text
    {
      get { return mText; }
      set { mText = value; }
    }
  }
}
