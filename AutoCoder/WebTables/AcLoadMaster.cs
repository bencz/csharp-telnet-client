using System;
using System.Configuration;
using System.Data ;
using System.Data.SqlClient ;
using AutoCoder.Web ;
using AutoCoder.Text ;

namespace AutoCoder.WebTables
{

	// ------------------------- AcLoadMasterRow ---------------------------
	public class AcLoadMasterRow
	{
		private string mLoadId ;
		private string mSessionId ;
		private int mSeqn ;		// page seqnbr within session.
		private string mPageName ;
		private string mPageFileName ;
		private string mPageDirPath ;
		private DateTime mLoadTs ;
		private DateTime mLastPostbackTs ;
		private int mPostbackCx ;
		private string mReferrerLoadId ;
		private string mRawUrl ;

		public AcLoadMasterRow( )
		{
		}

		public string LoadId
		{
			get { return mLoadId ; }
			set { mLoadId = value ; }
		}
		public string SessionId
		{
			get { return mSessionId ; }
			set { mSessionId = value ; }
		}
		public int Seqn
		{
			get { return mSeqn ; }
			set { mSeqn = value ; }
		}
		public string PageName
		{
			get { return Stringer.GetNonNull ( mPageName ) ; }
			set { mPageName = value ; }
		}
		public string PageFileName
		{
			get { return Stringer.GetNonNull( mPageFileName ) ; }
			set { mPageFileName = value ; }
		}
		public string PageDirPath
		{
			get { return Stringer.GetNonNull( mPageDirPath ) ; }
			set { mPageDirPath = value ; }
		}
		public DateTime LoadTs
		{
			get { return mLoadTs ; }
			set { mLoadTs = value ; }
		}
		public DateTime LastPostbackTs
		{
			get { return mLastPostbackTs ; }
			set { mLastPostbackTs = value ; }
		}
		public int PostbackCx
		{
			get { return mPostbackCx ; }
			set { mPostbackCx = value ; }
		}
		public string ReferrerLoadId
		{
			get { return Stringer.GetNonNull( mReferrerLoadId ) ; }
			set { mReferrerLoadId = value ; }
		}
		public string RawUrl
		{
			get { return Stringer.GetNonNull( mRawUrl ) ; }
			set { mRawUrl = value ; }
		}

		public void Fill( AcLoadMasterRow InRow )
		{
			mLoadId = InRow.mLoadId ;
			mSessionId = InRow.mSessionId ;
			mSeqn = InRow.mSeqn ;
			mPageName = InRow.mPageName ;
			mPageFileName = InRow.mPageFileName ;
			mPageDirPath = InRow.mPageDirPath ;
			mLoadTs = InRow.mLoadTs ;
			mLastPostbackTs = InRow.mLastPostbackTs ;
			mPostbackCx = InRow.mPostbackCx ;
			mReferrerLoadId = InRow.mReferrerLoadId ;
			mRawUrl = InRow.mRawUrl ;
		}
	}

	// -------------------------- LoadMasterException ---------------------
	public class LoadMasterException : ApplicationException
	{

		public LoadMasterException( string InMessage )
			: base( InMessage )
		{
		}
		public LoadMasterException( string InMessage, Exception InInnerException )
			: base( InMessage, InInnerException )
		{
		}
	}

	// --------------------------- AcLoadMaster --------------------------
	public class AcLoadMaster
	{
		public AcLoadMaster( )
		{
		}

		// all derived classes should override this virtual property and return the
		// actual application specific name of the AcLoadMaster table. 
		public virtual string TableName
		{
			get
			{
				throw( new ApplicationException(
					"derived class must override the TableName property" )) ;
			}
		}

		// ------------------------- CalcRandomLoadId ----------------------------
		private static string CalcRandomLoadId( )
		{
			int limit = 89998;
			int nLoadId ;
			Random r = new Random();
			nLoadId = r.Next(limit) + 1 ;
			nLoadId += 10000 ;
			return nLoadId.ToString( ) ;
		}

		// -------------------------- Add --------------------------------
		public virtual AcLoadMasterRow Add(
			AcPage InPage,
			AcLoadMasterRow InRow )
		{
			SqlConnection conn = null ;

			// assign a LoadId
			if ( InRow.LoadId == null )
				InRow.LoadId = System.Guid.NewGuid( ).ToString( ) ;

			try
			{
				conn = InPage.GetDatabaseConnection( ) ;
				SqlCommand cmd = new SqlCommand();

				cmd.Connection = conn ;
				cmd.CommandType = CommandType.Text;
				cmd.Parameters.Add(new SqlParameter("@LoadId", InRow.LoadId )) ;
				cmd.Parameters.Add(new SqlParameter("@SessionId", InRow.SessionId )) ;
				cmd.Parameters.Add(new SqlParameter("@Seqn", InRow.Seqn )) ;
				cmd.Parameters.Add(new SqlParameter("@PageName", InRow.PageName )) ;
				cmd.Parameters.Add(new SqlParameter("@PageFileName", InRow.PageFileName )) ;
				cmd.Parameters.Add(new SqlParameter("@PageDirPath", InRow.PageDirPath )) ;
				cmd.Parameters.Add(new SqlParameter("@LoadTs", InRow.LoadTs )) ;
				cmd.Parameters.Add(new SqlParameter("@LastPostbackTs", InRow.LastPostbackTs )) ;
				cmd.Parameters.Add(new SqlParameter("@PostbackCx", InRow.PostbackCx )) ;
				cmd.Parameters.Add(new SqlParameter("@ReferrerLoadId", InRow.ReferrerLoadId )) ;
				cmd.Parameters.Add(new SqlParameter("@RawUrl", InRow.RawUrl )) ;

				cmd.CommandText =
					"INSERT " + TableName + " ( " +
					"LoadId, SessionId, Seqn, PageName, PageFileName, PageDirPath, " +
					"LoadTs, LastPostbackTs, PostbackCx, ReferrerLoadId, RawUrl ) " +
					"Values( @LoadId, @SessionId, @Seqn, @PageName, @PageFileName, @PageDirPath, " +
					"@LoadTs, @LastPostbackTs, @PostbackCx, @ReferrerLoadId, @RawUrl )" ;
				cmd.ExecuteNonQuery();
			}
			finally
			{
				if ( conn != null )
					conn.Close( ) ;
			}
			return InRow ;
		}

		// ------------------------- AcLoadMaster.Chain ----------------------------
		// chain to row by OnlineId key
		public virtual AcLoadMasterRow Chain( Web.AcPage InPage, string InLoadId )
		{
			AcLoadMasterRow row = null ;
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
				cmd.Parameters.Add(new SqlParameter("@LoadId", InLoadId ));
				cmd.CommandText =
					"SELECT *" +
					" FROM " + TableName +
					" WHERE LoadId = @LoadId" ;

				reader = cmd.ExecuteReader();
				rc = reader.Read();

				// no rows 
				if (rc == false)
				{
					throw (new LoadMasterException( "LoadId " + InLoadId +
						" is not found in AcLoadMaster table"));
				}

				// got a row. extract each column from the reader. 
				row = NewRow( ) ;
				row.LoadId = reader.GetString(0) ;
				row.SessionId = reader.GetString(1) ;
				row.Seqn = reader.GetInt32(2) ;
				row.PageName = reader.GetString(3) ;
				row.PageFileName = reader.GetString(4) ;
				row.PageDirPath = reader.GetString(5) ;
				row.LoadTs = reader.GetDateTime(6) ;
				row.LastPostbackTs = reader.GetDateTime(7) ;
				row.PostbackCx = reader.GetInt32(8) ;
				row.ReferrerLoadId = reader.GetString(9) ;
				row.RawUrl = reader.GetString(10) ;
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

		// ---------------------AcLoadMaster.CreateTable -------------------------
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
					"LoadId varchar(40), " +
					"SessionId varchar(40), " +
					"Seqn INT, " +
					"PageName varchar(20), " +
					"PageFileName varchar(20), " +
					"PageDirPath varchar(40), " +
					"LoadTs DATETIME, " +
					"LastPostbackTs DATETIME, " +
					"PostbackCx INT, " +
					"ReferrerLoadId varchar(40), " +
					"RawUrl varchar(512) " +
					"PRIMARY KEY( LoadId ))",
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
		// delete Load master by LoadId
		public virtual void Delete(
			AcPage InPage, string InLoadId )
		{
			SqlConnection conn = null ;
			SqlCommand cmd;

			try
			{
				conn = InPage.GetDatabaseConnection( ) ;

				cmd = new SqlCommand();
				cmd.Connection = conn;
				cmd.CommandType = CommandType.Text;
				cmd.Parameters.Add(new SqlParameter("@LoadId", InLoadId ));
				cmd.CommandText =
					"DELETE " + TableName +
					" WHERE LoadId = @LoadId" ;

				cmd.ExecuteNonQuery( ) ;
			}
			finally
			{
				if ( conn != null )
					conn.Close( ) ;
			}
		}

		// ----------------------- NewRow ------------------------
		public virtual AcLoadMasterRow NewRow( )
		{
			return ( new AcLoadMasterRow( )) ;
		}

		// ------------------------ NewRowCopy ----------------------------------
		public virtual AcLoadMasterRow NewRowCopy( AcLoadMasterRow InRow )
		{
			AcLoadMasterRow row = NewRow( ) ;
			row.Fill( InRow ) ;
			return row ;
		}

		// -------------------------- Update --------------------------
		public virtual void Update(
			AcPage InPage,
			string InLoadId,
			AcLoadMasterRow InNewRow )
		{
			SqlConnection conn = null ;
			SqlCommand cmd = new SqlCommand( ) ;
			try
			{
				conn = InPage.GetDatabaseConnection( ) ;
				cmd.Connection = conn ;
				cmd.CommandType = CommandType.Text;
				cmd.Parameters.Add(new SqlParameter("@WhereLoadId", InLoadId )) ;
				cmd.Parameters.Add(new SqlParameter("@LoadId", InNewRow.LoadId )) ;
				cmd.Parameters.Add(new SqlParameter("@SessionId", InNewRow.SessionId )) ;
				cmd.Parameters.Add(new SqlParameter("@Seqn", InNewRow.Seqn )) ;
				cmd.Parameters.Add(new SqlParameter("@PageName", InNewRow.PageName )) ;
				cmd.Parameters.Add(new SqlParameter("@PageFileName", InNewRow.PageFileName )) ;
				cmd.Parameters.Add(new SqlParameter("@PageDirPath", InNewRow.PageDirPath )) ;
				cmd.Parameters.Add(new SqlParameter("@LoadTs", InNewRow.LoadTs )) ;
				cmd.Parameters.Add(new SqlParameter("@LastPostbackTs", InNewRow.LastPostbackTs )) ;
				cmd.Parameters.Add(new SqlParameter("@PostbackCx", InNewRow.PostbackCx )) ;
				cmd.Parameters.Add(new SqlParameter("@ReferrerLoadId", InNewRow.ReferrerLoadId )) ;
				cmd.Parameters.Add(new SqlParameter("@RawUrl", InNewRow.RawUrl )) ;


				cmd.CommandText =
					"UPDATE " + TableName +
					" SET LoadId = @LoadId, " +
					"SessionId = @SessionId, " +
					"Seqn = @Seqn, " +
					"PageName = @PageName, " +
					"PageFileName = @PageFileName, " +
					"PageDirPath = @PageDirPath, " +
					"LoadTs = @LoadTs, " +
					"LastPostbackTs = @LastPostbackTs, " +
					"PostbackCx = @PostbackCx, " +
					"ReferrerLoadId = @ReferrerLoadId, " +
					"RawUrl = @RawUrl" +
					" WHERE LoadId = @WhereLoadId" ;
				cmd.ExecuteNonQuery( ) ;
			}
			finally
			{
				if ( conn != null )
					conn.Close( ) ;
			}
		}

	} // end class AcLoadMaster
} // end namespace AutoCoder.WebTables

