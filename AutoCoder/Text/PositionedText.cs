using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AutoCoder.Ext;
using AutoCoder.Ext.System;

namespace AutoCoder.Text
{
  /// <summary>
  /// PositionedText is text + the start position of the text.
  /// </summary>
  public class PositionedText
  {
    public PositionedText()
    {
      this.Value = null;
    }

    public PositionedText(PositionedText Text)
    {
      this.Value = Text.Value;
      this.Position = Text.Position;
    }

    public void Append(string Text)
    {
      this.Value = this.Value + Text;
    }

    public char LastChar
    {
      get
      {
        if (this.IsEmpty() == true)
          throw new ApplicationException("text is empty");
        else
          return this.Value.Last();
      }
    }

    public int Length
    {
      get
      {
        return this.Value.Length;
      }
    }

    string _Value;
    public string Value
    {
      get
      {
        return _Value;
      }
      set
      {
        _Value = value;
        if (_Value == null)
          this.Position = -1;
      }
    }

    int _Position;
    public int Position
    {
      get { return _Position; }
      set
      {
        _Position = value;
      }
    }

    public void RemoveLastChar()
    {
      if (this.Value.Length > 0)
      {
        this.Value = this.Value.RemoveLastChar();
        if (this.Value.Length == 0)
          this.Position = -1;
      }
    }

    public override string ToString()
    {
      StringBuilder sb = new StringBuilder();
      sb.Append("Pos:" + this.Position.ToString());
      if (this.Value != null)
      {
        sb.Append(" " + this.Value);
      }
      return sb.ToString();
    }
  }

  public static class PositionedTextExt
  {
    public static void Clear(this PositionedText Text)
    {
      if (Text != null)
        Text.Value = null;
    }

    public static bool IsEmpty(this PositionedText Text)
    {
      if (Text == null)
        return true;
      else if (Text.Value == null)
        return true;
      else if (Text.Value.Length == 0)
        return true;
      else
        return false;
    }
  }
}

