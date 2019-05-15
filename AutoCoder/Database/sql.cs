using System;
using System.Collections ;
using System.Data;
using System.Data.SqlClient ;
using System.Data.SqlTypes;
using System.Text ;

namespace AutoCoder.Database
{
	/// <summary>
	/// Summary description for sql.
	/// </summary>
	public class Sql
	{
		public Sql()
		{
		}

		public static void UpdateRow(
			SqlConnection InConn, string InTable, string InRowSelect, Hashtable InColumns )
		{
			SqlCommand cmd = null ;
			string AssignValues ;

			// build column = value string from column dictionary
			AssignValues = BuildAssignValuesString( InConn, InTable, InColumns ) ;

			cmd = new SqlCommand();
			cmd.Connection = InConn ;
			cmd.CommandType = CommandType.Text;

			cmd.CommandText =
				"UPDATE " + InTable +
				" SET " + AssignValues +
				" WHERE " + InRowSelect ;

			cmd.ExecuteNonQuery( ) ;
		}

		// build column = value string from column dictionary
		private static string BuildAssignValuesString(
			SqlConnection InConn, string InTable, Hashtable InColumns )
		{
			StringBuilder sb = new StringBuilder( 512 ) ;
 
			foreach( DictionaryEntry hte in InColumns )
			{
				string ColumnName = hte.Key.ToString( ) ;
				string ColumnValue = hte.Value.ToString( ) ;
				if ( sb.Length > 0 )
					sb.Append( ", " ) ;
				sb.Append( ColumnName + " = '" + ColumnValue + "'" ) ;
			}
			return sb.ToString( ) ;
		}

	} // end class Sql
}
