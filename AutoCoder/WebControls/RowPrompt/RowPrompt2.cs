using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using AutoCoder.Core;

namespace AutoCoder.WebControls.RowPrompt
{

  public class RowPrompt2 : System.Web.UI.WebControls.WebControl, INamingContainer
//  public class RowPrompt2 : System.Web.UI.WebControls.CompositeControl, INamingContainer
  {
    protected Button mOkButton = null;
    protected Button mCancelButton = null ;
    protected Panel mPromptPanel = null ;
    AcColumnInfoDictionary mColumns = null;
    event RowPromptResultsEvent mRowPromptResultsEvent = null;
    AcNamedValues mInputColumnValues = null;
    RowPromptStyles mPromptStyles = null;
    string mFocusColumn = null ;

    public RowPrompt2()
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
          return (ActionCode)oAction;
      }
      set { ViewState["ActionCode"] = value; }
    }

    /// <summary>
    /// Allow changes to the prompted row. true or false.
    /// </summary>
    public bool AlwChg
    {
      get
      {
        if (ViewState["AlwChg"] == null)
          return false;
        else
          return bool.Parse(ViewState["AlwChg"].ToString());
      }
      set { ViewState["AlwChg"] = value; }
    }

    public AcColumnInfoDictionary Columns
    {
      get
      {
        if (mColumns == null)
        {
          mColumns = (AcColumnInfoDictionary)ViewState["Columns"];
          if ( mColumns == null )
          {
            Columns = new AcColumnInfoDictionary( ) ;
          }
        }
        return mColumns;
      }
      set
      {
        mColumns = value;
        ViewState["Columns"] = value;
      }
    }

    /// <summary>
    /// Connection string used to select the row from the database table, and
    /// then update the row in the table with the row prompt entries.
    /// </summary>
    public string ConnString
    {
      get { return GetViewStateValue("ConnString"); }
      set { ViewState["ConnString"] = value; }
    }

    /// <summary>
    /// the name of the column to set focus at.
    /// </summary>
    public string FocusColumn
    {
      get { return mFocusColumn; }
      set { mFocusColumn = value; }
    }

    /// <summary>
    /// initial load of the column values into the prompt controls is done.
    /// </summary>
    private bool InitialLoadDone
    {
      get { return AcCommon.BoolValue(ViewState["InitialLoadDone"]); }
      set { ViewState["InitialLoadDone"] = value; }
    }

    public AcNamedValues InputColumnValues
    {
      get
      {
        if (mInputColumnValues == null)
          mInputColumnValues = new AcNamedValues();
        return mInputColumnValues;
      }
      set { mInputColumnValues = value; }
    }

    /// <summary>
    /// the prompted value input results  
    /// </summary>
    public AcNamedValues OutputColumnValues
    {
      get { return GatherColumnValues(mPromptPanel); }
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

    public RowPromptStyles PromptStyles
    {
      get
      {
        if ( mPromptStyles == null )
          mPromptStyles = (RowPromptStyles)ViewState["PromptStyles"];
        if (mPromptStyles == null)
          mPromptStyles = new RowPromptStyles();
        return mPromptStyles;
      }
      set
      {
        mPromptStyles = value;
      }
    }

    /// <summary>
    /// the database table of the row being prompted
    /// </summary>
    public string PromptTable
    {
      get { return GetViewStateValue("PromptTable"); }
      set { ViewState["PromptTable"] = value; }
    }

    public RowPromptResultsEvent RowPromptResultsEvent
    {
      get { return mRowPromptResultsEvent; }
      set { mRowPromptResultsEvent = value; }
    }

    /// <summary>
    /// the "where" clause of the sql selection statement.
    /// </summary>
    public string RowSelect
    {
      get { return GetViewStateValue("RowSelect"); }
      set { ViewState["RowSelect"] = value; }
    }

    /// <summary>
    /// Title text of the prompt panel
    /// </summary>
    public string Title
    {
      get { return GetViewStateValue("Title"); }
      set { ViewState["Title"] = value; }
    }

    public override bool Visible
    {
      get
      {
        return base.Visible;
      }
      set
      {
        base.Visible = value;
        if (value == false)
        {
          InitialLoadDone = false;
          Columns = null;
        }
      }
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

    protected override void CreateChildControls()
    {
      Controls.Clear();

      mOkButton = new Button( ) ;
      mOkButton.ID = "OkButton1";
      mOkButton.Text = " OK ";
      mOkButton.Click += new EventHandler(this.OkButton_Click);

      mCancelButton = new Button( ) ;
      mCancelButton.ID = "CancelButton1";
      mCancelButton.Text = " Cancel ";
      mCancelButton.Click += new EventHandler(this.CancelButton_Click);

      mPromptPanel = new Panel( ) ;
      Controls.Add(mPromptPanel);
      mPromptPanel.CssClass = PromptStyles.PanelClass;

      // the prompt title.
      if (Title != null)
      {
        CreateChildControls_Title(mPromptPanel);
      }

      CreateChildControls_Columns(mPromptPanel);

      // ok and cancel buttons at the bottom of the entry form.
      mPromptPanel.Controls.Add(new LiteralControl("<br/>"));
      mPromptPanel.Controls.Add(mOkButton);
      mPromptPanel.Controls.Add(mCancelButton);
    }

    // create the property panel as a table. The panel contains each column
    // being prompted.
    private void CreateChildControls_Columns(Panel InPanel)
    {
      Label lbl = null;
      string columnValue;
      DataRow rowrow = null;
      DataTable rowtbl = null;

      // build a dataset holding the single row to be prompted.
      if ((ActionCode != ActionCode.Add) && (RowSelect != null))
      {
        DataSet rowds = TableRowToDataSet();
        rowtbl = rowds.Tables[0];
        rowrow = rowtbl.Rows[0];
      }

      foreach (KeyValuePair<string, AcColumnInfo> pair in Columns)
      {
        AcColumnInfo column = pair.Value;
        columnValue = null;

        // column prompt text
        string headingText = column.ColumnName;
        if (column.HeadingText != null)
          headingText = column.HeadingText;

        // the column prompt label.
        lbl = new Label();
        InPanel.Controls.Add(lbl);
        lbl.Text = headingText;
        lbl.CssClass = PromptStyles.HeadingClass;

        // Get the value from either the sql selected row ( see RowSelect property )
        // or the InputColumnValues ColumnName/ColumnValue dictionary.
        if (InitialLoadDone == true) 
          columnValue = null;
        else
        {
          if (rowrow != null)
            columnValue = rowrow[column.ColumnName].ToString();
          else if 
            ((mInputColumnValues != null) &&
            (mInputColumnValues.ContainsKey(column.ColumnName) == true))
          {
            columnValue = mInputColumnValues[column.ColumnName];
          }
        }

        // column has multiple allowed values. use DropDownList.
        if (column.HasAllowedValues == true)
        {
          DropDownList ddl = new DropDownList();
          foreach (string alwvlu in column.AllowedValues)
          {
            ddl.Items.Add(alwvlu);
          }
          ddl.CssClass = PromptStyles.FieldClass;
          ddl.ID = column.ColumnName;
          if ((mFocusColumn != null) && (mFocusColumn == column.ColumnName))
            ddl.Focus();
          InPanel.Controls.Add(ddl);
        }

          // prompt the value in a TextBox.
        else
        {
          TextBox tb = new TextBox();
          InPanel.Controls.Add(tb);
          tb.ID = column.ColumnName;
          tb.CssClass = PromptStyles.FieldClass;
          if (AlwChg == false)
            tb.ReadOnly = true;

          if (( mFocusColumn != null ) && ( mFocusColumn == column.ColumnName ))
            tb.Focus();  

          // apply the initial value
          if ( InitialLoadDone == false )
            tb.Text = columnValue;
        }
      }

      // apply initial value to the controls one time only.
      InitialLoadDone = true;
    }

    private void CreateChildControls_Title( Panel InPromptPanel )
    {
      Label lbl = new Label();
      InPromptPanel.Controls.Add(lbl);
      lbl.Text = Title;
      lbl.CssClass = PromptStyles.TitleClass;

      // double space.
      InPromptPanel.Controls.Add( new LiteralControl( "<br/>" )) ;
      InPromptPanel.Controls.Add(new LiteralControl("<br/>"));
    }

    /// <summary>
    /// process the input to the prompted columns.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void OkButton_Click(object sender, EventArgs e)
    {
      if (RowPromptResultsEvent != null)
      {
        RowPromptResults results = new RowPromptResults();
        results.Action = ActionCode;
        results.ColumnValues = OutputColumnValues;
        results.PrimaryKeys = PrimaryKeys;
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

    protected override void OnPreRender(EventArgs e)
    {
      ViewState["PromptStyles"] = mPromptStyles;
      base.OnPreRender(e);
    }

//    protected override void RecreateChildControls()
//    {
//      EnsureChildControls();
//    }

    protected override void Render(HtmlTextWriter InWriter)
    {
      base.Render(InWriter);
    }

    /// <summary>
    /// Gather the values of the prompted columns.
    /// </summary>
    /// <param name="InPanel"></param>
    /// <returns></returns>
    private AcNamedValues GatherColumnValues(Panel InPanel)
    {
      AcNamedValues ht = new AcNamedValues();
      string columnValue = null;
      string columnName = null;

      foreach (Control cntrl in InPanel.Controls)
      {
        if (cntrl is TextBox)
        {
          TextBox tb = (TextBox)cntrl;
          columnName = tb.ID;
          columnValue = tb.Text;
          ht.Add(columnName, columnValue);
        }
        else if (cntrl is DropDownList)
        {
          DropDownList ddl = (DropDownList)cntrl;
          columnName = ddl.ID;
          columnValue = ddl.SelectedValue;
          ht.Add(columnName, columnValue);
        }
      }
      return ht;
    }

    // ------------------------ GetViewStateValue ---------------------
    // get string value from state bag. if not found, return null.
    private string GetViewStateValue(string InKey)
    {
      object vs = ViewState[InKey];
      if (vs == null)
        return null;
      else
        return vs.ToString();
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
    private string debug()
    {
      string Text;
      Text =
        "SELECT * from " + PromptTable + " WHERE " + RowSelect;
      return Text;
    }

  } // end class RowPrompt2


}
