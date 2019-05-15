using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AutoCoder.Core.Enums
{

  // change to RelationOperator ?

  public enum ConditionOperator
  { LT, LE, GT, GE, EQ, NE, IS, none } ;

  public static class ConditionOperatorExt
  {
    public static string ToStatementString(this ConditionOperator Operator)
    {
      switch (Operator)
      {
        case ConditionOperator.LT:
          return "<";
        case ConditionOperator.LE:
          return "<=";
        case ConditionOperator.GT:
          return ">";
        case ConditionOperator.GE:
          return ">=";
        case ConditionOperator.EQ:
          return "==";
        case ConditionOperator.NE:
          return "!=";
        case ConditionOperator.IS:
          return "is";
        case ConditionOperator.none:
          return "";
        default:
          throw new ApplicationException("unsupported enum " + Operator.ToString());
      }
    }
  }
}
