using System;
using System.Collections ;
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

namespace AutoCoder.WebControls.HorizontalMenu
{

	// ----------------------- MenuOptionEventHandler ------------------------
	// some sort of event associated with a menu option ( ex: the option is selected ) 
	public delegate void
	MenuOptionEventHandler( object o, MenuOptionEventArgs args ) ;

	// ----------------------- MenuOptionEventArgs -----------------------------
	// the "EventArgs" parm of a MenuOption event.
	public class MenuOptionEventArgs : EventArgs
	{
		public MenuOptionRow MenuOption = null ;
	}

	// ------------------------- HorizontalMenu ----------------------------
	//	<div style="TEXT-ALIGN: center">
	//    <ac:HorizontalMenu runat="server" id="FootSiteMenu2"
	//			OptionSeparator="&nbsp;&nbsp;|&nbsp;&nbsp;"
  //			MenuCssClass="MenuDiv"  OptionCssClass="SiteMenu"    
	//      OnMenuOptionSelected="MenuOption_Selected" />
	//  </div>

	// ----------------- in Page_Load - load the MenuOptionList ------------------
	// footer site menu. load the menu.
	//		MenuOptionList SiteMenu =
	//		Tables.MenuOption.BuildMenuOptionList( this, "Site" ) ;
	//		FootSiteMenu2.MenuOptions = SiteMenu ;

	// ------------------------ MenuOption_Selected --------------------------
	// menu option selected in the footer or header site menu.
	//	protected void MenuOption_Selected(
	//		object InSender,
	//		AutoCoder.WebControls.HorizontalMenu.MenuOptionEventArgs InArgs )
	//		{
	//		home.RedirectTo( this, InArgs.MenuOption.DocumentFileName ) ;
	//		}

	public class HorizontalMenu : System.Web.UI.WebControls.WebControl, INamingContainer
	{

		// event fired when the menu option is selected
		public event MenuOptionEventHandler MenuOptionSelected ;
		string mWindowOpenFunctionName = "__AutoCoder_WindowOpen" ;

		public HorizontalMenu( )
		{
		}

		public Classes.MenuOptionList MenuOptions
		{
			get { return (Classes.MenuOptionList) ViewState["MenuOptions"] ; }
			set { ViewState["MenuOptions"] = value ; }
		}

		// a string placed between each menu option 
		public string OptionSeparator 
		{
			get { return GetViewStateValue( "OptionSeparator", null ) ; }
			set { ViewState["OptionSeparator"] = value ; }
		}

		// the css class used to govern the DIV in which all the menu options are rendered.
		public string MenuCssClass
		{
			get { return GetViewStateValue( "MenuCssClass", null ) ; }
			set { ViewState["MenuCssClass"] = value ; }
		}

		// the css class used to govern the <a> which the menu option is rendered as.
		public string OptionCssClass
		{
			get { return GetViewStateValue( "OptionCssClass", null ) ; }
			set { ViewState["OptionCssClass"] = value ; }
		}

		protected override void OnInit(EventArgs e)
		{
			base.OnInit(e) ;

			// make sure the WindowOpen javascript function is a part of the page.
			StringBuilder sb = new StringBuilder( 512 ) ;
			sb.Append( "<script language=JavaScript>\n" ) ;
			sb.Append( "function " + mWindowOpenFunctionName + "( url )\n\t{" ) ;
			sb.Append( "window.open( url, '', '') ;\n\t}" ) ;
			sb.Append( "\n</script>" ) ;
			if ( this.Page.IsClientScriptBlockRegistered( mWindowOpenFunctionName ) == false )
			{
				this.Page.RegisterClientScriptBlock( mWindowOpenFunctionName, sb.ToString( )) ;
			}

		}

		AcHtmlDiv AddDivControl( )
		{
			AcHtmlDiv div = new AcHtmlDiv( ) ;
			Controls.Add( div ) ;
			return div ;
		}

		protected override void CreateChildControls( )
		{
			bool FirstLink = true ; 
			MenuOptionList mul = MenuOptions ;

			// no menu options. exit.
			if ( mul == null )
				return ;

			// the menu is drawn within a <DIV> ... </DIV> control.
			AcHtmlDiv div = AddDivControl( ) ;
			if ( MenuCssClass != null )
				div.AddAttribute( "class", MenuCssClass ) ;
				
			foreach( MenuOptionRow OptnRow in mul )
			{
				if (( FirstLink == false ) && ( OptionSeparator != null ))
					div.AddNewHtmlLiteral( OptionSeparator ) ;

				if (( OptnRow.Category == MenuOptionCategory.Document ) ||
						( OptnRow.Category == MenuOptionCategory.SitePage ))
				{
					LinkButton link = new LinkButton( ) ;
					link.Text = OptnRow.OptionText ;
					link.CommandArgument = OptnRow.OptionName ; 
					link.Command += new CommandEventHandler( this.LinkButton_CommandClick ) ;
					if ( OptionCssClass != null )
						link.CssClass = OptionCssClass ;
					div.Controls.Add( link ) ;
				}
				else if ( OptnRow.Category == MenuOptionCategory.email )
				{
					HyperLink link = new HyperLink( ) ;
					link.Text = OptnRow.OptionText ;
					link.NavigateUrl = OptnRow.Url ;
					if ( OptionCssClass != null )
						link.CssClass = OptionCssClass ;
					div.Controls.Add( link ) ;
				}
				else if ( OptnRow.Category == MenuOptionCategory.Url )
				{
					HtmlAnchor anchor = new HtmlAnchor( ) ;
					div.Controls.Add( anchor ) ;
					anchor.InnerText = OptnRow.OptionText ;
					string ClickString =
						"javascript:window.open( '" +
						OptnRow.Url + 
						"', '', '');" ;
					ClickString =
						"javascript:" + mWindowOpenFunctionName + "( '" +
						OptnRow.Url +	"');" ;
					//					anchor.Attributes.Add( "onclick", ClickString ) ;
					anchor.HRef = ClickString ;
					if ( OptionCssClass != null )
						anchor.Attributes.Add( "class", OptionCssClass ) ;

					if ( 1 == 2 )
					{
						AcHtmlSpan span = div.AddNewHtmlSpan( OptnRow.OptionText ) ;
						if ( OptionCssClass != null )
							span.Attributes.Add( "class", OptionCssClass ) ;
						span.AddStyle( "text-decoration", "underline" ) ;
						span.AddStyle( "cursor", "hand" ) ;
						span.Attributes.Add( "onclick", ClickString ) ;
					}
				}
				else
				{
					div.AddNewHtmlLiteral( OptnRow.OptionText ) ;
				}
				FirstLink = false ;
			}
//			mOkButton.ID = "OkButton1" ;
//			mOkButton.Text = " OK " ;
//			mOkButton.Click += new EventHandler( this.OkButton_Click ) ;
		}

		// ------------------------ GetViewStateValue ---------------------
		// get string value from state bag. if not found, return null.
		// InNullValue - value to return if the ViewState value is missing.
		private string GetViewStateValue( string InKey, string InNullValue  )
		{
			object vs = ViewState[InKey] ;
			if ( vs == null )
				return InNullValue ;
			else
				return vs.ToString( ) ;
		}

		// link button is clicked.  fire the "SelectedMenuOption" event.
		private void LinkButton_CommandClick( object sender, CommandEventArgs e )
		{
			string OptionName = e.CommandArgument.ToString( ) ;
			MenuOptionRow optn = MenuOptions.FindMenuOption( OptionName ) ;
			if ( optn != null )
			{
				MenuOptionEventArgs args = new MenuOptionEventArgs( ) ;
				args.MenuOption = optn ;
				MenuOptionSelected( this, args ) ;
			}
		}
         
		protected override void OnPreRender(EventArgs e)
		{
			base.OnPreRender( e ) ;
		}

		protected override void Render( HtmlTextWriter InWriter )
		{
			base.Render( InWriter ) ;
		}
	} // end class HorizontalMenu
}
