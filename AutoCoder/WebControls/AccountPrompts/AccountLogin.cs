using System;
using System.Collections ;
using System.Collections.Specialized ;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient ;
using System.Data.SqlTypes;
using System.Text ;
using System.Web;
using System.Web.UI ;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using AutoCoder.WebControls.Classes ;

namespace AutoCoder.WebControls.AccountPrompts
{

	// ------------------------- AccountLogin ----------------------------
	//	<div style="TEXT-ALIGN: center">
	//    <ac:AccountLogin runat="server" id="FootSiteMenu2"
	//			OptionSeparator="&nbsp;&nbsp;|&nbsp;&nbsp;"
	//			MenuCssClass="MenuDiv"  OptionCssClass="SiteMenu"    
	//      OnAccountLoginSelected="AccountLogin_Selected" />
	//  </div>

	// ----------------- in Page_Load - load the AccountLoginList ------------------
	// footer site menu. load the menu.
	//		AccountLoginList SiteMenu =
	//		Tables.AccountLogin.BuildAccountLoginList( this, "Site" ) ;
	//		FootSiteMenu2.AccountLogins = SiteMenu ;

	// ------------------------ AccountLogin_Selected --------------------------
	// menu option selected in the footer or header site menu.
	//	protected void AccountLogin_Selected(
	//		object InSender,
	//		AutoCoder.WebControls.AccountLogin.AccountPromptEventArgs InArgs )
	//		{
	//		home.RedirectTo( this, InArgs.AccountLogin.DocumentFileName ) ;
	//		}

	public class AccountLogin :
		ServerControlBase,
//		System.Web.UI.WebControls.WebControl,
		INamingContainer,
		IPostBackDataHandler
	{

		// event fired when the menu option is selected
		public event AccountPromptEventHandler LoginClick ;
		public event AccountPromptEventHandler NewAccountClick ;

		public AccountLogin( )
		{
		}

		public string AccountNbr 
		{
			get { return GetViewStateValue( "AccountNbr", "" ) ; }
			set { ViewState["AccountNbr"] = value ; }
		}
		public string EmailAddress 
		{
			get { return GetViewStateValue( "EmailAddress", "" ) ; }
			set { ViewState["EmailAddress"] = value ; }
		}
		public string Password 
		{
			get { return GetViewStateValue( "Password", "" ) ; }
			set { ViewState["Password"] = value ; }
		}

		// PromptErrorMessage property. 
		public string PromptErrorMessage
		{
			get { return GetViewStateValue( "PromptErrorMessage", "" ) ; }
			set
			{
				SetPromptErrorMessage( value ) ;
				ApplyPromptErrorMessage( ) ;
			}
		}

		// the "CreateAccount" link text.
		public string PromptNewAccountLinkText
		{
			get { return GetViewStateValue( "PromptNewLink", "Create an account now" ) ; }
			set
			{
				ViewState["PromptNewLink"] = value ;
				ApplyNewAccountText( ) ;
			}
		}

		// the title of the "CreateAccount" link. ( displays above the Link text )
		public string PromptNewAccountLinkTitle
		{
			get { return GetViewStateValue( "PromptNewTitle", "Don't have an account with us?" ) ; }
			set
			{
				ViewState["PromptNewTitle"] = value ;
				ApplyNewAccountText( ) ;
			}
		}

		// the title text of the prompt. spcfd by the user of the control.
		// PromptTitle="xxxxx"
		// Text can contain html.
		public string PromptTitle
		{
			get { return GetViewStateValue( "PromptTitle", "Account Login" ) ; }
			set
			{
				ViewState["PromptTitle"] = value ;
				ApplyPromptTitle( ) ;
			}
		}

		public bool RememberPassword
		{
			get { return GetViewStateBoolValue( "RememberPassword", false ) ; }
			set { ViewState["RememberPassword"] = value ; }
		}
		public bool LoginWasClicked
		{
			get { return GetViewStateBoolValue( "LoginWasClicked", false ) ; }
			set { ViewState["LoginWasClicked"] = value ; }
		}
		public bool NewAccountWasClicked
		{
			get { return GetViewStateBoolValue( "NewAccountWasClicked", false ) ; }
			set { ViewState["NewAccountWasClicked"] = value ; }
		}

		// the css class used to govern the DIV in which all the menu options are rendered.
		public string LoginCssClass
		{
			get { return GetViewStateValue( "LoginCssClass", "" ) ; }
			set { ViewState["LoginCssClass"] = value ; }
		}
		public string NewAccountCssClass
		{
			get { return GetViewStateValue( "NewAccountCssClass", "" ) ; }
			set { ViewState["NewAccountCssClass"] = value ; }
		}

		// ----------------------- ApplyNewAccountText ---------------------
		private void ApplyNewAccountText( )
		{
			// make the title row visible depending on if a PromptTitle is spcfd.
			AcHtmlTableCell cell = FindTableCellControl( "NewAccountTitle" ) ;
			if ( cell != null )
			{
				cell.InnerHtml = PromptNewAccountLinkTitle ;
			}

			// apply the error text to the error text cell.
			AcAspLinkButton link = FindLinkButtonControl( "Link2" ) ;
			if ( link != null )
				link.Text = PromptNewAccountLinkText ;
		}

		// ----------------------- ApplyPromptErrorMessage ---------------------
		private void ApplyPromptErrorMessage( )
		{
			// make the error row visible depending on if an error message to display.
			AcHtmlTableRow row = FindTableRowControl( "ErrorRow1" ) ;
			if ( row != null )
			{
				if ( PromptErrorMessage == "" )
					row.Visible = false ;
				else
					row.Visible = true ;
			}

			// apply the error text to the error text cell.
			AcHtmlTableCell cell = FindTableCellControl( "ErrorText1" ) ;
			if ( cell != null )
			{
				cell.InnerHtml = PromptErrorMessage ;

				// clear the error message after writing it to the prompt.
				SetPromptErrorMessage( null ) ;
			}
		}

		// ----------------------- ApplyPromptTitle ---------------------
		private void ApplyPromptTitle( )
		{
			// make the title row visible depending on if a PromptTitle is spcfd.
			AcHtmlTableRow row = FindTableRowControl( "PromptTitleRow" ) ;
			if ( row != null )
			{
				if ( PromptTitle == "" )
					row.Visible = false ;
				else
					row.Visible = true ;
			}

			// apply the error text to the error text cell.
			AcHtmlTableCell cell = FindTableCellControl( "PromptTitleCell" ) ;
			if ( cell != null )
				cell.InnerHtml = PromptTitle ;
		}

		// --------------------------- CreateChildControls -------------------------
		// called after OnInit, but before LoadPostData.
		// After this method returns, asp.net will apply the posted values of the controls
		// from viewstate. ( so only if not postback do control value have to be set )
		protected override void CreateChildControls( )
		{
			// main table to hold two sub area tables. ( want the sub area tables to align
			// one under the other. )
			AcHtmlTable alignTable = AddNewTableControl( ) ;
			AcHtmlTableCell slot1 =	alignTable.AddNewRow( )
				.AddNewCell( ) ;
			AcHtmlTable subTable1 = slot1.AddNewTable( )
				.SetWidth( "100%" )
				.SetClass( LoginCssClass ) ;

			AcHtmlTableCell slot2 =	alignTable.AddNewRow( )
				.AddNewCell( ) ;
			AcHtmlTable subTable2 = slot2.AddNewTable( )
				.SetWidth( "100%" )
				.SetClass( NewAccountCssClass ) ;

			// create the HtmlTable control which will hold all the login controls.
			if ( LoginCssClass == "" )
				subTable1.AddStyle( "border", "1px black solid" ) ;
			CreateTable1ChildControls( subTable1 ) ;
			
			// prompt to create a new account in the second table ( below the first ).
			if ( NewAccountCssClass == "" )
				subTable2.AddStyle( "border", "1px black solid" ) ;
			CreateTable2ChildControls( subTable2 ) ;

			// apply the prompt error message text to the error message controls.
			ApplyPromptErrorMessage( ) ;
			ApplyPromptTitle( ) ; 
			ApplyNewAccountText( ) ;
		}

		// ---------------------- CreateTable1ChildControls -------------------------
		// called after OnInit, but before LoadPostData.
		// After this method returns, asp.net will apply the posted values of the controls
		// from viewstate. ( so only if not postback do control value have to be set )
		private void CreateTable1ChildControls( AcHtmlTable InTable )
		{
			AcHtmlTableRow row ;
			AcHtmlTableCell cell ;
			AcHtmlSubmitButton button ;

			// prompt title
			InTable.AddNewRow( )
				.SetID( "PromptTitleRow" )
				.AddNewCell( )
				.SetID( "PromptTitleCell" )
				.SetColSpan( 2 )
				.SetInnerHtml( PromptTitle )
				.SetClass( LoginCssClass )
				.AddStyle( "text-align", "center" ) ;

			// account number prompt
			row = InTable.AddNewRow( )
				.SetClass( LoginCssClass ) ;
			row.AddNewCell( )
				.SetInnerText( "Account number:" )
				.SetClass( LoginCssClass )
				.AddStyle( "text-align", "right" ) ;
			row.AddNewCell( ).AddNewInputText( )
				.SetSize( 20 )
				.SetClass( LoginCssClass )
				.SetID( "Text1" ) ;

			// email address prompt
			row = InTable.AddNewRow( ) 
				.SetClass( LoginCssClass ) ;
			row.AddNewCell( )
				.SetInnerText( "or Email address:" )
				.SetClass( LoginCssClass )
				.AddStyle( "text-align", "right" ) ;
			row.AddNewCell( ).AddNewInputText( )
				.SetSize( 20 )
				.SetClass( LoginCssClass )
				.SetID( "Text2" ) ;

			// password prompt
			row = InTable.AddNewRow( ) 
				.SetClass( LoginCssClass ) ;
			row.AddNewCell( )
				.SetInnerText( "Password:" )
				.SetClass( LoginCssClass )
				.AddStyle( "text-align", "right" ) ;
			row.AddNewCell( )
				.SetClass( LoginCssClass )
				.AddNewInputText( "password" )
				.SetClass( LoginCssClass )
				.SetSize( 20 )
				.SetID( "Text3" ) ;

			// Error message row.  Later on will set as visible or not depending on if there
			// is a message to display.
			row = InTable.AddNewRow( )
				.SetClass( LoginCssClass )
				.SetID( "ErrorRow1" ) ;
			row.AddNewBlankCell( ) 
				.SetClass( LoginCssClass ) ;
			row.AddNewCell( )
				.SetID( "ErrorText1" )
				.SetClass( LoginCssClass )
				.AddStyle( "color", "red" ) ;
				
			// Remember password check box.
			row = InTable.AddNewRow( ) 
				.SetClass( LoginCssClass ) ;
			row.AddNewCell( )
				.AddStyle( "text-align", "right" )
				.AddNewInputCheckBox( )
				.SetID( "CheckBox1" ) ;
			row.AddNewCell( )
				.SetInnerText( "Remember me on this computer." ) ;

			// "login" submit button
			row = InTable.AddNewRow( ) 
				.SetClass( LoginCssClass ) ;
			row.AddNewCell( ) 
				.SetClass( LoginCssClass ) ;
			button = row.AddNewCell( )
				.AddNewSubmitButton( )
				.SetValue( "Login" )
				.SetID( "Button1" ) ;
			button.ServerClick += new System.EventHandler( this.Button1_Click); 

			// "forgot your password" asp:LinkButton in a row of its own below the
			// login button.
			cell = InTable.AddNewRow( )
				.AddNewCell( )
				.SetColSpan( 2 )
				.SetAlign( "center" ) ;
			LinkButton link = new LinkButton( ) ;
			cell.Controls.Add( link ) ;
			link.Text = "Forgot your password?" ;
			link.ID = "Link1" ;
			link.Click += new System.EventHandler( this.Link1_Click ) ;
		}

		// ---------------------- CreateTable2ChildControls -------------------------
		// create the controls in the "create a new account" table.
		private void CreateTable2ChildControls( AcHtmlTable InTable )
		{
			InTable.AddNewRow( )
				.AddNewCell( )
				.SetAlign( "center" )
				.SetID( "NewAccountTitle" ) ;
			
			InTable.AddNewRow( )
				.AddNewCell( )
				.SetAlign( "center" )
				.AddNewLinkButton( )
				.SetText( "Create a new account" )
				.SetID( "Link2" )
				.AddClickHandler( new System.EventHandler( this.Link2_Click )) ;
		}

		// -------------------- FillNewAccountPromptInfo ----------------------
		private AccountPromptInfo FillNewAccountPromptInfo( )
		{
			AccountPromptInfo pi = new AccountPromptInfo( ) ;
			pi.EmailAddress = EmailAddress ;
			pi.AccountNbr = AccountNbr ;
			pi.Password = Password ;
			pi.RememberPassword = RememberPassword ;	
			return pi ;
		}

		// ------------------- Button1_Click -------------------------------
		// ( Child control events like this one are called after LoadPostData. )
		// login button clicked.
		private void Button1_Click( object sender, EventArgs e )
		{
			LoginWasClicked = true ;

			// there is an event handler in the code which implements this control.
			if ( LoginClick != null )
			{
				AccountPromptEventArgs args = new AccountPromptEventArgs( ) ;
				args.LoginClick = true ;
				args.PromptInfo = FillNewAccountPromptInfo( ) ;
				LoginClick( this, args ) ;
			}
		}

		// ------------------- Button2_Click -------------------------------
		// NewAccount button clicked.
		private void Button2_Click( object sender, EventArgs e )
		{
			NewAccountWasClicked = true ;
			if ( NewAccountClick != null )
			{
				AccountPromptEventArgs args = new AccountPromptEventArgs( ) ;
				args.CancelClick = true ;
				args.PromptInfo = FillNewAccountPromptInfo( ) ;
				NewAccountClick( this, args ) ;
			}
		}

		// ---------------------- Link1_Click --------------------------------
		// the "forgot your password" link button clicked.
		private void Link1_Click(object sender, System.EventArgs e)
		{
			string Server, User, Password ;

			AutoCoder.Network.Mail.EmailAddress from =
				new AutoCoder.Network.Mail.EmailAddress(
				"srichter@autocoder.com", "Steve Richter" ) ;
			AutoCoder.Network.Mail.EmailAddress to =
				new AutoCoder.Network.Mail.EmailAddress(
				"stephenrichter@gmail.com", "Stephen Richter" ) ;

			AutoCoder.Network.Mail.OutMail.MailMessage msg =
				new AutoCoder.Network.Mail.OutMail.MailMessage( from, to ) ;

			msg.Subject = "Testing OpenSmtp mail component" ;
			msg.Body = "Hello Joe Smith" ;

			AutoCoder.Network.Mail.OutMail.OutMailer mailer = null ;
			Server = "smtpout.secureserver.net" ;
			User = "autocoder" ;
			Password = "denville" ;
			mailer = new AutoCoder.Network.Mail.OutMail.OutMailer(
				Server, User, Password ) ;

			// send the email message.
			try
			{
				mailer.SendMail( msg ) ;
			}
			catch( AutoCoder.Network.Mail.MailException )
			{
			}
		}

		// ---------------------- Link2_Click --------------------------------
		// the "create a new account" link button clicked.
		private void Link2_Click(object sender, System.EventArgs e)
		{

		}

		protected override void OnPreRender(EventArgs e)
		{
			base.OnPreRender( e ) ;
		}

		// ----------------- IPostBackDataHandler.LoadPostData -----------------------
		// called by asp.net after CreateChildControls and after viewstate has been applied
		// to the set of controls. ( but called before events like button clicks. )
		// This is the place to pull the values from the posted web page. 
		bool IPostBackDataHandler.LoadPostData(
			string InPostDataKey, NameValueCollection InPostCollection )
		{
			LoginWasClicked = false ;
			NewAccountWasClicked = false ;
			RememberPassword = false ;

			AccountNbr = FindInputTextControl( "Text1" ).Value ;
			EmailAddress = FindInputTextControl( "Text2" ).Value ;
			Password = FindInputTextControl( "Text3" ).Value ;
			RememberPassword = FindCheckBoxControl( "CheckBox1" ).Checked ;

			return false ;
		}

		// -------------------------- OnInit --------------------------------
		// the first method to be called as part of the page/control request/response
		// cycle.
		protected override void OnInit(EventArgs e)
		{
			base.OnInit(e) ;
			Page.RegisterRequiresPostBack( this ) ;
		}

		// ----------------------- RaisePostDataChangedEvent ------------------------
		void IPostBackDataHandler.RaisePostDataChangedEvent( )
		{
		}

		protected override void Render( HtmlTextWriter InWriter )
		{
			base.Render( InWriter ) ;
		}

		// ---------------------- SetPromptErrorMessage ----------------------
		private void SetPromptErrorMessage( string InValue )
		{
			ViewState["PromptErrorMessage"] = InValue ;
		}

	} // end class AccountLogin
}
