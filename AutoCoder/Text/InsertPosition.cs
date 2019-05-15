using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AutoCoder.Core.Enums;

namespace AutoCoder.Text
{
  public class InsertPosition
  {
    int _Value;
    public int Value
    {
      get { return _Value; }
      set
      {
        if (value < 0)
          throw new ApplicationException("Insert position less than zero");
        _Value = value;
      }
    }

    RelativePosition _Relative;
    public RelativePosition Relative
    {
      get
      {
        return _Relative;
      }
      set
      {
        if ((_Relative == RelativePosition.At) || (_Relative == RelativePosition.None))
          throw new ApplicationException("invalid relative position");
        _Relative = value;
      }
    }

    public InsertPosition(RelativePosition Relative)
    {
      this.Relative = Relative;
      this.Value = 0;
    }

    public InsertPosition( RelativePosition Relative, int Value )
    {
      this.Relative = Relative;
      this.Value = Value;
    }

    public bool IsWithin(int Start, int End)
    {
      if ((this.Value >= Start) && (this.Value <= End))
        return true;
      else
        return false;
    }

    public bool IsWithin( StringRange Range )
    {
      if (( this.Value >= Range.Start ) && ( this.Value <= Range.End ))
        return true ;
      else
        return false ;
    }

    public InsertPosition PosWithinRange( StringRange Range )
    {
      int pos = this.Value - Range.Start ;
      var rltv = this.Relative ;
      return new InsertPosition( rltv, pos ) ;
    }

    public InsertPosition AddTo(int Value)
    {
      int pos = this.Value + Value;
      return new InsertPosition(this.Relative, pos);
    }

    public InsertPosition SubtractFrom( int Value )
    {
      int pos = this.Value - Value;
      return new InsertPosition(this.Relative, pos);
    }

    public override string ToString()
    {
      return this.Relative.ToString() + " " + Value.ToString();
    }

    public static InsertPosition operator +(InsertPosition Value1, int Value2)
    {
      var ip = new InsertPosition(Value1.Relative, Value1.Value + Value2);
      return ip;
    }

    public static InsertPosition operator -(InsertPosition Value1, int Value2)
    {
      var ip = new InsertPosition(Value1.Relative, Value1.Value - Value2);
      return ip;
    }
  }
}
