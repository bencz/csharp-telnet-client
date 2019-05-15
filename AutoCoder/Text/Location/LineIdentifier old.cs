using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AutoCoder.Text.Location
{
  /// <summary>
  /// value that identifies a text line in a text stream.
  /// </summary>
  public class LineIdentifier
  {
    public int Value
    {
      get;
      set;
    }

    public LineIdentifier(int Value)
    {
      this.Value = Value;
    }

    public override bool Equals(object obj)
    {
      if (obj is LineIdentifier)
        return ((obj as LineIdentifier).Value == this.Value);
      else
        return base.Equals(obj);
    }

    public override int GetHashCode()
    {
      return base.GetHashCode();
    }

    public override string ToString()
    {
      return this.Value.ToString();
    }
  }
}
