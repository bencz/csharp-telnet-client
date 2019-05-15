using System;
using System.Configuration;
using System.Data ;
using System.Data.SqlClient ;
using AutoCoder.Web ;

namespace AutoCoder.WebTables
{

	// ------------------------- UserMasterRow ---------------------------
	public class UserMasterRow
	{
		private string mUserId ;
		private string mPassword ;
		private string mUserName ;
		private string mEmailAddress ;

		public UserMasterRow( )
		{
		}

		public string UserId
		{
			get { return mUserId ; }
			set { mUserId = value ; }
		}
		public string Password
		{
			get { return mPassword ; }
			set { mPassword = value ; }
		}
		public string UserName
		{
			get { return mUserName ; }
			set { mUserName = value ; }
		}
		public string EmailAddress
		{
			get { return mEmailAddress ; }
			set { mEmailAddress = value ; }
		}
	}

	// -------------------------- UserMasterException ---------------------
	public class UserMasterException : ApplicationException
	{

		public UserMasterException( string InMessage )
			: base( InMessage )
		{
		}
		public UserMasterException( string InMessage, Exception InInnerException )
			: base( InMessage, InInnerException )
		{
		}
	}

	// --------------------------- UserMaster --------------------------
	// the UserMaster table stores the linkage between a session guid identifier
	// ( UserMaster ) and the UserName of the successfully logged in customer.
	public class UserMaster
	{
		string mTableName ;

		public UserMaster( )
		{
		}

		public string TableName
		{
			get
			{
				if ( mTableName == null )
					throw( new ApplicationException( "UserMaster table name not assigned" )) ;
				return mTableName ;
			}
			set { mTableName = value ; }
		}

		// -------------------------- Add --------------------------------
		public virtual UserMasterRow Add(
			AcPage InPage,
			UserMasterRow InRow )
		{
			SqlConnection conn = null ;

			try
			{
				conn = InPage.GetDatabaseConnection( ) ;
				SqlCommand cmd = new SqlCommand();

				// assign the UserId
				if ( InRow.UserId == null )
				{
					InRow.UserId = System.Guid.NewGuid( ).ToString( ) ;
				}

				cmd.Connection = conn ;
				cmd.CommandType = CommandType.Text;
				cmd.Parameters.Add(new SqlParameter("@UserId", InRow.UserId )) ;
				cmd.Parameters.Add(new SqlParameter("@Password", InRow.Password )) ;
				cmd.Parameters.Add(new SqlParameter("@UserName", InRow.UserName )) ;
				cmd.Parameters.Add(new SqlParameter("@EmailAddress", InRow.EmailAddress )) ;

				cmd.CommandText =
					"Insert " + TableName + " ( " +
					"UserId, Password, UserName, EmailAddress ) " +
					"Values( @UserId, @Password, @UserName, @EmailAddress )" ;
				cmd.ExecuteNonQuery();
			}
			finally
			{
				if ( conn != null )
					conn.Close( ) ;
			}
			return InRow ;
		}

		// ------------------------- UserMaster.Chain ----------------------------
		// chain to row by OnlineId key
		public virtual UserMasterRow Chain( Web.AcPage InPage, string InUserId )
		{
			UserMasterRow row = null ;
			SqlConnection conn = null ;
			SqlCommand cmd;
			SqlDataReader reader = null;
			bool rc;

			try
			{
				conn = InPage.GetDatabaseConnection( ) ;

				cmd = new SqlCommand();
				cmd.Connection = conn;
				cmd.CommandType = CommandType.Text;
				cmd.Parameters.Add(new SqlParameter("@UserId", InUserId ));
				cmd.CommandText =
					"SELECT *" +
					" FROM " + TableName +
					" WHERE UserId = @UserId" ;

				reader = cmd.ExecuteReader();
				rc = reader.Read();

				// no rows 
				if (rc == false)
				{
					throw (new UserMasterException( "UserId " + InUserId +
						" is not found in UserMaster table"));
				}

				// got a row. extract each column from the reader. 
				row = new UserMasterRow( ) ;
				row.UserId = reader.GetString(0);
				row.UserName = reader.GetString(1);
			}
			finally
			{
				if ( conn != null )
					conn.Close( ) ;
				if ( reader != null )
					reader.Close( ) ;
			}
			return row ;
		}

		// ---------------------UserMaster.CreateTable -------------------------
		// create the table in the database.
		public virtual void CreateTable( )
		{

			SqlConnection conn = null ;
			SqlCommand cmd;

			try
			{
				string ConnString = ConfigurationSettings.AppSettings["ConnectionString"] ;
				conn = new SqlConnection( ConnString ) ;
				conn.Open( ) ;
				try
				{
					cmd = new SqlCommand(
						"DROP TABLE " + TableName,
						conn);
					cmd.ExecuteNonQuery();
				}
				catch (SqlException)
				{
				}

				cmd = new SqlCommand(
					"CREATE TABLE " + TableName + " ( " +
					"UserId varchar(50), " +
					"UserName varchar(50), " +
					"Password varchar(20) " +
					"EmailAddress varchar(128) " +
					"PRIMARY KEY( UserId ))",
					conn ) ;

				cmd.ExecuteNonQuery();
			}
			finally
			{
				if ( conn != null )
					conn.Close( ) ;
			}
		}

		// -------------------------- UpdateUserName --------------------------
		// change the UserName in the UserMaster table row
		public virtual void UpdateUserName(
			Web.AcPage InPage, string InUserId, string InUserName )
		{
			SqlConnection conn = null ;
			SqlCommand cmd = new SqlCommand( ) ;
			try
			{
				conn = InPage.GetDatabaseConnection( ) ;
				cmd.Connection = conn ;
				cmd.CommandType = CommandType.Text;
				cmd.Parameters.Add(new SqlParameter("@UserId", InUserId )) ;
				cmd.Parameters.Add(new SqlParameter("@UserName", InUserName ));
				cmd.CommandText =
					"UPDATE " + TableName +
					" SET UserName = @UserName " +
					"WHERE UserId = @UserId" ;
				cmd.ExecuteNonQuery( ) ;
			}
			finally
			{
				if ( conn != null )
					conn.Close( ) ;
			}
		}

	} // end class UserMaster
} // end namespace AutoCoder.WebTables
