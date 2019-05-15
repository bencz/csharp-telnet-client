using System;
using System.Data;
using System.Drawing;
using System.Web;
using System.Web.UI ;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

namespace AutoCoder
{
	namespace Controls
	{

// ------------------------- ArticleSummary ----------------------------
		public class ArticleSummary : System.Web.UI.WebControls.WebControl
		{
			string mArticleName ;
			Color mTitleColor ;

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
				InWriter.RenderBeginTag( "td" ) ;
				InWriter.Write( "url to the full article." ) ;
				InWriter.RenderEndTag( ) ;
				InWriter.RenderEndTag( ) ;
				InWriter.RenderEndTag( ) ;
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
		}

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
	}
}
