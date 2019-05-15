using System;
using System.Collections.Generic;
using System.Data.Odbc;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScreenDefnLib.Scripts
{
  public static class SupportCode
  {
    public static IEnumerable<string> GetSrcmbrLines(
      string SrcfName, string SrcfLib, string SrcmbrName)
    {
      var userDsn = "S2222";
      var userName = "srichter";
      var password = "denville";
      var lines = new List<string>();

      using (var conn = OpenConnection(userDsn, userName, password))
      {
        var aliasCmd = conn.CreateCommand();
        aliasCmd.CommandText = "create or replace alias qtemp.demo3r for " + SrcfLib +
          "." + SrcfName + "(" + SrcmbrName + ")";
        aliasCmd.ExecuteNonQuery();

        var cmd = conn.CreateCommand();
        cmd.CommandText = "select srcseq, srcdat, srcdta " +
          "from qtemp.demo3r " +
          " order by srcseq ";

        using (OdbcDataReader reader = cmd.ExecuteReader())
        {
          while (reader.Read())
          {
            string srcseq = reader.GetString(0);
            string srcdat = reader.GetString(1);
            string srcdta = reader.GetString(2);

            lines.Add(srcdta);
          }
        }
      }
      return lines;
    }

    private static OdbcConnection OpenConnection(string Dsn, string User, string Pwd)
    {
      string connString = "DSN=" + Dsn + "; UID=SRICHTER; PWD=DENVILLE;";
      var conn = new OdbcConnection(connString);
      conn.Open();
      return conn;
    }
  }
}
