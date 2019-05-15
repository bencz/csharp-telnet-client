using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace AutoCoder.Ext
{
  public static class XAttributeExt
  {

    public static bool? BooleanOrDefault(
      this XAttribute Attribute, bool? Default = null)
    {
      if (Attribute == null)
        return Default;
      else
      {
        bool bv;
        bool rc = Boolean.TryParse(Attribute.Value, out bv);
        if (rc == true)
          return bv;
        else
          return Default;
      }
    }

    public static DateTime? DateTimeOrDefault(
      this XAttribute Attribute, DateTime? Default = null)
    {
      if (Attribute == null)
        return Default;
      else
      {
        DateTime dt;
        bool rc = DateTime.TryParse(Attribute.Value, out dt);
        if (rc == true)
          return dt;
        else
          return Default;
      }
    }

    public static decimal? DecimalOrDefault(
      this XAttribute Attribute, decimal? Default = null)
    {
      if (Attribute == null)
        return Default;
      else
      {
        decimal dv;
        bool rc = Decimal.TryParse(Attribute.Value, out dv);
        if (rc == true)
          return dv;
        else
          return Default;
      }
    }

    public static double? DoubleOrDefault(
      this XAttribute Attribute, double? Default = null)
    {
      if (Attribute == null)
        return Default;
      else
      {
        double dv;
        bool rc = Double.TryParse(Attribute.Value, out dv);
        if (rc == true)
          return dv;
        else
          return Default;
      }
    }

    public static int? IntOrDefault(
      this XAttribute Attribute, int? Default = null)
    {
      if (Attribute == null)
        return Default;
      else
      {
        int iv;
        bool rc = Int32.TryParse(Attribute.Value, out iv);
        if (rc == true)
          return iv;
        else
          return Default;
      }
    }

    public static string StringOrDefault(this XAttribute Attribute, string Default = "")
    {
      if (Attribute == null)
        return Default;
      else
        return Attribute.Value;
    }
  }
}
