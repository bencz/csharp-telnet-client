using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AutoCoder.Core.Enums
{

  public enum ValueOperator
  {
    Plus, Minus, Mult, Div, Mod, none 
  }

  public static class ValueOperatorExt
  {
    public static string ToStatementString(this ValueOperator Operator)
    {
      switch (Operator)
      {
        case ValueOperator.Plus:
          return "+";
        case ValueOperator.Minus:
          return "-";
        case ValueOperator.Mult:
          return "*";
        case ValueOperator.Div:
          return "/";
        case ValueOperator.Mod:
          return "%";
        default:
          throw new ApplicationException("unsupported enum " + Operator.ToString());
      }
    }
  }
}
