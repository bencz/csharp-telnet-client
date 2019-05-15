using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Data ;
using System.Data.SqlTypes;
using System.Data.SqlClient;

namespace AutoCoder.Sql
{
  public class SqlCore
  {

    public static void CreateDatabase(
      SqlConnection InConn, string InDatabaseName)
    {
      SqlCommand cmd = new SqlCommand();
      cmd.Connection = InConn;
      cmd.CommandType = CommandType.Text;
      cmd.Parameters.Add(new SqlParameter("@DatabaseName", InDatabaseName));
      cmd.CommandText =
        "create database  " + InDatabaseName;
      cmd.ExecuteNonQuery();
    }

    /// <summary>
    /// drop the index from sql database
    /// </summary>
    /// <param name="InConn"></param>
    /// <param name="InIndexName"></param>
    public static void DropIndex(
      SqlConnection InConn, string InTableName, string InIndexName)
    {
      SqlCommand cmd = new SqlCommand();
      cmd.Connection = InConn;
      cmd.CommandType = CommandType.Text;
      cmd.Parameters.Add(new SqlParameter("@TableName", InTableName));
      cmd.Parameters.Add(new SqlParameter("@IndexName", InIndexName));
      cmd.CommandText =
        "drop index      " + InIndexName + " " +
        "on              " + InTableName; 
      cmd.ExecuteNonQuery();
    }

    /// <summary>
    /// Drop the table from the connection string located database.
    /// </summary>
    /// <param name="InConnString"></param>
    /// <param name="InTableName"></param>
    public static void DropTable( string InConnString, string InTableName)
    {
      using (SqlConnection conn = new SqlConnection( InConnString ))
      {
        conn.Open();
        DropTable(conn, InTableName);
      }
    }

    /// <summary>
    /// Drop the table from the connected to database
    /// </summary>
    /// <param name="InConn"></param>
    /// <param name="InTableName"></param>
    public static void DropTable(SqlConnection InConn, string InTableName)
    {
      SqlCommand cmd = null;

      // first, drop all the indexes of the table.
      string[] indexesOfTable = GetTableIndexes(InConn, InTableName);
      foreach( string indexName in indexesOfTable )
      {
        DropIndex( InConn, InTableName, indexName ) ;
      }

      // now, drop the actual table.
      cmd = new SqlCommand("DROP TABLE " + InTableName, InConn);
      cmd.ExecuteNonQuery();
    }

    /// <summary>
    /// Determine if index exists within the connected to database. 
    /// </summary>
    /// <param name="InConn"></param>
    /// <param name="InIndexName"></param>
    /// <returns></returns>
    public static bool IndexExists(SqlConnection InConn, string InIndexName)
    {
      bool doesExist = false;
      SqlDataReader rdr = null;
      SqlCommand cmd = null;

      try
      {
        cmd = new SqlCommand();
        cmd.Connection = InConn;
        cmd.CommandType = CommandType.Text;
        cmd.Parameters.Add(new SqlParameter("@IndexName", InIndexName));
        cmd.CommandText = 
          "select          count(*) " +
          "from            sys.indexes a " +
          "join            sys.objects b " +
          "on              a.object_id = b.object_id " +
          "where           b.type = 'U' " +
          "                and a.is_primary_key = 0 " +
          "                and a.name = @IndexName " +
          "                and a.index_id > 1 " ;

        rdr = cmd.ExecuteReader();
        bool rc = rdr.Read();
        if (rc == true)
        {
          int rowCx = rdr.GetInt32(0);
          if (rowCx > 1)
            throw new ApplicationException("invalid index exists sql expression");
          else if (rowCx == 1)
            doesExist = true;
        }
      }
      finally
      {
        if (rdr != null)
          rdr.Close();
      }
      return doesExist;
    }

    public static string[] GetTableIndexes(SqlConnection InConn, string InTableName)
    {
      SqlDataReader rdr = null;
      SqlCommand cmd = null;
      List<string> listIndexes = new List<string>();

      try
      {
        cmd = new SqlCommand();
        cmd.Connection = InConn;
        cmd.CommandType = CommandType.Text;
        cmd.Parameters.Add(new SqlParameter("@TableName", InTableName));
        cmd.CommandText =
          "select          b.name Index_Name " +
          "from            sys.objects a " +
          "join            sys.indexes b " +
          "on              a.object_id = b.object_id " +
          "where           a.type = 'U' " +
          "                and a.name = @TableName " +
          "                and b.is_primary_key = 0 " +
          "                and b.index_id > 1 ";

        rdr = cmd.ExecuteReader();
        while (true)
        {
          bool rc = rdr.Read();
          if (rc == false)
            break;
          string indexName = rdr.GetString(0);
          listIndexes.Add(indexName);
        }
      }
      finally
      {
        if (rdr != null)
          rdr.Close();
      }

      return listIndexes.ToArray();
    }

    /// <summary>
    /// Determine if table exists within the database of the connection string.
    /// </summary>
    /// <param name="InConnString"></param>
    /// <param name="InTableName"></param>
    /// <returns></returns>
    public static bool TableExists( string InConnString, string InTableName)
    {
      bool doesExist = false;
      using (SqlConnection conn = new SqlConnection( InConnString ))
      {
        conn.Open();
        doesExist = TableExists(conn, InTableName);
      }
      return doesExist ;
    }

    /// <summary>
    /// Determine if table exists within the connected to database.
    /// </summary>
    /// <param name="InConn"></param>
    /// <param name="InTableName"></param>
    /// <returns></returns>
    public static bool TableExists(SqlConnection InConn, string InTableName)
    {
      bool doesExist = false;
      SqlDataReader rdr = null;
      SqlCommand cmd = null;

      try
      {
        cmd = new SqlCommand();
        cmd.Connection = InConn;
        cmd.CommandType = CommandType.Text;
        cmd.CommandText =
          "select     count(*) " +
          "from       INFORMATION_SCHEMA.TABLES " +
          "where      table_type = 'BASE TABLE' " +
          "           and table_name = @TableName ";
        cmd.Parameters.Add(new SqlParameter("@TableName", InTableName));

        rdr = cmd.ExecuteReader();
        bool rc = rdr.Read();
        if (rc == true)
        {
          int rowCx = rdr.GetInt32(0);
          if (rowCx > 1)
            throw new ApplicationException("invalid table exists sql expression");
          else if (rowCx == 1)
            doesExist = true;
        }
      }
      finally
      {
        if (rdr != null)
          rdr.Close();
      }
      return doesExist;
    }

    public static void UpdateRow(
      SqlConnection InConn, string InTable, string InRowSelect,
      Dictionary<string,string> InColumns)
    {
      SqlCommand cmd = null;
      string AssignValues;

      // build column = value string from column dictionary
      AssignValues = BuildAssignValuesString(InConn, InTable, InColumns);

      cmd = new SqlCommand();
      cmd.Connection = InConn;
      cmd.CommandType = CommandType.Text;

      cmd.CommandText =
        "UPDATE " + InTable +
        " SET " + AssignValues +
        " WHERE " + InRowSelect;

      cmd.ExecuteNonQuery();
    }

    // build column = value string from column dictionary
    private static string BuildAssignValuesString(
      SqlConnection InConn, string InTable, Dictionary<string,string> InColumns)
    {
      StringBuilder sb = new StringBuilder(512);

      foreach (KeyValuePair<string,string> dice in InColumns)
      {
        string ColumnName = dice.Key.ToString();
        string ColumnValue = dice.Value.ToString();
        if (sb.Length > 0)
          sb.Append(", ");
        sb.Append(ColumnName + " = '" + ColumnValue + "'");
      }
      return sb.ToString();
    }


  }
}
