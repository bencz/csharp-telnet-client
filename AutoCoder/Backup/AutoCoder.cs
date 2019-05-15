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

		// --------------- Common.IsDuplicateKeyExcp -------------------------
		// check the SqlException for error code = 2627 ( inserting duplicate key )
		public static bool IsDuplicateKeyExcp( SqlException InExcp )
		{
			if (( InExcp.Errors.Count >= 1 ) && ( InExcp.Errors[0].Number == 2627 ))
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
} // end namespace AutoCoder



