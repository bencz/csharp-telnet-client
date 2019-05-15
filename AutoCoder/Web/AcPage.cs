using System;
using System.Collections;
using System.Data ;
using System.Data.SqlClient ;
using System.Text ;
using System.Web.UI;
using System.Web.Security;
using System.Configuration;
using AutoCoder.WebTables ;

namespace AutoCoder.Web
{
	public class AcPage : Page
	{
		string mDatabaseConnectionString ;

//		WebTables.VisitorMaster mVisitorMaster ;
//		string mPageFileName ;
//		string mPageDirPath ;		// directory path, relative to application, of this page.

		public AcPage( )
		{
			mDatabaseConnectionString = null ;
//			mVisitorMaster = new Tables.VisitorMaster( ) ;
//			mPageDirPath = null ;
//			mPageFileName = null ;
		}

		// get database connection string that is stored in <Configuration>.<AppSettings> 
		// section of Web.Config. 
		public virtual string DatabaseConnectionString
		{
			get
			{
				throw(
					new ApplicationException(
					"Derived classes must implement the DatabaseConnectionString property." )) ;
			}
		}

		// directory page, relative to application, of this page. 
		// all derived classes should implement this virtual property.
		public virtual string PageDirPath
		{
			get
			{
				throw(
					new ApplicationException(
					"Derived classes must implement the PageDirPath property." )) ;
			}
		}

		// the file name of this page:  help.aspx, Catalogue.aspx, ...
		// all derived classes should implement this virtual property.
		public virtual string PageFileName
		{
			get
			{
				throw(
					new ApplicationException(
					"Derived classes must implement the PageFileName property." )) ;
			}
		}

		// the name of this page.  PageName is unique within the web application.
		public virtual string PageName
		{
			get
			{
				throw(
					new ApplicationException(
					"Derived classes must implement the PageName property." )) ;
			}
		}

		// ---------------------------- AcPage_Load ----------------------------
		// called by all .aspx pages from the Page_Load method.
		public void AcPage_Load( )
		{
			PageLoad_Common( false ) ;
		}
		public void AcPage_Load( bool InLoadIdFeatureUsed )
		{
			PageLoad_Common( InLoadIdFeatureUsed ) ;
		}

		// ---------------------- AppendSessIdToUrl --------------------------
		// SessId is used in the event the visiting browser does not support cookies.
		// If so, the SessId is included in the URL of all redirects and holds the
		// server side key to session info like VisitorId.
		public string AppendSessIdToUrl( string InUrl )
		{
			string PassSessId = ComposePassSessId( ) ;
			if ( PassSessId == null )
				return InUrl ;
			else if ( InUrl.IndexOf( "?" ) == -1 )
				return ( InUrl + "?" + PassSessId ) ;
			else
				return ( InUrl + "&" + PassSessId ) ;
		}

		// ---------------------- AssureDatabaseConnectionString ----------------
		private void AssureDatabaseConnectionString( )
		{
			if ( mDatabaseConnectionString == null )
			{
				mDatabaseConnectionString = DatabaseConnectionString ;
				if ( mDatabaseConnectionString == null )
					throw( new ApplicationException(
						"derived class returned a null ConnectionString" )) ;
			}
		}

		// calc the sub directory needed when redirecting from this page to another page.
		public string CalcRedirectDirPath( string InToPageDirPath )
		{
			string DirPath ;
			string FromPageDirPath = PageDirPath ;
			if (( FromPageDirPath == null ) && ( InToPageDirPath == null ))
				DirPath = null ;
			else if ( FromPageDirPath == null )
				DirPath = InToPageDirPath ;

				// to page in the appl root. add ".." to the relative page for each directory
			// in the from page dirpath. ( have to unwind 1 or more directory levels )
			else if ( InToPageDirPath == null )
			{
				StringBuilder sb = new StringBuilder( ) ;
				int Cx = Text.Stringer.Talley( FromPageDirPath, new char[] {'/', '\\'} ) ;
				++Cx ;
				for( int Ix = 0 ; Ix < Cx ; ++Ix )
				{
					if ( sb.Length > 0 )
						sb.Append( "/" ) ;
					sb.Append( ".." ) ;
				}
				DirPath = sb.ToString( ) ;
			}

			else if ( FromPageDirPath.ToLower( ) == InToPageDirPath.ToLower( ))
				DirPath = null ;
			else
				DirPath = "../" + InToPageDirPath ;
			return DirPath ;
		}

		// ---------------------- ComposePassSessId ----------------------
		// the SessId used in case the visitor does not support cookies. 
		// 
		public string ComposePassSessId( )
		{
			object oNoCookiesSessId = ViewState[ "NoCookiesSessId" ] ;
			if ( oNoCookiesSessId != null )
				return( "SessId=" + oNoCookiesSessId.ToString( )) ;
			else
				return null ;
		}

		// -------------------------- CreatePageLoadId -----------------------
		// create a LoadMaster record for this page.
		private void CreatePageLoadId( )
		{
			// virtual calls get actual table and row objects.
			AcLoadMaster lm = NewLoadMaster( ) ;
			AcLoadMasterRow row = lm.NewRow( ) ;

			// fill the row.
			row.SessionId = Session.SessionID ;
			row.PageName = PageName ;
			row.PageFileName = PageFileName ;
			row.PageDirPath = PageDirPath ;
			row.LoadTs = DateTime.UtcNow ;
			row.LastPostbackTs = DateTime.UtcNow ;
			row.PostbackCx = 0 ;
			row.RawUrl = Request.RawUrl ;

			// the seqnbr of the load of this page within the session.
			row.Seqn = IncSessionPageLoadSeqn( ) ;

			// add the LoadMasterRow to the table.
			row = lm.Add( this, row ) ;

			// store the LoadId in ViewState of this page.
			ViewState[ "LoadId" ] = row.LoadId ;
		}

		// ---------------------- GetDatabaseConnection -----------------------
		public SqlConnection GetDatabaseConnection( )
		{
			AssureDatabaseConnectionString( ) ;
			SqlConnection conn = new SqlConnection( mDatabaseConnectionString ) ;
			conn.Open( ) ;
			return conn ;
		}

		// ------------------------ GetPageViewStateIntValue ---------------------
		// get int value from state bag. if not found, return arg2 value.
		public int GetPageViewStateValue( string InKey, int InNullValue  )
		{
			object vsv = ViewState[InKey] ;
			if ( vsv == null )
				return InNullValue ;
			else
				return (int) vsv ;
		}

		// ------------------------ GetPageViewStateValue ---------------------
		// get string value from state bag. if not found, return null.
		// InNullValue - value to return if the ViewState value is missing.
		public string GetPageViewStateValue( string InKey, string InNullValue  )
		{
			object vsv = ViewState[InKey] ;
			if ( vsv == null )
				return InNullValue ;
			else
				return vsv.ToString( ) ;
		}

		// --------------------------- GetUserId ----------------------------
		// get the UserId of the current visitor
		public virtual string GetUserId( ) 
		{
			string VisitorId = Session["VisitorId"].ToString( ) ;

			// read the online Visitor record.  will throw exception if does not exist.
			AcVisitorMasterRow row = NewVisitorMaster( ).Chain( this, VisitorId ) ;

			return row.UserId ;
		}

		// ------------------------- GetUserMaster ------------------------------
		public WebTables.UserMasterRow GetUserMaster( )
		{
			AcVisitorMasterRow visitorRow = GetVisitorMaster( ) ;
			UserMasterRow userMasterRow =
				NewUserMaster( ).Chain( this, visitorRow.UserId ) ;
			return userMasterRow ;
		}

		// ------------------------ GetViewStateValue ---------------------
		// get string value from state bag. if not found, return null.
		// InNullValue - value to return if the ViewState value is missing.
		public string GetViewStateValue( string InKey, string InNullValue  )
		{
			object vs = ViewState[InKey] ;
			if ( vs == null )
				return InNullValue ;
			else
				return vs.ToString( ) ;
		}

		// --------------------------- GetVisitorId ----------------------------
		public string GetVisitorId( ) 
		{
			object oVisitorId = Session["VisitorId"] ;
			if ( oVisitorId == null )
				throw( new ApplicationException( "VisitorId missing from Session" )) ;
			return oVisitorId.ToString( ) ;
		}

		// ------------------------- GetVisitorMaster ------------------------------
		public AcVisitorMasterRow GetVisitorMaster( )
		{
			string VisitorId = GetVisitorId( ) ;

			// got the VisitorId.  now read its VisitorMaster.
			AcVisitorMasterRow row =
				NewVisitorMaster( ).Chain( this, VisitorId ) ;
			return row ;
		}

		// ------------------------ IncSessionPageLoadSeqn -----------------------
		private int IncSessionPageLoadSeqn( )
		{
			int LoadSeqn = 0 ; 
			object oPageLoadSeqn = Session[ "PageLoadSeqn" ] ;
			if ( oPageLoadSeqn == null )
				LoadSeqn = 0 ;
			else
				LoadSeqn = int.Parse( oPageLoadSeqn.ToString( )) ;
			++LoadSeqn ;
			Session[ "PageLoadSeqn" ] = LoadSeqn.ToString( ) ;
			return LoadSeqn ;
		}

		// ---------------------- NewAnonymousVisitorId -----------------------
		public string NewAnonymousVisitorId( )
		{
			AcVisitorMasterRow row = NewVisitorMaster( ).AddNewVisitor( this, "" ) ;
			Session[ "VisitorId" ] = row.VisitorId ;
			return row.VisitorId ;
		}

		// ------------------------------ NewUserMaster --------------------------
		public virtual UserMaster NewUserMaster( )
		{
			return( new UserMaster( )) ;
		}

		// ------------------------------ NewLoadMaster --------------------------
		// virtual method. return reference to LoadMaster used by the Page.  
		public virtual AcLoadMaster NewLoadMaster( ) 
		{
			throw(
				new ApplicationException(
				"Derived classes must implement the NewLoadMaster method." )) ;
		}

		// ------------------------------ NewVisitorMaster --------------------------
		public virtual AcVisitorMaster NewVisitorMaster( ) 
		{
			throw(
				new ApplicationException(
				"Derived classes must implement the NewVisitorMaster method." )) ;
		}

		// --------------------------- PageLoad_Common -----------------------------
		// each page in the application should run this method as the first step in the
		// Page_Load method.
		private void PageLoad_Common( bool InLoadIdFeatureUsed )
		{
			// Inc the page count within this session. ( this will assure a row exists 
			// in the SessionHistory table for the SessionID. )
//			SessionHistory.IncSessionPageCount( this ) ;

			// assure, retrieve the VisitorId for this page.
			string VisitorId = (string) Session[ "VisitorId" ] ;
			if ( VisitorId == null )
			{
				VisitorId = NewAnonymousVisitorId( ) ;
			}

			// read the VisitorMaster record.  will throw exception if does not exist.
			GetVisitorMaster( ) ;

			// the LoadMaster feature is used by this page.
			if ( InLoadIdFeatureUsed == true )
			{
				// not postback.  create the loadid.
				if ( IsPostBack == false )
					CreatePageLoadId( ) ;
				
					// postback.  update the LoadMaster row.
				else
					UpdatePageLoadId( ) ;
			}
		}

		// ---------------------- PullQueryString ---------------------------
		// return value of string item from QueryString.
		public string PullQueryString( string InKey )
		{
			object item = Request.QueryString[ InKey ] ;
			if ( item == null )
				return null ;
			else
				return item.ToString( ) ;
		}

		// ------------------------- RedirectTo -------------------------------
		// Called by pages when redirecting to this page.
		// aspx pages should implement their own version when the page contains 
		// QueryString parameters.  Otherwise, this base class can do the job.
		public virtual void RedirectTo( AcPage InFromPage )
		{
			RedirectTo( InFromPage, new RedirectParms( )) ;
		}

		// ------------------------- RedirectTo -------------------------------
		// Called by pages when redirecting to this page.
		// aspx pages should implement their own version when the page contains 
		// QueryString parameters.  Otherwise, this base class can sparamete
		public void RedirectTo( AcPage InFromPage, RedirectParms InParms )
		{
			bool bArgs = false ;
			string SubDir = InFromPage.CalcRedirectDirPath( PageDirPath ) ;
			string PassSessId = InFromPage.ComposePassSessId( ) ;
			
			// Build the full path name needed to redirect to this page relative to the
			// current page ( the from page )
			System.Text.StringBuilder url = new System.Text.StringBuilder( 256 ) ;
			if ( SubDir != null )
				url.Append( SubDir + "/" ) ;
			url.Append( PageFileName ) ;

			// add the SessId needed when a "no cookies browser" 
			if ( PassSessId != null )
			{
				url.Append( "?" + PassSessId ) ;
				bArgs = true ;
			}

			// add the querystring parms.
			if ( InParms.IsNotEmpty )
			{
				if ( bArgs == false )
					url.Append( "?" + InParms.QueryString ) ;
				else
					url.Append( "&" + InParms.QueryString ) ;
				bArgs = true ;
			}

			// redirect to the page.
			InFromPage.Response.Redirect( url.ToString( ) ) ;
		}

		// ---------------------- UpdatePageLoadId --------------------------
		// postback of a page. Update the last postback timestamp in the LoadMaster row
		// of this page.
		private void UpdatePageLoadId( )
		{
			// LoadId of the page is stored in view state.
			string LoadId = GetViewStateValue( "LoadId", null ) ;
 
			// read the LoadMaster row.
			AcLoadMaster lm = NewLoadMaster( ) ;
			AcLoadMasterRow row = lm.Chain( this, LoadId ) ;

			// update the postback columns.
			++row.PostbackCx ;
			row.LastPostbackTs = DateTime.UtcNow ;
			lm.Update( this, LoadId, row ) ;
		}

		// ------------------- VisitorIsLoggedIn ------------------------------
		// online Visitor is successfully logged in ( not anonymous Visitor )
		public bool VisitorIsLoggedIn( )
		{
			object oLoginAccountNbr = Session[ "LoginAccountNbr" ] ;
			if ( oLoginAccountNbr == null )
				return false ;
			else
				return true ;
		}

		// ---------------------- VisitorLogin --------------------------------------
		// update the online Visitor with the valid authenticated VisitorId
		public void VisitorLogin( string InUserId )
		{
			NewVisitorMaster( ).UpdateUserId( this, GetVisitorId( ), InUserId ) ;
		}

		// ---------------------- VisitorLogout --------------------------------
		// log a Visitor out by assigning a new anonymous Visitor id to their online id.
		public void VisitorLogout( )
		{
			NewVisitorMaster( ).UpdateUserId( this, GetVisitorId( ), "" ) ;
		}
	}

}
