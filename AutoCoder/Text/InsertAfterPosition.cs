using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AutoCoder.Core.Enums;

namespace AutoCoder.Text
{
  public class InsertAfterPosition : InsertPosition
  {
    public InsertAfterPosition( int Position )
      : base(RelativePosition.After, Position )
    {

    }
  }
}
