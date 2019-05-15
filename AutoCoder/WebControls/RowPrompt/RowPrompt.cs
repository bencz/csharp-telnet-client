using System;
using System.Collections ;
using System.Collections.Generic;
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
using AutoCoder.Core;

namespace AutoCoder.WebControls.RowPrompt
{
	// ------------------------- RowPrompt ----------------------------
	[
	ParseChildren( true )
	]
	public class RowPrompt : System.Web.UI.WebControls.WebControl, INamingContainer
	{
		protected Button mOkButton = new Button( ) ;
    protected Button mCancelButton = new Button();
    protected AcTable mPanelTable = new AcTable(); 
		protected ColumnPromptCollection mColumns = new ColumnPromptCollection( ) ;
    event RowPromptResultsEvent mRowPromptResultsEvent = null;
    AcNamedValues mInputColumnValues = null;
    AcColumnInfoDictionary mColumnInfo = null;

		public RowPrompt( )
		{
		}

    public ActionCode ActionCode
    {
      get
      {
        object oAction = ViewState["ActionCode"];
        if (oAction == null)
          return ActionCode.Change;
        else
          return (ActionCode) oAction;
      }
      set { ViewState["ActionCode"] = value; }
    }

    public AcColumnInfoDictionary ColumnInfo
    {
      get
      {
        if (mColumnInfo == null)
          mColumnInfo = (AcColumnInfoDictionary)ViewState["ColumnInfo"];
        return mColumnInfo;
      }
      set
      {
        mColumnInfo = value;
        ViewState["ColumnInfo"] = value;
      }
    }

    /// <summary>
    /// the prompted value input results  
    /// </summary>
    public AcNamedValues OutputColumnValues
    {
      get { return GatherColumnValues( mPanelTable ); }
    }

    public AcNamedValues InputColumnValues
    {
      set { mInputColumnValues = value; }
    }

		public ColumnPromptCollection Columns
		{
			get { return mColumns ; }
		}

    /// <summary>
    /// Connection string used to select the row from the database table, and
    /// then update the row in the table with the row prompt entries.
    /// </summary>
		public string ConnString
		{
			get { return GetViewStateValue( "ConnString" ) ; }
			set { ViewState["ConnString"] = value ; }
		}

    /// <summary>
    /// The keys to the row being prompted. 
    /// store in ViewState so this RowPrompt user control can return the keys
    /// to the caller via the RowPromptResultsEvent.
    /// </summary>
    public AcNamedValues PrimaryKeys
    {
      get { return (AcNamedValues)ViewState["PrimaryKeys"]; }
      set { ViewState["PrimaryKeys"] = value; }
    }

    /// <summary>
    /// the database table of the row being prompted
    /// </summary>
		public string PromptTable
		{
			get { return GetViewStateValue( "PromptTable" ) ; }
			set { ViewState["PromptTable"] = value ; }
		}

    /// <summary>
    /// Allow changes to the prompted row. true or false.
    /// </summary>
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

    /// <summary>
    /// Title text of the prompt panel
    /// </summary>
		public string Title
		{
			get { return GetViewStateValue( "Title" ) ; }
			set { ViewState["Title"] = value ; }
		}

    public RowPromptResultsEvent RowPromptResultsEvent
    {
      get { return mRowPromptResultsEvent ; }
      set { mRowPromptResultsEvent = value; }
    }

		public string RowSelect
		{
			get { return GetViewStateValue( "RowSelect" ) ; }
			set { ViewState["RowSelect"] = value ; }
		}

		protected override void OnInit(EventArgs e)
		{
			base.OnInit(e) ;
//			Context.Trace.Write( "OnInit", "Column count: " + Columns.Count.ToString( )) ;
//			foreach( ColumnPrompt column in Columns )
//			{
//				Context.Trace.Write( "OnInit", column.ColumnName ) ;
//			}
		}

    /// <summary>
    /// Cancel the RowPrompt. Signal the page that contains this control. 
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void CancelButton_Click(object sender, EventArgs e)
    {
      if (RowPromptResultsEvent != null)
      {
        RowPromptResults results = new RowPromptResults();
        results.Action = ActionCode.Cancel;
        results.PrimaryKeys = PrimaryKeys;
        RowPromptResultsEvent(this, results);
      }
    }

		protected override void CreateChildControls( )
		{
			mOkButton.ID = "OkButton1" ;
			mOkButton.Text = " OK " ;
			mOkButton.Click += new EventHandler( this.OkButton_Click ) ;

      mCancelButton.ID = "CancelButton1";
      mCancelButton.Text = " Cancel ";
      mCancelButton.Click += new EventHandler(this.CancelButton_Click);

			Controls.Add( mPanelTable ) ;
			CreatePropertyPanel( mPanelTable ) ;

			LiteralControl lit1 = new LiteralControl( "<br>" ) ;
			Controls.Add( lit1 ) ;
			Controls.Add( mOkButton ) ;

      lit1 = new LiteralControl("&nbsp;&nbsp;");
      Controls.Add(lit1);
      Controls.Add(mCancelButton);
    }
         
		protected override void OnPreRender(EventArgs e)
		{
			base.OnPreRender( e ) ;
		}

		protected override void Render( HtmlTextWriter InWriter )
		{
			base.Render( InWriter ) ;
		}

    /// <summary>
    /// process the input to the prompted columns.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
		private void OkButton_Click( object sender, EventArgs e )
		{
      if (RowPromptResultsEvent != null)
      {
        RowPromptResults results = new RowPromptResults();
        results.Action = ActionCode;
        results.ColumnValues = OutputColumnValues;
        results.PrimaryKeys = PrimaryKeys ;
        RowPromptResultsEvent(this, results);
      }

      else
      {
        using (SqlConnection conn = new SqlConnection(ConnString))
        {
          conn.Open();
          Sql.SqlCore.UpdateRow(conn, PromptTable, RowSelect, OutputColumnValues);
        }
      }
		}

    /// <summary>
    /// Gather the values of the prompted columns.
    /// </summary>
    /// <param name="InPanel"></param>
    /// <returns></returns>
		private AcNamedValues GatherColumnValues( AcTable InPanel )
		{
      AcNamedValues ht = new AcNamedValues( ) ;
			foreach( AcTableRow row in InPanel.Controls )
			{
				if ( row.Controls.Count == 2 )
				{
					AcTableCell cell00 = (AcTableCell)row.Controls[0] ;
					AcTableCell cell01 = (AcTableCell)row.Controls[1] ;
					if ( cell01.Controls.Count == 1 )
					{
            string columnValue = null ;
						string columnName = cell00.Text ;
            Control entryControl = cell01.Controls[0] ;
            if (entryControl is TextBox)
            {
              TextBox tb = (TextBox)entryControl;
              columnValue = tb.Text;
            }
            else
            {
              DropDownList ddl = (DropDownList)entryControl;
              columnValue = ddl.SelectedValue;
            }
						ht.Add( columnName, columnValue ) ;
					}
				}
			}
			return ht ;
		}

		// create the property panel as a table. The panel contains each column
		// being prompted.
    private void CreatePropertyPanel(AcTable InPanel)
    {
      AcTableRow row;
      AcTableCell cell;
      string columnValue;
      DataRow rowrow = null;
      DataTable rowtbl = null;

      // create the property panel as a table.
      InPanel.AddStyle("background-color", "lightgrey")
        .AddStyle("font-family", "Verdana, Arial")
        .AddStyle("border-color", "white");

      // first row holds the property panel title
      InPanel.AddNewRow().AddNewCell()
        .AddAttribute("ColSpan", "2")
        .AddStyle("text-align", "center")
        .SetText(Title);

      // build a dataset holding the single row to be prompted.
      if ((ActionCode != ActionCode.Add) && ( RowSelect != null ))
      {
        DataSet rowds = TableRowToDataSet();
        rowtbl = rowds.Tables[0];
        rowrow = rowtbl.Rows[0];
      }

      foreach (ColumnPrompt column in Columns)
      {

        // add a row to the panel grid for each column in the table row being prompted.
        //			foreach ( DataColumn column in rowtbl.Columns )
        //			{
        string columnName = column.ColumnName;
        columnValue = null;

        // AcColumnInfo of the column.
        AcColumnInfo info = null;
        if ((ColumnInfo != null) && ( ColumnInfo.ContainsKey( columnName )))
        {
          info = ColumnInfo[columnName];
        }

        // get ColumnPrompt of this column.
        // ( either default or the entry from mColumns collection )
        //				if ( Columns.Count == 0 )
        //					cp = new ColumnPrompt( ColumnName ) ;
        //				else
        //					cp = FindColumnPrompt( ColumnName ) ; 
        //				if ( cp == null )
        //					continue ;

        // add an AcTableRow to the panel AcTable
        row = InPanel.AddNewRow();

        // column prompt text
        string headingText = columnName ;
        if ((info != null) && (info.HeadingText != null))
          headingText = info.HeadingText;

        // first column is the column prompt text
        row.AddNewCell()
          .AddStyle("padding-right", "2%")
          .SetText(headingText);

        // second column is the column value.
        // Get the value from either the sql selected row ( see RowSelect property )
        // or the InputColumnValues ColumnName/ColumnValue dictionary.
        cell = row.AddNewCell();
        if (rowrow != null)
          columnValue = rowrow[columnName].ToString();
        else if (mInputColumnValues != null)
          columnValue = mInputColumnValues[columnName];

        if ((info != null) && (info.AllowedValues != null))
        {
          DropDownList ddl = new DropDownList();
          foreach (string alwvlu in info.AllowedValues)
          {
            ddl.Items.Add(alwvlu);
          }
          cell.Controls.Add(ddl);
        }
        else if (AlwChg == true)
        {
          TextBox tb = new TextBox();
          tb.Text = columnValue;
          cell.Controls.Add(tb);
        }
        else
        {
          cell.AddStyle("padding-right", "1%")
            .SetText(columnValue);
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
    private DataSet TableRowToDataSet()
    {
      DataSet rowds = null;
      SqlConnection conn = null;
      SqlDataAdapter da = null;
      SqlCommand cmd = null;
      try
      {
        conn = new SqlConnection(ConnString);
        conn.Open();

        cmd = new SqlCommand();
        cmd.Connection = conn;
        cmd.CommandType = CommandType.Text;
        if (RowSelect == null)
          cmd.CommandText =
            "SELECT * from " + PromptTable;
        else
          cmd.CommandText =
            "SELECT * from " + PromptTable + " WHERE " + RowSelect;

        da = new SqlDataAdapter();
        da.SelectCommand = cmd;

        rowds = new DataSet();
        da.Fill(rowds, "RowPrompt");
      }
      finally
      {
        if (conn != null)
          conn.Close();
      }
      return rowds;
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
