// AutoCoder.cs - 
using System ;
using System.Data ;
using System.Data.SqlClient ;
using System.Text ;
using System.Web;
using System.Web.SessionState;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

namespace AutoCoder
{

	public class BasePage : System.Web.UI.Page
	{
		private string mPageTitle ;
		private string mPageUrl ;		// relative url of this page. 
 
		// ----------------- BasePage.Constructor ---------------------------
		public BasePage( )
		{
		}

		// -------------------- BasePage.Properties --------------------------------
		public string PageTitle
		{
			get { return mPageTitle ; }
			set { mPageTitle = value ; }
		}
		public string PageUrl
		{
			get { return mPageUrl ; }
			set { mPageUrl = value ; }
		}
		public string PageLoadId
		{
			get { return ViewState["LoadId"].ToString( ) ; }
		}

		// get the LoadMaster of the referrer page. Could return null. 
		public LoadMaster PageReferrerLoadMaster
		{
			get
			{
				LoadMaster lm = null ;
				String refLoadId ;
				refLoadId = Request.QueryString["RefLoadId"];
				if ( refLoadId != null)
				{
					lm = new LoadMaster( ) ;
					lm.Chain( this, refLoadId ) ;
				}
				return lm ;
			}
		}

		// ------------------------ BasePage.PageLoad ------------------------
		public void PageLoad(object sender, System.EventArgs e)
		{
			if ( Page.IsPostBack == false )
			{
				string LoadId = LoadMaster.NewPage( this, PageTitle ) ;
				ViewState["LoadId"] = LoadId ;
				Trace.Write( "PageLoad. LoadId", ViewState["LoadId"].ToString( )) ;
			}
		}

		// ---------------------- BasePage.RedirectTo -------------------------
		// redirect to this page from another page.
		public void RedirectTo( AutoCoder.BasePage frompage )
		{
			StringBuilder url = new StringBuilder( ) ;
			url.Append( mPageUrl ) ;
			url.Append( "?RefLoadId=" ) ;
			url.Append( frompage.PageLoadId ) ;
			frompage.Response.Redirect( url.ToString( ) ) ;
		}

// --------------------- BasePage.ReturnLinkClickHandler ------------------
// return to referrer page link click handler.
		public void ReturnLinkClickHandler( System.Web.UI.WebControls.LinkButton InLink )
		{
			if ( InLink.Visible == true )
			{
				LoadMaster lm = PageReferrerLoadMaster ;
				if ( lm != null )
				{
					Response.Redirect( lm.RawUrl ) ;
				}
			}
		}

		// ----------------------- BasePage.ReturnLinkSetup -------------------- 
		// enable and setup the text of the return to referrer link.
		public void ReturnLinkSetup( System.Web.UI.WebControls.LinkButton InLink )
		{
			LoadMaster lm = PageReferrerLoadMaster ;
			if ( lm == null )
				InLink.Visible = false ;
			else
			{
				InLink.Visible = true ;
				InLink.Text = "back to " + lm.PageTitle ;
			}
		}
	} // end class BasePage

	// ------------------- AutoCoder.Common ------------------------    
	public class Common
	{

		// ---------------------------- Common.GetConnectionString -------------------
		public static string GetConnectionString( AutoCoder.BasePage InPage )
		{
			System.IO.StreamReader reader = null ;
			string connString = null ;
			string path = InPage.MapPath( "ConnectString.txt" ) ;
			try
			{
				try
				{
					reader = new System.IO.StreamReader( path ) ;
					connString = reader.ReadLine( ) ;
				}
				catch( System.IO.IOException excp )
				{
					throw( new ApplicationException( "GetConnectionString error. " +
											excp.Message )) ;
				}
			}
			finally
			{
				if ( reader != null )
					reader.Close( ) ;
			}
			return connString ;
		}

		// --------------- Common.IsObjectNotFoundExcp -------------------------
		// check the SqlException for error code = 208 ( Invalid object name )
		public static bool IsObjectNotFoundExcp( SqlException InExcp )
		{
			if (( InExcp.Errors.Count >= 1 ) && ( InExcp.Errors[0].Number == 208 ))
				return true ;
			else
				return false ;
		}

		// ----------------- Common.OpenConnection -----------------------------
		public static SqlConnection OpenConnection( AutoCoder.BasePage InPage )
		{
			SqlConnection conn;
			conn = new SqlConnection( GetConnectionString( InPage ));
			conn.Open( ) ;
			return conn ;
		}

		// -------------------- Common.PullQueryStringItemId ----------------------------
		// extract and return the ItemId value from the QueryString 
		public static int PullQueryStringItemId( System.Web.HttpRequest rqs )
		{
			String itemidx ;
			itemidx = rqs.QueryString["ItemId"];
			if ( itemidx == null)
				throw (new ApplicationException("ItemId= not found in query string" )) ;
			return( int.Parse( itemidx )) ;
		}

		// ----------------- Common.PullQueryStringRefLoadId -----------------
		// extract and return the RefLoadId value from the QueryString 
		public static string PullQueryStringRefLoadId( System.Web.HttpRequest rqs )
		{
			String refLoadId ;
			refLoadId = rqs.QueryString["RefLoadId"];
			if ( refLoadId == null)
				throw (new ApplicationException("RefLoadId= not found in query string" )) ;
			return refLoadId ;
		}
	}   // end class Common

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
			catch (SqlException excp)
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

}



