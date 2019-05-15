using System;
using System.Configuration;
using System.Data ;
using System.Data.SqlClient ;
using AutoCoder.Web ;

namespace AutoCoder.WebTables
{

	// ------------------------- AcVisitorMasterRow ---------------------------
	public class AcVisitorMasterRow
	{
		private string mVisitorId ;
		private string mUserId ;

		public AcVisitorMasterRow( )
		{
		}

		public string VisitorId
		{
			get { return mVisitorId ; }
			set { mVisitorId = value ; }
		}
		public string UserId
		{
			get { return mUserId ; }
			set { mUserId = value ; }
		}

		public bool IsAnonymous( )
		{
			if ( ( mUserId == null ) || ( mUserId == "" ))
				return true ;
			else
				return false ;
		}
	}

	// -------------------------- VisitorMasterException ---------------------
	public class VisitorMasterException : ApplicationException
	{

		public VisitorMasterException( string InMessage )
			: base( InMessage )
		{
		}
		public VisitorMasterException( string InMessage, Exception InInnerException )
			: base( InMessage, InInnerException )
		{
		}
	}

	// --------------------------- AcVisitorMaster --------------------------
	// the AcVisitorMaster table stores the linkage between a session guid identifier
	// ( AcVisitorMaster ) and the UserId of the successfully logged in customer.
	public class AcVisitorMaster
	{
		public AcVisitorMaster( )
		{
		}

		// all derived classes should override this virtual property and return the
		// actual application specific name of the AcVisitorMaster table. 
		public virtual string TableName
		{
			get
			{
				throw( new ApplicationException(
					"derived class must override the TableName property" )) ;
			}
		}

		// -------------------------- Add --------------------------------
		public virtual AcVisitorMasterRow Add(
			AcPage InPage,
			AcVisitorMasterRow InRow )
		{
			SqlConnection conn = null ;

			try
			{
				conn = InPage.GetDatabaseConnection( ) ;
				SqlCommand cmd = new SqlCommand();

				// assign the VisitorId
				if ( InRow.VisitorId == null )
					InRow.VisitorId = System.Guid.NewGuid( ).ToString( ) ;
				if ( InRow.UserId == null )
					InRow.UserId = "" ;

				cmd.Connection = conn ;
				cmd.CommandType = CommandType.Text;
				cmd.Parameters.Add(new SqlParameter("@VisitorId", InRow.VisitorId )) ;
				cmd.Parameters.Add(new SqlParameter("@UserId", InRow.UserId )) ;

				cmd.CommandText =
					"INSERT " + TableName + " ( " +
					"VisitorId, UserId ) " +
					"Values( @VisitorId, @UserId )" ;
				cmd.ExecuteNonQuery();
			}
			finally
			{
				if ( conn != null )
					conn.Close( ) ;
			}
			return InRow ;
		}

		// ----------------------- AcVisitorMaster.AddNewVisitor ---------------------------
		// create a new AcVisitorMaster row.
		public virtual AcVisitorMasterRow AddNewVisitor( Web.AcPage InPage, string InUserId )
		{
			SqlConnection conn = null ;
			SqlCommand cmd;
			
			// build the row.
			AcVisitorMasterRow row = NewVisitorMasterRow( ) ; 
			row.VisitorId = System.Guid.NewGuid( ).ToString( ) ;
			row.UserId = InUserId ;

			try
			{
				conn = InPage.GetDatabaseConnection( ) ;

				cmd = new SqlCommand( ) ;
				cmd.Connection = conn;
				cmd.CommandType = CommandType.Text;
				cmd.Parameters.Add(new SqlParameter("@VisitorId", row.VisitorId ));
				cmd.Parameters.Add(new SqlParameter("@UserId", row.UserId ));
				cmd.CommandText =
					"INSERT " + TableName +
					" ( VisitorId, UserId ) " +
					"VALUES( @VisitorId, @UserId )" ;
				cmd.ExecuteNonQuery();
			}
			finally
			{
				if ( conn != null )
					conn.Close();
			}

			return row ;
		}

		// ------------------------- AcVisitorMaster.Chain ----------------------------
		// chain to row by OnlineId key
		public virtual AcVisitorMasterRow Chain( Web.AcPage InPage, string InVisitorId )
		{
			AcVisitorMasterRow row = null ;
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
				cmd.Parameters.Add(new SqlParameter("@VisitorId", InVisitorId ));
				cmd.CommandText =
					"SELECT *" +
					" FROM " + TableName +
					" WHERE VisitorId = @VisitorId" ;

				reader = cmd.ExecuteReader();
				rc = reader.Read();

				// no rows 
				if (rc == false)
				{
					throw (new VisitorMasterException( "VisitorId " + InVisitorId +
						" is not found in AcVisitorMaster table"));
				}

				// got a row. extract each column from the reader. 
				row = NewVisitorMasterRow( ) ;
				row.VisitorId = reader.GetString(0);
				row.UserId = reader.GetString(1);
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

		// ---------------------AcVisitorMaster.CreateTable -------------------------
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
					"VisitorId varchar(50), " +
					"UserId varchar(50) " +
					"PRIMARY KEY( VisitorId ))",
					conn ) ;

				cmd.ExecuteNonQuery();
			}
			finally
			{
				if ( conn != null )
					conn.Close( ) ;
			}
		}

		// ----------------------- NewVisitorMasterRow ------------------------
		public virtual AcVisitorMasterRow NewVisitorMasterRow( )
		{
			return ( new AcVisitorMasterRow( )) ;
		}

		// -------------------------- UpdateUserId --------------------------
		// change the UserId in the AcVisitorMaster table row
		public virtual void UpdateUserId(
			Web.AcPage InPage, string InVisitorId, string InUserId )
		{
			SqlConnection conn = null ;
			SqlCommand cmd = new SqlCommand( ) ;
			try
			{
				conn = InPage.GetDatabaseConnection( ) ;
				cmd.Connection = conn ;
				cmd.CommandType = CommandType.Text;
				cmd.Parameters.Add(new SqlParameter("@VisitorId", InVisitorId )) ;
				cmd.Parameters.Add(new SqlParameter("@UserId", InUserId ));
				cmd.CommandText =
					"UPDATE " + TableName +
					" SET UserId = @UserId " +
					"WHERE VisitorId = @VisitorId" ;
				cmd.ExecuteNonQuery( ) ;
			}
			finally
			{
				if ( conn != null )
					conn.Close( ) ;
			}
		}

	} // end class AcVisitorMaster
} // end namespace AutoCoder.WebTables
