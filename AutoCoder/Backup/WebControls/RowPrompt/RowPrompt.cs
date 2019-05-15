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
using AutoCoder.Web.Controls ;

namespace AutoCoder.WebControls.RowPrompt
{
	// ------------------------- RowPrompt ----------------------------
	[
	ParseChildren( true )
	]
	public class RowPrompt : System.Web.UI.WebControls.WebControl, INamingContainer
	{
		protected Button mOkButton = new Button( ) ;
		protected AcTable mPanelTable = new AcTable( ) ; 
		protected ColumnPromptCollection mColumns = new ColumnPromptCollection( ) ;

		public RowPrompt( )
		{
		}

		public ColumnPromptCollection Columns
		{
			get { return mColumns ; }
		}

		public string ConnString
		{
			get { return GetViewStateValue( "ConnString" ) ; }
			set { ViewState["ConnString"] = value ; }
		}
		public string PromptTable
		{
			get { return GetViewStateValue( "PromptTable" ) ; }
			set { ViewState["PromptTable"] = value ; }
		}
		public bool AlwChg
		{
			get
			{
				if ( ViewState["AlwChg"] == null )
					return false ;
				else
					return bool.Parse( ViewState["AlwChg"].ToString( )) ;
			}
			set { ViewState["AlwChg"] = value ; }
		}
		public string Title
		{
			get { return GetViewStateValue( "Title" ) ; }
			set { ViewState["Title"] = value ; }
		}
		public string RowSelect
		{
			get { return GetViewStateValue( "RowSelect" ) ; }
			set { ViewState["RowSelect"] = value ; }
		}

		protected override void OnInit(EventArgs e)
		{
			base.OnInit(e) ;
			Context.Trace.Write( "OnInit", "Column count: " + Columns.Count.ToString( )) ;
			foreach( ColumnPrompt column in Columns )
			{
				Context.Trace.Write( "OnInit", column.ColumnName ) ;
			}
		}

		protected override void CreateChildControls( )
		{
			mOkButton.ID = "OkButton1" ;
			mOkButton.Text = " OK " ;
			mOkButton.Click += new EventHandler( this.OkButton_Click ) ;

			Controls.Add( mPanelTable ) ;
			CreatePropertyPanel( mPanelTable ) ;

			LiteralControl lit1 = new LiteralControl( "<br>" ) ;
			Controls.Add( lit1 ) ;

			Controls.Add( mOkButton ) ;
		}
         
		protected override void OnPreRender(EventArgs e)
		{
			base.OnPreRender( e ) ;
		}

		protected override void Render( HtmlTextWriter InWriter )
		{
			base.Render( InWriter ) ;
		}

		private void OkButton_Click( object sender, EventArgs e )
		{
			SqlConnection conn = null ;
			Hashtable ht = null ;
			try
			{
				ht = GatherColumnInput( mPanelTable ) ;
				conn = new SqlConnection( ConnString ) ;
				conn.Open( ) ;
				Database.Sql.UpdateRow( conn, PromptTable, RowSelect, ht ) ;
			}
			finally
			{
				if ( conn != null )
					conn.Close( ) ;
			}

			foreach( DictionaryEntry hte in ht )
			{
				Context.Trace.Write( hte.Key.ToString( ), hte.Value.ToString( )) ;
			}
		}

		private Hashtable GatherColumnInput( AcTable InPanel )
		{
			Hashtable ht = new Hashtable( ) ; 
			foreach( AcTableRow row in InPanel.Controls )
			{
				if ( row.Controls.Count == 2 )
				{
					AcTableCell cell00 = (AcTableCell)row.Controls[0] ;
					AcTableCell cell01 = (AcTableCell)row.Controls[1] ;
					if ( cell01.Controls.Count == 1 )
					{
						TextBox tb = (TextBox) cell01.Controls[0] ;
						string ColumnName = cell00.Text ;
						string ColumnValue = tb.Text ;
						ht.Add( ColumnName, ColumnValue ) ;
					}
				}
			}
			return ht ;
		}

		// create the property panel as a table. The panel contains each column
		// being prompted.
		private void CreatePropertyPanel( AcTable InPanel )
		{
			AcTableRow row ;
			AcTableCell cell ;

			// create the property panel as a table.
			InPanel.AddStyle( "background-color", "lightgrey" )
				.AddStyle( "font-family", "Verdana, Arial" )
				.AddStyle( "border-color", "white" ) ;

			// first row holds the property panel title
			InPanel.AddNewRow( ).AddNewCell( )
				.AddAttribute( "ColSpan", "2" )
				.AddStyle( "text-align", "center" )
				.SetText( Title ) ;

			// build a dataset holding the single row to be prompted.
			DataSet rowds = TableRowToDataSet( ) ;
			DataTable rowtbl = rowds.Tables[0] ;

			// add a row to the panel grid for each column in the table row being prompted.
			DataRow rowrow = rowtbl.Rows[0] ;
			foreach ( DataColumn column in rowtbl.Columns )
			{
				string ColumnName = column.ColumnName ;
				ColumnPrompt cp = null ;

				// get ColumnPrompt of this column.
				// ( either default or the entry from mColumns collection )
				if ( Columns.Count == 0 )
					cp = new ColumnPrompt( ColumnName ) ;
				else
					cp = FindColumnPrompt( ColumnName ) ; 
				if ( cp == null )
					continue ;

				// add an AcTableRow to the panel AcTable
				row = InPanel.AddNewRow( ) ;
						
				// first column is the column prompt text
				row.AddNewCell( )
					.AddStyle( "padding-right", "2%" )
					.SetText( ColumnName ) ;

				// second column is the column value
				cell = row.AddNewCell( ) ;
				string ColumnValue = rowrow[ ColumnName ].ToString( ) ;
				if ( AlwChg == true )
				{
					TextBox tb = new TextBox( ) ;
					tb.Text = ColumnValue ;
					cell.Controls.Add( tb ) ;
				}
				else
				{
					cell.AddStyle( "padding-right", "1%" )
						.SetText( ColumnValue ) ;
				}
			}
		}
		// get ColumnPrompt of this column.
		// ( either default or the entry from mColumns collection )
		private ColumnPrompt FindColumnPrompt( string InColumnName )
		{
			ColumnPrompt cp = null ;
			foreach( ColumnPrompt column in Columns )
			{
				if ( column.ColumnName == InColumnName )
				{
					cp = column ;
					break ;
				}
			}
			return cp ;
		}

		// ------------------------ GetViewStateValue ---------------------
		// get string value from state bag. if not found, return null.
		private string GetViewStateValue( string InKey )
		{
			object vs = ViewState[InKey] ;
			if ( vs == null )
				return null ;
			else
				return vs.ToString( ) ;
		}

		// ------------------------ TableRowToDataSet ---------------------
		private DataSet TableRowToDataSet( )
		{
			DataSet rowds = null ;
			SqlConnection conn = null ;
			SqlDataAdapter da = null ;
			SqlCommand cmd = null ;
			try
			{
				conn = new SqlConnection( ConnString ) ;
				conn.Open( ) ;

				cmd = new SqlCommand();
				cmd.Connection = conn ;
				cmd.CommandType = CommandType.Text;
				cmd.CommandText =
					"SELECT * from " + PromptTable +	" WHERE " + RowSelect ;

				da = new SqlDataAdapter( ) ;
				da.SelectCommand = cmd ;

				rowds = new DataSet();
				da.Fill( rowds, "RowPrompt" ) ;
			}
			finally
			{
				if ( conn != null )
					conn.Close( ) ;
			}
			return rowds ;
		}

		// ------------------------ debug ---------------------
		private string debug( )
		{
			string Text ;
			Text =
				"SELECT * from " + PromptTable +	" WHERE " + RowSelect ;
			return Text ;
		}

	} // end class RowPrompt
}
