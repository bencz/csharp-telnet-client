using System;
using System.Data ;
using System.Data.SqlClient ;

namespace AutoCoder
{
	// ---------------------------- ArticleMaster -------------------------
	public class ArticleMaster
	{
		private string mArticleName ;
		private string mArticleTitle ;
		private string mSummaryDocFile ;
		private string mFullDocFile ;

		public ArticleMaster()
		{
		}

		// --------------------------- ArticleMaster properties  ------------
		public string ArticleName
		{
			get { return mArticleName ; }
			set { mArticleName = value ; }
		}
		public string ArticleTitle
		{
			get { return mArticleTitle ; }
			set { mArticleTitle = value ; }
		}
		public string SummaryDocFile
		{
			get { return mSummaryDocFile ; }
			set { mSummaryDocFile = value ; }
		}
		public string FullDocFile
		{
			get { return mFullDocFile ; }
			set { mFullDocFile = value ; }
		}

		// ---------------------------- ArticleMaster.Add ------------------------
		// assign ArticleName and run sql insert for the fields stored in the class. 
		public void Add( AutoCoder.BasePage InPage )
		{
			SqlConnection conn = null;
			try
			{
				SqlCommand cmd = new SqlCommand();
				conn = new SqlConnection(Common.GetConnectionString( InPage ));
				conn.Open();

				cmd.Connection = conn;
				cmd.CommandType = CommandType.Text;
				cmd.Parameters.Add(new SqlParameter("@ArticleName", mArticleName));
				cmd.Parameters.Add(new SqlParameter("@ArticleTitle", mArticleTitle));
				cmd.Parameters.Add(new SqlParameter("@SummaryDocFile", mSummaryDocFile));
				cmd.Parameters.Add(new SqlParameter("@FullDocFile", mFullDocFile ));

				cmd.CommandText = "Insert ARTICLEMASTER ( " +
					"ArticleName, ArticleTitle, SummaryDocFile, FullDocFile ) " +
					"Values( @ArticleName, @ArticleTitle, @SummaryDocFile, @FullDocFile )";
				cmd.ExecuteNonQuery();
			}
			finally
			{
				if (conn != null)
					conn.Close();
			}
		}

		// ------------------------ ArticleMaster.BuildChainCommand ------------------------
		public static SqlCommand BuildChainCommand(SqlConnection InConn, string InArticleName )
		{
			SqlCommand cmd;
			cmd = new SqlCommand();
			cmd.Connection = InConn;
			cmd.CommandType = CommandType.Text;
			cmd.Parameters.Add(new SqlParameter("@ArticleName", InArticleName));
			cmd.CommandText = "SELECT * from ArticleMaster WHERE ArticleName=@ArticleName";
			return cmd;
		}

		// --------------------- ArticleMaster.Chain ------------------------------
		public void Chain( AutoCoder.BasePage InPage, string InArticleName )
		{
			SqlConnection conn = null;
			SqlCommand cmd = null ;
			SqlDataReader reader = null;
			bool rc;

			try
			{
				conn = new SqlConnection(Common.GetConnectionString( InPage ));
				conn.Open();
				cmd = BuildChainCommand( conn, InArticleName ) ;
				reader = cmd.ExecuteReader();
				rc = reader.Read();

				// not found exception. 
				if (rc == false)
				{
					string msg = "Article master not found for ArticleName " + InArticleName ;
					throw( new ArticleMasterException( msg )) ;
				}

				// got a row. extract each column from the reader. 
				mArticleName = reader.GetString(0) ;
				mArticleTitle = reader.GetString(1) ;
				mSummaryDocFile = reader.GetString(2) ;
				mFullDocFile = reader.GetString(3);
			}

			finally
			{
				if ( reader != null )
					reader.Close( ) ;
				if ( conn != null )
					conn.Close( ) ;
			}
		}

		// --------------------- ArticleMaster.CheckTable -------------------------
		// check for table existance.  return true if table exists.
		public static bool CheckTable( AutoCoder.BasePage InPage )
		{
			SqlCommand cmd = null ;
			SqlConnection conn = null;
			SqlDataReader reader = null;
			bool doesExist = false;
			bool rc;

			try
			{
				conn = Common.OpenConnection( InPage );
				cmd = new SqlCommand();
				cmd.Connection = conn;
				cmd.CommandType = CommandType.Text;
				cmd.CommandText = "select * from INFORMATION_SCHEMA.tables where table_name = " +
													"@TableName" ;
				cmd.Parameters.Add(new SqlParameter("@TableName", "ARTICLEMASTER" ));
				reader = cmd.ExecuteReader();
				rc = reader.Read();

				// not found exception. 
				if (rc == false)
					doesExist = false ;
				else
					doesExist = true ;
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

		// --------------------- ArticleMaster.CreateTable -------------------------
		// create the table in the database.
		public static void CreateTable( AutoCoder.BasePage InPage )
		{
			SqlConnection conn;
			SqlCommand cmd;

			conn = new SqlConnection( Common.GetConnectionString( InPage )) ;
			conn.Open();

			try
			{
				cmd = new SqlCommand("DROP TABLE ARTICLEMASTER", conn);
				cmd.ExecuteNonQuery();
			}
			catch
			{
			}

			cmd = new SqlCommand("CREATE TABLE ARTICLEMASTER ( " +
				"ArticleName varchar(50), " +
				"ArticleTitle varchar(100), " +
				"SummaryDocFile varchar(50), " +
				"FullDocFile varchar(50) " +
				"PRIMARY KEY( ArticleName ))",
				conn);
			cmd.ExecuteNonQuery();

			conn.Close();
		}
	} // end class ArticleMaster

	// ---------------------- AutoCoder.ArticleMasterException ---------------------
	public class ArticleMasterException : ApplicationException
	{
		public ArticleMasterException( string InMessage, Exception InInnerException )
			: base( InMessage, InInnerException )
		{
		}
		public ArticleMasterException( string InMessage )
			: base( InMessage )
		{
		}
	} // end class ArticleMasterException
	
} // end namespace AutoCoder 

