using System;
using System.Configuration;
using System.Data ;
using System.Data.SqlClient ;
using AutoCoder.Web ;
using AutoCoder.Text ;

namespace AutoCoder.WebTables
{

	// ------------------------- AcAccountMasterRow ---------------------------
	public class AcAccountMasterRow
	{
		private string mAccountNbr ;
		private string mAccountName ;
		private string mPassword ;
		private string mConfirmPassword ;
		private string mEmailAddress ;
		private DateTime mCreateTs ;		// account create date/time

		public AcAccountMasterRow( )
		{
		}

		public string AccountNbr
		{
			get { return mAccountNbr ; }
			set { mAccountNbr = value ; }
		}
		public string AccountName
		{
			get { return mAccountName ; }
			set { mAccountName = value ; }
		}
		public string ConfirmPassword
		{
			get { return Stringer.GetNonNull( mConfirmPassword ) ; }
			set { mConfirmPassword = value ; }
		}
		public DateTime CreateTs
		{
			get { return mCreateTs ; }
			set { mCreateTs = value ; }
		}
		public string Password
		{
			get { return Stringer.GetNonNull( mPassword ) ; }
			set { mPassword = value ; }
		}
		public string EmailAddress
		{
			get { return mEmailAddress ; }
			set { mEmailAddress = value ; }
		}

		public void Fill( AcAccountMasterRow InRow )
		{
			mAccountNbr = InRow.mAccountNbr ;
			mAccountName = InRow.mAccountName ;
			mPassword = InRow.mPassword ;
			mConfirmPassword = InRow.mConfirmPassword ;
			mEmailAddress = InRow.mEmailAddress ;
			mCreateTs = InRow.mCreateTs ;
		}
	}

	// -------------------------- AccountMasterException ---------------------
	public class AccountMasterException : ApplicationException
	{

		public AccountMasterException( string InMessage )
			: base( InMessage )
		{
		}
		public AccountMasterException( string InMessage, Exception InInnerException )
			: base( InMessage, InInnerException )
		{
		}
	}

	// -------------------------- LoginException ---------------------
	public class LoginException : ApplicationException
	{

		public LoginException( string InMessage )
			: base( InMessage )
		{
		}
	}

	// --------------------------- AcAccountMaster --------------------------
	public class AcAccountMaster
	{
		public AcAccountMaster( )
		{
		}

		// all derived classes should override this virtual property and return the
		// actual application specific name of the AcAccountMaster table. 
		public virtual string TableName
		{
			get
			{
				throw( new ApplicationException(
					"derived class must override the TableName property" )) ;
			}
		}

		// ------------------------- CalcRandomAccountNbr ----------------------------
		private static string CalcRandomAccountNbr( )
		{
			int limit = 89998;
			int nAccountNbr ;
			Random r = new Random();
			nAccountNbr = r.Next(limit) + 1 ;
			nAccountNbr += 10000 ;
			return nAccountNbr.ToString( ) ;
		}

		// -------------------------- Add --------------------------------
		public virtual AcAccountMasterRow Add(
			AcPage InPage,
			AcAccountMasterRow InRow )
		{
			SqlConnection conn = null ;

			ValidityCheck( InRow ) ;

			try
			{
				conn = InPage.GetDatabaseConnection( ) ;
				SqlCommand cmd = new SqlCommand();

				cmd.Connection = conn ;
				cmd.CommandType = CommandType.Text;
				cmd.Parameters.Add(new SqlParameter("@AccountNbr", InRow.AccountNbr )) ;
				cmd.Parameters.Add(new SqlParameter("@AccountName", InRow.AccountName )) ;
				cmd.Parameters.Add(new SqlParameter("@Password", InRow.Password )) ;
				cmd.Parameters.Add(new SqlParameter("@EmailAddress", InRow.EmailAddress )) ;
				cmd.Parameters.Add(new SqlParameter("@CreateTs", InRow.CreateTs )) ;

				cmd.CommandText =
					"INSERT " + TableName + " ( " +
					"AccountNbr, AccountName, Password, EmailAddress, CreateTs ) " +
					"Values( @AccountNbr, @AccountName, @Password, @EmailAddress, @CreateTs )" ;
				cmd.ExecuteNonQuery();
			}
			finally
			{
				if ( conn != null )
					conn.Close( ) ;
			}
			return InRow ;
		}

		// ----------------------- AddNewAccount ---------------------------
		public virtual AcAccountMasterRow AddNewAccount(
			AcPage InPage,
			AcAccountMasterRow InRow )
		{
			AcAccountMasterRow AddRow = null ;

			// error check.  ConfirmPassword and Password must match.
			if ( InRow.ConfirmPassword != InRow.Password )
				throw( new AccountMasterException( "Confirm password mismatch" )) ;

			// do the insert until a unique AccountNbr is assigned.
			while( true )
			{

				AddRow = NewRowCopy( InRow ) ;
				AddRow.AccountNbr = CalcRandomAccountNbr( ) ;
				AddRow.CreateTs = DateTime.Now ;

				try
				{
					Add( InPage, AddRow ) ;
				}
				catch( SqlException excp )
				{
					if ( AutoCoder.Common.IsDuplicateKeyExcp( excp ) == true )
						continue ;
					throw( excp ) ;
				}
				break ;
			} // end do while duplicate insert key
			return AddRow ;
		}

		// ------------------------- AcAccountMaster.Chain ----------------------------
		// chain to row by OnlineId key
		public virtual AcAccountMasterRow Chain( Web.AcPage InPage, string InAccountNbr )
		{
			AcAccountMasterRow row = null ;
			SqlConnection conn = null ;
			SqlCommand cmd;
			SqlDataReader reader = null;
			bool rc;

			try
			{
				conn = InPage.GetDatabaseConnection( ) ;

				cmd = new SqlCommand( ) ;
				cmd.Connection = conn;
				cmd.CommandType = CommandType.Text;
				cmd.Parameters.Add(new SqlParameter("@AccountNbr", InAccountNbr ));
				cmd.CommandText =
					"SELECT *" +
					" FROM " + TableName +
					" WHERE AccountNbr = @AccountNbr" ;

				reader = cmd.ExecuteReader();
				rc = reader.Read();

				// no rows 
				if (rc == false)
				{
					throw (new AccountMasterException( "AccountNbr " + InAccountNbr +
						" is not found in AcAccountMaster table"));
				}

				// got a row. extract each column from the reader. 
				row = NewRow( ) ;
				row.AccountNbr = reader.GetString(0);
				row.AccountName = reader.GetString(1);
				row.Password = reader.GetString(2);
				row.EmailAddress = reader.GetString(3);
				row.CreateTs = reader.GetDateTime(4) ;
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

		// ---------------------AcAccountMaster.CreateTable -------------------------
		// create the table in the database.
		public virtual void CreateTable( string InConnString )
		{

			SqlConnection conn = null ;
			SqlCommand cmd;

			try
			{
				conn = new SqlConnection( InConnString ) ;
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
					"AccountNbr varchar(10), " +
					"AccountName varchar(50), " +
					"Password varchar(10), " +
					"EmailAddress varchar(128), " +
					"CreateTs DATETIME " +
					"PRIMARY KEY( AccountNbr ))",
					conn ) ;

				cmd.ExecuteNonQuery();
			}
			finally
			{
				if ( conn != null )
					conn.Close( ) ;
			}
		}

		// ------------------------- Delete ----------------------------
		// delete account master by AccountNbr
		public virtual void Delete(
			AcPage InPage, string InAccountNbr )
		{
			SqlConnection conn = null ;
			SqlCommand cmd;

			try
			{
				conn = InPage.GetDatabaseConnection( ) ;

				cmd = new SqlCommand();
				cmd.Connection = conn;
				cmd.CommandType = CommandType.Text;
				cmd.Parameters.Add(new SqlParameter("@AccountNbr", InAccountNbr ));
				cmd.CommandText =
					"DELETE " + TableName +
					" WHERE AccountNbr = @AccountNbr" ;

				cmd.ExecuteNonQuery( ) ;
			}
			finally
			{
				if ( conn != null )
					conn.Close( ) ;
			}
		}

		// ------------------------- GetAccounts ------------------
		// return DataSet containing all the accounts. 
		public virtual DataSet GetAccounts( AcPage InPage )
		{
			SqlDataAdapter da = null ;
			SqlCommand cmd = null ;
			SqlConnection conn = null ;
			DataSet accounts = null ;

			try
			{
				conn = InPage.GetDatabaseConnection( ) ;
				cmd = new SqlCommand();
				cmd.Connection = conn ;
				cmd.CommandType = CommandType.Text;

				cmd.CommandText =
					"SELECT * " +
					" FROM " + TableName +
					" ORDER BY AccountNbr" ;

				da = new SqlDataAdapter( ) ;
				da.SelectCommand = cmd ;

				accounts = new DataSet();
				da.Fill( accounts, "Accounts" ) ;
			}
			finally
			{
				if ( conn != null )
					conn.Close( ) ;
			}

			return accounts ;
		}

		// ----------------------- NewRow ------------------------
		public virtual AcAccountMasterRow NewRow( )
		{
			return ( new AcAccountMasterRow( )) ;
		}

		// ------------------------ NewRowCopy ----------------------------------
		public virtual AcAccountMasterRow NewRowCopy( AcAccountMasterRow InRow )
		{
			AcAccountMasterRow row = NewRow( ) ;
			row.Fill( InRow ) ;
			return row ;
		}

		// -------------------------- Update --------------------------
		public virtual void Update(
			AcPage InPage,
			string InAccountNbr,
			AcAccountMasterRow InNewRow )
		{
			SqlConnection conn = null ;
			SqlCommand cmd = new SqlCommand( ) ;
			try
			{
				conn = InPage.GetDatabaseConnection( ) ;
				cmd.Connection = conn ;
				cmd.CommandType = CommandType.Text;
				cmd.Parameters.Add(new SqlParameter("@WhereAccountNbr", InAccountNbr )) ;
				cmd.Parameters.Add(new SqlParameter("@AccountNbr", InNewRow.AccountNbr )) ;
				cmd.Parameters.Add(new SqlParameter("@AccountName", InNewRow.AccountName )) ;
				cmd.Parameters.Add(new SqlParameter("@Password", InNewRow.Password )) ;
				cmd.Parameters.Add(new SqlParameter("@EmailAddress", InNewRow.EmailAddress )) ;
				cmd.Parameters.Add(new SqlParameter("@CreateTs", InNewRow.CreateTs )) ;

				cmd.CommandText =
					"UPDATE " + TableName +
					"SET AccountNbr = @AccountNbr, " +
					"AccountName = @AccountName, " +
					"Password = @Password, " +
					"EmailAddress = @EmailAddress, " +
					"CreateTs = @CreateTs " +
					"WHERE AccountNbr = @WhereAccountNbr" ;
				cmd.ExecuteNonQuery( ) ;
			}
			finally
			{
				if ( conn != null )
					conn.Close( ) ;
			}
		}

		// ------------------------- ValidateLogin ------------------------------
		public virtual AcAccountMasterRow ValidateLogin(
			AcPage InPage,
			string InAccountNbr,
			string InEmailAddress,
			string InPassword )
		{
			AcAccountMasterRow row = null ;
			try
			{
				row = Chain( InPage, InAccountNbr ) ;
			}
			catch( AccountMasterException )
			{
				throw( new LoginException( "Invalid account" )) ;
			}

			// verify the password.
			if ( row.Password != InPassword )
				throw( new LoginException( "Incorrect password" )) ;

			return row ;
		}

		// ------------------------ ValidityCheck --------------------------
		public virtual void ValidityCheck( AcAccountMasterRow InRow )
		{
			if ( Stringer.IsBlank( InRow.AccountName ))
				throw( new AccountMasterException( "Account name is blank" )) ;
			if ( Stringer.IsBlank( InRow.Password ))
				throw( new AccountMasterException( "Password is missing" )) ;
		}

	} // end class AcAccountMaster
} // end namespace AutoCoder.WebTables

