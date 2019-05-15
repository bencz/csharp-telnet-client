using System;
using System.Collections.Generic;
using System.Text;

namespace AutoCoder.Text
{
  /// <summary>
  /// a case neutral string. Used for comparing strings without regard
  /// to case.
  /// </summary>
  public class MonoCaseString : IEquatable<string>
  {
    string mValue;
    string mLowerCaseValue;

    public MonoCaseString(string InValue)
    {
      mValue = InValue;
      mLowerCaseValue = InValue.ToLower();
    }

    public override int GetHashCode()
    {
      return base.GetHashCode();
    }

    public static bool operator !=(MonoCaseString InValue1, string InValue2)
    {
      return !(InValue1.Equals(InValue2));
    }

    public static bool operator ==(MonoCaseString InValue1, string InValue2)
    {
      return InValue1.Equals(InValue2);
    }

    public override bool Equals(object obj)
    {
      if (obj is string)
      {
        string s1 = (string)obj;
        return Equals(s1);
      }
      else
        return base.Equals(obj);
    }

    public bool Equals(string other)
    {
      string lc = other.ToLower();
      return (lc == mLowerCaseValue);
    }

    public override string ToString()
    {
      return mLowerCaseValue;
    }

  }
}
