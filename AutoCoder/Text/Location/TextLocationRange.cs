using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AutoCoder.Text.Location
{
  public class TextLocationRange
  {
    public TextLocationRange()
    {
      this.From = null;
      this.To = null;
    }

    public TextLocationRange(TextLocation From, TextLocation To)
    {
      this.From = From;
      this.To = To;
    }

    public TextLocation From
    {
      get;
      set;
    }

    public TextLocation To
    {
      get;
      set;
    }
  }
}
