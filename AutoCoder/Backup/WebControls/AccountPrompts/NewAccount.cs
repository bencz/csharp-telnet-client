
using System;
using System.Collections ;
using System.Collections.Specialized ;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Data.SqlClient ;
using System.Data.SqlTypes;
using System.Text ;
using System.Web;
using System.Web.UI ;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using AutoCoder.Web.Controls ;
using AutoCoder.WebControls.Classes ;

namespace AutoCoder.WebControls.AccountPrompts
{
	public class NewAccount :
		ServerControlBase,
		//		System.Web.UI.WebControls.WebControl,
		INamingContainer,
		IPostBackDataHandler
	{

		// event fired when the menu option is selected
		public event AccountPromptEventHandler CreateAccountClick ;
		public event AccountPromptEventHandler CancelClick ;

		public NewAccount( )
		{
		}

		public string AccountName 
		{
			get { return GetViewStateValue( "AccountName", "" ) ; }
			set { ViewState["AccountName"] = value ; }
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
		public string ConfirmPassword 
		{
			get { return GetViewStateValue( "ConfirmPassword", "" ) ; }
			set { ViewState["ConfirmPassword"] = value ; }
		}

		public bool CreateAccountWasClicked
		{
			get { return GetViewStateBoolValue( "CreateAccountWasClicked", false ) ; }
			set { ViewState["CreateAccountWasClicked"] = value ; }
		}
		public bool CancelWasClicked
		{
			get { return GetViewStateBoolValue( "CancelWasClicked", false ) ; }
			set { ViewState["CancelWasClicked"] = value ; }
		}

		// the css class used to govern the DIV in which all the menu options are rendered.
		public string MenuCssClass
		{
			get { return GetViewStateValue( "MenuCssClass", null ) ; }
			set { ViewState["MenuCssClass"] = value ; }
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

		// ----------------------- ApplyPromptErrorMessage ---------------------
		private void ApplyPromptErrorMessage( )
		{
			// if the error message controls exist, apply the error msg to them.
			AcHtmlSpan span = FindSpanControl( "Error2" ) ;
			if ( span != null )
				span.SetInnerText( PromptErrorMessage ) ;

			// clear the error message after writing it to the prompt.
			SetPromptErrorMessage( null ) ;
		}

		// --------------------------- CreateChildControls -------------------------
		// called after OnInit, but before LoadPostData.
		// After this method returns, asp.net will apply the posted values of the controls
		// from viewstate. ( so only if not postback do control value have to be set )
		protected override void CreateChildControls( )
		{
			AcHtmlTableRow row ;
			AcHtmlSubmitButton button ;

			// create the HtmlTable control which will hold all the login controls.
			AcHtmlTable main = new AcHtmlTable( ) ;
			Controls.Add( main ) ;

			main.AddStyle( "border", "1px black solid" ) ;

			// title
			main.AddNewRow( ).AddNewCell( )
				.SetColSpan( 2 )
				.SetInnerText( "New Account" )
				.AddStyle( "text-align", "center" ) ;

			// account name prompt
			row = main.AddNewRow( ) ;
			row.AddNewCell( )
				.SetInnerText( "Account name" ) ;
			row.AddNewCell( ).AddNewInputText( )
				.SetSize( 40 )
				.SetID( "Text1" ) ;

			// email address prompt
			row = main.AddNewRow( ) ;
			row.AddNewCell( )
				.SetInnerText( "Email address" ) ;
			row.AddNewCell( ).AddNewInputText( )
				.SetSize( 40 )
				.SetID( "Text2" ) ;

			// password prompt
			row = main.AddNewRow( ) ;
			row.AddNewCell( )
				.SetInnerText( "Password" ) ;
			row.AddNewCell( ).AddNewInputText( "password" )
				.SetSize( 20 )
				.SetID( "Text3" ) ;

			// confirm password prompt
			row = main.AddNewRow( ) ;
			row.AddNewCell( )
				.SetInnerText( "ConfirmPassword" ) ;
			row.AddNewCell( ).AddNewInputText( "password" )
				.SetSize( 20 )
				.SetID( "Text4" ) ;

			// "Create account" submit button
			row = main.AddNewRow( ) ;
			button = row.AddNewCell( ).AddNewSubmitButton( )
				.SetValue( "Create Account" )
				.SetID( "Button1" ) ;
			button.ServerClick += new System.EventHandler( this.Button1_Click); 
			
			// "cancel" submit button
			button = row.AddNewCell( ).AddNewSubmitButton( )
				.SetValue( "Cancel" )
				.SetID( "Button2" ) ;
			button.ServerClick += new System.EventHandler( this.Button2_Click); 

			// error message line. 
			AddNewLiteralControl( )
				.SetText( "<br>" )
				.SetID( "Error1" ) ;
			AddNewSpanControl( )
				.SetID( "Error2" )
				.AddStyle( "color", "red" ) ;

			// apply the prompt error message text to the error message controls.
			ApplyPromptErrorMessage( ) ;
		}

		// -------------------- FillNewAccountPromptInfo ----------------------
		private AccountPromptInfo FillNewAccountPromptInfo( )
		{
			AccountPromptInfo pi = new AccountPromptInfo( ) ;
			pi.AccountName = AccountName ;
			pi.EmailAddress = EmailAddress ;
			pi.Password = Password ;
			pi.ConfirmPassword = ConfirmPassword ;	
			return pi ;
		}

		// ------------------- Button1_Click -------------------------------
		// ( Child control events like this one are called after LoadPostData. )
		// create account button clicked.
		private void Button1_Click( object sender, EventArgs e )
		{
			CreateAccountWasClicked = true ;

			// there is an event handler in the code which implements this control.
			if ( CreateAccountClick != null )
			{
				AccountPromptEventArgs args = new AccountPromptEventArgs( ) ;
				args.CreateAccountClick = true ;
				args.PromptInfo = FillNewAccountPromptInfo( ) ;
				CreateAccountClick( this, args ) ;
			}
		}

		// ------------------- Button2_Click -------------------------------
		// NewAccount button clicked.
		private void Button2_Click( object sender, EventArgs e )
		{
			CancelWasClicked = true ;
			if ( CancelClick != null )
			{
				AccountPromptEventArgs args = new AccountPromptEventArgs( ) ;
				args.CancelClick = true ;
				args.PromptInfo = FillNewAccountPromptInfo( ) ;
				CancelClick( this, args ) ;
			}
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
			CreateAccountWasClicked = false ;
			CancelWasClicked = false ;

			AccountName = FindInputTextControl( "Text1" ).Value ;
			EmailAddress = FindInputTextControl( "Text2" ).Value ;
			Password = FindInputTextControl( "Text3" ).Value ;
			ConfirmPassword = FindInputTextControl( "Text4" ).Value ;

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

	} // end class NewAccount
}
