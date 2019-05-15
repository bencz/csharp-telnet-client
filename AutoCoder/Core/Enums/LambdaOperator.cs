using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AutoCoder.Core.Enums
{

  public enum LambdaOperator
  {
    Lambda
  }

  public static class LambdaOperatorExt
  {
    public static string ToStatementString(this LambdaOperator Operator)
    {
      switch (Operator)
      {
        case LambdaOperator.Lambda:
          return "=>";
        default:
          throw new ApplicationException("unsupported enum " + Operator.ToString());
      }
    }

    public static LambdaOperator? TryParseStatementText(string Text)
    {
      if (Text == "=>")
        return LambdaOperator.Lambda;
      else
        return null;
    }

  }
}
