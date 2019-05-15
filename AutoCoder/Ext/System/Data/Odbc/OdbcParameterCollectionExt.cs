using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Odbc;
using System.Linq;
using System.Text;

namespace AutoCoder.Ext.System.Data.Odbc
{
  public static class OdbcParameterCollectionExt
  {
    public static OdbcParameter AddOdbcParameter(
      this OdbcParameterCollection Collection, string ParmName, int Length, string Value)
    {
      var parm = new OdbcParameter("@" + ParmName, OdbcType.Char, Length);
      parm.Value = Value;
      Collection.Add(parm);
      return parm;
    }

    public static OdbcParameter AddOdbcParameter(
      this OdbcParameterCollection Collection, string ParmName,
      int Length, int Precision, decimal Value)
    {
      var parm = new OdbcParameter("@" + ParmName, OdbcType.Decimal, Length);
      parm.Precision = (byte)Precision;
      parm.Value = Value;
      Collection.Add(parm);
      return parm;
    }
    public static OdbcParameter AddOdbcOutputParameter(
      this OdbcParameterCollection Collection, string ParmName, int Length)
    {
      var parm = new OdbcParameter("@" + ParmName, OdbcType.Char, Length);
      parm.Direction = ParameterDirection.Output;
      Collection.Add(parm);
      return parm;
    }
    public static OdbcParameter AddOdbcOutputParameter(
            this OdbcParameterCollection Collection, string ParmName,
            int Length, int Precision)
    {
      var parm = new OdbcParameter("@" + ParmName, OdbcType.Decimal, Length);
      parm.Precision = (byte)Precision;
      parm.Direction = ParameterDirection.Output;
      Collection.Add(parm);
      return parm;
    }
  }
}
