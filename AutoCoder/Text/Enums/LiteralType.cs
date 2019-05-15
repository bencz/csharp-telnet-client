using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AutoCoder.Text.Enums
{

  // Integer is a string of digits, including sign.
  // Decimal is string of digits, with decimal point. Also "m" at end of string.
  
  //        The WordClassification enum indicates if a Numeric or Quoted value.
  //        LiteralType classifies as 
  //        either a basic string of digits. or ( 25m, 18.75 - DecimalLiteral ),
  //        FloatLiteral, maybe  
  // either have NumericLiteralType 
  public enum LiteralType
  {
    String,
    VerbatimString,
    Char,
    Integer,
    Decimal,
    Float,
    Long,
    none
  }

  public static class LiteralTypeExt
  {
    public static bool IsNumeric(this LiteralType LitType)
    {
      if ((LitType == LiteralType.Decimal)
        || (LitType == LiteralType.Float)
        || (LitType == LiteralType.Integer)
        || (LitType == LiteralType.Long))
        return true;
      else
        return false;
    }

    public static bool IsQuoted(this LiteralType LitType)
    {
      if ((LitType == LiteralType.String)
        || (LitType == LiteralType.VerbatimString)
        || (LitType == LiteralType.Char))
        return true;
      else
        return false;
    }

    public static ScanAtomCode ToScanAtomCode(this LiteralType LitType)
    {
      switch(LitType)
      {
        case LiteralType.Char:
        case LiteralType.String:
        case LiteralType.VerbatimString:
          return ScanAtomCode.Quoted;
        case LiteralType.Decimal:
        case LiteralType.Float:
        case LiteralType.Integer:
        case LiteralType.Long:
          return ScanAtomCode.Numeric;
        default:
          throw new ApplicationException("unsupported LiteralType. " + LitType.ToString()) ;
      }
    }

  }
}
