
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AutoCoder.Text
{
  /// <summary>
  /// from and to position within a string
  /// </summary>
  public class StringRange
  {
    int _Start;
    public int Start
    {
      get
      {
        if (_Start == -1)
        {
          if ((_End == -1) || (_Length == -1))
            throw new Exception("Cannot calc Start position.");
          else
            _Start = _End - _Length + 1;
        }
        return _Start;
      }
      set { _Start = value; }
    }

    int _End;
    public int End
    {
      get
      {
        if (_End == -1)
        {
          if ((_Start == -1) || (_Length == -1))
            throw new Exception("Cannot calc End position.");
          else
            _End = _Start + _Length - 1;
        }
        return _End;
      }
      set { _End = value; }
    }

    int _Length;
    public int Length
    {
      get
      {
        if (_Length == -1)
        {
          if ((_Start == -1) || (_End == -1))
            throw new Exception("Cannot calc Length.");
          else
            _Length = _End - _Start + 1;
        }
        return _Length;
      }
      set
      {
        _Length = value;
      }
    }

    public StringRange()
    {
      _Start = -1;
      _End = -1;
      _Length = -1;
    }

    public StringRange(int Start, int End)
      : this()
    {
      _Start = Start;
      _End = End;
    }

    public bool IsWithinRange(int Pos)
    {
      if ((Pos >= this.Start) && (Pos <= this.End))
        return true;
      else
        return false;
    }

    public int PositionWithinRange(int Pos)
    {
      return Pos - this.Start;
    }
  }
}
