using System;
using System.Data ;
using System.Data.SqlClient ;

namespace AutoCoder
{
	// ---------------------------- LoadMaster -------------------------
	public class LoadMaster
	{
		private string mLoadId ;
		private string mRawUrl ;
		private string mPageTitle ;
		private DateTime mEntryTs ;

		public LoadMaster()
		{
		}

		// --------------------------- LoadMaster properties  ------------
		public string LoadId
		{
			get { return mLoadId; }
		}
		public string RawUrl
		{
			get { return mRawUrl ; }
			set { mRawUrl = value; }
		}
		public string PageTitle
		{
			get { return mPageTitle ; }
			set { mPageTitle = value; }
		}
		public DateTime EntryTs
		{
			get { return mEntryTs ; }
		}

		// ---------------------------- LoadMaster.Add ------------------------
		// assign LoadId and run sql insert for the fields stored in the class. 
		public void Add( AutoCoder.BasePage InPage )
		{
			SqlConnection conn = null;
			try
			{
				SqlCommand cmd = new SqlCommand();
				conn = new SqlConnection(Common.GetConnectionString( InPage ));
				conn.Open();

				// assign the LoadId.
				System.Guid guid;
				guid = System.Guid.NewGuid();
				mLoadId = guid.ToString();
				mEntryTs = DateTime.Now;

				cmd.Connection = conn;
				cmd.CommandType = CommandType.Text;
				cmd.Parameters.Add(new SqlParameter("@LoadId", mLoadId));
				cmd.Parameters.Add(new SqlParameter("@RawUrl", mRawUrl));
				cmd.Parameters.Add(new SqlParameter("@PageTitle", mPageTitle));
				cmd.Parameters.Add(new SqlParameter("@EntryTs", mEntryTs));

				cmd.CommandText = "Insert LoadMaster ( " +
					"LoadId, RawUrl, PageTitle, EntryTs ) " +
					"Values( @LoadId, @RawUrl, @PageTitle, @EntryTs )";
				cmd.ExecuteNonQuery();
			}
			finally
			{
				if (conn != null)
					conn.Close();
			}
		}

		// ------------------------ LoadMaster.BuildChainCommand ------------------------
		public static SqlCommand BuildChainCommand(SqlConnection InConn, string InLoadId)
		{
			SqlCommand cmd;
			cmd = new SqlCommand();
			cmd.Connection = InConn;
			cmd.CommandType = CommandType.Text;
			cmd.Parameters.Add(new SqlParameter("@LoadId", InLoadId));
			cmd.CommandText = "SELECT * from LoadMaster WHERE LoadId=@LoadId";
			return cmd;
		}

		// --------------------- LoadMaster.Chain ------------------------------
		public void Chain( AutoCoder.BasePage InPage, string InLoadId )
		{
			SqlConnection conn = null;
			SqlCommand cmd = null ;
			SqlDataReader reader = null;
			bool rc;

			try
			{
				conn = new SqlConnection(Common.GetConnectionString( InPage ));
				conn.Open();
				cmd = BuildChainCommand( conn, InLoadId ) ;
				reader = cmd.ExecuteReader();
				rc = reader.Read();

				// not found exception. 
				if (rc == false)
				{
					string msg = "Load master not found for LoadId " + InLoadId ;
					throw( new ApplicationException( msg )) ;
				}

				// got a row. extract each column from the reader. 
				mLoadId = reader.GetString(0) ;
				mRawUrl = reader.GetString(1) ;
				mPageTitle = reader.GetString(2) ;
				mEntryTs = reader.GetDateTime(3);
			}

			finally
			{
				if ( reader != null )
					reader.Close( ) ;
				if ( conn != null )
					conn.Close( ) ;
			}
		}

		// --------------------- LoadMaster.CheckTable -------------------------
		// check for table existance.  return true if table exists.
		public static bool CheckTable( AutoCoder.BasePage InPage )
		{
			SqlCommand cmd = null ;
			SqlConnection conn = null;
			SqlDataReader reader = null;
			bool doesExist = false;

			try
			{
				conn = Common.OpenConnection( InPage );
				cmd = new SqlCommand();
				cmd.Connection = conn;
				cmd.CommandType = CommandType.Text;
				cmd.Parameters.Add(new SqlParameter("@LoadId", "abc" ));
				cmd.CommandText = "SELECT * from LoadMaster WHERE LoadId=@LoadId";
				reader = cmd.ExecuteReader();
				doesExist = true;

			}
			catch (SqlException )
			{
				doesExist = false;
			}
			finally
			{
				if (reader != null)
					reader.Close();
				if (conn != null)
					conn.Close();
			}
			return doesExist;
		}

		// --------------------- LoadMaster.CreateTable -------------------------
		// create the table in the database.
		public static void CreateTable( AutoCoder.BasePage InPage )
		{
			SqlConnection conn;
			SqlCommand cmd;

			conn = new SqlConnection( Common.GetConnectionString( InPage )) ;
			conn.Open();

			try
			{
				cmd = new SqlCommand("DROP TABLE LOADMASTER", conn);
				cmd.ExecuteNonQuery();
			}
			catch
			{
			}

			cmd = new SqlCommand("CREATE TABLE LOADMASTER ( " +
				"LoadId varchar(50), " +
				"RawUrl varchar(1000), " +
				"PageTitle varchar(80), " +
				"EntryTs DATETIME, " +
				"PRIMARY KEY( LoadId ))",
				conn);
			cmd.ExecuteNonQuery();

			conn.Close();
		}

		// ------------------------ LoadMaster.NewPage --------------------------
		public static string NewPage( AutoCoder.BasePage InPage, string InPageTitle )
		{
			LoadMaster lm = new LoadMaster( ) ;
			lm.RawUrl = InPage.Request.RawUrl ;
			lm.PageTitle = InPageTitle ;
			try
			{
				lm.Add( InPage ) ;
			}

				// catch the add exception. ( probably the table does not exist. ) If the LoadMaster
				// table does not exist, try to create it, then try the add again.
			catch( SqlException excp )
			{
				if ( AutoCoder.Common.IsObjectNotFoundExcp( excp ) == false )
					throw( excp ) ;
				LoadMaster.CreateTable( InPage ) ;
				lm.Add( InPage ) ;
			}
			return lm.LoadId ;
		}

	} // end class LoadMaster

	// ---------------------- AutoCoder.LoadMasterException ---------------------
	public class LoadMasterException : ApplicationException
	{
		public LoadMasterException( string InMessage, Exception InInnerException )
			: base( InMessage, InInnerException )
		{
		}
	} // end class LoadMasterException
	
} // end namespace AutoCoder 
