using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AutoCoder.Text;

namespace AutoCoder.Core.Enums
{

  public enum AssignmentOperator
  {
    Equals,
    PlusEquals,
    MinusEquals,
    MultEquals,
    DivEquals,
    none
  }

  public static class AssignmentOperatorExt
  {
    public static string ToStatementString(this AssignmentOperator Operator)
    {
      switch (Operator)
      {
        case AssignmentOperator.Equals:
          return "=";
        case AssignmentOperator.PlusEquals:
          return "+=";
        case AssignmentOperator.MinusEquals:
          return "-=";
        case AssignmentOperator.MultEquals:
          return "*=";
        case AssignmentOperator.DivEquals:
          return "/=";
        case AssignmentOperator.none:
          return "";
        default:
          throw new ApplicationException("unsupported enum " + Operator.ToString());
      }
    }

    public static AssignmentOperator ParseSymbol(string Symbol)
    {
      if (Symbol == "=")
        return AssignmentOperator.Equals;
      else if (Symbol == "+=")
        return AssignmentOperator.PlusEquals;
      else if (Symbol == "-=")
        return AssignmentOperator.MinusEquals;
      else if (Symbol == "*=")
        return AssignmentOperator.MultEquals;
      else if (Symbol == "/=")
        return AssignmentOperator.DivEquals;
      else
        throw new ApplicationException("Unrecognized assignment operator symbol");
    }

    public static AssignmentOperator? TryParseStatementText(string Text)
    {
      if (Text == "=")
        return AssignmentOperator.Equals;
      else if (Text == "+=")
        return AssignmentOperator.PlusEquals;
      else if (Text == "-=")
        return AssignmentOperator.MinusEquals;
      else if (Text == "*=")
        return AssignmentOperator.MultEquals;
      else if (Text == "/=")
        return AssignmentOperator.DivEquals;
      else
        return null;
    }
  }
}
