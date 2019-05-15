using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AutoCoder.Core.Enums
{

  // change to RelationOperator ?

  public enum CompareOperator
  { LT, LE, GT, GE, EQ, NE, IS, none } ;

  public static class CompareOperatorExt
  {
    public static string ToStatementString(this CompareOperator Operator)
    {
      switch (Operator)
      {
        case CompareOperator.LT:
          return "<";
        case CompareOperator.LE:
          return "<=";
        case CompareOperator.GT:
          return ">";
        case CompareOperator.GE:
          return ">=";
        case CompareOperator.EQ:
          return "==";
        case CompareOperator.NE:
          return "!=";
        case CompareOperator.IS:
          return "is";
        case CompareOperator.none:
          return "";
        default:
          throw new ApplicationException("unsupported enum " + Operator.ToString());
      }
    }

    public static CompareOperator? TryParseStatementText(string Text)
    {
      if (Text == "==")
        return CompareOperator.EQ;
      else if (Text == "<=")
        return CompareOperator.LT;
      else if (Text == ">=")
        return CompareOperator.GT;
      else if (Text == ">")
        return CompareOperator.GT;
      else if (Text == "<")
        return CompareOperator.LT;
      else if (Text == "!=")
        return CompareOperator.NE;
      else if (Text == "is")
        return CompareOperator.IS;
      else
        return null;
    }

  }
}
