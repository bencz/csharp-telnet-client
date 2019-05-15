using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AutoCoder.Core.Enums;

namespace AutoCoder.Text
{
  public class InsertBeforePosition : InsertPosition
  {
    public InsertBeforePosition(int Position)
      : base(RelativePosition.Before, Position)
    {
    }
  }
}


