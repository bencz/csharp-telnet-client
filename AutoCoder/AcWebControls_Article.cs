// AcWebControls_Article.cs
using System;
using System.Collections ;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Data.SqlClient ;
using System.Data.SqlTypes;
using System.Web;
using System.Web.UI ;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

namespace AutoCoder
{
	namespace Web
	{
		public class ArticleShow
		{
			string mUrl ;
			string mTitle ;

			public string Url
			{
				get { return mUrl ; }
				set { mUrl = value ; }
			}
			public string Title 
			{
				get { return mTitle ; }
				set { mTitle = value ; }
			}
		} // end class ArticleShow
	} // end namespace Web
} // end namespace AutoCoder

		namespace AutoCoder.Web.Controls
		{

			// ------------------------- ArticleSummary ----------------------------
			[
			ParseChildren( true )
			]
			public class ArticleSummary : System.Web.UI.WebControls.WebControl
			{
				string mArticleName ;
				Color mTitleColor ;
				ArticleShow mShow ;
				Style mUrlStyle ;

				public ArticleSummary( )
				{
					mShow = new ArticleShow( ) ;
					mUrlStyle = new Style( ) ;
				}

				public string ArticleName
				{
					get { return mArticleName ; }
					set { mArticleName = value ; }
				}
				public Color TitleColor
				{
					get { return mTitleColor ; }
					set { mTitleColor = value ; }
				}
				public Style UrlStyle
				{
					get { return mUrlStyle ; }
				}
				public ArticleShow Show
				{
					get { return mShow ; }
					set { mShow = value ; }
				}

				protected override void Render( HtmlTextWriter InWriter )
				{
					InWriter.AddStyleAttribute( "font-family", "Verdana,Arial" ) ;
					InWriter.RenderBeginTag( "table" ) ;

					InWriter.RenderBeginTag( "tr" ) ;
					InWriter.AddStyleAttribute( "font-weight", "bold" ) ;
					InWriter.AddStyleAttribute( "font-size", "larger" ) ;
					InWriter.AddStyleAttribute( "color", ColorTranslator.ToHtml( mTitleColor )) ;
					InWriter.RenderBeginTag( "td" ) ;
					InWriter.Write( "article title text" ) ;
					InWriter.RenderEndTag( ) ;
					InWriter.RenderEndTag( ) ;

					InWriter.RenderBeginTag( "tr" ) ;
					InWriter.RenderBeginTag( "td" ) ;
					InWriter.Write( "article body text<br>line 2 of the article." ) ;
					InWriter.RenderEndTag( ) ;
					InWriter.RenderEndTag( ) ;

					InWriter.RenderBeginTag( "tr" ) ;
					mUrlStyle.AddAttributesToRender( InWriter ) ;
					InWriter.RenderBeginTag( "td" ) ;
					InWriter.Write( "url to the full article. " + mShow.Title ) ;
					InWriter.RenderEndTag( ) ;
					InWriter.RenderEndTag( ) ;
					InWriter.RenderEndTag( ) ;
				}

			} // end class ArticleSummary

// ----------------------------- CompDemo ----------------------------
// composite control demo
			public class CompDemo : System.Web.UI.Control, INamingContainer
			{
				protected override void CreateChildControls( )
				{
					LiteralControl text ;
					text = new LiteralControl(
						"<h1>ASP.NET Control Development in C#</h1>" ) ;
					Controls.Add( text ) ;
				}
			} // end class CompDemo

// ----------------------------- TableDemo ----------------------------
			public class TableDemo : WebControl, INamingContainer
			{
				Table mTable ;

				protected override void CreateChildControls( )
				{
					LiteralControl text ;
					text = new LiteralControl(
						"<h1>ASP.NET Control Development in C#</h1>" ) ;
					Controls.Add( text ) ;

					TableRow row ;
					TableCell cell ;

// create a table
					mTable = new Table( ) ;
					mTable.BorderWidth = 2 ;
					Controls.Add( mTable ) ;
					mTable.Visible = true ;

// add 10 rows each with 5 cells
					for( int x = 0 ; x < 10 ; x++ )
					{
						row = new TableRow( ) ;
						mTable.Rows.Add( row ) ;

						// create the cells of the row
						for( int y = 0 ; y < 5 ; y++ )
						{
							TextBox textbox ;
							textbox = new TextBox( ) ;
							textbox.Text = "Row: " + x  + " Cell: " + y ;

							cell = new TableCell( ) ;
							cell.Controls.Add( textbox ) ;
							row.Cells.Add( cell ) ;
						} // end for cells
					} // end for rows
				} // end CreateChildControls

			} // end class TableDemo

// ------------------ InputDemo -----------------------------
			public class InputDemo : WebControl, IPostBackDataHandler,
																IPostBackEventHandler
			{
				public event EventHandler TextChanged ; 


				public InputDemo( )	: base( "input" )
				{
					Text = "abc" ;
				}

// ----------------------- InputDemo.Number property ------------------------
				public int Number
				{
					get
					{
						if ( ViewState["Number"] != null )
							return (int) ViewState["Number"] ;
						else
							return 50 ;
					}
					set
					{
						ViewState["Number"] = value ;
					}
				}

				public string Text
				{
					get
					{
						string text = (string) ViewState["value"] ;
						if ( text == null )
							return "" ;
						else
							return text ;
					}
					set
					{ ViewState["value"] = value ; }
				}

				protected override void AddAttributesToRender( HtmlTextWriter InWriter )
				{
					base.AddAttributesToRender( InWriter ) ;
					InWriter.AddAttribute( HtmlTextWriterAttribute.Name, UniqueID ) ;
					InWriter.AddAttribute( HtmlTextWriterAttribute.Type, "input" ) ;
					InWriter.AddAttribute( "value", Text ) ;
				}

				protected override void Render( HtmlTextWriter InWriter )
				{
					base.Render( InWriter ) ;
					Page.Trace.Write( "InputDemo control", "Render" ) ;
					InWriter.Write( "<br>The Number is " + Number.ToString( ) + " (" ) ;
					InWriter.Write( "<a href=\"javascript:" +
													Page.ClientScript.GetPostBackEventReference( this, "inc" ) +
													"\">Increase Number</a>" ) ;
					InWriter.Write( " or " ) ;
					InWriter.Write( "<a href=\"javascript:" +
						Page.ClientScript.GetPostBackEventReference( this, "dec" ) +
						"\">Decrease Number)</a>" ) ;
				}

				bool IPostBackDataHandler.LoadPostData( string InPostKey,
											System.Collections.Specialized.NameValueCollection InPostCollection )
				{
					bool raiseEvent = false ;
					Page.Trace.Write( "InputDemo control", "Before change " + Text ) ;
					if ( Text != InPostCollection[InPostKey] )
						raiseEvent = true ;
					Text = InPostCollection[ InPostKey ] ;
					Page.Trace.Write( "InputDemo control", "LoadPostData " + InPostKey + " " + Text ) ;
					return raiseEvent ;
				}

				void IPostBackDataHandler.RaisePostDataChangedEvent( )
				{
					if ( TextChanged != null )
						TextChanged( this, EventArgs.Empty ) ;
				}

				void IPostBackEventHandler.RaisePostBackEvent( string InArgument )
				{
					if ( InArgument == "inc" )
						Number = Number + 1 ;
					if ( InArgument == "dec" )
						Number = Number - 1 ; 
				}

			} // end class InputDemo
	} // end namespace AutoCoder.Web.Controls

namespace AutoCoder
{

// ---------------------- ArticleSummaryOld --------------------------------
	public class ArticleSummaryOld : System.Web.UI.UserControl
	{
		protected System.Web.UI.HtmlControls.HtmlTable table1;
		protected System.Web.UI.WebControls.LinkButton LinkButton1;
		protected System.Web.UI.HtmlControls.HtmlTableCell Heading;

		private void Page_Load(object sender, System.EventArgs e)
		{
			Heading.InnerHtml = "abc<br>efg" ;
			// Put user code to initialize the page here
		}

		#region Web Form Designer generated code
		override protected void OnInit(EventArgs e)
		{
			//
			// CODEGEN: This call is required by the ASP.NET Web Form Designer.
			//
			InitializeComponent();
			base.OnInit(e);
		}
		
		/// <summary>
		///		Required method for Designer support - do not modify
		///		the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.LinkButton1.Click += new System.EventHandler(this.LinkButton1_Click);
			this.Load += new System.EventHandler(this.Page_Load);

		}
		#endregion

		private void LinkButton1_Click(object sender, System.EventArgs e)
		{
			
		}
	}
} // end namespace AutoCoder
