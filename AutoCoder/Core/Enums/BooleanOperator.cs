using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AutoCoder.Core.Enums
{

  // another name : logical operator

  public enum BooleanOperator
  {
    And,
    Or,
    none
  }

  public static class BooleanOperatorExt
  {
    public static string ToStatementString(this BooleanOperator Operator)
    {
      switch (Operator)
      {
        case BooleanOperator.And:
          return "&&";
        case BooleanOperator.Or:
          return "||";
        case BooleanOperator.none:
          return "";
        default:
          throw new ApplicationException("unsupported enum " + Operator.ToString());
      }
    }
  }
}
